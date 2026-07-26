# 策略模式（Strategy Pattern）

> 来源：《设计模式》GoF 第 5 章 + 《Head First 设计模式》第 1 章

---

## 一、书中定义

GoF 的定义：

> **"定义一系列的算法，把它们一个个封装起来，并且使它们可以相互替换。策略模式使得算法可以独立于使用它的客户而变化。"**

你在 OCP 那节课写的 `IGoldFormula` + `NormalGold` / `EliteGold` / `BossGold`，其实就是策略模式。

---

## 二、坏代码场景

假设你在做一个卡牌游戏的伤害计算系统。卡牌有不同属性（火、冰、雷），不同属性对护甲类型（重甲、法袍、皮甲）的计算规则完全不同：

```csharp
public class DamageCalculator
{
    public int CalcDamage(string element, string armorType, int baseAtk)
    {
        if (element == "Fire")
        {
            if (armorType == "Heavy")       return baseAtk * 2;   // 火克重甲
            else if (armorType == "Robe")   return baseAtk;       // 正常
            else if (armorType == "Leather") return (int)(baseAtk * 1.5);
        }
        else if (element == "Ice")
        {
            if (armorType == "Heavy")       return baseAtk;
            else if (armorType == "Robe")   return baseAtk * 3;   // 冰克法袍
            else if (armorType == "Leather") return baseAtk;
        }
        else if (element == "Thunder")
        {
            if (armorType == "Heavy")       return baseAtk;
            else if (armorType == "Robe")   return (int)(baseAtk * 1.5);
            else if (armorType == "Leather") return baseAtk * 2;  // 雷克皮甲
        }
        return baseAtk;
    }
}
```

---

## 问题

**这段代码和 OCP 那节课的 `SkillAreaCalculator` 长得几乎一样——双重 if-else。但这次多了一个维度（元素 × 护甲 = 3×3 种组合）。只靠 OCP 的"接口+多态"能解决吗？如果再加一个"武器类型"维度（剑/弓/杖），if-else 会炸成什么样？**

*（先思考：维度爆炸时，单靠继承能救吗？还是需要比策略模式更进一步的武器？）*

---

## 你的回答（2026-07-22）

### ① 关于 Boss 自己 new 策略 → DIP 问题

> "不应该写在实体类里，如果这个实体类是通用的，我应该把赋值的这一步提取到 init 的参数里，通过工厂创建。"

✅ 正确。这就是 DIP 的落地——Boss 不应该知道 `BossGold` 的存在，策略应该由外部注入：

```csharp
// ❌ Boss 依赖具体策略类
public class Boss {
    ICalcGold calcGold = new BossGold();  // Boss 知道了 BossGold
}

// ✅ 策略从外部注入（工厂/DI）
public class Boss {
    ICalcGold calcGold;
    public Boss(ICalcGold goldStrategy) {  // 谁创建 Boss，谁决定用什么策略
        calcGold = goldStrategy;
    }
}
```

→ 后面工厂模式会专门展开「谁来 new」的问题。

### ② 二维场景的核心洞察

> "不变 + 变 → 策略模式完美适配，线性增长；变 + 变 → 组合爆炸，开始吃力。"

这个直觉就是问题的关键。下面展开。

---

## 三、策略模式面对二维变化的三种解法

### 解法 A：暴力展开 —— 每个组合一个策略类

```csharp
public interface IDamageFormula {
    int CalcDamage(int baseAtk);
}

public class FireVsHeavy : IDamageFormula {
    public int CalcDamage(int baseAtk) => baseAtk * 2;
}
public class FireVsRobe : IDamageFormula {
    public int CalcDamage(int baseAtk) => baseAtk;
}
public class FireVsLeather : IDamageFormula {
    public int CalcDamage(int baseAtk) => (int)(baseAtk * 1.5);
}
// ... 还有 6 个类
```

| 维度 | 策略类数量 |
|------|-----------|
| 1 维（3 种元素） | 3 |
| 2 维（3 元素 × 3 护甲） | 9 |
| 3 维（3 元素 × 3 护甲 × 3 武器） | 27 |

**类是线性增长的，但类的数量 = 维度的乘积。** 策略模式没有失效，只是变得笨重。

### 解法 B：策略组合 —— 把两个维度拆成两个策略接口

```csharp
// 维度 1：元素的伤害规则
public interface IElementRule {
    float GetMultiplier(string armorType);
}

// 维度 2：护甲的被伤害规则（可选，取决于谁"拥有"规则）
public interface IArmorRule {
    float GetMitigation(string element);
}
```

但问题是：**伤害公式 = 元素 × 护甲的交叉运算**，不是简单的"先算元素再加护甲"，拆开后反而难以表达交叉逻辑。

