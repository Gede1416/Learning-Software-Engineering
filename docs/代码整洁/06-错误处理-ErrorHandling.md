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

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 6「错误处理」苏格拉底问答中]`
