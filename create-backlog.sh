#!/usr/bin/env bash
set -euo pipefail

# Aline Écoute - création complète du backlog GitHub Issues.
# Usage:
#   gh auth login
#   cd racine-du-repo
#   chmod +x create-backlog.sh
#   DRY_RUN=true ./create-backlog.sh   # tester sans créer
#   ./create-backlog.sh                # créer les issues
#
# Optionnel:
#   PROJECT_NUMBER=1 ./create-backlog.sh
#
# Le script:
# - crée les labels si absents;
# - crée 100+ issues;
# - ajoute phase, priorité, type et statut via labels;
# - remplit chaque issue avec: En tant que, règles d'affaires,
#   critères, cas limites, cas d'erreur et sketch d'écran.

DRY_RUN="${DRY_RUN:-false}"
PROJECT_NUMBER="${PROJECT_NUMBER:-}"

if ! command -v gh >/dev/null 2>&1; then
  echo "Erreur: GitHub CLI 'gh' n'est pas installé."
  exit 1
fi

if ! gh auth status >/dev/null 2>&1; then
  echo "Erreur: lance d'abord: gh auth login"
  exit 1
fi

OWNER="$(gh repo view --json owner -q .owner.login)"
REPO="$(gh repo view --json name -q .name)"

echo "Repo: $OWNER/$REPO"
echo "DRY_RUN=$DRY_RUN"

ensure_label() {
  local name="$1"
  local color="$2"
  local description="$3"

  if gh label list --limit 300 | cut -f1 | grep -Fxq "$name"; then
    echo "Label existe: $name"
  else
    echo "Création label: $name"
    if [ "$DRY_RUN" = "false" ]; then
      gh label create "$name" --color "$color" --description "$description" >/dev/null
    fi
  fi
}

ensure_label "story" "7057ff" "User story produit"
ensure_label "feature" "0e8a16" "Fonctionnalité"
ensure_label "tech" "1d76db" "Travail technique"
ensure_label "bug" "d73a4a" "Bug"
ensure_label "priority:high" "b60205" "Priorité haute"
ensure_label "priority:medium" "fbca04" "Priorité moyenne"
ensure_label "priority:low" "cfd3d7" "Priorité basse"
ensure_label "status:backlog" "cfd3d7" "Backlog"
ensure_label "status:en-cours" "fbca04" "En cours"
ensure_label "status:done" "0e8a16" "Terminé"

for phase in A1 A2 A3 A4.1 A4.2 A4.3 A4.4 A4.5 A5 A6; do
  ensure_label "phase:$phase" "5319e7" "Phase $phase"
done

issue_exists() {
  local title="$1"
  gh issue list --state all --limit 500 --search "$title in:title" --json title -q '.[].title' | grep -Fxq "$title"
}

make_body() {
  local title="$1"
  local phase="$2"
  local type="$3"
  local priority="$4"
  local status="$5"
  local persona="$6"
  local want="$7"
  local benefit="$8"

  cat <<EOF
## 🎯 User Story

En tant $persona,  
je veux $want,  
afin de $benefit.

## ✅ Critères d'acceptation

- [ ] La fonctionnalité couvre le besoin principal: **$title**.
- [ ] L'interface est simple, lisible et utilisable par un sénior.
- [ ] Les textes visibles sont traduits en français et en anglais lorsque pertinent.
- [ ] Les erreurs sont affichées clairement, sans langage technique.
- [ ] La fonctionnalité fonctionne en local et après déploiement.

## 📋 Règles d'affaires

- La solution doit respecter l'objectif de simplicité d'Aline Écoute.
- Les actions sensibles doivent demander confirmation ou être explicitement autorisées.
- Les données personnelles doivent rester associées au bon utilisateur.
- Les fonctionnalités non prêtes ne doivent pas être visibles dans le menu principal sénior.
- Les choix proposés à l'aîné doivent rester limités et compréhensibles.

## ⚠️ Cas limites

- Utilisateur non connecté ou session expirée.
- Données manquantes ou incomplètes.
- Traduction manquante.
- Route ou service non disponible.
- Utilisateur avec faible aisance numérique.

## ❌ Cas d'erreur

- API inaccessible.
- Erreur de validation.
- Erreur de base de données.
- Action impossible ou non autorisée.
- Message d'erreur technique exposé à l'utilisateur.

## 🖼 Impression d'écran / sketch d'écran

À ajouter dans l'issue avec glisser-déposer.

Sketch attendu:
- Titre clair de l'écran.
- Une action principale très visible.
- Gros boutons avec icône + texte.
- Bouton Accueil et Retour si applicable.
- Aucun écran surchargé.

## 🔧 Notes techniques

- Phase: **$phase**
- Type: **$type**
- Priorité: **$priority**
- Statut initial: **$status**
- Prévoir tests local + build avant fermeture.
EOF
}

