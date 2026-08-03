using System;

namespace StudyNotes.Homework.Refactor.Cast
{
    /// <summary>
    /// 重构 Day 4 作业：坏味道「过长参数列表」（Long Parameter List）
    /// 任务：把 CastSkill 的 8 个参数用「引入参数对象」（Introduce Parameter Object）收拢。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：技能释放
    /// </summary>
    public class Enemy
    {
        public int Id;
        public int Hp;
        public void TakeDamage(int dmg) { Hp -= dmg; }
    }

    public static class FxManager { public static void Play(string fx, int x, int y, float angle) { } }
    public static class SoundManager { public static void Play(string clip) { } }

    public class SkillSystem
    {
        private bool SkillUnlocked(int skillId, int level, string source) => true;
        private Enemy FindEnemy(int targetId) => null;
        private int CalcDamage(int skillId, int level, int power) => skillId * level + power;

        // TODO: 8 个参数——哪几伙天生是一伙的？把它们包成一个对象传进来。
        public void CastSkill(int skillId, int level, int x, int y,
                              int targetId, float angle, int power, string source)
        {
            if (!SkillUnlocked(skillId, level, source)) return;
            if (targetId < 0) return;
            Enemy target = FindEnemy(targetId);
            if (target == null) return;
            int dmg = CalcDamage(skillId, level, power);
            target.TakeDamage(dmg);
            FxManager.Play("skill_" + skillId, x, y, angle);
            SoundManager.Play("cast_" + skillId);
        }
    }
}
