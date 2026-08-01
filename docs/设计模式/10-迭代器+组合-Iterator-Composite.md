# 迭代器模式 + 组合模式（Iterator + Composite）

> 来源：《设计模式》GoF 第 4 章 + 《Head First 设计模式》第 9 章

---

## 迭代器模式（Iterator）

### 一、书中定义

GoF 的定义：

> **"提供一种方法顺序访问一个聚合对象中的各个元素，而又不暴露其内部的表示。"**

Head First 的直觉版：**迭代器就是餐厅服务员——她不管你后厨用数组还是 List 存菜单，她只管一个一个念给你听。**

---

### 二、坏代码场景

你的游戏背包里存了道具，用 `List<Item>` 实现。战斗系统需要遍历背包找药水，商店系统需要遍历背包找可出售物品，UI 系统需要遍历背包显示所有道具：

```csharp
public class Inventory
{
    private List<Item> _items = new();

    // 三个系统都直接依赖 _items 的内部结构
    public List<Item> Items => _items;  // ← 暴露了内部实现
}

// 战斗系统
foreach (var item in player.Inventory.Items)
{
    if (item.Type == ItemType.Potion)
        UsePotion(item);
}

// 商店系统
foreach (var item in player.Inventory.Items)
{
    if (item.CanSell)
        Sell(item);
}
```

后来背包改成 `Dictionary<int, Item>`（按格子索引），三个系统的 `foreach` 全炸。

---

## 组合模式（Composite）

### 一、书中定义

GoF 的定义：

> **"将对象组合成树形结构以表示'部分-整体'的层次结构。组合模式使得客户对单个对象和组合对象的使用具有一致性。"**

Head First 的直觉版：**文件夹里可以放文件，也可以放文件夹——不管打开哪个，你用的都是同一个"打开"操作。**

---

### 二、坏代码场景

你的游戏 UI 系统有三种元素：

```csharp
public class Button { public void Render() { /* 画按钮 */ } }
public class Label  { public void Render() { /* 画文字 */ } }
public class Panel  { public List<Button> Buttons; public List<Label> Labels; public void Render() { /* 画面板 + 遍历子元素 */ } }
```

Button 和 Label 是叶子，Panel 是容器。但 Panel 只能装 Button 和 Label——Panel 里不能装 Panel。如果要实现 Window → Panel → SubPanel → Button 的嵌套，类型系统直接炸了。

---

## 问题

1. 迭代器：背包换数据结构，三个系统全炸——怎么做到「不管底层是 List 还是 Dictionary，遍历方式不变」？

2. 组合：Panel 里只能装 Button/Label，不能装 Panel——如果想统一用 `IUIElement` 接口，让 Panel 里可以装任意 IUIElement（包括 Panel 自己），接口怎么写？

3. 这两个模式为什么在 Head First 合为一章？迭代器和组合有什么天然联系？

---

## 你的回答（2026-07-27）

1. **创建独立于背包的迭代器，类似链表指针，或单独做数据管理器** ✅ — 迭代器的本质就是「不暴露内部结构，只提供遍历接口」。你说的 "itemPre → item → itemNext" 就是迭代器的指针模型。C# 的 `IEnumerator` / `foreach` 语法糖就是干这件事的。

2. **缓存 IElement 接口查特定组件（方便但性能开销），或像 Unity GameObject 绑定脚本解耦** ✅ — 前者就是组合模式的类继承方案，后者是 Unity 的组件方案。两套都成立。

3. **需要通用数据结构来迭代不同组件，但为了通用性进行了限制** ✅ — 这就是组合模式的核心矛盾。Head First 把两者放一章，因为：**组合创建树、迭代器遍历树。**

---

## 三、迭代器模式 —— 标准解

### 3.1 C# 已经帮你实现好了

在 C# 里，迭代器模式被内置为 `IEnumerable<T>` / `IEnumerator<T>` + `foreach` 语法糖。写游戏不需要从头写迭代器，但**知道它怎么工作的**让你更好地设计自己的集合类。

