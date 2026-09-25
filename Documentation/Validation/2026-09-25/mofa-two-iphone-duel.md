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

## 两机面对面摆放的复测（11:51）

Crack B 与 I 均由 CoreDevice 显示 connected、`passcodeRequired=false`。在同一
已安装的充能等待版 MOFA 上，以 `devicectl` 分别启动 host/client。两机
AR tracking 均为 true；client 发现名为 `iPhone` 的 host，host 自动批准邀请，
双方于 11:51:49 进入 MPC `Connecting`，11:51:59 均回到 `Disconnected`。
最终报告中 host `playerCount=1`、client `playerCount=0`，都没有进入
Fighting，`watchTriggers=0`。双端 `results.json` 与 `connection.png` 在
`output/validation-20260925/duel-face-to-face-{host-B,client-I}-complete/`。

截图显示 host 的 QR 分享面板已打开；client 后置摄像头拍到的是旁边物品，
不是 host 屏幕上的 QR。MPC 握手先于 QR 空间对齐，这次失败仍发生在握手
阶段；若之后连接成功，需让 client 后置摄像头对准 host 的屏幕二维码，
才能验证真实空间对齐。此次没有记录 Wi-Fi、蓝牙、蜂窝数据的系统开关
状态，不能把断线归因于摆放角度或某个网络开关。

## 同一签名包再次双机复测（13:10）

将 Crack B 已安装的同一 MOFA 签名包安装到 I 后，两机分别以 host/client
启动包内 `DeviceMofaDuelTwoPhoneTests`。两端 AR tracking 为 true；I 于
13:10:28 发现 B，B 收到并自动批准邀请，两端进入原生 MPC `Connecting`，
13:10:38 双端又进入 `Disconnected`。最终 host `playerCount=1`、client
`playerCount=0`，均未进入 Fighting、未触发 Watch 射击。原始报告和截图在
`output/validation-20260925/duel-same-build-{host-B,client-I}-complete/`。

此次 I 的屏幕截图还显示先前 QRealm 的首次相机授权系统弹窗覆盖在 MOFA
画面之上；即使结束 QRealm 进程，弹窗仍在。I 的后置相机也尚未对准 B 的
屏幕二维码。这两项是本轮环境限制，但目前没有证据证明它们造成 10 秒的
MPC 握手断线。需清除弹窗并让 I 后置相机面向 B 的二维码后，再跑一次完整
连接、空间对齐和双端施法测试。

另一台 iPhone 15 Pro A 后来变为 CoreDevice `connected`。将同一 MOFA
签名包安装到 A 成功，但 iOS 在启动时明确以无效签名、entitlement 或
开发者 profile 未被信任为由拒绝启动。因此本轮仍无法用 A 绕开 I 的系统
弹窗做第二组双机握手；没有把 A 的安装成功计为真机测试通过。

## 修复资源引用后的同包双机复测（16:55）

MOFA 独立产品从修复资源引用后的 Unity CLI 导出
`Products/MOFA/Builds/iOS-current-ref-fix/` 以 Xcode 27 命令行完成
`ReleaseForRunning` 签名构建；结果包
`/tmp/holokit-mofa-ref-fix-iphone.xcresult` 为 0 错误、49 警告。
同一最终源码的完整 Unity CLI EditMode 复测为 **59/60**，唯一失败是
资源 GUID 审计，报告 `output/validation-20260925/mofa-ref-fix-full-editor.xml`。
同一 `MOFA.app` 已分别安装到 Crack B 和 I，分别用包内
`DeviceMofaDuelTwoPhoneTests` 的 host/client 环境变量启动。

本轮原生 MPC 日志显示 B 在 16:55:22 收到并批准 I 的邀请，
两端于 16:55:24 进入 `Connected`，不再停在前一轮的握手阶段。
两端设备报告曾同时记录 `playerCount=2`、`networkListening=true`。
随后 host 日志于 16:55:45 出现 `Disconnected`，但报告尚未记录进入
Fighting。抓屏中 B 显示共享二维码，I 仍显示 “Scan The QR Code”且
相机背景为黑色；I 的状态是 `SyncingPose`，没有完成二维码空间对齐。
两端报告采样时 `arTracking=false`，运行错误列表为空，Watch 射击计数为 0。
原始报告与两端画面保存在 `output/validation-20260925/duel-ref-fix-*`。
因此只验证了两机的发现、握手和 NGO 双玩家短暂建立；空间对齐、
真正开打、双方施法及对面人物可见性还未通过。

16:58 再次以同一签名包重启 host/client，I 发现 B、B 批准邀请，
但两端于 16:58:41 从 `Connecting` 直接回到 `Disconnected`；
host 最终报告 `playerCount=1`、`connected=false`，client 最后一次报告
`playerCount=0`。这次运行中 I 的前台画面切换为 HoloKit 诊断应用，
而 B 仍显示 MOFA 二维码；因此 client 没有持续保持在 MOFA 扫码界面。
报告在 `output/validation-20260925/duel-ref-fix-retry-{host-B,client-I}/`。
不能据此判断连接不稳定的单一根因，也不能把这次运行计为完整双人验收。
