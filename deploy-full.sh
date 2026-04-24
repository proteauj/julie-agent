#!/usr/bin/env bash
set -e

echo "🧹 Nettoyage anciens builds..."
rm -rf frontend/dist
rm -rf backend/Memora.Api/wwwroot

echo "🏗 Build Angular..."
cd frontend
npm run build
cd ..

echo "📦 Détection du dossier dist..."
DIST_DIR=$(find frontend/dist -mindepth 1 -maxdepth 1 -type d | head -n 1)

if [ -z "$DIST_DIR" ]; then
  echo "❌ Aucun dossier trouvé dans frontend/dist"
  exit 1
fi

echo "✅ Dossier Angular trouvé: $DIST_DIR"

echo "📁 Copie vers backend/Memora.Api/wwwroot..."
mkdir -p backend/Memora.Api/wwwroot
cp -R "$DIST_DIR"/* backend/Memora.Api/wwwroot/

echo "🔍 Vérification index.html..."
if [ ! -f backend/Memora.Api/wwwroot/index.html ]; then
  echo "❌ index.html absent dans wwwroot"
  exit 1
fi

echo "🔍 Vérification localhost..."
if grep -R "localhost:5000" backend/Memora.Api/wwwroot; then
  echo "❌ Le build contient encore localhost:5000"
  exit 1
fi

echo "🔍 Vérification vieux loader translate..."
if grep -R "TRANSLATE_HTTP_LOADER_CONFIG" backend/Memora.Api/wwwroot; then
  echo "❌ Le build contient encore TRANSLATE_HTTP_LOADER_CONFIG"
  exit 1
fi

echo "✅ Build frontend prêt dans wwwroot"

echo "🧪 Build backend..."
cd backend/Memora.Api
dotnet build
cd ../..

echo "📌 Git status:"
git status

echo ""
echo "✅ Terminé."
echo "Prochaine étape si tout est OK:"
echo "git add ."
echo "git commit -m \"Build frontend for deploy\""
echo "git push origin main"
echo "git push heroku main -f"
