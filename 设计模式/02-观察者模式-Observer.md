# 观察者模式（Observer Pattern）

> 来源：《设计模式》GoF 第 5 章 + 《Head First 设计模式》第 2 章

---

## 一、书中定义

GoF 的定义：

> **"定义对象间的一种一对多的依赖关系，当一个对象的状态发生改变时，所有依赖于它的对象都得到通知并被自动更新。"**

Head First 把它总结成一句话：**出版者（Subject）+ 订阅者（Observer）= 观察者模式。**

---

## 二、坏代码场景

假设你在做一个 ARPG 的 Boss 战系统。玩家击杀 Boss 后，游戏里**很多系统**都要响应这件事：

- 🏆 **成就系统**：检查是否解锁"屠龙者"/"初见杀"等成就
- 📋 **任务系统**：检查是否有"击杀 XX"的支线任务目标完成
- 📊 **统计系统**：更新玩家击杀 Boss 的总次数
- 🔊 **音效系统**：播放胜利 BGM
- 💬 **UI 系统**：弹出"Boss 击败！"的横幅

当前代码长这样：

```csharp
public class Boss
{
    public int BossId;
    public string BossName;

    public void OnKilled(Player killer)
    {
        // 死亡逻辑：掉落、动画……
        DropLoot(killer);

        // ====== 然后通知一大堆系统 ======
        AchievementSystem.OnBossKilled(killer, BossId);
        QuestSystem.OnBossKilled(killer, BossId);
        StatisticsSystem.OnBossKilled(killer, BossId);
        SoundSystem.PlayBossVictory(BossName);
        UISystem.ShowBossKillBanner(BossName);
        // 运营说下周还要加「每日首杀奖励」……
    }
}
```

---

## 问题

**Boss 类知道了太多它不该知道的东西。**

1. Boss 的本质职责是「管理自己的战斗逻辑（血量、技能、死亡）」，但它现在知道了成就系统、任务系统、统计系统……每加一个新系统，Boss 要改一行。这和 OCP / SRP 有什么关系？

2. 如果你要做一个单元测试，只测「Boss 死亡时掉落是否正确」，你必须把 AchievementSystem、QuestSystem 全部 mock 一遍——一个纯粹的掉落测试被五个不相关的系统污染了。怎么解耦？

3. 反过来想：这些系统真的关心「Boss」这个对象本身吗？它们关心的只是「发生了一个 Boss 击杀事件」。能不能让 Boss 只知道「有人关心这件事」，但不知道具体是谁？

---

## 你的回答（2026-07-22）

1. **违背 OCP** ✅ —— 每加一个新系统，必须打开 `Boss.OnKilled()` 加一行调用。
2. **事件总线** ✅ —— 你项目中已经在用了。事件总线本质上就是观察者模式的分布式变体。
3. **只关心事件数据（BossId、死亡消息），不关心 Boss 对象本身** ✅ —— 这就是「什么是变化」的答案：变化的不是 Boss，而是「谁关心 Boss 死亡」。

> 这节课你不会陌生——你已经在用的 EventBus，就是观察者模式。

---

## 三、标准重构：从紧耦合到事件驱动

### 3.1 原代码的问题图

```
Boss.OnKilled()
  ├── AchievementSystem.OnBossKilled()   ← Boss 直接依赖 5 个系统
  ├── QuestSystem.OnBossKilled()
  ├── StatisticsSystem.OnBossKilled()
  ├── SoundSystem.PlayBossVictory()
  └── UISystem.ShowBossKillBanner()
```

Boss 是「出版者」，但它直接知道了所有「订阅者」——这是**反向依赖**。高层模块（Boss）依赖了低层模块（各种 System）。

### 3.2 第一步：定义事件

让 Boss 不依赖具体系统，只依赖一个「事件」抽象：

```csharp
// 事件参数——只包含「发生了什么」，不包含「谁要处理」
public class BossKilledEvent
{
    public int BossId;
    public string BossName;
    public Player Killer;
}

// 事件处理器——订阅者实现这个接口
public interface IBossKilledHandler
{
    void OnBossKilled(BossKilledEvent e);
}
```

### 3.3 第二步：Boss 只发布事件

```csharp
public class Boss
{
    public int BossId;
    public string BossName;

    // Boss 只知道「有人关心我死了」，不知道具体是谁
    private List<IBossKilledHandler> _handlers = new();

    public void Register(IBossKilledHandler handler)
    {
        _handlers.Add(handler);
    }

    public void Unregister(IBossKilledHandler handler)
    {
        _handlers.Remove(handler);
    }

    public void OnKilled(Player killer)
    {
        DropLoot(killer);

        var e = new BossKilledEvent { BossId = BossId, BossName = BossName, Killer = killer };
        foreach (var handler in _handlers)
        {
            handler.OnBossKilled(e);
        }
    }
}
```

```csharp
// 成就系统——实现接口，自己关心 Boss 死亡
public class AchievementSystem : IBossKilledHandler
{
    public void OnBossKilled(BossKilledEvent e)
    {
        if (e.BossId == 10) UnlockAchievement("屠龙者");
    }
}

// 音效系统
public class SoundSystem : IBossKilledHandler
{
    public void OnBossKilled(BossKilledEvent e)
    {
        Audio.Play($"BossVictory_{e.BossName}");
    }
}
```

### 3.4 结果图

