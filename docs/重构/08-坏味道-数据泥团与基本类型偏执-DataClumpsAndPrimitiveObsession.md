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

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 用**引入参数对象（Introduce Parameter Object）**把 x/y/z 收进一个 `Position` 类——**铁律：只拆不换**（数值、分支条件、调用顺序一律不动）
3. 骨架：[Homework/重构/Position.cs](Homework/重构/Position.cs)

---

`[进度：阶段三-重构 → Day 8「数据泥团+基本类型偏执」进行中]`
