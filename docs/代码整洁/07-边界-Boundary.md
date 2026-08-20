# 边界（Boundaries）

> 来源：《代码整洁之道》Robert C. Martin 第 8 章
> 跨书联动：设计模式-适配器模式（把丑接口翻译成我们认识的接口）；Day 5 对象与数据结构（封装）；Day 6 异常包装（边界处翻译，异曲同工）

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
        if (_sdk.Get_State() == 2)                              // 魔法数字 2 = 就绪
        {
            _sdk.Set_Listener(code =>
            {
                if (code == 1) GiveCoins(100);                  // 魔法数字 1 = 奖励
            });
            _sdk.SDK_Show();
        }
    }
}
```

## 二、为什么这是坏的（边界泄漏）

1. **供应商一改 API，游戏代码跟着改**——SDK 的每一次升级都变成对全项目的大扫荡（霰弹式修改，联动重构 Day 6 坏味道）
2. **int 状态码 0/1/2 的含义散落在业务代码里**——SDK 的怪癖（魔法数字、命名风格）泄漏进游戏领域，业务代码里全是供应商的行话
3. **边界不清晰**——游戏该关心的是「广告就绪没？看了给不给奖励？」，不该关心 SDK 的 `SDK_Show` / `Get_State`

## 三、边界正解

在我们自己的代码与第三方之间，插一层**我们定义的契约**：

```
游戏业务代码 ── 只认 IRewardedAd（就绪/展示/奖励事件）── 包装器 RewardedAd ── 翻译 SDK 怪癖 ── RewardedAdSdk
```

- 游戏侧只看自己定义的接口，永远不出现 SDK 字样
- 包装器是唯一摸 SDK 的地方，把怪癖翻译成游戏事件
- 供应商怎么变，只改包装器，游戏代码零改动

> 与异常包装（Day 6）异曲同工：底层变化被翻译成我们领域的语言，不泄漏。

## 四、作业布置（2026-08-20）

文件：`Homework/代码整洁/第二轮-干净地写/AdSdkBoundary.cs`

| TODO | 内容 |
|------|------|
| 1 | 定义 `IRewardedAd` —— 游戏方契约（就绪 / 展示 / 奖励成功、失败事件） |
| 2 | 实现 `RewardedAd : IRewardedAd` 包装 SDK：构造时 `SDK_Init`、`IsReady` ← `Get_State()`==2、`Show()` → `SDK_Show()`、`Set_Listener` 的 int 回调 → 翻译成游戏事件 |
| 3 | `AdManager.PlayerClickedWatchAd` 走通流程：就绪才展示，奖励事件 → 发金币 |

**验收红线**：使用方（AdManager 及任何游戏代码）出现 `RewardedAdSdk` 字样 = 边界泄漏，不通过。

## 五、你的回答 / 纠错 / 标准解

（待作业验收后回填）

## 六、作业验收记录

（待补）

---

`[进度：阶段四-代码整洁 → Day 7「边界」概念已导入、作业布置中]`