create_issue() {
  local title="$1"
  local phase="$2"
  local type="$3"
  local priority="$4"
  local status="$5"
  local persona="$6"
  local want="$7"
  local benefit="$8"

  local status_label="status:backlog"
  if [ "$status" = "Done" ]; then
    status_label="status:done"
  elif [ "$status" = "En cours" ]; then
    status_label="status:en-cours"
  fi

  local labels="story,$type,priority:$priority,phase:$phase,$status_label"
  local body_file
  body_file="$(mktemp)"

  make_body "$title" "$phase" "$type" "$priority" "$status" "$persona" "$want" "$benefit" > "$body_file"

  if issue_exists "$title"; then
    echo "Issue existe déjà, ignorée: $title"
    rm "$body_file"
    return
  fi

  echo "Création: [$phase][$priority] $title"
  if [ "$DRY_RUN" = "false" ]; then
    issue_url="$(gh issue create --title "$title" --body-file "$body_file" --label "$labels")"
    echo "  -> $issue_url"

    if [ -n "$PROJECT_NUMBER" ]; then
      gh project item-add "$PROJECT_NUMBER" --owner "$OWNER" --url "$issue_url" >/dev/null || true
    fi
  else
    echo "  labels=$labels"
  fi

  rm "$body_file"
}

while IFS='|' read -r title phase type priority status persona want benefit; do
  [ -z "$title" ] && continue
  create_issue "$title" "$phase" "$type" "$priority" "$status" "$persona" "$want" "$benefit"
