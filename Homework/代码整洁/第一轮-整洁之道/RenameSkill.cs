using System;

namespace StudyNotes.Homework.CleanCode.Renaming
{
    /// <summary>
    /// 代码整洁 Day 1 作业：有意义的命名——只改名，不动任何逻辑
    /// 铁律：行为、顺序、数值一律不变（改名不改义）
    /// </summary>
    public class SkillData
    {
        public int dmg;        // TODO 改：伤害量还是治疗量？
        public float r;        // TODO 改：爆炸半径还是射程？（按你场景的语义定）
        public bool f;         // TODO 改：友伤开关？（布尔用 is- 前缀）
        public int cd;         // TODO 改：基础冷却（静态配置）还是剩余冷却（动态状态）？
    }

    public class SkillSystem
    {
        public void Use(int sk, Player p, Enemy e)     // TODO 改：sk → skillId
        {
            var d = GetD(sk, p);                        // TODO 改：CalcDamage(caster, skillId)
            e.HP -= d;
            if (e.HP <= 0) Kill(e, p);                  // TODO 改：副作用要暴露在名字里
        }

        private int GetD(int sk, Player p)
        {
            // 技能伤害计算（不改内容）
            return p.Atk * 2 + sk;
        }

        private void Kill(Enemy e, Player p)
        {
            // 实际做了三件事：扣血结算、掉落、经验（名字必须反映这些）
            e.HP = 0;
            p.Exp += e.ExpReward;
            DropLoot(e);
        }
    }

    public class Player { public int Atk; public int Exp; }
    public class Enemy { public int HP; public int ExpReward; public void DropLoot(Enemy e) { } }
}
