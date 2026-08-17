# 对象与数据结构（Objects and Data Structures）

> 来源：《代码整洁之道》Robert C. Martin 第 6 章
> 跨书联动：重构 Day 12 纯数据类；阶段一 SRP；迪米特法则（Day 10 消息链）

---

## 一、坏代码场景

```csharp
// 玩家数据类——全是公开字段 + 无脑 getter/setter
public class PlayerData
{
    public int Hp;
    public int MaxHp;
    public int Mana;
    public int MaxMana;

    public int GetHp() { return Hp; }
    public void SetHp(int value) { Hp = value; }
    // ……每个字段都有无脑 getter/setter
}

// 使用方：规则散落
if (player.GetHp() <= 0) { /* 判死 */ }
// 中毒系统也有一份 if (player.GetHp() <= 0)
// 任务系统还有一份……
```

## 二、问题（2026-08-14 布置）

1. `GetHp()`/`SetHp()` 这种**无脑 getter/setter** 算封装吗？和直接公开字段有什么区别？（Hint：封装 = 隐藏实现细节——getter/setter 只是装饰过的裸字段，行为规则一点没藏）
2. 「Hp 小于 0 判死」这条规则散落几处？规则该住哪？（联动 Day 7 依恋情结 / Day 12 纯数据类）
3. 什么才是真正的封装？（Hint：接口暴露**行为**不暴露**数据**——`TakeDamage(int)`/`Heal(int)` 而不是 `SetHp`）

## 三、你的回答（2026-08-14，同步自 00-我的回答.md）

1. 根据外部需求开放 GetSet 不去无脑开放；属性私有化 且名称前加 _ 特殊标识方便理解
2. 属于 Player 的逻辑应该封装到类的内部符合 OOP 原则
3. 向外暴露的是具体业务需求的具体行为，而不是过于抽象的简单数据获取

### 验收（第 1 轮即过 ✅ 2026-08-14）
- Q1 按需开放 + 私有化 ✓（补充：无脑 getter/setter 只藏数据不藏规则）
- Q2 规则住进 Player 内部 ✓（补充：场景中判死散落 3 处——主战斗/中毒/任务）
- Q3 暴露行为而非数据 ✓——原文级理解

## 四、标准解（2026-08-14）

### 封装 = 隐藏实现细节（《代码整洁之道》第 6 章）
- 过程式（数据 + 外部函数）vs OOP（数据 + 行为在一起）
- getter/setter 只藏了字段名，**没藏规则**——判死/上限/死亡处理才是真正的实现细节
- 判断标准：这个类的规则归谁——归 Player 的就进 Player

### 标准答案代码

```csharp
public class Player
{
    private int _hp;
    private int _maxHp;
    private int _mana;

    public bool IsDead => _hp <= 0;          // 判死规则封装内部

    public void TakeDamage(int amount)
    {
        _hp = Math.Max(0, _hp - amount);
        if (IsDead) OnDeath();               // 死亡处理也在这
    }

    public void Heal(int amount) => _hp = Math.Min(_maxHp, _hp + amount);
    public bool IsManaEnough(int cost) => _mana >= cost;
    public void UseMana(int cost) => _mana -= cost;

    private void OnDeath() { /* 掉落/事件 */ }
}
// 外部：player.TakeDamage(dmg); if (player.IsDead) ...
// 判死规则一处，散落消失
```

### 联动
- 重构 Day 12 纯数据类：DTO（纯数据传输）无行为合法；**游戏实体必须有行为**
- 重构 Day 10 消息链：迪米特法则——不链式访问他人内部

### 作业验收（2026-08-14，纠错 1 轮后 ✅）
- 字段全私有化（`_hp/_maxHp/_mana/_maxMana`）✓
- `IsDead()` 无副作用——`_hp = 0` 移回 `TakeDamage`（Command/Query 分离）✓
- `damage` 拼写修正（Day 1 复发点）✓
- 防御逻辑加分：满血不能奶/死亡不能奶/空蓝不能用 ✓
- 小提示：`bool res` + `if` 可简化 `return _hp <= 0;`；残留 TODO 注释待清

---

`[进度：阶段四-代码整洁 → Day 5「对象与数据结构」苏格拉底问答中]`
