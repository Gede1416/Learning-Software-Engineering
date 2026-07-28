using System;

namespace StudyNotes.Homework
{
    /// <summary>
    /// 迭代器 + 组合模式作业：战斗队伍 Buff 系统
    /// 来源：docs/设计模式/10-迭代器+组合-Iterator-Composite.md 作业
    /// </summary>

    // ====== 统一接口 ======
    public interface ICombatUnit
    {
        string Name { get; }
        void ApplyBuff(string buffName);
    }

    // ====== 叶子：角色 ======
    public class Character : ICombatUnit
    {
        public string Name { get; init; }

        public void ApplyBuff(string buffName)
        {
            Console.WriteLine($"  {Name} 获得 {buffName}");
        }
    }

    // ====== TODO：容器 — 小队 ======
    public class Squad : ICombatUnit
    {
        public string Name { get; init; }
        private List<ICombatUnit> _members = new();

        public void Add(ICombatUnit unit)
        {
            // TODO
        }

        public void ApplyBuff(string buffName)
        {
            // TODO: 遍历 _members，每个成员递归调用 ApplyBuff
        }
    }

    // ====== 思考题 ======
    // 如果要求"只给前排战士加 Buff，法师和牧师不加"，
    // 当前递归分发 ApplyBuff 的方案怎么改？
    //
    // 你的答案：
    // _____________________________________________________________
}
