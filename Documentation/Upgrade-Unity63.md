# Unity 6.3 / iOS 27 upgrade

## 2026-09-27 Editor 全场景巡检、渲染资源检查与产品图标

- **Editor 全场景巡检**（`Packages/com.holoi.library.holokit-app-lib/Tests/Editor/EditorScenePlaythroughTests.cs`，
  `[Explicit]`，用 `--filter EditorScenePlaythroughTests` 运行）：打开产品启动场景进入
  Play Mode，按产品目录用 XR Simulation 逐个以 Player/Spectator 进场，检查
  `SessionTracking`、本机 host、共享管理器、返回列表、运行报错、缺失脚本、
  不受支持的着色器、空 VFX，并把主相机渲染成 PNG。四个产品**依次**运行结果：

  | 产品 | 用例 | 报错 | XR Simulation 内部报错 |
  | --- | ---: | ---: | ---: |
  | MOFA | 10/10 | 0 | 0 |
  | Typed | 14/14 | 0 | 4 |
  | QRealm | 2/2 | 0 | 0 |
  | HoloKit | 26/26 | 0 | 0 |

  Typed 的 4 条来自 AR Foundation `SimulationEnvironmentScanner.CreateMeshColliders`
  在换场景时访问已销毁的模拟环境，仅存在于 Editor 模拟，单独记录、不计失败。
  报告与截图在 `output/validation-20260927/playthrough/<产品>/`（未入 Git）。
  **局限**：多数游戏内容需交互（伸手、点开始）才生成，模拟房间背景在部分
  截图中也未出现，所以截图多为黑底+提示，只能证明没有品红错误着色器，
  不能单独证明所有游戏内容渲染正确。并行运行多个 Editor 会争用 UnityTransport
  7777 端口，必须顺序跑。
- **渲染资源静态检查**（`PlayableProductRenderingAssetTests`，常规套件）：对产品依赖闭包
  内全部材质、Shader/Shader Graph、VFX 检查着色器缺失、编译错误、当前 GPU 不支持、
  以及 URP 无可绘制 pass（旧内置着色器会画成品红）。四个产品各 **3/3 通过**。
  覆盖交互后才出现的内容，但不证明美术效果正确。
- **iPhone 模拟器无法运行**：`libUnityARKit.a`、`libUnityHoloKit.a`、PHASE、
  MPC 原生库均只有真机切片（LC_BUILD_VERSION platform 2），模拟器构建会在
  链接阶段失败；要跑模拟器需为这些库做替身，改动面大，本轮未做。
- **产品图标**（`aed24896`）：MOFA 用用户提供的法阵图（iPhone 与 Watch）；
  QRealm 用画廊佛像网格 `SM_Buddhas.obj` 正面渲染成金色粒子；Typed 为 ObjectSans
  斜体 “T” 与流出的字母。原地替换 PNG，`ProjectSettings` 中的图标 GUID 不变。
  三个产品重新导出并经 Xcode 27 签名构建（0 error、签名校验通过）；从 `.app` 取出的
  `AppIcon60x60@2x.png` 已是新图标。**更正**：当时称 MOFA Watch 的 `Assets.car` 含 1024
  AppIcon，但那仍是旧 HoloKit 图标——导出到已有 Xcode 工程时 Watch 打包步骤提前返回、
  没有重新复制。已在 `79cea69c` 修复，并由 `Tools/verify_ios_export.py` 自动检查。

## 2026-09-27 四 App 编译复核

- 源提交 `4c0cb888`（Animator/Mobile Input 清理及产品 Apple 构建配置入库）。
  四个 App 均用 Unity CLI `--profile iOS` 分别导出成功，Unity 日志均为
  `Build Finished, Result: Success`；随后 Xcode 27 `ReleaseForRunning`
  签名构建均 exit 0、**0 个 error**，`codesign --verify --deep --strict` 通过。

  | App | Bundle ID | 导出目录 | 最低 iOS / SDK | Watch |
  | --- | --- | --- | --- | --- |
  | HoloKit | `com.holoi.holokit.holokit-app` | `Builds/Upgrade/HoloKit-four-products` | 18.0 / iphoneos27.0 | 内嵌 |
  | MOFA | `org.realitydeslab.mofa` | `Products/MOFA/Builds/iOS` | 18.0 / iphoneos27.0 | 内嵌 |
  | Typed | `org.realitydeslab.typed` | `Products/Typed/Builds/iOS` | 18.0 / iphoneos27.0 | 无 |
  | QRealm | `org.realitydeslab.qrealm` | `Products/QRealm/Builds/iOS` | 18.0 / iphoneos27.0 | 无 |

  日志与 `.xcresult` 在本机 `output/validation-20260927/`（未入 Git）。
  本轮只证明编译与签名；新包未安装真机，不代表交互验收。
- 并行导出须给 `--output-path` 传**绝对路径**：相对路径按当前目录解析，
  首轮三个产品同时写入根目录 `Builds/iOS` 并互相覆盖，该目录内容作废。
- 联网传输：用户决定继续使用原有 `com.holoi.netcode.transport.multipeer-connectivity`
  1.0.3，不迁移 Network.framework 2.0.0，也不改原生代码。上游各 MPC 版本
  （1.0.3/1.1.0/1.2.0）的 `MPCSession.m` 除版权头外相同，换版本不会改变握手行为。
  Apple 已在 Xcode 27 弃用 MPC（TN3213）；DTS 对 iOS 26 同类
  `Connecting` 不稳定的答复是只能在应用层超时并重置会话。双机握手问题仍未解决。
- 三处剩余缺失贴图保持不动：本机仅找到 `10ff51d2…` 对应的 Amplify 示例
  `MudNormal.tif`（`gravfield3`），挂在 `FireBall Core.mat` 旧属性
  `_BrushedMetalNormal`，当前内置着色器应不读取该属性，未经 Unity 确认前不删除；
  `f343d204…`（FireBall/SmokeDust `_BumpMap`）与 `d9df6ed1…`（LifeShield 等
  Shader Graph 默认贴图）本机无原文件。
- 产品构建会重写 MOFA XR Simulation 的 `.meta` GUID 并生成
  `AddressableAssetsData/link.xml`；前者构建后还原，后者已加入 `.gitignore`。

## 2026-09-25 最新复测

- 继续清理序列化断链：Training 控制器的 4 个无引用旧 transition、两份 Dragon
  控制器各 51 个无引用子对象，以及 14 个旧 Malbers Mobile Input prefab 的
  Unity UI 脚本 GUID。Hunting 包内新增 3 个 Editor 测试验证方向按钮和
  指针事件。MOFA 完整测试 **62/63**、根项目 **130/131**；唯一失败均为严格
  GUID 审计，已从 132 处降到 **4 处、3 个 GUID**。剩下的是真正被当前
  FireBall/LifeShield 材质使用、尚未找回原文件的贴图，未猜测替换。
  细节见 `Documentation/Serialized-Reference-Audit-2026-09-22.md`。

