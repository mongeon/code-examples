# 06 - Preview Environments

Exemple complet avec le workflow GitHub Actions configuré pour les preview environments sur les PR et les branches `dev` et `staging`.

## Comment ça marche

1. Ouvrez une PR contre `main` : un environnement temporaire est créé automatiquement
2. Poussez sur `dev` ou `staging` : un environnement de branche stable est créé
3. Fermez la PR : l'environnement temporaire est détruit

## URLs

- Production : `https://<hostname>.azurestaticapps.net`
- PR #1 : `https://<hostname>-1.<region>.azurestaticapps.net`
- Branche dev : `https://<hostname>-dev.<region>.azurestaticapps.net`
- Branche staging : `https://<hostname>-staging.<region>.azurestaticapps.net`
