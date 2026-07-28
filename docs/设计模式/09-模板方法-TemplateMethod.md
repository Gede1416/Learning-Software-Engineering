# 模板方法模式（Template Method）

> 来源：《设计模式》GoF 第 5 章 + 《Head First 设计模式》第 8 章

---

## 一、书中定义

GoF 的定义：

> **"在一个方法中定义一个算法的骨架，而将一些步骤延迟到子类中。模板方法使得子类可以不改变算法结构的情况下，重新定义算法中的某些步骤。"**

Head First 的直觉版：**"模板方法 = 基类定好流程清单，子类填空。"**

---

## 二、坏代码场景

假设你在做一个卡牌游戏的**技能释放流程**。所有技能都遵循同样的阶段，但每个阶段的细节不同：

```
技能释放流程：
① 前置检查（蓝够不够、CD 到了没）
② 播放施法动画
③ 选择目标
④ 执行效果（造成伤害 / 治疗 / 加 Buff）
⑤ 进入冷却
⑥ 触发被动技能检测
```

当前代码——每种技能各写一套完整流程：

```csharp
public class Fireball
{
    public void Cast(Player caster)
    {
        // ① 前置检查
        if (caster.MP < 30) return;
        if (_cooldown > 0) return;

        // ② 播放动画
        caster.Animator.Play("CastFireball");

        // ③ 选择目标
        var target = GetTargetedEnemy();

        // ④ 执行效果
        target.TakeDamage(50);

        // ⑤ 进入冷却
        _cooldown = 3.0f;

        // ⑥ 触发被动
        PassiveSystem.OnSpellCast(caster, "Fireball");
    }
}

public class Heal
{
    public void Cast(Player caster)
    {
        // ① 前置检查
        if (caster.MP < 40) return;
        if (_cooldown > 0) return;

        // ② 播放动画
        caster.Animator.Play("CastHeal");

        // ③ 选择目标
        var target = GetLowestHPAlly();

        // ④ 执行效果
        target.HP += 60;

        // ⑤ 进入冷却
        _cooldown = 5.0f;

        // ⑥ 触发被动
        PassiveSystem.OnSpellCast(caster, "Heal");
    }
}
```

---

## 问题

1. 如果策划说「所有技能释放前加一步：先扣金币，金币不够放不出技能」，你要改多少处？怎么保证不会忘记改某个技能？

2. 这和策略模式有什么区别？——策略是「整个算法替换」，模板方法是「算法骨架固定，只填空」。什么时候用哪个？

3. 模板方法里，基类定好的骨架步骤，子类能不能跳过某一步？如果不能，怎么办？（提示：钩子方法 Hook）

---

## 你的回答（2026-07-27）

1. **定义技能释放管线（类似渲染管线）** ✅ — 统一流程，所有技能走同一条管线。这就是模板方法。
2. **策略是单一小方法，模板是多个策略接口 + 链接代码组成的完整管线** ✅ — 大小粒度之分。
3. **通过定义接口的返回值 bool 去判断** — 方向对，这就是钩子方法的机制。下面展开。

---

## 三、标准重构：基类定骨架，子类填空

### 3.1 代码

```csharp
// ① 抽象基类 —— 定义技能释放的骨架（管线）
public abstract class Spell
{
    public int ManaCost;
    public float Cooldown;

    // ★ 模板方法 —— 算法的骨架，子类不能重写
    public void Cast(Player caster)
    {
        // ① 前置检查
        if (!CheckCondition(caster)) return;

        // ② 扣资源
        ConsumeResource(caster);

        // ③ 播放动画
        PlayAnimation(caster);

        // ④ 选择目标
        var target = SelectTarget(caster);

        // ⑤ 执行效果 —— 子类来填空
        ExecuteEffect(caster, target);

        // ⑥ 进入冷却
        StartCooldown();

        // ⑦ 触发被动
        NotifyPassive(caster);
    }

    // ==== 固定步骤（基类实现，子类一般不重写）====
    protected virtual bool CheckCondition(Player caster)
    {
        if (caster.MP < ManaCost) return false;
        if (CooldownRemaining > 0) return false;
        return true;
    }

    protected virtual void ConsumeResource(Player caster)
    {
        caster.MP -= ManaCost;
    }

    protected virtual void StartCooldown()
    {
        CooldownRemaining = Cooldown;
    }

    protected virtual void NotifyPassive(Player caster)
    {
        PassiveSystem.OnSpellCast(caster, GetType().Name);
    }

    // ==== 钩子方法 —— 子类可以重写来控制是否执行某步 ====
    protected virtual bool ShouldNotifyPassive() => true;  // ← 钩子

    // ==== 抽象方法 —— 子类必须填空 ====
    protected abstract void PlayAnimation(Player caster);
    protected abstract Character SelectTarget(Player caster);
    protected abstract void ExecuteEffect(Player caster, Character target);
}

// ② 火球术 —— 只填自己不同的部分
public class Fireball : Spell
{
    public Fireball() { ManaCost = 30; Cooldown = 3.0f; }

    protected override void PlayAnimation(Player caster)
    {
        caster.Animator.Play("CastFireball");
    }

    protected override Character SelectTarget(Player caster) => caster.GetTargetedEnemy();

    protected override void ExecuteEffect(Player caster, Character target)
    {
        target.TakeDamage(50);
    }
}

// ③ 治疗术
public class Heal : Spell
{
    public Heal() { ManaCost = 40; Cooldown = 5.0f; }

    protected override void PlayAnimation(Player caster)
    {
        caster.Animator.Play("CastHeal");
    }

    protected override Character SelectTarget(Player caster) => caster.GetLowestHPAlly();

    protected override void ExecuteEffect(Player caster, Character target)
    {
        target.HP += 60;
    }
}

// ④ 被动技能 —— 没有动画、不选目标、不触发被动
public class PassiveAura : Spell
{
    public PassiveAura() { ManaCost = 0; Cooldown = 0; }

    protected override void PlayAnimation(Player caster) { }  // 无动画

    protected override Character SelectTarget(Player caster) => caster;  // 目标是自身

    protected override void ExecuteEffect(Player caster, Character target)
    {
        // 持续光环，不需要 Cast 触发
    }

    protected override bool ShouldNotifyPassive() => false;  // ← 钩子：不触发被动
}
```

