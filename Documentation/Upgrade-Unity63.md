# Unity 6.3 / iOS 27 upgrade

## Preservation and target

The working upgrade starts from checkpoint `72434830` on
`upgrade/unity63-ios27-four-apps`. The App Store reference is `a00a9713`.
Existing package renames and asset GUIDs are preserved. Older assets are evidence
for individual repairs, not a replacement for the current tree. The existing
MOFA and Typed sibling checkouts have local changes and are read-only inputs.

Target: four separately installable iPhone apps, HoloKit, MOFA, Typed, and QRealm,
using Unity 6000.3.24f1 (the latest 6.3 patch returned by Unity CLI on 2026-09-22)
and Xcode 27 / iOS 27 SDK. Keep the current iOS 18 minimum unless a dependency
requires a higher version. iPhone 18 Pro optical calibration and physical AR
behavior require separate validation from successful SDK compilation.

## Architecture

- Keep one source of truth for shared Unity packages in this repository. Preserve
  existing package names and GUIDs; do not perform another broad rename.
- Shared layers are asset data, HoloKit XR/native functionality, app shell and
  common UI, and ARUX. Game packages depend on those layers; the shell must not
  depend on individual games.
- Four thin product projects select packages and product configuration. HoloKit
  contains the current full catalog; MOFA selects its five enabled game scenes;
  Typed selects its seven enabled scenes; QRealm selects QuantumRealm. Preserve
  other existing game packages, without silently adding them to a release.
- Each product owns its bundle identifier, icons/cover, reality catalog, avatar
  and object catalogs, and app feature settings. Reuse UI prefabs and styles.
  Move product-specific references out of shared package defaults by injecting
  the product settings during bootstrap/build, preserving existing HoloKit use.
- Package manifests must declare their actual dependency closure. A smaller
  product must compile without unrelated game packages installed. Merely hiding
  scenes in the full project's build list is insufficient to prove decoupling.
- Make Watch companion packaging an explicit product feature and keep native
  build steps conditional on iOS. Preserve the full app's existing functionality.

## Validation and evidence

Use Unity CLI for editor operations and builds. Logs for this session are in
`/tmp/holokit-upgrade-20260922/`.

1. Static GUID/package audit against the App Store commit:
   `python3 Tools/audit_unity_references.py --output Logs/reference-audit.json`.
   This is a discovery aid, not proof of runtime correctness; cached package
   metadata can be stale and some unresolved GUIDs can predate the upgrade.
2. Authoritative Unity scene audit:
   `unity run . --editor-path /Applications/Unity/6000.3.24f1/Unity.app -- -buildTarget iOS -executeMethod HoloKitUpgradeAudit.Run -logFile Logs/upgrade-audit.log`.
   This loads every enabled scene, checks missing components and serialized
   object references, and verifies the migrated AR plane scripts compile.
3. Build all four isolated product projects to Xcode, then signed device apps.
   Use the native Build Profile: `unity build . --profile iOS --output-path
   Builds/Upgrade/HoloKit --editor-path /Applications/Unity/6000.3.24f1/Unity.app`.
   No custom C# build entry point is required. The initial export used a temporary
   entry point that has been removed; the native profile export has also passed.
   Validate launch, each catalog scene, AR permissions/tracking, stereo and mono
   transitions, multiplayer/spectator mode, recording, audio/haptics, and Watch
   features where enabled. Keep per-app evidence and explicit unsupported tests.
4. Validate iPhone 18 Pro with the real hardware. The currently connected physical
   device is an iPhone 15 Pro; an iPhone 18 Pro simulator cannot prove AR/optical
   correctness. Do not call the whole upgrade complete based on simulator tests.

## Initial findings

- MOFA still references the old app-shell scene path and old permissions package
  location; Typed references the current package locations but has local edits.
- The migrated AR plane C# scripts lacked an assembly boundary and Unity skipped
  compiling them. An assembly reference now assigns them to `Holoi.Library.ARUX`.
- Embedded package manifests still contain old AR Foundation / Netcode / render
  pipeline requirements. Actual resolved versions and assembly usage must guide
  dependency repairs.
