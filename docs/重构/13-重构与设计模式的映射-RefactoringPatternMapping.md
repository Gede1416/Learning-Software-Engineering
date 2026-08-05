# 重构与设计模式的映射（Refactoring → Design Patterns）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 10-12 章 + 《设计模式》GoF（阶段二全表回顾）
> 核心问题：重构到哪里是头？——所有 if-else 都该变成多态吗？

---

## 一、坏代码场景

战斗结算——按敌人类型分流（Day 9 你跳过的 switch→多态，今天补回来；也是策略模式作业 CalcGold 的同一结构）：

```csharp
// 战斗结算：按敌人类型分流
public int CalculateReward(Enemy enemy)
{
    if (enemy.Type == EnemyType.Normal) return 10;
    else if (enemy.Type == EnemyType.Elite) return 50 + Random.Range(0, 20);
    else if (enemy.Type == EnemyType.Boss) return 200 + Random.Range(50, 100);
    return 0;
}
```

## 二、问题（2026-08-06 布置）

1. 这段代码你熟不熟？策略模式作业的 `CalcGold` 一模一样的结构——当时你把 if-else 换成了什么？那个重构手法在 Fowler 里的名字是 **Replace Conditional with ______**（填空）？
2. 但是——**是不是所有 if-else 都该换成多态**？Fowler 说重构不是炫技，什么时候这个 switch **不该动**？（提示：看分支会不会扩散、变化频率）
3. （后续）提炼类→组合、观察者 的映射——你 Day 11/12 和 Day 6 其实都实操过了，说出对应关系。

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段三-重构 → Day 13「重构与设计模式的映射」苏格拉底问答中]`
