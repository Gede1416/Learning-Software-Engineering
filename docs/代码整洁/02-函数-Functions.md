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

（2026-08-14 用户跳过概念问答，标准解直接给出）

## 四、标准解（2026-08-14，用户跳过问答直接给出）

### Uncle Bob 函数三纪律（《代码整洁之道》第 3 章）

1. **短小**：函数应该很短——一个函数体 20 行以内；`ProcessCombat` 一屏装不下就是超标
2. **只做一件事（Do One Thing）**：函数只做"位于同一抽象层级"的一件事；`ProcessCombat` 混了三层（数值计算层：暴击倍率 / 实体操作层：扣血掉落 / 系统通知层：音效成就）
3. **参数少**：0/1/2 个最好；`isCrit` 这类开关参数意味着函数在做两件事

### 拆法（标准答案）

```csharp
public void ProcessCombat(Player p, Enemy e, bool isCrit, int dmg)
{
    var finalDmg = CalcFinalDamage(dmg, isCrit);   // 数值层
    if (!ApplyDamage(e, finalDmg)) return;         // 未击杀直接结束

    OnKillSettle(p, e);                            // 击杀结算：金币/经验/成就
    DropLoot(e, p.Luck);
    PlayKillSound();
    e.Destroy();
}

int CalcFinalDamage(int dmg, bool isCrit) => isCrit ? (int)(dmg * 1.5f) : dmg;
bool ApplyDamage(Enemy e, int dmg) { e.HP -= dmg; return e.HP <= 0; }
```

- 反击/游戏结束属于**敌方回合**，不该塞进玩家攻击函数——那是另一件事
- 成就 if-else 用 Day 13 的多态拆（AchievementUnlocker）
- 函数名能完整说出函数做的事——**说不清 = 做太多**

### 联动
- 重构 Day 3 过长函数（提炼函数）是手法，今天是纪律——「短小 + 一件事 + 一抽象层级」

---

`[进度：阶段四-代码整洁 → Day 2「函数」苏格拉底问答中]`
