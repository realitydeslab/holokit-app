# QRealm 与 Typed 独立产品复测，2026-09-25

## QRealm

当前源码的 QRealm 场景定向 Unity CLI EditMode 测试 **10/10 通过**，报告：
`output/validation-20260925/qrealm-current-scene-editor.xml`。独立产品的
Unity 6000.3.24f1 CLI iOS 导出成功，产物：
`Products/QRealm/Builds/iOS/`；Xcode 27 `ReleaseForRunning` 签名构建
成功，结果包 `/tmp/holokit-qrealm-20260925-iphone.xcresult`，错误 0、
警告 57。bundle ID 为 `org.realitydeslab.qrealm`，已安装在 iPhone 15 I
（iOS 27）。

包内 `Tests/Device/DeviceQuantumRealmInteractionTests.cs` 已在 I 上启动，
但首次相机权限弹窗尚未被允许，协程停在 `RequestUserAuthorization(WebCam)`。
现场截图在 `output/validation-20260925/qrealm-I-live-screen.png`。
因此手部光点、悬停开始和佛像切换**尚无这版真机结果**。该测试使用注入的
21 关节姿态，完成后仍只能证明识别之后的游戏响应，不能证明原生 Vision
手部识别成功。

## Typed

当前源码七个 Typed 游戏包的场景定向 Unity CLI EditMode 测试 **58/58 通过**，
报告 `output/validation-20260925/typed-current-scenes-editor.xml`。独立
产品的 Unity 6000.3.24f1 CLI iOS 导出成功，产物：
`Products/Typed/Builds/iOS/`。Xcode 27 `ReleaseForRunning` 签名构建
成功，结果包 `/tmp/holokit-typed-20260925-iphone.xcresult`，错误 0、
警告 60，其中包含 AR 参考图库的重复 QR 图像名称警告。bundle ID 为
`org.realitydeslab.typed`，已安装在 iPhone 15 Crack B。尚未打开这版
Typed 包或运行各游戏的真机交互测试。

## 修复资源指针后的新包

Typed 的四个启用场景 Lighting Settings 指针和共享 URP 旧字段修复后，
Typed 完整 Unity CLI EditMode 测试 **75/75 通过**，报告
`output/validation-20260925/typed-clean-guid-full-editor.xml`；QRealm 完整
测试 **27/27 通过**，报告
`output/validation-20260925/qrealm-clean-guid-full-editor.xml`。
两个产品的 `PlayableProductAssetGuidsResolve` 都通过。

两产品已由 Unity CLI 重新导出至各自的 `Builds/iOS-current-ref-fix/`；
QRealm 新版 Xcode 27 签名构建成功，结果包
`/tmp/holokit-qrealm-ref-fix-iphone.xcresult` 为 0 错误、57 警告，
已安装到 I。新包以 `QREALM_INTERACTION_TEST=1` 启动后，设备截图
`output/validation-20260925/qrealm-ref-fix-I-permission.png` 呈纯白横屏；
当前 `results.json` 仍为 `complete=false`、所有交互标记 false、错误 0。
CoreDevice 启动会话尚未返回终态，因此不能把白屏判为应用逻辑崩溃，
也不能认定相机授权已解除。Typed 新版 Xcode 27 `ReleaseForRunning`
签名构建成功，结果包 `/tmp/holokit-typed-ref-fix-iphone.xcresult`
为 0 错误、58 警告，已安装到 Crack B 并启动 Finger Ribbon 包内设备测试。
启动时屏幕截图 `output/validation-20260925/typed-ref-fix-B-permission.png`
明确显示 iOS「本地网络」授权弹窗；测试进程存在，但授权尚未完成，
所以不能把 Finger Ribbon 的触摸/交互判为通过。该测试随后被切换到
MOFA 双机复测和根项目场景测试，尚未取得完成报告；需在系统授权后重跑。
