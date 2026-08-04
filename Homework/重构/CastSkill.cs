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

    //技能基础数据
    public class SkillData
    {
        public int skillId;
        public int level;//技能等级 要是玩家等级的话放在OwnerData中

    }

    //技能拥有者数据
    public class OwnerData
    {
        public int Id;
        public int power;
        public string source; //技能消耗资源
    }

    //释放表现数据
    public class LookData
    {
        public int x;
        public int y;
        public float angle;
    }

    //技能目标数据
    public class TargetData
    {
        public int targetId;
    }

    public static class FxManager { public static void Play(string fx, int x, int y, float angle) { } }
    public static class SoundManager { public static void Play(string clip) { } }

    public class SkillSystem
    {
        private bool SkillUnlocked(SkillData skillData, OwnerData ownerData) => true;
        private Enemy FindEnemy(TargetData targetData) => null;
        private int CalcDamage(SkillData skillData, OwnerData ownerData) => skillData.skillId * skillData.level + ownerData.power;

        // TODO: 8 个参数——哪几伙天生是一伙的？把它们包成一个对象传进来。
        public void CastSkill(TargetData targetData, LookData lookData, SkillData skillData, OwnerData ownerData)
        {
            if (!SkillUnlocked(skillData, ownerData)) return;
            if (targetData.targetId < 0) return;
            Enemy target = FindEnemy(targetData);
            if (target == null) return;
            int dmg = CalcDamage(skillData, ownerData);
            target.TakeDamage(dmg);
            FxManager.Play("skill_" + skillData.skillId, lookData.x, lookData.y, lookData.angle);
            SoundManager.Play("cast_" + skillData.skillId);
        }
    }
}
