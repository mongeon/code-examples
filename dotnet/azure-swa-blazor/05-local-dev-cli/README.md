# 05 - Local Dev CLI

Développement local avec le CLI `swa`. Inclut le fichier `swa-cli.config.json` et la configuration pour le proxy local.

## Démarrage rapide

```bash
# Option 1 : avec swa-cli.config.json
swa start

# Option 2 : commande complète
swa start http://localhost:5000 --run "dotnet run --project Client.csproj" --api-location Api
```

L'app est disponible sur `http://localhost:4280`.

## Debug avec Visual Studio

Lancez les deux projets en debug dans Visual Studio, puis dans un terminal :

```bash
swa start http://localhost:5000 --api-location http://localhost:7071
```
