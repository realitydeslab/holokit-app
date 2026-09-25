# MOFA Training 双目实测，2026-09-25

版本：MOFA 开发版，Unity 导出源提交 `3afd1d58`；iPhone 15 I
(`00008130-000848EA38298D3A`，iOS 27.0)。运行包内
`Tests/Device/DeviceMofaCombatTests.cs`，环境变量
`MOFA_COMBAT_TEST=1`、`MOFA_COMBAT_STEREO_TEST=1`。

阶段报告 `output/validation-20260925/stereo-training-I/results.json`：

- `complete=true`，`renderMode=Stereo`，`trackingDeviceFound=true`，
  `arTracking=true`，`hostListening=true`，运行错误 0。
- 双目 Star 触发组件确实按下/松开，进入 Fighting。模拟 Apple Watch 的手机端
  射击事件使本机施法计数 0→1；AI 自行施法 1 次。这不验证实体 Watch 的动作识别。
- `aiSpawned=true`、`aiWeaponPresent=true`、`aiFallbackMaterials=14`。
  这些是运行时对象状态，不能单独证明身体或魔杖在画面中可见。
- 本轮没有可选的 floor 平面，测试显式启用场景的默认回退落点：
  `centerEyeY=0.00124 m`，`placementY=aiOriginY=-0.59876 m`。

人工查看 `fighting-before-shot.png`、`watch-shot-1.png`、`ai-attack.png`：
双目画面显示 UI 和法术光效，却没有可辨认的 AI 身体或魔杖；视觉验收失败。
双目黑底本身符合该 SDK 在 Stereo 模式关闭 `ARCameraBackground`、使用光学透视
的实现，不能单凭黑底判断相机故障。包内测试增加了 AI 的 SkinnedMeshRenderer
bounds、相机距离和各活动相机的 viewport 坐标。新版在 Crack B
(`00008130-001168E428EB8D3A`) 的真机报告显示身体网格
`aiBodyRenderers=1`、bounds 高约 1.75 m、距中心眼约 6.55 m，但左右眼
viewport 中心 x 分别约 2.46、2.43，明显超出 0–1 的可见范围。因此该轮的
缺席至少涉及落点与相机视野不一致；材质是否在视野内正确渲染仍待复测。

共享 `ARPlacementManager` 默认落点原来用 `centerEyePose.eulerAngles.y` 构造
水平偏移。双目手机横放后可同时带 pitch 和 roll，此 Euler Y 不保证等于相机
forward 的水平朝向。代码已改为投影相机真实 forward 到水平面，并让 floor
命中分支用同一朝向；Training 包内新增横放旋转测试，Unity CLI 定向测试通过
（`output/validation-20260925/mofa-rolled-placement-editor.xml`）。
修复版 `a9f87263` 已在 iPhone 15 Crack B 重新编译并实测：
`output/validation-20260925/stereo-training-B-horizontal/results.json`
记录左右眼 viewport x 仍为 2.42、2.39，截图仍看不到对手。因此该修改
**没有解决真机画面问题**。它的横放朝向单元测试通过，但不能据此宣称视觉修复。
包内真机测试现增加放置、开战、AI 攻击时的中心眼和各相机 pose/落点投影记录，
诊断包已通过 MOFA 包内 12/12 个 Unity CLI 定向 Editor 测试、Unity iOS 导出
和 Xcode 签名构建，但在 Crack B 连续两次启动后，ARSession 均未在 30 秒内进入
Tracking；测试在放置前中止，未取得 pose 对比。两次原始报告保存在
`output/validation-20260925/stereo-training-B-pose-diagnostics/` 与
`output/validation-20260925/stereo-training-B-pose-diagnostics-retry/`。
需在 AR tracking 恢复后重跑，区分放置时相机方向、战斗期间追踪姿态变化及
投影配置问题。

