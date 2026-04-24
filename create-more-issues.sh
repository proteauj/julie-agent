#!/usr/bin/env bash
set -e

create_issue() {
  local title="$1"
  local phase="$2"
  local priority="$3"
  local body="$4"

  gh issue create \
    --title "$title" \
    --body "$body" \
    --label "story,feature,priority:$priority,phase:$phase,status:backlog"
}

create_issue \
  "Calendrier des activités" \
  "A4.4" \
  "high" \
  "## User Story
En tant que sénior,
je veux voir les activités dans un calendrier,
afin de comprendre facilement ce qui est disponible par jour.

## Critères d’acceptation
- [ ] Les activités sont regroupées par date.
- [ ] Chaque activité affiche le nom, l’heure et les places.
- [ ] Je peux m’inscrire depuis la vue calendrier.

## Cas limites
- Aucune activité.
- Activité complète.
- Activité sans description.

## Sketch
Vue calendrier simple avec cartes d’activités par journée."

create_issue \
  "Rappels liés aux sources" \
  "A4.3" \
  "high" \
  "## User Story
En tant que sénior,
je veux que les rappels soient reliés aux activités, rendez-vous, médicaments ou anniversaires,
afin de comprendre pourquoi un rappel existe.

## Critères d’acceptation
- [ ] Un rappel peut avoir SourceType.
- [ ] Un rappel peut avoir SourceId.
- [ ] L’écran Rappels affiche la source du rappel.

## Cas limites
- Source supprimée.
- Rappel manuel existant.
- Source inconnue.

## Sketch
Liste de rappels avec étiquette: Activité, Rendez-vous, Médicament."

create_issue \
  "Cacher création directe de rappels" \
  "A4.3" \
  "medium" \
  "## User Story
En tant que sénior,
je veux voir mes rappels sans devoir comprendre un formulaire technique,
afin de ne pas être confuse.

## Critères d’acceptation
- [ ] L’écran Rappels devient principalement une liste.
- [ ] Les rappels sont créés depuis chat, agenda, activités ou médicaments.
- [ ] Le formulaire direct peut être caché ou réservé admin/dev.

## Cas limites
- Aucun rappel.
- Rappel créé par chat.
- Rappel créé par activité.

## Sketch
Page Rappels = liste simple + bouton retour accueil."
