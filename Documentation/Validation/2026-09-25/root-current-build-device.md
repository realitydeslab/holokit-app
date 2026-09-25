# HoloKit 根项目当前源码构建与真机启动，2026-09-25

Unity 6000.3.24f1 CLI 的完整 EditMode 测试报告在
`output/validation-20260925/root-current-editor.xml`：**127/128 通过**。
唯一失败 `SerializedAssetReferenceTests.PlayableProductAssetGuidsResolve`
在 604 个文件中报告未解析 GUID；该审计尚未通过。

同一源码通过 Unity CLI iOS 导出，产物
`Builds/Upgrade/HoloKit-four-products/`，其
`unity-build.provenance.json` 为 `outcome=success`。Xcode 27
`ReleaseForRunning` 签名构建结果包
`/tmp/holokit-root-20260925-unrestricted-iphone.xcresult` 为
`succeeded`、0 错误、49 警告。首次在受限沙盒内运行 Xcode 失败，
Swift 宏服务和签名证书不可访问；在已批准的完整 Xcode 环境中重跑成功。

构建出的 `com.holoi.holokit.holokit-app` 已安装到 iPhone 15 Crack B
（iOS 27）。根项目包内 `DeviceSceneSmokeTests` 定向运行 `MOFATheDuel`
的 Player 和 Spectator：**2/2 通过**，两次均达到 AR `SessionTracking`，
本机 host listening、共享 multiplayer manager 已生成，并返回 Gallery；
测试错误 0。报告及两张截图在
`output/validation-20260925/root-current-duel-B/`。画面检查表明仅进入
Mono AR 的准备 UI；此项不能证明双机连接、双目视觉或实际法术战斗。
