# 单例模式（Singleton Pattern）

> 来源：《设计模式》GoF 第 3 章 + 《Head First 设计模式》第 5 章

---

## 一、书中定义

GoF 的定义：

> **"保证一个类仅有一个实例，并提供一个访问它的全局访问点。"**

Head First 补了一句关键的话：**"单例是设计模式里最简单的一个，但也是被滥用最多的一个。"**

---

## 二、坏代码场景

假设你在做一个 Roguelike 游戏的**存档管理系统**。存档数据（当前关卡、金币数、已解锁角色）在游戏各处都要读写——战斗胜利了要存、商店购买要存、角色死亡要写墓碑数据。

初级做法：把存档数据塞给一个普通类，哪里需要就在哪里 new：

```csharp
public class SaveData
{
    public int Gold;
    public int CurrentFloor;
    public List<string> UnlockedHeroes;

    public void Save() { /* 序列化写入磁盘 */ }
    public void Load() { /* 从磁盘读取 */ }
}

// 战斗系统里
var save1 = new SaveData();
save1.Load();
save1.Gold += 100;
save1.Save();

// 商店系统里
var save2 = new SaveData();  // ← 又 new 了一个
save2.Load();                // ← 又读了一次磁盘
save2.Gold -= 50;            // ← 操作的是副本，save1 里的 +100 没有被感知到
save2.Save();                // ← 覆盖了 save1 的修改 → 100 金币丢了
```

---

## 问题

1. 上面的代码，玩家的 100 金币为什么丢了？问题出在第几行？

2. 如果你用 `static` 类来解决——所有字段和方法都 static——能避免「多实例」问题，但会引入什么新问题？想想看：static 类能实现接口吗？能 mock 做单元测试吗？能懒加载（第一次用到时才初始化）吗？

3. 在 Unity 里，`MonoBehaviour` 挂载在 GameObject 上。如果你把 `SaveData` 做成 `MonoBehaviour` 单例，场景切换时 GameObject 被销毁了怎么办？DontDestroyOnLoad 有什么坑？

---

## 你的回答（2026-07-24）

1. **new 了两个不同对象，数据不同步** ✅ —— `save1` 和 `save2` 各管各的状态，`save2.Save()` 覆盖了 `save1` 的修改。
2. **static 类不能实现接口** ✅ —— 你抓到要害了。Mock 和测试方面需要展开。
3. **丢失单例载体** ✅ —— 场景切换后 GameObject 销毁，引用变成 null。具体坑下面展开。

---

## 三、标准实现：C# 纯代码单例

### 3.1 最简版（非线程安全，适合单线程游戏逻辑）

```csharp
public class SaveData
{
    // ① 私有静态字段 —— 唯一的实例
    private static SaveData _instance;

    // ② 私有构造函数 —— 堵死外部 new 的入口
    private SaveData() { }

    // ③ 公有静态访问点 —— 全局唯一入口
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SaveData();
            return _instance;
        }
    }

    // ====== 业务方法 ======
    public int Gold { get; set; }
    public int CurrentFloor { get; set; }

    public void Save() { /* 序列化到磁盘 */ }
    public void Load() { /* 从磁盘反序列化 */ }
}

// 使用方 —— 不管在哪，拿到的都是同一个对象
// 战斗系统：
SaveData.Instance.Load();
SaveData.Instance.Gold += 100;
SaveData.Instance.Save();

// 商店系统：
SaveData.Instance.Load();
SaveData.Instance.Gold -= 50;   // ← 减的是上面 +100 后的结果
SaveData.Instance.Save();       // ← 写回的是 150 - 50 = 100，不会丢
```

### 3.2 线程安全版（如果用了异步加载 / 多线程）

```csharp
public class SaveData
{
    private static SaveData _instance;
    private static readonly object _lock = new object();

    private SaveData() { }

    public static SaveData Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new SaveData();
            }
            return _instance;
        }
    }
}
```

或直接用 C# 的静态构造器（天然线程安全，且是懒加载）：

```csharp
public class SaveData
{
    private static readonly Lazy<SaveData> _instance =
        new Lazy<SaveData>(() => new SaveData());

    private SaveData() { }

    public static SaveData Instance => _instance.Value;
}
```

---

## 四、Static 类 vs 单例 —— 你第二问的答案

| | static 类 | 单例（对象） |
|------|-----------|-------------|
| 实现接口 | ❌ 不能 | ✅ 可以 `SaveData : ISaveSystem` |
| 继承 | ❌ 不能 | ✅ 可以被子类扩展 |
| 多态 | ❌ 不支持 | ✅ `ISaveSystem s = SaveData.Instance` |
| 单元测试 Mock | ❌ 不能 mock static 方法 | ✅ mock 接口 / 虚方法 |
| 懒加载 | 首次访问类时自动初始化，时机不可控 | `Lazy<T>` 或手动延迟，时机可控 |
| 销毁重建 | ❌ 无法主动释放 | ✅ 可以 `_instance = null` 后重建（如重新开档） |
| 传参构造 | ❌ 不行 | ✅ `new SaveData(config)` |

