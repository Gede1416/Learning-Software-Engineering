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

1. 需求变换要同步更新多个逻辑分支（AI / Icon / HeroFactory）
2. 解决方法：HeroFactory 创建 Hero 的时候传入 AI、icon 获取接口
3. 霰弹式修改

## 四、纠错（第一轮，2026-08-03）

- 第 1、3 点 ✅：识别出「加英雄 → AI/Icon/Factory 三个文件一起改」= **霰弹式修改**（一个原因，N 处修改）。
- 第 2 点 ❌ 抢跑 + 工具不对：传接口进去是「换」不是「拆」（铁律：只拆不换），且这轮先诊断，解法轮不到。
- 缺：**发散式变化**没答——另一个方向：一个类被多种不同需求轮流改（血量公式/复活规则/初始装备都改 Hero）。两味道都是 SRP 违规但尺度相反：发散式变化 = 一个类多种变化原因 → 该**拆**（提炼类）；霰弹式修改 = 一个原因散落多处 → 该**合**（搬移收敛）。

**待答子问题**：策划改「血量成长公式」→ 只改 Hero；改「复活规则」→ 也只改 Hero；改「初始装备」→ 还是改 Hero。一个类被多种不同需求轮流改——这个方向叫什么？和霰弹式修改的区别在哪（一个怎么拆、一个怎么合）？

## 五、标准解（待给出）

（子问题回答正确后给出）

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题已答，待按子问题修正
2. 用**搬移函数/搬移语句**把散落三处的「英雄类型 → 数据/行为」收敛成一张表（Dictionary），三个系统都查表——**铁律：只拆不换**（数值、输出、行为一律不变）——**未做**
3. 骨架：[Homework/重构/HeroSystem.cs](Homework/重构/HeroSystem.cs)

---

`[进度：阶段三-重构 → Day 6「发散式变化+霰弹式修改」进行中]`
