using System.Net.Http.Headers;
using System.Text;

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
    #region 全局数据
    public static class SaveData
    {
        public static int coin = 0;   // 金币
        public static int level = 1;  // 等级

        public static bool ToLoadSaveData(string[] parts)
        {
            if (parts[0] == "coin") { coin = int.Parse(parts[1]); return true; }
            if (parts[0] == "level") { level = int.Parse(parts[1]); return true; }
            return false;
        }

        public static string ToSaveData(string data)
        {
            data += "coin|" + SaveData.coin + "\n";
            data += "level|" + SaveData.level + "\n";
            return data;
        }


    }
    #endregion

    #region 物品数据
    public enum ItemTypeEnum
    {
        None = 0,
        Weapon = 1,
        Medicine = 2
    }

    public interface IItemAction
    {
        public string Save(Item item);
        public int Load(string[] parts);
    }

    public class NoneItemAction : IItemAction
    {
        private string _preVal = "|mat:";
        private int _preCount => _preVal.Count() - 1;
        public int Load(string[] parts) => int.Parse(parts[3].Substring(_preCount));
        public string Save(Item item) => _preVal + item.value;
    }
    public class MedicineItemAction : IItemAction
    {
        private string _preVal = "|hp:";
        private int _preCount => _preVal.Count() - 1;
        public int Load(string[] parts) => int.Parse(parts[3].Substring(_preCount));

        public string Save(Item item) => _preVal + item.value;
    }
    public class WeaponItemAction : IItemAction
    {
        private string _preVal = "|atk:";
        private int _preCount => _preVal.Count() - 1;
        public int Load(string[] parts) => int.Parse(parts[3].Substring(_preCount));
        public string Save(Item item) => _preVal + item.value;
    }

    public class Item : IEquatable<Item>
    {
        public string name;
        public int type;     // 1=武器 2=药水 3=材料（魔法数字）
        public int amount;
        public int value;
        private static Dictionary<ItemTypeEnum, IItemAction> actionMap =
        new Dictionary<ItemTypeEnum, IItemAction>
        {
            {ItemTypeEnum.None, new NoneItemAction() },
            {ItemTypeEnum.Weapon, new WeaponItemAction() },
            {ItemTypeEnum.Medicine, new MedicineItemAction() },
        };

        //todo ToSave
        public string ToSave()
        {
            string line = this.name + "|" + this.type + "|" + this.amount;
            if (actionMap.TryGetValue((ItemTypeEnum)type, out var itemAction))
                line += itemAction.Save(this);
            else
                line += actionMap[ItemTypeEnum.None].Save(this);
            return line + "\n";
        }

        //todo ToLoad
        public void CreatByLoad(string[] parts)
        {
            name = parts[0];
            type = int.Parse(parts[1]);
            amount = int.Parse(parts[2]);
            if (actionMap.TryGetValue((ItemTypeEnum)type, out var itemAction))
                this.value = itemAction.Load(parts);
            else
                this.value = actionMap[ItemTypeEnum.None].Load(parts);
        }

        // todo TotalValue
        public int TotalValue()
        {
            int total = 0;
            total += this.value * this.amount;
            return total;
        }

        public bool Equals(Item? other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            bool isNameEquals = name.Equals(other.name);
            bool isTypeEquals = type.Equals(other.type);
            bool isAmountEquals = amount.Equals(other.amount);
            bool isValueEquals = value.Equals(other.value);
            return isAmountEquals
                && isNameEquals
                && isTypeEquals
                && isValueEquals;
        }

    }
    #endregion

    #region 目录
    public class Inventory : IEquatable<Inventory>
    {
        public List<Item> items = new List<Item>();
        //toSave
        //ToLoad

        //TotalValue
        public int TotalValue()
        {
            int total = 0;
            foreach (var it in this.items)
                total += it.TotalValue();
            return total;
        }

        public bool Equals(Inventory? other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (other.items.Count != this.items.Count)
                return false;

            int count = this.items.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var itemThis = this.items[i];
                var itemOther = other.items[i];
                if (!itemThis.Equals(itemOther))
                    return false;
            }
            return true;
        }

    }
    #endregion

    #region 系统

    public class LegacySaveSystem
    {
        public void SaveGame(Inventory inv, string path)
        {
            string data = "";
            data = SaveData.ToSaveData(data);
            foreach (var it in inv.items)
            {
                data += it.ToSave();
            }
            System.IO.File.WriteAllText(path, data);
            Console.WriteLine("saved to " + path);
        }

        public void LoadGame(Inventory inv, string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (var line in lines)
            {
                string[] parts = SplitLine(line);

                bool flowControl = SaveData.ToLoadSaveData(parts);
                if (flowControl) continue;

                Item item = new Item();
                item.CreatByLoad(parts);
                inv.items.Add(item);
            }
        }

        private string[] SplitLine(string line)
        {
            return line.Split('|');
        }
    }
    #endregion
}
