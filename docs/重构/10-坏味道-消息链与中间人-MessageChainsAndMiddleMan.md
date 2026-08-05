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

## 三、你的回答（2026-08-03）

### 第一轮

1. 外界只关心队长的装备叫什么，却要通过多重调用才能获得
2. 如果中间有报错也不好定位
3. 和其他类的参数名称耦合严重，修改参数名称时需要修改多个地方（现在有快捷重命名的快捷键了）

### 第二轮（子问题）

尽量压缩到 `Player.leaderWeaponName()` 这种调用吧

## 四、纠错（第一轮，2026-08-03）

- 第 1、2 点 ✅：外界只关心一件事却要走 5 层；链上断点难定位。
- 第 3 点 ⚠️ 合理化：重命名快捷键救不了**结构变化**——Leader 的 `Equipment` 改成 `Inventory` 时，快捷键无法自动重写链，调用方「知道队长→装备→武器」的知识要靠结构修复，不是改名。
- 核心：**调用方知道得太多了**（迪米特法则：只和你的朋友说话）。

**子问题判定（第二轮）**：✅ 通过——`Player.GetLeaderWeaponName()` 正是「隐藏委托」：调用方知识归零。

## 五、标准解（2026-08-03 给出）

Fowler《重构》第 8 章：消息链 → **隐藏委托**（压扁链）；中间人 → **移除中间人**（直接找真主）。

```csharp
public class Player
{
    private Party _party = new Party();

    public string GetLeaderWeaponName() => _party.GetLeaderWeaponName();   // 调用方只和 Player 说话
}

public class Party
{
    private Leader _leader = new Leader();

    public string GetLeaderWeaponName() => _leader.GetWeaponName();
}

public class Leader
{
    private Equipment _equipment = new Equipment();

    public string GetWeaponName() => _equipment.GetWeapon().Name;
}

public class DungeonSystem
{
    public void OnEnterDungeon(Player player)
    {
        if (player.GetLeaderWeaponName() == "圣剑")   // 链没了，知识归零
        {
            GrantBonus(player);
        }
    }

    public void GrantBonus(Player player) { }
}
```

- 效果：装备系统改结构（Equipment → Inventory）只改内部，`OnEnterDungeon` 一行不动——**调用方的知识 = 0 层**。
- 中间人移除的判定：某层只是纯转发、且没人需要中间结果时砍掉。
- 命名修正：方法名用 PascalCase + 动词（`leaderWeaponName` 像字段名）。

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（两轮 + 标准解）
2. 用**隐藏委托（Hide Delegate）**把 5 层链压成 `Player.GetLeaderWeaponName()`——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/重构/第二轮-结构型坏味道/DungeonChain.cs](Homework/重构/第二轮-结构型坏味道/DungeonChain.cs)

---

`[进度：阶段三-重构 → Day 10「消息链+中间人」进行中]`
