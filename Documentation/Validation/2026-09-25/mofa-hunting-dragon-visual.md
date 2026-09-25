# MOFA Hunting Dragon 召唤与画面检查，2026-09-25

设备：Holo iPhone 15 Crack B（`iPhone16,1`，iOS 27）。用 `devicectl`
启动 Hunting 包的 `Tests/Device/DeviceMofaHuntingSpawnTests.cs`，测试调用
游戏的召唤按钮，并在没有分类为 Floor 的平面时使用场景的默认放置点。

旧版测试报告 `complete=true`、`arTracking=true`、`dragonSpawned=true`、
`portalSpawned=true`、错误 0；但 `dragon-summoned.png` 中没有龙。
这张图是在对象刚生成时截取，而 `DragonController.EntranceAnimation()` 会在
前 5 秒开启身体裁切，不能据此判定动画结束后仍不可见。原始报告与截图：
`output/validation-20260925/hunting-spawn-current-B/`。

Hunting 包内设备测试现增加入口动画结束后 6 秒的截图与几何报告。Unity CLI
定向 Editor 测试 8/8 通过；Unity 6000.3.24f1 CLI 导出成功，Xcode 27
`ReleaseForRunning` 构建在补入现有开发团队号后成功，结果包：
`/tmp/holokit-hunting-visual-team-iphone.xcresult`。已将该签名包安装到 Crack B
并再次运行同一召唤流程。新版结果在
`output/validation-20260925/hunting-after-entrance-B/`：

- `complete=true`，AR tracking、host、按钮、默认落点、Dragon/Portal 生成
  均正常；`dragonReadyForControl=true`，运行错误 0。
- 入口动画结束后仍有 2 个启用的 Dragon 身体 SkinnedMeshRenderer；合并 bounds
  中心约 `(0.00, 2.26, 3.79)`，尺寸约 `(4.23, 3.73, 2.78)` 米。
- 主相机对该中心的 viewport 为 `(0.674, −1.729, z=2.236)`，位于画面
  下方；`dragon-after-entrance.png` 也看不到龙。这证明当前摆放和朝向下的
  视觉验收失败，但不能仅凭中心点投影断定所有机位都看不到龙，也不能归因于
  材质。测试时后置摄像头对着近处的棕色物体，没有面向开阔空间。

下一步需要在后置摄像头朝向开阔区域、接近水平视线时重跑同一设备测试，
比较 viewport 与截图，再决定是否需要改 Hunting 放置逻辑。双机 Dragon
复制仍受 MPC 握手问题阻断；本报告只覆盖单机 host 召唤与其画面。

两机面对面摆放时，Crack B 上同一签名包再次完成单机 Hunting 流程。
`output/validation-20260925/hunting-face-to-face-B/` 记录龙可控制、两块身体
网格仍启用、错误 0，但身体中心的主相机 viewport 为
`(0.824, −2.906, z=1.623)`，动画结束后的截图仍没有龙。屏幕上的相机画面
仍主要是近处的棕色物体；面对面摆放不等于后置镜头已面向开阔区域，
因此尚不能区分机位与默认放置逻辑的贡献。
