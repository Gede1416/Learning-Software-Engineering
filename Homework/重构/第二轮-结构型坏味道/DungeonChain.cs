using System;

namespace StudyNotes.Homework.Refactor.Dungeon
{
    /// <summary>
    /// 重构 Day 10 作业：坏味道「消息链 + 中间人」（Message Chains / Middle Man）
    /// 任务：用「隐藏委托」（Hide Delegate）把消息链压扁。
    /// 铁律：只拆不换——数值、分支条件、调用顺序一律不动。
    /// 场景：player.GetParty().GetLeader().GetEquipment().GetWeapon().Name
    /// </summary>
    public class Weapon
    {
        public string Name;
    }

    public class Equipment
    {
        private Weapon _weapon = new Weapon { Name = "铁剑" };
        public Weapon GetWeapon() => _weapon;   // 中间人：只会转发
    }

    public class Leader
    {
        private Equipment _equipment = new Equipment();
        public Equipment GetEquipment() => _equipment;   // 中间人
    }

    public class Party
    {
        private Leader _leader = new Leader();
        public Leader GetLeader() => _leader;
    }

    public class Player
    {
        private Party _party = new Party();
        public Party GetParty() => _party;
    }

    // TODO: 一条链点到 5 层。把「找队长的武器名」压成一个委托：Player.GetLeaderWeaponName()
    public class DungeonSystem
    {
        public void OnEnterDungeon(Player player)
        {
            string leaderWeapon = player
                .GetParty()
                .GetLeader()
                .GetEquipment()
                .GetWeapon()
                .Name;

            if (leaderWeapon == "圣剑")
            {
                GrantBonus(player);
            }
        }

        public void GrantBonus(Player player) { /* 全队加成 */ }
    }
}
