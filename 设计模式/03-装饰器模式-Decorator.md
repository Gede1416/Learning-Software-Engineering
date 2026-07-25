# 装饰器模式（Decorator Pattern）

> 来源：《设计模式》GoF 第 4 章 + 《Head First 设计模式》第 3 章

---

## 一、书中定义

GoF 的定义：

> **"动态地给一个对象添加一些额外的职责。就增加功能来说，装饰器模式比生成子类更为灵活。"**

Head First 的直觉版：**"以包装器（wrapper）的方式，在运行时给对象套上一层又一层的功能。"**

---

## 二、坏代码场景

假设你在做一个 ARPG 的**武器附魔系统**。一把剑可以被多种附魔强化：

- 🔥 **火焰附魔**：+5 伤害，攻击时附加燃烧效果
- ❄️ **冰冻附魔**：减速敌人 2 秒
- ☠️ **剧毒附魔**：造成 3 秒持续伤害

**附魔可以叠加**——一把剑可以同时有火 + 毒，或者冰 + 火 + 毒。

有人用继承来写：

```csharp
// 基类
public class Weapon
{
    public virtual int GetDamage() => 10;
    public virtual string GetDescription() => "铁剑";
}

// 继承爆炸 —— 每个组合一个子类
public class FireSword : Weapon {
    public override int GetDamage() => 10 + 5;
    public override string GetDescription() => "火焰铁剑";
}
public class IceSword : Weapon {
    public override int GetDamage() => 10;
    public override string GetDescription() => "冰冻铁剑";
}
public class FireIceSword : Weapon {
    public override int GetDamage() => 10 + 5;
    public override string GetDescription() => "火焰冰冻铁剑";
}
public class PoisonFireIceSword : Weapon {  // 😱
    // ...
}
```

| 附魔种类 | 可能的组合数 |
|----------|-------------|
| 1 种 | 2（有 / 无） |
| 2 种 | 4（无、火、冰、火冰） |
| 3 种 | 8 |
| N 种 | **2^N** |

加第 4 种附魔「雷电」→ 组合从 8 变成 16，需要 8 个新子类。**继承方案，类数量指数爆炸。**

---

## 问题

1. 这个场景和策略模式那节课的「元素 × 护甲 × 武器」组合爆炸有什么本质不同？（提示：策略模式是「选一种算法」，附魔系统是「叠加多种效果」。一个是择一，一个是累加。）

2. 能不能让「火焰附魔」本身也是一把武器？——它包装另一把武器，在它的伤害基础上 +5，在它的描述后面追加 " + 火焰"？

---

## 你的回答（2026-07-22）

1. **策略 = 择一，装饰器 = 累加** ✅ —— 两种完全不同的爆炸。
2. **属性管道（Attribute Pipeline）** ✅ —— 把属性抽离 + 修改器链。这是游戏 Buff 系统的标准做法。

> 你选择了进入装饰器模式的方向：附魔不只是改数值，还要在攻击时做额外行为（燃烧动画、DoT、减速）。行为扩展才是装饰器赢的场景。

---

## 三、标准重构：用装饰器消灭继承爆炸

### 3.1 核心思路

```
装饰器本身也是一个 Weapon，它包装另一个 Weapon。

调用者 → 火焰附魔 → 冰冻附魔 → 铁剑

铁的剑 .GetDamage()  →  返回 10
  冰冻包装它 .GetDamage()  →  先调铁剑.GetDamage()，再触发减速效果
    火焰包装它 .GetDamage()  →  先调冰冻.GetDamage()，再触发燃烧效果

外层先执行额外操作，然后委托给内层 —— 像剥洋葱。
```

### 3.2 代码

```csharp
// ① 组件接口 —— 所有武器和装饰器的共同契约
public abstract class Weapon
{
    public abstract int GetDamage();
    public abstract string GetDescription();
    public abstract void OnAttack(Enemy target);  // 行为扩展入口
}

// ② 具体组件 —— 被装饰的核心
public class IronSword : Weapon
{
    public override int GetDamage() => 10;
    public override string GetDescription() => "铁剑";
    public override void OnAttack(Enemy target)
    {
        // 基础攻击：只有伤害，没有特效
        target.TakeDamage(GetDamage());
    }
}

// ③ 装饰器抽象基类 —— 关键：它继承 Weapon，同时持有一个 Weapon
public abstract class EnchantDecorator : Weapon
{
    protected Weapon _wrapped;  // 被包装的武器

    public EnchantDecorator(Weapon wrapped)
    {
        _wrapped = wrapped;
    }

    // 默认：全部委托给被包装的武器
    public override int GetDamage() => _wrapped.GetDamage();
    public override string GetDescription() => _wrapped.GetDescription();
    public override void OnAttack(Enemy target) => _wrapped.OnAttack(target);
}

// ④ 具体装饰器 —— 火焰附魔
public class FireEnchant : EnchantDecorator
{
    public FireEnchant(Weapon wrapped) : base(wrapped) { }

    public override int GetDamage() => _wrapped.GetDamage() + 5;
    public override string GetDescription() => _wrapped.GetDescription() + " + 火焰";
    public override void OnAttack(Enemy target)
    {
        _wrapped.OnAttack(target);          // 先执行内层的攻击逻辑
        target.ApplyBurning(3, 2);          // 额外的：3 秒燃烧，每跳 2 点伤害
        Vfx.Play("fire_explosion", target); // 额外的：播放火焰特效
    }
}

// ⑤ 具体装饰器 —— 冰冻附魔
public class IceEnchant : EnchantDecorator
{
    public IceEnchant(Weapon wrapped) : base(wrapped) { }

    public override string GetDescription() => _wrapped.GetDescription() + " + 冰冻";
    public override void OnAttack(Enemy target)
    {
        _wrapped.OnAttack(target);
        target.ApplySlow(2.0f);  // 额外：减速 2 秒
    }
}
```

