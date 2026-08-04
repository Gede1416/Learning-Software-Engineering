# 依恋情结（Feature Envy）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 8 位
> 跨书联动：《敏捷》数据封装 —— 数据和行为应该住在同一个类

---

## 一、坏代码场景

伤害计算——一个方法乱摸别人的字段：

```csharp
public class DamageSystem
{
    // 计算物理伤害
    public int CalcPhysicalDamage(Player player, Enemy enemy)
    {
        int atk = player.Atk;
        int weaponBonus = player.Weapon.Bonus;
        int def = enemy.Def;
        int armor = enemy.Armor.Reduction;
        int dmg = atk + weaponBonus - def - armor;
        if (player.Level > enemy.Level)
            dmg += (player.Level - enemy.Level) * 2;   // 等级压制
        return Math.Max(1, dmg);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：`CalcPhysicalDamage` 用的数据是「谁的」？这段逻辑更应该是谁的方法？
   敌人加新属性（格挡）要改哪里？玩家改攻击公式要改哪里？）

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 用**搬移函数（Move Function）**把伤害计算搬回它该待的地方——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/重构/DamageSystem.cs](Homework/重构/DamageSystem.cs)

---

`[进度：阶段三-重构 → Day 7「依恋情结」进行中]`
