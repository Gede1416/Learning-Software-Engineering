# 工厂模式（Factory Pattern）

> 来源：《设计模式》GoF 第 3 章 + 《Head First 设计模式》第 4 章

---

## 一、书中定义

工厂模式实际上包含两个模式，GoF 分别定义：

### 工厂方法（Factory Method）

> **"定义一个用于创建对象的接口，让子类决定实例化哪一个类。工厂方法使一个类的实例化延迟到其子类。"**

### 抽象工厂（Abstract Factory）

> **"提供一个接口，用于创建一系列相关或相互依赖的对象，而无需指定它们具体的类。"**

Head First 把它们串成一条线：**简单工厂 → 工厂方法 → 抽象工厂**，本节课也按这个顺序展开。

---

## 二、坏代码场景：敌人刷怪系统

你在做一个地牢游戏的刷怪系统。不同关卡刷不同的敌人，而且同一关内，不同波次（wave）刷的敌人也不同：

```csharp
public class EnemySpawner
{
    public Enemy Spawn(string enemyType)
    {
        Enemy enemy;

        if (enemyType == "Skeleton")
        {
            enemy = new Enemy();
            enemy.HP = 50;
            enemy.ATK = 8;
            enemy.AIController = new MeleeAI();
            enemy.DropTable = new[] { "Bone", "Rusty Sword" };
        }
        else if (enemyType == "DarkMage")
        {
            enemy = new Enemy();
            enemy.HP = 30;
            enemy.ATK = 20;
            enemy.AIController = new RangedAI();
            enemy.DropTable = new[] { "Mana Potion", "Spell Scroll" };
        }
        else if (enemyType == "DragonBoss")
        {
            enemy = new Enemy();
            enemy.HP = 500;
            enemy.ATK = 50;
            enemy.AIController = new BossAI();
            enemy.DropTable = new[] { "Dragon Scale", "Legendary Sword" };
        }
        else
        {
            enemy = new Enemy();
        }

        return enemy;
    }
}
```

不止这里——**玩家的技能、商人出售的道具、任务奖励的装备**，到处都散落着这样的 `if (type == "...") { new ... }`。

---

## 问题

1. 如果策划说「所有敌人血量上调 20%」，你要改多少处？如果策划说「骷髅兵现在改用 PatrolAI 而不是 MeleeAI」，你要改多少处？

2. 策略模式那节课你发现 Boss 不应该自己 `new BossGold()`。但如果不让使用方 new，**谁来 new？** 这个「专门负责 new」的对象叫什么？

3. 如果敌人有「阵营」概念——亡灵阵营（骷髅法师 + 骷髅战士 + 巫妖）、恶魔阵营（小恶魔 + 地狱犬 + 恶魔领主），每个阵营内部是固定搭配。怎么确保刷怪时**一族敌人总是配套出现**，不会搞出「骷髅战士 + 恶魔领主」的混搭？

---

## 你的回答（2026-07-23）

1. **每个分支都要改** ✅ —— 创建逻辑散落各处，改一个敌人类型要改 N 个 if-else 分支。
2. **实体管理器 + 根据关卡不同 Init** ✅ —— 这就是简单工厂和工厂方法。
3. **阵营工厂 + 策划配置驱动** ✅ —— 这就是抽象工厂。

> 你已经在项目里用工厂模式了，这节课只是给它们正名。

---

## 三、三层工厂：从简单到抽象

### 3.1 简单工厂（Simple Factory）

你 Q2 说的「实体管理器」就是它——**把 new 的逻辑集中到一个地方**：

```csharp
// 简单工厂 —— 你项目里的「EnemyManager」
public class EnemyFactory
{
    public Enemy Create(string enemyType)
    {
        return enemyType switch
        {
            "Skeleton" => new Enemy { HP = 50, ATK = 8, AI = new MeleeAI() },
            "DarkMage" => new Enemy { HP = 30, ATK = 20, AI = new RangedAI() },
            "Dragon"   => new Enemy { HP = 500, ATK = 50, AI = new BossAI() },
            _ => new Enemy()
        };
    }
}

// 使用方：不再自己 new，全部走工厂
public class EnemySpawner
{
    private EnemyFactory _factory = new EnemyFactory();

    public Enemy Spawn(string type) => _factory.Create(type);
}
```

