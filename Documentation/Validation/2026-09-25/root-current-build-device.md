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

资源引用修复后的根项目再次由 Unity CLI 导出到
`Builds/Upgrade/HoloKit-four-products-ref-fix/`，并由 Xcode 27 命令行
完成 `ReleaseForRunning` 签名构建。结果包
`/tmp/holokit-root-ref-fix-iphone.xcresult` 为 `succeeded`、0 错误、
49 警告。上面的 Crack B 2/2 场景启动结果属于此前签名包。

修复后完整 Unity CLI EditMode 复测为 **127/128**，报告
`output/validation-20260925/root-ref-fix-full-editor.xml`；唯一失败仍是
`HoloKit.Tests.SerializedAssetReferenceTests.PlayableProductAssetGuidsResolve`。

新签名包也已安装到 Crack B，并用相同的包内 `DeviceSceneSmokeTests`
重跑 `MOFATheDuel` Player/Spectator。报告
`output/validation-20260925/root-ref-fix-duel-B/results.json` 为
`complete=true`、两角色 host listening、共享管理器生成、返回 Gallery，
启动错误 0；但两次 AR 均停在 `SessionInitializing`，各有 1 条
`AR did not reach SessionTracking`，因此 **0/2 通过**。所保存的 Player
截图显示相机背景为黑色。本轮机位/光照条件尚未确认，不能由这个失败
单独推出代码回归，也不能把此前 2/2 通过套用到新包。