- Shared app settings currently reference project-owned catalogs, creating a
  reverse dependency that needs product-specific bootstrap configuration.
- Initial static audit: 258 unresolved GUIDs, no duplicate project GUIDs. These
  include legacy/unused assets and need Unity and baseline classification before
  making repairs; they are not 258 confirmed regressions.
- Unity loaded all 15 enabled scenes and checked 1,757 components. The two
  migrated AR plane scripts resolve to compiled classes. The initial nine broken
  object references have been repaired: obsolete texture overrides were removed
  from QRealm/Sculpture only after checking that the current graph exposes no such
  texture property, and a disabled Finger Ribbon Animator's missing controller
  reference was cleared. The controller was already absent in the App Store
  version; no replacement animation was invented.
- Package-local scene tests additionally found null AR occlusion references in
  Finger Ribbon, Flock, and Hair, which their gameplay code dereferences. Each
  now points to the existing scene camera's occlusion component. Unity 6.3 also
  migrated scene serialization when saving these repairs.
- The initial HoloKit device-target Unity export succeeded with zero errors and
  317 warnings. Xcode 27 signed compilation passed, and the resulting app was
  installed and launched on the iPhone 15 Pro running iOS 27. Unity/Metal startup
  succeeded. This first launch predates the scene repairs and does not establish
  that all games or interactions work.

## Package-local tests

All 13 enabled games own their fixtures and test assemblies under
`Packages/<game>/Tests/Editor`. The app library supplies shared assertions under
`Tests/Editor`; it does not depend on game test assemblies. The checks cover scene
scripts and serialized references, catalog/cover/entrance configuration, network
prefabs, UI panel identifiers, and game-specific required interaction references.

Run with the user's Unity CLI:

```sh
/Users/amber/.unity/bin/unity test . --mode EditMode --filter HoloKit.GameTests \
  --editor-path /Applications/Unity/6000.3.24f1/Unity.app \
  --output Logs/game-editmode.xml --report-format nunit,junit \
  --timeout 1200 -- -buildTarget iOS -logFile Logs/game-editmode.log
```

On 2026-09-22 the initial run passed 83/89, with six failures. After repairing
the references above, the rerun passed **89/89, zero skipped**. NUnit and JUnit
reports are in `/tmp/holokit-upgrade-20260922/game-editmode-fixed*.xml`.

The app library's `Tests/Device` has an opt-in smoke runner compiled only into
development players. Launch a development app with `--holokit-scene-test` to
traverse its own catalog as player and spectator, checking local host startup,
AR tracking, scene exit, and logged errors, and saving screenshots for visual
inspection. `--holokit-test-scene=TypedTheFlock` limits the run to one scene.
Results are written to `Documents/HoloKitSceneTests/results.json` in the app's
data container. This is a startup/lifecycle smoke test, not proof of physical
hand gestures, optical calibration, remote-peer synchronization, or Watch use.

### Existing-install migration

The first development launch with logging enabled exposed a startup blocker that
the silent initial launch could not reveal: the installed App Store preferences
refer to assembly `Holoi.Library.HoloKitApp`, while the renamed runtime assembly
is `Holoi.Library.HoloKitAppLib`. BinaryFormatter threw before bootstrap finished.
The save loader now accepts only the known legacy settings types, migrates to a
versioned JSON document, and retains the original `.save` file. Repeated JSON
saves keep a previous copy; unreadable JSON is preserved before fallback.

Eight migration regression tests passed through `unity test`, including both
assembly names, corrupt data, unexpected object types, null preference maps,
round trips, and previous-file preservation. The old-assembly test also used a
copy of the connected iPhone's actual settings, supplied using
`-legacySettingsFixture /path/to/HoloKitAppGlobalSettings.save` after `--`.
The device fixture remains outside Git.

For iOS launches, the smoke runner also supports process environment variables
`HOLOKIT_SCENE_TEST=1` and optional `HOLOKIT_TEST_SCENE=<scene name>` through
`devicectl --environment-variables`. This avoids relying on Unity's managed
command-line argument forwarding. The migration build still requires renewed
device validation before the startup bug can be considered resolved on iPhone.

This document describes the intended architecture and work in progress, not a
claim that the four apps have built or run successfully.
