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
        public string Name => _weapon.Name;
    }

    public class Leader
    {
        private Equipment _equipment = new Equipment();
        public string EquipmentName => _equipment.Name;
    }

    public class Party
    {
        private Leader _leader = new Leader();
        public string LeaderEquipmentName => _leader.EquipmentName;
    }

    public class Player
    {
        private Party _party = new Party();
        public string PartyLeaderWeaponName=> _party.LeaderEquipmentName;
    }

    public class DungeonSystem
    {
        public void OnEnterDungeon(Player player)
        {
            string leaderWeapon = player.PartyLeaderWeaponName;

            if (leaderWeapon == "圣剑")
            {
                GrantBonus(player);
            }
        }

        public void GrantBonus(Player player) { /* 全队加成 */ }
    }
}
