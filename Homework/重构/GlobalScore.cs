using System;
using System.Reflection.Metadata;

namespace StudyNotes.Homework.Refactor.Global
{
    /// <summary>
    /// 重构 Day 5 作业：坏味道「全局数据/可变数据」（Global / Mutable Data）
    /// 任务：用「封装变量」（Encapsulate Variable）把全局字段收进私有字段 + 属性/方法访问。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：全局分数 / 全局难度
    /// </summary>
    public class Player
    {
        public int Hp;
        public void TakeDamage(int dmg) { Hp -= dmg; }
    }

    // 把它们收进私有字段，只留属性和方法访问（可加保护：分数不能为负）。
    public static class GameState
    {
        private static int _score;        // 全局分数
        private static int _difficulty;   // 全局难度
        public static int Score
        {
            get { return _score; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                _score = value;
                if (_score > 1000) {
                    Difficulty = 3;
                }
                if(_score > 99999) {
                    _score = 99999;
                    Console.WriteLine("分数上限 99999");
                }
            }
        }
        public static int Difficulty
        {
            get { return _difficulty; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                if( value >= 3) Console.WriteLine("怪物强化");
                _difficulty = value;
            }
        }
    }

    public class ScoreSystem
    {
        public void OnEnemyKilled(int value)
        {
            GameState.Score += value;
        }
    }

    public class Enemy
    {
        public void Attack(Player player)
        {
            int dmg = 10 * GameState.Difficulty;
            player.TakeDamage(dmg);
        }
    }
}
