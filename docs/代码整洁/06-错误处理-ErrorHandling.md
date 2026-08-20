# 错误处理（Error Handling）

> 来源：《代码整洁之道》Robert C. Martin 第 7 章
> 跨书联动：重构 Day 5 全局数据；《修改代码的艺术》（测试错误路径）

---

## 一、坏代码场景

存档加载——错误用返回码 + null：

```csharp
public int LoadGame(string path, out GameData data)
{
    if (!File.Exists(path)) return -1;          // -1 = 文件不存在
    var lines = File.ReadAllLines(path);
    if (lines.Length == 0) return -2;           // -2 = 空文件
    var raw = string.Join("\n", lines);
    var parsed = TryParse(raw);                  // 解析失败返回 null
    if (parsed == null) return -3;               // -3 = 解析失败
    data = parsed;
    return 0;                                    // 0 = 成功
}

var code = LoadGame(path, out var data);
if (code == 0) { StartGame(data); }
else if (code == -1) { ShowError("文件不存在"); }
else if (code == -2) { ShowError("存档损坏"); }
else { ShowError("未知错误"); }
```

## 二、问题（2026-08-18 布置）

1. 返回码方案有什么问题？-1/-2/-3 谁记得住？**调用方忘了检查返回值**会怎样？（错误静默 = 最坏情况）
2. `ReadAllLines` 抛 `IOException`（磁盘坏了）——返回码接得住吗？异常会怎样？（穿透调用栈，out 参数还是 null，调用方毫无防备）
3. 正确的错误处理长什么样？（Hint：异常 + 具体异常类型 + 异常信息带上下文）

## 三、你的回答（2026-08-18，同步自 00-我的回答.md）

1. 错误码包含信息太少；不看后续输出无法理解问题；错误提醒全靠外部处理；静默
2. 方法中断无法接收错误；异常爆出整个调用堆栈；不便与审查
3. LoadGame 内部处理；输出错误日志；try catch 来控制异常抛出

## 三·五、纠错记录（第 1 轮，2026-08-18）

- ✅ 第 1 题基本过：信息太少 / 调用方不检查就静默
- ⚠️ 第 2 题偏了：**调用栈是优点不是缺点**——哪一行炸的、谁调用的，一目了然（断点/日志/行号定位），这是异常优于返回码的核心
- ⚠️ 第 3 题半对：try-catch 方向对，但缺「异常类型 + 上下文信息」；且「内部处理」要小心——catch 后只记日志 = 错误又静默了（与 Q1 自相矛盾），正确兜底 = 顶层统一处理 / 重抛带上下文异常
- 子问题已给，待回答

### 第 2 轮（2026-08-18）✅
- 补答 1：「帮助定位」——调用栈 = 定位利器 ✓
- 补答 2：异常包装手法不熟悉 → 讲解后给标准解

## 四、标准解（2026-08-18）

### 异常包装（Exception Wrapping / Exception Translation）——《代码整洁之道》第 7 章

核心三步：**底层抛不吞 → catch 后包装重抛（带上下文）→ 顶层统一处理一次**

```csharp
public class SaveLoadException : Exception
{
    public SaveLoadException(string message, Exception inner) : base(message, inner) { }
}

public GameData LoadGame(string path)
{
    try
    {
        var lines = File.ReadAllLines(path);
        return Parse(string.Join("\n", lines));
    }
    catch (IOException ex)
    {
        // 包装：底层异常 → 带上下文的上层异常（不吞！）
        throw new SaveLoadException($"读取存档 {path} 失败", ex);
    }
    catch (FormatException ex)
    {
        throw new SaveLoadException($"存档 {path} 格式损坏", ex);
    }
}

// 顶层统一处理一次
try { StartGame(LoadGame(path)); }
catch (SaveLoadException ex)
{
    ShowError(ex.Message);                    // 用户看到：可读信息
    Log(ex);                                   // 开发看到：ex.InnerException 完整栈
}
```

### 为什么这是正解
- **不吞**：错误不静默（Q1 你抓的核心）
- **带上下文**：`IOException` → 「读存档 C:/save/slot1.sav 失败」
- **InnerException 保留原始栈**：用户可读 + 开发可查，两者兼得
- **顶层一次 catch**：不在每层 try-catch 堆防御（否则又是噪音）
- 联动：返回码还有个致命伤——**异常不可被忽略**（编译器强制），返回码可以被悄悄扔掉

### 补充：异常类型选型（2026-08-18 用户提问「throw 都有哪些类型怎么用」）