- 最新资源修复从相邻 HoloKit 原始工程找回 26 个 Dragon/Wolf 动画、FBX 和
  粒子贴图及其原 `.meta` GUID；没有引入会带来 489 个额外断链的三份第三方
  Animator Controller。Unity CLI 资源测试还修正了 AssetDatabase 缓存导致的
  假阳性，并清理当前 VFX Graph/Shader Graph 已弃用的序列化贴图字段。
  MOFA 严格审计从 333 处、52 个 GUID 降到 **132 处、21 个 GUID**，仍为
  1/2 通过；根项目相同为 **132 处、21 个 GUID**、1/2 通过。
  MOFA 完整 EditMode 套件 **59/60**，根项目 **127/128**，唯一失败都是
  该资源审计；Typed 完整套件 **75/75**、QRealm **27/27**，其严格审计
  各 2/2 通过。证据和保留的风险见
  `Documentation/Serialized-Reference-Audit-2026-09-22.md`。当前设备签名包早于
  这些资产改动，不能用旧包的画面为它们验收。
- 两台 iPhone 15 Pro（Crack B 与 I）用同一已安装 MOFA 包再次跑了双机 Duel：
  两端都到 `playerCount=2`，I 的 AR tracking 为真；I 停在
  `SyncingPose`，B 等待空间对齐超时。截图显示 B 的二维码已显示，但 I 的
  后置相机未拍到它；所以 Fighting、对方人物和双端 Watch 施法仍未验收。
  见 `Documentation/Validation/2026-09-25/mofa-two-iphone-duel.md`。
- 资产提交 `1784a5ae` 已由 Unity CLI 重新导出 MOFA iOS，并经 Xcode 27
  `ReleaseForRunning` 签名构建成功（0 错误、59 警告），同一新包已安装到
  B/I。第一轮新包双机测试没有建立第二名玩家；第二轮原生日志在发现、邀请后
  双端从 `Connecting` 约 10 秒回到 `Disconnected`。I 在本轮未进入 AR
  tracking。新包的二维码对齐、Fighting、人物可见性和 Watch 施法仍未通过。
  详情见上述双机记录。

- Typed 当前完整 Unity CLI Editor 套件 **75/75**、QRealm **27/27** 均通过，
  两产品的启用资源 GUID 审计现为 **0** 未解析引用。MOFA 的法术 VFX 网格
  已连同原 GUID 移入基础包，最终源码完整套件 **59/60**；根项目最终源码
  完整套件 **127/128**。两者唯一失败均为
  `PlayableProductAssetGuidsResolve`；最终完整报告分别是
  `output/validation-20260925/mofa-ref-fix-full-editor.xml` 和
  `output/validation-20260925/root-ref-fix-full-editor.xml`。
  两者唯一失败仍是资源审计；清除当前 Dragon shader 已不使用的旧材质条目后，
  剩余引用各 **333** 处。详见
  `Documentation/Serialized-Reference-Audit-2026-09-22.md`；此前已安装的
  iOS 包早于这些资产改动，须重建后再做真机验收。
- 修复版四产品已分别完成 Unity CLI iOS 导出与 Xcode 27
  `ReleaseForRunning` 签名构建，四个结果包均为 0 编译错误。
  QRealm 新版已安装到 I，但其设备交互测试尚无终态，所见屏幕为纯白横屏；
  Typed 新版已安装到 Crack B，其首次本地网络授权弹窗挡住了 Finger Ribbon
  测试。MOFA 同一新版包已安装到 B/I 并重跑双机：一次短暂进入 MPC
  `Connected` 且双方 `playerCount=2`，却仍未完成二维码对齐或进入 Fighting；
  再次重跑在 `Connecting` 后断开。根项目新版签名包已安装 Crack B，
  Duel Player/Spectator 场景各自启动、开 host 和回 Gallery 均成功，
  但两次 AR 都停在 `SessionInitializing`，场景启动测试 **0/2 通过**；
  截图相机背景为黑色，本轮不能归为真机通过。
  详见 `Documentation/Validation/2026-09-25/` 中各产品报告。
- 根项目当前源码的 Unity CLI EditMode 全套 **127/128 通过**；唯一失败是
  `SerializedAssetReferenceTests.PlayableProductAssetGuidsResolve`，在 604 个
  文件中发现未解析 GUID。报告为
  `output/validation-20260925/root-current-editor.xml`。同一源码的根项目
  iOS Unity CLI 导出成功，产物为 `Builds/Upgrade/HoloKit-four-products/`；
  Xcode 27 `ReleaseForRunning` 签名构建成功，结果包为
  `/tmp/holokit-root-20260925-unrestricted-iphone.xcresult`（0 错误、49 警告）。
  新包在 Crack B 的 Duel Player/Spectator 场景启动测试 2/2 通过，
  详见 `Documentation/Validation/2026-09-25/root-current-build-device.md`；
  这些结果还不是四应用的真机交互验收。
- QRealm 当前源码场景测试 10/10、独立 iOS 导出与 Xcode 签名构建成功，新包
  已安装 iPhone 15 I；首次相机权限弹窗尚待在设备上允许，交互测试仍未开始。
  Typed 七个游戏包场景测试 58/58、独立 iOS 导出和 Xcode 签名构建成功，
  新包已安装 Crack B，仍待真机交互测试。
  详见 `Documentation/Validation/2026-09-25/qrealm-typed-device.md`。
- Hunting Dragon 在 Crack B 上通过实际召唤按钮生成，Portal 也生成，入口动画
  结束后龙已可控制、身体网格存在、运行错误 0；但当前机位的龙身体中心投影
  在主相机 viewport y≈−1.73，截图看不到龙。已在 Hunting 包的设备测试中
  增加动画结束后的截图和网格/投影记录，并完成 Unity CLI 8/8 与新包真机复测。
  需在后置摄像头朝向开阔区域时复测视觉，详见
  `Documentation/Validation/2026-09-25/mofa-hunting-dragon-visual.md`。
- MOFA Training 的双目追踪父节点修复已在 iPhone 15 Crack B 真机运行：
  `SessionTracking`，实际进入 Fighting，模拟 Watch 施法 0→1、AI 施法 1，
  运行错误 0。左右眼中 AI 身体中心 viewport x≈0.495/0.484，三张战斗截图
  实际显示对手身体；先前“完全没有小人”的画外问题在本轮消失。人物姿态、特效
  遮挡、魔杖的独立可辨性及真实 floor 高度仍需验收，不能把实例存在等同于最终
  视觉通过。证据在 `Documentation/Validation/2026-09-25/mofa-stereo-training.md`。
- Duel 双机在 Crack B/I 同一签名包上曾成功从原生 MPC `Connecting` 进入
  `Connected`，两端 `playerCount=2`、空间同步且进入 Fighting；模拟 Watch
  触发在首发法术未充能时被正常拒绝。Duel 包内真机测试已改为等待护盾与基础
  法术充能，Unity CLI 定向测试 8/8，通过后新 iOS 签名包已安装双机。
  该新包随后三次重跑又在 MPC 握手阶段断开（第三次为两机面对面摆放），
  同一签名包的再次双机复测也在约 10 秒后断开；I 上另有 QRealm 首次相机
  授权系统弹窗覆盖 MOFA 画面，本轮环境不纯净，
  因此充能后的双端施法与复制
  **仍未真机通过**；蜂窝数据状态待确认。详见
  `Documentation/Validation/2026-09-25/mofa-two-iphone-duel.md`。
- 从 Git 历史和本地 LFS 找回原资产/原 GUID 后，进一步修正了 Typed 场景
  Lighting Settings 指针、废弃 URP 字段和 MOFA 独立产品缺失的 VFX 网格；
  最新严格资源审计见上方及
  `Documentation/Serialized-Reference-Audit-2026-09-22.md`。

## 最新进度：2026-09-25，双机复测后

