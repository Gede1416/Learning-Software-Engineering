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

## 三、你的回答（2026-08-14，同步自 00-我的回答.md）

1. 变量应该在方法的上面；数据的校验应该和 Build 放在一起；Save 的调用应该放在两行
2. 代码的关键信息 变量缓存
3. ValidateData 和 build 相关；顺序：Save / Validate / Build

## 三·五、纠错记录（第 1 轮，2026-08-14）

- ✅ 第 1 题基本过：字段位置 ✓ / 校验归族 ✓ / 调用拆行 ✓；补漏：`SaveGame`/`LoadGame` 同族被 `BuildData` 隔开、空行乱用看不出分组
- ⚠️ 第 2 题偏了：新闻报式头条 = 公开入口（`SaveGame()`/`LoadGame()`），读者第一个问题「这个类能干什么」——变量是细节，在正文
- ⚠️ 第 3 题判断反了：`ValidateData(string raw)` 的参数 `raw`（原始数据）来自 **LoadGame 读盘**，不是 BuildData 构建——它属于加载族，该挨着 LoadGame
- 子问题已给，待回答

### 第 2 轮（2026-08-14）：补答含糊（给了排列顺序但未答出 Q2 头条 / Q3 raw 来源，且顺序自相矛盾）→ 直接给标准解

## 四、标准解（2026-08-14）

### 垂直格式五规则（《代码整洁之道》第 5 章）
1. **概念分组空行**：相关的一组紧挨，组间空行（读者按块扫读）
2. **字段在类顶部**：统一规则，不埋在方法中间
3. **新闻报式**：文件开头 = 公开入口（这个类能干什么）→ 细节往下
4. **垂直距离**：相关的挨着；**调用者在上、被调用者在下**
5. **同族放一起**：Save 族（SaveGame/BuildData/SaveFile）、Load 族（LoadGame/ValidateData）

### 正确布局

```csharp
public class SaveManager
{
    // 字段——类顶部
    public int Version = 1;
    public string SavePath;

    // 公开入口（头条）
    public void SaveGame()
    {
        var data = BuildData();
        SaveFile(data);
    }

    public void LoadGame()
    {
        var raw = ReadFile(SavePath);   // 读盘
        ValidateData(raw);              // raw 来自磁盘 → 加载族
    }

    // 细节——被调用者在调用者下方
    private string BuildData() { return "data"; }
    private void SaveFile(string data) { /* 写盘 */ }
    private string ReadFile(string path) { return ""; }
    private void ValidateData(string raw) { /* 校验 */ }
}
```

### 验收
- Q1 基本过（字段位置/校验归族/调用拆行）；补漏：同族隔开、空行分组
- Q2 头条 = 公开入口（SaveGame/LoadGame），非变量
- Q3 `raw` = LoadGame 读盘所得 → ValidateData 属加载族，挨着 LoadGame
- 作业（2026-08-14 ✅）：FormatSaveManager.cs 重排完全符合五规则——字段顶部/公开入口在上/细节沉底/同族紧挨且保持调用链顺序
- 附加作业（2026-08-14 ✅）：FormatSkillCaster.cs——CastSkill 四逻辑块拆行顺序未动、字段顶部、空行分组；纠错 1 轮（GetSkill 未沉底）后移入细节位，最终结构：字段→公开入口→属性→细节

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 4「格式」苏格拉底问答中]`
