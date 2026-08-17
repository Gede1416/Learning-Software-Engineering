using System;

namespace StudyNotes.Homework.CleanCode.Formatting
{
    /// <summary>
    /// 代码整洁 Day 4 作业：格式重排——只动格式，不动任何逻辑
    /// 规则：字段类顶部 / 公开入口在上 / 调用者在上被调用者在下 / 同族紧挨 / 概念分组空行
    /// 当前是乱序版，请重排成标准解布局
    /// </summary>
    public class SaveManager
    {
        private void SaveFile(string data) { /* 写盘 */ }

        public void SaveGame()
        {
            var data = BuildData();
            SaveFile(data);
        }

        public int Version = 1;
        public string SavePath;

        private void ValidateData(string raw) { /* 校验 */ }

        public void LoadGame()
        {
            var raw = ReadFile(SavePath);
            ValidateData(raw);
        }

        private string ReadFile(string path) { return ""; }
        private string BuildData() { return "data"; }
    }
}
