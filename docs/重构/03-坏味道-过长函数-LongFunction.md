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

### 第一轮

1. 随着需求增加代码量增加后续维护压力逐渐增大
2. 关心了不属于这个方法所关心的事情，通过消息来解耦去其他系统
3. 三种操作叠加在一起：基础属性变化、特定等级属性变化、满级操作

### 第二轮（子问题）

1. 较难找到一个合适的位置来修改这个代码
2. 基础属性 / 阶段属性变化 / 满级操作 / 升级事件（增加新事件修改处）

## 四、纠错（第一轮，2026-08-03）

- 第 1 点：所有坏味道的共性症状，非「过长函数」的特异诊断。Fowler 的特异标准是：**多个抽象层次混在同一函数**——数值细节（`Hp += 20`）、分支意图（「每 10 级大成长」）、流程步骤（「结算完通知 UI」）平铺在一条流水线里，读的人眼睛要在三个层次之间来回跳。
- 第 2 点：工具用错。消息解耦是「换」不是「拆」（铁律：只拆不换）。「过长函数」的标准武器只有一个：**提炼函数 Extract Function**——把每块步骤抠出来命名，函数名成为意图的注释，调用顺序一行不动。
- 第 3 点：方向对——确实看到三块（基础成长 / 10 级里程碑 / 满级 God Mode）。

**子问题纠错（第二轮）**：「难找位置」答对了症状的一半，但没答出核心——**要记住多少件事**。100 级分支同时满足 `%10 == 0`（两个分支都会执行），尾部 UI→音频→成就→存档顺序敏感（存档必须在最后）。没有函数名作路标，任何改动都要求**全局推理**：把全部 40 行的每一步都记在脑子里才能确认改对。

## 五、标准解（2026-08-03 给出）

Fowler 原文（《重构》第 3 章）：**"The longer a function is, the harder it is to understand."**（函数越长，越难理解。）

过长函数的代价是破坏**推理的局部性**（locality of reasoning）：

- 改第 100 级需求，你得先通读 40 行找到 `if (player.Level == 100)`——没有名字作路标；
- 100 级同时满足 10 级里程碑条件，两个分支都执行——只盯一个分支改就会漏；
- 尾部四个调用（UI/音频/成就/存档）顺序敏感——存档必须在最后。

标准武器：**提炼函数 Extract Function**（第 6 章）——把每块步骤抠出来命名，函数名成为意图的注释：

```csharp
public void LevelUp(Player player)
{
    if (!HasEnoughExp(player)) return;
    ApplyBaseGrowth(player);
    if (player.Level % 10 == 0) ApplyMilestoneBonus(player);
    if (player.Level == 100)   ApplyGodMode(player);
    NotifyLevelUp(player);
}

private bool HasEnoughExp(Player p) => p.Exp >= p.Level * 100 + 50;

private void ApplyBaseGrowth(Player p) { /* 原基础成长块：数值不动 */ }
private void ApplyMilestoneBonus(Player p) { /* 原 %10 块 */ }
private void ApplyGodMode(Player p) { /* 原 ==100 块 */ }
private void NotifyLevelUp(Player p) { /* UI+音频+成就+存档，顺序不动 */ }
```

现在改「100 级加专属皮肤」：直接进 `ApplyGodMode`，只读那一个函数体，其余 6 行不用看。**名字 = 路标 = 局部推理的入口。**

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（两轮纠错 + 标准解）
2. 把 `LevelUp` 拆成多个小函数——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/LevelUp.cs](Homework/LevelUp.cs)

---

`[进度：阶段三-重构 → Day 3「过长函数」进行中]`
