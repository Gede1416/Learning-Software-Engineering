# 状态模式（State Pattern）

> 来源：《设计模式》GoF 第 5 章 + 《Head First 设计模式》第 10 章

---

## 一、书中定义

GoF 的定义：

> **"允许一个对象在其内部状态改变时改变它的行为。对象看起来似乎修改了它的类。"**

Head First 用了一个经典例子：**自动售货机**。投币、退币、出货、售罄——每种状态下，同一个操作（如「按下出货按钮」）的行为完全不同。

---

## 二、坏代码场景

假设你在做一个 ARPG 的 **Boss AI 系统**。Boss 有三种行为状态：

- 😴 **待机（Idle）**：站着不动，玩家靠近到一定范围切换到巡逻
- 🚶 **巡逻（Patrol）**：在固定路线上来回走，发现玩家切换到追击
- ⚔️ **追击（Chase）**：追向玩家，玩家逃出视野范围切回巡逻，追到攻击范围切攻击
- 💢 **攻击（Attack）**：释放技能，技能放完切回追击

当前代码：

```csharp
public enum BossState { Idle, Patrol, Chase, Attack }

public class BossAI : MonoBehaviour
{
    public BossState State = BossState.Idle;

    void Update()
    {
        switch (State)
        {
            case BossState.Idle:
                if (DistanceToPlayer() < 10f)
                    State = BossState.Patrol;
                break;

            case BossState.Patrol:
                MoveAlongPath();
                if (CanSeePlayer())
                {
                    State = BossState.Chase;
                    PlaySound("Alert!");  // 发现玩家音效
                }
                break;

            case BossState.Chase:
                MoveTowards(Player.Position);
                if (DistanceToPlayer() > 20f)
                    State = BossState.Patrol;  // 玩家跑了
                else if (DistanceToPlayer() < 3f)
                    State = BossState.Attack;  // 进入攻击范围
                break;

            case BossState.Attack:
                if (AttackCooldown <= 0)
                {
                    PlayAttackAnimation();
                    Player.TakeDamage(50);
                    AttackCooldown = 2.0f;
                }
                if (DistanceToPlayer() > 3f)
                    State = BossState.Chase;  // 玩家后撤了
                break;
        }
    }

    // 策划：Boss 血低于 30% 时进入「狂暴状态」——攻速翻倍、无视距离追击
    // 策划：再加一个「召唤小兵」状态——每 15 秒召唤一次
    // 这个 switch —— 还看得下去吗？
}
```

---

## 问题

1. 这个 switch 和策略模式那节课的 if-else 长得一模一样。但为什么**策略模式救不了这个场景**？提示：策略是外部选择算法，状态是自己切换状态。

2. 「狂暴状态」的加入会怎么破坏这个 switch？加一个 enum 值、加一个 case——但「低于 30% 血量进入狂暴」这个条件写在哪个 case 里？写了之后还要改哪些 case？

3. 状态模式在外观上和策略模式几乎一样（都是一个接口 + 多个实现类）。区别在哪？——想想：谁会调用 `SetState`？是外部还是状态类自己？

---

## 你的回答（2026-07-24）

1. **策略不依赖主体，状态依赖主体** ✅ — 「策略是复用算法，状态是管理内部变化」——精准。
2. **每个判断都要加 case，还要改已有 case** ✅ — 狂暴状态的「低于 30% 血量」条件要写进每一个 case，这是**横切关注点**的散弹枪手术。
3. **区别在于是否互相依赖，状态自己调用切换** ✅ — 这就是策略和状态的核心分界线。

---

## 三、标准重构：让状态自己管理自己

### 3.1 核心思路

```
之前：BossAI.Update() 里一个巨大的 switch
      → BossAI 需要知道「每个状态下做什么 + 什么时候切换」
      → BossAI 承担了所有状态的逻辑 = SRP 灾难

之后：每个状态是一个类，自己管自己的行为和切换条件
      → BossAI 只需要知道「当前状态是谁」，把 Update 委托给它
```

