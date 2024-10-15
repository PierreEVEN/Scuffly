# Objectifs initiaux


## Problemes techniques

- Terrain procedural

- Folliage

- Simulations aerodynamiques

## Le terrain

Pour un simulateur de vol, il est vite compliqué de modéliser un terrain entier. (surtout si l'on veut aller dans le detail) 
Pour ca, on peut passer par de la génération procedural à partir d'algorithmes, ou utiliser des données de terrain réelles.

Dans les deux cas, on est confronté aux même contraintes :

- Comment modeliser un terrain pseudo-infini ?

- Comment rendre une partie de ce terrain modifiable à volonté par les artistes ?

### Terrain infini

#### Principes de base

A l'origine, ce projet est une continuation d'un precedent projets (fait avec Three.JS).
[voir ici](https://github.com/PierreEVEN/ThreeFlightSimulator/blob/main/documentation/CompteRendu.md)

#### Ameliorations

Le probleme de ce systeme est que generer le maillage sur le CPU est relativement long et peut conduire à des freezes reguliers
(Peu etre partiellement corrigé en multithreadant). L'idée serait donc de transferer tout le traitement des vertices **sur le GPU**.

Ce choix presente de nombreux avantages

- La charge de travail du CPU est quasi nulle

- Le transfert sur GPU permet une mise a jour en temps réel du maillage (pas besoin d'attendre plusieurs secondes que le maillage soit regénéré pour visualiser le resultat)

- Sous unity, compiler des shaders est beaucoup plus rapide que de recompiler le code en C#

- Il est possible de sampler efficacement des textures pour faire des masques via des textures (heightmaps)

#### Implementation

Pour chaque node du quadtree, au lieu d'envoyer au vertex shader une liste de sommet, on lui donne l'ordre de dessiner N vertices (N = resolution * resolution * 6) via un DrawProcedural
Ensuite, pour chacun de ces sommets le vertex shader va reconstruire les coordonées x et z du sommet correspondant
```hlsl
float posX = (quadId % _Subdivision) * _Width + (vertId == 2 || vertId == 4 || vertId == 5 ? _Width : 0);
float posY = (quadId / _Subdivision) * _Width + (vertId == 1 || vertId == 2 || vertId == 4 ? _Width : 0);
```
Il ne reste plus qu'à utiliser la fonction de generation de hauteur `GetAltitudeAtLocation()` pour avoir l'axe y du point.
On en profite pour generer la normal du point, on transmet le tout a un fragment shader classique et le tour est joué.

**Limitation et ameliorations futures**

a. L'appel de `GetAltitudeAtLocation()` peut devenir assez gourmant, il faudrait donc ideallement stocker pour chaque node une heightmap pour eviter de regenerer toutes les donnees a chaque frame.
Dans notre cas ce cout est encore relativement negligeable.

b. Dans le cas où on regenere les données chaque frame, le calcul des normals est assez gourmand. Il est alors possible de faire le calcul par pixel plutot que par sommet,
mais le resultat est visuellement moins appreciable. On a choisi de laisser le calcul des normales par vertex, mais c'est une piste d'amélioration a garder en tete.

### Heightmap d'un terrain infini.

Pour generer un terrain procedurallement, on se base generallement sur des fonctions de type noise (perlin / simplex...)
Cette methode a l'avantage d'etre "relativement" peu couteuse, mais ne permet pas un controle facile de la topologie.

Pour eviter ce probleme, on passe par des masques qui vont permettre de definir manuellement dans des zones predefinies la hauteur pour chaque point.

- On peut a partir de ce moment imaginer plusieurs types de masques : box / cercles / spheres / textures etc...



## Folliage

## Simulations aerodynamiques

Pour faire simple, simuler la physique d'un avion en temps réel, c'est compliqué !

