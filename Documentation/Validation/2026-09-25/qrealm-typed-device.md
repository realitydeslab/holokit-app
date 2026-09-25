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
