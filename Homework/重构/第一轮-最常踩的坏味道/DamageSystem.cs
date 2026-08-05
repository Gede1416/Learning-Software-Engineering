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
        public Weapon Weapon = new();

        public int GetAtk()
        {
            return Atk + Weapon.Bonus;
        }
    }

    public class Enemy
    {
        public int Def;
        public int Level;
        public Armor Armor = new();

        public int GetDef()
        {
            return Def + Armor.Reduction;
        }

        public int GetDmg(Player player)
        {
            var atk = player.GetAtk();
            var def = GetDef();
            var dmg = atk - def;
            if (player.Level > Level)
                dmg += (player.Level - Level) * 2;
            return Math.Max(dmg, 1);
        }
    }
}
