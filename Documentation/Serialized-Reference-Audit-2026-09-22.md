# Playable serialized-reference audit — 2026-09-22

## 2026-09-25: controller dead objects and legacy Unity UI

The next pass reduced both root and independent MOFA audits from **132
occurrences / 21 GUIDs** to **4 occurrences / 3 GUIDs**. In Training's
`AC_MofaTraining_Avatar.controller`, four broken transitions were not referenced
by any state and used parameters absent from the controller. Each of the two
Hunting Dragon controllers had another 51 unreferenced subassets carrying 58
broken GUID occurrences. These objects were removed only after confirming
their local file IDs occur nowhere else in their controller; reachable states,
transitions, and animation clips remain serialized.

Fourteen Malbers Mobile Input prefabs still referenced an old, absent Unity UI
DLL GUID. Their serialized fields identify seven current Unity UI components:
Image, Button, Text, EventTrigger, Shadow, CanvasScaler, and GraphicRaycaster.
All 61 old script references were retargeted to the matching Unity 6 UGUI
source-script GUIDs. Hunting package Editor tests verify all Mobile Input
prefabs load without missing scripts and that both playable direction buttons
retain their Image, Button, Text, EventTrigger, target graphic, and two pointer
events. This removed eight occurrences from the playable dependency audit.

| Product | Full Unity CLI EditMode suite | Strict unresolved GUIDs | Report |
| --- | ---: | ---: | --- |
| MOFA | **62/63** | **4 occurrences / 3 GUIDs** | `output/validation-20260925/mofa-mobile-input-migration-full-editor.xml` |
| root | **130/131** | **4 occurrences / 3 GUIDs** | `output/validation-20260925/root-mobile-input-migration-full-editor.xml` |

Each suite has one failure: `PlayableProductAssetGuidsResolve`. The remaining
GUIDs are `f343d204…` (two FireBall material references), `10ff51d2…`
(FireBall Core), and `d9df6ed1…` (HarryPotter LifeShield). These are actual
texture inputs in the current materials; the LifeShield Shader Graph also
embeds `d9df6ed1…` as the texture slot's default. Exact original `.meta`
assets were not found in the nearby source checkouts or this repository's Git
history. They remain unresolved rather than being removed or guessed. The
currently installed iOS app predates this controller/UI cleanup.

## 2026-09-25: Dragon source recovery and physical GUID audit

The latest Unity CLI audit checks that a GUID resolves to a file or directory
that actually exists in the product. The prior `AssetDatabase.GUIDToAssetPath`
check could report stale Library mappings as valid after a package asset was
removed. `SerializedAssetReferenceTests` now verifies physical package paths;
this exposes missing controller GUIDs in the independent MOFA product rather
than hiding them. Typed and QRealm still pass this stricter audit, **2/2 each**
(`output/validation-20260925/typed-physical-guid-audit-editor.xml` and
`qrealm-physical-guid-audit-editor.xml`).

Exact original assets and `.meta` GUIDs were found in sibling HoloKit source
checkouts. **26** missing Dragon/Wolf animations, FBXs, and particle textures
were restored to the Hunting package with their original GUIDs. Three original
third-party animator controllers were deliberately left out: importing them
created 489 additional unresolved serialized dependencies, with no local
originals found for that larger chain. The gameplay Dragon controller's
recovered animation clips now resolve; the thumbnail controller and Training
AI controller still contain unresolved transitions.

The absent texture GUID `5044169c…` appeared 125 times as old named texture
overrides across 25 prefabs/scenes. The current `V_Flare_Explosion` and
`V_Flare_Follow` graphs no longer expose those property names, so those
obsolete overrides were removed. Five more absent texture references were
removed only where the current material shader/graph no longer uses their
serialized property: Thunder LifeShield (1), Hunting Portal (1), and the
Stag's `_BaseMap`/`_MainTex`/`_BumpMap` (3). Current shader inputs and other
missing material textures were preserved.

