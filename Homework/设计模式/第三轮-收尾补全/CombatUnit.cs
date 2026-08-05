using System;

namespace StudyNotes.Homework
{
    /// <summary>
    /// 迭代器 + 组合模式作业：战斗队伍 Buff 系统
    /// 来源：docs/设计模式/10-迭代器+组合-Iterator-Composite.md 作业
    /// </summary>

    public enum CombatUnitType
    {
        NONE = 0,
        ZHANSHI = 1, // 战士
        FASHI = 2, // 法师
        MUSHI = 3 // 牧师
    }

    public enum WeizhiEnum
    {
        NONE = 0,
        Qianpai = 1,
        ZhongPai = 2,
        Houpai = 3
    }

    /**
    * 关于类型buff添加问题
    * 我想 是如果简单几个 单维度 类型 直接填表按照类型获取可以添加的buff 或者不可以添加的buff
    * 多维度的话 可能需要使用 装饰器模式 创建构造可以使用buff接口 定义不同种单一类型的接口
    * 再获取角色状态判断 角色处于哪种状态再进行组合
    * 结合工厂和装饰器模式 超过本篇范围 我先不实现？
    */


    // ====== 统一接口 ======
    public interface ICombatUnit
    {
        string Name { get; }
        void ApplyBuff(string buffName);
    }

    public interface ICanAddBuff
    {
        void SetCanAddBuff(HashSet<string> canAddBuff);
        bool CanAddBuff(string buffName);
    }



    // ====== 叶子：角色 ======
    public class Character : ICombatUnit, ICanAddBuff
    {
        private HashSet<string> _canAddBuff = new();
        public string Name { get; init; }

        public void SetCanAddBuff(HashSet<string> canAddBuff)
        {
            _canAddBuff = canAddBuff;
        }
        public bool CanAddBuff(string buffName)
        {
            var canAdd = _canAddBuff.Contains(buffName);
            return canAdd;
        }

        public void ApplyBuff(string buffName)
        {
            if (!CanAddBuff(buffName))
                Console.WriteLine($"{Name}can`t add {buffName}");
            else
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
            _members.Add(unit);
        }

        public void ApplyBuff(string buffName)
        {
            // TODO: 遍历 _members，每个成员递归调用 ApplyBuff
            foreach (var unit in _members)
            {
                unit.ApplyBuff(buffName);
            }
        }
    }

    // ====== 思考题 ======
    // 如果要求"只给前排战士加 Buff，法师和牧师不加"，
    // 当前递归分发 ApplyBuff 的方案怎么改？
    //
    // 你的答案：
    // 容器分类 职业容器 按照职业应用
    // 接口 实现判断 buff再添加
    // 问题是如果添加不同的buff都要继续进行判断
    // 能不能把buff变成 抽象接口 应用buff 和 buff添加前提判断
    // _____________________________________________________________

    // ====== 验收测试（骨架，导师补）======
    // 构造：军团 → (前锋小队(战士, 骑士), 后卫小队(法师, 牧师))
    // 施加 "攻击力提升"，预期四个角色各收到一次。
    // 验证方式：把下面代码复制到 Program.cs 的 Main 中，dotnet run。
    //
    // var legion = new Squad { Name = "军团" };
    // var vanguard = new Squad { Name = "前锋小队" };
    // vanguard.Add(new Character { Name = "战士" });
    // vanguard.Add(new Character { Name = "骑士" });
    // var rear = new Squad { Name = "后卫小队" };
    // rear.Add(new Character { Name = "法师" });
    // rear.Add(new Character { Name = "牧师" });
    // legion.Add(vanguard);
    // legion.Add(rear);
    // legion.ApplyBuff("攻击力提升");
    //
    // 预期输出：
    //   战士 获得 攻击力提升
    //   骑士 获得 攻击力提升
    //   法师 获得 攻击力提升
    //   牧师 获得 攻击力提升
}
