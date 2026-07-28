# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概览

这是一个基于 **Unity 2022.3.55f1c1** 的第三人称角色移动系统 Demo，核心是使用状态机组织玩家在地面的 Idle / Walk / Run / Dash / Jump / Fall / Landing 等移动逻辑。项目代码主要使用 C#，注释和文档以中文为主。

## 常用开发与运行方式

- **打开项目**：通过 Unity Hub 打开本项目根目录，目标版本 `2022.3.55f1c1`。
- **运行场景**：在编辑器中打开 `Assets/Scenes/SampleScene.unity` 后点击 Play。
- **构建**：使用 Unity 编辑器菜单 `File > Build Settings` 进行构建；本项目未配置命令行构建脚本。
- **测试**：当前项目没有编写单元测试或 PlayMode 测试。如需添加测试，可通过 `Window > General > Test Runner` 创建，并置于 `Assets/Tests/Editor` 或 `Assets/Tests/Runtime`。
- **重新生成 C# 工程**：在 Unity 中点击 `Edit > Preferences > External Tools > Regenerate project files`，或双击脚本触发 IDE 同步。

## 默认输入键位

输入由新输入系统 `PlayerInputActions` 管理，资源路径为 `Assets/InputAction/Characters/Player/PlayerInputActions.inputactions`：

- 移动：`W / A / S / D`
- 视角：`鼠标移动`
- 缩放：`鼠标滚轮`
- 步行切换：`Left Ctrl`
- 冲刺/突进：`Left Shift` 或 `鼠标右键`
- 跳跃：`Space`
- 退出面板：`Escape`

## 高层架构

### 通用状态机层

- `Assets/Scripts/StateMachine/IState.cs`：状态接口，定义 Enter / Exit / HandleInput / Update / PhysicsUpdate，以及动画事件和触发器回调。
- `Assets/Scripts/StateMachine/StateMachine.cs`：抽象状态机，持有 `currentState` 并提供 `ChangeState` 与更新转发。注意：当前实现中 `OnAnimationExitEvent()` 和 `OnAnimationTransitionEvent()` 错误地转发了 `OnAnimationEnterEvent()`，这是一个已知稳定性问题。

### 玩家移动状态机

- `Assets/Scripts/Characters/Player/Player.cs`：MonoBehaviour 入口。在 `Awake` 中缓存 Rigidbody、Animator、Input、碰撞体工具、相机工具、动画数据，并实例化 `PlayerMovementStateMachine`；在 `Start` 中切换到 `IdlingState`；在 `Update` / `FixedUpdate` 中转发状态机更新。
- `Assets/Scripts/Characters/Player/StateMachines/Movement/PlayerMovementStateMachine.cs`：继承通用 `StateMachine`，持有玩家引用、共享数据 `ReusableData`，并在构造函数中实例化所有具体状态（Idle、Walk、Run、Sprint、Dash、Stopping、Landing、Jump、Fall 等）。
- `Assets/Scripts/Characters/Player/StateMachines/Movement/States/PlayerMovementState.cs`：所有移动状态的基类，封装输入读取、移动/旋转/减速、动画启停、相机回中等公共逻辑。子状态按层次继承：
  - `PlayerMovementState` → `PlayerGroundedState` / `PlayerAirborneState`
  - `PlayerGroundedState` → `PlayerIdlingState`、`PlayerMovingState` → Walk/Run/Sprint/Dash，`PlayerStoppingState` 子类，`PlayerLandingState` 子类
  - `PlayerAirborneState` → `PlayerJumpingState`、`PlayerFallingState`

### 数据驱动

- `Assets/Scripts/Characters/Player/Data/ScriptableObject/PlayerSO.cs`：玩家主数据 ScriptableObject，暴露 `GroundedData` 与 `AirborneData`。
- `Assets/Scripts/Characters/Player/Data/States/`：按地面/空中/各状态拆分的数据类（如 `PlayerGroundedData`、`PlayerRunData`、`PlayerJumpData` 等），用于在 Inspector 中序列化配置。
- `Assets/Scripts/Characters/Player/Data/States/PlayerStateReusableData.cs`：跨状态共享的运行时数据容器，包括输入、速度修改器、旋转目标、减速力、相机回中配置等。

### 输入

- `Assets/Scripts/Characters/Player/Utilities/Input/PlayerInput.cs`：封装 `PlayerInputActions`，在 `OnEnable/OnDisable` 中启用/禁用输入，并提供 `DisableActionFor` 用于临时禁用某个 Action。
- `Assets/InputAction/Characters/Player/PlayerInputActions.inputactions`：Input System 资源，定义 Player Action Map 与绑定。

### 相机与 UI

- `Assets/Scripts/Camera/CameraZoom.cs`：挂载在 Cinemachine 虚拟相机上，通过 `CinemachineFramingTransposer` 和 `CinemachineInputProvider` 实现滚轮缩放。
- `Assets/Scripts/UI/QuitPanel.cs`：退出确认面板，监听按钮与 `Escape` 键，暂停/恢复游戏时间。

## 已知问题与注意事项

1. **动画事件转发错误**：`StateMachine.OnAnimationExitEvent()` 与 `OnAnimationTransitionEvent()` 当前调用了 `currentState?.OnAnimationEnterEvent()`，应分别改为调用 `OnAnimationExitEvent()` 与 `OnAnimationTransitionEvent()`。
2. **共享数据命名不一致**：代码中存在 `ReuseableData` / `ReusableData` 混用，新增或修改状态时应统一使用 `ReusableData` 命名，避免引用断链。
3. **无自动化测试**：当前没有单元测试或 PlayMode 测试；修改状态机核心逻辑后应在 `SampleScene` 中手动回归移动、跳跃、冲刺、落地等流程。
4. **脚本编码**：部分历史脚本使用 GB2312/GBK 编码保存，直接用 UTF-8 读取时中文注释可能乱码；编辑后建议统一保存为 UTF-8。

## 扩展建议

- 新增状态应继承 `PlayerMovementState` 或其地面/空中子类，在 `PlayerMovementStateMachine` 中注册实例，并通过已有数据类或新增 Serializable 数据类配置参数。
- UI 与状态机解耦：如需新增血条等 UI，建议通过独立组件（如 `PlayerHealth` + `PlayerHealthBarUI`）以事件/回调方式通信，不直接侵入移动状态机。