- 新版 MOFA 在 iPhone 15 I（iOS 27）的双目 Training 设备测试完成：
  `renderMode=Stereo`、追踪设备与 AR tracking 均正常；按实际双目触发按钮
  进入 Fighting，模拟 Watch 射击计数 0→1，AI 自行施法 1 次，手部武器实例存在，
  运行错误 0。但三张双目战斗截图均未呈现 AI 身体或魔杖，**画面可见性仍不通过**。
  本轮无 floor 平面，包内测试用了场景默认的相机相对回退落点（相机 Y≈0，
  AI 脚点 Y≈−0.60 m）；这不是实测地面高度。SDK 历史中的
  `WorldOriginResetter` 是显式重置 ARKit 世界原点的接口；ARKit `gravity`
  模式默认原点是启动追踪时设备位置，不是 floor。已在 Training 包内设备测试
  增加 AI 身体 bounds 和各相机投影坐标记录后，Crack B 的新版真机报告确认
  AI 身体网格存在（高约 1.75 m），但中心在左右眼 viewport x≈2.45，超出
  可见范围 0–1；这解释该轮截图中身体不出现。共享默认落点此前取
  `centerEyePose.eulerAngles.y` 当水平朝向，手机带 roll 时不等同于真实相机
  forward；现改为沿相机 forward 的水平投影放置，并让地板命中分支使用同一
  水平朝向。MOFA 包内新增横放双目相机测试，Unity CLI 定向测试通过；
  但 `a9f87263` 修复版在 Crack B 真机的左右眼 viewport x 仍约 2.42、2.39，
  截图仍看不到对手，**视觉问题未解决**。包内设备测试进一步记录放置、
  开战和攻击阶段的相机 pose；定向 Editor 测试 12/12，通过 Unity CLI 再次
  导出并由 Xcode 签名。诊断包在 Crack B 上连续两次未进入 AR Tracking，
  因此还没有得到可比较的 pose，需继续真机复测。详情：
  `Documentation/Validation/2026-09-25/mofa-stereo-training.md`。
  后续用 CoreDevice 将 Crack B 测试方向设为 `landscapeLeft` 后，AR Tracking
  恢复，得到的姿态数据显示 `Center Eye Pose` 比 AR 主相机低约 1.12 m，
  左右眼对手投影仍在画外。对照同机新版 HoloKit SDK，已在共享相机 prefab
  加入 `Stereo Tracked Pose` 父节点，让低延迟追踪更新父节点，保留子节点
  的相机到眼睛校准偏移；MOFA Training 包内测试 13/13 通过，新的 iOS
  签名包已安装至 Crack B 与 I。该阶段两机锁屏；后续真机结果见上方最新复测。
  MOFA 完整 EditMode 套件在该修复前为 **57/58**：唯一失败是原有
  `SerializedAssetReferenceTests.PlayableProductAssetGuidsResolve`，不能声称
  指针审计通过；报告 `output/validation-20260925/mofa-visual-diagnostics-editor.xml`。
- 最新 MOFA Unity CLI iOS 导出成功：`Products/MOFA/Builds/iOS/unity-build.provenance.json`
  的结果为 `success`，源提交 `3afd1d58`；Xcode 27 使用命令行完成
  `ReleaseForRunning` 签名构建，结果包在
  `/tmp/holokit-upgrade-20260924/mofa-stereo-cleanup-iphone.xcresult`。此版本已安装并
  能在两台 iPhone 15 Pro（Crack B、I，均 iOS 27.0）启动。15 A 虽可安装，iOS
  拒绝启动并报告签名、entitlement 或开发者信任不足；原先指定的 iPhone 16 J
  当前在 CoreDevice 中显示 unavailable，不能把 15 Pro 结果当作 16 Pro 验收。
- 两台 15 Pro 的 Duel 重跑已越过本地网络权限弹窗，且双端 AR tracking。
  原生 MPC 日志显示 client 发现 host、发出邀请，host 自动接受；两端在
  `Connecting` 约 10 秒后转为 `Disconnected`，从未到 `Connected`。host 的
  `playerCount=1`，client 为 0，二维码空间对齐与双人施法均未开始。无 console
  附着的重跑也没有第二名玩家，故不能把故障归于 console 附着。
  证据与设备编号、阶段门槛写在
  `Documentation/Validation/2026-09-25/mofa-two-iphone-duel.md`；原始报告与
  截图保存在本机 `output/validation-20260925/`。下一步在两机暂时关闭蜂窝数据、
  保持 Wi-Fi/蓝牙的条件下复测，以检验网络路径假设；尚未将此假设当作原因。

## 先前进度：2026-09-24

- 2026-09-23 的第二次双机实测改跑 Hunting，仍在 MPC `Connecting` 后约
  10 秒双端断开；未出现 `Connected`，Dragon/Portal 的双机复制未能开始。
  日志在 `/tmp/holokit-upgrade-20260923/mofa-hunting-{host,client}-console.log`。
  因为 Duel 和 Hunting 两个独立场景都以相同方式失败，下一步先诊断原生
  MultipeerConnectivity 握手；不能把问题归给某个游戏的网络 prefab。
  清理期空引用与双目测试入口修复已通过 MOFA 包内 Unity CLI Editor 测试
  **49/49**，报告 `/tmp/holokit-upgrade-20260923/mofa-dual-fixes-editor.xml`，
  但仍未在新 iOS 包上验证。

- **首次两台 iPhone 的 MOFA Duel 实测定位到连接失败阶段。** 16 Pro host 已广播，
  15 Pro client 搜到 host、发出连接请求，host 接受；约 10 秒后双方
  MultipeerConnectivity 都报告断开，Netcode 始终未出现第二名玩家（host
  `playerCount=1`，client `playerCount=0`）。原始日志与阶段报告：
  `/tmp/holokit-upgrade-20260922/mofa-duel-two-phone-{host,client}/`、
  同名前缀 `*-console.log`。因此二维码空间对齐和双人战斗均未开始，不能当作
  多人测试通过。15 Pro 断线清理时还在 `MofaFightingPanel.CheckAppearance()`
  遇到 `LocalPlayer` 空引用；已给网络管理器及面板添加销毁期判空，待编译、
  重装和重试。客户端首次运行还记录一次 `Touch was already deallocated`，
  与连接断开之间尚无确定因果关系。
- **首次双目 Training 实测已找到 ARKit 追踪设备。** 16 Pro 的报告
  `/tmp/holokit-upgrade-20260922/mofa-stereo-wand-iphone16/results.json` 显示
  `renderMode=Stereo`、`trackingDeviceFound=true`、AR tracking/host 正常；
  测试之后因只寻找单目开始按钮而中止，尚未验证魔杖或 AI 高度。
  已让包内设备测试打开双目的 `StarAR` 面板并调用其真实触发组件，且去掉测试
  人为将 AI 放在 1.5 米前方的特殊落点，改用场景的默认落点；待新包复测。

- **MOFA Training 真机战斗已跑通，对手外观仍需校正。** 最新 Unlit 构建在
  iPhone 16 Pro（`iPhone17,1`）上完成真实开始按钮进入 Fighting、模拟 Watch
  射击令本机施法计数 0→2、AI 自己施法 1 次，运行错误 0，`complete=true`。
  报告与原始截图在 `/tmp/holokit-upgrade-20260922/mofa-unlit-final/`。截图可见
  对手身体，但位置/尺寸明显异常，不能把运行时 `aiAvatarVisible=true` 当成视觉验收。
  用户又指出手中没有魔杖；追查发现 Meebits prefab 的 `_holdWeapon=1`，但
  `WeaponPrefab` 为空，`MeebitsController.Start()` 因而不会生成武器。Training AI
  现在在头像 `Start()` 前注入已有的 `ancient art vfx` 武器 prefab，并让设备测试
  断言手部武器实例存在；新构建与真机复测待完成。