```
┌─────────────────────────────────────────────────┐
│                    BossAI (Context)              │
│  当前状态: IBossState                            │
│  Update() → _currentState.Update(this)          │
└──────────────┬──────────────────────────────────┘
               │ 持有
               ▼
       ┌──────────────┐
       │  IBossState  │ ← 状态接口
       │  Enter(ctx)  │    进入状态时初始化
       │  Update(ctx) │    每帧逻辑 + 切换判断
       │  Exit(ctx)   │    离开状态时清理
       └──────┬───────┘
              △
   ┌──────────┼──────────┬──────────┐
   │          │          │          │
 IdleState  PatrolState ChaseState AttackState
 (自己判断 (自己判断 (自己判断 (自己判断
  何时切)   何时切)   何时切)  何时切)
```

### 3.2 代码

```csharp
// ① 状态接口 —— 每个状态自己管理自己的行为和切换
public interface IBossState
{
    void Enter(BossAI boss);   // 进入状态（初始化计时器等）
    void Update(BossAI boss);  // 每帧逻辑 + 切换判断
    void Exit(BossAI boss);    // 离开状态（清理）
}

// ② 具体状态 —— 待机
public class IdleState : IBossState
{
    public void Enter(BossAI boss) { }

    public void Update(BossAI boss)
    {
        // 待机行为：站着不动
        // 切换条件：玩家靠近
        if (boss.DistanceToPlayer() < 10f)
            boss.SetState(new PatrolState());
    }

    public void Exit(BossAI boss) { }
}

// ③ 具体状态 —— 巡逻
public class PatrolState : IBossState
{
    private int _waypointIndex;

    public void Enter(BossAI boss)
    {
        _waypointIndex = 0;
    }

    public void Update(BossAI boss)
    {
        boss.MoveAlongPath(_waypointIndex);

        // 切换条件
        if (boss.DistanceToPlayer() < 3f)
            boss.SetState(new AttackState());
        else if (boss.CanSeePlayer())
        {
            boss.PlaySound("Alert!");
            boss.SetState(new ChaseState());
        }
    }

    public void Exit(BossAI boss) { }
}

// ④ 具体状态 —— 追击
public class ChaseState : IBossState
{
    public void Enter(BossAI boss) { }

    public void Update(BossAI boss)
    {
        boss.MoveTowards(boss.PlayerPosition);

        if (boss.DistanceToPlayer() > 20f)
            boss.SetState(new PatrolState());
        else if (boss.DistanceToPlayer() < 3f)
            boss.SetState(new AttackState());
    }

    public void Exit(BossAI boss) { }
}

// ⑤ 具体状态 —— 攻击
public class AttackState : IBossState
{
    private float _timer;

    public void Enter(BossAI boss)
    {
        _timer = 0;
    }

    public void Update(BossAI boss)
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            boss.PlayAttackAnimation();
            boss.Player.TakeDamage(50);
            _timer = 2.0f;
        }

        if (boss.DistanceToPlayer() > 3f)
            boss.SetState(new ChaseState());
    }

    public void Exit(BossAI boss) { }
}

// ⑥ Context —— BossAI 变成薄薄一层，只管委托
public class BossAI : MonoBehaviour
{
    private IBossState _currentState;

    void Start()
    {
        _currentState = new IdleState();
        _currentState.Enter(this);
    }

    void Update()
    {
        _currentState.Update(this);  // ← 全部委托给当前状态
    }

    public void SetState(IBossState newState)
    {
        _currentState.Exit(this);    // 旧状态清理
        _currentState = newState;
        _currentState.Enter(this);   // 新状态初始化
    }

    // 这些方法供各个 State 调用 —— BossAI 是数据 + 能力的提供者
    public float DistanceToPlayer() => Vector3.Distance(transform.position, Player.transform.position);
    public bool CanSeePlayer() { /* 视线检测 */ return true; }
    public void MoveAlongPath(int index) { /* ... */ }
    public void MoveTowards(Vector3 target) { /* ... */ }
    public void PlaySound(string clip) { /* ... */ }
    public void PlayAttackAnimation() { /* ... */ }
    public Vector3 PlayerPosition => Player.transform.position;
    public Player Player => Player.Instance;
}
```

### 3.3 加「狂暴状态」——现在有多简单？

