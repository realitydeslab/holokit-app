# MOFA: The Hunting

Players team up against a dragon that the host controls.

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host (dragon controller) | Summon the dragon with the Spawn Dragon button; steer it; Lock a player as its target; attack with the Primary/Secondary Attack buttons | Cast spells |
| Player | Wear a life shield; cast spells with an Apple Watch wrist swing once the basic spell has charged; damage the dragon | Control the dragon |
| Spectator | Watch the fight (dragon, spells, shields, health bar) | Cast spells, be locked as a target, count as a combatant |

Round flow: host summons → every player is set ready → the dragon flies out of the portal → Fighting.
The dragon's health (50) and each player's shield fragments are server state, so every device shows the same values.

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `MofaHuntingMultiplayerTests` | Three Editors: host, player, spectator. Host summons the dragon, locks the player and taps the on-screen Primary Attack button until the player's shield is damaged. The player turns to the dragon and casts with simulated Watch swings until its health drops. The spectator is not a combatant, cannot cast, sees both hits and renders the fight. All three end with the same dragon health. | `Tools/run_multiplayer_test.sh MOFA MofaHuntingMultiplayerTests` |
| `MofaHuntingJourneyTests` | End to end in the test arena with three Editors, entering as the product does (host as the dragon, i.e. puppeteer): the player and spectator walk to their spots; the host summons the dragon, waits for its controls, looks at it (its view must show it), taps Lock (one indicator over the player) and locks the player, then taps Primary Attack until a fireball breaks the player's shield. The player casts at the dragon with simulated Watch swings until it dies and keeps swinging through the death animation. Every peer must see the dragon die once and the round end once (round over, result, waiting; round count 2), and report the same stats; then the host summons the next dragon. Event timelines are compared with `Tests/Editor/Baselines/MofaHuntingJourney-<role>.json` | `Tools/run_multiplayer_test.sh MOFA MofaHuntingJourneyTests` |
| `MOFATheHuntingSceneTests` | Scene, prefab and UI references | EditMode suite |
| `MOFAMobileInputPrefabTests` | Malbers mobile buttons keep their UI components and pointer events | EditMode suite |
| `Tests/Device` | On iPhones: summon the dragon (single device) and two-phone connection over Multipeer | development build, see the test files |

## Not covered by the Editor tests

- Multipeer Connectivity and QR alignment: the Editor uses UnityTransport on 127.0.0.1 and one shared simulated room.
- Real Apple Watch motion recognition: tests emit the event the phone receives after a recognized swing.
- Dragon steering with the joystick, and what the fight looks like on a HoloKit headset.
