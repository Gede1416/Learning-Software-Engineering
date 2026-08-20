using System;

namespace StudyNotes.Homework.CleanCode.Tests
{
    /// <summary>
    /// 代码整洁 Day 8 作业：把坏测试改造成 F.I.R.S.T. 干净测试
    /// 要求：
    ///   1. 拆成独立测试（Save 和 Load 分开）
    ///   2. 命名带行为（测试名称 = 行为描述）
    ///   3. Assert 自动判断（不用 Console.WriteLine 人眼读）
    ///   4. 每个测试一个行为（Given-When-Then）
    /// 铁律：不改 Player/SaveGame/LoadGame 生产代码
    /// </summary>
    public class Player : IEquatable<Player>
    {
        public int Hp;
        public int MaxHp = 100;
        public string Name = "";

        public bool Equals(Player? other)
        {
            if (other == null)
                return false;
            var hpEquals = this.Hp.Equals(other.Hp);
            var maxHpEquals = this.MaxHp.Equals(other.MaxHp);
            var nameEquals = this.Name.Equals(other.Name);
            var resEquals = hpEquals && maxHpEquals && nameEquals;
            return resEquals;
        }

    }

    public static class SaveSystem
    {
        private static Player _slot;   // 模拟存档槽（简化，不碰真实磁盘）

        public static void SaveGame(Player p) { _slot = p; }
        public static Player LoadGame() { return _slot; }

        public static bool SoltEquals(Player player)
        {
            return _slot.Equals(player);
        }
    }

    public static class CleanSaveTests
    {
        private static int _passed, _failed;
        private static void Assert(bool cond, string name)
        {
            if (cond) { _passed++; Console.WriteLine($"  PASS {name}"); }
            else { _failed++; Console.WriteLine($"  FAIL {name}"); }
        }
        private static Player playerTest1 = new Player()
        {
            Name = "playerTest1",
            MaxHp = 1008611,
            Hp = 10086
        };
        private static Player playerTest2 = new Player()
        {
            Name = "playerTest2",
            MaxHp = 10086,
            Hp = 100
        };
        public static void Run()
        {
            Console.WriteLine("== 存档测试（F.I.R.S.T. 版）==");
            // TODO 1: Save 测试——保存后数据被完整写入存档槽（血量、名字）

            SaveSystem.SaveGame(playerTest1);
            var saveTest = SaveSystem.SoltEquals(playerTest1);
            Assert(saveTest, "Save 测试——保存后数据被完整写入存档槽（血量、名字）");

            // TODO 2: Load 测试——读取返回与保存一致的对象
            var playerTestLoad = SaveSystem.LoadGame();
            var loadTest = playerTestLoad.Equals(playerTest1);
            Assert(loadTest, "Load 测试——读取返回与保存一致的对象");

            // TODO 3: 独立测试——保存不影响 Load 的另一个场景
            SaveSystem.SaveGame(playerTest2);
            var saveThenloadTest = playerTestLoad.Equals(playerTest1);
            Assert(saveThenloadTest, "独立测试——保存不影响 Load 的另一个场景");

            Console.WriteLine($"结果：通过 {_passed} / 失败 {_failed}");
        }


    }
}
