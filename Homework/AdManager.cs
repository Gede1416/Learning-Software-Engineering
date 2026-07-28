using System;

namespace StudyNotes.Homework
{
    /// <summary>
    /// 适配器 + 外观模式作业：广告 SDK 统一接口
    /// 来源：docs/设计模式/08-适配器+外观-Adapter-Facade.md 作业
    /// </summary>

    // ====== 统一接口 ======
    public interface IRewardAdService
    {
        void ShowRewardAd(Action onComplete);
    }

    // ====== 模拟穿山甲 SDK（API 不能改）======
    public static class BytedanceAd
    {
        public static void ShowRewardVideo(string placementId, Action onRewardCallback)
        {
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
            BytedanceAd.ShowRewardVideo(_placementId, onComplete);
        }
    }

    // ====== 模拟优量汇 SDK（API 不能改）======
    public static class YLHAd
    {
        public struct YLHConfig { public string placementId; }

        public static void LoadAndShow(string type, YLHConfig config)
        {
            Console.WriteLine($"[优量汇] 加载并展示广告: {config.placementId}");
        }
    }

    // 优量汇适配器
    public class YLHAdAdapter : IRewardAdService
    {
        private string _placementId = "reward_001";

        public void ShowRewardAd(Action onComplete)
        {
            YLHAd.LoadAndShow("reward", new YLHAd.YLHConfig { placementId = _placementId });
            onComplete?.Invoke();
        }
    }

    // ====== 外观 — AdManager ======
    public class AdManager
    {
        private IRewardAdService _rewardAdService;

        public AdManager(IRewardAdService rewardAdService)
        {
            _rewardAdService = rewardAdService;
        }

        public void ShowRewardAd(Action onRewardGranted)
        {
            _rewardAdService.ShowRewardAd(onRewardGranted);
        }
    }

    // ====== 思考题 ======
    // ...
}
