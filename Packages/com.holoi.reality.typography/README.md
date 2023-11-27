# reality-typography

## What is reality-typography

Last year we decided to launch official app of HoloKit, we realized that we should give our customer(especially developers) a glipse on what we can do with iPhone and HoloKit, so we created following scenes(realities we called) aim to utilize and demonstrate the capabilities of AR Foundation on the iPhone to create AR experiences that showcase both basic and advanced features.
This project contains 7 typography realities released on holokit app, includes:
1. TypedRealityTheFlock
2. TypedRealityTheFingerRibbon
3. TypedRealityTheHair
4. TypedRealityTheRain
5. TypedRealityTheScanner
6. TypedRealityTheSculpture
7. TypedRealityTheTornado

<img src="Documentation~/images/all.png" width="640" />


And an additional one currently undergoing testing.

1. TypedRealityTheAudio

## How to try it
1. Clone the project.
2. Open with Unity.
3. Open a scene from path: Assets->Scenes.
4. Build select scene into an Xcode project.
5. Build to you device, open the app and try it out.

## How does it work

### TypedRealityTheFlock

TypedRealityTheFlock creates an Stereo-AR experience, a flock of "birds" in a style of typography fly around the player and sing at same time.

<img src="Documentation~/images/bird.avif" width="320" />

To create a realistic experience, this reality uses boid algorithm to drive "birds" fly around the player.
It takes the head position (the position of the mobile device) as input to create circular movement.
We used AR shadows on real-world ground to create a more realistic effect(shader included in this project). 

#### Movements

The movement of flock controls by “GPU Boids” under object “Boid Typo Reality Manager”

<img src="Documentation~/images/flock01.png" width="320" />

***Customization:*** 

Change properties of “GPU Boids” to customize movement.

#### Visuals

The visual part controls by VFX on object “Boid”

<img src="Documentation~/images/flock02.png" width="320" />

***Customization:*** 

Here you can change the size of each bird and change the text of each bird, but is should be a 2*7 flipbook texture.

<img src="Documentation~/images/flock03.png" width="320" />


### TypedRealityTheFingerRibbon

TypedRealityTheFlock creates an Stereo-AR experience that ribbons flows along with your finger-tips.

<img src="Documentation~/images/finger.avif" width="320" />

TypedRealityTheFingerRibbon uses Hand-Tracking feature from HoloKit SDK.
Tracking the positions of all your finger-tip to create ribbons

***Customization:*** 

The visual part controls by vfx objcet under “Finger VFX Manager”, here are 5 objects controls all 5 fingers, the only difference of them is that they use different “Texture2D” to identify each finger.

Replace texture asset of “Texture2D” of object “Finger Lines_FingerName”.

<img src="Documentation~/images/fingerribbon01.png" width="320" />


### TypedRealityTheHair

TypedRealityTheHair creates an Stereo-AR experience that several Octopus-like long hair in a style of typography sways with your head movement.

<img src="Documentation~/images/hair.avif" width="320" />


In this scene, we use the player's head position as input to estimate the position of hair growth and attach physically responsive ribbons.

***Customization:***

Object “Wig Controller” controls the movement of each hair. Change properties on “Wig Controller” to adjust movement of hair.

<img src="Documentation~/images/hair01.png" width="320" />

Under 'Wig Controller,' the 'Wig VFX' handles the visual aspect.

***Customization:***

Adjust the 'Size' to control the thickness of the hair.

<img src="Documentation~/images/hair02.png" width="320" />



### TypedRealityTheRain

TypedRealityTheRain creates an Stereo-AR experience where a white cloud, styled like typography, follows the player(person in camera in Screen-AR mode), continuously dropping raindrops. These raindrops collide with the real-world ground, splitting and fading away.

<img src="Documentation~/images/rain.avif" width="320" />

In this reality, we use 3DBodyTracking Feature to track a person in camera in Screen-AR mode. In Stereo-AR mode, the cloud follows the player.

***Customization:***

<img src="Documentation~/images/rain01.png" width="320" />

Object “The Rain” controls the visual of cloud. Object “Text Rain” under “The Rain” controls the visual of raindrops.

Feel free to adjust properties of  VFX components of “The Rain” and “Text Rain” to create sth new.

### TypedRealityTheScanner

TypedRealityTheScanner creates and Stereo-AR experience where  white rectangular ribbons in a style of typography on the surrounding floors, walls, and other environmental surfaces. These ribbons nest layer by layer, expanding over time.

