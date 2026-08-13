# 函数（Functions）

> 来源：《代码整洁之道》Robert C. Martin 第 3 章
> 跨书联动：重构 Day 3 过长函数、Day 13 Replace Conditional with Polymorphism；SRP（函数级）

---

## 一、坏代码场景

战斗结算函数——这个函数做了几件事？

```csharp
public void ProcessCombat(Player p, Enemy e, bool isCrit, int dmg)
{
    var finalDmg = dmg;
    if (isCrit) finalDmg = (int)(dmg * 1.5f);
    e.HP -= finalDmg;
    if (e.HP <= 0)
    {
        p.Gold += e.GoldReward;
        p.Exp += e.ExpReward;
        if (p.Level >= 10) UnlockAchievement("slayer_10");
        else if (p.Level >= 5) UnlockAchievement("slayer_5");
        DropLoot(e, p.Luck);
        PlayKillSound();
        e.Destroy();
    }
    else
    {
        e.AttackBack(p);
        if (p.HP <= 0) GameOver();
    }
}
```

## 二、问题（2026-08-14 布置）

1. `ProcessCombat` 做了几件事？数一数（暴击计算、扣血、击杀结算、成就、掉落、音效、反击、游戏结束判断……）
2. 当需求变化时——「暴击倍率 1.5 → 2.0」和「新增击杀排行榜」——改动会牵动哪些**不相干**的代码？
3. 按「一件事」拆，你会拆成哪几个函数？

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 2「函数」苏格拉底问答中]`
