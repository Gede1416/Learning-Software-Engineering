# 重复代码（Duplicated Code）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单（第 1 版第 1 位 / 第 2 版第 2 位）

---

## 一、坏代码场景

两种敌人，死亡奖励公式各写了一份：

```csharp
// 普通怪
public class NormalEnemy
{
    public void OnDie(Player player)
    {
        int gold = 10 + player.Level * 2;
        int xp   = 20 + player.Level * 3;
        player.GainGold(gold);
        player.GainXp(xp);
    }
}

// 精英怪
public class EliteEnemy
{
    public void OnDie(Player player)
    {
        int gold = 10 + player.Level * 2;   // ← 和上面一模一样的公式
        int xp   = 20 + player.Level * 3;
        player.GainGold(gold);
        player.GainXp(xp);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：策划明天改等级公式、加周末双倍、加首杀翻倍——每一处变化，你要记得改几个地方？）

## 三、你的回答（2026-08-03）

1. 尝试 1：抽通用接口 `IOnEnemyDieReward` —— ⚠️ 接口统一「调用」不统一「方法体」，两份公式还躺在两个类里，N 没变小。
2. 尝试 2：公式住进接口实现，敌人 `Init` 注入 + 委托 `_onEnemyDieReward.OnDieReward(player)` —— ✅ 设计正确（注入式提取，重复消灭，且为「差异」预留了策略位），但方法体里 `onEnemyDieReward` 丢了 `_` 前缀（编译不过，Day 1 的 `_` 约定当天复发）。

## 四、标准解 —— 提取函数 / 提取类（Extract Function）

Fowler《重构（第 2 版）》重构手法目录第 1 位：**提炼函数（Extract Function）**——把重复的逻辑搬到一个共用处，多处改一处。

```csharp
public class RewardCalculator
{
    public static void GrantKillReward(Player player)
    {
        player.GainGold(10 + player.Level * 2);
        player.GainXp(20 + player.Level * 3);
    }
}
public class NormalEnemy { public void OnDie(Player player) => RewardCalculator.GrantKillReward(player); }
public class EliteEnemy  { public void OnDie(Player player) => RewardCalculator.GrantKillReward(player); }
```

### 比例感（跨书关联，阶段二策略模式的边界）

- **一模一样** → 提取共用（今天）：重复是「不变」的部分，接口/策略派不上用场，共用方法/类刚好。
- **不一样但形状相同** → 策略/接口：差异才是策略的猎物。
- 升级版：注入式策略（学生尝试 2）同样消灭重复，并为后续差异预留——但今天用静态共用方法更符合比例。

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 把重复的奖励公式抽到一处，让 `NormalEnemy` 和 `EliteEnemy` 共用——**铁律：只抽取代码，不改数值、不改行为**
3. 骨架：[Homework/重构/第一轮-最常踩的坏味道/EnemyReward.cs](Homework/重构/第一轮-最常踩的坏味道/EnemyReward.cs)

## 六、作业验收（2026-08-03）

- 方案 B（注入式策略）实现 ✅：公式只住一处（`NormalOnEnemyDieReward`），两个敌人持接口委托
- 修正过程：
  1. `onEnemyDieReward` → `_onEnemyDieReward`（Day 1 的 `_` 约定，方法体内漏前缀）
  2. `NormalOnEnemyDieReward : IOnEnemyDieReward` 补上接口声明（否则 `Init(new ...)` 编译不过）
  3. 清除悬空 TODO 注释
- 编译通过（0 错误）

---

`[进度：阶段三-重构 → Day 2「重复代码」✓（作业验收通过 2026-08-03）]`
