namespace StudyNotes.Models;

/// <summary>
/// 游戏角色，拥有名字和二维坐标位置
/// </summary>
public class Character
{
    public string Name { get; }
    public Vector2Int Position { get; private set; }

    public Character(string name, Vector2Int startPos)
    {
        Name = name;
        Position = startPos;
    }

    public void MoveTo(Vector2Int target)
    {
        Position = target;
    }
}
