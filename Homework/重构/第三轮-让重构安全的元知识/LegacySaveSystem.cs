using System;
using System.Collections.Generic;

namespace StudyNotes.Homework.Refactor.Legacy
{
    /// <summary>
    /// 重构 Day 15 作业：综合实战（阶段考）
    /// 一段 ~100 行的遗留存档/背包代码——集中了 Day 1-14 学过的多种坏味道。
    /// 毕业考 4 步：
    ///   1. 找坏味道：列清单（至少 5 个，写进 00-我的回答.md）
    ///   2. 找接缝 + 写特征测试固化当前行为（不动原逻辑）
    ///   3. 测试保护下小步重构（一次一步，每步跑测试）
    ///   4. 全部测试保持 PASS，验收提交
    /// 铁律：行为、顺序、数值一律不变
    /// </summary>

    // 全局数据
    public static class SaveData
    {
        public static int coin = 0;   // 金币
        public static int level = 1;  // 等级
    }

    public class Item
    {
        public string name;
        public int type;     // 1=武器 2=药水 3=材料（魔法数字）
        public int amount;
        public int value;
    }

    public class Inventory
    {
        public List<Item> items = new List<Item>();
    }

    public class LegacySaveSystem
    {
        // 一坨：序列化 + 写盘 + 打印全在这里
        public void SaveGame(Inventory inv, string path)
        {
            string data = "";
            data += "coin:" + SaveData.coin + "\n";
            data += "level:" + SaveData.level + "\n";
            foreach (var it in inv.items)
            {
                string line = it.name + "|" + it.type + "|" + it.amount;
                if (it.type == 1) line += "|atk:" + it.value;
                else if (it.type == 2) line += "|hp:" + it.value;
                else line += "|mat:" + it.value;
                data += line + "\n";
            }
            System.IO.File.WriteAllText(path, data);
            Console.WriteLine("saved to " + path);
        }

        // 与 SaveGame 对称的重复分发 + 魔法数字
        public void LoadGame(Inventory inv, string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts[0] == "coin") { SaveData.coin = int.Parse(parts[1]); continue; }
                if (parts[0] == "level") { SaveData.level = int.Parse(parts[1]); continue; }
                var item = new Item();
                item.name = parts[0];
                item.type = int.Parse(parts[1]);
                item.amount = int.Parse(parts[2]);
                if (item.type == 1) item.value = int.Parse(parts[3].Substring(4));
                else if (item.type == 2) item.value = int.Parse(parts[3].Substring(3));
                else item.value = int.Parse(parts[3].Substring(4));
                inv.items.Add(item);
            }
        }

        // 依恋情结：背包的总价逻辑，为什么住在存档系统里？
        public int TotalValue(Inventory inv)
        {
            int total = 0;
            foreach (var it in inv.items)
                total += it.value * it.amount;
            return total;
        }
    }
}
