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

### 第一轮

1. 更应该放在 Player 或 Enemy
2. 没有使用到系统参数，只用 Player/Enemy 的数据
3. 在方法内部增加局部变量，从敌人那获取，再改变公式
4. 改攻击公式也要在 CalcPhysicalDamage 方法内部改

### 第二轮（子问题）

1. 把攻击力计算公式和受击计算公式拆分，策划可单独调整
2. 玩家负责攻击力计算，敌人负责伤害计算——各自依赖自己的数据
3. 中间传输 HitData 记录传递值，后续扩展在这个类里扩展

## 四、纠错（第一轮，2026-08-03）

- 第 1 点 ✅：方向对——逻辑应该住在数据家里。
- 第 2 点 ✅：好观察——`DamageSystem` 没有任何自己的状态，方法用的**别人的数据比自己的多**，这就是「依恋」的定义。
- 第 3、4 点 ⚠️ 半对：感觉到了「改公式要进 CalcPhysicalDamage」，但没说清后果——攻击公式、防御公式、敌人加「格挡」**全都要进 DamageSystem 改**：数据住在 Player/Enemy 家，行为却住在 DamageSystem 家——这是 Day 6 的**发散式变化**现场（跨书联动）。
- 缺：没回答「放哪边」。方法同时摸 Player（Atk/Weapon.Bonus/Level）与 Enemy（Def/Armor.Reduction/Level）两家的字段，但子公式各自只摸一家的数据。

**子问题判定（第二轮，2026-08-03）**：✅ 通过——玩家负责攻击力计算、敌人负责伤害计算（各自依赖自家数据）、HitData 传递中间值（加分项，Day 4 联动）。概念题收官。

## 五、标准解（2026-08-03 给出）

Fowler《重构》第 8 章 Move Function 原则：**方法恋上谁，就搬到谁家**——方法应该和它用的数据住在一起。

```csharp
public class Player
{
    public int Atk;
    public int Level;
    public Weapon Weapon;

    public int GetAttack() => Atk + Weapon.Bonus;   // 攻击公式住进 Player 家
}

public class Enemy
{
    public int Def;
    public int Level;
    public Armor Armor;

    public int GetDefense() => Def + Armor.Reduction;  // 防御公式住进 Enemy 家

    public int CalcDamageTaken(Player player)          // 受击结算住进数据最全的一方
    {
        int dmg = player.GetAttack() - GetDefense();
        if (player.Level > Level)
            dmg += (player.Level - Level) * 2;
        return Math.Max(1, dmg);
    }
}

public class DamageSystem
{
    // 从「所有战斗数值变化的集散地」缩成一行调度——或直接删掉
    public int CalcPhysicalDamage(Player player, Enemy enemy)
        => enemy.CalcDamageTaken(player);
}
```

- 攻击公式改 → 只改 `Player.GetAttack`；防御公式改 → 只改 `Enemy.GetDefense`；加格挡 → 只改 Enemy——**全部局部**，发散式变化归零。
- HitData 点评：多阶段战斗流程（结算前后钩子、伤害事件总线）里是好设计；简单流程直接返回值——别为一个 int 造对象（Day 4 铁律：参数对象要「同生共死」才值得建）。

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（第二轮通过 + 标准解）
2. 搬移函数——**第一轮验收 ⚠️**：结构全对（GetAtk/GetDef/GetDmg 搬移 ✅、DamageSystem 删除 ✅）；**一处行为漂移**：`Math.Max(1, dmg)` → `Math.Max(dmg, 0)`——保底伤害 1 变 0（完全格挡时原版掉 1 滴、新版掉 0 滴），Day 3 `need` 坑重演（重构手痒）。改回后收官。
3. 骨架：[Homework/重构/DamageSystem.cs](Homework/重构/DamageSystem.cs)

---

`[进度：阶段三-重构 → Day 7「依恋情结」进行中]`
