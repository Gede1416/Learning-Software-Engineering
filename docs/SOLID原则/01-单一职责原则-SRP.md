# 单一职责原则（Single Responsibility Principle, SRP）

> 来源：《敏捷软件开发：原则、模式与实践》— Robert C. Martin，第 8 章

---

## 一、书中定义

> **"一个类应该有且仅有一个引起它变化的原因。"**
> — Robert C. Martin

"变化的原因" = "职责"。如果有多于一个原因会导致一个类被修改，那么这个类就承担了多个职责。

---

## 二、坏代码场景

假设你在开发一个 ARPG 的角色系统：

```csharp
public class Player
{
    // 属性
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Attack { get; set; }

    // 攻击逻辑
    public void AttackEnemy(Enemy enemy)
    {
        int damage = Attack - enemy.Defense;
        enemy.Hp -= damage;
        if (enemy.Hp <= 0)
            DropLoot(enemy);
    }

    // 掉落逻辑
    private void DropLoot(Enemy enemy)
    {
        var loot = enemy.LootTable.Roll();
        SaveToInventory(loot);
    }

    // 存档
    private void SaveToInventory(Item item) { /* SQLite 写入 */ }
    public void SavePlayerToDb() { /* 序列化 + SQLite */ }
    public void LoadPlayerFromDb(int id) { /* SQLite + 反序列化 */ }

    // 渲染
    public void PlayHitEffect() { /* 粒子特效 */ }
    public void UpdateHealthBar() { /* UI 刷新 */ }
}
```

### 诊断问题

> 美术想改血条样式、策划想调掉落表、后端想换持久化方案——这三个人的需求变更，分别要改谁？如果答案都是"去改 `Player` 类"，那就是 SRP 违反。

`Player` 类承担的职责：
| 职责 | 服务于 |
|------|--------|
| 属性存储 | 数据层 |
| 伤害计算 | 策划/战斗系统 |
| 掉落 + 背包写入 | 策划/道具系统 |
| 数据库存取 | 后端/持久化 |
| 粒子特效 + UI 刷新 | 美术/渲染 |

---

## 三、重构后

按 SRP 拆分成各司其职的类：

```csharp
// 1. 纯数据 + 战斗规则（策划关心）
public class Player
{
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Attack { get; set; }

    public int CalcDamage(Enemy enemy)
        => Attack - enemy.Defense;
}

// 2. 渲染（美术/UI 关心）
public class PlayerRenderer
{
    public void PlayHitEffect(Player p) { /* 粒子 */ }
    public void UpdateHealthBar(Player p) { /* UI */ }
}

// 3. 持久化（后端/工具关心）
public class PlayerRepository
{
    public void Save(Player p) { /* SQLite */ }
    public Player Load(int id) { /* SQLite */ }
}
```

---

## 四、关键补充：不要过度拆分

Martin 在书中强调：SRP 的触发条件是 **"变化实际发生"**。

> 如果一个职责今天没有变的迹象，强行拆它是过度设计（YAGNI）。

**实践策略**：允许类和职责暂时耦合，但不允许这种耦合持续到第二次被修改。
> "哪天策划让你同时调整掉落和技能逻辑且它们互相绊脚时，那一刻立刻拆。"

---

## 五、跨书关联

| 关联概念 | 书籍与章节 |
|----------|------------|
| 观察者模式（渲染监听数据变化） | 《设计模式》GoF 第 5 章 |
| 提炼类（Extract Class） | 《重构》Martin Fowler 第 7 章 |

---

## 六、作业

下面这个 `SkillManager` 有 3 个职责，请写出拆分后的类骨架（只需类名 + 核心方法签名，不用写实现）：

```csharp
public class SkillManager
{
    public void CastSkill(int skillId, Character caster, Character target) { }
    public bool IsSkillUnlocked(int skillId, Character caster) { }  // 查技能树
    public void PlaySkillAnimation(int skillId) { }                  // 播放动画
    public void PlaySkillSound(int skillId) { }                      // 播放音效
    public void SaveSkillConfig(int skillId, string json) { }        // 存配置表
}
```

**要求**：拆成不多于 4 个类，每个类一句话注释说明"它服务于谁"。
