using StudyNotes.Models;

namespace StudyNotes.Commands;

/// <summary>
/// 移动命令接口 —— 支持执行、撤销和查询起止位置
/// </summary>
public interface IMoveCommand
{
    void Execute();
    void Undo();
    Vector2Int GetFrom();
    Vector2Int GetTo();
}