```csharp
// 背包 —— 不暴露内部结构，只暴露迭代能力
public class Inventory : IEnumerable<Item>
{
    private List<Item> _items = new();
    // 或 private Dictionary<int, Item> _items = new();
    // ← 改成 Dictionary，只要 IEnumerable 不变，调用方无感知

    // 返回迭代器 —— 这是 GoF 迭代器模式的 C# 版
    public IEnumerator<Item> GetEnumerator() => _items.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(Item item) => _items.Add(item);
    // 其他背包方法……
}

// 调用方 —— 不关心 _items 是 List 还是 Dictionary
foreach (var item in player.Inventory)
{
    if (item.Type == ItemType.Potion)
        UsePotion(item);
}
```

### 3.2 自定义迭代器 —— 你「itemPre → item → itemNext」的思路

你说的链表式遍历就是手动实现 IEnumerator：

```csharp
public class SkillTree
{
    public SkillNode Root;

    // 自定义迭代器：先序遍历技能树
    public IEnumerable<SkillNode> PreOrder()
    {
        if (Root == null) yield break;

        var stack = new Stack<SkillNode>();
        stack.Push(Root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return node;               // ← 你说的 item 指针
            foreach (var child in node.Children)
                stack.Push(child);           // ← 你说的 itemNext
        }
    }
}

// 使用
foreach (var skill in skillTree.PreOrder())
{
    Console.WriteLine(skill.Name);
}
```

---

## 四、组合模式 —— 标准解

### 4.1 你的 IElement 方案

你说的「缓存 IElement 接口，使用时再查特定组件」就是组合模式的类继承版：

```csharp
// ① 统一接口 —— 叶子、容器，都是一种 UI 元素
public interface IUIElement
{
    void Render();
    void Add(IUIElement child);  // ← 关键：容器和叶子共用同一个接口
}

// ② 叶子 —— Button
public class Button : IUIElement
{
    public string Text;

    public void Render() => Console.WriteLine($"  渲染 Button: {Text}");

    public void Add(IUIElement child)
    {
        throw new NotSupportedException();  // ← 叶子不能添加子节点
        // 你提到的"为了通用性进行了限制"就在这里
        // → GoF 认可这种做法，或提供默认空实现
    }
}

// ③ 叶子 —— Label
public class Label : IUIElement
{
    public string Text;

    public void Render() => Console.WriteLine($"  渲染 Label: {Text}");

    public void Add(IUIElement child) => throw new NotSupportedException();
}

// ④ 容器 —— Panel（可以装任何 IUIElement，包括 Panel 自己）
public class Panel : IUIElement
{
    private List<IUIElement> _children = new();

    public void Render()
    {
        Console.WriteLine("渲染 Panel 开始");
        foreach (var child in _children)
            child.Render();  // ← 递归渲染子元素
        Console.WriteLine("渲染 Panel 结束");
    }

    public void Add(IUIElement child)
    {
        _children.Add(child);
    }
}

// ⑤ 使用 —— 嵌套自如
var root = new Panel();
root.Add(new Button { Text = "开始游戏" });
root.Add(new Label  { Text = "版本 1.0" });

var subPanel = new Panel();
subPanel.Add(new Button { Text = "设置" });
subPanel.Add(new Button { Text = "退出" });

root.Add(subPanel);  // ← Panel 里装 Panel，无限嵌套

// 渲染整棵树
root.Render();
// 输出：
// 渲染 Panel 开始
//   渲染 Button: 开始游戏
//   渲染 Label: 版本 1.0
//   渲染 Panel 开始
//     渲染 Button: 设置
//     渲染 Button: 退出
//   渲染 Panel 结束
// 渲染 Panel 结束
```

### 4.2 你说的 Unity GameObject 方案

Unity 没走复合模式的类继承路线，而是用 **组件模式**：GameObject 就是那个统一的节点，Button/Text/Image 是挂在上面的组件。两种方案解决同一个问题——**部分和整体用同一套接口操作。**

