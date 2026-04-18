# MovementStstem

一个基于 **Unity 2022 LTS** 的第三人称角色移动系统 Demo，使用状态机组织玩家地面/空中移动逻辑。

## 环境要求

- Unity `2022.3.55f1c1`
- Windows / macOS / Linux（可运行 Unity 2022 LTS）

## 主要特性

- 状态机驱动：Idle / Walk / Run / Dash / Jump / Fall / Landing
- 新输入系统（Input System）
- Cinemachine 跟随与缩放
- 数据驱动参数（PlayerSO + 分状态数据）
- 基础 UI（退出确认面板）

## 项目结构（核心）

- `Assets/Scripts/StateMachine/`：通用状态机接口与基类
- `Assets/Scripts/Characters/Player/`：玩家逻辑、状态机、状态与数据
- `Assets/InputAction/Characters/Player/`：输入映射
- `Assets/Animations/Characters/Player/`：动画资源与控制器
- `Assets/Scenes/SampleScene.unity`：示例场景

## 操作说明（默认键位）

- 移动：`W A S D`
- 视角：鼠标移动
- 缩放：鼠标滚轮
- 步行切换：`Left Ctrl`
- 冲刺/突进：`Left Shift` 或鼠标右键

## 快速开始

1. 使用 Unity Hub 打开本项目目录。
2. 确认编辑器版本为 `2022.3.55f1c1`（或兼容 2022.3 LTS）。
3. 打开场景 `Assets/Scenes/SampleScene.unity`。
4. 点击 Play 运行。

## 依赖包（节选）

- `com.unity.inputsystem`
- `com.unity.cinemachine`
- `com.unity.ugui`
- `com.unity.probuilder`

## 备注

项目内有一份更详细的中文分析文档：[项目说明文档.md](./项目说明文档.md)。