→ 当两个维度**不是正交可叠加的**，而是**交叉耦合**的时候，策略组合就不如直接展开。

### 解法 C：查表法 —— 用数据替代代码

```csharp
// 伤害倍率表：伤害表[元素][护甲] = 倍率
var damageTable = new Dictionary<string, Dictionary<string, float>> {
    ["Fire"] = new() { ["Heavy"] = 2.0f, ["Robe"] = 1.0f, ["Leather"] = 1.5f },
    ["Ice"]  = new() { ["Heavy"] = 1.0f, ["Robe"] = 3.0f, ["Leather"] = 1.0f },
    ["Thunder"] = new() { ["Heavy"] = 1.0f, ["Robe"] = 1.5f, ["Leather"] = 2.0f },
};

public class DamageCalculator {
    public int CalcDamage(string element, string armorType, int baseAtk) {
        return (int)(baseAtk * damageTable[element][armorType]);
    }
}
```

| 方案 | 优点 | 缺点 |
|------|------|------|
| 策略展开（9 个类） | 编译期安全、每个公式可含复杂逻辑 | 类爆炸 |
| 策略组合（2+3 接口） | 类少 | 交叉耦合时拆不干净 |
| 查表法 | 极简、策划可配表 | 只能表达纯倍率公式，复杂逻辑放不进表 |

**实际项目里，这三种经常混用**：核心规则用策略类，数值倍率用配置表。

### 一句话分界线（2026-07-22）

> **"普遍性的用查表，特殊性的用策略。"**

| 场景 | 工具 |
|------|------|
| 所有人都遵守同一套规则，只是数值不同（如每种移动方式对所有地形都有速度值） | 查表法 |
| 不同的人有**不同的规则逻辑**（如飞行关心天气，行走不关心） | 策略模式 |

查表强制统一，策略允许差异。实际项目分层使用：策略类决定**规则**，表决定**数值**。

---

## 四、关键认知：谁是不变量？

你在回答里提到的「不变 + 变」，其实是在问：**从调用方的视角，哪个维度是固定的？**

```csharp
// 场景 1：角色已确定元素（火法），对不同护甲的敌人放技能
// → 元素是"不变的"，护甲是"变的"
// → 策略 = 火元素策略，内部处理不同护甲分支（3 个分支，可控）

// 场景 2：角色可以使用多种元素技能，攻击一个已知护甲的敌人
// → 护甲是"不变的"，元素是"变的"
// → 策略 = 护甲策略，内部处理不同元素分支（3 个分支，可控）
```

**策略模式的核心不是消灭 if-else，而是把 if-else 关进一个策略类内部，不让它扩散到调用方。** 当两个维度都变化时，你选择让其中一个维度成为"策略的划分依据"，另一个维度成为"策略内部的 if-else/查表"。

这就是设计决策——**选择哪个维度变化频率更高、更不稳定，让它做策略接口的划分维度。**

---

## 五、何时策略模式不够用？

当出现以下信号时，需要换武器：

| 信号 | 需要的模式 |
|------|-----------|
| 多个维度交叉运算，且每个维度独立演化 | **访问者模式（Visitor）**——双重分发 |
| 策略本身有生命周期/状态（如"燃烧"持续伤害） | **状态模式（State）**——策略+状态机 |
| 策略之间可以组合叠加（火+冰=融化加成） | **装饰器模式（Decorator）** 或 **责任链模式（Chain of Responsibility）** |
| 策略的选择逻辑本身很复杂 | 策略的**工厂** + **规则引擎** |

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP → 策略模式是 OCP 的标准实现 | 《敏捷》第 9 章 / GoF 第 5 章 |
| DIP → 策略不应由使用方自己 new | 《敏捷》第 11 章 |
| Replace Conditional with Polymorphism | 《重构》第 10 章 |
| 多维变化 → Visitor 模式（预告） | GoF 第 5 章 |

---

## 七、作业（预计 10 分钟）

你在做的一个游戏里，角色有不同的**移动方式**（行走、骑马、飞行），每种移动方式对不同的**地形**（平原、沼泽、山地）有不同的速度和消耗：

```
行走：平原 1.0x，沼泽 0.5x，山地 0.7x
骑马：平原 2.0x，沼泽 0.3x，山地 0.5x
飞行：平原 1.5x，沼泽 1.5x，山地 1.5x（无视地形）
```

要求：
1. 用策略模式定义移动策略接口
2. 实现 3 种移动方式
3. 思考：地形是"变化维度"还是移动方式是？你选哪个做策略划分维度，为什么？

```csharp
// 框架给你：
public interface IMoveStrategy
{
    float GetSpeedMultiplier(string terrain);
}
```

`[进度：设计模式-①策略模式 → 核心讲解完成，等待作业 ✓]`
