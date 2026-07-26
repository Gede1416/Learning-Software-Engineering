using StudyNotes.Models;

namespace StudyNotes.Commands;

/// <summary>
/// 具体移动命令：记录起点和终点，执行即移动到目标，撤销即回到起点
/// </summary>
public class MoveCommand : IMoveCommand
{
    private readonly Character _character;
    private readonly Vector2Int _from;
    private readonly Vector2Int _to;

    public MoveCommand(Character character, Vector2Int from, Vector2Int to)
    {
        _character = character;
        _from = from;
        _to = to;
    }

    public void Execute()
    {
        _character.MoveTo(_to);
    }

    public void Undo()
    {
        _character.MoveTo(_from);
    }

    public Vector2Int GetFrom() => _from;
    public Vector2Int GetTo() => _to;
}
