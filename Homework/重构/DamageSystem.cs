using System;

namespace StudyNotes.Homework.Refactor.Damage
{
    /// <summary>
    /// 重构 Day 7 作业：坏味道「依恋情结」（Feature Envy）
    /// 任务：用「搬移函数」（Move Function）把伤害计算搬回它该待的地方。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：伤害计算乱摸别人的字段
    /// </summary>
    public class Weapon
    {
        public int Bonus;
    }

    public class Armor
    {
        public int Reduction;
    }

    public class Player
    {
        public int Atk;
        public int Level;
        public Weapon Weapon;
    }

    public class Enemy
    {
        public int Def;
        public int Level;
        public Armor Armor;
    }

    // TODO: CalcPhysicalDamage 用的是谁的数据？这段逻辑更应该是谁的？
    public class DamageSystem
    {
        public int CalcPhysicalDamage(Player player, Enemy enemy)
        {
            int atk = player.Atk;
            int weaponBonus = player.Weapon.Bonus;
            int def = enemy.Def;
            int armor = enemy.Armor.Reduction;
            int dmg = atk + weaponBonus - def - armor;
            if (player.Level > enemy.Level)
                dmg += (player.Level - enemy.Level) * 2;
            return Math.Max(1, dmg);
        }
    }
}