- **Dragon 召唤真机通过。** iPhone 16 Pro 上按 Hunting 实际召唤按钮，在
  AR 已追踪、没有 `Floor` 分类平面的场景中，默认放置点允许继续进入生成流程；
  Dragon 和 Portal 两个网络对象均生成，错误 0，`complete=true`。报告和截图在
  `/tmp/holokit-upgrade-20260922/mofa-hunting-spawn-final/`。截图拍摄时相机朝桌面，
  只证明召唤流程与控制 UI，尚不能证明龙模型在画面中正确渲染。
- **双目模式落点高度正在复查。** `ARPlacementManager` 原来用带俯仰的
  `CenterEyePose.TransformVector` 计算默认落点；当距离配置为 6 米时，轻微抬头
  会显著改变 Y 坐标。已改为仅绕竖直轴计算水平距离，保留配置的固定 Y 偏移；
  同时接受包含 `Floor` 标志的复合 ARKit 平面分类，而非要求分类恰好等于
  `Floor`。这修复了可确认的落点计算缺陷，但双目下 AI 的实际离地高度还需
  真机数据与画面验证，不能把用户观察的全部高度问题视为已解决。MOFA 独立
  项目对应游戏包的 Unity CLI Editor 测试 **49/49 通过**，报告
  `/tmp/holokit-upgrade-20260922/mofa-wand-placement-editor.xml`；魔杖与落点
  新逻辑尚未经过 iPhone 复测。
- **已查到 HoloKit SDK 的对应历史修复。** 同机的
  `holokit-unity-sdk` 仓库提交 `5bc5b40`（2025-01-07，
  `fix position shift problem in stereo mode for unity 6000`）说明 Unity 6 的
  ARKit 输入设备只带 `TrackedDevice` 特征；旧代码还额外要求 `HeadMounted`，
  可能导致双目头部位置完全不更新。App 内 `HoloKitTrackedPoseDriver` 仍是旧筛选，
  已按 SDK 提交改为 `TrackedDevice`。包内设备测试现可用
  `MOFA_COMBAT_STEREO_TEST=1` 显式切换双目，并记录是否找到追踪设备、中心眼
  Y、放置点 Y 和 AI 根节点 Y；此历史修复移植需重新编译并真机验证。
- **双机实测准备中。** `devicectl` 确认 iPhone 16 Pro
  `00008140-001111563A0B001C` 与 iPhone 15 Pro
  `00008130-000848EA38298D3A` 均物理连接。分别在 Duel/Hunting 游戏包的
  `Tests/Device` 加了开发版双机 runner：一端作为 host 展示现有 QR 配对面板，
  另一端作为 client 走现有 MultipeerConnectivity 发现、连接和 ARKit 图像追踪；
  分阶段保存玩家数、空间对齐状态、双人战斗施法或 Dragon/Portal 两端生成状态。
  新 runner 已通过 MOFA 独立项目 Unity CLI 编译及包内 Editor 测试 **49/49**，
  报告 `/tmp/holokit-upgrade-20260922/mofa-two-phone-editor.xml`；仍待两台真机
  共同验证，不能仅凭联网对象数宣称空间对齐成功。
- MOFA 双机/双目最终 Unity CLI iOS 导出已成功，日志在
  `/tmp/holokit-upgrade-20260922/mofa-two-phone-final-export.log`；Xcode 原生
  签名编译正在运行，结果目录
  `/tmp/holokit-upgrade-20260922/mofa-two-phone-final-iphone.xcresult`。
  当前尚未将这一版安装到两台 iPhone，不能把前一版战斗/召唤报告当作新版本验收。

- **Dragon 召唤排查与修复待真机确认。** Hunting 召唤按钮最终调用
  `MofaHuntingRealityManager.TryGetReady()`；原实现只有 ARPlacementManager 找到
  `HorizontalUp` 且精确分类为 `Floor` 的平面才可进入 `SetupRound()`。在 AR 已正常追踪、
  但当前房间没有这种地面分类时，用户按按钮只会触发失败事件。现在改为使用场景原本
  配置并持续更新的相机前方默认放置点，继续生成 Portal 和 Dragon；在 Hunting 包
  `Tests/Device` 新增真机测试，实际调用召唤按钮并断言网络 Dragon/Portal 生成。
  MOFA 包内场景测试 **48/48** 通过；新 iOS 包正在重编，尚未有真机召唤证据。
- **对手材质首个 fallback 失败，已改 Unlit 待真机复测。** 前一轮把 14 个
  `SH_Meebits` 材质替换为 URP Lit，运行日志确认替换，但截图中人仍不可见，Metal
  同样报告 Lit 的插值输入不匹配。报告与截图：
  `/tmp/holokit-upgrade-20260922/mofa-visible-avatar-final/`。已改用更简单的
  URP Unlit，并重新通过 MOFA 包内 **48/48** 场景测试；必须以新 iPhone 截图判定
  是否真的修复，不能以材质替换计数作为可见性证据。

- **MOFA 对手可见性修复待重编译复测。** 最新 iPhone 16 Pro 真机战斗报告
  `/tmp/holokit-upgrade-20260922/mofa-combat-awake-final/results.json`：
  AR、host、AI 对象、Fighting 均正常；模拟 Watch 使本机施法 0→3，AI 自己施法
  1 次。因此“AI 不攻击”已由本轮头像参数修复；但截图里仍看不到对手身体，
  原生日志报 `Shader Graphs/SH_Meebits` 的 Metal pipeline state 创建失败。
  同次运行还有一次 `Touch was already deallocated`，因此设备测试未标为通过。
  已给 Training AI avatar 增加 iOS 专用的 URP Lit 材质 fallback，逐个保留原
  Meebits 材质底色；新增包内 Editor 测试和设备端 fallback 计数断言。
  **材质修复尚待 Unity 编译、Xcode 签名和真机截图复测。**

- **序列化指针专项排查进行中。** 在共享 `Tests/Editor` 新增自动测试，沿每个产品
  所启用场景与产品配置的资源依赖，检查场景、prefab、ScriptableObject、Animator、
  材质里的 GUID 是否可由该产品的 Unity AssetDatabase 解析，并检查 prefab 缺失
  脚本。HoloKit 根项目首轮精确 GUID 扫描发现 359 个引用位置、67 个不同缺失
  GUID，主要是旧 Dragon 动画控制器、旧 VFX/材质纹理和 URP 调试资源；prefab
  缺失脚本检查通过。四产品的精确扫描结果分别是 HoloKit **359 处/67 GUID**、
  MOFA **365 处/61 GUID**、Typed **20 处/13 GUID**、QRealm **11 处/10 GUID**；
  四者的 prefab 缺失脚本测试均通过。所有未解析 GUID 和首个源码位置已保存到
  `Documentation/Serialized-Reference-Audit-2026-09-22.md`，完整逐行结果在
  `/tmp/holokit-upgrade-20260922/guid-audit-{root,mofa,typed,qrealm}.xml`。
  已移除 MOFA Death Circle 里无用途的旧调试脚本组件，
  将 MOFA Hunting Portal 所需的两个 FBX 连原 GUID 移到游戏包，清理 UI 的
  12 个未使用的断链按钮状态精灵引用。扫描结果未清零，不能宣称指针问题全部修复。
  其中 MOFA Training 头像 Animator Controller 还有 4 处转场指向已不存在的
  GUID `74535b8bda892f4428e70719b7801c63`，可能影响 AI 战斗动作；须结合真机
  施法测试判断实际影响，不应只把这些当成旧美术残留。
