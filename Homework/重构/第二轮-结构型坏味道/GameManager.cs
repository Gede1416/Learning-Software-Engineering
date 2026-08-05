using System;

namespace StudyNotes.Homework.Refactor.Manager
{
    /// <summary>
    /// 重构 Day 11 作业：坏味道「大类 + 惰性元素」（Large Class / Lazy Element）
    /// 任务 1：用「提炼类」（Extract Class）把 GameManager 按职责拆开（输入/音频/成就/存档各一伙）。
    /// 任务 2：惰性元素 ScoreDisplay 内联或删除。
    /// 铁律：只拆不换——行为、顺序、数值一律不变。
    /// 场景：3000 行上帝类 + 空壳类
    /// </summary>
    public class GameManager
    {
        // 什么都是它的：输入、音频、成就、存档……3000 行
        public int Score;
        private bool _paused;

        public void Update()
        {
            HandleInput();
            UpdateAudio();
            CheckAchievements();
            SaveIfNeeded();
        }

        public void HandleInput() { /* 读手柄/键盘 */ }
        public void UpdateAudio() { /* 音量、BGM 切换 */ }
        public void CheckAchievements() { /* 分数成就 */ }
        public void SaveIfNeeded() { /* 写档 */ }
    }

    // TODO 2: 惰性元素——什么都不干。内联或删除。
    public class ScoreDisplay
    {
        public void Refresh() { }
    }
}