<img src="Documentation~/images/scanner.avif" width="320" />


In this reality, we use Meshing feature to get the mesh information of our surroundings in real-time, to change the material of mesh to render rectangular ribbons effect.

***Customization:***

The material of meshes control all the visual elements of scene Scanning. 

<img src="Documentation~/images/scanning01.png" width="320" />

Select “Meshing” object and click on “Mesh Prefab” field to check prefabs we provided.

<img src="Documentation~/images/scanning02.png" width="320" />

In the initial design, we created different prefabs for various types of meshing, with the sole distinction being the different materials. Each material is specifically overlaid with text of that type.

However, we couldn't address the UV mapping issue with meshing, which resulted in a material being unable to consistently represent both vertical and horizontal planes. Consequently, we primarily scan the ground, using only one prefab, “DetectedMeshesHorizontal_Floor”

If you wish to customize the appearance of the scanned meshing, please duplicate the prefab, modify its material, and replace the original “Mesh Prefab” with your modified prefab.


### TypedRealityTheSculpture

TypedRealityTheSculpture creates and Stereo-AR experience allows us to launch text particles from our fingertips, filling a human-shaped sculpture.

<img src="Documentation~/images/sculpture.avif" width="320" />

In this reality, we use Hand-Tracking feature from HoloKit SDK to capture player’s hand(all joints) in real-time, creating particle from index tip joint.

***Customization:***

The primary visual focus of this scene is the appearance of the sculpture. Select the object "The Sculpture Reality Manager" and locate the "RealityConfiguration" component. Click on "Sticker Man" under the "Network Prefabs" field to inspect the prefab.

<img src="Documentation~/images/sculpture01.png" width="320" />

Open the prefab, adjust properties of Visual Effect Component of “VFX” object. 

<img src="Documentation~/images/sculpture02.png" width="320" />


### TypedRealityTheTornado

This augmented reality experience creates a text storm that follows the target/player.

<img src="Documentation~/images/tornado.avif" width="320" />


In this reality, we take head position as input, create a storm following the target(in Screen-AR mode)/player(in Stereo-AR mode).

***Customizatin:***

The main visual element in this scene is the text-styled tornado. This tornado is entirely created using VFX. To customize its visual appearance, locate the Visual Effect asset under the "Tornado" object and modify its parameters to adjust the visual representation of the tornado.

<img src="Documentation~/images/tornado01.png" width="320" />


### TypedRealityTapText

This augmented reality experience creates a line segment covered with messages formed by stretching the index finger and thumb.

<!-- https://github.com/holoi/reality-typography/assets/52849063/869d9e8a-c80a-49fa-92ee-452e34779eb7 -->
[ ] taptext 

In this reality, we utilized the gesture tracking capabilities of the SDK, recognizing the fingertips of the index finger and thumb and calculating the distance between them. A certain threshold triggers the retention of the line segment.

### TypedREalityTheAudio

This scene generates a music ball that synchronizes with sound input, be it human voices or music, maintaining a visual style reminiscent of text.

<img src="Documentation~/images/audio.avif" width="320" />

The music ball consists of three nested spheres, arranged from the innermost to the outermost, creating a harmonious visual effect. These spheres are entirely created using VFX, and they share the same VFX asset to drive their visual effects.

***Customization:***

Select and open the object "Audio Typography," and you will see three sub-objects, each representing a layer of the music ball.

<img src="Documentation~/images/audio01.png" width="320" />


Each sub-object's VFX shares the same set of attributes, and you can modify the visual effects by adjusting these properties. Of the four properties shown in the following image, "Init Mesh" determines the initial shape, which is a sphere in this case. "Amplitude" is autonomously determined by the program, and modifying it won't change its visual appearance during runtime. "Index Count" and "Index Multipier" together determine the particle count, with larger values resulting in more particles.

<img src="Documentation~/images/audio02.png" width="320" />

In addition, music ball is interactively linked to the position of the hand: the distance between the hand and the center of the field of view determines the size of the music ball.

***Customization:***

<img src="Documentation~/images/audio03.png" width="320" />

Select the music ball and locate the "Audio VFX Manager" component, which has two properties. The first property is of type Transform and serves as a reference to the hand's position. In our case, we selected Middle0 as the reference, corresponding to the lower joint of the middle finger. The second property is an intensity value, determining how much the change in hand position affects the visual content.


# Reference
