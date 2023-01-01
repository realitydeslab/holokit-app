# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Added Audios for Spells do not have one [@sizheng]

- Added `RealityConfiguration` script, which is a dependent class of `RealityManager`. Now all `PlayerPrefab`, `NetworkPrefab`s and URP Setting file are stored in the corresponding `RealityConfiguration` [@yuchen]

- Added version number tracking and display the current version number in menu page [@yuchen]

- Added New Typed Reality: Coin Path [@sizheng]

- Added new `MofaHuntingDragonControllerUIPanel` for "MOFA: The Hunting" [@yuchen]

- Added Dragon lock target system [@yuchen]

### Changed

- Changed the audio of AceFire Basic Spell: Spawn [@sizheng]

- Updated visual of Spells for a better looking [@sizheng]

- Updated visual of Death Circle (enemy) [@sizheng]

- Updated visual of Dragon Powers [@sizheng]

- Use VideoKit instead of my own implementation of NatCorder for video recording [@yuchen]

- Updated new tutorial videos for "Getting Started Page", "MOFA: The Training" and "MOFA: The Duel" [@yuchen]

- Refactored `MofaHuntingRealityManager` for "MOFA: The Hunting" [@yuchen]

- Updated visual of Buddhas in QuantumRealm Reality [@sizheng]

### Removed

### Fixed

- Disable `ARPlaneManager` after spawning the avatar in "MOFA: The Training" [@yuchen]

- Fixed Dragon Ball Animation Bug [@sizheng]


## [1.0.0] - 2022-12-27

### Added 

- Added Meebits fade in animation when spawned [@sizheng]

- Added `EditorBuildSettings.asset` in .gitignore file [@yuchen]

- Added `playerRegistered` analytical event when a new player signed in with their Apple Id for the first time [@yuchen]

- Added death circle display for enemy players [@sizheng]

- Added two preview videos for "Quantum Realm" and "Typed Reality: Tornado" [@yuchen]

- Added `Assets/NMLBuildCache` and `Assets/StreamingAssets` to .gitignore file, which are build files for NatCorder and UGS respectively [@yuchen]

### Changed

- For ScanQRCode UI panel, changed back to use static event instead of checking in every `Update()` to update state [@yuchen]

### Removed

- Removed `HoloKitAppUIEventReactor` class, which was responsible for reacting UI events. Now all actions related to UI events are handled by the corresponding UI panel scripts [@yuchen]

- Removed Chibi Apes from MOFA avatar collection [@yuchen]

### Fixed

- Fixed Rescan UI panel popping up again after pressing 'Rescan' button bug [@yuchen]
