# MOFA: The Duel

Two players duel with spells; spectators watch.

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Player (host = Blue, client = Red) | Press the Trigger button to get ready (the round starts when both are); cast the basic spell with an Apple Watch wrist swing, or Trigger, once charged; break the opponent's life shield | Hurt a teammate |
| Spectator | Watch both players, their shields and spells | Get ready, cast (no Trigger button; Watch swings are ignored), count as a duelist |

Spells fly from the phone camera along its heading and pitch, so players aim by pointing the phone.
Opposing spells that meet in the air cancel each other. The round lasts 80 s; the summary board then
shows each team's kills and hit rate (a team whose player left shows zeros).

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `MofaDuelMultiplayerTests` | Three Editors: host and client duel, spectator watches. The client stands 3 m away facing the host. Both press the on-screen Trigger to start. Players take turns aiming at the other's shield and casting with simulated Watch swings until it is damaged, and each sees its own shield damaged. The spectator is not a duelist, has no Trigger button, cannot cast, sees both shields damaged and renders the duel. All three report the same casts and shield state. | `Tools/run_multiplayer_test.sh MOFA MofaDuelMultiplayerTests` |
| `MofaPlayerStatsTests` | The round summary copes with a team whose player left | EditMode suite |
| `MOFATheDuelSceneTests` | Scene, prefab and UI references | EditMode suite |
| `Tests/Device` | Two iPhones over Multipeer Connectivity with QR alignment | development build |

## Not covered by the Editor tests

- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
- Real Apple Watch motion recognition; tests emit the event the phone receives after a recognized swing.
- Players firing at the same moment along one line: their spells cancel, so the test takes turns.
