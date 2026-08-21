# 边界（Boundaries）

> 来源：《代码整洁之道》Robert C. Martin 第 8 章
> 跨书联动：设计模式-适配器模式；Day 5 对象与数据结构（封装）；Day 6 异常包装（翻译边界）

---

## 一、坏代码场景

第三方激励视频广告 SDK 又怪又丑，且供应商定死不能改。游戏各处要「看广告 → 发金币」，于是业务代码直接摸 SDK：

```csharp
// 供应商的 SDK：接口又怪又丑，且不能改
public class RewardedAdSdk
{
    public void SDK_Init(string appId) { }
    public int Get_State() { return 2; }                       // 0=未加载 1=加载中 2=就绪
    public void SDK_Show() { }
    public void Set_Listener(Action<int> onEvent) { }          // 0=展示 1=奖励 2=失败
}

// 游戏业务代码到处直接摸 SDK
public class AdManager
{
    private readonly RewardedAdSdk _sdk;

    public void PlayerClickedWatchAd()
    {
        if (_sdk.Get_State() == 2) GiveCoins(100);             // 魔法数字 2 = 就绪
        _sdk.SDK_Show();
    }
}
```

## 二、问题

**这段代码有什么问题？当需求变化时，具体会在哪里崩盘？**

（Hint：想想「就绪」「奖励」这两个概念，现在是谁在定义它们？）

## 三、你的回答

### 第一轮（2026-08-20）

> 由于是外部提供，外部更改的话，后续直接调用的位置都要修改。

方向正确：识别到了第三方变化会沿着直接依赖扩散到游戏业务代码。

### 第二轮（2026-08-20）

> `2` 表示玩家应该得到奖励；应该由 `Get_State() == 2` 决定；调用获取奖励的代码会受影响。

## 四、纠错记录

### 第一轮

回答还停留在“外部改了，调用方都要改”的泛化层面，没有落到题目中两个具体业务概念。请只盯住坏代码第 29 行：

1. 数字 `2` 在这里表示“广告已就绪”还是“玩家应得奖励”？
2. 如果玩家打开广告后立刻关闭，金币应由 `Get_State() == 2` 决定，还是由某个回调事件决定？
3. 当供应商把状态码或奖励事件码改掉时，哪些游戏代码被迫理解这些数字？

### 第二轮

未通过。把 SDK 的两套数字协议混在了一起：`Get_State() == 2` 只表示广告已经加载完、可以展示；它不证明玩家已经看完广告。玩家是否获得奖励应由 `Set_Listener` 收到的奖励事件 `1` 决定。

## 五、标准解

> “Software at the boundaries needs clear separation and tests that define expectations.”
>
> ——Robert C. Martin，《代码整洁之道》第 8 章

边界的核心不是“给第三方 API 换个好听的名字”，而是把第三方定义的语言翻译成游戏自己定义的契约：

- `Get_State() == 2`：供应商语言中的“广告已就绪”，只决定能否调用展示。
- `Set_Listener` 的事件 `1`：供应商语言中的“奖励条件已满足”，此时游戏才能发金币。
- `AdManager`：只认识游戏语言 `IsReady`、`Show()`、`RewardEarned`、`Failed`，不认识 SDK 类型、方法名或魔法数字。
- `RewardedAd` 包装器：边界翻译者，独占所有 `RewardedAdSdk` 依赖及 `1/2` 的含义。

原代码有三个具体崩盘点：

1. 玩家尚未观看广告，只因“已就绪”就提前得到金币，业务语义错误。
2. 即使广告未就绪，`SDK_Show()` 仍会无条件执行，因为 `if` 只控制 `GiveCoins(100)`。
3. 供应商修改状态码、事件码或方法名时，所有直接依赖 SDK 的游戏代码都要同步修改。

跨书关联：这是 GoF《设计模式》的**适配器模式**——包装器把供应商接口转换成游戏所需接口；也是《敏捷软件开发》的 **DIP**——高层游戏业务依赖游戏方抽象 `IRewardedAd`，而不是依赖第三方具体类。

学习测试（Learning Tests）：先用小测试验证对 SDK 的假设，例如 `Get_State()` 的实际返回语义和回调时机；把供应商行为变化集中暴露在边界测试，而不是让业务代码猜测。

## 六、作业布置（2026-08-20）

文件：`Homework/代码整洁/第二轮-干净地写/AdSdkBoundary.cs`

