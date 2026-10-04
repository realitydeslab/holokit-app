# Typed: The Hair

"Typed: The Hair" creates a stereoscopic AR experience that serves Octopus-like long hair in a style of typography sways with your head movement.

<img src="Documentation~/images/hair.avif" width="320" />


In this scene, we use the player's head position as input to estimate the position of hair growth and attach physically responsive ribbons.

***Customization:***

Object “Wig Controller” controls the movement of each hair. Change properties on “Wig Controller” to adjust movement of hair.

<img src="Documentation~/images/hair01.png" width="320" />

Under 'Wig Controller,' the 'Wig VFX' handles the visual aspect.

***Customization:***

Adjust the 'Size' to control the thickness of the hair.

<img src="Documentation~/images/hair02.png" width="320" />

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Move the head; letters grow like long hair from a cap on the head and sway | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheHairJourneyTests` | Three Editors: the cap sits on the host's head and every strand grows from it (read from the effect's PositionMap), also after walking and turning; an early and a late spectator see the cap where the host does. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheHairJourneyTests` |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Not covered by the Editor tests

- How the strands sway is not compared, only where they grow from.
- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
