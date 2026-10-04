# TypedRealityTheSculpture

TypedRealityTheSculpture creates and Stereo-AR experience allows us to launch text particles from our fingertips, filling a human-shaped sculpture.

<img src="Documentation~/images/sculpture.avif" width="320" />

In this reality, we use Hand-Tracking feature from HoloKit SDK to capture player's hand(all joints) in real-time, creating particle from index tip joint.

***Customization:***

The primary visual focus of this scene is the appearance of the sculpture. Select the object "The Sculpture Reality Manager" and locate the "RealityConfiguration" component. Click on "Sticker Man" under the "Network Prefabs" field to inspect the prefab.

<img src="Documentation~/images/sculpture01.png" width="320" />

Open the prefab. Adjust properties of Visual Effect Component of "VFX" object. 

<img src="Documentation~/images/sculpture02.png" width="320" />

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Hold a hand on the floating start button (or press Trigger) to start the sculpture; reach into it and it follows the hand | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheSculptureJourneyTests` | Three Editors: a hand held on the start button starts the sculpture; the host walks up and reaches into it and the sculpture follows the hand; an early and a late spectator see the sculpture and the host's hand where the host does. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheSculptureJourneyTests` |
| `TypedTheSculptureDrawTests` | The sculpture is visible from where it was started, a step away, after a hand was lowered, and when started with a hand | EditMode suite |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Not covered by the Editor tests

- On iPhone (2026-09-30) its close-up drew nothing; that does not reproduce in the Editor.
- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