| TODO | 内容                                      |
| ---- | ----------------------------------------- |
| 1    | 定义 `IRewardedAd` —— 游戏方契约          |
| 2    | 实现 `RewardedAd : IRewardedAd` 包装 SDK  |
| 3    | `AdManager.PlayerClickedWatchAd` 走通流程 |

**验收红线**：使用方（AdManager 及任何游戏代码）出现 `RewardedAdSdk` 字样 = 边界泄漏，不通过。

## 七、作业验收记录

### 第一轮（2026-08-20）——未通过

做对的部分：

- `AdManager` 只依赖 `IRewardedAd`，没有直接引用 `RewardedAdSdk`，类型边界没有泄漏。
- SDK 初始化、状态码和事件码都留在 `RewardedAd` 包装器中，方向正确。
- 本题文件没有新增编译错误；全项目构建仍被 Day 1 `RenameSkill.cs` 的既有 `DropLoot` 错误阻断。

需要纠正的核心：把异步奖励事件写成了同步轮询。

- 第 49-53 行：`CanGetReward()` 调用时才注册监听，然后立刻返回 `_canGetReward`。广告奖励回调晚于函数返回时，这次读取必然拿不到新结果。
- 第 81-85 行：`Show()` 后立刻查询奖励，把“未来发生的回调”当成了“现在可读取的状态”。回调真正到达时，没有任何游戏代码收到通知并发金币。
- 第 57 行：SDK 注释已经说明 `2 = 就绪`，当前却用 `0` 才展示；而且 `AdManager` 没有通过游戏契约判断就绪。
- 契约还没有表达失败事件 `2`。

子问题：假设 `SDK_Show()` 调用 **3 秒后**，SDK 才执行监听器并传入事件 `1`。请沿着当前第 81→82→51→52 行逐步判断：`CanGetReward()` 当场返回什么？3 秒后又由谁调用发金币逻辑？据此重写契约和调用流程。

### 第二轮（2026-08-20）——未通过，给出标准实现

进步：

- 就绪码已从 `0` 修正为 `2`。
- SDK 方言仍全部留在包装器内，边界未泄漏。
- 全项目构建成功：0 错误，24 个既有警告；Day 1 的 `DropLoot` 编译阻塞已修复。

仍未解决：

- `GetReward(Action)` 仍在调用时才注册 SDK 监听，并立即读取 `_canGetReward`，本质还是同步轮询。
- `async void PlayerClickedWatchAd()` 内没有 `await`，并不会“等待广告加载”。
- SDK 回调到达后只把布尔值设为 `true`，没有主动通知 `AdManager`；该值也没有复位，领取按钮可重复发奖。
- 失败事件 `2` 仍未翻译成游戏契约。

标准实现的关键时序是：**构造包装器时注册一次 SDK 监听 → 游戏订阅干净事件 → 点击时仅检查就绪并展示 → SDK 将来回调时由事件主动发奖或报告失败。**

```csharp
public interface IRewardedAd
{
    bool IsReady { get; }
    event Action? RewardEarned;
    event Action? Failed;
    void Show();
}

public sealed class RewardedAd : IRewardedAd
{
    private readonly RewardedAdSdk _sdk = new();
    public bool IsReady => _sdk.Get_State() == 2;
    public event Action? RewardEarned;
    public event Action? Failed;

    public RewardedAd(string appId)
    {
        _sdk.SDK_Init(appId);
        _sdk.Set_Listener(OnSdkEvent);
    }

    public void Show() => _sdk.SDK_Show();

    private void OnSdkEvent(int code)
    {
        if (code == 1) RewardEarned?.Invoke();
        if (code == 2) Failed?.Invoke();
    }
}

public sealed class AdManager
{
    private readonly IRewardedAd _ad;

    public AdManager(IRewardedAd ad)
    {
        _ad = ad;
        _ad.RewardEarned += GiveCoins;
        _ad.Failed += HandleAdFailure;
    }

    public void PlayerClickedWatchAd()
    {
        if (_ad.IsReady) _ad.Show();
    }

    private void GiveCoins() { /* 发放 100 金币 */ }
    private void HandleAdFailure() { /* 提示玩家 */ }
}
```

请按此时序亲手重写作业文件。最终验收只检查三点：`AdManager` 不认识 SDK、奖励由 SDK 回调主动触发、失败码被翻译。

### 收尾（2026-08-20）

用户选择“下一轮”，不再进行最终重写验收。本节按“概念标准解已掌握、代码标准实现已提供、个人作业停留在第二轮未通过版本”收尾，不记作代码通过。

---

`[进度：阶段四-代码整洁 → Day 7「边界」已收尾（最终重写跳过）]`
