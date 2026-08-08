# 测试保护下的重构工作流（Test-Driven Refactoring Workflow）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 4 章「构筑测试」+ 重构节奏
> 跨书联动：《修改代码的艺术》Michael Feathers —— 接缝（Seam）、遗留代码测试策略
> 核心问题：怎么安全改？——凭什么说「只拆不换」？

---

## 一、坏代码场景

```csharp
// 你刚做了一次重构：把伤害公式从 atk 参数改成读玩家属性
public class DamageSystem
{
    public int CalcDamage(Player p, int atk)
    {
        return atk * 2 - p.Def;          // 重构前
        // return p.Atk * 2 - p.Def;     // 重构后——改对了吗？
    }
}
```

旁边是 Day 11 那个 3000 行没有测试的 GameManager。

## 二、问题（2026-08-06 布置）

1. 重构铁律是「只拆不换」——你改完代码，怎么证明「没换」？**靠眼睛读一遍够吗**？不够的话需要什么？
2. Fowler 的重构节奏是什么？（填空：______ → 重构 → ______）
3. 如果是 3000 行无测试的遗留代码（Day 11 的 GameManager）——《修改代码的艺术》说的**接缝（Seam）**是什么？从哪里开始下手？

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段三-重构 → Day 14「测试保护下的重构工作流」苏格拉底问答中]`
