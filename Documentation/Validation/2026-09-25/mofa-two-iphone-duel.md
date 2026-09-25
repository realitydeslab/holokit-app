# MOFA Duel 双机实测，2026-09-25

测试版本：Unity 6000.3.24f1，MOFA iOS 导出源提交 `3afd1d58`；
`Products/MOFA/Builds/iOS/unity-build.provenance.json` 记录导出成功。
Xcode 27 `ReleaseForRunning` 命令行签名构建成功，结果包：
`/tmp/holokit-upgrade-20260924/mofa-stereo-cleanup-iphone.xcresult`。

| 角色 | 设备 | 系统 | AR tracking | Netcode players | 最终状态 |
| --- | --- | --- | --- | ---: | --- |
| Host | Holo iPhone 15 Crack B (`00008130-001168E428EB8D3A`) | iOS 27.0 | true | 1 | 未连接第二名玩家 |
| Client | Holo iPhone 15 I (`00008130-000848EA38298D3A`) | iOS 27.0 | true | 0 | 未加入 host |

两机均使用同一已签名开发版，`MOFA_DUEL_TWO_PHONE_ROLE=host/client` 启动
各自游戏包 `Tests/Device/DeviceMofaDuelTwoPhoneTests.cs`。首次运行时 Crack B
出现本地网络权限弹窗，放行后 host 显示配对二维码，client 显示正在连接。
再次以 `--terminate-existing --console` 重跑时，原生日志的关键时间线如下：

| 时间（设备本地） | Host | Client |
| --- | --- | --- |
| 10:18:04 | Started advertising | — |
| 10:18:14–15 | — | Started browsing；Browser found peer `iPhone` |
| 10:18:15 | Received invitation，auto approved，`Connecting` | `Connecting` |
| 10:18:25–26 | `Disconnected` | `Disconnected`，随后 shutdown |

双方没有记录 `Connected with peer`，因此二维码扫描、空间对齐和 Fighting/施法
没有开始。这是原生 MultipeerConnectivity 会话握手阶段的失败，尚不能证明具体
网络或代码根因。同一设备组合在不附着 `--console` 的重跑中也未出现第二名玩家。
详细阶段报告和屏幕截图在本机 `output/validation-20260925/`（未纳入 Git）。

设备限制：指定的 iPhone 16 J (`00008140-001111563A0B001C`) 在这轮
CoreDevice 列表中为 unavailable；iPhone 15 A 可安装新版 MOFA，但 iOS 拒绝
启动，报告无效签名、entitlement 不足或开发者 profile 未信任。此次只验证
Crack B 与 I 两台 iPhone 15 Pro，不能代替 iPhone 16 Pro 的真机验收。
