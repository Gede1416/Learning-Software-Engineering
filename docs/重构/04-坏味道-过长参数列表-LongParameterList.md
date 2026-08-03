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

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 把 `CastSkill` 的参数用**引入参数对象（Introduce Parameter Object）**收拢——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/CastSkill.cs](Homework/CastSkill.cs)

---

`[进度：阶段三-重构 → Day 4「过长参数列表」进行中]`
