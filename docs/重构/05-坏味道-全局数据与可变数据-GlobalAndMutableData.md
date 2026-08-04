# 全局数据与可变数据（Global / Mutable Data）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 5、6 位（合讲）
> 跨书联动：《设计模式》单例模式的代价——单例本质是全局数据的高级形态

---

## 一、坏代码场景

全局分数 + 全局难度——谁都能读、谁都能写：

```csharp
public static class GameState
{
    public static int Score;        // 全局分数
    public static int Difficulty;   // 全局难度
}

public class ScoreSystem
{
    public void OnEnemyKilled(int value)
    {
        GameState.Score += value;
        if (GameState.Score > 10000)
        {
            GameState.Difficulty = 3;   // 击杀系统偷偷改了全局难度
        }
    }
}

public class Enemy
{
    public void Attack(Player player)
    {
        int dmg = 10 * GameState.Difficulty;   // 敌人读全局难度
        player.TakeDamage(dmg);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：`Score` 被几个地方写？想查「谁把分数改成了负数」要翻几个文件？策划要加「分数上限 99999」，你要在哪几个地方加判断？`Difficulty` 被谁偷偷改了，怪物强度突然变化玩家怎么想？）

## 三、你的回答（2026-08-03）

1. 外界直接赋值，每次修改值的时候都需要进行判断，额外增加复杂度
2. 任何一个地方都可以修改
3. 所有引用的地方，不管是读取还是写入
4. 所有给分数增加值的位置（都要加判断）
5. 没有任何心理准备（幽灵修改）

## 四、纠错（第一轮，2026-08-03）

- 第 1、4 点 ✅：加约束（如分数上限）要加到**每一个写入点**——「加一个需求 = 霰弹枪修改」的全局版。
- 第 2、3 点 ✅：任意位置可读写——数据没有唯一主人。
- 第 5 点 ✅：大白话点中「**幽灵修改**」——难度被击杀系统偷改，玩家无征兆撞上强怪。
- 缺的核心：**无法事后追查「谁改的」**——全局字段的读/写全不可见，断点打在字段行会同时命中所有读写，无法过滤「最后一次写入」。

**待答子问题**：线上玩家反馈「打了一局，分数变成 -500」。`GameState.Score` 被 6 个文件、14 处代码读写。怎么查出「谁在什么时候把它写成负数」？在 `public static int Score` 这一行打断点，会发生什么？

## 五、标准解（待给出）

（子问题回答正确后给出）

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题已答，待按子问题修正
2. 用**封装变量（Encapsulate Variable）**把全局字段收进私有字段 + 属性/方法访问——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/重构/GlobalScore.cs](Homework/重构/GlobalScore.cs)

---

`[进度：阶段三-重构 → Day 5「全局数据/可变数据」进行中]`
