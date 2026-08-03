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

### 第二轮（子问题）

编译器拦住，运行时在传入数据为 null 时暴露

## 四、纠错（第一轮，2026-08-03）

- 第 1 点：又是共性症状（Day 3 的坑重蹈覆辙）。「过长参数列表」的特异诊断是：**调用方无法凭类型判断对错**——8 个参数里 6 个是 int/float，顺序传错编译器一声不吭，运行时才崩；以及**一伙数据被拆散**（cohesive data torn apart）。
- 第 2 点 ✅：看到 CalcDamage 连坐——这正是「加参数 = 霰弹枪修改」的传播路径：加一个参数要动所有调用点 + 链上所有内部函数。
- 第 3 点：方向对（这就是「引入参数对象」），但分组标准应该是「同生共死」——power 和 x/y/angle 会一起变吗？source 和 skillId 会一起变吗？

**子问题判定（第二轮，2026-08-03）**：两个判断都错——①「编译器拦住」❌：6 个 int/float 参数类型全合法，编译器无分辨能力；②「null 时暴露」❌：交换的是 skillId/level/x/y，targetId 没变，null 检查不触发。

## 五、标准解（2026-08-03 给出）

Fowler《重构》第 3 章「过长参数列表」：参数列表越长，调用方越容易传错，编译器无法分辨；第 6 章「引入参数对象（Introduce Parameter Object）」：一伙同生共死的数据应该收成一个对象。

子问题走查：`CastSkill(2, 5, 4, 3, 1, 45f, 100, "AI")` 完全合法——skillId=2（放错技能）、level=5（等级错）、x=4/y=3（特效打偏）；targetId 没变 → null 不触发，无异常无日志，**静默做错事**。玩家看到打偏/放错技能，Bug 单写「技能表现诡异」，查半天找不到调用点。

参数对象的本质（两个杀手锏）：

1. **编译期拦截**：`CastSkill(SkillData, LookData, TargetData, OwnerData)` 类型各异——传错对象，编译器直接报 CS1503。把「上线后被玩家发现的错」提前成「编译时被发现的错」。
2. **属性名代替参数顺序**：`new SkillData { skillId = 5, level = 2 }` 自带意图，不用记顺序。

跨书联动：参数对象是**值对象（Value Object）**的雏形（《敏捷》数据封装、《整洁之道》函数参数）。

## 六、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（两轮纠错 + 标准解）
2. 把 `CastSkill` 的参数用**引入参数对象（Introduce Parameter Object）**收拢——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）——✅ **验收通过**
3. 骨架：[Homework/CastSkill.cs](Homework/CastSkill.cs)

## 七、验收记录（第一轮，2026-08-03）

- 编译 0 错误；数值（CalcDamage 公式）、分支条件、调用顺序全部保持 ✓
- 分组修正到位：`power` → `OwnerData`（伤害数据），`x/y/angle` → `LookData`（表现数据），`skillId/level` → `SkillData`，`targetId` → `TargetData`——符合「同生共死」标准 ✓
- 待打磨：文件头 TODO 残留；`OwnerData.Id` 未使用（数据占位）
- 概念子问题已答（判定：两个判断都错）→ 标准解已给出 → **Day 4 收官 ✅（2026-08-03）**

---

`[进度：阶段三-重构 → Day 4「过长参数列表」进行中]`
