# 大类 + 惰性元素（Large Class / Lazy Element）

> 来源：《重构：改善既有代码的设计》Martin Fowler 第 3 章 —— 坏味道清单第 16、17 位（合讲）
> 跨书联动：《敏捷》SRP——一个类一个变化原因；提炼类（Extract Class）/ 内联（Inline）

---

## 一、坏代码场景

3000 行的上帝类——输入、音频、成就、存档全在它肚子里：

```csharp
public class GameManager
{
    // 什么都是它的：输入、音频、成就、存档……3000 行
    public int Score;
    private bool _paused;

    public void Update()
    {
        HandleInput();       // 输入处理
        UpdateAudio();       // 音频控制
        CheckAchievements(); // 成就检查
        SaveIfNeeded();      // 存档
    }

    public void HandleInput() { /* 读手柄/键盘 */ }
    public void UpdateAudio() { /* 音量、BGM 切换 */ }
    public void CheckAchievements() { /* 分数成就 */ }
    public void SaveIfNeeded() { /* 写档 */ }
}
```

旁边还有一个「惰性元素」——什么都不干的类：

```csharp
public class ScoreDisplay
{
    public void Refresh() { }   // 空的——什么都不干
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时，具体会在哪里崩盘？
   （提示：改音频逻辑要进 GameManager 翻几千行；改存档也要进 GameManager——**每一个**功能的变化都在同一个类里——这是 Day 6「发散式变化」的极限形态（SRP 最彻底违背：一个类装着所有变化原因）。
   类里哪些字段/方法天然是一伙的？（输入 / 音频 / 成就 / 存档）——一伙的成员怎么搬出去？
   `ScoreDisplay.Refresh()` 空方法——一个什么都不干的类，是「未来会用到」的自我安慰还是真实需求？）

## 三、你的回答（待填写）

（2026-08-03 布置，等你回答）

## 四、标准解（待给出）

（回答后给出）

## 五、作业（预计 5-10 分钟）

1. 回答上面的问题，写进 `00-我的回答.md`
2. 用**提炼类（Extract Class）**把 GameManager 按职责拆开（输入/音频/成就/存档各一伙）——**铁律：只拆不换**（行为、顺序、数值一律不变）；惰性元素 `ScoreDisplay` 内联或删除
3. 骨架：[Homework/重构/第二轮-结构型坏味道/GameManager.cs](Homework/重构/第二轮-结构型坏味道/GameManager.cs)

---

`[进度：阶段三-重构 → Day 11「大类+惰性元素」进行中]`
