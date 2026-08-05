using System;

namespace StudyNotes.Homework.Refactor.Position
{
    /// <summary>
    /// 重构 Day 8 作业：坏味道「数据泥团 + 基本类型偏执」（Data Clumps / Primitive Obsession）
    /// 任务：用「引入参数对象」（Introduce Parameter Object）把 x/y/z 收进 Position 类。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：坐标三兄弟到处飞
    /// </summary>
    public class Player
    {
        public int X;
        public int Y;
        public int Z;
    }

    // TODO: x/y/z 出现在几个方法的签名里？把它们收进一个 Position 类。
    public class TeleportSystem
    {
        // 传送：坐标三兄弟到处飞
        public void Teleport(Player player, int x, int y, int z)
        {
            player.X = x;
            player.Y = y;
            player.Z = z;
            if (IsSafe(x, y, z))
            {
                PlayPortalEffect(x, y, z);
            }
        }

        public bool IsSafe(int x, int y, int z) { return true; /* 检查区域 */ }

        public void PlayPortalEffect(int x, int y, int z) { /* 传送特效 */ }
    }

    public class SpawnSystem
    {
        public void SpawnEnemy(int x, int y, int z)
        {
            // 又一组 x, y, z——和传送系统没有任何关系
        }
    }
}
