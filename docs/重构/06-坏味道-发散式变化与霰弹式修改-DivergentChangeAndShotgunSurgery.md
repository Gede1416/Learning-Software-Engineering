# 发散式变化 + 霰弹式修改（Divergent Change / Shotgun Surgery）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 6、7 位（合讲）
> 跨书联动：《敏捷》SRP（一个类一个变化原因）与 OCP（对扩展开放）

---

## 一、坏代码场景

加一个新英雄，要动几个文件？

```csharp
public class Hero
{
    public string Name;
    public int Hp;
}

public class HeroFactory
{
    public Hero Create(string type)
    {
        switch (type)
        {
            case "战士": return new Hero { Name = "战士", Hp = 100 };
            case "法师": return new Hero { Name = "法师", Hp = 60 };
            default:     return new Hero { Name = "路人", Hp = 40 };
        }
    }
}

public class HeroAI
{
    public void Update(Hero hero)
    {
        if (hero.Name == "战士")   { /* 冲锋 */ }
        else if (hero.Name == "法师") { /* 放风筝 */ }
    }
}

public class HeroIcon
{
    public string GetIcon(Hero hero) => hero.Name switch
    {
        "战士" => "sword.png",
        "法师" => "staff.png",
        _ => "default.png"
    };
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：策划要加一个新英雄「刺客」——你要动几个文件？每个文件都要加一个分支，漏掉一个会怎样？
   反过来问：什么需求会让 `Hero` 类本身频繁改？什么需求会让 3 个文件一起改？——这两种"变化的方向"各叫什么名字？）

## 三、你的回答（2026-08-03）

### 第一轮

1. 需求变换要同步更新多个逻辑分支（AI / Icon / HeroFactory）
2. 解决方法：HeroFactory 创建 Hero 的时候传入 AI、icon 获取接口
3. 霰弹式修改

### 第二轮（子问题）

继承

### 第三轮（动手题第二轮）

继承 + 子类方案：Warrior/Mage 继承抽象 Hero，构造器写死属性，Factory switch 返回子类

### 第四轮（动手题第三轮）

HeroData struct + HeroTable 字典（只存 2 行）+ Factory 仍 switch + WarriorAI/MageAI 复活

## 四、纠错（第一轮，2026-08-03）

- 第 1、3 点 ✅：识别出「加英雄 → AI/Icon/Factory 三个文件一起改」= **霰弹式修改**（一个原因，N 处修改）。
- 第 2 点 ❌ 抢跑 + 工具不对：传接口进去是「换」不是「拆」（铁律：只拆不换），且这轮先诊断，解法轮不到。
- 缺：**发散式变化**没答——另一个方向：一个类被多种不同需求轮流改（血量公式/复活规则/初始装备都改 Hero）。两味道都是 SRP 违规但尺度相反：发散式变化 = 一个类多种变化原因 → 该**拆**（提炼类）；霰弹式修改 = 一个原因散落多处 → 该**合**（搬移收敛）。

**子问题判定（第二轮，2026-08-03）**：「继承」❌——继承/多态是治「重复的 switch」的武器（Day 9 预告），不是这两个味道的名字。正确答案：**发散式变化**。

**动手题第二轮验收 ❌（2026-08-03）**：问题比第一轮更多——
1. **默认英雄被消灭**：原「路人/Hp40」→ `default` 变 `new Warrior()`——数值+名字全变
2. **icon 值变了**：原 "sword.png/staff.png/default.png" → 存 "战士/法师"——输出字符串改变
3. **还是没成表**：switch 仍在 + 每英雄一个类，加英雄 = 2 处（case + 新类）
4. **死代码**：`IHeroAI`/`WarriorAI`/`MageAI` 不再被引用
5. **发明行为**：原版 AI 是注释占位（空行为），现在全部输出「英雄AI」
6. ✅ `HeroAIAction` 命名修正；⚠️ abstract 拦截直建（但改类型系统属「换」，且消灭「路人」合法类型）

## 五、标准解（2026-08-03 给出）

Fowler《重构》第 3 章（原文立场）：

| | 发散式变化 Divergent Change | 霰弹式修改 Shotgun Surgery |
|---|---|---|
| 方向 | 一个类，被**多种**不同需求轮流改 | 一个需求，散落在**多个**类里改 |
| 典型 | 血量公式/复活规则/初始装备都改 Hero | 加英雄要改 Factory + AI + Icon |
| 治法 | **拆**（提炼类，每类一个变化原因 = SRP） | **合**（搬移函数/字段，收敛到一处） |
| 关系 | 同一枚硬币的两面 | 同一枚硬币的两面 |

判别口诀：**发散 = 一个类多种原因 → 拆；霰弹 = 一个原因多处 → 合。**

**为什么多态/继承在这里是死路**：战士/法师的差异是**数据差异**（血量/图标/名字），不是**逻辑差异**——数据型差异用**表**，逻辑型差异才用**多态**（Day 9 预告）。继承把数据差异硬造成类差异：加英雄 = 新类 + switch case，霰弹枪没治好，还消灭「路人」这种廉价默认值。

Fowler 武器：**搬移语句 + 查表**——散落数据收敛成一张表：

```csharp
public class Hero
{
    public string Name;
    public string icon;
    public int Hp;
}

