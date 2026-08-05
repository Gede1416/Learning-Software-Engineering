# 神秘命名（Mysterious Name）

> 来源：《重构：改善既有代码的设计（第 2 版）》Martin Fowler 第 3 章 —— 坏味道清单第 1 位

---

## 一、坏代码场景

重构 Day 1 的场景来自**你自己上周的收官作业** [Homework/设计模式/第三轮-收尾补全/MvcDemo.cs:119-134](Homework/设计模式/第三轮-收尾补全/MvcDemo.cs#L119-L134)：

```csharp
public class AIPlayerMnager
{
    private Player _player;
    private IController usePotionController;

    public void Init(Player player)
    {
        _player = player;
        usePotionController = new AIController();
    }

    public void Update()
    {
        usePotionController.HandleInput(_player);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：崩的可能不是运行时——程序跑得好好的。崩的是人。）

## 三、你的回答（2026-08-03）

1. 第一次尝试：`Init` 是什么 / `Update` 又是什么 / `IController` 是否太抽象 —— ⚠️ 方向对（命名领域），靶子错：`Init`/`Update` 是游戏循环惯例名，`IController` 是 MVC 作业里最正确的抽象名。
2. 修正后：`_usePotionController`、`AIPlayerManager` ✅ —— 两处都抓到了：类名拼写错误（Mnager→Manager）、字段名违反 `_` 前缀约定。

## 四、标准解 —— 神秘命名（Mysterious Name）

Fowler 原文（《重构（第 2 版）》第 3 章，坏味道清单第 1 位）：

> 如果你不知道一段代码在做什么，其他人当然也不知道。

Fowler 名言（《重构》前言）：

> 任何傻瓜都能写出计算机能读懂的代码。只有写出人类容易读懂的代码，才是优秀的程序员。

### 崩盘点：不崩在运行时，崩在搜索与阅读（人）

- `AIPlayerMnager` 拼错一个字母 → grep/Ctrl+F 搜 `AIPlayerManager`（正确拼写）**找不到它** → 需求变化时（加自动攻击/换 AI 策略）你漏改它，或另写一个同名类 → 两个"AI 管理器"并存，行为分叉。
- `usePotionController` 违反本文件 `_` 前缀约定（旁边躺着 `_player`）→ 读者误判它是局部变量；且名字描述「动作」（动词短语），字段名应该说「它是什么」。

### 正确写法（只改名字，行为零变化 —— 重构定义的活例子）

```csharp
public class AIPlayerManager
{
    private Player _player;
    private IController _controller;   // 它是什么：绑定的控制器

    public void Init(Player player)
    {
        _player = player;
        _controller = new AIController();
    }

    public void Update()
    {
        _controller.HandleInput(_player);
    }
}
```

### 配套手法：重命名（Rename）

最简单、最安全的重构：机械替换标识符，行为不可能变。IDE 的 F2 一键全量替换，靠编译器和测试兜底。

### 跨书联动

《代码整洁之道》第 2 章「有意义的命名」：名字要回答三个问题——**它是什么？为什么存在？做什么？**（阶段四教材，届时系统展开）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 动手修复 [Homework/设计模式/第三轮-收尾补全/MvcDemo.cs](Homework/设计模式/第三轮-收尾补全/MvcDemo.cs) 里的命名——**铁律：只改名字，不改任何行为**
3. 完成骨架 [Homework/重构/第一轮-最常踩的坏味道/DailyRewardSystem.cs](Homework/重构/第一轮-最常踩的坏味道/DailyRewardSystem.cs) 里的 TODO

## 六、作业验收（2026-08-03）

- MvcDemo.cs：`AIPlayerMnager` → `AIPlayerManager`、`usePotionController` → `_controller` ✅ 编译通过（0 错误）
- DailyRewardSystem.cs 四轮修正全部通过：
  1. 改名 `R()` → `Reward()`、`G()` → `AllRewardCount()` ✅
  2. 删除撒谎的 `hp` 字段 ✅
  3. 引入 `IReward` 策略并接线 `_reward.Reward(this)`（阶段二知识迁移，替代硬编码）✅
  4. `Init` 转 public 消除 NRE 隐患；清除全部悬空 TODO 注释 ✅
- 教训：注释描述的对象不存在后，注释本身就在撒谎——与「神秘命名」同源的坏味道。

---

`[进度：阶段三-重构 → Day 1「神秘命名」✓（作业验收通过 2026-08-03）]`
