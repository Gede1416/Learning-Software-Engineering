using System;

namespace StudyNotes.Homework.Refactor
{
    /// <summary>
    /// 重构 Day 1 作业：坏味道「神秘命名」（Mysterious Name）
    /// 任务：找出下列代码中所有「名字在撒谎」的地方，用重命名修复。
    /// 铁律：只改名字，不改任何行为 —— 这正是重构的定义。
    /// 场景：每日任务结算系统
    /// </summary>
    public class DailyRewardSystem
    {
        private IReward _reward;
        public int gold;
        public int xp;

        public void Init(IReward reward)
        {
            _reward = reward;
        }

        public void Reward()
        {
            _reward.Reward(this);
        }

        public int AllRewardCount()
        {
            return gold + xp;
        }
    }

    public interface IReward
    {
        public void Reward(DailyRewardSystem dailyRewardSystem);
    }

    public class NormalReward : IReward
    {
        public void Reward(DailyRewardSystem dailyRewardSystem)
        {
            dailyRewardSystem.gold += 100;
            dailyRewardSystem.xp += 50;
        }
    }

    public class DoubleReward : IReward
    {
        public void Reward(DailyRewardSystem dailyRewardSystem)
        {
            dailyRewardSystem.gold += 100 * 2;
            dailyRewardSystem.xp += 50 * 2;
        }
    }
}
