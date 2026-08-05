using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace StudyNotes.Homework.Refactor.Monster
{
    /// <summary>
    /// 重构 Day 9 作业：坏味道「重复的 switch + 循环」（Repeated Switches / Loops）
    /// 任务 1：用「以多态取代条件」（Replace Conditional with Polymorphism）把两个 switch 换成多态。
    /// 任务 2：用「以管道取代循环」（Replace Loop with Pipeline）把手写 for 换成 LINQ。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：怪物类型 switch 重复两处 + 背包手写 for
    /// </summary>
    public class Player
    {
        public int Hp;
    }

    public class Monster
    {
        public string Type;
        public int Hp;
    }

    // TODO 1: 两个 switch 都按 Type 分支——每个新怪物类型要改两处。
    // 把「按类型的行为」用多态表达：Monster 子类 + virtual 方法（数值、条件原样）。
    public class CombatSystem
    {
        public int CalcDamage(Monster m, Player player)
        {
            switch (m.Type)
            {
                case "Slime":  return 5;
                case "Wolf":   return 8 + (player.Hp < 30 ? 5 : 0);
                case "Dragon": return 25 + 10;
                default:       return 3;
            }
        }
    }

    public class RewardSystem
    {
        public int GetGold(Monster m)
        {
            switch (m.Type)
            {
                case "Slime":  return 10;
                case "Wolf":   return 20;
                case "Dragon": return 200 + m.Hp / 10;
                default:       return 5;
            }
        }
    }

    public class Item
    {
        public string Name;
        public int Value;
    }

    public class Backpack
    {
        public List<Item> Items = new();

        // TODO 2: 手写 for——用 LINQ 管道替换（返回结果不变）。
        public Item Find(string name)
        {
            var item =  Items.FirstOrDefault(item => item.Name == name);
            return item ?? new Item();
        }

        public int TotalValue()
        {
            return Items.Sum(item => item.Value);
        }
    }
}
