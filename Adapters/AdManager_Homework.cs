namespace StudyNotes.Adapters;

/// <summary>
/// 适配器 + 外观模式作业：广告 SDK 统一接口
/// 来源：docs/设计模式/08-适配器+外观-Adapter-Facade.md 作业
/// </summary>

// ====== TODO 1：定义统一接口 ======
public interface IRewardAdService
{
    /// <summary>
    /// 展示激励广告
    /// </summary>
    /// <param name="onComplete">广告看完后的回调（发放奖励）</param>
    void ShowRewardAd(Action onComplete);
}

// ====== TODO 2：两个 SDK 适配器 ======

// 模拟穿山甲 SDK（API 不能改）
public static class BytedanceAd
{
    public static void ShowRewardVideo(string placementId, Action onRewardCallback)
    {
        // 第三方 SDK 代码，不可修改
        Console.WriteLine($"[穿山甲] 播放激励视频: {placementId}");
        onRewardCallback?.Invoke();
    }
}

// 穿山甲适配器
public class BytedanceAdAdapter : IRewardAdService
{
    private string _placementId = "reward_001";

    public void ShowRewardAd(Action onComplete)
    {
        // TODO: 把统一接口翻译成穿山甲的调用方式
    }
}

// 模拟优量汇 SDK（API 不能改）
public static class YLHAd
{
    public struct YLHConfig { public string placementId; }

    public static void LoadAndShow(string type, YLHConfig config)
    {
        // 第三方 SDK 代码，不可修改
        Console.WriteLine($"[优量汇] 加载并展示广告: {config.placementId}");
    }
}

// 优量汇适配器
public class YLHAdAdapter : IRewardAdService
{
    private string _placementId = "reward_001";

    public void ShowRewardAd(Action onComplete)
    {
        // TODO: 把统一接口翻译成优量汇的调用方式
    }
}

// ====== TODO 3：外观 — AdManager ======
public class AdManager
{
    private IRewardAdService _rewardAdService;

    public AdManager(IRewardAdService rewardAdService)
    {
        _rewardAdService = rewardAdService;
    }

    /// <summary>
    /// 游戏内唯一对外入口：展示激励广告
    /// 调用方不需要知道底层是哪个 SDK
    /// </summary>
    public void ShowRewardAd(Action onRewardGranted)
    {
        // TODO: 委托给 _rewardAdService，加上埋点/日志等通用逻辑
    }

    // 切换 SDK 只改这一处注入：
    // new AdManager(new BytedanceAdAdapter())   → 穿山甲
    // new AdManager(new YLHAdAdapter())          → 优量汇
}

// ====== 思考题 ======
// 如果加一个新广告类型「插屏广告（Interstitial）」，
// 哪些要改？哪些不用改？
//
// Facade（AdManager）：需要加 ShowInterstitial() 方法和对应的 IService
// Adapter：需要新建 IInterstitialAdService 接口 + 两个新 Adapter
//
// 你的答案：
// _____________________________________________________________