现在加「所有技能释放前扣金币」——**只改基类 `Spell` 的 `CheckCondition` 方法，所有子类零修改。**

---

## 四、钩子方法（Hook）—— 你第三问的答案

你说的「通过定义 bool 返回值去判断」就是钩子方法。

> **钩子方法 = 基类提供一个默认返回 true/false 的 virtual 方法，子类可以重写它来控制「某一步要不要执行」。**

```csharp
// 基类里的钩子
protected virtual bool ShouldNotifyPassive() => true;

// 模板方法里使用钩子
public void Cast(Player caster)
{
    // ...
    ExecuteEffect(caster, target);
    StartCooldown();

    if (ShouldNotifyPassive())  // ← 钩子控制是否执行这一步
        NotifyPassive(caster);
}

// 被动技能：不触发被动 → 重写钩子返回 false
protected override bool ShouldNotifyPassive() => false;
```

| 普通 virtual 方法 | 钩子方法 |
|-------------------|---------|
| 子类重写来**改变怎么做** | 子类重写来**控制做不做** |
| `PlayAnimation()` → 子类决定播什么动画 | `ShouldNotifyPassive()` → 子类决定是否触发被动 |

---

## 五、模板方法 vs 策略模式 —— 你第二条的洞察

| | 策略模式 | 模板方法 |
|------|----------|----------|
| **粒度** | 一个算法 | 一个**流程**（多个步骤） |
| **控制权** | 在外部（谁持有策略引用） | 在基类（骨架定死了） |
| **替换范围** | 整个算法 | 单一步骤 |
| **关系** | Context + Strategy = 组合 | 基类 + 子类 = 继承 |
| **你的话** | "单一小方法" | "多个策略接口 + 链接代码组成的完整管线" |

> **策略是「整个算法可替换」；模板方法是「算法骨架固定，步骤可替换」。**

实际的游戏项目里，**模板 + 策略经常混用**：模板定义释放管线（①②③④⑤⑥），其中第④步「执行效果」内部使用策略模式（伤害策略 / 治疗策略 / Buff 策略）。

---

## 六、设计原则：好莱坞原则

Head First 把模板方法绑定了一个原则：

> **"Don't call us, we'll call you."（别打电话给我们，我们会打给你。）**

基类说：你（子类）别自己主动搞事情，把你的步骤写好放那儿，**我来决定什么时候调用你。**

这也是 Ioc（控制反转）的最原始形态。

---

## 七、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 工厂方法 —— 本身就是模板方法的一个应用（`SpawnWave` = 模板，`CreateEnemy` = 工厂方法） | GoF 第 3 章 |
| 策略模式 —— 模板 + 策略经常混用 | GoF 第 5 章 |
| 好莱坞原则 → IoC/DI 的雏形 | 《Head First》第 8 章 |
| 渲染管线、HttpPipeline、中间件链 —— 全是模板方法模式 | 业界实践 |

---

## 八、作业（预计 10 分钟）

做一个**关卡加载模板**。每个关卡的加载流程相同，但具体步骤不同：

```
骨架（模板方法）：
① 显示 Loading 界面
② 加载场景资源 ← 各关卡不同
③ 初始化敌人配置 ← 各关卡不同
④ 播放 BGM ← 各关卡不同
⑤ 隐藏 Loading 界面
⑥ （可选）播放过场动画 ← 钩子：不是每关都有
```

```csharp
public abstract class LevelLoader
{
    // 模板方法 —— 你来写
    public void Load() { /* ... */ }

    // 固定步骤
    private void ShowLoading() { Console.WriteLine("显示 Loading..."); }
    private void HideLoading() { Console.WriteLine("隐藏 Loading"); }

    // 抽象步骤 —— 子类填空
    protected abstract void LoadAssets();
    protected abstract void SetupEnemies();
    protected abstract void PlayBGM();

    // 钩子 —— 子类可选重写
    protected virtual bool HasCutscene() => false;
    protected virtual void PlayCutscene() { }
}
```

要求：实现两个关卡 `ForestLevel`（有过场）和 `DungeonLevel`（无过场）。

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 → 核心讲解完成，等待作业 ✓]`
