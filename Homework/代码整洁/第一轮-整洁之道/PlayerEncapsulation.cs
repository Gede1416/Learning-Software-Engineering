using System;

namespace StudyNotes.Homework.CleanCode.Encapsulation
{
    /// <summary>
    /// 代码整洁 Day 5 作业：封装改造——把 PlayerData 从「裸数据 + 无脑 getter/setter」
    /// 改造成「私有字段 + 行为接口」。
    /// 要求：
    ///   1. 字段私有化（_ 前缀）
    ///   2. 暴露行为：TakeDamage / Heal / UseMana / IsDead / IsManaEnough(int cost)
    ///   3. 规则集中：判死、血/蓝上限、死亡处理全部在类内
    ///   4. 外部调用全部改为行为式（不能再出现 if (x.GetHp() <= 0) 这类散落规则）
    /// 铁律：数值规则不变（判死 = Hp <= 0，上限 = Max 夹取）
    /// </summary>
    public class PlayerData
    {
        public int Hp;
        public int MaxHp;
        public int Mana;
        public int MaxMana;

        public int GetHp() { return Hp; }
        public void SetHp(int value) { Hp = value; }
        // ……无脑 getter/setter（待改造）

        // TODO: 私有化字段 + 行为接口 + 规则集中
    }
}
