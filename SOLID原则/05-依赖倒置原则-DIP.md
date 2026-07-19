# 依赖倒置原则（Dependency Inversion Principle, DIP）

> 来源：《敏捷软件开发：原则、模式与实践》— Robert C. Martin，第 11 章

---

## 一、书中定义

> **"高层模块不应该依赖于低层模块。二者都应该依赖于抽象。"**
> **"抽象不应该依赖于细节。细节应该依赖于抽象。"**
> — Robert C. Martin

Martin 的经典图示：

```
❌ 传统依赖方向：
   高层模块 → 低层模块（高层直接依赖底层实现）

✅ DIP 依赖方向：
   高层模块 → 抽象 ← 低层模块（两者都只依赖抽象）
```

---

## 二、坏代码场景

横版动作游戏的输入系统：

```csharp
public class InputHandler
{
    private KeyboardDevice keyboard = new KeyboardDevice(); // 焊死在键盘上

    public void HandleInput()
    {
        if (keyboard.GetKeyDown(Key.A))  Jump();
        else if (keyboard.GetKeyDown(Key.B)) Attack();
    }
}
```

---

## 三、诊断

### 谁依赖谁？

**高层模块 `InputHandler` 直接依赖了低层模块 `KeyboardDevice`。** 证据：`new KeyboardDevice()` 和 `keyboard.GetKeyDown()` —— 两处都把 `InputHandler` 焊在具体硬件上。

### 三个下不了手的痛点

| 痛点 | 位置 |
|------|------|
| ① `new KeyboardDevice()` | 字段声明就绑死了类型，换 Joy-Con 必须改这行 |
| ② `keyboard.GetKeyDown(Key.A)` | 具体的方法调用——Joy-Con 的 API 可能叫 `IsButtonPressed(Button)`，签名完全不同 |
| ③ `Key.A` / `Key.B` | 键盘枚举值——Switch 只有 ABXY 按键和摇杆，语义映射丢失（键盘 A 键 ≠ Switch A 键） |

> 你的回答"加入一个中间层将外层输入规范化"——正是针对痛点 ③ 和 ②，方向对。

---

## 四、重构方案

```csharp
// ① 定义抽象 —— 这是 DIP 的核心
public interface IInputDevice
{
    bool IsActionTriggered(InputAction action); // 不是 GetKeyDown，不是 IsButtonPressed，而是"动作"
}

// ② 抽象出"动作"语义，不再用硬件按键枚举
public enum InputAction { Jump, Attack, Menu, Interact }

// ③ 每个硬件 = 一个实现细节
public class KeyboardInput : IInputDevice
{
    public bool IsActionTriggered(InputAction action)
    {
        return action switch
        {
            InputAction.Jump   => Win32API.CheckKeyState(Key.Space),
            InputAction.Attack => Win32API.CheckKeyState(Key.J),
            _ => false
        };
    }
}

public class JoyConInput : IInputDevice
{
    public bool IsActionTriggered(InputAction action)
    {
        return action switch
        {
            InputAction.Jump   => NintendoSDK.IsPressed(JoyConButton.B),
            InputAction.Attack => NintendoSDK.IsPressed(JoyConButton.Y),
            _ => false
        };
    }
}

// ④ 高层模块只依赖抽象，不关心硬件是谁
public class InputHandler
{
    private readonly IInputDevice _input; // 注入了抽象，不再 new 具体硬件

    public InputHandler(IInputDevice input) => _input = input; // 构造函数注入

    public void HandleInput()
    {
        if (_input.IsActionTriggered(InputAction.Jump))  Jump();
        else if (_input.IsActionTriggered(InputAction.Attack)) Attack();
    }
}
```

**依赖方向翻转完毕**：

```
之前：InputHandler → KeyboardDevice → Win32API
现在：InputHandler → IInputDevice ← KeyboardInput / JoyConInput
```

加新手柄类型 = 新建一个 `IInputDevice` 实现类。`InputHandler` 一行不动。

---

## 五、关键心得

> **DIP 不是在说"不要用具体类"，而是在说"高层策略不应该被底层细节制约"。**

`InputHandler` 的职责是"把输入映射到动作"，这是一个高层策略。这个策略不应该因为换了键盘/手柄/触屏就被迫重写。接口 `IInputDevice` 就是隔离层。

Martin 在书中的判断标准：
> **"高层模块包含的是应用程序的业务逻辑——这些逻辑不应该随着底层实现的改变而改变。"**

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 依赖注入（DI）——DIP 的实现手段 | 《敏捷软件开发》第 11 章 + 众多 DI 框架模式 |
| 适配器模式——`KeyboardInput` 实际上是 Key → InputAction 的适配器 | 《设计模式》GoF 第 4 章 |
| ISP 与 DIP 联动——`IInputDevice` 是一个小接口，调用方只依赖它需要的 | 前面学过的 ISP |

---

## 七、SOLID 五原则全回顾

| 原则 | 一句话 | 违反时的信号 |
|------|--------|-------------|
| **S**RP | 一个类只为一个人服务 | 改一个功能要动到不相关的代码 |
| **O**CP | 加新功能 = 加新代码，不改旧代码 | 每次加需求都要改已有类 |
| **L**SP | 子类能透明替换父类 | 子类抛父类没有的异常 / 拒绝父类接受的输入 |
| **I**SP | 接口最小化，不强迫冗余实现 | 类里有 `NotImplementedException` |
| **D**IP | 高层和底层都依赖抽象 | `new` 具体类的代码散落各处 |

---

## 八、作业（预计 5 分钟）

下面是一个简单的日志系统，`GameEngine` 直接依赖 `FileLogger`：

```csharp
public class FileLogger
{
    public void Log(string msg)
    {
        System.IO.File.AppendAllText("log.txt", msg + "\n");
    }
}

public class GameEngine
{
    private FileLogger logger = new FileLogger();

    public void Start()
    {
        logger.Log("游戏启动");
    }
}
```

**要求**：用 DIP 重构。定义日志接口、让 `FileLogger` 实现它、`GameEngine` 通过构造函数接收接口。额外加分：再写一个 `ConsoleLogger` 实现。

---

---

## 九、作业审阅

```csharp
// ✅ 结构正确
public interface ILog { void Log(string msg); }
public class GameEngine
{
    private readonly ILog _logger;
    public GameEngine(ILog _logger) => _logger = _logger;
    public void Start() => _logger.Log("游戏启动");
}

// ⚠️ ConsoleLogger 名字叫 Console，实现却是写文件
// 应改为：Console.WriteLine(msg);
```

### 修正

```csharp
public interface ILogger { void Log(string msg); }
public class ConsoleLogger : ILogger { public void Log(string msg) => Console.WriteLine(msg); }
public class FileLogger : ILogger { public void Log(string msg) => System.IO.File.AppendAllText("log.txt", msg + "\n"); }
```

---

`[进度：SOLID-①SRP ✓ / ②OCP ✓ / ③LSP ✓ / ④ISP ✓ / ⑤DIP ✓ → SOLID 全部完成]`
