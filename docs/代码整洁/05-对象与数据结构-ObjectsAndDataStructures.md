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

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 5「对象与数据结构」苏格拉底问答中]`
