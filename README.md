
# Description du projet

Scuffly est un petit simulateur de vol réalisé dans le cadre de l'apprentissage d'Unity3D pour les cours de moteur de jeu 3D en 3e année de cursus CMI à Strasbourg.

### Objectifs techniques et gameplay
- Pouvoir faire décoller un avion et atterrir sur plusieurs aéroports.
- Physique de vol réaliste et dynamique (influencée par l'état de l'avion).
- Possibilité d'utiliser un armement basique (missiles / canon).
- Terrain pseudo-infini généré procéduralement.
- Permettre la possibilité de jouer à plusieurs.
- Interactions dans le cockpit.

```diff
- /!\ BUG SUR CARTE GRAPHIQUE AMD :
Il semblerait qu'il y a un gros problème d'affichage avec les cartes graphiques AMD
Si vous rencontrez ce problème, nous sommes intéréssés par toute informations
pouvant nous aider à en trouver l'origine (Screenshots / Dump RenderDoc etc...)
```

# Documentation

Réalisé avec Unity 2021 version 2.0f1 *(nécessaire pour avoir certaines fonctions de la pipeline hdrp manquantes dans les précédentes versions d'unity, et les nuages volumétriques)*

**Notice d'utilisation :**

Le fonctionnement d'un avion n'est pas simple, il est donc recommandé, au moins la première fois, de suivre [ce petit guide](Doc/HowToFly.md).

**Documentation développeur**

La documentation technique du projet est disponible [ici](Doc/Doc.md) *(en cours de rédaction)*

## Auteurs

- Pierre EVEN
- Léo GOSSELIN

## Credits

- [Unity3D](https://unity3d.com/)
- [Amplifyimpostor](http://amplify.pt/unity/amplify-impostors/)
