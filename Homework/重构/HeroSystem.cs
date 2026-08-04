using System;
using System.Collections.Generic;

namespace StudyNotes.Homework.Refactor.Hero
{
    /// <summary>
    /// 重构 Day 6 作业：坏味道「发散式变化 + 霰弹式修改」（Divergent Change / Shotgun Surgery）
    /// 任务：用「搬移函数/搬移语句」把散落三处的「英雄类型 → 数据/行为」收敛成一张表（Dictionary），
    ///       三个系统（Factory / AI / Icon）都查表。
    /// 铁律：只拆不换——数值、输出、行为一律不变。
    /// 场景：加一个新英雄要改 3 个文件
    /// </summary>
    public class Hero
    {
        public string Name;
        public int Hp;
    }

    // TODO: 加一个英雄要改 3 个文件（Factory / AI / Icon）。
    // 把三处「英雄类型 → 属性/行为」收敛成一张表，三个系统查表。
    public class HeroFactory
    {
        public Hero Create(string type)
        {
            switch (type)
            {
                case "战士": return new Hero { Name = "战士", Hp = 100 };
                case "法师": return new Hero { Name = "法师", Hp = 60 };
                default:     return new Hero { Name = "路人", Hp = 40 };
            }
        }
    }

    public class HeroAI
    {
        public void Update(Hero hero)
        {
            if (hero.Name == "战士")   { /* 冲锋 */ }
            else if (hero.Name == "法师") { /* 放风筝 */ }
        }
    }

    public class HeroIcon
    {
        public string GetIcon(Hero hero) => hero.Name switch
        {
            "战士" => "sword.png",
            "法师" => "staff.png",
            _ => "default.png"
        };
    }
}