- MOFA 对手修复版已完成 Unity 导出、Xcode 签名并安装到 iPhone 16 Pro。
  首两次真机战斗测试在 App 运行十余秒后被系统退到后台，测试尚未跑完，
  不能据此判断对手是否已修好。已给仅开发版设备测试加防自动休眠，并在
  战斗阶段保存中途状态；需重编译并复测。

- **升级验收新阻断：MOFA Training 真机战斗缺失对手。** 点击游戏实际开始按钮后
  已进入 Fighting，模拟 Watch 射击使本机法术计数从 0 增至 3；但本轮共 141
  次错误，其中 1 次 AI 头像生成失败、其余主要是 AI 动画引用空值。报告和
  战斗/射击截图：`/tmp/holokit-upgrade-20260922/mofa-combat-2-iphone16/`。
  根因是 AI 头像参数列表第一项 Chibi Apes 指向不存在的 collection GUID，
  `GetAvatarCollectionParams()` 未判空就调用 `Equals`，使 Meebits 对手也无法生成；
  攻击需要头像动画事件，因而玩家看到对手不出现、也不攻击。已移除不可解析
  条目、增加判空及 package 内编辑器测试，并把真机测试升级为断言 AI 模型可见、
  AI 自己实际施法、运行错误为 0。**这些修复尚待重编译/真机复测，当前不算通过。**
  MOFA 独立项目修复后 Unity CLI 编辑器测试 **54/54** 通过；修复版 iOS 导出中。
- 用户要求先完成升级，不额外搭 Mac 双人环境。现有 Editor 联机使用
  UnityTransport `127.0.0.1:7777`，Mac 双实例有可行路径；Dragon/Hunting 与
  Duel 的真实双人加入、空间对齐和战斗同步暂列后续专项验证。Hand Tracking
  的合成姿态测试不代表原生 Vision 成功，日志出现间歇 `Failed to get scene depth`；
  本轮暂不改原生算法，先完成四 App 构建与现有真机问题修复。

- **交互测试正在补齐：** 用户要求 MOFA 真正点击开始进入战斗，并模拟 Apple Watch
  挥腕射击；Type-D 与 Q-Quantum 也要覆盖手势、晃动等实际交互。已在 MOFA
  包内加入仅开发版启用的真机战斗测试：调用 Star 按钮的按下/松开回调，断言进入
  Fighting 阶段，注入 WatchConnectivity 射击事件，检查法术计数并保存截图。
  Unity CLI 编辑器测试 **52/52** 通过，新 iOS 导出成功，Xcode 签名编译中；
  **尚不能声称真机战斗或射击通过**。该模拟从 Watch 识别出挥腕之后的手机事件
  开始，不能验证实体 Watch 的 CoreMotion 识别。
- 用户要求测试代码放在每个游戏的 `Tests` 文件夹。MOFA 与 QRealm 的开发版
  真机交互脚本已移至各自 `Tests/Device`，各有 iOS/Editor 测试程序集；运行时
  仅保留 Watch 事件与手部姿态的开发版注入接口。MOFA 首次真机交互运行发现
  测试误找通用 Star 按钮，实际单目模式使用 `MofaUIPanel.OnTriggerButtonPressed()`；
  首次报告 `starButtonActive=false`、`fighting=false`，位于
  `/tmp/holokit-upgrade-20260922/mofa-combat-iphone16/`。已修正入口，待重编译
  与真机复测；修正后 MOFA 独立项目 Unity CLI 编辑器测试 **53/53** 通过，
  第二轮 iOS 导出中。不能把首次失败算作战斗通过。
- QRealm 新增设备测试拟用合成 21 关节姿态驱动实际手部追踪路径，检查手部光点
  移动、悬停开始和佛像切换，截图留存。合成姿态不验证相机视觉识别；此测试
  已在独立项目通过 Unity CLI 编辑器测试 **25/25**，新 iOS 导出中；真机交互
  尚未验证。Typed Finger Ribbon 的 palm/fist/手部晃动测试也已放入其
  `Tests/Device`，Typed 独立项目 Unity CLI 编辑器测试 **73/73** 已通过，
  新 iOS 导出中，真机交互尚未验证。随后又给 Typed Sculpture 包增设
  `Tests/Device`，检查手势激活悬停、按住启动及雕塑网络对象生成，待编译/导出。
  这些注入姿态
  只能覆盖识别之后的游戏响应，不能证明 Vision 摄像头识别能力。
- Typed 产品目录清理后的独立包已在 iPhone 16 Pro 完成 **14/14** 场景生命周期
  回归，启动错误 0、场景错误 0。报告与截图：
  `/tmp/holokit-upgrade-20260922/typed-catalog-iphone16-final/`。
- MOFA 缺法术包修复版已在指定 iPhone 16 Pro 通过 **10/10** 场景生命周期
  检查，启动与场景错误均为 0；报告及 10 张截图：
  `/tmp/holokit-upgrade-20260922/mofa-iphone16-fixed/`。Typed 清理目录前的版本
  **14/14**、QRealm 清理目录前的版本 **2/2**；两者最终清理版 Unity 导出与
  Xcode 签名成功，但还需重新安装回归。主 HoloKit **26/26** 已通过。
- QRealm 清理目录后的最终包真机回归发现新问题：Player 例无错，但 Spectator
  返回列表时，`QuantumRealmHostHandPoint` 的 LeanTween 仍访问已销毁的光点
  MeshRenderer，产生 77 次空引用。已给动画回调加入对象生命周期检查，**修复尚需
  重新 Unity 导出、Xcode 构建及 iPhone 复测**。失败报告：
  `/tmp/holokit-upgrade-20260922/qrealm-catalog-iphone16/results.json`。
- QRealm 加入交互测试与上述空引用修复的 iOS 包已导出、Xcode 签名并安装。
  第一次专项真机运行在等待 6 秒后检查 AR 时碰到短暂的 `SessionInitializing`/
  追踪受限，尚未运行手部/悬停/切换步骤；报告：
  `/tmp/holokit-upgrade-20260922/qrealm-interaction-iphone16/results.json`。
  已把设备测试改为最多等待 30 秒并记录最终 AR 状态，待重新导出/编译/真机复测。
  同一已安装包的 2 场景生命周期回归正在跑，用于验证退场空引用修复。

- **第二个真机发现（MOFA，修复待真机复测）：** 独立 MOFA 启动时
  `Reality.IsCompatibleWithMetaObjectCollection` 空引用，修复前 10 例共记录
  2932 次错误，报告 `/tmp/holokit-upgrade-20260922/mofa-iphone16-before-spells/`。
  根因是生成器漏把 `org.realitydeslab.mofa.object.spells` 纳入 MOFA 包闭包。
  现已补包；Typed/QRealm 没有兼容 MetaObject 的游戏，因此把两者产品目录中
  指向该包的无效条目清空。新增产品测试检查所有目录条目均可解析。MOFA 修复后
  Unity CLI 测试 **52/52** 已通过，新 iOS 导出与真机复测正在进行；修复前的
  10 例结果不能算通过。