**内建异常（够用 80%）**：`ArgumentNullException`（参数 null）/ `ArgumentOutOfRangeException`（超范围）/ `InvalidOperationException`（状态不合法）/ `FormatException`（解析失败）/ `IOException` 家族（磁盘）/ `FileNotFoundException`

**自定义异常**（领域层核心武器）：`SaveLoadException : Exception`，构造函数带 message + inner

**绝不抛**：`NullReferenceException` / `IndexOutOfRangeException`——bug 症状，不是错误处理

**决策线**：
- 正常业务分支（蓝不够/金币不足/敌人已死）→ **if 判断，不用异常**（玩家可控 + 避免高频抛异常的性能灾难）
- 意外（不可预判/外部环境）→ 异常：参数→Argument 家族 / 状态→InvalidOperation / IO 解析→IOException、FormatException（或包装） / 领域意外→自定义

**两条铁律（第 7 章）**：
1. 异常用于意外情况，不用来当流程控制
2. 自定义异常按「调用方怎么处理」分类（一两个就够），不按「发生了什么」分类——`SaveLoadException` 一个类吃下所有存档错误

## 五、C# 异常语法补课（2026-08-20，作业前补）

> 前置：用户不熟悉 C# 异常语法，作业前补一课机制。全程绑定存档加载场景。

### 五件套
1. **throw**：抛出一个异常对象，当前函数立即停止，异常沿调用栈上飞——谁接住谁处理，没人接程序就崩（「错误不可能被静默丢弃」的机制来源）
2. **try-catch**：按异常类型匹配，越具体越靠前（`FileNotFoundException` ⊂ `IOException` ⊂ `Exception`）；最底下 `catch (Exception)` 是兜底网 = 「顶层统一处理一次」
3. **throw; vs throw ex;**：`throw;` 保留原始 StackTrace；`throw ex;` 把 StackTrace 重置成重抛行——「错误还是那个错误，案发现场的指纹被擦掉了」
4. **自定义异常**：继承 `Exception`，把原始异常作为 inner 传给 `base(message, inner)` → InnerException 链
5. **finally**：不管成败都执行（资源清理）

### 自检题（2 轮纠错）`catch (FormatException ex) { throw ex; }` 丢了什么？
- 第 1 答「丢掉了前面出栈的异常」→ 不精确：异常对象本身没丢，`throw ex` 抛的还是同一个对象
- 第 2 答「InnerException 没办法找到」→ 偏：没包装就没有 InnerException，`throw ex` 根本不碰 InnerException
- **标准解**：丢的是 **StackTrace**（原始炸点行号）——日志只剩重抛行，最初在哪炸的、谁调用的链路全没了

## 六、作业验收记录（LoadGameWithExceptions.cs，2026-08-20）

| 轮次 | 结果 | 问题 |
|------|------|------|
| 1 | 硬伤 2 + 概念 2 | ① `SaveLoadException` 未定义（编译不过）② `new SaveLoadException("文件不存在", path)` 第二参类型错（应传 Exception）③ **包装对象搞反**：`ReadAllLines`/`Parse` 裸奔，IOException/FormatException 逃逸不包装，inner 永远 null ④ 顶层 catch 后又 `throw;` 重抛 |
| 2 | 修一半 | ①④ 修好；②③ 仍在——包装链始终没建 |
| 3 | 功能通过 | 四场景全覆盖（`FileNotFoundException` 是 `IOException` 子类，一个 catch 接住文件不存在+磁盘坏）、包装链建立、顶层收尾一次；但新引入 `public string message;` 冗余字段遮蔽基类 `Message` 属性（CS0108） |
| 4 ✅ | 通过 | `message` 字段删除，改用 `ex.Message` + 顶层补 `ex.ToString()` 留完整链，收官 |

标准解关键段（第 3 轮给出）：catch 原始异常 → 包装带上下文：
```csharp
catch (IOException ex)     // FileNotFoundException 也是 IOException 子类，一次接住
{
    throw new SaveLoadException($"读取存档 {path} 失败", path, ex);
}
catch (FormatException ex)
{
    throw new SaveLoadException($"存档 {path} 格式损坏", path, ex);
}
```

> 教学点：**技术债会累积**——Day 1 留待回补的 RenameSkill.cs `DropLoot` 编译错（`e.DropLoot(e)`）+ `damge` 拼写 + `sk`/`p` 参数 + TODO 残留，2026-08-20 构建时暴露，堵住了整个项目构建。

---

`[进度：阶段四-代码整洁 → Day 6「错误处理」✅ 收官（2026-08-20）| Day 7「边界」作业布置中]`
