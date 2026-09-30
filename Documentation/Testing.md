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
ignored by Git). Roles exchange checkpoints as files and finish together.

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

- Editor playthrough renders (`EditorScenePlaythroughTests`) come out black for most scenes, so their
  error-shader (magenta) check proves little. Visual checks rely on the iPhone runs until this is fixed.
- `typed-sculpture` on iPhone: the sculpture spawns, but its close-up draws nothing, even after restarting
  its VFX in view. Unresolved (2026-09-30); look at The Sculpture by hand to tell a real problem from a
  test artefact.

### 2026-09-28

- `QuantumRealmBuddhaInteractionTests.ActivatedBuddhaIsVisibleToThePlayer`: the Buddha point cloud
  draws nothing in the Editor while the seat ring does. Needs a device check.
