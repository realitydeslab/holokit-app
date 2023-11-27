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

<!-- https://github.com/holoi/reality-typography/assets/52849063/2efbd68b-7d06-4bdd-bccd-4b25feef5387 -->
[ ] flock video

To create a realistic experience, this reality uses boid algorithm to drive "birds" fly around the player.
It takes the head position (the position of the mobile device) as input to create circular movement.

In this scene, we used AR shadows on real-world ground to create a more realistic effect(shader included in this project). 

### TypedRealityTheFingerRibbon

TypedRealityTheFlock creates an Stereo-AR experience that ribbons flows along with your finger-tips.

<!-- https://github.com/holoi/reality-typography/assets/52849063/38d99d45-cdd7-49d6-b43b-87a287986b96 -->
[ ] finger ribbon video

TypedRealityTheFingerRibbon uses Hand-Tracking feature from HoloKit SDK.
Tracking the positions of all your finger-tip to create ribbons

### TypedRealityTheHair

TypedRealityTheHair creates an Stereo-AR experience that several Octopus-like long hair in a style of typography sways with your head movement.

<!-- https://github.com/holoi/reality-typography/assets/52849063/4b922a44-9aaa-4750-a336-00d826819b59 -->
[ ] hair video

In this scene, we use the player's head position as input to estimate the position of hair growth and attach physically responsive ribbons.

### TypedRealityTheRain

TypedRealityTheRain creates an Stereo-AR experience where a white cloud, styled like typography, follows the player(person in camera in Screen-AR mode), continuously dropping raindrops. These raindrops collide with the real-world ground, splitting and fading away.

<!-- https://github.com/holoi/reality-typography/assets/52849063/c69cc5aa-bc8a-44c4-9b3e-95c82af2933f -->
[ ] rain video

In this reality, we use 3DBodyTracking Feature to track a person in camera in Screen-AR mode. In Stereo-AR mode, the cloud follows the player.

### TypedRealityTheScanner

TypedRealityTheScanner creates and Stereo-AR experience where  white rectangular ribbons in a style of typography on the surrounding floors, walls, and other environmental surfaces. These ribbons nest layer by layer, expanding over time.

<!-- https://github.com/holoi/reality-typography/assets/52849063/9012fe22-617c-4219-af83-a85f64903930 -->
[ ] scanner video

In this reality, we use Meshing feature to get the mesh information of our surroundings in real-time, to change the material of mesh to render rectangular ribbons effect.

### TypedRealityTheSculpture

TypedRealityTheSculpture creates and Stereo-AR experience allows us to launch text particles from our fingertips, filling a human-shaped sculpture.

<!-- https://github.com/holoi/reality-typography/assets/52849063/93bc0c9c-281c-4dbf-bd5c-83d224b8617f -->
[ ] sculpture video

In this reality, we use Hand-Tracking feature from HoloKit SDK to capture player’s hand(all joints) in real-time, creating particle from index tip joint.

### TypedRealityTheTornado

This augmented reality experience creates a text storm that follows the target/player.

<!-- https://github.com/holoi/reality-typography/assets/52849063/d17b3b14-c1c6-47f8-91c1-ef880aa6d3d1 -->

[ ] tornado video

In this reality, we take head position as input, create a storm following the target(in Screen-AR mode)/player(in Stereo-AR mode).

### TypedRealityTapText

This augmented reality experience creates a line segment covered with messages formed by stretching the index finger and thumb.

<!-- https://github.com/holoi/reality-typography/assets/52849063/869d9e8a-c80a-49fa-92ee-452e34779eb7 -->
[ ] taptext 

In this reality, we utilized the gesture tracking capabilities of the SDK, recognizing the fingertips of the index finger and thumb and calculating the distance between them. A certain threshold triggers the retention of the line segment.

### TypedREalityTheAudio

This scene generates a music ball that synchronizes with sound input, be it human voices or music, maintaining a visual style reminiscent of text.

The music ball consists of three nested spheres, arranged from the innermost to the outermost, creating a harmonious visual effect. These spheres are entirely created using VFX, and they share the same VFX asset to drive their visual effects.

Select and open the object "Audio Typography," and you will see three sub-objects, each representing a layer of the music ball.

[]audio 01

**Customization:** Each sub-object's VFX shares the same set of attributes, and you can modify the visual effects by adjusting these properties. Of the four properties shown in the following image, "Init Mesh" determines the initial shape, which is a sphere in this case. "Amplitude" is autonomously determined by the program, and modifying it won't change its visual appearance during runtime. "Index Count" and "Index Multipier" together determine the particle count, with larger values resulting in more particles.

[] audio 02

# Reference