后续在 Crack B 上用 `xcrun devicectl device orientation set ... landscapeLeft`
设置测试方向后，AR Tracking 恢复，`stereo-training-B-landscape-pose` 报告
完成真实 Fighting 流程（模拟 Watch 施法 0→1，AI 施法 1，运行错误 0）。
截图依旧看不到对手。放置时 `Center Eye Pose.y≈−0.199 m`，AR `Main Camera.y≈
0.918 m`，两者相差约 1.12 m；放置到 AI 攻击期间该差值持续存在。落点
`(-5.69,−0.80,−1.47)`；AI 身体中心 `(-6.28,0.08,−0.62)`，左右眼
viewport x≈2.30、2.28，仍在画外。这证明了相机/眼睛姿态不一致，
但单次报告不能断定差值全部来自同一种坐标错误。报告中的相机层级采样只
包含 AR 主相机，下一版已修正为记录左右眼、中心眼本地位姿与屏幕方向。

对照同机 `holokit-unity-sdk` 的 `Runtime/HoloKitCameraManager.cs` 和
`Runtime/LowLatencyTrackingManager.cs`，新版 SDK 将低延迟追踪写到
`Stereo Tracked Pose` 父节点、保留子节点 `Center Eye Pose` 的相机到眼睛校准
偏移。此项目此前直接在每帧写 `Center Eye Pose` 世界位姿，覆盖了
`SetupHoloKitCameraData` 写入的本地偏移。现已按该层级修改共享相机 prefab
与追踪驱动；MOFA Training 包内对应 Editor 测试 13/13 通过，MOFA iOS
导出和 Xcode 签名构建成功，构建已安装到 Crack B 和 I。两台手机此时锁屏，
**新层级的真机画面尚未验收**。

随后在同一台 Crack B 安装该签名包并重新运行包内真机战斗测试，报告
`output/validation-20260925/stereo-training-B-tracked-parent/results.json`：
AR `SessionTracking`、屏幕与设备均为 `LandscapeLeft`；左右眼 AI 身体
viewport 中心 x≈0.495、0.484，y≈0.411，均在可见范围。中心眼本地偏移
`(0.0399,−0.0615,−0.0781) m` 在放置和战斗阶段保持一致。
`fighting-before-shot.png`、`ai-attack.png` 均实际显示双目 AI 身体，
先前的“完全没有小人”已在本轮复测中消失。模拟 Watch 施法 0→1、AI 施法 1、
运行错误 0。不过人物姿态、大小、法术特效遮挡以及手中魔杖的独立可辨性
仍需继续视觉验收；`aiWeaponPresent=true` 只证明实例存在。仍无 floor 平面，
本轮使用默认回退落点，不能据此确认真实地板高度。

同一共享修改后的 Unity CLI EditMode 全套结果：MOFA 59/60、Typed 74/75、
QRealm 26/27、HoloKit 根项目 127/128。四套各自唯一失败均为
`SerializedAssetReferenceTests.PlayableProductAssetGuidsResolve`，报告在
`output/validation-20260925/*-after-stereo-parent-editor.xml` 与
`mofa-stereo-pose-parent-full-editor.xml`；这些失败仍须按资源 GUID 清单修复，
不能将回归测试记为全部通过。

高度参考：Training 场景默认偏移为 `{x:0,y:-0.6,z:6}`，单位为米。Apple 的
[ARKit gravity 世界对齐文档](https://developer.apple.com/documentation/arkit/arconfiguration/worldalignment-swift.enum/gravity)
指出原点是会话启动时的设备位置，而非地板；
[setWorldOrigin 文档](https://developer.apple.com/documentation/arkit/arsession/setworldorigin%28relativetransform%3A%29)
说明可在找到参考点后显式重定义原点。同机 SDK 仓库的 README “Reset World
Origin” 和 `Runtime/iOS/WorldOriginResetter.cs` 提供显式接口；提交 `5bc5b40`
另修复 Unity 6 双目追踪设备筛选，不负责 floor 高度。当前回退落点仅适于
测试战斗路径，不能当作真实地板的替代测量。
