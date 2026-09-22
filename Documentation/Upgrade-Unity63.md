# Unity 6.3 / iOS 27 upgrade

## 最新进度：2026-09-22，继续执行后

- **新发现的真机问题与当前修复（待复测）：** QRealm 的首个独立签名包已安装到
  iPhone 16 Pro，但启动后 `TMP_Settings.get_defaultStyleSheet()` 连续空引用。
  生成器误写 `Text Mesh Pro`，实际目录是 `Assets/TextMesh Pro`，使三个小产品
  都漏掉 TMP 默认 Resources。已修正生成器、给三个产品补入原 GUID 的 TMP 资源，
  并新增共享产品测试断言。必须重新运行三个产品测试、Unity 导出与 Xcode 编译，
  再做真机测试；之前三个小产品的导出/编译成功只证明旧版本可构建。
- 上一轮 Xcode 27 原生构建最终成功；修复包已安装到指定的 iPhone 16 Pro
  (`00008140-001111563A0B001C`，iOS 26.0)。完整真机结果位于
  `/tmp/holokit-upgrade-20260922/iphone16pro-fixed/results.json`，附 26 张截图。
  **26/26 场景生命周期检查通过，启动错误 0，场景错误 0**；每例 AR tracking、
  host listening、共享管理器已生成、回到列表均为真。截图已人工检查 QRealm 和 Tornado。
  这仍不等于真实手势、双设备联网、录屏和光学校准均通过。
- 产品配置已转为 `Assets/Resources/HoloKitAppGlobalSettings.asset`，共享壳在运行时
  加载各产品的配置副本。HoloKit 与三个小产品各拥有游戏目录、封面和 App 图标。
  Watch 打包由产品配置控制，HoloKit 和 MOFA 启用，Typed 与 QRealm 关闭。
- 已将 21 个被多个游戏引用的渲染/特效资源连同原 GUID 移到
  `Packages/com.holoi.library.shared-assets`。原路径与新路径记录在
  `Documentation/Validation/2026-09-22/shared-asset-moves.json`。
- `Products/MOFA`、`Products/Typed`、`Products/QRealm` 是新生成的独立 Unity 项目，
  游戏目录分别 5/7/1 项、场景总数 7/9/3、引用本仓库本地包数 17/18/12。
  用户的原有兄弟项目没有被修改。生成脚本在
  `Tools/Products/generate_thin_projects.py`，拒绝覆盖已存在的产品目录。
- 主项目在配置变更后 Unity CLI 编辑器测试 **118/118 通过**，报告
  `/tmp/holokit-upgrade-20260922/product-config-tests-3.xml`。三个小项目的独立
  Unity CLI 测试现已通过：MOFA **50/50**、Typed **70/70**、QRealm **22/22**；
  报告分别为 `/tmp/holokit-upgrade-20260922/mofa-isolated-tests.xml`、
  `typed-isolated-tests-2.xml`、`qrealm-isolated-tests-2.xml`。首次 QRealm 测试
  发现生成器 `file:` 路径少了一层；首次 Typed 测试发现雕塑场景材质原先只在主项目，
  均已修复并通过重跑。
- MOFA、Typed、QRealm 三个独立项目的原生 Unity CLI iOS 导出均已成功，
  各自 `Products/<产品>/Builds/iOS/unity-build.provenance.json` 的 `outcome`
  均为 `success`。QRealm 和 Typed 的 Xcode 27 原生编译正在运行；三个小产品
  尚未在真机安装。生成器已排除主项目的本地 NatML/NatShare 配置，不把这些
  非产品配置复制到小项目或提交到 Git。
- 共享资源迁移后的 HoloKit 根项目再次通过 Unity CLI 编辑器测试 **118/118**，
  报告 `/tmp/holokit-upgrade-20260922/holokit-root-after-split.xml`。根项目
  新 iOS 导出也已成功，`Builds/Upgrade/HoloKit-four-products/unity-build.provenance.json`
  的 `outcome` 为 `success`。其新导出尚未做 Xcode 编译与真机回归。

下一步确认三个独立产品使用原生
`unity build Products/<产品> --profile iOS --output-path ...` 分别导出，
用 Xcode 签名编译并在 iPhone 16 Pro 上安装验证。完成所有产品后，再重建当前
HoloKit 根项目并做回归，因为根项目的产品资源归属也已变化。

## 续接记录：2026-09-22，用户要求及时保存进度

**先读本节再继续。项目尚未完成四 App 拆分；不要把编辑器测试通过或旧包真机
启动成功写成四个产品全部验收通过。**

