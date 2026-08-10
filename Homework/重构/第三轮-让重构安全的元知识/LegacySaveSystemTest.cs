
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace StudyNotes.Homework.Refactor.Legacy
{
    public class LegacySaveSystemTest
    {
        private static int _passed, _failed;
        private static string path = Path.GetTempFileName();
        private static int TestCoin = 10;
        private static int TestLevel = 11;
        private static Inventory CreateInventoryTest()
        {

            Item item1 = new Item
            {
                name = "武器",
                type = 1,
                amount = 10,
                value = 10
            }; Item item2 = new Item
            {
                name = "药水",
                type = 2,
                amount = 10,
                value = 10
            }; Item item3 = new Item
            {
                name = "材料",
                type = 3,
                amount = 10,
                value = 10
            };
            List<Item> items = new List<Item> { item1, item2, item3 };
            Inventory inventory = new Inventory { items = items };
            return inventory;
        }
        private static void AssertLegacy(bool cond, string name)
        {
            if (cond) { _passed++; Console.WriteLine($"  PASS {name}"); }
            else { _failed++; Console.WriteLine($"  FAIL {name}"); }
        }

        public static void Run()
        {
            TestSave();
            TestLoad();
        }

        private static void TestSave()
        {
            LegacySaveSystem legacySaveSystem = new LegacySaveSystem();
            Inventory inventory = CreateInventoryTest();
            SaveData.coin = TestCoin;
            SaveData.level = TestLevel;
            legacySaveSystem.SaveGame(inventory, path);


        }

        private static void TestLoad()
        {
            LegacySaveSystem legacySaveSystem = new LegacySaveSystem();
            SaveData.coin = 0;
            SaveData.level = 0;
            Inventory inventoryTest = CreateInventoryTest();

            Inventory inventory = new Inventory();
            legacySaveSystem.LoadGame(inventory, path);

            bool isInventoryPassed = inventoryTest.Equals(inventory);
            bool isLoadPassed = SaveData.coin == TestCoin && SaveData.level == TestLevel;
            AssertLegacy(isLoadPassed && isInventoryPassed, "加载测试");
        }

        //测试读取写入
        //写入 SaveData Inventory path
        //读取 Inventory path
    }
}