| 改前 | 改后 |
|------|------|
| 骷髅改血量 → 改所有 if-else 分支 | 只改 `EnemyFactory` 一处 |
| 加一个新敌人 | 在 `EnemyFactory` 加一个 case |

**但简单工厂仍然是 if-else / switch。它没消除分支，只是集中了。** 要消除分支，需要下一层。

### 3.2 工厂方法（Factory Method）

你 Q2 说的「根据关卡不同 Init」就是它——**把创建推迟到子类决定**：

```csharp
// ① 抽象创建者
public abstract class LevelSpawner
{
    // 工厂方法 —— 子类决定创建什么敌人
    public abstract Enemy CreateEnemy();

    // 模板方法 —— 刷怪流程固定，但创建哪个敌人由子类决定
    public void SpawnWave()
    {
        var enemy = CreateEnemy();   // ← 工厂方法
        enemy.OnSpawn();
    }
}

// ② 具体创建者 —— 每个关卡自己决定
public class ForestLevel : LevelSpawner
{
    public override Enemy CreateEnemy()
    {
        return new Enemy { Name = "树精", HP = 80, AI = new PatrolAI() };
    }
}

public class DungeonLevel : LevelSpawner
{
    public override Enemy CreateEnemy()
    {
        return new Enemy { Name = "骷髅兵", HP = 50, AI = new MeleeAI() };
    }
}

public class VolcanoLevel : LevelSpawner
{
    public override Enemy CreateEnemy()
    {
        return new Enemy { Name = "火焰蜥蜴", HP = 120, AI = new RangedAI() };
    }
}
```

```
                    ┌──────────────┐
                    │ LevelSpawner │ ← 抽象类，定义骨架
                    │ CreateEnemy()│ ← 工厂方法（子类实现）
                    │ SpawnWave()  │ ← 模板方法（不用改）
                    └──────┬───────┘
                           △
            ┌──────────────┼──────────────┐
            │              │              │
   ┌────────┴────┐ ┌──────┴──────┐ ┌─────┴──────┐
   │ForestLevel  │ │DungeonLevel │ │VolcanoLevel│
   │CreateEnemy()│ │CreateEnemy()│ │CreateEnemy()│
   │→ 树精      │ │→ 骷髅兵    │ │→ 火焰蜥蜴 │
   └────────────┘ └─────────────┘ └────────────┘
```

**加一个新关卡 = 新建一个子类，不改已有代码。** 这就是 OCP。

### 3.3 抽象工厂（Abstract Factory）

你 Q3 说的「阵营工厂」就是它——**创建一族相关对象，确保成套出现**：

```csharp
// ① 抽象工厂接口 —— 一族敌人的契约
public interface IFactionFactory
{
    Enemy CreateMelee();   // 该阵营的近战
    Enemy CreateRanged();  // 该阵营的远程
    Enemy CreateBoss();    // 该阵营的 Boss
}

// ② 亡灵阵营工厂
public class UndeadFactory : IFactionFactory
{
    public Enemy CreateMelee()  => new Enemy { Name = "骷髅战士", HP = 60 };
    public Enemy CreateRanged() => new Enemy { Name = "骷髅法师", HP = 35 };
    public Enemy CreateBoss()   => new Enemy { Name = "巫妖王",   HP = 400 };
}

// ③ 恶魔阵营工厂
public class DemonFactory : IFactionFactory
{
    public Enemy CreateMelee()  => new Enemy { Name = "小恶魔",   HP = 40 };
    public Enemy CreateRanged() => new Enemy { Name = "地狱犬",   HP = 70 };
    public Enemy CreateBoss()   => new Enemy { Name = "恶魔领主", HP = 500 };
}

// ④ 使用方：注入工厂，不关心具体阵营
public class DungeonGenerator
{
    private IFactionFactory _faction;

    public DungeonGenerator(IFactionFactory faction)
    {
        _faction = faction;  // 注入什么阵营，就生成什么敌人族
    }

    public void PopulateDungeon()
    {
        var melee  = _faction.CreateMelee();   // 近战 ×5
        var ranged = _faction.CreateRanged();  // 远程 ×3
        var boss   = _faction.CreateBoss();    // Boss ×1
        // ← 保证这三个是一族的，不会混搭
    }
}

// ✅ 亡灵地牢
new DungeonGenerator(new UndeadFactory()).PopulateDungeon();

// ✅ 恶魔地牢 —— 同一套代码，换一个工厂
new DungeonGenerator(new DemonFactory()).PopulateDungeon();
```

