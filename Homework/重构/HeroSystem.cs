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
        void HeroAIAction();
    }

    //战士
    public class WarriorAI : IHeroAI
    {
        public void HeroAIAction()
        {
            Console.WriteLine("战士AI");
        }
    }

    //法师
    public class MageAI : IHeroAI
    {
        public void HeroAIAction()
        {
            Console.WriteLine("法师AI");
        }
    }

    //路人
    public class CivilianAI : IHeroAI
    {
        public void HeroAIAction()
        {
            Console.WriteLine("路人AI");
        }
    }

    public struct HeroData
    {
        public static readonly Dictionary<string, HeroData> HeroTable = new()
        {
            {"战士", new HeroData{Name = "战士", Hp = 100, Icon = "sword.png"}},
            {"法师", new HeroData{Name = "法师", Hp = 60, Icon = "staff.png"}},
            {"路人", new HeroData{Name = "路人", Hp = 40, Icon = "default.png"}}
        };

        public string Name { get; set; }
        public int Hp { get; set; }
        public string Icon { get; set; }
    }

    public class Hero
    {
        public HeroData heroData = new();
        public IHeroAI heroAI = new CivilianAI();
    }

    // TODO: 加一个英雄要改 3 个文件（Factory / AI / Icon）。
    // 把三处「英雄类型 → 属性/行为」收敛成一张表，三个系统查表。
    public class HeroFactory
    {
        private readonly static Dictionary<string,Hero> keyValues = new Dictionary<string, Hero>
        {
            {"战士", new Hero{heroData = HeroData.HeroTable["战士"],heroAI = new WarriorAI()}},
            {"法师", new Hero{heroData = HeroData.HeroTable["法师"],heroAI = new MageAI()}},
            {"路人", new Hero{heroData = HeroData.HeroTable["路人"],heroAI = new CivilianAI()}}
        };

        public Hero Create(string type)
        {
            return keyValues.TryGetValue(type, out var hero) ? hero : keyValues["路人"];
        }
    }

    public class HeroAI
    {
        public void Update(Hero hero)
        {
            hero.heroAI.HeroAIAction();
        }
    }

    public class HeroIcon
    {
        public string GetIcon(Hero hero) => hero.heroData.Icon;
    }
}
