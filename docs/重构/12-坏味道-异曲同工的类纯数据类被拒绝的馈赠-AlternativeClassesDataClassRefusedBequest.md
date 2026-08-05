# 异曲同工的类 + 纯数据类 + 被拒绝的馈赠（Alternative Classes / Data Class / Refused Bequest）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 18、19、20 位（合讲）
> 跨书联动：《敏捷》SRP、LSP；Day 7 依恋情结；Day 5 可变数据

---

## 一、坏代码场景

### 场景 1：异曲同工的类——同一个动作，两套接口

```csharp
public class Player { public void AddGold(int n) { /* 加钱 */ } }
public class Wallet  { public void DepositGold(int n) { /* 加钱 */ } }
```

### 场景 2：纯数据类——只有数据，没有行为

```csharp
public class PlayerStats
{
    public int Hp; public int MaxHp; public int Atk;
    // 一个方法都没有，全是 public 字段
}
```

### 场景 3：被拒绝的馈赠——继承了一个不想要的爸爸

```csharp
public class FlyingEnemy : GroundEnemy
{
    public override void Move() { /* 空中飞行，重写掉地面移动 */ }
    public override void Chase() { }   // 空实现——飞行怪根本不追
}
```

## 二、问题（2026-08-06 布置，先聚焦场景 1）

`Player.AddGold` 和 `Wallet.DepositGold` 做的是同一件事吗？假设打怪掉钱逻辑改了（掉 100 变掉 80），会漏改吗？怎么让「加钱」变成**一处实现、一套接口**？（提示：往学过的什么手法/模式上靠？）

（场景 2、3 待场景 1 通过后逐个来）

## 三、你的回答（2026-08-06，同步自 00-我的回答.md）

1. 迭代器模式？ Player Wallet 都实现 IAddGold { void AddGold(int n); }
   并且 引用对应的具体实现来实现接口
   再 通过玩家或者 GoldGetManager 来统一管理 获得金币事件
2. 或者 说两者的逻辑是一样的
   玩家拥有钱包的引用
   Player { AddGold() { Wallet.DepositGold() } }

## 四、标准解（2026-08-06）

### 场景 1：异曲同工的类（Alternative Classes with Different Interfaces）

**解法核心**：首要工具是**函数改名（Rename Function）**——让做同一件事的方法同名同签名；若逻辑重复则合并（Move Function），若只是转调则内联（Inline Function，联动 Day 10 中间人）。

**判据**：有货是重复 → 改名统一 + 合并；没货是中间人 → 内联删除。

**错误示范**（用户第 1 次代码：`IAddGold` + `NormalAddGold` + Player/Wallet 双转调）：
- 钱没有主人——`NormalAddGold` 身上没有 gold 字段，"加钱的动作"没有"钱"
- 接口没统一——对外仍是 `AddGold` / `DepositGold` 两个名字
- Player/Wallet 双双变中间人（Day 10 复发）
- **为消一个坏味道引入三个新坏味道 = 失败的重构**

**正确写法**：
```csharp
// 情形 A：Player 是钱的主人（最常见）
public class Player
{
    public int Gold;
    public void AddGold(int n) => Gold += n;
}

// 情形 B：Wallet 是数据主人
public class Wallet
{
    public int Gold;
    public void AddGold(int n) => Gold += n;   // DepositGold 改名统一
}
// 调用方直接 wallet.AddGold(n)——不需要中间人
```

**辨析**：策略模式解决「同一动作、多种算法可替换」；这里**只有一种算法**，用策略 = 给惰性元素续命（Day 11 镜像）。为"统一"造抽象层是常见翻车点。

**验收（2026-08-06，重写 2 轮收官）**：用户重写后核心落地（`_gold` 归位 Player、抽象层删净），残留 `Wallet` 中间人（Day 10 复发）→ 删除 ✅。最终形态：
```csharp
public class Player
{
    private int _gold;
    public void AddGold(int n) => _gold += n;
}
```

### 场景 2：纯数据类（Data Class）✅ 用户代码已通过（2026-08-06）

把散落在外的行为搬进类里（Fowler：Move Function / Encapsulate Record）——用户搬入 `IsDeath`、`Damage` ✅
瑕疵：`GetDamage` 是属性却动词命名 → Day 1 神秘命名，属性应叫 `Damage`

### 场景 3：被拒绝的馈赠（Refused Bequest）✅ 用户代码已通过（2026-08-06）

用「组合/委托替换继承」（Replace Inheritance with Delegation）：`FlyingEnemy` 只实现 `IMove`，不再被迫接受 `Chase` ✅（ISP 落地）
小提醒：`GroundEnemy` 内部 `NormalMove`/`NormalChase` 两层策略类略重——没有"换移动方式"需求就不必为将来付费

---

`[进度：阶段三-重构 → Day 12「异曲同工的类」苏格拉底问答中]`