### 3.3 使用：运行时自由组合

```csharp
// 普通铁剑
Weapon sword = new IronSword();
sword.OnAttack(enemy);
// 伤害 10，无特效

// 火焰铁剑
Weapon fireSword = new FireEnchant(new IronSword());
fireSword.OnAttack(enemy);
// 伤害 15，燃烧特效

// 火焰 + 冰冻 铁剑
Weapon fireIceSword = new FireEnchant(new IceEnchant(new IronSword()));
fireIceSword.OnAttack(enemy);
// 伤害 15，先减速再燃烧

// 加第 4 种附魔？一个新类，0 个已有类被修改。
var ultraSword = new PoisonEnchant(new FireEnchant(new IceEnchant(new IronSword())));
```

---

## 四、装饰器模式 vs 属性管道 —— 你家项目的选择

你提到的属性管道：

```csharp
// 属性管道：遍历修改器链，只加工数值
weapon.Modifiers = [fireMod, iceMod];
int dmg = weapon.GetDamage();  // → 10 + 5 + 0 = 15  ← 只算数值
```

| 场景 | 适合 |
|------|------|
| 附魔只改数值（+5 攻击、+10% 暴击） | **属性管道** — 增删方便，撤销自然 |
| 附魔要加行为（燃烧动画、DoT、减速、连锁闪电跳转到下个目标） | **装饰器模式** — 在委托链上挂接额外操作 |
| 两者都需要 | **混用** —— 装饰器管 OnAttack 行为，属性管道管 GetDamage 数值 |

实际上成熟的游戏项目里，**Buff 系统用属性管道，武器升级系统可能用装饰器**。不是非此即彼，是各司其职。

---

## 五、核心结构（GoF）

```
        ┌──────────┐
        │  Weapon  │  ← 组件接口
        │(abstract)│
        └────┬─────┘
             △
    ┌────────┴─────────┐
    │                  │
┌───┴────────┐  ┌──────┴──────────┐
│ IronSword  │  │EnchantDecorator │ ← 装饰器基类
│ (具体组件)  │  │  ┌───────────┐ │
│            │  │  │ _wrapped  │ │ ← 持有被包装对象
└────────────┘  │  └───────────┘ │
                └──────┬─────────┘
                        △
            ┌───────────┼───────────┐
            │           │           │
    ┌───────┴──┐  ┌─────┴───┐  ┌───┴──────┐
    │FireEnchant│ │IceEnchant│ │PoisonEnch│
    └──────────┘  └─────────┘  └──────────┘
```

**关键特征**：
- 装饰器**既是 Weapon（继承），又持有 Weapon（组合）**
- 继承给了它「和原始对象一样的类型」，组合给了它「可以无限嵌套」的能力

---

## 六、和前两个模式的对比

| | 策略模式 | 观察者模式 | 装饰器模式 |
|------|----------|------------|------------|
| 关系 | 1 对 1 | 1 对多 | 1 包 1（链式） |
| 目的 | 封装可替换的算法 | 通知多个依赖者 | 动态叠加功能 |
| 方向 | 向外（我用什么算法） | 向外（我要通知谁） | 向内（在我外面包了什么） |
| 是否改变接口 | 不改变 | 不改变 | **不改变**（装饰后的对象和原始对象类型相同） |

三条都满足 OCP：都可以在不修改已有代码的前提下扩展行为。

---

## 七、跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP —— 加新附魔不改已有代码 | 《敏捷》第 9 章 |
| 组合优于继承（Favor Composition over Inheritance） | 《Head First》第 3 章 Design Principle 4 |
| Wrapper / Adapter / Proxy —— 三种"包装"模式的区别，后面结构型模式会串讲 | GoF 第 4 章 |
| 中间件管道（Middleware Pipeline）—— ASP.NET 的 `app.Use(...)` 本质就是装饰器链 | 企业开发惯例 |

---

## 八、作业（预计 10 分钟）

你的游戏里，角色有一个**基础攻击技能**。不同的 Buff 可以在攻击前后添加额外行为：

```
基础攻击：造成 100% 攻击力伤害
暴击 Buff：攻击前判断暴击率，暴击则伤害 ×2
吸血 Buff：攻击后恢复造成伤害的 20%
连击 Buff：攻击后有 30% 概率再攻击一次
```

用装饰器模式实现。框架：

```csharp
public abstract class AttackSkill
{
    public int BaseDamage;
    public abstract void Execute(Enemy target, Character owner);
}

public class NormalAttack : AttackSkill
{
    public override void Execute(Enemy target, Character owner)
    {
        target.TakeDamage(BaseDamage);  // 基础攻击，没别的
    }
}

// 你来写：
// public class AttackBuffDecorator : AttackSkill { ... }
// public class CritBuff : AttackBuffDecorator { ... }
// public class LifeStealBuff : AttackBuffDecorator { ... }
// public class DoubleHitBuff : AttackBuffDecorator { ... }
```

---

`[进度：设计模式-①策略模式 ✓ / ②观察者模式 ✓ / ③装饰器模式 → 核心讲解完成，等待作业 ✓]`