**static 类是死的，单例是活的对象。** 能实现接口、能被 mock、能延迟初始化——这三条已经足够判 static 出局了。

---

## 五、Unity MonoBehaviour 单例 —— 你第三问的答案

### 5.1 核心实现

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // ① 重复检查
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 场景重复加载时，销毁重复的
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### 5.2 你问的坑

**坑 1：场景切换时 GameObject 被销毁** → `DontDestroyOnLoad` 解决，但别忘了。

**坑 2：另一个场景里已经挂了一个 GameManager**

```
Scene_A 加载 → Awake → Instance = GameManager_A
Scene_B 加载 → Awake → Instance = GameManager_B（覆盖了！）
                     → Destroy(GameManager_A) → 引用了 GameManager_A 的组件全炸
```

解决：上面代码里的 `Instance != null && Instance != this → Destroy(gameObject)`。

**坑 3：`OnDestroy` 后 Instance 指向 null**

```csharp
// 如果游戏退出的清理流程里有：
Destroy(GameManager.Instance.gameObject);
// → Instance（static）仍然指向已销毁的对象，但不是 null
// → 后续调用 GameManager.Instance.SomeMethod() → MissingReferenceException

// 解决：OnDestroy 里清空 static 引用
private void OnDestroy()
{
    if (Instance == this)
        Instance = null;
}
```

**坑 4：执行顺序** —— 其他脚本的 `Awake` 可能跑在 GameManager 之前，此时 `Instance` 还是 null。解决：放在 `Script Execution Order` 最早，或用 `[RuntimeInitializeOnLoadMethod]`。

**坑 5：直接挂场景里，编辑时不方便** → 很多人用 `[RuntimeInitializeOnLoadMethod]` 自动生成 GameObject，完全脱离场景依赖。

---

## 六、单例被滥用的重灾区

Head First 专门警告了一章——单例是 GoF 23 个模式里**最容易被滥用**的，因为它太简单了。

| 滥用信号 | 你的代码 | 应该 |
|----------|----------|------|
| `XXXManager.Instance` 满天飞 | 30 个类直接调 `GameManager.Instance` | 依赖注入，只让真正需要的类拿到引用 |
| 单例之间互相调用 | `AudioManager.Instance.Play(GameManager.Instance.CurrentBGM)` | 用观察者模式解耦 |
| 测试跑不起来 | 单例持有全局状态，测试之间互相污染 | 提供 Reset 方法，或改用接口注入 |
| 单例做了太多事 | GameManager 同时管存档、音效、UI、网络…… | 拆成多个职责单一的单例 |

> **单例解决了「全局唯一」，但制造了「全局耦合」。**

---

## 七、什么时候确实该用单例？

- 硬件/引擎级资源：音频设备、渲染管线、文件系统入口
- 全局配置：分辨率设定、存档路径
- 对象池：子弹池、特效池（整个游戏只该有一份）
- 日志系统：一个 `Logger.Instance.Log(...)`

**核心标准：如果「两个实例」会导致系统级 bug（不是逻辑 bug，是引擎/硬件/数据损坏），才用单例。** 存档数据其实不一定需要单例——如果以后要做多存档槽位，单例反而碍事。

---

## 八、跨书关联

| 关联概念 | 来源 |
|----------|------|
| SRP —— 单例只应管控「全局唯一」本身，不绑定业务逻辑 | 《敏捷》第 8 章 |
| DIP —— 依赖 `ISaveSystem` 接口而非 `SaveData.Instance` 具体类 | 《敏捷》第 11 章 |
| 工厂模式常配合单例 —— 工厂本身往往是单例 | GoF 第 3 章 |
| 观察者模式替代方案 —— 不要单例互相调，用事件总线 | 《Head First》第 2 章 |

---

## 九、作业（预计 10 分钟）

你的游戏里有一个**音效管理器 AudioManager**。要求：

1. 用 C# 纯代码实现线程安全的单例 `AudioManager`
2. 它有一个 `PlaySFX(string clipName)` 方法和一个 `SetMasterVolume(float v)` 方法
3. 思考：如果以后要做「不同场景有不同混响参数」（地下城有回声、野外没有），单实例的 `AudioManager` 怎么支持？——提示：是否需要把 AudioManager 拆成「全局管理器单例 + 场景级混响配置」？

```csharp
// 框架：
public class AudioManager
{
    // 你来写单例骨架

    public void PlaySFX(string clipName) { /* ... */ }
    public void SetMasterVolume(float v) { /* ... */ }
}
```

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 → 核心讲解完成，等待作业 ✓]`
