namespace StudyNotes.Commands;

/// <summary>
/// 命令历史 —— 维护 Undo/Redo 双栈，支持执行、撤销和重做
/// </summary>
public class CommandHistory
{
    private readonly Stack<IMoveCommand> _undoStack = new();
    private readonly Stack<IMoveCommand> _redoStack = new();

    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// 执行命令并推入撤销栈，同时清空重做栈（新操作使旧重做链失效）
    /// </summary>
    public void ExecuteCommand(IMoveCommand cmd)
    {
        cmd.Execute();
        _undoStack.Push(cmd);
        _redoStack.Clear();
    }

    /// <summary>
    /// 撤销最近一次操作。撤销栈为空时无效果。
    /// </summary>
    public void Undo()
    {
        if (_undoStack.Count == 0) return;

        var cmd = _undoStack.Pop();
        try
        {
            cmd.Undo();
        }
        catch (Exception)
        {
            _undoStack.Push(cmd); // 撤销失败则回推，保持一致性
            return;
        }
        _redoStack.Push(cmd);
    }

    /// <summary>
    /// 重做最近一次撤销的操作。重做栈为空时无效果。
    /// </summary>
    public void Redo()
    {
        if (_redoStack.Count == 0) return;

        var cmd = _redoStack.Pop();
        try
        {
            cmd.Execute();
        }
        catch (Exception)
        {
            _redoStack.Push(cmd);
            return;
        }
        _undoStack.Push(cmd);
    }
}
