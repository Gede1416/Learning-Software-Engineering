# 数据泥团 + 基本类型偏执（Data Clumps / Primitive Obsession）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 9、10 位（合讲）
> 跨书联动：Day 4「过长参数列表」的武器「引入参数对象」在这里再次登场；值对象（Value Object）

---

## 一、坏代码场景

坐标三兄弟（x/y/z）到处飞——每个方法都要带一组：

```csharp
public class TeleportSystem
{
    // 传送：坐标三兄弟到处飞
    public void Teleport(Player player, int x, int y, int z)
    {
        player.X = x;
        player.Y = y;
        player.Z = z;
        if (IsSafe(x, y, z))
        {
            PlayPortalEffect(x, y, z);
        }
    }

    public bool IsSafe(int x, int y, int z) { return true; /* 检查区域 */ }

    public void PlayPortalEffect(int x, int y, int z) { /* 传送特效 */ }
}

public class SpawnSystem
{
    public void SpawnEnemy(int x, int y, int z)
    {
        // 又一组 x, y, z——和传送系统没有任何关系
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：x/y/z 出现在几个方法的签名里？想加一个「面向朝向 yaw」要改几个签名？
   传错顺序——把 y 和 z 写反——编译器拦吗？（Day 4 的参数对象还记得吗？）
   x/y/z 这三个值，是不是天生就是「一个东西」的三面？）

## 三、你的回答（2026-08-03）

1. 外部传入的参数过多，且 xyz 类型相同，顺序写错也不知道，很容易引起奇怪 bug 但不知道哪里错了
2. 想要创建一个对 xyz 整体修改的方法时，需要给每个需要的地方创建对应的方法
3. 抽离到一个类里面，不如把数据和方法做成类放在一起，去进行修改
4. 是一个整体的三个表现

## 四、标准解（2026-08-03 给出）

**判定**：✅ 第一轮即过（全场最佳）——四发全中：①类型相同顺序传错编译器不拦（Day 4 复用）②整体操作无家可归 ③数据+方法成类=值对象思维（超出最低要求）④一体三面=数据泥团定义。

**补全**（非纠错）：
- 第二个味道名字：**基本类型偏执（Primitive Obsession）**——用 int/float/string 表达领域概念（坐标/金币/血量），治法：以对象取代基本类型。
- 具体崩盘点：世界坐标↔格子坐标转换、地图边界检查——「坐标整体逻辑」无家可住，各处重复实现，改规则时霰弹枪（Day 6 联动）。

Fowler《重构》第 3 章「数据泥团」：总是一起出现的一组数据应收成一个对象（引入参数对象/封装记录 → 值对象）。

```csharp
public class Position
{
    public int X, Y, Z;
    public Position(int x, int y, int z) { X = x; Y = y; Z = z; }

    // 数据和方法住一起：位移、边界检查、格子转换都是它的职责
    public Position Offset(int dx, int dy, int dz) => new(X + dx, Y + dy, Z + dz);
}

public class TeleportSystem
{
    public void Teleport(Player player, Position pos)
    {
        player.Position = pos;
        if (IsSafe(pos)) PlayPortalEffect(pos);
    }
    public bool IsSafe(Position pos) { return true; }
    public void PlayPortalEffect(Position pos) { }
}

public class SpawnSystem
{
    public void SpawnEnemy(Position pos) { /* 同一套 Position */ }
}
```

加 yaw → 改 Position 一处；位移/边界/转换 → 住进 Position；顺序传错 → 类型不匹配编译器直接拦。

## 五、作业（预计 5-10 分钟）

1. ~~回答上面的问题~~ 概念题 ✅（第一轮通过 + 标准解）
2. 引入参数对象——**第一轮通过 ✅（Day 8 收官，2026-08-03）**：数值/分支/顺序零变化，四个签名统一收进 `Position`，无发明行为。Build 0 错误。
3. 骨架：[Homework/重构/第一轮-最常踩的坏味道/Position.cs](Homework/重构/第一轮-最常踩的坏味道/Position.cs)

---

`[进度：阶段三-重构 → Day 8「数据泥团+基本类型偏执」进行中]`
