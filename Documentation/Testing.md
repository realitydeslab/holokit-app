# Automated tests

Run everything for the four apps (HoloKit, MOFA, Typed, QRealm), one product at a time:

```sh
Tools/run_all_tests.sh --export --multiplayer   # everything (about 2 hours)
Tools/run_all_tests.sh --products "MOFA" --skip-playthrough   # quick single product
Tools/run_multiplayer_test.sh MOFA MofaDuelMultiplayerTests    # one three-Editor test
python3 -m unittest discover -s Tools/tests                   # the tooling only
```

Results, logs and screenshots go to `output/test-run-<timestamp>/`; the runner prints a summary
and exits non-zero on any failure. Close Unity Editors on these projects first. Products run one
after another: parallel Editors collide on the local Netcode port 7777 and recompile shared packages.
A run also fails if tests changed Git-tracked files (for example a script writing to a shared
material while the game plays); Unity's own XR Simulation settings are excluded.

## What is covered

"Shared" means `Packages/com.holoi.library.holokit-app-lib/Tests/Editor`; each product project
runs it against its own catalog and settings.

| Area | Test | Where |
| --- | --- | --- |
| Scenes, catalog, network prefabs, UI ids per game | package scene contract tests | `Packages/<game>/Tests/Editor` |
| Serialized GUIDs and missing scripts in product dependencies | `SerializedAssetReferenceTests` | shared |
| Missing assets inside Shader Graph / VFX Graph files, against a baseline of 73 pre-upgrade references (fails on new ones and on fixed ones still listed) | `SerializedAssetReferenceTests.PlayableProductGraphReferencesResolveOrAreKnown`, `KnownMissingGraphReferences.txt` | shared |
| Materials, shaders, VFX usable in URP | `PlayableProductRenderingAssetTests` | shared |
| Every catalog scene plays in the Editor (XR Simulation) as Player and Spectator; hover-to-start games start from Trigger | `EditorScenePlaythroughTests` (explicit, run by `run_all_tests.sh`) | shared |
| Hover-to-start without hand tracking: Star Trigger, cooldown, hidden button, phone-mode tap hit/miss/over UI | `HoverableStartButtonTriggerTests` | shared |
| Product icons: sizes, opaque, not HoloKit's for other products, Watch icon = iPhone icon | `ProductIconTests` | shared |
| Product Watch source identical to the shared `WatchApp` (except its icon) | `ProductWatchAppSourceTests` | shared |
| Settings save/migration, including concurrent writers | `GlobalSettingsMigrationTests` | shared |
| Quantum Realm: hand dwell on all 10 Buddhas, short hover, Switch; Buddha effect properties; Buddha visible to the player | `QuantumRealmBuddhaInteractionTests`, `QuantumRealmBuddhaVfxTests` | `quantum-realm/Tests/Editor` |
| MOFA round summary with a team whose player left | `MofaPlayerStatsTests` | `the-duel/Tests/Editor` |
| Exported Xcode project: bundle id, local network/camera keys, app icon, Watch target identity, health text, Watch icon, exceptions Unity swallowed during export | `Tools/verify_ios_export.py` | run by `--export` |
| The tools above (synthetic export, stub Unity, temporary Git repository) | `Tools/tests/test_tools.py` | `Tools/tests` |
| On-device lifecycle and interactions | development-build device runners | `Packages/<game>/Tests/Device`, shared `Tests/Device` |

### Multiplayer (three Editors)

`Tools/run_multiplayer_test.sh <Product> <TestClass>` runs a test as host in `Products/<Product>`,
player in `Products/<Product>-peer1` and spectator in `Products/<Product>-peer2`. Peers are created
on first use (`Tools/make_peer_project.sh`: symlinked Assets/Packages/ProjectSettings, own Library,
ignored by Git). Roles exchange checkpoints as files and finish together; when one role fails, the others stop within a
second instead of waiting for their timeouts.

| Test | Product | What the roles do |
| --- | --- | --- |
| `CatalogMultiplayerSyncTests` | any | For every catalog scene: all three join; the spectator registers as a spectator; the host starts hover-to-start games; all three end with the same network objects; all return to the gallery |
| `MofaHuntingMultiplayerTests` | MOFA | Host summons and steers the dragon, locks the player and attacks with the on-screen button; the player casts at the dragon; the spectator is no combatant and cannot cast; all agree on dragon health |
| `MofaDuelMultiplayerTests` | MOFA | Two players get ready with Trigger and take turns hitting each other's shields; the spectator has no Trigger, cannot cast and sees both hits; all agree on the result |
| `QuantumRealmMultiplayerTests` | QRealm | Host starts and dwells on three Buddhas, then Switch; the client and spectator see the host's hand point and the same Buddha |

