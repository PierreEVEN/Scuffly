
# Description du projet

Scuffly est un petit simulateur de vol réalisé dans le cadre de l'apprentissage d'Unity3D pour les cours de moteur de jeu 3D en 3e année de licence CMI à Strasbourg.

### Objectifs techniques et gameplay
- Pouvoir faire décoller un avion et atterrir sur plusieurs aéroports.
- Physique de vol réaliste et dynamique (influencée par l'état de l'avion).
- Possibilité d'utiliser un armement basique (missiles / canon).
- Terrain pseudo-infini généré procéduralement.
- Permettre la possibilité de jouer à plusieurs.
- Interactions dans le cockpit.
>____
># Notes pour l'alpha
>
>Un mode multijoueur est a l'étude, mais demande encore de la mise au point.
>
>En attendant, nous avons ramené Don Quichotte du passé que nous retrouvons sur son nouveau destrier modernisé *(il sera votre instructeur de vol derrière vous dans l'avion)*. Les meuniers 2.0 eux ne l'ont pas oublié, faites donc bien attention à ne pas trop vous en approcher !
>
>## Performances
>Le jeu est assez gourmand, et certains GPU peuvent avoir du mal à le faire tourner.
En attendant un menu pour régler les options, il est possible de le faire via l’éditeur d'Unity.
>
>Dans la scène, trouvez un Objet du nom de "GPULandscape". Celui ci est composé de 2 principaux component : un pour le maillage, et un 2e pour la végétation.
La densité du maillage peut y être réduite. La densité de la végétation se règle pour chaque type de folliage utilisé. Pour cela, sélectionner chaque asset de folliage dans la liste des folliage du component dédié, puis réduire la densité manuellement.
>___

## Informations techniques

Réalisé avec Unity 2021 version 2.0f1 *(nécessaire pour avoir certaines fonctions de la pipeline hdrp manquantes dans les précédentes versions d'unity, et les nuages volumétriques)*


# Documentation

**Notice d'utilisation :**

Le fonctionnement d'un avion n'est pas simple, il est donc recommandé, au moins la première fois, de suivre [ce petit guide](Doc/HowToFly.md).

**Documentation développeur**

La documentation technique du projet est disponible [ici](Doc/Doc.md)

## Auteurs

- Pierre EVEN
- Léo GOSSELIN

## Credits
[Unity3D](https://unity3d.com/)
[Amplifyimpostor](http://amplify.pt/unity/amplify-impostors/)
