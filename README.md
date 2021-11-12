# Description du projet

Scuffly est un petit simulateur de vol réalisé dans le cadre de l'apprentissage d'Unity3D pour les cours de moteur de jeu 3D en 3e année de licence CMI à Strasbourg.

### Objectifs techniques et gameplay
- Pouvoir faire décoller un avion et atterrir sur plusieurs aéroports.
- Physique de vol réaliste et dynamique (influencée par l'état de l'avion).
- Possibilité d'utiliser un armement basique (missiles / canon).
- Terrain pseudo-infini généré procéduralement.
- Permettre la possibilité de jouer à plusieurs.
- Interactions dans le cockpit.
____

# Notes pour l'alpha

Un mode multijoueur est a l'étude, mais demande encore de la mise au point.

En attendant, nous avons ramené Don Quichotte du passé que nous retrouvons sur son nouveau destrier modernisé *(il sera votre instructeur de vol derrière vous dans l'avion)*. Les meuniers 2.0 eux ne l'ont pas oublié, faites donc bien attention à ne pas trop vous en approcher !

## Performances
Le jeu est assez gourmand, et certains GPU peuvent avoir du mal à le faire tourner.
En attendant un menu pour régler les options, il est possible de le faire via l’éditeur d'Unity.

Dans la scène, trouvez un Objet du nom de "GPULandscape". Celui ci est composé de 2 principaux component : un pour le maillage, et un 2e pour la végétation.
La densité du maillage peut y être réduite. La densité de la végétation se règle pour chaque type de folliage utilisé. Pour cela, sélectionner chaque asset de folliage dans la liste des folliage du component dédié, puis réduire la densité manuellement.


## Procédure pour le décollage
a. Touche `V` pour passer en première personne (les informations sont sur le HUD de l'avion).
b. Touche `O`pour activer l'alimentation électrique de l'avion.
c. Touche `P` pour activer l'APU. Attendre quelques secondes le temps que la génératrice monte en puissance.
d. Touche `I` pour débloquer la manette des gaz et démarrer le moteur. *(Note : pour l'instant seul l'audio de l'APU a été implémenté, celui ci se coupe automatiquement dès que le moteur est en route)*

L'avion est maintenant fonctionnel il ne reste plus qu'à décoller.

e. Pour avancer, il faut déjà relâcher le frein avec `B`. Donner ensuite un peu de puissance au moteur avec les flèches `Haut` et `Bas` *(attention à ne pas aller trop vite, le frein est là pour ça)*. Utiliser `Q`et `D` pour aller a droite et à gauche.
f. Une fois aligné sur la piste, mettre la pleine puissance (appuyer plusieurs fois sur `flèche haut`
d. Quand les 150 kt sont atteints *(La vitesse est indiquée sur la partie gauche du HUD)*, tirer légèrement sur le manche en appuyant sur `S` (l'état de la gouverne de profondeur se règle au clavier avec `S`et `Z`)
g. Si tout s'est bien passé, l'avion devrait commencer à décoller. Attention à ne pas trop cabrer et à le laisser prendre un peu de vitesse. Une fois en vol, rentrer le train atterrissage avec `G`.

En vol, l'avion se contrôle avec `A, E, Z, Q, S, D`.

## En vol

Pour l'instant, il n'est possible que de tirer quelques missiles *(aim9 infrarouge)* via la touche `espace`. La cible de ceux ci est déjà prédéfinie, il sera possible plus tard de la choisir précisément.

## Atterrissage

Si votre avion est toujours en un seul morceau, cela ne devrait pas être trop compliqué. C'est tout de même une manœuvre qui demande un peu d'entrainement, prenez donc votre temps pour faire votre approche.

a. Pensez à sortir le train d’atterrissage *(il n'y a pas encore de voyant dans le cockpit, vous pouvez vérifier en passant un coup en vue extérieure avec `V`)*

b. Durant votre descente vers la piste, réduisez votre vitesse vers les 200 kt. Le contact avec le sol doit se faire aux alentours des 180 kt *(normalement l'approche de ce genre d'avion se fait à l'assiette et pas à la vitesse, mais c'est beaucoup plus compliqué)*

c. Une fois sur la piste, freinez avec `B`.

Si l'avion est toujours entier, vous devriez maintenant avoir saisis les bases pour le piloter.

___

## Informations techniques

Réalisé avec Unity 2021 version 2.0f1 *(nécessaire pour avoir certaines fonctions de la pipeline hdrp manquantes dans les précédentes versions d'unity, et les nuages volumétriques)*

La documentation (en cours d'écriture) se trouve [ICI](Doc/README.md)

## Auteurs

- Pierre EVEN
- Léo GOSSELIN

## Credits
[Unity3D](https://unity3d.com/)
[Amplifyimpostor](http://amplify.pt/unity/amplify-impostors/)
