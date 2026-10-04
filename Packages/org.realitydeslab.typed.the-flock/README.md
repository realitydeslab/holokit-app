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

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Walk around; the flock of letters circles the host 1.5 m away and follows wherever the host goes | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheFlockJourneyTests` | Three Editors: the flock circles the host and keeps circling after the host walks 3 m; an early and a late spectator see the host's head and the flock where the host does. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheFlockJourneyTests` |
| `TypedTheFlockDrawTests` | One moment of the simulated birds, held still, is drawn where the birds are | EditMode suite |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Fixed

- Before 2026-10-04 the head pose had no source (scene constraint pointed at nothing), so the flock circled the session origin instead of the player, on device too; fixed and covered by the journey.

## Not covered by the Editor tests

- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
