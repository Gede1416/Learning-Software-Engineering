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
        public int gold;
        public int xp;
        public int hp;          // TODO: 任务奖励系统里出现 hp，这名字在说什么？

        public void R()         // TODO: R 是谁？三个月后的你还认识它吗？
        {
            gold += 100;
            xp += 50;
            // TODO: 如果明天策划要加「奖励计算 ×2 双倍日」，你要在哪里改？
        }

        public int G()          // TODO: G？
        {
            return gold + xp;
        }
    }
}
