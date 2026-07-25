# 命令模式（Command Pattern）

> 来源：《设计模式》GoF 第 5 章 + 《Head First 设计模式》第 6 章

---

## 一、书中定义

GoF 的定义：

> **"将一个请求封装为一个对象，从而使你可以用不同的请求对客户进行参数化；对请求排队或记录请求日志，以及支持可撤销的操作。"**

Head First 把它概括成一句话：**命令模式把「方法调用」封装成一个对象。**

---

## 二、坏代码场景

假设你在做一个格斗游戏的**技能输入系统**。玩家按键释放技能，每个技能的执行逻辑不同（伤害、特效、CD 检查）。最初的做法是用 if-else 绑定按键和技能：

```csharp
public class SkillInputHandler : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 火球术：先判断 CD，再扣蓝，再放技能
            if (player.MP < 30) return;
            if (fireballCD > 0) return;
            player.MP -= 30;
            fireballCD = 3.0f;
            // 播放火球动画、计算伤害……
            var targets = FindEnemiesInRange(player.transform, 5.0f);
            foreach (var t in targets)
                t.TakeDamage(50);
            animator.Play("Fireball");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            // 冰锥术
            if (player.MP < 20) return;
            if (icicleCD > 0) return;
            player.MP -= 20;
            icicleCD = 2.0f;
            var target = GetTargetedEnemy();
            target.TakeDamage(30);
            target.ApplySlow(1.5f);
            animator.Play("Icicle");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            // 治疗术
            if (player.MP < 40) return;
            if (healCD > 0) return;
            player.MP -= 40;
            healCD = 5.0f;
            player.HP += 60;
            animator.Play("Heal");
        }
        // 策划说：下个版本加「技能连击系统」——Q+W 放 combo 技
        // 还要加「撤销」——训练模式下可以回退到放技能前
        // 还要加「战斗回放」——录像模式下重放整场战斗操作
        // if-else 怎么搞？
    }
}
```

---

## 问题

1. 技能的逻辑（CD 检查、消耗 MP、造成伤害）和按键绑定硬编码在一起。如果策划说「手机版用虚拟摇杆搓招，PC 版用键盘」，你要改什么？

2. 「撤销」的本质是什么？——撤销 = 反向执行。如果不把「执行了哪个技能」记下来，你能撤销吗？

3. 「战斗回放」的本质是什么？——回放 = 按时间戳重新执行命令列表。if-else 里的逻辑，能被「记录」和「重放」吗？

---

## 你的回答（2026-07-24）

1. **输入和表现绑定，复杂度到了一定程度要解耦** ✅ —「把操作抽象为"打字"，对输入数据处理后再表现」——这就是命令模式的核心：把方法调用封装成对象。
2. **撤销 = 恢复到操作前的样子，通过规范数值影响接口保证能回退** ✅ — 直接关联到你之前 Buff 系统的属性管道设计。
3. **回放 = 记录时间戳 + 操作命令，重新计时释放副本** ✅ — 格斗游戏战斗回放的标准实现。

> 你在用命令模式的思路，现在给它正名。

---

## 三、标准重构：把请求封装成对象

### 3.1 核心思路

```
之前：按键 → 直接执行逻辑（if-else 里什么都干了）
之后：按键 → 创建命令对象 → 命令对象自己执行

                    ┌──────────────────┐
按键（调用者） ──→  │  Command 对象     │ ──→ Execute() → 技能逻辑
                    │  (请求的载体)      │ ──→ Undo()    → 反向操作
                    └──────────────────┘

命令对象 = 可传递、可排队、可记录、可撤销的「操作」
```

### 3.2 代码

