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

## Not automated

- iPhone Simulator: ARKit, HoloKit, PHASE and Multipeer native libraries ship device slices only.
- Camera hand recognition, Apple Watch motion, optical calibration, Multipeer handshakes and QR
  alignment need physical devices; the device runners cover them only when launched on phones.
- Screens are checked for error shaders and specific objects, not for overall visual quality.

## Known failing tests (2026-09-28)

- `QuantumRealmBuddhaInteractionTests.ActivatedBuddhaIsVisibleToThePlayer`: the Buddha point cloud
  draws nothing in the Editor while the seat ring does. Needs a device check.