| Product | Earlier strict audit | Current strict audit | Evidence |
| --- | ---: | ---: | --- |
| MOFA | 333 occurrences / 52 GUIDs | **132 / 21**, 1/2 tests | `output/validation-20260925/mofa-final-guid-audit-editor.xml` |
| root | 333 occurrences / 52 GUIDs | **132 / 21**, 1/2 tests | `output/validation-20260925/root-final-guid-audit-editor.xml` |
| Typed | 0 | **0**, 2/2 tests | `output/validation-20260925/typed-physical-guid-audit-editor.xml` |
| QRealm | 0 | **0**, 2/2 tests | `output/validation-20260925/qrealm-physical-guid-audit-editor.xml` |

The remaining MOFA references include the thumbnail animator's missing
transitions, the Training AI controller's four missing transition references,
the Mobile Input DLL GUID, and material textures still used by their current
shaders. These are recorded as unresolved rather than guessed replacements.
The installed iOS apps predate these source edits, so this audit is not a
physical-device visual pass.

Final full EditMode runs on this source: MOFA **59/60** and root **127/128**
(their only failure is this strict GUID audit); Typed **75/75** and QRealm
**27/27**. XML reports are `output/validation-20260925/{mofa,root,typed,qrealm}-final-reference-full-editor.xml`.

Run from each Unity product with `/Users/amber/.unity/bin/unity test <project> --mode EditMode --filter HoloKit.Tests.SerializedAssetReferenceTests`. The test checks GUID resolution across enabled scenes and their asset dependencies, and separately checks prefab missing scripts. Every row below is a GUID that Unity cannot resolve in that product; counts are serialized occurrences, not necessarily separate runtime failures.

| Product | Unresolved occurrences | Distinct GUIDs | Source files | Missing-script test |
| --- | ---: | ---: | ---: | --- |
| root | 359 | 67 | 41 | Passed |
| mofa | 365 | 61 | 36 | Passed |
| typed | 20 | 13 | 11 | Passed |
| qrealm | 11 | 10 | 4 | Passed |

The MOFA AI avatar collection null entry was fixed earlier. This audit additionally found a missing debug component in `Death Circle` and two Hunting Portal meshes absent from the MOFA product; those were repaired before the counts above. Twelve broken UI Button sprite-state pointers were cleared because their buttons use Color Tint (`m_Transition: 1`). Existing unresolved GUIDs remain and need visual or gameplay review before being replaced or removed.

## 2026-09-25: recovered original assets from Git and local LFS

The old removed typography, thumbnail, and ARUX package histories still contain
several exact source assets and `.meta` GUIDs. They have been restored into the
corresponding game or shared package, preserving the asset content and GUIDs:

- Typed Sculpture's `TypoStickerManSDF.asset` (`4058f446…`) from commit `0b1812ac`.
- The QuantumRealm thumbnail controller (`1e28e33c…`) and animation FBXs
  (`32923e45…`, `c384443e…`, `59fdb1bb…`) into `com.holoi.library.shared-assets`,
  which both Typed and QRealm load. Existing QRealm files were moved with their
  `.meta`; missing FBXs were recovered from local Git LFS objects.
- ARUX hover texture `Asset 17build.png` (`ca361939…`) from `910ee8e2`.
- Typed Scanner floor textures (`6eb205c4…`, `a3b7830…`) from `7c0f05e0`.

Unity CLI `SerializedAssetReferenceTests` no longer lists those GUIDs. Strict
remaining counts (occurrences / distinct GUIDs / source files) are:

| Product | Earlier | Current | Current report |
| --- | ---: | ---: | --- |
| root | 359 / 67 / 41 | 350 / 61 / 35 | `output/validation-20260925/root-restored-guid-final-editor.xml` |
| mofa | 365 / 61 / 36 | 365 / 61 / 36 | `output/validation-20260925/mofa-restored-guid-final-editor.xml` |
| typed | 20 / 13 / 11 | 12 / 8 / 6 | `output/validation-20260925/typed-restored-floor-textures-editor.xml` |
| qrealm | 11 / 10 / 4 | 8 / 7 / 2 | `output/validation-20260925/qrealm-restored-hover-texture-editor.xml` |

