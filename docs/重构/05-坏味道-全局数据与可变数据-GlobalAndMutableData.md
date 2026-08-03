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

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 用**封装变量（Encapsulate Variable）**把全局字段收进私有字段 + 属性/方法访问——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/GlobalScore.cs](Homework/GlobalScore.cs)

---

`[进度：阶段三-重构 → Day 5「全局数据/可变数据」进行中]`