- **新发现的真机问题与已验证的修复：** QRealm 的首个独立签名包已安装到
  iPhone 16 Pro，但启动后 `TMP_Settings.get_defaultStyleSheet()` 连续空引用。
  生成器误写 `Text Mesh Pro`，实际目录是 `Assets/TextMesh Pro`，使三个小产品
  都漏掉 TMP 默认 Resources。已修正生成器、给三个产品补入原 GUID 的 TMP 资源，
  并新增共享产品测试断言。修复后三个独立项目 Unity CLI 测试分别为 QRealm
  **23/23**、MOFA **51/51**、Typed **71/71**，根项目 **119/119**。QRealm 修复版
  已重新导出、Xcode 签名并在指定 iPhone 16 Pro 通过 **2/2** 场景生命周期测试，
  启动和场景错误均为 0；报告与两张截图位于
  `/tmp/holokit-upgrade-20260922/qrealm-iphone16-fixed/`。Typed 修复版已重导出，
  Xcode 编译中；MOFA 修复版尚未重导出。旧版成功结果不能代表这两款已真机通过。
- HoloKit 根项目的新 Xcode 编译已成功，新包已安装到 iPhone 16 Pro 并通过
  **26/26** 场景生命周期检查。报告和 26 张截图位于
  `/tmp/holokit-upgrade-20260922/holokit-four-products-iphone16/`：启动错误 0、
  场景错误 0，26 例均确认 AR tracking、host listening、共享管理器生成及返回列表。
- Typed 的 TMP 修复版已在 iPhone 16 Pro 通过 **14/14** 场景检查；报告和 14 张
  截图位于 `/tmp/holokit-upgrade-20260922/typed-iphone16-fixed/`。其产品目录
  又按上述 MOFA 发现做了清理，最终包需再增量构建和回归。MOFA 的上一版
  Unity 导出和 Xcode 签名已成功，但含缺包错误，不能作为最终验证。
- 上一轮 Xcode 27 原生构建最终成功；修复包已安装到指定的 iPhone 16 Pro
  (`00008140-001111563A0B001C`，iOS 26.0)。完整真机结果位于
  `/tmp/holokit-upgrade-20260922/iphone16pro-fixed/results.json`，附 26 张截图。
  **26/26 场景生命周期检查通过，启动错误 0，场景错误 0**；每例 AR tracking、
  host listening、共享管理器已生成、回到列表均为真。截图已人工检查 QRealm 和 Tornado。
  这仍不等于真实手势、双设备联网、录屏和光学校准均通过。
- 产品配置已转为 `Assets/Resources/HoloKitAppGlobalSettings.asset`，共享壳在运行时
  加载各产品的配置副本。HoloKit 与三个小产品各拥有游戏目录、封面和 App 图标。
  Watch 打包由产品配置控制，HoloKit 和 MOFA 启用，Typed 与 QRealm 关闭。
- 已将 21 个被多个游戏引用的渲染/特效资源连同原 GUID 移到
  `Packages/com.holoi.library.shared-assets`。原路径与新路径记录在
  `Documentation/Validation/2026-09-22/shared-asset-moves.json`。
- `Products/MOFA`、`Products/Typed`、`Products/QRealm` 是新生成的独立 Unity 项目，
  游戏目录分别 5/7/1 项、场景总数 7/9/3、引用本仓库本地包数 17/18/12。
  用户的原有兄弟项目没有被修改。生成脚本在
  `Tools/Products/generate_thin_projects.py`，拒绝覆盖已存在的产品目录。
- 主项目在配置变更后 Unity CLI 编辑器测试 **118/118 通过**，报告
  `/tmp/holokit-upgrade-20260922/product-config-tests-3.xml`。三个小项目的独立
  Unity CLI 测试现已通过：MOFA **50/50**、Typed **70/70**、QRealm **22/22**；
  报告分别为 `/tmp/holokit-upgrade-20260922/mofa-isolated-tests.xml`、
  `typed-isolated-tests-2.xml`、`qrealm-isolated-tests-2.xml`。首次 QRealm 测试
  发现生成器 `file:` 路径少了一层；首次 Typed 测试发现雕塑场景材质原先只在主项目，
  均已修复并通过重跑。
- MOFA、Typed、QRealm 三个独立项目的原生 Unity CLI iOS 导出均已成功，
  各自 `Products/<产品>/Builds/iOS/unity-build.provenance.json` 的 `outcome`
  均为 `success`。QRealm 和 Typed 的 Xcode 27 原生编译正在运行；三个小产品
  尚未在真机安装。生成器已排除主项目的本地 NatML/NatShare 配置，不把这些
  非产品配置复制到小项目或提交到 Git。
- 共享资源迁移后的 HoloKit 根项目再次通过 Unity CLI 编辑器测试 **118/118**，
  报告 `/tmp/holokit-upgrade-20260922/holokit-root-after-split.xml`。根项目
  新 iOS 导出也已成功，`Builds/Upgrade/HoloKit-four-products/unity-build.provenance.json`
  的 `outcome` 为 `success`。其新导出尚未做 Xcode 编译与真机回归。

下一步确认三个独立产品使用原生
`unity build Products/<产品> --profile iOS --output-path ...` 分别导出，
用 Xcode 签名编译并在 iPhone 16 Pro 上安装验证。完成所有产品后，再重建当前
HoloKit 根项目并做回归，因为根项目的产品资源归属也已变化。

## 续接记录：2026-09-22，用户要求及时保存进度

**先读本节再继续。项目尚未完成四 App 拆分；不要把编辑器测试通过或旧包真机
启动成功写成四个产品全部验收通过。**

- 工作目录：`/Users/amber/Projects/HoloKit/HoloKit2/holokit-app`。
- 工作分支：`upgrade/unity63-ios27-four-apps`。用户原始改动已保存为
  `72434830`；场景引用修复和第一批测试为 `ae12da52`；旧设置迁移为 `7cd5d197`。
  本文与本轮运行时修复会一起提交；用 `git log -1` 获取本次保存的提交。
- 用户明确要求使用 Unity CLI，尤其 `/Users/amber/.unity/bin/unity test`；
  游戏测试放在各自 package 内。不要恢复已删除的 `Assets/Editor/HoloKitProductBuild.cs`，
  也不要改用 Unity MCP。构建使用原生 `--profile iOS`。
- **当前指定真机：iPhone 16 Pro，Holo’s iPhone 16 J，iOS 26.0 (23A341)，
  UDID `00008140-001111563A0B001C`，型号代码 `iPhone17,1`。**
  之前使用的 iPhone 15 Pro / iOS 27 不是当前主测试机。iPhone 18 Pro 尚未到手。
- 用户不希望反复确认权限；已授权的常规构建、测试、修复和保存直接继续。
  当前沙盒限制仍由环境控制，不能声称已关闭；已有 Unity/Xcode/devicectl/Git 命令许可。

### 已验证及当前正在运行的工作

1. 最新 Unity CLI 测试 **115/115 passed，0 failed，0 skipped**。
   包括 106 个游戏场景检查、8 个设置迁移检查，以及 1 个进入 Play Mode 的
   本机主机/客户端测试（共享服务生成、复用实例、无重复、远端销毁）。
   报告：`/tmp/holokit-upgrade-20260922/iphone16-regressions.xml`。
2. iPhone 16 Pro 上旧设置迁移与封面列表启动成功。修复前完整真机基线跑完
   13 场景 × Player/Spectator = 26 例，26 例均 tracking、host listening、返回列表；
   但日志共有 569 次错误，**该基线不通过**。