The audit still fails in all four products. These counts are serialized
references, not a claim that every remaining GUID causes a runtime failure;
the unresolved Dragon animation, MOFA Training controller, VFX materials, and
URP renderer resource pointers need separate assessment.

The MOFA Training AI uses `AC_MofaTraining_Avatar.controller`, which still has four transitions to missing GUID `74535b8bda892f4428e70719b7801c63`. These may affect attack animation events; physical-device combat validation is pending. The large number of Dragon thumbnail controller references also means a passed scene-open test cannot establish that every animation renders correctly.

## 2026-09-25: enabled Typed scenes and obsolete URP fields

Four enabled Typed scenes referenced the absent Lighting Settings GUID
`4e030e48131472843ac21feecf4a46a2`. They now use the existing
`Typo Default.lighting` (`edc2bd7dccbb846e482c1e69de9a6afa`) with the same
Unity file ID; other Typed scenes already used this asset. The seven missing
GUIDs in the product renderer data were stored only in URP fields marked
obsolete in the installed Unity 6.3 URP package: APV probe debug resources
and `xrSystemData`, both moved to Graphics Settings. Those stale serialized
entries were removed from the project's five renderer assets, not suppressed
in the audit test.

The VFX mesh `SM_MF_Flash_2.fbx` and its original `.meta`/GUID
`65588ce964fcf45aca098c8d81bd4661` moved from root `Assets/VFX` into
`org.realitydeslab.mofa.library.base`; this makes the same mesh available in
the independent MOFA product. Its 20 missing MOFA references, including
Dragon Fireball and spell effects, now resolve.

| Product | Latest unresolved occurrences | Unity CLI evidence |
| --- | ---: | --- |
| root | 336 | `output/validation-20260925/root-restored-flash-mesh-editor.xml` (GUID test 1/2) |
| MOFA | 336 | `output/validation-20260925/mofa-restored-flash-mesh-editor.xml` (full suite 59/60) |
| Typed | **0** | `output/validation-20260925/typed-clean-guid-full-editor.xml` (full suite **75/75**) |
| QRealm | **0** | `output/validation-20260925/qrealm-clean-guid-full-editor.xml` (full suite **27/27**) |

The three red Dragon materials also held the same missing texture GUID
`d775cd6c…` under an old generated Shader Graph sample-property name.
That property is absent from their current `SH_Dragon_Body.shadergraph`, so its
three stale material entries were removed without changing the current shader
inputs. A subsequent Unity CLI audit reports **333** unresolved occurrences in
MOFA (`output/validation-20260925/mofa-removed-stale-dragon-texture-editor.xml`)
and **333** in root
(`output/validation-20260925/root-removed-stale-dragon-texture-editor.xml`).

Root and MOFA still have unresolved project-owned references, notably the
MOFA spell texture `5044169c…` and Dragon thumbnail animator transitions.
Typed/QRealm passing the serialized audit does not prove native hand tracking
or all rendered interactions; the current signed device packages predate these
asset edits and need rebuilding before runtime acceptance.

## root

