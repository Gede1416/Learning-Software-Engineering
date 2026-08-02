# 模型-视图-控制器（MVC）

> 来源：《Head First 设计模式》第 14 章 复合模式（MVC 是多种模式的组合）
> 本章为设计模式阶段收官章——前面所有模式在这里汇合

---

## 一、坏代码场景

你的游戏有个人物状态栏：左上角显示 HP/MP，玩家按 **H** 键喝药、按 **J** 键放技能，HP 归零显示「你死了」。你写了个类，一肩挑：

```csharp
public class PlayerStatusUI
{
    private Player _player;

    public void Update()
    {
        // ① 读数据 —— 从模型拿状态
        int hp = _player.Hp;
        int mp = _player.Mp;

        // ② 处理输入 —— 键盘快捷键直接改数据
        if (Input.GetKeyDown(KeyCode.H)) _player.UsePotion();
        if (Input.GetKeyDown(KeyCode.J)) _player.CastSkill();

        // ③ 画界面 —— 把状态画到屏幕上
        DrawText($"HP: {hp}/{_player.MaxHp}", new Vector2(10, 10));
        DrawText($"MP: {mp}/{_player.MaxMp}", new Vector2(10, 30));
        if (_player.Hp <= 0) DrawText("你死了", new Vector2(10, 50));
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时——比如加第二个 UI（小地图上的迷你血条）、把键盘操作换成触屏按钮、或者加一段「复活提示」动画——**具体会在哪里崩盘**？

2. 「读数据」「处理输入」「画界面」这三件事挤在同一个类里——它们的**变化频率**一样吗？谁最常改？

3. 你已经学过：观察者模式（②）、策略模式（①）、组合模式（⑩）——如果分别把这三件事抽出去，**每个模式恰好负责拆哪一件**？想想「数据变化了谁要知道」「输入来了该问谁」「界面元素怎么组织」三个问题。

---

## 三、你的回答（2026-08-02）

1. **全部逻辑放在 Update 里违反 SRP；UI 会最快崩、最频繁改动** ✅ — 正确：一肩挑违反单一职责；UI 是变化最频繁的部分，崩盘点就在这。
2. **变化频率从高到低：UI → 输入操作 → 数据** ✅ — 教科书级的判断（Head First 原话：视图最容易变，模型最稳定）。
3. **数据模块注册变化事件通知 UI（观察者）；输入用事件总线抽离（观察者变体）；UI 模块用接口抽离；Player 拆成 数据模块 / 操作事件接收器 / UI 展示接口** ⚠️→✅ — M/V/C 三分拆对了，观察者用对了两次；但题目点名的三个模式只用了观察者一个——**策略和组合还没有归宿**（待补）。
4. **策略：换掉外部操作触发器（控制器），不改动视图/模型** ✅ — 控制器是可整体替换的行为对象，视图把它当策略持有——正中。
5. **组合：Player 被多个 UI 关心，用组合 + 迭代器统一刷新 UI 元素树** ✅ — UI 元素组织成树（Panel 装 Text/Button），递归遍历刷新；第十章两件套直接用上。

---

## 四、标准解 —— MVC = 观察者 + 策略 + 组合

MVC 不是 GoF 的 23 个模式之一（Head First 第 14 章称其为**复合模式**）——它是你学过的模式的合体：

| 角色 | 模式 | 职责 |
|------|------|------|
| **模型 Model** | 被观察者（观察者） | 存数据 + 业务规则，**不知道任何 UI 存在** |
| **视图 View** | 观察者 + 组合 | 订阅模型事件；UI 元素树递归渲染 |
| **控制器 Controller** | 策略 | 接收输入 → 调模型；可整体替换（键盘/触屏/AI） |

```csharp
// ====== ① 模型（被观察者，不知道 UI 存在）======
public class Player
{
    public int Hp { get; private set; } = 100;
    public int Mp { get; private set; } = 100;

    public event Action<int> OnHpChanged;   // ← 观察者：数据变化通知
    public event Action<int> OnMpChanged;