3. 本轮已修复 HTTP 启动请求、NGO 未注册共享管理器、QuantumRealm 手部深度引用、
   QuantumRealm/Flock/Rain 音频引用、Tornado 不存在的 CameraForward 参数写入。
   NGO 客户端使用 prefab handler 复用预先创建的实例，避免重复服务。
4. 最新修复的 Unity iOS 导出已成功，2026-09-22 11:17:45 UTC 完成：
   `Builds/Upgrade/HoloKit/unity-build.provenance.json` 的 outcome 为 success。
   导出日志：`/tmp/holokit-upgrade-20260922/holokit-iphone16-fixes-export.log`。
5. **保存本记录时 Xcode 原生编译仍在进行；尚未确认成功，尚未安装本轮修复包，
   尚未重新跑真机 26 例。** 当前执行会话为 `81150`（仅本会话可用），
   xcodebuild 最初 PID 为 `34254`。后续须先检查进程/结果，避免并行重复构建。
   结果目录：`/tmp/holokit-upgrade-20260922/holokit-iphone16-fixes.xcresult`。
   曾打印旧 Xcode 账号缺少 token 的提示，不能据此认定构建失败；以终态为准。

### 立即续接步骤与命令

先检查上述 Xcode 构建是否结束及其结果。产物路径一直复用，**旧的 .app 存在不代表
这轮构建成功**。若需要重跑，使用新的、不存在的 `-resultBundlePath`：

```sh
xcodebuild -project Builds/Upgrade/HoloKit/Unity-iPhone.xcodeproj \
  -scheme Unity-iPhone -configuration ReleaseForRunning \
  -destination generic/platform=iOS \
  -derivedDataPath /tmp/holokit-upgrade-20260922/xcode-holokit \
  -resultBundlePath /tmp/holokit-upgrade-20260922/holokit-iphone16-fixes-retry.xcresult \
  CODE_SIGN_STYLE=Automatic DEVELOPMENT_TEAM=KR9H35SQQ9 \
  -allowProvisioningUpdates -quiet
```

确认该轮成功后安装并运行；这会替换当前 HoloKit 开发包并终止现有 app 进程：

```sh
xcrun devicectl device install app --device 00008140-001111563A0B001C \
  /tmp/holokit-upgrade-20260922/xcode-holokit/Build/Products/ReleaseForRunning-iphoneos/HoloKit.app
xcrun devicectl device process launch --device 00008140-001111563A0B001C \
  --terminate-existing --console --environment-variables '{"HOLOKIT_SCENE_TEST":"1"}' \
  com.holoi.holokit.holokit-app --holokit-scene-test
```

等日志出现 `[DEVICE TEST] COMPLETE=True`，约 4–6 分钟。console 会话在测试完后仍可能
保持打开；不要等待 app 自行退出。测试运行后 app 会留在封面列表。复制新报告与截图：

```sh
xcrun devicectl device copy from --device 00008140-001111563A0B001C \
  --domain-type appDataContainer --domain-identifier com.holoi.holokit.holokit-app \
  --source Documents/HoloKitSceneTests \
  --destination /tmp/holokit-upgrade-20260922/iphone16pro-fixed
```

验收 `complete=true`、26 个结果、所有 `errors` 和 `startupErrors` 为空，
以及新增的 `multiplayerManagerSpawned=true`。检查截图，发现错误就继续修复。
目前这些测试不证明真实手势、双手机 MPC、Watch、录屏音轨或头显光学校准已经通过。

### 证据、保护范围与后续大项

- 修复前真机 JSON 与 26 张截图：
  `/tmp/holokit-upgrade-20260922/iphone16pro-baseline/`。
  型号确认：`iphone16pro-details.json`、`iphone16pro-before-install.png`，同一临时根目录。
- 仓库内持久摘要：`Documentation/Validation/2026-09-22/iphone16-progress.json`。
  原始 XML、xcresult、截图体积较大，仍在上述 `/tmp`，系统清理后可能消失。
  真实旧设置 fixture `device-legacy-settings.save` 是用户数据，禁止提交进 Git。
- 现有未跟踪的 `mono_crash.*`、`holokit-app.slnx`、Watch Xcode userdata
  都没有纳入此次提交，不删除、不覆盖。兄弟项目 mofa-realverse、typed-realverse
  有用户本地改动，不直接改写它们。
- Unity 构建过程中会暂时删除 XR simulation resources、修改图像库与 ProjectSettings；
  成功结束时恢复。不要把构建中的临时变化误提交。
- 四薄项目、产品自己的配置/封面/目录、共享壳解耦、Watch 按产品启用：
  **仍是接下来要实现的架构，不是已完成内容。**
- 已做只读资源闭包分析：`MOFA-asset-closure.json`、`Typed-asset-closure.json`、
  `QRealm-asset-closure.json` 位于临时根目录。MOFA/Typed/QRealm 之间仍有共享
  skybox、renderer、粒子纹理/VFX 对其他游戏包的依赖；应保留 GUID 提取到共享资源包，
  再证明每个小产品不安装其他游戏包也能构建。不能只过滤根项目 scene list 就声称解耦。
- HoloKit SDK 当前资料包含 iPhone 16 Pro，iPhone 18 Pro 的光学校准不能凭空编造；
  等硬件与校准数据可用再做物理验证。

## Preservation and target

The working upgrade starts from checkpoint `72434830` on
`upgrade/unity63-ios27-four-apps`. The App Store reference is `a00a9713`.
Existing package renames and asset GUIDs are preserved. Older assets are evidence
for individual repairs, not a replacement for the current tree. The existing
MOFA and Typed sibling checkouts have local changes and are read-only inputs.

Target: four separately installable iPhone apps, HoloKit, MOFA, Typed, and QRealm,
using Unity 6000.3.24f1 (the latest 6.3 patch returned by Unity CLI on 2026-09-22)
and Xcode 27 / iOS 27 SDK. Keep the current iOS 18 minimum unless a dependency
requires a higher version. iPhone 18 Pro optical calibration and physical AR
behavior require separate validation from successful SDK compilation.

## Architecture

- Keep one source of truth for shared Unity packages in this repository. Preserve
  existing package names and GUIDs; do not perform another broad rename.
- Shared layers are asset data, HoloKit XR/native functionality, app shell and
  common UI, and ARUX. Game packages depend on those layers; the shell must not
  depend on individual games.
- Four thin product projects select packages and product configuration. HoloKit
  contains the current full catalog; MOFA selects its five enabled game scenes;
  Typed selects its seven enabled scenes; QRealm selects QuantumRealm. Preserve
  other existing game packages, without silently adding them to a release.
- Each product owns its bundle identifier, icons/cover, reality catalog, avatar
  and object catalogs, and app feature settings. Reuse UI prefabs and styles.
  Move product-specific references out of shared package defaults by injecting
  the product settings during bootstrap/build, preserving existing HoloKit use.
- Package manifests must declare their actual dependency closure. A smaller
  product must compile without unrelated game packages installed. Merely hiding
  scenes in the full project's build list is insufficient to prove decoupling.
- Make Watch companion packaging an explicit product feature and keep native
  build steps conditional on iOS. Preserve the full app's existing functionality.

## Validation and evidence

Use Unity CLI for editor operations and builds. Logs for this session are in
`/tmp/holokit-upgrade-20260922/`.

