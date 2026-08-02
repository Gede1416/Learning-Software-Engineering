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

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 把 `LevelUp` 拆成多个小函数——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/LevelUp.cs](Homework/LevelUp.cs)

---

`[进度：阶段三-重构 → Day 3「过长函数」进行中]`