```csharp
// ① 命令接口 —— 每个技能都是一个命令
public interface ICommand
{
    void Execute();
    void Undo();
}

// ② 具体命令 —— 火球术
public class FireballCommand : ICommand
{
    private Player _player;
    private int _mpCost = 30;
    private int _damage = 50;
    private List<Enemy> _hitTargets;  // 记录打中了谁，给 Undo 用

    public FireballCommand(Player player)
    {
        _player = player;
    }

    public void Execute()
    {
        // ① 前置检查
        if (_player.MP < _mpCost) return;

        // ② 扣资源
        _player.MP -= _mpCost;

        // ③ 执行逻辑
        _hitTargets = FindEnemiesInRange(_player.transform, 5.0f);
        foreach (var t in _hitTargets)
            t.TakeDamage(_damage);

        // ④ 表现（动画/音效/VFX —— 这些不参与逻辑，可以用观察者通知）
        _player.animator.Play("Fireball");
    }

    public void Undo()
    {
        // 反向执行：回血、回蓝
        _player.MP += _mpCost;
        foreach (var t in _hitTargets)
            t.TakeDamage(-_damage);  // 恢复伤害
    }
}

// ③ 冰锥术
public class IcicleCommand : ICommand
{
    private Player _player;
    private Enemy _target;
    private int _damage = 30;

    public IcicleCommand(Player player) => _player = player;

    public void Execute()
    {
        if (_player.MP < 20) return;
        _player.MP -= 20;
        _target = GetTargetedEnemy();
        _target.TakeDamage(_damage);
        _target.ApplySlow(1.5f);
    }

    public void Undo()
    {
        _player.MP += 20;
        _target?.TakeDamage(-_damage);
        _target?.RemoveSlow();  // 移除减速
    }
}

// ④ 调用者 —— SkillInputHandler 不再知道技能逻辑，只管按键映射
public class SkillInputHandler : MonoBehaviour
{
    private Player _player;
    private Dictionary<KeyCode, ICommand> _keyBindings = new();

    void Start()
    {
        // 按键 → 命令的映射：纯数据，逻辑在 Command 里
        _keyBindings[KeyCode.Q] = new FireballCommand(_player);
        _keyBindings[KeyCode.W] = new IcicleCommand(_player);
        _keyBindings[KeyCode.E] = new HealCommand(_player);
    }

    void Update()
    {
        foreach (var (key, command) in _keyBindings)
        {
            if (Input.GetKeyDown(key))
            {
                command.Execute();
            }
        }
    }

    // 改按键映射？只改字典，不改逻辑：
    public void RebindKey(KeyCode oldKey, KeyCode newKey)
    {
        if (_keyBindings.TryGetValue(oldKey, out var cmd))
        {
            _keyBindings.Remove(oldKey);
            _keyBindings[newKey] = cmd;
        }
    }
}
```

### 3.3 结果

| 场景 | 之前 | 之后 |
|------|------|------|
| 改按键映射 | 改 if-else 里每个分支 | 改字典一条映射 |
| 加新技能 | 加一个 else if 分支 | 新建一个 Command 类 + 加一行映射 |
| PC / 手机双输入 | 两套 if-else | 两个 InputHandler 共享同一套 Command |
| 训练模式撤销 | 做不了 | `_undoStack.Pop().Undo()` |

---

## 四、三个进阶玩法

### 4.1 撤销栈（Undo Stack）

```csharp
public class CommandHistory
{
    private Stack<ICommand> _undoStack = new();
    private Stack<ICommand> _redoStack = new();

    public void ExecuteCommand(ICommand cmd)
    {
        cmd.Execute();
        _undoStack.Push(cmd);
        _redoStack.Clear();  // 新操作后，重做栈清空
    }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;
        var cmd = _undoStack.Pop();
        cmd.Undo();
        _redoStack.Push(cmd);
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        var cmd = _redoStack.Pop();
        cmd.Execute();
        _undoStack.Push(cmd);
    }
}
```

```csharp
// 训练模式使用
var history = new CommandHistory();
history.ExecuteCommand(new FireballCommand(player));  // 放火球
history.ExecuteCommand(new IcicleCommand(player));    // 放冰锥
history.Undo();  // ← 撤销冰锥
history.Undo();  // ← 撤销火球
history.Redo();  // ← 重做火球
```

