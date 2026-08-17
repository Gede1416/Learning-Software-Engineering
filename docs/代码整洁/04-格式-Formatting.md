# 格式（Formatting）

> 来源：《代码整洁之道》Robert C. Martin 第 5 章
> 核心：格式是给读者的地图——垂直格式（空行/分组/顺序）、水平格式（缩进/行长）

---

## 一、坏代码场景

```csharp
public class SaveManager
{
    public void SaveGame() {
        var data = BuildData(); SaveFile(data);
    }
    private string BuildData() { return "data"; }
    private void SaveFile(string data) { /* 写盘 */ }

    public void LoadGame() { /* 读档 */ }
    private void ValidateData(string raw) { /* 校验 */ }

    public int Version = 1;
    public string SavePath;
}
```

## 二、问题（2026-08-14 布置）

1. 这段代码的格式乱在哪？（空行缺失 / 方法挤一行 / 字段混在方法中间 / 相关概念被拆散……逐处说）
2. **新闻报式（Newspaper Metaphor）**：报纸——头条摘要 → 细节。代码同理：一个文件开头应该是摘要，读者眼睛想先看到什么？
3. **垂直距离**：`ValidateData` 和谁相关？它应该挨着谁？`BuildData` 应该挨着谁？（提示：调用者在上、被调用者在下）

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 4「格式」苏格拉底问答中]`
