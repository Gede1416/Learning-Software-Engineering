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

## 三、你的回答（日期）

（待填）

---

## 四、标准解

（待填）

---

## 五、作业

（待填）

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 ✓ / ⑩迭代器+组合 ✓ 核心完成 / ⑪代理模式 → 提问中]`
