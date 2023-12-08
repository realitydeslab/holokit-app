# The Flock

TypedRealityTheFlock creates an Stereo-AR experience, a flock of "birds" in a style of typography fly around the player and sing at same time.

<img src="Documentation~/images/flock.avif" width="320" />

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

## Reference:

- [BoidsSimulationOnGPU](https://github.com/IndieVisualLab/UnityGraphicsProgramming/tree/master/Assets/BoidsSimulationOnGPU)
