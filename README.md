# 故理

> 以中国古代物理原理为核心玩法的第一人称 3D 解谜游戏

**中国大学生计算机设计大赛 · 国家三等奖** · 2025.5 – 2025.6 · 独立开发（策划 / 程序 / 关卡 / 打包）

**引擎版本**：Unity 2022.3 LTS · URP · C#

📺 演示视频：[B 站](https://www.bilibili.com/video/BV1XmkxBeESV) · [仓库内视频文件](docs/demo.mp4)

---

## 游戏内容

玩家扮演一名穿越到古代的初中生，需要利用**杠杆、滑轮、小孔成像**三种物理原理解开谜题。玩法灵感来自《墨经》中的物理学记载。

| 关卡 | 物理机制 | 玩法目标 |
| --- | --- | --- |
| 第一关 | 杠杆平衡 | 在杠杆两端放置重物，让 3 组杠杆同时保持平衡 |
| 第二关 | 滑轮组 | 借助滑轮组省力，把直接搬不动的重物抬起来收进柜子 |
| 第三关 | 小孔成像 | 调整蜡烛与光屏的位置，在光屏上得到清晰的倒立成像 |

**操作**：`WASD` 移动 · 鼠标右键转视角 · 鼠标左键拾取 / 拖拽 · 滚轮缩放视野 · `ESC` 暂停

---

## 技术实现

### 目录结构

```
Assets/Script/
├── Player/          第一人称控制器（CharacterController + 自定义重力）、射线交互
├── KeyholeImage/    小孔成像系统
├── Pry/             杠杆谜题（PlankController / BalanceCondition / PryController）
├── Pulley/          滑轮谜题（Obi 绳索、滑轮转动、载重计算、平衡判定）
├── NPC/             NPC 与对话
└── Tools/
    ├── Interaction/Drag/   拖拽系统（工厂 + 策略）
    ├── Managers/           EventsManager / GameManager / UIManager / DialogueManager
    ├── DialogueTriggerWays/ 对话触发（抽象基类 + 子类）
    └── Interface/          各系统接口
```

### 1. 小孔成像系统

用 URP Decal Projector 把画面投影到光屏上，并让成像大小随蜡烛与光屏的位置实时变化。

- **运行时材质克隆**：`ScreenSetting` 用 `new Material(projector.material)` 复制一份 Decal 材质再回写，避免运行时修改污染共享材质资源。
- **光路遮挡检测**：`KeyholeProjector` 每帧从小孔中心沿 forward 方向发一条 `Physics.Raycast`（限定 `Light` 层），命中蜡烛判定为光路被挡，成像消失。
- **成像缩放**：按小孔到物体与到光屏的距离比计算出 `_ScaleFactor`，每帧写入自定义 Shader Graph（`MyDecalShader`）。着色器在 **UV 空间**做以中心为锚点的缩放，因此成像尺寸随物距增大而变小、随像距增大而变大，方向与真实光学一致。
- **倒立成像**：由一台挂在烛焰上的 RenderTexture 采集相机完成（负的 orthographic size 使纵轴翻转），Decal 负责把它投到光屏表面。

### 2. 拖拽交互系统（Zenject + 工厂策略）

不同物体需要不同的拖拽方式，用依赖注入 + 工厂把"选哪种拖拽"和"拖拽怎么做"拆开。

- `DragInstaller : MonoInstaller` 集中绑定 `MouseWorldPosition`、`DragInitialSetting`、`IDragFactory`，三个玩法场景各有一个 `SceneContext`。
- `DragInteraction` 通过 `[Inject]` 取得依赖，在 `OnMouseDown` 时向工厂申请策略。
- `DragFactory` 在运行时按目标 `Rigidbody` 的质量与运动学状态分派三种策略：

| 目标状态 | 策略 | 行为 |
| --- | --- | --- |
| 有刚体且非 kinematic，质量 < 20 | `DragOfFree` | 自由拖拽，清除速度与角速度后 `MovePosition` |
| 有刚体且非 kinematic，质量 ≥ 20 | `DragOfLimitY` | 重物，锁定 Y 轴，只能水平拖动 |
| 无刚体或 kinematic | `DragOfLimitXY` | 沿单一轴移动 |

三种策略实现同一个 `IDragInteraction` 接口，新增拖拽方式不需要改调用方。

### 3. 事件总线

`EventsManager` 是一个静态类，暴露 8 个 `public static event Action`（关卡完成、杠杆平衡 / 失衡、点击聚焦、滑轮平衡、对话流程等），并用 `TriggerXxx()` 方法封装 `?.Invoke()`，避免外部直接触发。

关卡完成、杠杆状态、滑轮状态、UI 面板、对话流程之间因此互不持有引用。

### 4. 对话触发框架

`BaseDialogueTrigger` 抽象基类统一了**延迟触发**与**重入守卫**（`triggerDelay` / `canRetrigger` / 正在打字时不再触发），子类以两种方式扩展触发时机：

- `TimedDialogueTrigger`：重写 `Initialize()`，延迟一段时间后触发
- `QuestDialogueTrigger`：订阅 `EventsManager` 事件，在关卡完成时触发

### 5. 物理谜题

**杠杆关**：运行时配置 `HingeJoint` 作为支点，`BalanceCondition` 以**旋转角度 + 角速度双阈值**配合持续时长判定是否稳定，`PryController` 汇总 3 组杠杆的状态并给予灯光反馈。

**滑轮关**：接入 **Obi Rope** 绳索物理与 `HingeJoint` 滑轮。

- `LoadCalculation` 通过碰撞进出累加每个平台上 `Weight` 标签物体的总质量；
- `PulleyBalanceController` 把两侧载重加上平台 / 动滑轮组自重后比较，以**「较重的一端同时也是较高的一端」**作为省力平衡条件；
- 判定成立后由较重端决定滑轮的扭矩方向（`PulleyRotation`），用冲量扭矩驱动滑轮转动。

### 使用的设计模式

单例（各 Manager）、工厂 + 策略（拖拽）、观察者（`EventsManager`）、依赖注入（Zenject）、模板方法（对话触发基类）

---

## 运行项目

> ⚠️ 本仓库**不包含第三方资源**（版权限制与体积原因），clone 后需要补齐才能运行。排除清单见 `.gitignore`。

| 缺失内容 | 补齐方式 |
| --- | --- |
| **Obi Rope** | 付费插件，需在 Asset Store 购买后导入 `Assets/Obi`。绳索物理依赖它，缺失时脚本无法编译 |
| **TextMesh Pro** | 菜单 `Window > TextMeshPro > Import TMP Essential Resources` |
| 天空盒 / 家具 / 木箱等资源包 | 已从仓库排除，可自行用任意资源替代 |
| 环境贴图（约 278MB） | 已从仓库排除，可自行用任意贴图替代 |

**步骤**

1. 用 Unity Hub 以 **Unity 2022.3.x** 打开本目录
2. 按上表补齐缺失资源
3. 打开 `Assets/Scenes/StartMenu.unity`，点击 Play

> `Assets/Plugins/Zenject` 已包含在仓库中（MIT 协议），无需额外导入。

### 关于 NPC 对话功能

`Assets/Script/NPC/` 下有一个接入 DeepSeek API 的 NPC 对话原型。它需要 `Assets/StreamingAssets/config.json` 提供密钥，该文件已被 `.gitignore` 排除：

```json
{ "deepSeekApiKey": "sk-你的key" }
```

没有该文件时功能会直接跳过，不影响项目其余部分运行。

---

## 已知限制与后续计划

- 杠杆平衡采用角度 / 角速度阈值判定，**未做真实的力矩计算**，后续计划改为按力矩与力臂计算
- Obi 绳索物理偶有抖动
- 目前为 3 个玩法关卡 + 主菜单 / 选关，内容量较小，缺少新手引导
- 场景美术为 ProBuilder 基础几何体，非美术重点

---

## 关于

王胤 · 皖西学院 通信工程

- GitHub：[@JunMoChang](https://github.com/JunMoChang)
- 技术博客：[CSDN / Mo_Chang](https://blog.csdn.net/Mo_Chang?type=blog)
- 邮箱：junmochang@126.com

本项目为个人独立作品，采用 MIT 协议。第三方资源版权归各自作者所有。
