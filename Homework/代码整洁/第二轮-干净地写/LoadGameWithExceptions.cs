using System;

namespace StudyNotes.Homework.CleanCode.ErrorHandling
{
    /// <summary>
    /// 代码整洁 Day 6 作业：返回码 → 异常包装改造
    /// 把 LoadGame 从「返回码 + null」改造成「异常 + 包装」：
    ///   1. 定义 SaveLoadException（带 message + inner）
    ///   2. LoadGame 改为：成功返回 GameData；失败 throw SaveLoadException（带路径上下文）
    ///   3. 顶层调用：try-catch 一次，用户可读信息 + 日志留 InnerException
    /// 铁律：错误场景必须全部覆盖——文件不存在 / 空文件 / 解析失败 / 磁盘 IO 异常
    /// </summary>
    public class GameData { public string Raw; }

    public static class TryParse
    {
        public static GameData Parse(string raw)
        {
            if (string.IsNullOrEmpty(raw)) throw new FormatException("空内容");
            return new GameData { Raw = raw };
        }
    }

    public class SaveLoadException : Exception
    {
        public SaveLoadException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class SaveSystem
    {
        // TODO 1: 改造 LoadGame —— 去掉返回码，用异常包装
        public void LoadGame(string path, out GameData data)   // 旧签名，待改
        {
            data = default;
            try
            {
                var lines = System.IO.File.ReadAllLines(path);
                data = TryParse.Parse(string.Join("\n", lines));
            }
            catch (IOException ex)
            {
                throw new SaveLoadException($"路径错误{path}", ex);
            }
            catch (FormatException ex)
            {
                throw new SaveLoadException($"空存档{path}", ex);
            }
        }

        // TODO 2: 顶层调用处 —— try-catch 一次
        public void BootGame(string path)
        {
            try
            {
                LoadGame(path, out var data);
                StartGame(data);
            }
            catch (SaveLoadException ex)
            {
                ShowError(ex.Message);
                ShowError(ex.ToString());
            }
        }

        private void StartGame(GameData d) { }
        private void ShowError(string msg) { Console.WriteLine(msg); }
    }
}
