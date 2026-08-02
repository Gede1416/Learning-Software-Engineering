using System;

namespace StudyNotes.Homework.Refactor
{
    /// <summary>
    /// 重构 Day 3 作业：坏味道「过长函数」（Long Function）
    /// 任务：把 LevelUp 拆成多个小函数，让函数名成为「意图的注释」。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：玩家升级结算
    /// </summary>
    public class Player
    {
        public int Level;
        public int Exp;
        public int MaxHp;
        public int Hp;
        public int MaxMp;
        public int Mp;
        public int Atk;
        public int Def;
        public int SkillPoints;
        public bool HasGodMode;
    }

    public static class UIManager { public static void ShowLevelUp(int level) { } }
    public static class AudioManager { public static void PlayLevelUp() { } }
    public static class AchievementSystem { public static void Unlock(string id) { } }
    public static class SaveSystem { public static void Save(Player p) { } }

    public class LevelUpSystem
    {
        // TODO: 这个函数做了几件事？每件事是不是可以是一个小函数？
        public void LevelUp(Player player)
        {
            int need = player.Level * 100 + 50;
            if (player.Exp >= need)
            {
                player.Level++;
                player.Exp -= need;
                player.MaxHp += 20;
                player.Hp = player.MaxHp;
                player.MaxMp += 10;
                player.Mp = player.MaxMp;
                player.Atk += 5;
                player.Def += 3;
                if (player.Level % 10 == 0)
                {
                    player.MaxHp += 50;
                    player.MaxMp += 30;
                    player.Atk += 15;
                    player.Def += 8;
                    player.SkillPoints += 3;
                }
                if (player.Level == 100)
                {
                    player.HasGodMode = true;
                    player.SkillPoints += 100;
                }
                UIManager.ShowLevelUp(player.Level);
                AudioManager.PlayLevelUp();
                AchievementSystem.Unlock("level_" + player.Level);
                SaveSystem.Save(player);
            }
        }
    }
}