---

## 五、组合模式的核心矛盾 —— 你第三条的直觉

你说的「为了通用性进行了限制」在 GoF 里叫**透明性 vs 安全性**：

| 方案 | 做法 | 优点 | 缺点 |
|------|------|------|------|
| **透明式** | `Add()` 放在接口里，叶子 throw 异常 | 客户端代码统一，不需要判断类型 | 运行时才发现「叶子不能 Add」 |
| **安全式** | `Add()` 只放在容器类里，接口里没有 | 编译期安全 | 客户端需要判断 `is Panel` 才能 Add |

你直觉到的「为了通用性进行了限制」就是透明式的代价。GoF 认为这两种都合理，选哪个取决于你的项目为什么时候报错（编译期 vs 运行时）。

---

## 六、迭代器 + 组合 = 串起来

你的第三条已经答了——组合创建树，迭代器遍历树：

```csharp
// 给 Panel 加迭代能力
public class Panel : IUIElement, IEnumerable<IUIElement>
{
    private List<IUIElement> _children = new();

    public IEnumerator<IUIElement> GetEnumerator() => _children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ... 其他不变
}

// 现在可以 foreach 遍历 Panel 的所有直接子节点
var root = BuildUI();
foreach (var child in root)
{
    // child 可能是 Button、Label，也可能是另一个 Panel……
    // 迭代器不关心，它只管"下一个是谁"
}
```

**组合模式让所有节点类型统一，迭代器让遍历方式统一。两者合一 = 递归遍历树。**

---

## 七、和前面模式的对比

| 模式 | 解决的问题 |
|------|-----------|
| 迭代器 | 怎么遍历集合而不暴露内部结构？ |
| 组合 | 怎么让单个对象和容器对象有统一接口？ |
| 迭代器 + 组合 | 怎么遍历**树形结构**中的每一个节点？ |
| 装饰器（对比）| 装饰器也是 1 包 1 的嵌套链，**组合是 1 包 N 的树** |

---

## 八、跨书关联

| 关联概念 | 来源 |
|------|------|
| 单一职责 —— 迭代器把遍历逻辑从集合类中分离 | 《敏捷》第 8 章 |
| OCP —— 换底层数据结构不改遍历代码 | 《敏捷》第 9 章 |
| 组合模式 vs 装饰器 —— 都是"嵌套同类型"，但一个是树、一个是链 | GoF 第 4 章 |
| Unity GameObject / Transform 层级 = 组合模式 | 业界实践 |

---

## 九、作业（预计 15 分钟）

用组合模式实现一个**战斗队伍的 Buff 系统**。一个队伍里可以有：

- **角色**（叶子）：受到 Buff 影响，`ApplyBuff()` 直接加到自己身上
- **小队**（容器）：包含多个角色或小队，`ApplyBuff()` 时把 Buff 下发给所有成员

```csharp
public interface ICombatUnit
{
    string Name { get; }
    void ApplyBuff(string buffName);
}

public class Character : ICombatUnit
{
    public string Name { get; init; }
    public void ApplyBuff(string buffName)
    {
        Console.WriteLine($"  {Name} 获得 {buffName}");
    }
}

public class Squad : ICombatUnit
{
    public string Name { get; init; }
    private List<ICombatUnit> _members = new();

    public void Add(ICombatUnit unit) { /* TODO */ }
    public void ApplyBuff(string buffName)
    {
        // TODO: 遍历 _members，每个成员都 ApplyBuff(buffName)
    }
}
```

要求：
1. 实现 `Squad.ApplyBuff()`（递归分发 Buff）
2. 构造一个两级嵌套：军团 → (前锋小队(战士, 骑士), 后卫小队(法师, 牧师))
3. 对军团施加 "攻击力提升" Buff，预期输出四个角色各收到一次

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 ✓ / ⑩迭代器+组合 ✓（作业 2026-08-02）]`