```csharp
public class BerserkState : IBossState
{
    public void Enter(BossAI boss)
    {
        boss.PlaySound("Berserk!");
        boss.SetAttackSpeedMultiplier(2.0f);  // 攻速翻倍
    }

    public void Update(BossAI boss)
    {
        // 无视距离，直接追
        boss.MoveTowards(boss.PlayerPosition);

        // 进攻击范围就砍
        if (boss.DistanceToPlayer() < 3f)
        {
            boss.PlayAttackAnimation();
            boss.Player.TakeDamage(50);
        }

        // 不需要切回 Patrol —— 狂暴状态下不死不休
    }

    public void Exit(BossAI boss)
    {
        boss.SetAttackSpeedMultiplier(1.0f);  // 恢复
    }
}

// 在 AttackState（或其他任意状态）的 Update 里加一行：
if (boss.HP < boss.MaxHP * 0.3f)
{
    boss.SetState(new BerserkState());
    return;
}
```

| 对比 | switch 方案 | 状态模式 |
|------|------------|---------|
| 加狂暴状态 | 改 enum + 加 case + **改所有已有 case** | 新建 1 个类 + 在关心的状态里加 3 行 |
| 已有代码 | 每一个 case 都要加 `if (HP < 30%) → go Berserk` | 只改需要切狂暴的状态 |
| 每个状态行数 | BossAI.Update() = 100+ 行 | 每个状态文件 < 30 行 |

---

## 四、状态模式 vs 策略模式 —— 你的核心洞察

你回答的第三条「状态内部自己调用切换」就是关键。展开对比：

| | 策略模式 | 状态模式 |
|------|----------|----------|
| **谁决定切换** | 外部（Context 或调用方） | 内部（状态类自己） |
| **接口依赖** | 策略不依赖 Context | 状态持有 Context 引用（`Update(BossAI boss)`） |
| **知道彼此吗** | 各策略互不知对方存在 | 状态知道「可以切换到哪些状态」 |
| **变化来源** | 「选择什么算法」 | 「当前在什么状态」 |
| **类图** | 几乎一样 | 几乎一样 |
| **意图** | 封装**可替换的算法** | 封装**状态相关的行为** |

> **一句话：策略是「外界让你用什么」，状态是「你自己现在是什么」。**

---

## 五、有限状态机（FSM）—— 状态模式的升级版

你如果做过 Unity 的 Animator，它的状态机就是状态模式 + 可视化：

```
Animator 的状态节点 = 各个 IBossState 实现类
Animator 的 Transition = 状态类内部的 SetState() 调用
Animator 的参数 (bool/float) = BossAI 的 HP/Distance 等数据
```

当状态数量 > 5 且切换逻辑复杂时，手写状态模式会变成蜘蛛网。此时有两种升级路径：
- **状态机框架**（自研或用现成的 FSM 库）：状态 + 转换条件声明式配置
- **行为树（Behavior Tree）**：AI 专用，比 FSM 更适合「条件 → 动作」的非线性 AI

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| 策略模式 vs 状态模式 —— 结构相同，意图不同 | GoF 第 5 章 |
| SRP —— 每个状态一个类，BossAI 不再管所有状态的细节 | 《敏捷》第 8 章 |
| OCP —— 加新状态不改已有状态类 | 《敏捷》第 9 章 |
| Replace Type Code with State/Strategy | 《重构》第 8 章 |

---

## 七、作业（预计 15 分钟）

做一个**游戏主菜单状态机**。状态有四个：

```
MainMenu  → （点击"开始游戏"）→ Playing
Playing   → （按 ESC）→ Paused
Paused    → （按 ESC）→ Playing
Paused    → （点击"退出"）→ MainMenu
Playing   → （角色死亡）→ GameOver
GameOver  → （点击"返回主菜单"）→ MainMenu
```

框架：

```csharp
public interface IGameState
{
    void Enter(GameManager gm);
    void Update(GameManager gm);
    void Exit(GameManager gm);
}

public class GameManager
{
    private IGameState _currentState;
    public void SetState(IGameState s) { /* ... */ }
    void Update() => _currentState.Update(this);
}
```

要求：
1. 实现四个状态类，每个状态在 `Enter` 时打印 `"进入 XXX 状态"`
2. 实现 `GameManager.SetState()`（Enter/Exit 切换）
3. 在 `Program.cs` 里模拟流程：MainMenu → Playing → Paused → Playing → GameOver → MainMenu

思考：状态模式的 `Update(BossAI boss)` 参数里传 Context，会不会让状态和 Context 耦合过紧？有没有更好的办法？（提示：接口隔离）

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 → 核心讲解完成，等待作业 ✓]`
