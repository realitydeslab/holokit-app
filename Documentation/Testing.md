# Automated tests

Run everything for the four apps (HoloKit, MOFA, Typed, QRealm), one product at a time:

```sh
Tools/run_all_tests.sh --export          # EditMode + scene playthrough + iOS export check
Tools/run_all_tests.sh --products "MOFA" --skip-playthrough   # quick single product
```

Results, logs and playthrough screenshots go to `output/test-run-<timestamp>/`; the
script prints a summary and exits non-zero on any failure. Close Unity Editors on these
projects first. Products must run sequentially: parallel Editors collide on the local
Netcode port 7777 and recompile the shared packages at the same time.

## What is covered

| Area | Test | Where |
| --- | --- | --- |
| Scenes, catalog, network prefabs, UI ids per game | package scene contract tests | `Packages/<game>/Tests/Editor` |
| Serialized GUIDs and missing scripts in product dependencies | `SerializedAssetReferenceTests` | shared `Tests/Editor` |
| Materials, shaders, VFX usable in URP | `PlayableProductRenderingAssetTests` | shared `Tests/Editor` |
| Every catalog scene plays in the Editor (XR Simulation), Player and Spectator; hover-to-start games start from the Trigger button | `EditorScenePlaythroughTests` (explicit) | shared `Tests/Editor` |
| Hover-to-start without hand tracking: Star Trigger, cooldown, hidden button, phone-mode tap hit/miss/over UI | `HoverableStartButtonTriggerTests` | shared `Tests/Editor` |
| Product icons: sizes, opaque, not HoloKit's for other products, Watch icon = iPhone icon | `ProductIconTests` | shared `Tests/Editor` |
| Product Watch source identical to the shared `WatchApp` (except its icon) | `ProductWatchAppSourceTests` | shared `Tests/Editor` |
| Legacy settings migration, product configuration, multiplayer service spawn | existing shared tests | shared `Tests/Editor` |
| Exported Xcode project: bundle id, local network/camera keys, app icon, Watch target identity, health text, Watch icon, exceptions swallowed by Unity during export | `Tools/verify_ios_export.py` | run by `--export` |
| The two tools above, with a synthetic product/export and a stub Unity CLI | `Tools/tests/test_tools.py` (`python3 -m unittest discover -s Tools/tests`) | `Tools/tests` |
| On-device lifecycle and interactions (MOFA combat, two-phone Duel, QRealm/Typed hand input) | development-build device runners | `Packages/<game>/Tests/Device`, shared `Tests/Device` |

"Shared `Tests/Editor`" is `Packages/com.holoi.library.holokit-app-lib/Tests/Editor`; each
product project runs it against its own catalog and settings.

## Not automated

- iPhone Simulator: ARKit, HoloKit, PHASE and Multipeer native libraries ship device slices only.
- Real hand-gesture recognition, Apple Watch motion, optical calibration, and two-phone
  Multipeer handshakes need physical devices; the device runners cover them only when
  launched on phones.
- Playthrough screenshots are mostly the start screen: they prove no error-shader output,
  not that every effect looks right.