1. Static GUID/package audit against the App Store commit:
   `python3 Tools/audit_unity_references.py --output Logs/reference-audit.json`.
   This is a discovery aid, not proof of runtime correctness; cached package
   metadata can be stale and some unresolved GUIDs can predate the upgrade.
2. Authoritative Unity scene audit:
   `unity run . --editor-path /Applications/Unity/6000.3.24f1/Unity.app -- -buildTarget iOS -executeMethod HoloKitUpgradeAudit.Run -logFile Logs/upgrade-audit.log`.
   This loads every enabled scene, checks missing components and serialized
   object references, and verifies the migrated AR plane scripts compile.
3. Build all four isolated product projects to Xcode, then signed device apps.
   Use the native Build Profile: `unity build . --profile iOS --output-path
   Builds/Upgrade/HoloKit --editor-path /Applications/Unity/6000.3.24f1/Unity.app`.
   No custom C# build entry point is required. The initial export used a temporary
   entry point that has been removed; the native profile export has also passed.
   Validate launch, each catalog scene, AR permissions/tracking, stereo and mono
   transitions, multiplayer/spectator mode, recording, audio/haptics, and Watch
   features where enabled. Keep per-app evidence and explicit unsupported tests.
4. Validate iPhone 18 Pro with the real hardware when available. Initial checks
   used Holo iPhone 15 I (iPhone 15 Pro, iOS 27). At the user's request, subsequent
   scene testing targets Holo's iPhone 16 J (iPhone 16 Pro, iOS 26.0), UDID
   `00008140-001111563A0B001C`. The Xcode/iOS SDK remains 27; device runtime
   versions must be reported separately. A simulator cannot prove AR/optical
   correctness.

## Initial findings

- MOFA still references the old app-shell scene path and old permissions package
  location; Typed references the current package locations but has local edits.
- The migrated AR plane C# scripts lacked an assembly boundary and Unity skipped
  compiling them. An assembly reference now assigns them to `Holoi.Library.ARUX`.
- Embedded package manifests still contain old AR Foundation / Netcode / render
  pipeline requirements. Actual resolved versions and assembly usage must guide
  dependency repairs.
- Shared app settings currently reference project-owned catalogs, creating a
  reverse dependency that needs product-specific bootstrap configuration.
- Initial static audit: 258 unresolved GUIDs, no duplicate project GUIDs. These
  include legacy/unused assets and need Unity and baseline classification before
  making repairs; they are not 258 confirmed regressions.
- Unity loaded all 15 enabled scenes and checked 1,757 components. The two
  migrated AR plane scripts resolve to compiled classes. The initial nine broken
  object references have been repaired: obsolete texture overrides were removed
  from QRealm/Sculpture only after checking that the current graph exposes no such
  texture property, and a disabled Finger Ribbon Animator's missing controller
  reference was cleared. The controller was already absent in the App Store
  version; no replacement animation was invented.
- Package-local scene tests additionally found null AR occlusion references in
  Finger Ribbon, Flock, and Hair, which their gameplay code dereferences. Each
  now points to the existing scene camera's occlusion component. Unity 6.3 also
  migrated scene serialization when saving these repairs.
- The initial HoloKit device-target Unity export succeeded with zero errors and
  317 warnings. Xcode 27 signed compilation passed, and the resulting app was
  installed and launched on the iPhone 15 Pro running iOS 27. Unity/Metal startup
  succeeded. This first launch predates the scene repairs and does not establish
  that all games or interactions work.

## Package-local tests

All 13 enabled games own their fixtures and test assemblies under
`Packages/<game>/Tests/Editor`. The app library supplies shared assertions under
`Tests/Editor`; it does not depend on game test assemblies. The checks cover scene
scripts and serialized references, catalog/cover/entrance configuration, network
prefabs, UI panel identifiers, and game-specific required interaction references.

Run with the user's Unity CLI:

```sh
/Users/amber/.unity/bin/unity test . --mode EditMode --filter HoloKit.GameTests \
  --editor-path /Applications/Unity/6000.3.24f1/Unity.app \
  --output Logs/game-editmode.xml --report-format nunit,junit \
  --timeout 1200 -- -buildTarget iOS -logFile Logs/game-editmode.log
```

On 2026-09-22 the initial run passed 83/89, with six failures. After repairing
the references above, the rerun passed **89/89, zero skipped**. NUnit and JUnit
reports are in `/tmp/holokit-upgrade-20260922/game-editmode-fixed*.xml`.

The app library's `Tests/Device` has an opt-in smoke runner compiled only into
development players. Launch a development app with `--holokit-scene-test` to
traverse its own catalog as player and spectator, checking local host startup,
AR tracking, scene exit, and logged errors, and saving screenshots for visual
inspection. `--holokit-test-scene=TypedTheFlock` limits the run to one scene.
Results are written to `Documents/HoloKitSceneTests/results.json` in the app's
data container. This is a startup/lifecycle smoke test, not proof of physical
hand gestures, optical calibration, remote-peer synchronization, or Watch use.

### Existing-install migration

The first development launch with logging enabled exposed a startup blocker that
the silent initial launch could not reveal: the installed App Store preferences
refer to assembly `Holoi.Library.HoloKitApp`, while the renamed runtime assembly
is `Holoi.Library.HoloKitAppLib`. BinaryFormatter threw before bootstrap finished.
The save loader now accepts only the known legacy settings types, migrates to a
versioned JSON document, and retains the original `.save` file. Repeated JSON
saves keep a previous copy; unreadable JSON is preserved before fallback.

Eight migration regression tests passed through `unity test`, including both
assembly names, corrupt data, unexpected object types, null preference maps,
round trips, and previous-file preservation. The old-assembly test also used a
copy of the connected iPhone's actual settings, supplied using
`-legacySettingsFixture /path/to/HoloKitAppGlobalSettings.save` after `--`.
The device fixture remains outside Git.

For iOS launches, the smoke runner also supports process environment variables
`HOLOKIT_SCENE_TEST=1` and optional `HOLOKIT_TEST_SCENE=<scene name>` through
`devicectl --environment-variables`. This avoids relying on Unity's managed
command-line argument forwarding. The migration was verified on the user's
iPhone 16 Pro (`Holo’s iPhone 16 J`, iOS 26.0): the old settings loaded, the
original binary was retained, and the gallery appeared.

### iPhone 16 Pro baseline

The first full device run completed all 13 catalog scenes as both Player and
Spectator (26 cases). All reached AR `SessionTracking`, started a local host,
and returned to the gallery. It was **not a passing run**: 569 logged errors
included repeated instances of an unregistered multiplayer service, missing
audio/depth references, and a Tornado feeder writing a nonexistent VFX property,
plus an insecure HTTP request at startup. The report and 26 screenshots are
preserved outside Git at
`/tmp/holokit-upgrade-20260922/iphone16pro-baseline/`.

The device OS is iOS 26.0; the build toolchain remains Xcode 27 with the iOS 27
SDK. These are separate compatibility checks.

The corresponding fixes register the shared multiplayer prefab and reuse the
pre-instantiated client service through an NGO prefab handler, restore the
existing audio/depth component references, remove Tornado's obsolete direction
feeder, and use HTTPS for the startup permission request. The expanded Unity
CLI run passed **115/115 tests, zero skipped**, including a loopback host/client
spawn-and-despawn test and package-local checks for the device-discovered
references. Evidence: `/tmp/holokit-upgrade-20260922/iphone16-regressions.xml`.
The rebuilt player still needs the full device rerun.

This document describes the intended architecture and work in progress, not a
claim that the four apps have built or run successfully.
