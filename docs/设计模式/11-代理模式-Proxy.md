# 代理模式（Proxy）

> 来源：《设计模式》GoF 第 4 章 + 《Head First 设计模式》第 11 章

---

## 一、坏代码场景

你的游戏里有一个 30 级副本「深渊地牢」，进入需要：等级 ≥ 30 + 持有深渊钥匙。目前有**两个入口**：野外传送门、城里 NPC 对话。两个入口的代码都写了同一套检查：

```csharp
// 入口一：野外传送门
public class DungeonPortal
{
    public void Enter(Player player)
    {
        if (player.Level < 30)
        {
            UI.ShowHint("等级不足，无法进入深渊地牢");
            return;
        }
        if (!player.HasKey("abyss_key"))
        {
            UI.ShowHint("缺少深渊钥匙");
            return;
        }
        DungeonSystem.Enter(player);   // ← 真正的进入逻辑
    }
}

// 入口二：城里 NPC 对话
public class DungeonNpc
{
    public void Talk(Player player)
    {
        if (player.Level < 30)
        {
            UI.ShowHint("等级不足，无法进入深渊地牢");
            return;
        }
        if (!player.HasKey("abyss_key"))
        {
            UI.ShowHint("缺少深渊钥匙");
            return;
        }
        DungeonSystem.Enter(player);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时——比如加一个「每天限进 3 次」——具体会在哪里崩盘？

2. 检查逻辑（等级、钥匙）和真正进入副本的逻辑（`DungeonSystem.Enter`）现在是什么关系？它们应该是什么关系？

3. 如果副本以后还有第三个入口（比如「组队面板」）、第四个（「商店出售入场券」），你希望复制几遍检查代码？有没有一种办法，让**检查只写一次**，所有入口都自动带上？

---

## 三、你的回答（2026-08-02）

1. **重复逻辑抽到统一管理，后续修改只修一个地方** ✅ — 正确诊断：检查复制两份，加「每天限进 3 次」要改两个入口，且第三个入口（组队面板）很可能忘改。
2. **检查逻辑是进入的必要条件，抽到 `DungeonSystem.Enter(player)` 里一个地方** ⚠️ — 可行方案，但被三个反例挑战（GM 免检 / 入口规则不同 / SRP），见对话批注。
3. **判断链：创建一个类加判断方法返回它本身，`object1.检查1().检查2().flag`** 💡 — 流畅式校验链是真实存在的设计（校验管线），但当时悬空——链条需要宿主。
4. **场景进入管理器：持有条件判断类 + 场景进入实现类的引用，Enter 里链式判断后进入** ⚠️→✅ — 功能达成（检查集中、入口解耦、判断链有宿主），但姿势是**外观**不是**代理**：管理器「协调两个类」，代理是「和真实对象同接口的一对一替身」。

---

## 四、标准解 —— 保护代理（Protection Proxy）

GoF 原文：**"为另一个对象提供一个替身或占位符，以控制对这个对象的访问。"**

```csharp
// ① 共同接口 —— 入口只认识它，不认识 DungeonSystem
public interface IEnterable
{
    void Enter(Player player);
}

// ② 真实对象 —— 只干「进入副本」，不知道任何资格规则
public class DungeonSystem : IEnterable
{
    public void Enter(Player player)
    {
        Console.WriteLine($"  {player.Name} 进入深渊地牢");   // 传送、加载、实例化……
    }
}

// ③ 代理 —— 和 DungeonSystem 长一样（同一个接口），但多了检查
public class DungeonProxy : IEnterable
{
    private DungeonSystem _real = new();
    private int _todayEnterCount = 0;              // ← 新需求：每天限进 3 次，只改这里

    public void Enter(Player player)
    {
        if (player.Level < 30)          { UI.ShowHint("等级不足"); return; }
        if (!player.HasKey("abyss_key")){ UI.ShowHint("缺少深渊钥匙"); return; }
        if (_todayEnterCount >= 3)      { UI.ShowHint("今日次数已用完"); return; }

        _todayEnterCount++;
        _real.Enter(player);            // ← 检查通过，转手把活交给真正的系统
    }
}

// ④ 两个入口 —— 一行检查都不用写，全部注入代理
public class DungeonPortal { private IEnterable _e = new DungeonProxy(); public void Enter(Player p) => _e.Enter(p); }
public class DungeonNpc    { private IEnterable _e = new DungeonProxy(); public void Talk(Player p)   => _e.Enter(p); }
```

### 三个反例的解法

| 反例 | 代理怎么解 |
|------|-----------|
| GM 免检 | GM 直接持有 `DungeonSystem`，不走代理（或注入免检代理） |
| 公会入口免钥匙 | 做第二个代理 `GuildProxy`，规则不同而已 |
| SRP | `DungeonSystem` 永远不知道资格规则的存在 |

### 辨析（GoF 第 4 章的核心区分）

| 模式 | 结构 | 意图 |
|------|------|------|
| **代理** | 包着同一个接口 | **控制访问**（限制/延迟/远程） |
| **装饰器**（第 3 章） | 包着同一个接口 | 添加行为（增强） |
| **外观**（第 8 章） | 给多个子系统一个新门面 | 简化调用 |

代理三种（GoF）：**保护代理**（权限控制，本章）、**虚拟代理**（延迟加载）、**远程代理**（本地替身调远程）。

---

## 五、作业（预计 5-10 分钟）

用**保护代理**实现技能释放系统。真实技能 `FireballSkill` 只关心伤害；代理 `SkillProxy` 检查**蓝量**和**冷却**，检查通过才委托真实技能。

要求：
1. 实现 `SkillProxy.Cast` 的 3 个 TODO（蓝量 < 15 拒绝 / 冷却 3 秒内拒绝 / 通过后扣蓝、记冷却、委托）
2. 用验收测试骨架跑通 4 个场景（成功 → 冷却拒绝 ×2 → 蓝量不足拒绝）

框架文件：[Homework/SkillProxy.cs](Homework/SkillProxy.cs)（接口、TODO、测试骨架已备好）

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 ✓ / ⑩迭代器+组合 ✓ 核心完成 / ⑪代理模式 ✓ 核心讲解完成，等待作业]`
