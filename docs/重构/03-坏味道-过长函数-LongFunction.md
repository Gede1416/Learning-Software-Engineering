# 过长函数（Long Function）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 3 位

---

## 一、坏代码场景

玩家升级结算——一个函数干了八件事：

```csharp
public void LevelUp(Player player)
{
    int need = player.Level * 100 + 50;
    if (player.Exp >= need)
    {
        player.Level++;
        player.Exp -= need;
        player.MaxHp += 20;
        player.Hp = player.MaxHp;
        player.MaxMp += 10;
        player.Mp = player.MaxMp;
        player.Atk += 5;
        player.Def += 3;
        if (player.Level % 10 == 0)
        {
            player.MaxHp += 50;
            player.MaxMp += 30;
            player.Atk += 15;
            player.Def += 8;
            player.SkillPoints += 3;
        }
        if (player.Level == 100)
        {
            player.HasGodMode = true;
            player.SkillPoints += 100;
        }
        UIManager.ShowLevelUp(player.Level);
        AudioManager.PlayLevelUp();
        AchievementSystem.Unlock("level_" + player.Level);
        SaveSystem.Save(player);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：数一数它做了几件事。策划改属性成长公式、加 50 级里程碑、换存档系统——每次你要在这个函数里找到哪里？这个函数会越来越长吗？）

## 三、你的回答（2026-08-03）

1. 随着需求增加代码量增加后续维护压力逐渐增大
2. 关心了不属于这个方法所关心的事情，通过消息来解耦去其他系统
3. 三种操作叠加在一起：基础属性变化、特定等级属性变化、满级操作

## 四、纠错（第一轮，2026-08-03）

- 第 1 点：所有坏味道的共性症状，非「过长函数」的特异诊断。Fowler 的特异标准是：**多个抽象层次混在同一函数**——数值细节（`Hp += 20`）、分支意图（「每 10 级大成长」）、流程步骤（「结算完通知 UI」）平铺在一条流水线里，读的人眼睛要在三个层次之间来回跳。
- 第 2 点：工具用错。消息解耦是「换」不是「拆」（铁律：只拆不换）。「过长函数」的标准武器只有一个：**提炼函数 Extract Function**——把每块步骤抠出来命名，函数名成为意图的注释，调用顺序一行不动。
- 第 3 点：方向对——确实看到三块（基础成长 / 10 级里程碑 / 满级 God Mode）。

**待答子问题**：策划新需求——第 100 级时，除 God Mode 外还要解锁专属皮肤。在哪一行改？动手前需要在脑子里记住多少件事，才能确定改对？

## 五、标准解（待给出）

（子问题回答正确后给出）

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题已答，待按子问题修正
2. 把 `LevelUp` 拆成多个小函数——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/LevelUp.cs](Homework/LevelUp.cs)

---

`[进度：阶段三-重构 → Day 3「过长函数」进行中]`
