# 有意义的命名（Meaningful Names）

> 来源：《代码整洁之道》Robert C. Martin 第 2 章
> 跨书联动：重构 Day 1「神秘命名」——从"坏味道"到"预防"的同一枚硬币

---

## 一、坏代码场景

技能系统——名字在撒谎：

```csharp
public class SkillData
{
    public int dmg;        // 伤害值？还是治疗量？
    public float r;        // 半径？还是射程？
    public bool f;         // 友伤？群伤？还是飞行？
    public int cd;         // 冷却 cd？还是倒计时？
}

public class SkillSystem
{
    public void Use(int sk, Player p, Enemy e)     // sk = 技能 id？玩家用还是敌人用？
    {
        var d = GetD(sk, p);                        // GetD 是什么？
        e.HP -= d;
        if (e.HP <= 0) Kill(e, p);                  // Kill 只扣血还是结算奖励？
    }
}
```

## 二、问题（2026-08-09 布置）

1. 这段代码里的名字，哪些在撒谎？——`dmg` / `r` / `f` / `cd` / `sk` / `d` / `GetD` / `Kill`，逐个说
2. 当需求变化时（比如新技能"圣光治疗队友"），这些名字具体在哪一步**误导你改错**？
3. 一个变量叫 `d`，全文件搜索 `d` 会搜出什么？（搜索友好性）

## 三、你的回答（2026-08-14，同步自 00-我的回答.md）

1. 我的第一印象：dmg 伤害、r 半径、f 无法判断、sk 可能是技能id、d 计算后的伤害、p 玩家、e 敌人
2. 计算技能效果时我是该加等于还是 减等于 dmg；f 会不会影响我的技能效果
3. 所有包含 d 的单词

## 四、标准解（2026-08-14）

### Uncle Bob 命名五原则（《代码整洁之道》第 2 章）

1. **名副其实（Intention-Revealing）**：名字揭示意图——`d` 不揭示，`damageDealt` 揭示
2. **避免误导（Avoid Disinformation）**：`r` 半径/射程歧义；名字不能让人猜错
3. **做有意义的区分（Meaningful Distinctions）**：静态定义 vs 动态状态要分开——`baseCooldown` / `remainingCooldown`，不能都叫 `cd`
4. **读得出（Pronounceable）**：`sk` 怎么读？`skillId` 能
5. **可搜索（Searchable）**：单字母 `d` 全局搜索 = 所有含 d 的单词；`MAX_ITEMS` 好搜

### 场景改名单（用户第 2 轮补答通过）

| 坏名 | 好名 | 原则 |
|------|------|------|
| `dmg` | `damage`（治疗技能用 `healAmount` 更明确） | 名副其实 |
| `r` | `blastRadius` / `range`（按语义定） | 避免误导 |
| `f` | `isFriendlyFire`（布尔用 is- 前缀） | 避免误导 |
| `cd` | `baseCooldown`（配置）/ `remainingCooldown`（状态） | 有意义的区分 |
| `sk` | `skillId` | 读得出 |
| `d` | `damage` | 可搜索 |
| `GetD` | `CalcDamage(Player caster, int skillId)` | 动作+对象 |
| `Kill` | `KillAndSettle` 或拆出结算函数 | 名实相符——副作用必须暴露在名字里（Uncle Bob：函数要么做事要么回答问题，不能又做事又回答） |

### 验收
- Q2（治疗加减歧义）、Q3（搜索噪音）第一轮即过 ✅
- Q1 纠错 1 轮后补全：`cd` 双义 / `GetD` 词不达意 / `Kill` 副作用 / `r` 双义 全命中 ✅
- 作业（RenameSkill.cs）：主体命名全对（damage/range/canHitFriend/baseCoolDown/skillId/player/enemy/HpToZeroDropLootGetExp）；纠错 2 轮指出 `d`→`damge` 拼写错误（搜索 damage 找不到 damge）、残留 TODO 注释、`sk`/`p` 参数未改——**用户跳过最终修正，留待回补**（2026-08-14）

---

`[进度：阶段四-代码整洁 → Day 1「有意义的命名」苏格拉底问答中]`