### 4.2 战斗回放（Replay）

你回答里说的「记录时间戳 + 操作命令」就是这个：

```csharp
// 记录帧
public class ReplayRecorder
{
    private List<(float timestamp, ICommand command)> _log = new();
    private float _startTime;

    public void StartRecord()
    {
        _log.Clear();
        _startTime = Time.time;
    }

    public void RecordCommand(ICommand cmd)
    {
        _log.Add((Time.time - _startTime, cmd));
    }

    public List<(float timestamp, ICommand command)> GetLog() => _log;
}

// 回放：按时间戳重放命令
public IEnumerator Replay(ReplayRecorder log)
{
    foreach (var (timestamp, command) in log.GetLog())
    {
        yield return new WaitForSeconds(timestamp - Time.time);  // 对齐时间
        command.Execute();
    }
}
```

### 4.3 命令队列（Command Queue）—— 搓招系统

你说的「打字」——格斗游戏搓招本质就是命令队列：

```csharp
public class ComboInputHandler
{
    private Queue<ICommand> _inputBuffer = new();  // 输入缓冲
    private float _bufferWindow = 0.2f;  // 200ms 窗口内的按键算 combo

    public void OnKey(KeyCode key)
    {
        var cmd = KeyToCommand(key);
        _inputBuffer.Enqueue(cmd);
        // 200ms 后消费队列，匹配 combo 模式
        // 如 [↓↘→, 拳] → 波动拳，而不是单独的 ↓, ↘, →, 拳
    }
}
```

---

## 五、和前几个模式的对比

| 模式 | 封装了什么 | 能干什么 |
|------|-----------|----------|
| 策略 | 算法 | 运行时选一种算法 |
| 装饰器 | 额外行为 | 动态叠加功能 |
| 观察者 | 通知关系 | 一对多广播 |
| **命令** | **请求/操作本身** | **排队、撤销、重放、日志** |

命令模式是五种里唯一把「动作」提升为一等公民的——策略替换的是"怎么做"，命令记录的是"做了什么"。

---

## 六、跨书关联

| 关联概念 | 来源 |
|----------|------|
| OCP —— 加新技能不改已有代码 | 《敏捷》第 9 章 |
| 观察者模式 —— 命令执行后通知 UI/VFX | 《Head First》第 2 章 |
| 备忘录模式（Memento）—— 撤销的另一种实现，保存快照而非反向执行 | GoF 第 5 章 |
| 命令模式 + 责任链 —— 命令经过多个处理器（权限检查 → CD 检查 → 执行 → 记日志） | GoF 第 5 章 |

---

## 七、作业（预计 15 分钟）

做一个简易的**移动命令系统**。角色在网格上移动，要求支持撤销和重放：

```csharp
// 框架
public interface IMoveCommand
{
    void Execute();       // 执行移动
    void Undo();          // 撤销（回到原位）
    Vector2Int GetFrom(); // 起点（回放用）
    Vector2Int GetTo();   // 终点
}

public class MoveCommand : IMoveCommand
{
    private Character _char;
    private Vector2Int _from, _to;

    public MoveCommand(Character c, Vector2Int from, Vector2Int to)
    {
        _char = c; _from = from; _to = to;
    }

    public void Execute()
    {
        _char.MoveTo(_to);
    }

    public void Undo()
    {
        _char.MoveTo(_from);
    }

    public Vector2Int GetFrom() => _from;
    public Vector2Int GetTo() => _to;
}
```

要求：
1. 实现 `CommandHistory`（Undo + Redo 双栈）
2. 写一段测试流程：角色 A 从 (0,0) 移动到 (1,0)，再移动到 (1,1)，然后 Undo 一次，断言角色回到 (1,0)
3. 思考：移动命令的 Undo 是存起点坐标来回退。那**对敌人造成伤害的命令**，要 Undo 需要存什么？

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 → 核心讲解完成，等待作业 ✓]`
