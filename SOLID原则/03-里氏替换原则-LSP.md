# 里氏替换原则（Liskov Substitution Principle, LSP）

> 来源：《敏捷软件开发：原则、模式与实践》— Robert C. Martin，第 10 章

---

## 一、书中定义

> **"子类型必须能够替换掉它们的基类型。"**
> — Barbara Liskov（Martin 在《敏捷软件开发》中引用并展开）

Martin 的解释：使用基类引用的地方，换成子类对象后，程序行为不应该发生变化。**子类可以扩展父类的行为，但不能改变父类已有的行为契约。**

---

## 二、坏代码场景

回合制游戏的 Buff 系统——`BurnBuff` 继承 `Buff`，但重写时破坏了契约：

```csharp
public class Buff
{
    protected int stacks;

    public virtual void AddStack()
    {
        stacks++;
    }

    public virtual int GetEffectValue(int baseValue)
    {
        return baseValue * stacks;
    }
}

// 燃烧 Buff：叠到 5 层时爆炸
public class BurnBuff : Buff
{
    public override void AddStack()
    {
        stacks++;
        if (stacks >= 5)
            throw new BurnExplosionException(); // 💥
    }

    public override int GetEffectValue(int baseValue)
    {
        if (stacks == 0)
            throw new InvalidOperationException("不能对 0 层燃烧计算伤害");
        return baseValue * stacks * 2;
    }
}

// 对所有 Buff 统一操作的代码
public void TickAllBuffs(List<Buff> buffs)
{
    foreach (var buff in buffs)
    {
        buff.AddStack();
        int dmg = buff.GetEffectValue(10);
        ApplyDamage(dmg);
    }
}
```

---

## 三、诊断

`BurnBuff` 从两个方向打破父类契约：

| 方法 | 父类契约 | BurnBuff 的实际行为 | LSP 违规类型 |
|------|----------|---------------------|-------------|
| `AddStack()` | 加一层，无副作用 | 叠到 5 层抛异常 | **弱化后置条件** — 父类承诺"只加层"，子类多了"可能崩溃" |
| `GetEffectValue()` | 任意 stacks 下都能算 | `stacks == 0` 抛异常 | **强化前置条件** — 父类接受 0 层，子类拒绝 |

Martin 在书中总结的两类 LSP 违规：
1. **强化前置条件** → 子类比父类更苛刻
2. **弱化后置条件** → 父类保证的结果，子类没保证

**根本原因**：`TickAllBuffs` 拿着 `List<Buff>`，它只知道父类的契约——"加层 + 算伤害，稳的"。换成 `BurnBuff` 后两个方法都可能炸，而调用方根本不知道要 try-catch。

---

## 四、重构方案

**核心思路**：让继承只表达"同一个契约的不同实现"，把"契约外的额外行为"抽到继承体系之外。

```csharp
// ① 父类保持纯粹——只定义所有 Buff 共有的东西
public abstract class Buff
{
    protected int stacks;

    public virtual void AddStack() => stacks++;
    public virtual int GetEffectValue(int baseValue) => baseValue * stacks;
}

// ② 燃烧 Buff：不再抛异常，重写方法的行为保持在契约范围内
public class BurnBuff : Buff
{
    public override int GetEffectValue(int baseValue)
    {
        // 即使 stacks == 0，也能正常返回 0（父类契约允许）
        return baseValue * stacks * 2;
    }

    public bool ShouldExplode() => stacks >= 5; // 爆炸逻辑暴露为"查询方法"
}

// ③ 爆炸判定被提升到调用方显式处理
public void TickAllBuffs(List<Buff> buffs)
{
    foreach (var buff in buffs)
    {
        buff.AddStack();

        // 调用方显式询问是否需要特殊处理
        if (buff is BurnBuff burn && burn.ShouldExplode())
        {
            ApplyExplosionDamage(burn.GetEffectValue(20));
        }
        else
        {
            int dmg = buff.GetEffectValue(10);
            ApplyDamage(dmg);
        }
    }
}
```

**按你的分析**：爆炸伤害计算现在回到了 `TickAllBuffs` 的流程里，`AddStack` 不再偷偷搞事。`GetEffectValue` 也不再拒绝输入。

---

## 五、关键心得

> **LSP 不是在说"子类不能做新事情"，而是"子类不能拒绝父类已经承诺的事情"。**

`Stacks == 0` 时父类承诺能返回伤害，子类就不能说不。把新行为暴露为"可以问的方法"（`ShouldExplode()`）而不是藏在副作用里，就是 LSP 安全。

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 契约式设计（Design by Contract）——前置/后置条件 | 《敏捷软件开发》第 10 章 |
| 用组合替代继承（如果写不出满足 LSP 的子类，考虑别继承） | 《重构》第 7 章 + GoF 第 1 章 "Favor composition over inheritance" |

---

## 七、作业（预计 5 分钟）

下面是一个装备耐久系统，子类强化了前置条件。请重写，使 `IndestructibleItem` 满足 LSP：

```csharp
public class Item
{
    protected int durability;

    public virtual void Repair(int amount)
    {
        durability = Math.Min(durability + amount, MaxDurability);
    }
}

public class IndestructibleItem : Item
{
    public override void Repair(int amount)
    {
        throw new InvalidOperationException("传说装备不能被修理！");
    }
}
```

要求：消除异常，通过方法签名或返回值让调用方能安全使用。

---

## 八、作业审阅

### 你的答案

```csharp
enum ItemType { _1, _2 }  // 含义不明
public class Item
{
    protected ItemType _type;

    public virtual bool Repair(int amount)
    {
        if (_type != _2)  // 父类知道了子类是谁 → 违反 OCP
        {
            durability = Math.Min(durability + amount, MaxDurability);
            return true;
        }
        return false;
    }
}
```

| 问题 | 说明 |
|------|------|
| ❌ `ItemType._1 / _2` | 命名无意义，看不出来代表什么 |
| ❌ 父类判断子类类型 | `if (_type != _2)` — 父类知道了"某种特殊子类"的存在，反过来违反 OCP |
| ❌ `IndestructibleItem.Repair()` 只是转发 | 没有任何自己的逻辑，继承没有体现价值 |

**根本问题**：你引入类型枚举来"绕过"继承，但 LSP 的精髓是**用多态本身来表达行为差异**，父类不需要知道子类是谁。

### 标准解法

```csharp
public class Item
{
    protected int durability;

    public virtual bool Repair(int amount)
    {
        durability = Math.Min(durability + amount, MaxDurability);
        return true; // 修理成功
    }
}

public class IndestructibleItem : Item
{
    // 子类不抛异常，而是返回 false——父类契约允许
    public override bool Repair(int amount)
    {
        return false; // 不可修理 ← 调用方能安全处理
    }
}

// 调用方
foreach (var item in inventory)
{
    if (!item.Repair(50))
        ShowMessage("该装备不可修理");
}
```

> 返回值传递"我是谁"信息，不放异常。`bool` 是最简方式；更复杂的场景可用 `RepairResult` 类。

---

`[进度：SOLID-①SRP ✓ / ②OCP ✓ / ③LSP ✓]`