```
Boss.OnKilled() → 遍历 List<IBossKilledHandler>
                      ├── AchievementSystem.OnBossKilled(e)  ← 实现接口
                      ├── SoundSystem.OnBossKilled(e)        ← 实现接口
                      └── ...
                 Boss 只知道接口，不知道具体实现
```

| 对比 | 改前 | 改后 |
|------|------|------|
| Boss 依赖 | 5 个具体系统 | 1 个接口 `IBossKilledHandler` |
| 加新系统 | 改 Boss 代码 | 新增一个实现类 + 注册 |
| 单元测试 | Mock 5 个系统 | Mock 1 个接口 |

### 3.5 这就是你项目里的 EventBus

你的事件总线本质上就是把 `List<IBossKilledHandler>` 提升到了**全局级别**：

```csharp
// 你项目里可能长这样：
EventBus.Publish(new BossKilledEvent { BossId = 1 });
// 或
EventBus.Subscribe<BossKilledEvent>(OnBossKilled);
```

| 变体 | 出版者知道订阅者？ | 订阅者知道出版者？ |
|------|-------------------|-------------------|
| 经典观察者（上面的 foreach） | 知道接口列表 | 知道要注册到谁 |
| EventBus（你的项目） | 不知道（只发到总线） | 不知道（只从总线收） |

EventBus 多了一层中间人，**出版者和订阅者互相完全不知对方存在**——这是观察者模式的完全解耦形态。

---

## 四、核心结构（GoF）

```
┌──────────┐     订阅/取消订阅     ┌──────────────┐
│  Subject │ ◄────────────────── │  Observer    │
│ (Boss)  │                      │ (接口)       │
│          │ ──────────────────► │ Update()     │
│ 通知所有  │     遍历调用          └──────────────┘
│ Observer │                           △
└──────────┘                           │
                              ┌────────┴────────┐
                              │ ConcreteObserver │
                              │ (Achievement等)  │
                              │ Update() { ... } │
                              └─────────────────┘
```

**两个角色：**
- **Subject（主题/出版者）**：拥有状态，状态变化时通知所有 Observer
- **Observer（观察者/订阅者）**：注册自己到 Subject，收到通知后执行自己的逻辑

---

## 五、推模型 vs 拉模型

你给的回答里说只关心 `BossId`——这触及了一个设计细节：

| 模型 | 做法 | 优点 | 缺点 |
|------|------|------|------|
| **推（Push）** | Subject 把所有数据打包成 Event 推给 Observer | Observer 拿到一切，立即能用 | 改了 Event 结构，所有 Observer 重编译 |
| **拉（Pull）** | Subject 只通知"我变了"，Observer 自己来拉需要的字段 | Observer 按需取数据，Event 不会膨胀 | Observer 要持有 Subject 引用（耦合回升） |

**游戏里的惯例：推模型为主。** 因为 Observer 通常不关心 Subject 对象本身（如音效系统不需要 Boss 引用），事件数据对象（`BossKilledEvent`）足够。

---

## 六、容易踩的坑

### 坑 1：忘记取消订阅
```csharp
// Boss 死了，但成就系统还注册在它身上 → 内存泄漏
// 解决：OnDestroy / OnDisable 里 Unregister
```

### 坑 2：通知顺序依赖
```csharp
// 你先注册了 UI，后注册了音效
// 代码暗含"UI 在音效前执行"的假设
// 如果某天有人改了注册顺序，表现就不一样了
// → 各 Observer 应该互相独立，不依赖通知顺序
```

### 坑 3：Observer 在回调里修改 Subject
```csharp
public class Boss : IBossKilledHandler
{
    public void OnBossKilled(BossKilledEvent e)
    {
        // 在遍历 _handlers 的回调里，这个 Boss 又注册了新 handler
        // → foreach 集合被修改 → 抛异常
    }
}
```
→ 解决：遍历前拷贝列表，或用 `for` 倒序遍历。

---

## 七、跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP —— 加新 Observer 不改 Subject | 《敏捷》第 9 章 |
| SRP —— Boss 只管战斗，通知逻辑委托给观察者 | 《敏捷》第 8 章 |
| DIP —— Boss 依赖 `IBossKilledHandler` 抽象，不依赖具体系统 | 《敏捷》第 11 章 |
| MVC 模式 —— Model（Subject）←→ View（Observer） | 《Head First》第 2 章结尾 |
| 发布-订阅（Pub-Sub）—— EventBus 的架构模式 | 《Head First》第 2 章 "Design Principle 4" |

---

## 八、作业（预计 10 分钟）

你的游戏里有一个**玩家血量系统**。当血量变化时，以下模块需要响应：

- **血条 UI**：更新血量显示
- **濒死特效**：血量 < 20% 时屏幕变红
- **成就系统**：满血通关、丝血反杀等判定
- **AI 系统**：敌人发现玩家残血时切换成追击行为

```csharp
// 当前坏代码：
public class PlayerHealth
{
    public int HP;
    
    public void TakeDamage(int damage)
    {
        HP -= damage;
        
        // 直接调用各系统……
        UIManager.UpdateHealthBar(HP);
        VfxManager.CheckLowHealth(HP);
        AchievementSystem.CheckHealthMilestone(HP);
        AISystem.OnPlayerHealthChanged(HP);
    }
}
```

要求：用观察者模式重构。定义事件 + 接口，`PlayerHealth` 只依赖接口列表。至少实现血条 UI 和濒死特效两个 Observer。

---

`[进度：设计模式-①策略模式 ✓ / ②观察者模式 → 核心讲解完成，等待作业 ✓]`