| GUID | Occurrences | First source location |
| --- | ---: | --- |
| `5044169c7ab3c50489096daebd396381` | 50 | `Packages/org.realitydeslab.mofa.library.base/Assets/Prefabs/Spells/13_Wind_Basic.prefab:3151` |
| `03d36e925ad2a46408710ac096feb6e6` | 34 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4378` |
| `e834e2e4ac347f8449f277aa1bc559bc` | 34 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4575` |
| `91d46eed503eb344dae9b2fd274c4bd9` | 16 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4719` |
| `13e7de80e5b7bad4ba411c7d1ee9252d` | 14 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2969` |
| `5ebaf492aebfd62409b0523888120176` | 10 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2953` |
| `9833f505934f6404a9b403f047305aa8` | 10 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2649` |
| `f70555f144d8491a825f0804e09c671c` | 8 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Prefabs/Mobile Input/Mobile B Down.prefab:60` |
| `0c5e41c01450a3e4c8c131035f711320` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2930` |
| `18274f5b0123bb549966621255abd943` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2844` |
| `1d24328b68ea0bb4baabb59fb912f7f6` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3008` |
| `2e2b192bce6812542930043b366d3d21` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2860` |
| `311cdef313e09624faf13475db2eddb5` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2688` |
| `329f7ce9761325a4b9508cb77966acc0` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3383` |
| `37925afb7dcf9064cb3c4f9d22fb28b8` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3594` |
| `3cb2b1fb9ca9d1d49b95cc890f678cd6` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3563` |
| `553ba19a700be794d9b09b3e32584031` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2961` |
| `74f10383b898a8b41891f883bc6c3ac5` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3391` |
| `920543dcda2bfe649a82fa83363c6453` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3555` |
| `94429e923d495cc42b89a79e590f91cc` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3078` |
| `9a047427df9a37f428634f1cee9a3117` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2665` |
| `9ca7c0b4fa24b0b40a2f876bcd3974a4` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3586` |
| `c0c3be63a7380c8459ae064f00417374` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3461` |
| `e68f885be2e80b943a16b7275d0a6b93` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:5088` |
| `f427a45de99469a4db86a980cff8db7c` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2922` |
| `21c2d4837e660154a886a421680ef613` | 4 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:9687` |
| `4e030e48131472843ac21feecf4a46a2` | 4 | `Packages/org.realitydeslab.typed.the-finger-ribbon/Assets/Scenes/TypedTheFingerRibbon.unity:97` |
| `5a256c0e9bac24f4aa2204ccf1ced1e7` | 4 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2160` |
| `60e1133243b97e347b653163a8c01b64` | 4 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:31` |
| `74535b8bda892f4428e70719b7801c63` | 4 | `Packages/org.realitydeslab.mofa.the-training/Assets/ArtResources/Animator Controllers/AC_MofaTraining_Avatar.controller:553` |
| `d775cd6c0ceca45a8a63ae1f9959d8c2` | 3 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/ArtResources/Materials/M_DragonBody_RED.mat:57` |
| `0cc0c2377c13efa4c8052e91f35f0670` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3453` |
| `0f41ebad585600c40a0e963e6261f83d` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3727` |
| `30e27d6a229335148ad1d1363901a95b` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3829` |
| `323ea1098cd055641860d0ee90b07727` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3837` |
| `38333f68f68dd0444b54d3625088cf02` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3532` |
| `39c0a1a0747f73d48a186742af267efb` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3759` |
| `3bc01580ce4f1834eb3562397607f51d` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall.mat:50` |
| `4058f4469476b4cb09037c723756ee0c` | 2 | `Assets/Covers/TypedTheSculpture.prefab:270` |
| `423afdfddf1a27041a8be2f6614d9d7c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3524` |
| `5843a9d4cf100654caeaf363d3441694` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3751` |
| `6eb205c49be57459e8660cdc00f84cc4` | 2 | `Packages/org.realitydeslab.typed.the-scanner/Assets/ArtResources/Materials/M_PrismText_3_Floor.mat:32` |
| `7914218e71cf3504cb1824e7a49930ce` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3500` |
| `a3b78302d902d4c318a6d0afb113b542` | 2 | `Packages/org.realitydeslab.typed.the-scanner/Assets/ArtResources/Materials/M_PrismText_3_Floor.mat:36` |
| `c3e581a01152d84439944ead2bad0451` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/Trail.mat:23` |
| `c5867f61036914cc19af1bd9512ad2e2` | 2 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Secondary Spell/M_HarryPotter_Secondary_Stag.mat:28` |
| `ca3f0f85b3bb98b4b971ad382db34512` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:650` |
| `d6b4fa25f1843b54496dae81ab4caf90` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3508` |
| `e203b0f00c3a5114896fae503f41e04f` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:10361` |
| `ec9a8b702a1532d4aa4ce9efe8641a01` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3813` |
| `f343d204ab60b284597aa1742824864c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall.mat:22` |
| `f9d1c10e2b9b7ce4e8ead27bc60947e1` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3516` |
| `fce85fe0aee6b5046b15712c26e3ca1c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3821` |
| `03cfc4915c15d504a9ed85ecc404e607` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:21` |
| `10ff51d2d87fb7b46b70b55f8551c146` | 1 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall Core.mat:22` |
| `24ec0e140fb444a44ab96ee80844e18e` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:25` |
| `2df14f415947d6c48a18146793b7b790` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/2_Thunder/LifeShield/M_Thunder_LifeShield.mat:36` |
| `32923e45f5f374754b8abff583040df9` | 1 | `Packages/org.realitydeslab.quantum-realm/Assets/ArtResources/Thumbnails/AC_QuantumRealm_Meebits.controller:23` |
| `53a11f4ebaebf4049b3638ef78dc9664` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:22` |
| `55eece6eca2f74860ab78c331d890f86` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Secondary Spell/M_HarryPotter_Secondary_Stag.mat:32` |
| `57d7c4c16e2765b47a4d2069b311bffe` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:24` |
| `59fdb1bbaa31d46ee8e7479fe05ebea5` | 1 | `Packages/org.realitydeslab.quantum-realm/Assets/ArtResources/Thumbnails/AC_QuantumRealm_Meebits.controller:97` |
| `7c41f9e9dea01cb49b64034f9e7bed86` | 1 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/ArtResources/Portal/M_PortalInner.mat:49` |
| `8f96cd657dc40064aa21efcc7e50a2e7` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:23` |
| `ca3619397e60545d98a45df3f623f224` | 1 | `Packages/com.holoi.library.arux/Assets/HoverButton/Shader Graphs_load text -3.mat:40` |
| `d9df6ed1a4283b5498fd5c4125aa651c` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Life Shield/M_HarryPotter_LifeShield.mat:73` |
| `e5c6678ed2aaa91408dd3df699057aae` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:20` |

## mofa

| GUID | Occurrences | First source location |
| --- | ---: | --- |
| `5044169c7ab3c50489096daebd396381` | 50 | `Packages/org.realitydeslab.mofa.library.base/Assets/Prefabs/Spells/13_Wind_Basic.prefab:3151` |
| `03d36e925ad2a46408710ac096feb6e6` | 34 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4378` |
| `e834e2e4ac347f8449f277aa1bc559bc` | 34 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4575` |
| `65588ce964fcf45aca098c8d81bd4661` | 20 | `Packages/org.realitydeslab.mofa.library.base/Assets/Prefabs/Spells/11_AceFire_Basic.prefab:420` |
| `91d46eed503eb344dae9b2fd274c4bd9` | 16 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:4719` |
| `13e7de80e5b7bad4ba411c7d1ee9252d` | 14 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2969` |
| `5ebaf492aebfd62409b0523888120176` | 10 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2953` |
| `9833f505934f6404a9b403f047305aa8` | 10 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2649` |
| `f70555f144d8491a825f0804e09c671c` | 8 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Prefabs/Mobile Input/Mobile B Down.prefab:60` |
| `0c5e41c01450a3e4c8c131035f711320` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2930` |
| `18274f5b0123bb549966621255abd943` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2844` |
| `1d24328b68ea0bb4baabb59fb912f7f6` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3008` |
| `2e2b192bce6812542930043b366d3d21` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2860` |
| `311cdef313e09624faf13475db2eddb5` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2688` |
| `329f7ce9761325a4b9508cb77966acc0` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3383` |
| `37925afb7dcf9064cb3c4f9d22fb28b8` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3594` |
| `3cb2b1fb9ca9d1d49b95cc890f678cd6` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3563` |
| `553ba19a700be794d9b09b3e32584031` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2961` |
| `74f10383b898a8b41891f883bc6c3ac5` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3391` |
| `920543dcda2bfe649a82fa83363c6453` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3555` |
| `94429e923d495cc42b89a79e590f91cc` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3078` |
| `9a047427df9a37f428634f1cee9a3117` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2665` |
| `9ca7c0b4fa24b0b40a2f876bcd3974a4` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3586` |
| `c0c3be63a7380c8459ae064f00417374` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3461` |
| `e68f885be2e80b943a16b7275d0a6b93` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:5088` |
| `f427a45de99469a4db86a980cff8db7c` | 6 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2922` |
| `21c2d4837e660154a886a421680ef613` | 4 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:9687` |
| `5a256c0e9bac24f4aa2204ccf1ced1e7` | 4 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:2160` |
| `74535b8bda892f4428e70719b7801c63` | 4 | `Packages/org.realitydeslab.mofa.the-training/Assets/ArtResources/Animator Controllers/AC_MofaTraining_Avatar.controller:553` |
| `60e1133243b97e347b653163a8c01b64` | 3 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:31` |
| `d775cd6c0ceca45a8a63ae1f9959d8c2` | 3 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/ArtResources/Materials/M_DragonBody_RED.mat:57` |
| `0cc0c2377c13efa4c8052e91f35f0670` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3453` |
| `0f41ebad585600c40a0e963e6261f83d` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3727` |
| `30e27d6a229335148ad1d1363901a95b` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3829` |
| `323ea1098cd055641860d0ee90b07727` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3837` |
| `38333f68f68dd0444b54d3625088cf02` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3532` |
| `39c0a1a0747f73d48a186742af267efb` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3759` |
| `3bc01580ce4f1834eb3562397607f51d` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall.mat:50` |
| `423afdfddf1a27041a8be2f6614d9d7c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3524` |
| `5843a9d4cf100654caeaf363d3441694` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3751` |
| `7914218e71cf3504cb1824e7a49930ce` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3500` |
| `c3e581a01152d84439944ead2bad0451` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/Trail.mat:23` |
| `c5867f61036914cc19af1bd9512ad2e2` | 2 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Secondary Spell/M_HarryPotter_Secondary_Stag.mat:28` |
| `ca3f0f85b3bb98b4b971ad382db34512` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:650` |
| `d6b4fa25f1843b54496dae81ab4caf90` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3508` |
| `e203b0f00c3a5114896fae503f41e04f` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:10361` |
| `ec9a8b702a1532d4aa4ce9efe8641a01` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3813` |
| `f343d204ab60b284597aa1742824864c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall.mat:22` |
| `f9d1c10e2b9b7ce4e8ead27bc60947e1` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3516` |
| `fce85fe0aee6b5046b15712c26e3ca1c` | 2 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/Prefabs/Thumbnail/Poly Dragon Animator.controller:3821` |
| `03cfc4915c15d504a9ed85ecc404e607` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:21` |
| `10ff51d2d87fb7b46b70b55f8551c146` | 1 | `Packages/org.realitydeslab.mofa.the-hunting/Runtime/Plugins/Malbers Animations/Common/Particles/Materials/FireBall Core.mat:22` |
| `24ec0e140fb444a44ab96ee80844e18e` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:25` |
| `2df14f415947d6c48a18146793b7b790` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/2_Thunder/LifeShield/M_Thunder_LifeShield.mat:36` |
| `53a11f4ebaebf4049b3638ef78dc9664` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:22` |
| `55eece6eca2f74860ab78c331d890f86` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Secondary Spell/M_HarryPotter_Secondary_Stag.mat:32` |
| `57d7c4c16e2765b47a4d2069b311bffe` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:24` |
| `7c41f9e9dea01cb49b64034f9e7bed86` | 1 | `Packages/org.realitydeslab.mofa.the-hunting/Assets/ArtResources/Portal/M_PortalInner.mat:49` |
| `8f96cd657dc40064aa21efcc7e50a2e7` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:23` |
| `d9df6ed1a4283b5498fd5c4125aa651c` | 1 | `Packages/org.realitydeslab.mofa.library.base/Assets/ArtResources/MOFA Spells/3_HarryPotter/Life Shield/M_HarryPotter_LifeShield.mat:73` |
| `e5c6678ed2aaa91408dd3df699057aae` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:20` |

