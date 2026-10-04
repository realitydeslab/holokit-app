# Typed: The Finger Ribbon

The Finger Ribbon creates an Stereo-AR experience that ribbons flows along with your finger-tips.

<img src="Documentation~/images/finger.avif" width="320" />

TypedRealityTheFingerRibbon uses Hand-Tracking feature from HoloKit SDK.
Tracking the positions of all your finger-tip to create ribbons

***Customization:*** 

The visual part controls by vfx objcet under “Finger VFX Manager”, here are 5 objects controls all 5 fingers, the only difference of them is that they use different “Texture2D” to identify each finger.

Replace texture asset of “Texture2D” of object “Finger Lines_FingerName”.

<img src="Documentation~/images/fingerribbon01.png" width="320" />

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Raise a hand; five ribbons flow from the fingertips and follow the hand | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheFingerRibbonJourneyTests` | Three Editors: a raised hand starts five ribbons whose tips catch up with the fingertips and follow the hand; an early spectator and one who joins while the hand is up see the ribbons and their tips where the host's are. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheFingerRibbonJourneyTests` |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Fixed

- A spectator joining while the host's hand was up never saw the ribbons (the state was read only on change); fixed 2026-10-04.

## Not covered by the Editor tests

- Camera hand detection is a device check; the Editor injects a hand pose.
- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
