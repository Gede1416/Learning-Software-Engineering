using System;

namespace StudyNotes.Homework.CleanCode.Boundary
{
    /// <summary>
    /// 代码整洁 Day 7 作业：边界——第三方 SDK 封装
    /// 激励视频广告 SDK 很难用（供应商定死，不能改），游戏各处要「看广告 → 发金币」。
    /// 铁律：游戏业务代码不许直接摸 RewardedAdSdk，只能通过 IRewardedAd（边界不能泄漏）。
    ///   TODO 1: 定义 IRewardedAd —— 游戏方关心的契约（就绪 / 展示 / 奖励事件）
    ///   TODO 2: 实现 RewardedAd : IRewardedAd 包装 Sdk，翻译怪癖：
    ///     · 构造时 SDK_Init
    ///     · IsReady ← Get_State 的 int 返回值（2 = 就绪）
    ///     · Show() → Sdk.SDK_Show()
    ///     · Set_Listener 的 int 回调 → 翻译成游戏事件（1 = 奖励，2 = 失败）
    ///   TODO 3: AdManager.PlayerClickedWatchAd 走通流程（就绪才展示）
    /// </summary>

    // ===== 供应商的 SDK：定死的，不能改 =====
    public class RewardedAdSdk
    {
        public void SDK_Init(string appId) { }
        public int Get_State() { return 2; }                       // 0=未加载 1=加载中 2=就绪
        public void SDK_Show() { }
        public void Set_Listener(Action<int> onEvent) { }          // 0=展示 1=奖励 2=失败
    }

    // TODO 1: 游戏方契约——先想：游戏业务代码只关心哪几件事？
    public interface IRewardedAd
    {
        // 你定义（提示：IsReady / Show / 奖励成功、失败事件）
    }

    // TODO 2: 包装器——翻译 SDK 的怪癖，把丑 API 变成干净的契约
    public class RewardedAd : IRewardedAd
    {
        private readonly RewardedAdSdk _sdk = new();

        // 你实现
    }

    // TODO 3: 使用方——这段代码里不允许出现 RewardedAdSdk 字样
    public class AdManager
    {
        private readonly IRewardedAd _ad;
        public AdManager(IRewardedAd ad) { _ad = ad; }

        public void PlayerClickedWatchAd()
        {
            // 就绪才展示；奖励事件 → 发金币
        }
    }
}
