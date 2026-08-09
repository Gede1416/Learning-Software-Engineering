using System;
using StudyNotes.Homework.Refactor.Level;

namespace StudyNotes.Homework.Refactor.Level
{
    /// <summary>
    /// 重构 Day 14 作业：测试保护下的重构工作流
    /// 任务：用 Day 13 的行为契约（4 条）给 LevelUpSystem.LevelUp 写测试
    /// 铁律：只加测试，不动 LevelUpSystem 任何代码；在 Program.cs 的 Main 里调用 LevelUpTests.Run() 运行
    /// </summary>
    public static class LevelUpTests
    {
        // 断言辅助（已给，不要改）
        private static int _passed, _failed;
        private static void Assert(bool cond, string name)
        {
            if (cond) { _passed++; Console.WriteLine($"  PASS {name}"); }
            else { _failed++; Console.WriteLine($"  FAIL {name}"); }
        }

        // 造一个指定等级的玩家（其余属性固定初始值）
        private static Player CreatePlayer(int level, int exp)
        {
            return new Player
            {
                Level = level,
                Exp = exp,
                MaxHp = 100,
                Hp = 60,
                MaxMp = 50,
                Mp = 20,
                Atk = 10,
                Def = 5,
                SkillPoints = 0
            };
        }

        public static void Run()
        {
            Console.WriteLine("== LevelUpSystem 行为契约测试 ==");
            TestMainLevelUp();      // 契约 1：主升级
            TestTenthLevel();       // 契约 2：十级倍奖励
            TestGodMode();          // 契约 3：满级神模式
            TestNotEnoughExp();     // 契约 4：经验不足
            Console.WriteLine($"结果：通过 {_passed} / 失败 {_failed}");
        }

        // TODO 契约 1：Level=5, Exp=600（needExp=550）→ Level=6, Exp=50, MaxHp=120 且 Hp 回满 120, MaxMp=60 且 Mp 回满, Atk=15, Def=8
        private static void TestMainLevelUp()
        {
            Console.WriteLine("[契约 1] 主升级");
            // 提示：var p = CreatePlayer(5, 600); new LevelUpSystem().LevelUp(p);
            // Assert(p.Level == 6, "等级 +1");
            // ...

            var p = CreatePlayer(5, 600);
            var levelUpSystem = new LevelUpSystem();
            levelUpSystem.LevelUp(p);
            bool isPast =
                p.Level == 6 && p.Exp == 50 && p.MaxHp == 120 && p.Hp == 120
                && p.MaxMp == 60 && p.Mp == 60 && p.Atk == 15 && p.Def == 8;
            Assert(isPast, "主升级");
        }

        // TODO 契约 2：Level=9, Exp=960 → Level=10, 且叠加 SkillPoints=3, MaxHp=170(100+20+50), MaxMp=90, Atk=30, Def=16
        private static void TestTenthLevel()
        {
            Console.WriteLine("[契约 2] 十级倍奖励");
            var p = CreatePlayer(9, 960);
            var levelUpSystem = new LevelUpSystem();
            levelUpSystem.LevelUp(p);
            bool isPast =
                p.Level == 10 && p.SkillPoints == 3 && p.MaxHp == 170
                && p.MaxMp == 90 && p.Atk == 30 && p.Def == 16;
            Assert(isPast, "十级倍奖励");
        }

        // TODO 契约 3：Level=99, Exp=9950 → Level=100, HasGodMode=true, SkillPoints=103（十倍 3 + 神模式 100）
        private static void TestGodMode()
        {
            Console.WriteLine("[契约 3] 满级神模式");
            var p = CreatePlayer(99, 9950);
            var levelUpSystem = new LevelUpSystem();
            levelUpSystem.LevelUp(p);
            bool isPast =
                p.Level == 100 && p.HasGodMode && p.SkillPoints == 103;
            Assert(isPast, "满级神模式");
        }

        // TODO 契约 4：Level=5, Exp=100（<550）→ 任何数值不变、无事件
        private static void TestNotEnoughExp()
        {
            Console.WriteLine("[契约 4] 经验不足");
            var p = CreatePlayer(5, 100);
            var levelUpSystem = new LevelUpSystem();
            levelUpSystem.LevelUp(p);
            bool isPast =
                p.Level == 5;
            Assert(isPast, "经验不足");
        }
    }
}
