# 过长参数列表（Long Parameter List）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 4 位

---

## 一、坏代码场景

玩家释放技能——一个方法 8 个参数：

```csharp
public class SkillSystem
{
    // 玩家释放技能：8 个参数
    public void CastSkill(int skillId, int level, int x, int y,
                          int targetId, float angle, int power, string source)
    {
        if (!SkillUnlocked(skillId, level, source)) return;
        if (targetId < 0) return;
        Enemy target = FindEnemy(targetId);
        if (target == null) return;
        int dmg = CalcDamage(skillId, level, power);
        target.TakeDamage(dmg);
        FxManager.Play("skill_" + skillId, x, y, angle);
        SoundManager.Play("cast_" + skillId);
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：调用方每次要带几样东西？参数顺序传错会不会编译报错？策划要加「暴击率」参数要动几个地方？这 8 个参数里，哪几伙天生是一伙的？）

## 三、你的回答（2026-08-03）

1. int skillId, int level, int x, int y, int targetId, float angle, int power, string source——结构散乱，理解困难；增加新参数还要修改原本的参数列表；后续维护困难
2. 修改参数列表还要连带改 CalcDamage 参数
3. 整合：skillData { skillId, level, source } / castData { angle, x, y, power } / targetData { targetId }

## 四、纠错（第一轮，2026-08-03）

- 第 1 点：又是共性症状（Day 3 的坑重蹈覆辙）。「过长参数列表」的特异诊断是：**调用方无法凭类型判断对错**——8 个参数里 6 个是 int/float，顺序传错编译器一声不吭，运行时才崩；以及**一伙数据被拆散**（cohesive data torn apart）。
- 第 2 点 ✅：看到 CalcDamage 连坐——这正是「加参数 = 霰弹枪修改」的传播路径：加一个参数要动所有调用点 + 链上所有内部函数。
- 第 3 点：方向对（这就是「引入参数对象」），但分组标准应该是「同生共死」——power 和 x/y/angle 会一起变吗？source 和 skillId 会一起变吗？

**待答子问题**：调用方（AI 脚本）把参数顺序记错——`CastSkill(5, 2, 3, 4, 1, 45f, 100, "AI")` 写成 `CastSkill(2, 5, 4, 3, 1, 45f, 100, "AI")`——编译器会不会拦？运行时会在哪里、以什么方式暴露？

## 五、标准解（待给出）

（子问题回答正确后给出）

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题已答，待按子问题修正
2. 把 `CastSkill` 的参数用**引入参数对象（Introduce Parameter Object）**收拢——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——**未做**
3. 骨架：[Homework/CastSkill.cs](Homework/CastSkill.cs)

---

`[进度：阶段三-重构 → Day 4「过长参数列表」进行中]`
