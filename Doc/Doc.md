# Documentations et notes sur l'architecture du projet

Ce document contient les informations principales à connaitre pour comprendre l'architecture du projet, et le role de chaque composant.

## Le joueur

Le joueur se resume principallement à une camera qui sera par exemple attachée à un avion.
Le gameObject racine du joueur doit contenir un component PlayerManager qui centralise les informations sur l'etat actuel du joueur.

Les inputs sont répartis dans plusieurs components

### PlanePlayerInput

Gere les inputs pour l'avion auquel est attaché le joueur

### CameraManager

Gere les inputs de camera (vue fps / tps / camera libre etc...). Positionne la camera dans le monde en fonction du besoin

## L'avion

Pour creer un avion, on assemblera differents composants présentés ci-dessous.
Chacun de ces composants a un role, et leur assemblage permet de definir le comportement de l'avion désiré.

Le gameObject à la racine de l'avion doit comporter un component PlaneManager. Ce component centralise toutes les informations sur l'avion et permet de faire l'interface avec ses composants

### AerodynamicComponent

Pour simuler "efficacement" la physique de l'avion, on attache à chacune de ces parties un AerodynamicComponent 
sur lequel on définit le mesh sur lequel on applique la simulation physique. (Mesh simplifié de l'object sur lequel on applique la physique)

Ce component va simuler les forces de trainées sur chaque face de l'objet simulé (en fonction de la position / aire / normale)

Note : Comme cette technique est assez approximative, on remplacera le modele physique du fuselage par un modele en croix. (une planche sur l'axe Y et une planche sur l'axe X)
(utiliser le mesh du fuselage directement risque de conduire à une instabilité en vol).

### MobilePart

Certains parties de l'avion vont etre mobiles et réagir en fonction des inputs. Pour cela on utilise un component "MobilePart" qui se chargera de faire tourner l'objet auquel il
est attaché en fonction de l'input correspondant à l'axe définit. L'axe est paramétré dans le Tag du gameObject (Roll / Pitch / Thrust / Yaw)

### APU

Component à integrer à l'avion : ce component simule la présence de l'APU dans l'avion. (= génératrice de forte puissance necessaire au demarrage du reacteur)

### Thruster

Moteur de l'avion. Il fournit une force de poussée en fonction de son orientation.
Le component Thruster simule l'inertie du reacteur, ainsi que l'activation ou non de la post-combustion
Note : il est possible de combiner un Thruster et un MobilePart pour implementer une poussee vectorielle.

### PlaneWheelController

Gere la rotation des roues, ainsi que l'animation de rentree et de sortie du train d'aterissage. Ce component utilise un WheelCollider pour definir la rotation des roues.

### WeaponPod

Point d'attache d'une arme. (missile / pod etc...)
Il est possible de definir une liste d'arme attachables au pod.

## Le terrain

### Le maillage

Le terrain est un quadTree découpé en sections elles mêmes découpées en nodes

Le terrain charge un carré de NxN sections autours de la camera (voir GPULandscape), chacune de ces sections sera ensuite découpée en un QuadTree (voir GPULandscapeNode)
Si le quadtree est géré sur le GPU, toute la generation du maillage est faite sur GPU :

Le CPU crée les drawcalls, puis le GPU definir les positions des vertice en fonction de l'index du sommet. (voir GPULandscapeShader.shader)

L'altitude de chaque point est determinée par une fonction `float GetAltitudeAtLocation(float2 position)`. Cette fonction est définie dans `AltitudeGenerator.cginc`

### Les masques de terrain

Pour rendre le terrain modifiable à volonté, on utilise des masques de terrain.