The Editor uses UnityTransport on 127.0.0.1 and one shared simulated room, so these tests cover game
networking, not Multipeer Connectivity or QR alignment. Peers' XR Origins are moved apart where a game
needs players at distinct positions. Watch swings and hand poses are injected after recognition.

Each game's README (`Packages/<game>/README.md`) lists its roles, interactions and the tests that cover them.

### End-to-end journeys (a simulated person in XR Simulation)

A journey plays a game the way a person does, in the Editor, and records every step.

| Piece | What it does |
| --- | --- |
| `SimulatedPerson` | Carries the phone: `WalkTo` / `LookAt` move XR Simulation's simulated device (`SimulationCameraPoseProvider`), so tracking, plane discovery and raycasts follow; the XR Origin never moves. `RaiseHandTo` / `HoldHandOn` / `LowerHand` inject an open 21-joint hand, only within arm's length (0.75 m) and in front of the camera, held relative to the body and re-sent every input update. `HoldHandOn(..., extension)` supports games that touch with a point projected beyond the hand (Quantum Realm, 0.5 m). |
| `SimulationArena` | A 12 m x 12 m room (walls, table, light) for XR Simulation, built from code and saved as `Tests/Editor/Environments/HoloKit Test Arena.prefab`; `Use()` selects it for the next Play session, `Restore()` (in `[TearDown]`) puts the previous environment back. The default room only allows about 1.3 m of movement. |
| `E2ETrace` | Named events with a snapshot of every registered probe (game state), samples every 0.25 s, checkpoint renders (`Capture` from the phone camera, `CapturePlayerView` from a warmed-up camera at the phone's pose, used for visibility checks) and logged errors. Saved as `<scenario>.trace.json` even when the run fails. |
| Baselines | `AssertMatchesBaseline` compares event names and snapshots (not times or poses) with `Packages/<game>/Tests/Editor/Baselines/<scenario>.json`, listing every difference. Record or refresh with `HOLOKIT_E2E_UPDATE_BASELINES=1`, review the JSON, commit it. |
| `EditorAppDriver.Run` | Runs a journey so that a failing step still runs every enclosing `finally`/`using` (Unity abandons outer coroutines when a nested one throws). |

| Journey | Where | Steps |
| --- | --- | --- |
| `QuantumRealmJourneyTests` | `quantum-realm/Tests/Editor` | In the arena: look at the floor and wait for the placement indicator (in the Editor `ARPlacementManager` shows a default point without a raycast, so plane detection is a device check); hold a hand on the floating start button; walk the circle, standing 1 m from each of the ten Buddhas in turn, look at it (the player's view must show it), hold the extended touch on it until the next Buddha appears, wait for the previous one to fade. Baseline: plane found, start, ten touches with the active effect and exactly one Buddha shown, wrap to the first. About 75 s. |
| `MofaTrainingJourneyTests` | `mofa.the-training/Tests/Editor` | In the arena: wait for the placement indicator; press Trigger (round shortened to 40 s); look at the AI (the player's view must show it); step from side to side until the AI attacks; then aim at the AI's shield and cast with simulated Watch swings, stepping aside after each cast, until the round ends by time; wait for the summary board; press Trigger for the next round. Checks: casts counted equal the spells cast, 1 ≤ hits ≤ casts, kills and deaths mirror the AI's, the round result follows the kills. Baseline: placement, countdown, fighting, AI attacked, first hit, round over, result, summary in round 2, next countdown. About 70 s. |
| `MofaDuelJourneyTests` | `mofa.the-duel/Tests/Editor` (three Editors: `Tools/run_multiplayer_test.sh MOFA MofaDuelJourneyTests`) | In the arena the two players walk to opposite ends (4 m apart) and the spectator to the side; both players press Trigger (round shortened to 40 s); each looks at the opponent (its view must show it); they take turns, host first, casting at each other's shield once the opponent's spell has landed and once their own shield has renovated, stepping aside after each cast, until the round ends by time; summary board; Trigger again starts round 2. The spectator has no Trigger, its swings cast nothing, it sees the host and both players hit. Checks: casts, 1 ≤ hits ≤ casts, kills mirror deaths, distance walked > 1 m, result follows kills; the three Editors publish identical stats and summary-board text. Baselines per role. About 6 min including startup. |
| `MofaHuntingJourneyTests` | `mofa.the-hunting/Tests/Editor` (three Editors: `Tools/run_multiplayer_test.sh MOFA MofaHuntingJourneyTests`) | Host enters as the dragon (puppeteer, as in the product), player and spectator walk to their spots. Host: summon, wait for the dragon's controls, look at it, Lock (one indicator over the player) and lock the player, Primary Attack until the player's shield breaks. Player: cast at the dragon's body with simulated Watch swings until it dies (about 56 spells for 50 health) and keep swinging through the 5 s death animation. Every peer: the dragon dies once, the round ends once (round over, result, waiting; round count 2), identical stats; the host summons the next dragon. Baselines per role. |

Run one: `unity test Products/QRealm --mode EditMode --filter QuantumRealmJourneyTests` (the runner sets
`HOLOKIT_PLAYTHROUGH_OUTPUT`; the trace and checkpoint images go to `E2E-<scenario>/` under it).

### On iPhones (unattended)

```sh
python3 Tools/run_device_tests.py --device <UDID> [--second-device <UDID>] [--products MOFA Typed] [--tests mofa-combat] [--no-build]
```

Builds each product (Unity export, signed `ReleaseForRunning` Xcode build), installs it with
`devicectl`, launches each development-build test through its environment variable, waits for the
test's `Documents/<Folder>/results.json` to report this run complete and copies the folder (report
and screenshots) to `output/device-<timestamp>/<test>/<device>/`. `progress.log` there has one
PASS/FAIL line per build, install and test. Phones must be unlocked (or have no passcode).

| Test | Product | Checks |
| --- | --- | --- |
| `*-scenes` | all | Every catalog scene loads as player and spectator, AR tracks, the local host starts, the frame matches any forced orientation, a screenshot is saved for review, exit returns to the gallery; no errors logged |
| `qrealm-interaction` | QRealm | Host hand dwell switches Buddhas; the active Buddha draws on the device (virtual-camera close-up) |
| `mofa-hunting-spawn` | MOFA | Dragon summons and enters; close-up of the dragon (pixels drawn, colour) and the ambient lighting in use |
| `mofa-combat`, `mofa-combat-stereo` | MOFA | Training round in mono and stereo: casting, hits, summary |
| `typed-finger-ribbon`, `typed-sculpture` | Typed | Ribbon: with an injected open hand the five ribbons appear, sit on the fingertips, follow the hand and leave a trail. Sculpture: hand dwell spawns the sculpture, and a close-up shows it draws |
| `mofa-duel-two-phone`, `mofa-hunting-two-phone` | MOFA | Two phones over Multipeer: host and client (needs `--second-device`) |

Permission prompts (camera, local network) appear on the first launch of each app; grant them by
hand once, or the test waits on the prompt and times out.

## Not automated

- iPhone Simulator: ARKit, HoloKit, PHASE and Multipeer native libraries ship device slices only.
- Camera hand recognition, Apple Watch motion, optical calibration, Multipeer handshakes and QR
  alignment need physical devices; the device runners cover them only when launched on phones.
- Screens are checked for error shaders and specific objects, not for overall visual quality.

## Known limits and failing tests

No known failing tests (2026-10-04).

- Editor playthrough renders show XR Simulation's room and world-space content. Effects that need a
  hand or appear outside this one view (Typed Rain and Tornado letters) are not in frame; the iPhone runs
  remain the visual check for those. Each case fails if its render is almost all black.
- In batch mode no Game view renders the main camera, so VFX Graph treats effects as unseen and does not
  simulate them. Editor tests that look at effects wait with `EditorAppDriver.RenderMainCameraFor` /
  `RenderMainCameraFrames`, which render the main camera every frame.
- `typed-sculpture` on iPhone: the sculpture spawns, but its close-up draws nothing, even after restarting
  its VFX in view. Unresolved (2026-09-30); look at The Sculpture by hand to tell a real problem from a
  test artefact.

### Fixed 2026-10-04

- MOFA Hunting: the dragon's entrance and death animations clipped it through its shared material assets, so a
  dead dragon left `_IsClip` on in the assets (saved into the project when played in the Editor, carried into
  the next dragon in an app session). Each dragon now clips its own copies (found by the multiplayer runner's
  asset check during `MofaHuntingJourneyTests`).
- MOFA: a timed popup destroyed whatever popup was current when its timer ran out, so the Victory/Defeat
  popup (3 s, as long as the result phase) could remove the summary board that replaced it; a player then saw
  no summary. Each timed popup now removes only itself (`MofaPopupTests`; found by `MofaDuelJourneyTests`).
- After the first scene in a Play session XR Simulation stopped showing its room, so later playthrough
  renders were black; XR now restarts at the gallery between cases.
- `QuantumRealmBuddhaInteractionTests.ActivatedBuddhaIsVisibleToThePlayer` failed since 2026-09-28: it
  waited without rendering, so the Buddha's effect was never simulated. It now renders while waiting; the
  Buddha changes about 13% of the view, and fixed-view renders show it upright above its seat.
