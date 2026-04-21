# Julie-Agent Backend (.NET)

Ce dossier contient l’API backend du projet Julie-Agent sous .NET, connectée à une base PostgreSQL (Neon).

## Lancement local

1. Copier `.env.example` → `.env` et renseigner la variable `DATABASE_URL` (communique avec la base Neon : postgresql://...).

2. Appliquer le schéma SQL (`schema.sql`) sur votre instance Neon/Postgres.

3. Depuis ce dossier, lancer :
   ```
   dotnet restore
   dotnet build
   dotnet run --project Memora.Api/Memora.Api.csproj
   ```
   L'API démarre sur http://localhost:5000 (ou selon config /env).

### Organisation
- `Memora.Api/` : source Web API (.NET)
- `schema.sql` : init de la table `users`

Voir README à la racine pour doc fullstack et API doc.