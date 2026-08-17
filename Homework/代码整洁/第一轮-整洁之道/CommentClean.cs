using System;

namespace StudyNotes.Homework.CleanCode.Comments
{
    /// <summary>
    /// 代码整洁 Day 3 作业：注释净化——逐条判断：删 / 留 / 改
    /// 铁律：注释改了，代码行为不能变
    /// </summary>
    public class Player
    {
        public int hp;   // 血量
        public int mp;   // 蓝量
        public int atk;  // 攻击
        public int MaxHp;

        public void Heal(int amount)
        {
            // 保证血量不超过上限
            hp = Math.Min(hp + amount, MaxHp);
            UiManager.Refresh(this);
        }

        // TODO: Goethe 2026::08:17 以后要加暴击
        public int GetAttack() => atk;
    }

    public static class UiManager { public static void Refresh(Player p) { } }
}