// 一张表：加英雄 = 加一行（霰弹枪 → 单发）
public static class HeroTable
{
    public static readonly Dictionary<string, Hero> Config = new()
    {
        ["战士"] = new Hero { Name = "战士", icon = "sword.png", Hp = 100 },
        ["法师"] = new Hero { Name = "法师", icon = "staff.png", Hp = 60 },
        ["路人"] = new Hero { Name = "路人", icon = "default.png", Hp = 40 },
    };
}

public class HeroFactory
{
    public Hero Create(string type) =>
        HeroTable.Config.TryGetValue(type, out var h) ? h : HeroTable.Config["路人"];
}

public class HeroAI
{
    public void Update(Hero hero)
    {
        if (hero.Name == "战士")   { /* 冲锋 */ }        // 注释占位原样保留，不发明行为
        else if (hero.Name == "法师") { /* 放风筝 */ }
    }
}

public class HeroIcon
{
    public string GetIcon(Hero hero) => hero.icon;
}
```

对照铁律：路人回来了（40hp）、icon 原值（sword.png…）、AI 注释原样——**行为零变化**。加「刺客」= 表里加一行，Factory 零改动（`TryGetValue` 自动兜底路人）。三个系统只依赖表，霰弹枪变单发。

**动手题第三轮验收 ❌（2026-08-03）**：8 项差距——
1. 表只存 2 行，**路人消失**（原 default 路人 40hp）
2. **法师 Hp 60 → 80**（数值错）
3. icon 改存 `"WarriorIcon"`/`"MageIcon"`（原 sword.png 等，输出值变）
4. Factory 仍 switch，**表没被用起来**；`default: new Hero()` 兜底 → heroData 全空 + heroAI null
5. **直建/兜底 NRE 回归**：`HeroAI.Update` → `heroAI.HeroAIAction()` 空引用崩溃（第二轮 abstract 拦截被撤销）
6. AI 发明行为（WriteLine）第三遍
7. 表键英文 "Warrior"/"Mage" 与 switch 中文**双轨**
8. struct 挂静态 Dictionary + 可变字段（反模式）

**收尾要求**：不再发明新结构，按标准解逐行对照修改（6 条清单见对话记录）。

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（两轮 + 标准解）
2. 收敛成表——**三轮未过** → 标准解 + 8 项对照清单已给出，**按清单收官**
3. 骨架：[Homework/重构/HeroSystem.cs](Homework/重构/HeroSystem.cs)

---

`[进度：阶段三-重构 → Day 6「发散式变化+霰弹式修改」进行中]`
