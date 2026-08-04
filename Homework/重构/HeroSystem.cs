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
    
    public interface IHeroAI
    {
        void HeroAI();
    }

    //战士
    public class WarriorAI : IHeroAI
    {
        public void HeroAI()
        {
            Console.WriteLine("战士AI");
        }
    }

    //法师
    public class MageAI : IHeroAI
    {
        public void HeroAI()
        {
            Console.WriteLine("法师AI");
        }
    }

    public class Hero
    {
        public string Name;
        public string icon;
        public int Hp;
        public IHeroAI AI;
    }

    // TODO: 加一个英雄要改 3 个文件（Factory / AI / Icon）。
    // 把三处「英雄类型 → 属性/行为」收敛成一张表，三个系统查表。
    public class HeroFactory
    {
        public Hero Create(string type)
        {
            switch (type)
            {
                case "战士": return new Hero { Name = "战士", icon = "sword.png", Hp = 100, AI = new WarriorAI() };
                case "法师": return new Hero { Name = "法师", icon = "staff.png", Hp = 60, AI = new MageAI() };
                default:     return new Hero { Name = "路人", icon = "default.png", Hp = 40, AI = new WarriorAI() };
            }
        }
    }

    public class HeroAI
    {
        public void Update(Hero hero)
        {
            hero.AI.HeroAI();
        }
    }

    public class HeroIcon
    {
        public string GetIcon(Hero hero) => hero.icon;
    }
}
