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
