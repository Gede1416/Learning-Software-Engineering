# 开闭原则（Open-Closed Principle, OCP）

> 来源：《敏捷软件开发：原则、模式与实践》— Robert C. Martin，第 9 章

---

## 一、书中定义

> **"软件实体（类、模块、函数等）应该对扩展开放，对修改关闭。"**
> — Bertrand Meyer（Martin 在《敏捷软件开发》中重申并展开）

Martin 的解释：你应当能够在不修改已有代码的前提下，通过添加新代码来改变系统的行为。

---

## 二、坏代码场景

假设你在做一个 MOBA 游戏的英雄技能系统：

```csharp
public class SkillAreaCalculator
{
    public float GetArea(string heroType)
    {
        if (heroType == "Mage")            // 圆形: πr²
            return MathF.PI * 5 * 5;
        else if (heroType == "Archer")     // 扇形: ½θr²
            return 0.5f * (MathF.PI / 3) * 8 * 8;
        else if (heroType == "Assassin")   // 矩形: w×h
            return 3 * 8;
        return 0;
    }
}
```

| 违反项 | 具体表现 |
|--------|----------|
| **对修改不关闭** | 每加一个英雄，必须打开这个类加 `else if` |
| 线上风险 | 改已有代码 → 可能引入回归 bug，已上线的法师/射手/刺客全炸 |

---

## 三、标准重构：接口 + 多态

```csharp
// ① 定义抽象（对扩展开放）
public interface ISkillArea
{
    float GetArea();
}

// ② 每个英雄 = 一个新类（对修改关闭）
public class MageSkillArea : ISkillArea
{
    public float GetArea() => MathF.PI * 5 * 5;
}

public class ArcherSkillArea : ISkillArea
{
    public float GetArea() => 0.5f * (MathF.PI / 3) * 8 * 8;
}

public class AssassinSkillArea : ISkillArea
{
    public float GetArea() => 3 * 8;
}

// ③ 使用方不再依赖具体类型
public class SkillAreaCalculator
{
    public float GetArea(ISkillArea skill) => skill.GetArea();
}
```

添加新英雄只需新建一个类，**一行已有代码都不改**。

---

## 四、关于你的字典方案

你提出的"字典 dispatch"方案：

```csharp
var areaMap = new Dictionary<string, Func<float>>
{
    ["Mage"] = () => MathF.PI * 5 * 5,
    ["Archer"] = () => 0.5f * (MathF.PI / 3) * 8 * 8,
};
```

| 方案 | OCP 合规度 |
|------|-----------|
| if-else 链 | ❌ 完全违反 |
| 字典 dispatch | ⚠️ 半开半闭（字典仍需修改注册新条目） |
| 接口 + 多态 | ✅ 完全符合 |

**字典方案的定位**：适合英雄类型极少变化、但想消除长 if-else 的场景。不是完整 OCP 解，但比裸 if-else 好。

---

## 五、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 策略模式（Strategy）——ISkillArea 就是策略接口 | 《设计模式》GoF 第 5 章 |
| 多态替换条件逻辑 | 《重构》Martin Fowler 第 10 章 "Replace Conditional with Polymorphism" |

---

## 六、作业（预计 5 分钟）

下面是一个 RPG 的金钱掉落计算，用 if-else 按敌人等级返回金钱。请用接口+多态重构：

```csharp
public class GoldCalculator
{
    public int CalcGold(string enemyRank)
    {
        if (enemyRank == "Normal")       return 10;
        else if (enemyRank == "Elite")   return 50 + Random.Range(0, 20);
        else if (enemyRank == "Boss")    return 200 + Random.Range(50, 100);
        return 0;
    }
}
```

要求：定义接口 + 至少 3 个实现类，`GoldCalculator` 只依赖接口。

`[进度：SOLID-①SRP ✓ / ②OCP → 讲解完成，等待作业 ✓]`
