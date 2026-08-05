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

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 任务 1：用**以多态取代条件（Replace Conditional with Polymorphism）**把两个 switch 换成多态；任务 2：用**以管道取代循环（Replace Loop with Pipeline）**把手写 for 换成 LINQ——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/重构/第二轮-结构型坏味道/MonsterSystem.cs](Homework/重构/第二轮-结构型坏味道/MonsterSystem.cs)

---

`[进度：阶段三-重构 → Day 9「重复的switch+循环」进行中]`
