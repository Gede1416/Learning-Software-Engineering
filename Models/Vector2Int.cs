namespace StudyNotes.Models;

/// <summary>
/// 二维整数坐标
/// </summary>
public struct Vector2Int
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}