done <<'BACKLOG'
Setup projet backend|A1|tech|high|Done|que développeuse|setup projet backend|améliorer l'expérience Aline Écoute
Setup base de données|A1|tech|high|Done|que développeuse|setup base de données|améliorer l'expérience Aline Écoute
Création modèle User|A1|tech|high|Done|que développeuse|création modèle User|améliorer l'expérience Aline Écoute
Authentification JWT|A1|feature|high|Done|que sénior|authentification JWT|améliorer l'expérience Aline Écoute
Login utilisateur|A1|feature|high|Done|que sénior|login utilisateur|améliorer l'expérience Aline Écoute
Register utilisateur|A1|feature|high|Done|que sénior|register utilisateur|améliorer l'expérience Aline Écoute
Structure API|A1|tech|high|Done|que développeuse|structure API|améliorer l'expérience Aline Écoute
CRUD rappels|A2|feature|high|Done|que sénior|cRUD rappels|ne pas oublier les choses importantes
CRUD rendez-vous|A2|feature|high|Done|que sénior|cRUD rendez-vous|organiser mon horaire simplement
Service Reminder|A2|tech|high|Done|que développeuse|service Reminder|améliorer l'expérience Aline Écoute
Service Appointment|A2|tech|high|Done|que développeuse|service Appointment|améliorer l'expérience Aline Écoute
Stockage en base|A2|tech|high|Done|que développeuse|stockage en base|améliorer l'expérience Aline Écoute
UI profil sénior|A3|feature|high|Done|que sénior|uI profil sénior|utiliser l'application facilement sans me perdre
UI agenda|A3|feature|high|Done|que sénior|uI agenda|utiliser l'application facilement sans me perdre
UI rappels|A3|feature|high|Done|que sénior|uI rappels|utiliser l'application facilement sans me perdre
UI activités|A3|feature|high|Done|que sénior|uI activités|utiliser l'application facilement sans me perdre
Admin création comptes|A3|feature|high|Done|qu'administrateur de résidence|admin création comptes|améliorer l'expérience Aline Écoute
Admin gestion activités|A3|feature|high|Done|qu'administrateur de résidence|admin gestion activités|améliorer l'expérience Aline Écoute
Admin gestion médecins|A3|feature|high|Done|qu'administrateur de résidence|admin gestion médecins|améliorer l'expérience Aline Écoute
Connexion frontend|A3|feature|high|Done|que sénior|connexion frontend|améliorer l'expérience Aline Écoute
Gestion traduction|A3|feature|medium|Done|que sénior|gestion traduction|améliorer l'expérience Aline Écoute
Styles globaux|A3|tech|medium|Done|que développeuse|styles globaux|améliorer l'expérience Aline Écoute
Design UI sénior simplifié|A4.1|feature|high|En cours|que sénior|design UI sénior simplifié|utiliser l'application facilement sans me perdre
Menu principal limité à 6 options|A4.1|feature|high|En cours|que sénior|menu principal limité à 6 options|utiliser l'application facilement sans me perdre
Navigation bouton accueil|A4.1|feature|high|Backlog|que sénior|navigation bouton accueil|utiliser l'application facilement sans me perdre
Navigation bouton retour|A4.1|feature|high|Backlog|que sénior|navigation bouton retour|utiliser l'application facilement sans me perdre
Icônes dans toute l’app|A4.1|feature|high|Backlog|que sénior|icônes dans toute l’app|améliorer l'expérience Aline Écoute
Zones cliquables larges|A4.1|feature|high|Backlog|que sénior|zones cliquables larges|améliorer l'expérience Aline Écoute
Police large accessible|A4.1|feature|high|Backlog|que sénior|police large accessible|améliorer l'expérience Aline Écoute
Flux simple 1 action 1 écran|A4.1|feature|high|Backlog|que sénior|flux simple 1 action 1 écran|améliorer l'expérience Aline Écoute
Uniformisation styles globaux|A4.1|tech|medium|Backlog|que développeuse|uniformisation styles globaux|améliorer l'expérience Aline Écoute
Layout page/card cohérent|A4.1|tech|medium|Backlog|que développeuse|layout page/card cohérent|améliorer l'expérience Aline Écoute
Accessibilité inputs/forms|A4.1|feature|high|Backlog|que sénior|accessibilité inputs/forms|améliorer l'expérience Aline Écoute
Responsive tablette/sénior|A4.1|feature|medium|Backlog|que sénior|responsive tablette/sénior|améliorer l'expérience Aline Écoute
Chat intelligent OpenAI|A4.2|feature|high|En cours|que sénior|chat intelligent OpenAI|être accompagné par Aline de façon naturelle
Mémoire conversation|A4.2|feature|high|En cours|que sénior|mémoire conversation|préserver et transmettre mon histoire
Historique messages|A4.2|tech|high|En cours|que développeuse|historique messages|améliorer l'expérience Aline Écoute
Prompt système Aline|A4.2|feature|high|En cours|que sénior|prompt système Aline|améliorer l'expérience Aline Écoute
Gestion contexte conversation|A4.2|tech|medium|Backlog|que développeuse|gestion contexte conversation|améliorer l'expérience Aline Écoute
Amélioration UX chat|A4.2|feature|medium|Backlog|que sénior|amélioration UX chat|améliorer l'expérience Aline Écoute
Indicateur typing|A4.2|feature|low|Backlog|que sénior|indicateur typing|améliorer l'expérience Aline Écoute
Scroll automatique messages|A4.2|tech|low|Backlog|que développeuse|scroll automatique messages|améliorer l'expérience Aline Écoute
Gestion erreurs API chat|A4.2|tech|medium|Backlog|que développeuse|gestion erreurs API chat|améliorer l'expérience Aline Écoute
Retry message|A4.2|feature|low|Backlog|que sénior|retry message|améliorer l'expérience Aline Écoute
Parsing intention utilisateur|A4.3|tech|high|Backlog|que développeuse|parsing intention utilisateur|améliorer l'expérience Aline Écoute
Créer rappel via chat|A4.3|feature|high|Backlog|que sénior|créer rappel via chat|ne pas oublier les choses importantes
Créer rendez-vous via chat|A4.3|feature|high|Backlog|que sénior|créer rendez-vous via chat|organiser mon horaire simplement
Confirmation action utilisateur|A4.3|feature|high|Backlog|que sénior|confirmation action utilisateur|améliorer l'expérience Aline Écoute
Consultation agenda via chat|A4.3|feature|medium|Backlog|que sénior|consultation agenda via chat|organiser mon horaire simplement
Inscription activité via chat|A4.3|feature|medium|Backlog|que sénior|inscription activité via chat|améliorer l'expérience Aline Écoute
Logs actions agent|A4.3|tech|medium|Backlog|que développeuse|logs actions agent|améliorer l'expérience Aline Écoute
Détection mots-clés médicaux|A4.3|feature|high|Backlog|que sénior|détection mots-clés médicaux|améliorer l'expérience Aline Écoute
Routage modèle médical|A4.3|feature|high|Backlog|que sénior|routage modèle médical|améliorer l'expérience Aline Écoute
Réponse sécurisée sans diagnostic|A4.3|feature|high|Backlog|que sénior|réponse sécurisée sans diagnostic|améliorer l'expérience Aline Écoute
Détection risque urgence|A4.3|feature|high|Backlog|que sénior|détection risque urgence|améliorer l'expérience Aline Écoute
Suggestion appel 911|A4.3|feature|high|Backlog|que sénior|suggestion appel 911|améliorer l'expérience Aline Écoute
Gestion contacts|A4.4|feature|high|Backlog|que sénior|gestion contacts|garder le lien avec mes proches
Ajouter contact|A4.4|feature|high|Backlog|que sénior|ajouter contact|garder le lien avec mes proches
Modifier contact|A4.4|feature|medium|Backlog|que sénior|modifier contact|garder le lien avec mes proches
Supprimer contact|A4.4|feature|medium|Backlog|que sénior|supprimer contact|garder le lien avec mes proches
Afficher contacts|A4.4|feature|high|Backlog|que sénior|afficher contacts|garder le lien avec mes proches
Contact proche accès complet|A4.4|feature|high|Backlog|que proche ou sénior|contact proche accès complet|garder le lien avec mes proches
Liste contacts simplifiée|A4.4|feature|medium|Backlog|que sénior|liste contacts simplifiée|garder le lien avec mes proches
Rappel anniversaires contacts|A4.4|feature|high|Backlog|que sénior|rappel anniversaires contacts|ne pas oublier les choses importantes
Notification anniversaire|A4.4|feature|high|Backlog|que sénior|notification anniversaire|améliorer l'expérience Aline Écoute
Menu résidence|A4.4|feature|medium|Backlog|qu'administrateur de résidence|menu résidence|utiliser l'application facilement sans me perdre
Afficher menu quotidien|A4.4|feature|medium|Backlog|que sénior|afficher menu quotidien|améliorer l'expérience Aline Écoute
Téléversement photos souvenirs|A4.4|feature|medium|Backlog|que sénior|téléversement photos souvenirs|améliorer l'expérience Aline Écoute
Galerie photos souvenirs|A4.4|feature|medium|Backlog|que sénior|galerie photos souvenirs|améliorer l'expérience Aline Écoute
Lecture audio text-to-speech|A4.5|feature|high|Backlog|que sénior|lecture audio text-to-speech|améliorer l'expérience Aline Écoute
Interaction vocale|A4.5|feature|high|Backlog|que sénior|interaction vocale|améliorer l'expérience Aline Écoute
Commandes vocales simples|A4.5|feature|medium|Backlog|que sénior|commandes vocales simples|améliorer l'expérience Aline Écoute
Musique années 50-70|A4.5|feature|high|Backlog|que sénior|musique années 50-70|me divertir avec du contenu adapté à mon époque
Suggestions musique vintage|A4.5|feature|medium|Backlog|que sénior|suggestions musique vintage|me divertir avec du contenu adapté à mon époque
Lecture histoires vintage|A4.5|feature|medium|Backlog|que sénior|lecture histoires vintage|me divertir avec du contenu adapté à mon époque
Lecture poèmes vintage|A4.5|feature|medium|Backlog|que sénior|lecture poèmes vintage|me divertir avec du contenu adapté à mon époque
Recherche contenu populaire années 50-70|A4.5|tech|medium|Backlog|que développeuse|recherche contenu populaire années 50-70|améliorer l'expérience Aline Écoute
Mode lecture simplifié|A4.5|feature|medium|Backlog|que sénior|mode lecture simplifié|améliorer l'expérience Aline Écoute
Écrire ses mémoires|A5|feature|high|Backlog|que sénior|écrire ses mémoires|préserver et transmettre mon histoire
Éditeur mémoires simple|A5|feature|high|Backlog|que sénior|éditeur mémoires simple|préserver et transmettre mon histoire
Sauvegarde automatique mémoires|A5|tech|high|Backlog|que développeuse|sauvegarde automatique mémoires|préserver et transmettre mon histoire
Partage mémoires famille|A5|feature|high|Backlog|que proche ou sénior|partage mémoires famille|préserver et transmettre mon histoire
Amis et famille peuvent voir les mémoires|A5|feature|high|Backlog|que proche ou sénior|amis et famille peuvent voir les mémoires|préserver et transmettre mon histoire
Association proche-sénior|A5|feature|high|Backlog|que proche ou sénior|association proche-sénior|améliorer l'expérience Aline Écoute
Accès proche au compte|A5|feature|high|Backlog|que proche ou sénior|accès proche au compte|améliorer l'expérience Aline Écoute
Gestion autorisations proches|A5|tech|high|Backlog|que proche ou sénior|gestion autorisations proches|améliorer l'expérience Aline Écoute
Préparer héritage|A5|feature|medium|Backlog|que proche ou sénior|préparer héritage|améliorer l'expérience Aline Écoute
Préparer funérailles|A5|feature|medium|Backlog|que proche ou sénior|préparer funérailles|améliorer l'expérience Aline Écoute
Arbre généalogique|A5|feature|medium|Backlog|que sénior|arbre généalogique|améliorer l'expérience Aline Écoute
Créer membre famille|A5|feature|medium|Backlog|que proche ou sénior|créer membre famille|améliorer l'expérience Aline Écoute
Groupe de discussion|A5|feature|medium|Backlog|que sénior|groupe de discussion|améliorer l'expérience Aline Écoute
Chat entre résidents|A5|feature|medium|Backlog|que sénior|chat entre résidents|être accompagné par Aline de façon naturelle
Contacts inter-résidences|A5|feature|low|Backlog|qu'administrateur de résidence|contacts inter-résidences|garder le lien avec mes proches
Mini réseau social|A5|feature|low|Backlog|que sénior|mini réseau social|améliorer l'expérience Aline Écoute
Mur messages|A5|feature|low|Backlog|que sénior|mur messages|améliorer l'expérience Aline Écoute
Jeux cartes|A5|feature|medium|Backlog|que sénior|jeux cartes|me divertir avec du contenu adapté à mon époque
Jeu patience|A5|feature|medium|Backlog|que sénior|jeu patience|améliorer l'expérience Aline Écoute
Casse-têtes|A5|feature|low|Backlog|que sénior|casse-têtes|améliorer l'expérience Aline Écoute
Marketplace|A5|feature|low|Backlog|que sénior|marketplace|améliorer l'expérience Aline Écoute
Patrons de tricot|A5|feature|medium|Backlog|que sénior|patrons de tricot|améliorer l'expérience Aline Écoute
Vidéos tricot YouTube Aline Écoute|A5|feature|medium|Backlog|que sénior|vidéos tricot YouTube Aline Écoute|améliorer l'expérience Aline Écoute
Intégration YouTube|A5|tech|medium|Backlog|que développeuse|intégration YouTube|améliorer l'expérience Aline Écoute
Paiement résidence|A6|feature|low|Backlog|qu'administrateur de résidence|paiement résidence|améliorer l'expérience Aline Écoute
Mode de paiement|A6|feature|low|Backlog|que sénior|mode de paiement|améliorer l'expérience Aline Écoute
Historique paiements|A6|feature|low|Backlog|que sénior|historique paiements|améliorer l'expérience Aline Écoute
Analyse PDF financier|A6|feature|low|Backlog|que sénior|analyse PDF financier|améliorer l'expérience Aline Écoute
Extraction données PDF|A6|tech|low|Backlog|que développeuse|extraction données PDF|améliorer l'expérience Aline Écoute
Explication placements financiers|A6|feature|low|Backlog|que sénior|explication placements financiers|améliorer l'expérience Aline Écoute
Logs centralisés|A6|tech|medium|Backlog|que développeuse|logs centralisés|améliorer l'expérience Aline Écoute
Monitoring backend|A6|tech|medium|Backlog|que développeuse|monitoring backend|améliorer l'expérience Aline Écoute
Audit sécurité|A6|tech|medium|Backlog|que développeuse|audit sécurité|améliorer l'expérience Aline Écoute
Gestion erreurs globales|A6|tech|medium|Backlog|que développeuse|gestion erreurs globales|améliorer l'expérience Aline Écoute
Déploiement domaine alineecoute.com|A6|tech|high|Backlog|que développeuse|déploiement domaine alineecoute.com|améliorer l'expérience Aline Écoute
Déploiement domaine alienecoute.ca|A6|tech|high|Backlog|que développeuse|déploiement domaine alienecoute.ca|améliorer l'expérience Aline Écoute
Gestion configuration prod|A6|tech|medium|Backlog|que développeuse|gestion configuration prod|améliorer l'expérience Aline Écoute
Backup base de données|A6|tech|medium|Backlog|que développeuse|backup base de données|améliorer l'expérience Aline Écoute
BACKLOG

echo "Backlog terminé."
