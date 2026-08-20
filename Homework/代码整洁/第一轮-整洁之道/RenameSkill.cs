using System;

namespace StudyNotes.Homework.CleanCode.Renaming
{
    /// <summary>
    /// 代码整洁 Day 1 作业：有意义的命名——只改名，不动任何逻辑
    /// 铁律：行为、顺序、数值一律不变（改名不改义）
    /// </summary>
    public class SkillData
    {
        public int damage;       
        public float range;       
        public bool canHitFriend;       
        public int baseCoolDown;        
    }

    public class SkillSystem
    {
        public void Use(int skillId, Player player, Enemy enemy)     // TODO 改：sk → skillId
        {
            var damge = GetSkillDamage(skillId, player);                        // TODO 改：CalcDamage(caster, skillId)
            enemy.HP -= damge;
            if (enemy.HP <= 0) HpToZeroDropLootGetExp(enemy, player);                  // TODO 改：副作用要暴露在名字里
        }

        private int GetSkillDamage(int sk, Player p)
        {
            // 技能伤害计算（不改内容）
            return p.Atk * 2 + sk;
        }

        private void HpToZeroDropLootGetExp(Enemy e, Player p)
        {
            // 实际做了三件事：扣血结算、掉落、经验（名字必须反映这些）
            e.HP = 0;
            p.Exp += e.ExpReward;
            e.DropLoot(e);
        }
    }

    public class Player { public int Atk; public int Exp; }
    public class Enemy { public int HP; public int ExpReward; public void DropLoot(Enemy e) { } }
}
