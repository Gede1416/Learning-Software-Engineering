# 综合实战（阶段考）——遗留存档/背包系统重构

> 来源：全部教材（《重构》Fowler + 《修改代码的艺术》Feathers + 《敏捷》+ GoF + 《代码整洁之道》）
> 阶段问题：「这坨烂代码怎么安全改？」——毕业考
> 场景代码：[Homework/重构/第三轮-让重构安全的元知识/LegacySaveSystem.cs](Homework/重构/第三轮-让重构安全的元知识/LegacySaveSystem.cs)

---

## 一、遗留代码场景（~100 行）

```csharp
public static class SaveData
{
    public static int coin = 0;   // 全局金币
    public static int level = 1;  // 全局等级
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
    public void SaveGame(Inventory inv, string path)   // 一坨：序列化+写盘+打印
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

    public void LoadGame(Inventory inv, string path)   // 对称的重复分发
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

    public int TotalValue(Inventory inv)   // 依恋情结
    {
        int total = 0;
        foreach (var it in inv.items)
            total += it.value * it.amount;
        return total;
    }
}
```

## 二、毕业考任务（4 步）

1. **找坏味道**：列清单（至少 5 个），写进 `00-我的回答.md`
2. **找接缝 + 写特征测试**固化当前行为（最大接缝 = `System.IO.File`，直接读写真实磁盘）
3. **测试保护下小步重构**：一次一步，每步跑测试
4. **全部测试 PASS，验收提交**

铁律：行为、顺序、数值一律不变；**序列化格式字符串（`|atk:`、`Substring(4)`）是隐藏契约**，必须被特征测试锁住。

## 三、你的回答（待填写）

（等你交坏味道清单）

## 四、标准解/验收（待给出）

（重构完成后给出）

---

`[进度：阶段三-重构 → Day 15「综合实战（阶段考）」坏味道清单待交]`