## typed

| GUID | Occurrences | First source location |
| --- | ---: | --- |
| `4e030e48131472843ac21feecf4a46a2` | 4 | `Packages/org.realitydeslab.typed.the-finger-ribbon/Assets/Scenes/TypedTheFingerRibbon.unity:97` |
| `4058f4469476b4cb09037c723756ee0c` | 2 | `Assets/Covers/TypedTheSculpture.prefab:270` |
| `60e1133243b97e347b653163a8c01b64` | 2 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:31` |
| `6eb205c49be57459e8660cdc00f84cc4` | 2 | `Packages/org.realitydeslab.typed.the-scanner/Assets/ArtResources/Materials/M_PrismText_3_Floor.mat:32` |
| `a3b78302d902d4c318a6d0afb113b542` | 2 | `Packages/org.realitydeslab.typed.the-scanner/Assets/ArtResources/Materials/M_PrismText_3_Floor.mat:36` |
| `03cfc4915c15d504a9ed85ecc404e607` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:21` |
| `1e28e33ca6457438ea0711b41589160f` | 1 | `Assets/Covers/TypedTheSculpture.prefab:374` |
| `24ec0e140fb444a44ab96ee80844e18e` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:25` |
| `53a11f4ebaebf4049b3638ef78dc9664` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:22` |
| `57d7c4c16e2765b47a4d2069b311bffe` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:24` |
| `8f96cd657dc40064aa21efcc7e50a2e7` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:23` |
| `ca3619397e60545d98a45df3f623f224` | 1 | `Packages/com.holoi.library.arux/Assets/HoverButton/Shader Graphs_load text -3.mat:40` |
| `e5c6678ed2aaa91408dd3df699057aae` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:20` |

## qrealm

| GUID | Occurrences | First source location |
| --- | ---: | --- |
| `60e1133243b97e347b653163a8c01b64` | 2 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:31` |
| `03cfc4915c15d504a9ed85ecc404e607` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:21` |
| `24ec0e140fb444a44ab96ee80844e18e` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:25` |
| `32923e45f5f374754b8abff583040df9` | 1 | `Packages/org.realitydeslab.quantum-realm/Assets/ArtResources/Thumbnails/AC_QuantumRealm_Meebits.controller:23` |
| `53a11f4ebaebf4049b3638ef78dc9664` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:22` |
| `57d7c4c16e2765b47a4d2069b311bffe` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:24` |
| `59fdb1bbaa31d46ee8e7479fe05ebea5` | 1 | `Packages/org.realitydeslab.quantum-realm/Assets/ArtResources/Thumbnails/AC_QuantumRealm_Meebits.controller:97` |
| `8f96cd657dc40064aa21efcc7e50a2e7` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:23` |
| `ca3619397e60545d98a45df3f623f224` | 1 | `Packages/com.holoi.library.arux/Assets/HoverButton/Shader Graphs_load text -3.mat:40` |
| `e5c6678ed2aaa91408dd3df699057aae` | 1 | `Packages/com.holoi.library.holokit-app-lib/Assets/Settings/HoloKit App UI Universal Render Pipeline Asset_Renderer.asset:20` |
