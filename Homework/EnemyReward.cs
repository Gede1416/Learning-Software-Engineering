using System;

namespace StudyNotes.Homework.Refactor
{
    /// <summary>
    /// 重构 Day 2 作业：坏味道「重复代码」（Duplicated Code）
    /// 任务：把 NormalEnemy 与 EliteEnemy 里重复的奖励公式抽到一处。
    /// 铁律：只抽取代码，不改数值、不改任何行为。
    /// 场景：两种敌人死亡结算
    /// </summary>
    public class Player
    {
        public int Level;
        public int Gold;
        public int Xp;

        public void GainGold(int g) => Gold += g;
        public void GainXp(int x) => Xp += x;
    }

    public class NormalEnemy
    {
        public void OnDie(Player player)
        {
            // TODO: 重复公式 ×2，抽取到共用的结算处
            player.GainGold(10 + player.Level * 2);
            player.GainXp(20 + player.Level * 3);
        }
    }

    public class EliteEnemy
    {
        public void OnDie(Player player)
        {
            // TODO: 和 NormalEnemy 一模一样 —— 改动必须记得 ×2
            player.GainGold(10 + player.Level * 2);
            player.GainXp(20 + player.Level * 3);
        }
    }

    // TODO: 新建共用的奖励结算（方法 or 类），让两个敌人只调一处
}
