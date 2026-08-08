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

## 三、你的回答（2026-08-06，同步自 00-我的回答.md）

1. 换成了多态接口  Replace Conditional with 多态
2. 重构判断
   1. 工厂顶层 用来顶部决策 不该进行多态 增加额外复杂度
   2. 已经明确使用需求不会发生变化的功能
3. 提炼类组合 是将耦合多个抽象功能的类抽离出 功能管理类 -> 多个功能实现类
   观察者是让 类不去关心不属于自己的这个抽象功能 通过消息去通知真正的抽象功能

## 四、标准解（2026-08-06）

### 1. 填空：Replace Conditional with **Polymorphism**（以多态取代条件表达式）——《重构》第 10 章

策略模式作业 `CalcGold` 的 if-else → `ICalcGold` + NormalGold/EliteGold/BossGold 就是这个手法的落地：多态 = 行为变体的封装。

### 2. 重构到哪里是头（核心判据）

- **该改**（场景 B）：分支代表「类型的行为变体」，且新类型持续增加 → 多态让「加新类型 = 加新类」不碰旧代码（OCP 生效）
- **不该改**（场景 A/C）：分支多年不变 / 分支极少且简单 → if-else 直白可读，多态反而多出接口+类 = 阅读成本
- 判据一句话：**重构是为"即将到来的变化"付费**——没有变化预期，抽象就是 Speculative Generality（夸夸其谈的通用性，Day 11 惰性元素的近亲；Day 12 你为统一加钱造 `IAddGold`+`NormalAddGold` 就是这个反面教材）
- 「工厂顶层决策」的校正：创建对象时的类型选择（工厂 switch）与运行时行为分发不同——前者是**一次性创建决策**，查表/工厂处理即可；但若工厂 switch 分支的行为本身复杂且会扩展，它同样是 Replace Conditional 的候选。**判据不看位置，看分支会不会扩散**

### 3. 映射表（用户问题 3 回答基本全对 ✅）

| 重构手法 | 对应模式/原则 | 你的实例 |
|---|---|---|
| Replace Conditional with Polymorphism | 策略/状态模式 | CalcGold |
| 提炼类 Extract Class | 组合（组合优先于继承） | GameManager → 4 系统类 |
| 事件通知解耦 | 观察者模式 | Day 6 事件总线 |

### 4. 验收：概念第一轮即过 ✅（填空正确；「不变化不改」命中核心判据；问题 3 映射准确）

---

`[进度：阶段三-重构 → Day 13「重构与设计模式的映射」苏格拉底问答中]`
