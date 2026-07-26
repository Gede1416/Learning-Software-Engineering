# 接口隔离原则（Interface Segregation Principle, ISP）

> 来源：《敏捷软件开发：原则、模式与实践》— Robert C. Martin，第 12 章

---

## 一、书中定义

> **"不应该强迫客户依赖于它们不使用的方法。"**
> — Robert C. Martin

Martin 的解释：胖接口（fat interfaces）应该被拆分成更小、更内聚的接口。每个接口只服务于一个客户（调用方），而不是把所有方法堆在一个接口里让所有人实现。

**SRP vs ISP 的区别**：
- SRP 管的是**类**："一个类只为一个人服务"
- ISP 管的是**接口**："一个接口不应该包含调用方不需要的方法"

---

## 二、坏代码场景

RPG 游戏所有角色被迫实现一个巨无霸接口：

```csharp
public interface ICharacterAction
{
    void Move();
    void Attack();
    void CastSpell();
    void PickPocket();   // 偷窃
    void Prayer();        // 祈祷
    void ShieldBlock();   // 盾牌格挡
}

// 战士
public class Warrior : ICharacterAction
{
    public void Move() { /* 实现 */ }
    public void Attack() { /* 实现 */ }
    public void CastSpell() { throw new NotImplementedException(); }
    public void PickPocket() { throw new NotImplementedException(); }
    public void Prayer() { throw new NotImplementedException(); }
    public void ShieldBlock() { /* 实现 */ }
}

// 盗贼
public class Thief : ICharacterAction
{
    public void Move() { /* 实现 */ }
    public void Attack() { /* 实现 */ }
    public void CastSpell() { throw new NotImplementedException(); }
    public void PickPocket() { /* 实现 */ }
    public void Prayer() { throw new NotImplementedException(); }
    public void ShieldBlock() { throw new NotImplementedException(); }
}
```

---

## 三、诊断：两个工程问题

### 1. 编译层面 —— 接口污染导致的级联修改

在 `ICharacterAction` 上加一个新方法（比如 `Prayer()`），**所有实现了这个接口的类都必须加这个方法**——哪怕战士、盗贼根本不需要祈祷。改一个接口，炸掉 N 个类。

### 2. 使用层面 —— 客户端被暴露了不该看到的依赖

假设战斗系统只需要角色移动：

```csharp
public void MoveToTarget(ICharacterAction unit, Vector3 target)
{
    unit.Move();  // 我只需要 Move
    // 但 IDE 自动补全会列出 CastSpell、PickPocket、Prayer...
}
```

`MoveToTarget` 的调用方只需要 `Move()`，但它依赖了整个 `ICharacterAction`。结果是：
- 调用方**传递依赖了不需要的方法**——如果接口变了（即使只是 `Prayer` 改名），`MoveToTarget` 的模块也要重新编译
- 无法通过看方法签名判断**它到底会调用哪些方法**——可读性差、重构风险高

---

## 四、重构方案：按角色能力拆分接口

```csharp
// 每个角色只实现自己需要的能力
public interface IMovable    { void Move(); }
public interface IAttacker   { void Attack(); }
public interface ISpellCaster { void CastSpell(); }
public interface IThiefSkill  { void PickPocket(); }
public interface IPrayer      { void Prayer(); }
public interface IBlocker     { void ShieldBlock(); }

// 战士：只拿自己需要的
public class Warrior : IMovable, IAttacker, IBlocker
{
    public void Move() { }
    public void Attack() { }
    public void ShieldBlock() { }
}

// 盗贼：只拿自己需要的
public class Thief : IMovable, IAttacker, IThiefSkill
{
    public void Move() { }
    public void Attack() { }
    public void PickPocket() { }
}

// 调用方只声明最小依赖
public void MoveToTarget(IMovable unit, Vector3 target)
{
    unit.Move(); // 现在方法签名精确描述了需求
}
```

> 你说"接口的定义过于宽泛要进行收敛分成不同的接口"——就是这个方向。

---

## 五、关键心得

> **ISP 的本质是"只声明你真正需要的东西"。类不要被迫实现不需要的方法，调用方也不要被迫依赖不需要的接口。**

Martin 在书中的判断标准：**如果一个接口有多个客户端（使用者），且不同的客户端只用到不同的方法子集，那就该拆。**

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 接口隔离 → 接口拆分的具体手法 | 《重构》第 8 章 |
| ISP 违反常常导致 SRP 违反（胖接口 → 胖类） | 《敏捷软件开发》第 12 章 |

---

## 七、作业（预计 5 分钟）

下面是一个游戏实体的接口，NPC 被迫实现了敌人专属的方法。请拆分：

```csharp
public interface IEntity
{
    void TakeDamage(int dmg);    // 受击
    void DropLoot();             // 掉落——NPC 不需要
    void Patrol();               // 巡逻——敌人专属
    void Talk();                 // 对话——NPC 专属
}

public class Enemy : IEntity
{
    public void TakeDamage(int dmg) { }
    public void DropLoot() { }
    public void Patrol() { }
    public void Talk() { throw new NotImplementedException(); }
}

public class VillagerNpc : IEntity
{
    public void TakeDamage(int dmg) { }
    public void DropLoot() { throw new NotImplementedException(); }
    public void Patrol() { throw new NotImplementedException(); }
    public void Talk() { }
}
```

**要求**：拆成 3~4 个接口，让 `Enemy` 和 `VillagerNpc` 各自只实现自己需要的方法。不出现 `NotImplementedException`。

---

`[进度：SOLID-①SRP ✓ / ②OCP ✓ / ③LSP ✓ / ④ISP → 讲解完成，等待作业 ✓]`