    public void UsePotion() { Hp = Math.Min(Hp + 30, 100); OnHpChanged?.Invoke(Hp); }
    public void CastSkill() { Mp -= 15; OnMpChanged?.Invoke(Mp); }
    public void TakeDamage(int dmg) { Hp -= dmg; OnHpChanged?.Invoke(Hp); }
}

// ====== ② 控制器（策略：可整体替换的行为对象）======
public interface IController
{
    void HandleInput(Player player);
}

public class KeyboardController : IController
{
    public void HandleInput(Player player)
    {
        if (Input.GetKeyDown(KeyCode.H)) player.UsePotion();
        if (Input.GetKeyDown(KeyCode.J)) player.CastSkill();
    }
}

public class TouchController : IController { /* 触屏按钮 → 同样调 player.UsePotion() */ }
public class AIController    : IController { /* 自动战斗 */ }

// ====== ③ 视图（观察者 + 组合：UI 元素树）======
public abstract class UIElement          // ← 组合：叶子/容器统一接口
{
    public abstract void Draw();
}

public class UIText : UIElement          // 叶子
{
    public string Text;
    public override void Draw() => DrawText(Text);
}

public class UIPanel : UIElement         // 容器
{
    private List<UIElement> _children = new();
    public void Add(UIElement e) => _children.Add(e);
    public override void Draw() { foreach (var c in _children) c.Draw(); }  // 递归刷新
}

// 视图本体：订阅模型 + 持有控制器策略
public class PlayerStatusUI
{
    private Player _player;
    private IController _controller = new KeyboardController();   // ← 策略，想换就换
    private UIPanel _root = new();
    private UIText _hpText = new();

    public PlayerStatusUI(Player player)
    {
        _player = player;
        _root.Add(_hpText);
        _player.OnHpChanged += hp => _hpText.Text = $"HP: {hp}/100";   // ← 观察者：只刷新对应元素
    }

    public void Update()
    {
        _controller.HandleInput(_player);  // 输入 → 控制器（策略）
        _root.Draw();                      // 渲染 → 组合树递归
    }
}
```

### 你的两个答案怎么落进标准解

- 「换掉外部操作触发器」= 换 `_controller` 字段——视图和模型一个字节都不改，这就是策略。
- 「组合+迭代统一刷新」= `UIPanel.Draw()` 递归遍历子树——第十章的两件套在这合体。

### 为什么 Q2 的频率排序是拆分的依据

UI 天天改、输入偶尔改、数据几乎不改——所以三个对象按频率分层：改 UI 不动模型，改输入不动视图，数据变了只有关心它的人被通知（观察者）。这就是 MVC 的价值：**把变化频率不同的东西放进不同的类**。

---

## 五、作业（预计 5-10 分钟）

给上面的 MVC 骨架做**收官验证**——两个 TODO：

1. **加第二个视图** `MiniHpBar`（小地图迷你血条）：订阅同一个 `Player.OnHpChanged`，模型一个字节不改
2. **换控制器**：写一个 `AIController`（每帧自动喝药），插进 `PlayerStatusUI` 替换 `KeyboardController`，视图零改动

验证：模型攻击一次 → 两个视图都收到事件；换控制器 → 行为变、视图不动。

框架文件：[Homework/MvcDemo.cs](Homework/MvcDemo.cs)

### 验收（2026-08-02）

四项全过：模型一次变化 → 双视图各刷一次；键盘控制器喝药；换 AIController 视图零改动；最终 HP=100。

踩坑记录：`MiniHpBar._uiPanel` 未初始化 NRE——未初始化字段问题第三次出现（CombatUnit / MiniHpBar / AIPlayerMnager），**字段声明即初始化**已成必检项。延伸：`AIPlayerMnager` 是 MonoBehaviour 式的游戏循环驱动组件（Init 先行、Update 每帧驱动控制器），思路正确；但 `usePotionController` 在 Init 赋值，Update 先于 Init 调用仍会 NRE——生命周期约定要显式。

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 ✓ / ⑩迭代器+组合 ✓ / ⑪代理模式 ✓ / ⑫MVC ✓（作业验收通过 2026-08-02）——**设计模式阶段 12 章全部毕业**]`
