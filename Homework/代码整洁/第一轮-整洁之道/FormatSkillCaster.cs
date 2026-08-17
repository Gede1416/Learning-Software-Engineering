using System;

namespace StudyNotes.Homework.CleanCode.Formatting
{
    /// <summary>
    /// 代码整洁 Day 4 附加作业：技能释放系统格式重排（比 SaveManager 更乱）
    /// 要处理的格式问题：
    ///   1. CastSkill 一行挤了四个逻辑块（冷却检查/魔法检查/扣蓝/施放）——拆行
    ///   2. 字段、属性、方法混排——字段顶部
    ///   3. 方法顺序乱（GetSkill 夹在中间）——公开入口在上、细节沉底
    ///   4. 无空行分组——概念分组
    /// 铁律：只动格式，不动任何逻辑
    /// </summary>
    public class SkillCaster
    {
        private int mana;
        private int cooldownRemain;
        public void CastSkill(int skillId, Player target)
        {
            if (cooldownRemain > 0)
                return;
            var skill = GetSkill(skillId);
            if (mana < skill.Cost)
                return;

            mana -= skill.Cost;
            cooldownRemain = skill.Cooldown;
            skill.Apply(target);
        }
        public void Tick()
        {
            if (cooldownRemain > 0)
                cooldownRemain--;
        }
        public int Mana { get { return mana; } }
        public int CooldownRemain { get { return cooldownRemain; } }
        private Skill GetSkill(int id) { return SkillTable.Get(id); }
    }

    public class Skill
    {
        public int Cost;
        public int Cooldown;
        public void Apply(Player target) { }
    }
    public class Player { }
    public static class SkillTable
    {
        public static Skill Get(int id) => new Skill();
    }
}