- 工作目录：`/Users/amber/Projects/HoloKit/HoloKit2/holokit-app`。
- 工作分支：`upgrade/unity63-ios27-four-apps`。用户原始改动已保存为
  `72434830`；场景引用修复和第一批测试为 `ae12da52`；旧设置迁移为 `7cd5d197`。
  本文与本轮运行时修复会一起提交；用 `git log -1` 获取本次保存的提交。
- 用户明确要求使用 Unity CLI，尤其 `/Users/amber/.unity/bin/unity test`；
  游戏测试放在各自 package 内。不要恢复已删除的 `Assets/Editor/HoloKitProductBuild.cs`，
  也不要改用 Unity MCP。构建使用原生 `--profile iOS`。
- **当前指定真机：iPhone 16 Pro，Holo’s iPhone 16 J，iOS 26.0 (23A341)，
  UDID `00008140-001111563A0B001C`，型号代码 `iPhone17,1`。**
  之前使用的 iPhone 15 Pro / iOS 27 不是当前主测试机。iPhone 18 Pro 尚未到手。
- 用户不希望反复确认权限；已授权的常规构建、测试、修复和保存直接继续。
  当前沙盒限制仍由环境控制，不能声称已关闭；已有 Unity/Xcode/devicectl/Git 命令许可。

### 已验证及当前正在运行的工作

1. 最新 Unity CLI 测试 **115/115 passed，0 failed，0 skipped**。
   包括 106 个游戏场景检查、8 个设置迁移检查，以及 1 个进入 Play Mode 的
   本机主机/客户端测试（共享服务生成、复用实例、无重复、远端销毁）。
   报告：`/tmp/holokit-upgrade-20260922/iphone16-regressions.xml`。
2. iPhone 16 Pro 上旧设置迁移与封面列表启动成功。修复前完整真机基线跑完
   13 场景 × Player/Spectator = 26 例，26 例均 tracking、host listening、返回列表；
   但日志共有 569 次错误，**该基线不通过**。
3. 本轮已修复 HTTP 启动请求、NGO 未注册共享管理器、QuantumRealm 手部深度引用、
   QuantumRealm/Flock/Rain 音频引用、Tornado 不存在的 CameraForward 参数写入。
   NGO 客户端使用 prefab handler 复用预先创建的实例，避免重复服务。
4. 最新修复的 Unity iOS 导出已成功，2026-09-22 11:17:45 UTC 完成：
   `Builds/Upgrade/HoloKit/unity-build.provenance.json` 的 outcome 为 success。
   导出日志：`/tmp/holokit-upgrade-20260922/holokit-iphone16-fixes-export.log`。
5. **保存本记录时 Xcode 原生编译仍在进行；尚未确认成功，尚未安装本轮修复包，
   尚未重新跑真机 26 例。** 当前执行会话为 `81150`（仅本会话可用），
   xcodebuild 最初 PID 为 `34254`。后续须先检查进程/结果，避免并行重复构建。
   结果目录：`/tmp/holokit-upgrade-20260922/holokit-iphone16-fixes.xcresult`。
   曾打印旧 Xcode 账号缺少 token 的提示，不能据此认定构建失败；以终态为准。

### 立即续接步骤与命令

先检查上述 Xcode 构建是否结束及其结果。产物路径一直复用，**旧的 .app 存在不代表
这轮构建成功**。若需要重跑，使用新的、不存在的 `-resultBundlePath`：

```sh
xcodebuild -project Builds/Upgrade/HoloKit/Unity-iPhone.xcodeproj \
  -scheme Unity-iPhone -configuration ReleaseForRunning \
  -destination generic/platform=iOS \
  -derivedDataPath /tmp/holokit-upgrade-20260922/xcode-holokit \
  -resultBundlePath /tmp/holokit-upgrade-20260922/holokit-iphone16-fixes-retry.xcresult \
  CODE_SIGN_STYLE=Automatic DEVELOPMENT_TEAM=KR9H35SQQ9 \
  -allowProvisioningUpdates -quiet
```

确认该轮成功后安装并运行；这会替换当前 HoloKit 开发包并终止现有 app 进程：

```sh
xcrun devicectl device install app --device 00008140-001111563A0B001C \
  /tmp/holokit-upgrade-20260922/xcode-holokit/Build/Products/ReleaseForRunning-iphoneos/HoloKit.app
xcrun devicectl device process launch --device 00008140-001111563A0B001C \
  --terminate-existing --console --environment-variables '{"HOLOKIT_SCENE_TEST":"1"}' \
  com.holoi.holokit.holokit-app --holokit-scene-test
```

