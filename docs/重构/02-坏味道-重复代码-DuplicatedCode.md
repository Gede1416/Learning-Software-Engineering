# 重复代码（Duplicated Code）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单（第 1 版第 1 位 / 第 2 版第 2 位）

---

## 一、坏代码场景

两种敌人，死亡奖励公式各写了一份：

```csharp
// 普通怪
public class NormalEnemy
{
    public void OnDie(Player player)
    {
        int gold = 10 + player.Level * 2;
        int xp   = 20 + player.Level * 3;
        player.GainGold(gold);
        player.GainXp(xp);
    }
}

// 精英怪
public class EliteEnemy
{
    public void OnDie(Player player)
    {
        int gold = 10 + player.Level * 2;   // ← 和上面一模一样的公式
        int xp   = 20 + player.Level * 3;
        player.GainGold(gold);
        player.GainXp(xp);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：策划明天改等级公式、加周末双倍、加首杀翻倍——每一处变化，你要记得改几个地方？）

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 把重复的奖励公式抽到一处，让 `NormalEnemy` 和 `EliteEnemy` 共用——**铁律：只抽取代码，不改数值、不改行为**
3. 骨架：[Homework/EnemyReward.cs](Homework/EnemyReward.cs)

---

`[进度：阶段三-重构 → Day 2「重复代码」进行中]`
