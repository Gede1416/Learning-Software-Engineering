# 重复的 switch + 循环（Repeated Switches / Loops）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 12、13 位（合讲）
> 跨书联动：Day 6 的伏笔兑现——**逻辑型差异用多态**；策略/状态模式；以管道取代循环

---

## 一、坏代码场景

怪物类型 switch 重复两处 + 背包手写 for：

```csharp
public class Monster
{
    public string Type;   // "Slime" "Wolf" "Dragon"
    public int Hp;
}

public class CombatSystem
{
    // switch #1：伤害计算——每种怪规则不一样
    public int CalcDamage(Monster m, Player player)
    {
        switch (m.Type)
        {
            case "Slime":  return 5;
            case "Wolf":   return 8 + (player.Hp < 30 ? 5 : 0);   // 残血暴击
            case "Dragon": return 25 + 10;                        // 范围加成
            default:       return 3;
        }
    }
}

public class RewardSystem
{
    // switch #2：同样的 Type 分支再来一遍
    public int GetGold(Monster m)
    {
        switch (m.Type)
        {
            case "Slime":  return 10;
            case "Wolf":   return 20;
            case "Dragon": return 200 + m.Hp / 10;   // 龙按血量给
            default:       return 5;
        }
    }
}

public class Backpack
{
    public List<Item> Items = new();

    // 手写 for：找道具
    public Item Find(string name)
    {
        for (int i = 0; i < Items.Count; i++)
            if (Items[i].Name == name) return Items[i];
        return null;
    }

    // 手写 for：统计总价值
    public int TotalValue()
    {
        int sum = 0;
        for (int i = 0; i < Items.Count; i++) sum += Items[i].Value;
        return sum;
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：策划加新怪物「哥布林 Goblin」——要改几个地方？
   两个 switch 的 Type 分支一模一样——这叫**重复的 switch**：同样的分支结构散落在不同类里，加一个类型 = 霰弹枪（Day 6 复习）。
   狼残血暴击、龙按血量掉金——行为真的不同，这是**逻辑型差异**——Day 6 的话还记得吗：逻辑型差异用什么？
   背包那两个 for 循环在「手把手教机器怎么找/怎么加」——有没有更声明式的写法？）

## 三、你的回答（2026-08-03）

1. 多个结构相似的 switch，新需求同步修改，容易漏改出 bug
2. 抽离行为到接口，使用多态
3. 使用 LINQ 简化表达，或将循环结构抽离、把逻辑操作作为参数传入

## 四、标准解（2026-08-03 给出）

**判定**：✅ 第一轮即过（连续第二天）——重复 switch 诊断 ✅、多态 ✅、LINQ/高阶函数 ✅。

「重复的 switch」的本质：同一个类型判断散落 N 处，每个新类型都要找全所有 switch。多态把「按类型分支」变成「按类分派」。

```csharp
public class Monster
{
    public int Hp;

    public virtual int CalcDamage(Player player) => 3;   // 默认分支（原 default）保留
    public virtual int GetGold() => 5;
}

public class Slime : Monster
{
    public override int CalcDamage(Player player) => 5;
    public override int GetGold() => 10;
}

public class Wolf : Monster
{
    public override int CalcDamage(Player player) => 8 + (player.Hp < 30 ? 5 : 0);
    public override int GetGold() => 20;
}

public class Dragon : Monster
{
    public override int CalcDamage(Player player) => 25 + 10;
    public override int GetGold() => 200 + Hp / 10;
}

public class CombatSystem
{
    public int CalcDamage(Monster m, Player player) => m.CalcDamage(player);
}

public class RewardSystem
{
    public int GetGold(Monster m) => m.GetGold();
}
```

加 Goblin = 建一个类，两个系统零改动——霰弹枪变单发。default 分支 → 基类 virtual 默认值。

**Day 6 口诀完整版**：英雄血量/图标是**数据** → 表；怪物伤害/掉落规则是**行为** → 多态。数据用表，逻辑用多态。

**循环 → 管道**（声明式：你要什么，不是怎么找）：

```csharp
public Item Find(string name) => Items.FirstOrDefault(i => i.Name == name);
public int TotalValue() => Items.Sum(i => i.Value);
```

手写 for 的问题：把「意图」淹没在「怎么走」里，每处循环重新发明一遍轮子。

## 五、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（第一轮通过 + 标准解）
2. 任务 1：两个 switch → 多态；任务 2：两个手写 for → LINQ——**铁律：只拆不换**——**未做**
3. 骨架：[Homework/重构/第二轮-结构型坏味道/MonsterSystem.cs](Homework/重构/第二轮-结构型坏味道/MonsterSystem.cs)

---

`[进度：阶段三-重构 → Day 9「重复的switch+循环」进行中]`
