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

## 三、你的回答（2026-08-03）

1. 更应该放在 Player 或 Enemy
2. 没有使用到系统参数，只用 Player/Enemy 的数据
3. 在方法内部增加局部变量，从敌人那获取，再改变公式
4. 改攻击公式也要在 CalcPhysicalDamage 方法内部改

## 四、纠错（第一轮，2026-08-03）

- 第 1 点 ✅：方向对——逻辑应该住在数据家里。
- 第 2 点 ✅：好观察——`DamageSystem` 没有任何自己的状态，方法用的**别人的数据比自己的多**，这就是「依恋」的定义。
- 第 3、4 点 ⚠️ 半对：感觉到了「改公式要进 CalcPhysicalDamage」，但没说清后果——攻击公式、防御公式、敌人加「格挡」**全都要进 DamageSystem 改**：数据住在 Player/Enemy 家，行为却住在 DamageSystem 家——这是 Day 6 的**发散式变化**现场（跨书联动）。
- 缺：没回答「放哪边」。方法同时摸 Player（Atk/Weapon.Bonus/Level）与 Enemy（Def/Armor.Reduction/Level）两家的字段，但子公式各自只摸一家的数据。

**待答子问题**：`Atk + Weapon.Bonus` 只摸 Player 家，`Def + Armor.Reduction` 只摸 Enemy 家。策划把攻击公式改成「Atk × 1.2 + Weapon.Bonus」——现在要改哪里？把「攻击值」搬给 Player 自己算，又要改哪里？哪个更局部？

## 五、标准解（待给出）

（子问题回答正确后给出）

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题已答，待按子问题修正
2. 用**搬移函数（Move Function）**把伤害计算搬回它该待的地方——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/重构/DamageSystem.cs](Homework/重构/DamageSystem.cs)

---

`[进度：阶段三-重构 → Day 7「依恋情结」进行中]`
