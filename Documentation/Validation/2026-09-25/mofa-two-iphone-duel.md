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

## 后续同机复测

两机解锁后，用已安装的 `8feab44e` MOFA 签名包再次运行游戏包内
`DeviceMofaDuelTwoPhoneTests`。本轮原生 MPC 在两端从 `Connecting` 进入
`Connected`；两份 `results.json` 均记录 `arTracking=true`、`playerCount=2`、
`spatialSynced=true`、`localStatus=Checked`、`fighting=true`。原始报告与
`connection.png` 在 `output/validation-20260925/duel-tracked-parent-{host-B,client-I}/`。
这证明两机至少成功连接过一次；本轮没有记录两机蜂窝数据开关状态，
不能把变化归因于某项系统设置。

两端 `complete=false`，唯一测试异常都是「模拟 Watch 射击没有在本地施法」。
代码检查显示原测试在进入 Fighting 后立即触发 Watch，而基础法术需要先充能；
该事件会被 `MofaInputManager.TryCastBasicSpell` 正常拒绝。Duel 包内真机测试
现改为等待本机护盾和首发基础法术充能就绪，并报告等待时的充能比例；MOFA
定向 Unity CLI Editor 测试 8/8 通过。修正测试时序后的双机施法及远端复制
**尚待新签名包复测**。

新包已由 Unity CLI 成功导出、Xcode 27 `ReleaseForRunning` 签名，并安装到
Crack B 与 I。同一对设备连续两次运行时，host 收到并批准邀请，双方均进入
`Connecting`，约 10 秒后又变为 `Disconnected`，未达到新测试的充能阶段。
第二次 host 最终 `playerCount=1`、`connected=false`，报告在
`output/validation-20260925/duel-charged-shot-retry-{host-B,client-I}-final/`。
前一版在同日连接成功、新版两次失败，说明 MPC 握手结果不稳定；当前证据
既不能证明新包引入网络回归，也不能证明蜂窝数据是原因。两机实际蜂窝数据
状态尚待确认，双端充能后施法和远端复制仍未验收。