等日志出现 `[DEVICE TEST] COMPLETE=True`，约 4–6 分钟。console 会话在测试完后仍可能
保持打开；不要等待 app 自行退出。测试运行后 app 会留在封面列表。复制新报告与截图：

```sh
xcrun devicectl device copy from --device 00008140-001111563A0B001C \
  --domain-type appDataContainer --domain-identifier com.holoi.holokit.holokit-app \
  --source Documents/HoloKitSceneTests \
  --destination /tmp/holokit-upgrade-20260922/iphone16pro-fixed
```

验收 `complete=true`、26 个结果、所有 `errors` 和 `startupErrors` 为空，
以及新增的 `multiplayerManagerSpawned=true`。检查截图，发现错误就继续修复。
目前这些测试不证明真实手势、双手机 MPC、Watch、录屏音轨或头显光学校准已经通过。

### 证据、保护范围与后续大项

- 修复前真机 JSON 与 26 张截图：
  `/tmp/holokit-upgrade-20260922/iphone16pro-baseline/`。
  型号确认：`iphone16pro-details.json`、`iphone16pro-before-install.png`，同一临时根目录。
- 仓库内持久摘要：`Documentation/Validation/2026-09-22/iphone16-progress.json`。
  原始 XML、xcresult、截图体积较大，仍在上述 `/tmp`，系统清理后可能消失。
  真实旧设置 fixture `device-legacy-settings.save` 是用户数据，禁止提交进 Git。
- 现有未跟踪的 `mono_crash.*`、`holokit-app.slnx`、Watch Xcode userdata
  都没有纳入此次提交，不删除、不覆盖。兄弟项目 mofa-realverse、typed-realverse
  有用户本地改动，不直接改写它们。
- Unity 构建过程中会暂时删除 XR simulation resources、修改图像库与 ProjectSettings；
  成功结束时恢复。不要把构建中的临时变化误提交。
- 四薄项目、产品自己的配置/封面/目录、共享壳解耦、Watch 按产品启用：
  **仍是接下来要实现的架构，不是已完成内容。**
- 已做只读资源闭包分析：`MOFA-asset-closure.json`、`Typed-asset-closure.json`、
  `QRealm-asset-closure.json` 位于临时根目录。MOFA/Typed/QRealm 之间仍有共享
  skybox、renderer、粒子纹理/VFX 对其他游戏包的依赖；应保留 GUID 提取到共享资源包，
  再证明每个小产品不安装其他游戏包也能构建。不能只过滤根项目 scene list 就声称解耦。
- HoloKit SDK 当前资料包含 iPhone 16 Pro，iPhone 18 Pro 的光学校准不能凭空编造；
  等硬件与校准数据可用再做物理验证。

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
4. Validate iPhone 18 Pro with the real hardware when available. Initial checks
   used Holo iPhone 15 I (iPhone 15 Pro, iOS 27). At the user's request, subsequent
   scene testing targets Holo's iPhone 16 J (iPhone 16 Pro, iOS 26.0), UDID
   `00008140-001111563A0B001C`. The Xcode/iOS SDK remains 27; device runtime
   versions must be reported separately. A simulator cannot prove AR/optical
   correctness.

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
command-line argument forwarding. The migration was verified on the user's
iPhone 16 Pro (`Holo’s iPhone 16 J`, iOS 26.0): the old settings loaded, the
original binary was retained, and the gallery appeared.

### iPhone 16 Pro baseline

The first full device run completed all 13 catalog scenes as both Player and
Spectator (26 cases). All reached AR `SessionTracking`, started a local host,
and returned to the gallery. It was **not a passing run**: 569 logged errors
included repeated instances of an unregistered multiplayer service, missing
audio/depth references, and a Tornado feeder writing a nonexistent VFX property,
plus an insecure HTTP request at startup. The report and 26 screenshots are
preserved outside Git at
`/tmp/holokit-upgrade-20260922/iphone16pro-baseline/`.

The device OS is iOS 26.0; the build toolchain remains Xcode 27 with the iOS 27
SDK. These are separate compatibility checks.

The corresponding fixes register the shared multiplayer prefab and reuse the
pre-instantiated client service through an NGO prefab handler, restore the
existing audio/depth component references, remove Tornado's obsolete direction
feeder, and use HTTPS for the startup permission request. The expanded Unity
CLI run passed **115/115 tests, zero skipped**, including a loopback host/client
spawn-and-despawn test and package-local checks for the device-discovered
references. Evidence: `/tmp/holokit-upgrade-20260922/iphone16-regressions.xml`.
The rebuilt player still needs the full device rerun.

This document describes the intended architecture and work in progress, not a
claim that the four apps have built or run successfully.