抽象工厂的核心价值：**不只是封装 new，更重要的是保证一族对象之间的兼容性。**

---

## 四、三层工厂对比

| | 简单工厂 | 工厂方法 | 抽象工厂 |
|------|----------|----------|----------|
| 创建数量 | 1 个对象 | 1 个对象 | **一组**相关对象 |
| 分支在哪 | if-else / switch | 子类重写 | 整个工厂接口 |
| OCP | ⚠️ 加类型要改 switch | ✅ 新类型 = 新子类 | ✅ 新阵营 = 新工厂实现 |
| 你的叫法 | 实体管理器 | 不同关卡 Init | 阵营工厂 |
| GoF | 不算 23 种模式之一 | ✅ 23 种之一 | ✅ 23 种之一 |

---

## 五、和前三个模式的关联

### 5.1 工厂 + 策略：解决「谁来 new 策略」

还记得策略模式那节课你发现的痛点吗？

```csharp
// ❌ Boss 自己 new 了策略
public class Boss {
    ICalcGold calcGold = new BossGold();
}

// ✅ 工厂来 new 策略，Boss 只接收接口
public class Boss {
    ICalcGold calcGold;
    public Boss(ICalcGold gold) => calcGold = gold;  // 注入
}

// 工厂负责决定「什么敌人用什么策略」
public class GoldStrategyFactory {
    public ICalcGold Create(string enemyType) => enemyType switch {
        "Boss"  => new BossGold(),
        "Elite" => new EliteGold(),
        _       => new NormalGold()
    };
}
```

**策略封装「做什么」，工厂封装「谁来做」。** 两者天然配对。

### 5.2 工厂 + 装饰器：批量生产带附魔的武器

```csharp
public class EnchantedWeaponFactory
{
    public Weapon CreateFireIceSword()
    {
        return new FireEnchant(new IceEnchant(new IronSword()));  // 装饰器链
    }
}
```

使用方拿到的就是成品，不需要知道内部套了多少层装饰器。

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP —— 工厂方法让加新类型不改已有代码 | 《敏捷》第 9 章 |
| DIP —— 使用方依赖 `IFactionFactory` 抽象，不依赖具体工厂 | 《敏捷》第 11 章 |
| 依赖注入（DI）—— 工厂是 DI 容器的基础概念 | 《敏捷》第 11 章 |
| 策略 + 工厂组合 —— 封装创建逻辑 | 《Head First》第 4 章 "结合策略模式" |

---

## 七、作业（预计 10 分钟）

你的游戏有三种**技能类型**（火球术、冰锥术、治疗术），每种技能的伤害公式和施法条件不同。同时，技能有**品质等级**（普通、稀有、传说），同一技能的稀有版比普通版数值 ×1.5，传说版 ×2.0 且有额外效果。

要求：
1. 用**工厂方法**设计：`SkillFactory` 为抽象基类，`FireballFactory`、`IceShardFactory`、`HealFactory` 各负责创建一种技能
2. 用**抽象工厂**设计：按品质分 `NormalSkillFactory`、`RareSkillFactory`、`LegendarySkillFactory`，保证同一品质下火球/冰锥/治疗 **成套匹配**

选一个实现即可（推荐先工厂方法练手，抽象工厂想想就行）。

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 → 核心讲解完成，等待作业 ✓]`
