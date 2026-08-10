# 有意义的命名（Meaningful Names）

> 来源：《代码整洁之道》Robert C. Martin 第 2 章
> 跨书联动：重构 Day 1「神秘命名」——从"坏味道"到"预防"的同一枚硬币

---

## 一、坏代码场景

技能系统——名字在撒谎：

```csharp
public class SkillData
{
    public int dmg;        // 伤害值？还是治疗量？
    public float r;        // 半径？还是射程？
    public bool f;         // 友伤？群伤？还是飞行？
    public int cd;         // 冷却 cd？还是倒计时？
}

public class SkillSystem
{
    public void Use(int sk, Player p, Enemy e)     // sk = 技能 id？玩家用还是敌人用？
    {
        var d = GetD(sk, p);                        // GetD 是什么？
        e.HP -= d;
        if (e.HP <= 0) Kill(e, p);                  // Kill 只扣血还是结算奖励？
    }
}
```

## 二、问题（2026-08-09 布置）

1. 这段代码里的名字，哪些在撒谎？——`dmg` / `r` / `f` / `cd` / `sk` / `d` / `GetD` / `Kill`，逐个说
2. 当需求变化时（比如新技能"圣光治疗队友"），这些名字具体在哪一步**误导你改错**？
3. 一个变量叫 `d`，全文件搜索 `d` 会搜出什么？（搜索友好性）

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 1「有意义的命名」苏格拉底问答中]`
