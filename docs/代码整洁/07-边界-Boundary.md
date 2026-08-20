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

## 三、你的回答（待补）

## 四、纠错记录（待补）

## 五、标准解（待两轮答错后给出）

## 六、作业布置（2026-08-20）

文件：`Homework/代码整洁/第二轮-干净地写/AdSdkBoundary.cs`

| TODO | 内容 |
|------|------|
| 1 | 定义 `IRewardedAd` —— 游戏方契约 |
| 2 | 实现 `RewardedAd : IRewardedAd` 包装 SDK |
| 3 | `AdManager.PlayerClickedWatchAd` 走通流程 |

**验收红线**：使用方（AdManager 及任何游戏代码）出现 `RewardedAdSdk` 字样 = 边界泄漏，不通过。

## 七、作业验收记录（待补）

---

`[进度：阶段四-代码整洁 → Day 7「边界」苏格拉底问答进行中]`
