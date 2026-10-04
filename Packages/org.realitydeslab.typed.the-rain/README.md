# TypedRealityTheRain

TypedRealityTheRain creates an Stereo-AR experience where a white cloud, styled like typography, follows the player(person in camera in Screen-AR mode), continuously dropping raindrops. These raindrops collide with the real-world ground, splitting and fading away.

<img src="Documentation~/images/rain.avif" width="320" />

In this reality, we use 3DBodyTracking Feature to track a person in camera in Screen-AR mode. In Stereo-AR mode, the cloud follows the player.

***Customization:***

<img src="Documentation~/images/rain01.png" width="320" />

Object “The Rain” controls the visual of cloud. Object “Text Rain” under “The Rain” controls the visual of raindrops.

Feel free to adjust properties of VFX components of “The Rain” and “Text Rain” to create sth new.

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Walk around; a cloud of letters floats 1.5 m above the head and rains on the host | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheRainJourneyTests` | Three Editors: the cloud floats above the host's head and the rain falls around it, also after a 3 m walk; an early and a late spectator see the head and the cloud where the host does. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheRainJourneyTests` |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Fixed

- Before 2026-10-04 the head pose had no source, so the cloud stayed at the session origin; fixed and covered by the journey.

## Not covered by the Editor tests

- The rain's floor height needs a plane classified as Floor; XR Simulation never classifies planes, so this is a device check.
- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
