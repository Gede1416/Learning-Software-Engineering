# 消息链 + 中间人（Message Chains / Middle Man）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 14、15 位（合讲）
> 跨书联动：迪米特法则（Law of Demeter）——只和你的朋友说话；外观模式（Facade）

---

## 一、坏代码场景

一路点下去：玩家 → 队伍 → 队长 → 装备 → 武器：

```csharp
public class DungeonSystem
{
    public void OnEnterDungeon(Player player)
    {
        string leaderWeapon = player
            .GetParty()
            .GetLeader()
            .GetEquipment()
            .GetWeapon()
            .Name;

        if (leaderWeapon == "圣剑")
        {
            GrantBonus(player);
        }
    }

    public void GrantBonus(Player player) { /* 全队加成 */ }
}
```

队伍里还有一个「中间人」——只会转发的方法：

```csharp
public class Leader
{
    private Equipment _equipment;
    public Equipment GetEquipment() => _equipment;   // 中间人：只会转发，自己不干事
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：`GetParty()` 返回 null 会怎样？装备系统改结构——Leader 不再有 Equipment，改成 Inventory——这一条链上要改几处？
   `GetEquipment()` 只是把字段转手交出去——调用方为什么需要知道 Leader 内部有 Equipment？（迪米特法则：只和你的朋友说话）
   链上任何一环改名/改签名，所有经过它的调用全部断——这就是「消息链」。）

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 用**隐藏委托（Hide Delegate）**把消息链压扁——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/重构/第二轮-结构型坏味道/DungeonChain.cs](Homework/重构/第二轮-结构型坏味道/DungeonChain.cs)

---

`[进度：阶段三-重构 → Day 10「消息链+中间人」进行中]`
