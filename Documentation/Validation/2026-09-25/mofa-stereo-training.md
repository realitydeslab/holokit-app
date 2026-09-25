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
缺席是落点/朝向问题，材质是否在视野内正确渲染仍待复测。

共享 `ARPlacementManager` 默认落点原来用 `centerEyePose.eulerAngles.y` 构造
水平偏移。双目手机横放后可同时带 pitch 和 roll，此 Euler Y 不保证等于相机
forward 的水平朝向。代码已改为投影相机真实 forward 到水平面，并让 floor
命中分支用同一朝向；Training 包内新增横放旋转测试，Unity CLI 定向测试通过
（`output/validation-20260925/mofa-rolled-placement-editor.xml`）。
修复版尚未在 iPhone 上重编、观察和验收。

高度参考：Training 场景默认偏移为 `{x:0,y:-0.6,z:6}`，单位为米。Apple 的
[ARKit gravity 世界对齐文档](https://developer.apple.com/documentation/arkit/arconfiguration/worldalignment-swift.enum/gravity)
指出原点是会话启动时的设备位置，而非地板；
[setWorldOrigin 文档](https://developer.apple.com/documentation/arkit/arsession/setworldorigin%28relativetransform%3A%29)
说明可在找到参考点后显式重定义原点。同机 SDK 仓库的 README “Reset World
Origin” 和 `Runtime/iOS/WorldOriginResetter.cs` 提供显式接口；提交 `5bc5b40`
另修复 Unity 6 双目追踪设备筛选，不负责 floor 高度。当前回退落点仅适于
测试战斗路径，不能当作真实地板的替代测量。
