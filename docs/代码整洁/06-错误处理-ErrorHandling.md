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

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 6「错误处理」苏格拉底问答中]`
