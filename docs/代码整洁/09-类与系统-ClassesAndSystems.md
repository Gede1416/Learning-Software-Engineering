# 类与系统（Classes and Systems）

> 来源：《代码整洁之道》Robert C. Martin 第 10、11 章
> 跨书联动：阶段一 SRP；重构 Day 11 大类/提炼类；Day 12 纯数据类

---

## 一、坏代码场景

游戏系统管理器——这个类有多少职责？

```csharp
public class GameSystem
{
    public void Init() { LoadConfig(); InitAudio(); InitInput(); }
    public void Update() { UpdateInput(); UpdateAI(); UpdatePhysics(); UpdateAudio(); }

    public void SaveGame() { /* 写档 */ }
    public void LoadGame() { /* 读档 */ }
    public void UnlockAchievement(string id) { /* 成就 */ }
    public void ShowMenu() { /* UI */ }
    public void PlayBgm(string name) { /* 音频 */ }
    // ……还有 40 个方法
}
```

## 二、问题（2026-08-20 布置）

1. `GameSystem` 有多少职责？数一数（配置/音频/输入/AI/物理/存档/成就/UI……）
2. 每个职责都有**自己的变化原因**——改 BGM 淡入动哪？加新敌人 AI 动哪？新存档格式动哪？它们挤在一个类里会发生什么？（SRP 类级——Day 11 的「大类」已经学过，这是复习）
3. 怎么拆？（Hint：提炼类——你把 GameManager 拆成 4 个系统类的 Day 11 作业）

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 9「类与系统」苏格拉底问答中]`
