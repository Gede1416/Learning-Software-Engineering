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

## 三、你的回答（2026-08-09，同步自 00-我的回答.md「阶段考」）

坏味道清单：
1. Inventory 只包含数据 没有与数据对应的操作方法（数据存储、总价值计算）
2. Item 初始化操作 没有包装到类里面
3. SaveData 的数据赋值（全局可变数据）
4. Item 明显的多态实现替换逻辑分支
5. Save Load 多种抽象方法耦合（基础数据读写 + item 数据读写）

### 验收（第 1 步 ✅ 通过 2026-08-09）
5 个全中且方向正确（纯数据类+依恋情结 / 构造封装 / 全局数据 / 多态替换 / 过长函数职责耦合）。补充两个漏的方向（不阻塞）：
- 神秘命名（Day 1）：`it`/`inv` 缩写、`SaveData` 全局类名不达意（存的不是"数据操作"）
- 魔法数字（Day 8）：`type` 1/2/3、`Substring(4)`/`Substring(3)` 的偏移量

## 四、标准解/验收（2026-08-09，全部 PASS）

### 第 2 步：特征测试（纠错 3 轮后全绿 ✅）
- 修复清单：`Path.GetTempFileName()` 临时文件、删 `InitTest` throw、**破坏现场断言**（清零后断言恢复 10/11）、背包逐项断言
- 暴露的遗留 bug：保存格式 `"coin:10"`（冒号）与加载匹配 `"coin"` 不一致 → Load 必崩（越界）
- 用户修法：**改保存格式**（`"coin:"` → `"coin|"`）——教学点：真实遗留项目中**存档格式是线上兼容契约**，正确姿势是「改解析不改格式」（`StartsWith("coin:")` + `Substring(5)`）；练习语境无旧存档可接受，但原则必须记住

### 第 3 步：小步重构（完成度高 ✅）
- `SaveData.ToSaveData/ToLoadSaveData`：基础数据读写归位
- `Item.ToSave/CreatByLoad/TotalValue/Equals`：物品序列化归位
- `Inventory.TotalValue`（委托 `it.TotalValue()`）+ Equals 修复（`i>=0`、`!Equals`）
- `LegacySaveSystem` 瘦身为编排器
- 流程点评：理想节奏 = 修解析 bug → 测试绿 → 再重构；用户「改格式+重构」一步到位——结果全绿但流程不规范（无保护飞行，好在测试补上后验证了行为）

### 剩余可选坏味道（未消，不阻塞收官）
- 魔法数字：`type` 1/2/3、`Substring(4)`/`Substring(3)` 偏移
- 全局数据：`SaveData` 仍 public static 裸字段
- 重复分发：`Item.ToSave` 与 `CreatByLoad` 两处 `type==1/2/3` 分支（Day 9 重复 switch）
- `SplitLine` 多余私有方法（惰性元素苗头）

---

`[进度：阶段三-重构 → Day 15「综合实战（阶段考）」坏味道清单待交]`
