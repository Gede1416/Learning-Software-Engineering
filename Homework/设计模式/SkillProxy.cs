namespace StudyNotes.Homework
{
    /// <summary>
    /// 代理模式作业：技能释放保护代理
    /// 来源：docs/设计模式/11-代理模式-Proxy.md 作业
    /// </summary>

    // ====== 共同接口 ======
    public interface ISkill
    {
        void Cast(Player caster, Enemy target);
    }

    // ====== 简单数据类 ======
    public class Player
    {
        public string Name = "勇者";
        public int Mana = 100;
    }

    public class Enemy
    {
        public int Hp = 100;
    }

    // ====== 真实技能：火球术（只关心伤害，不关心蓝量/冷却）======
    public class FireballSkill : ISkill
    {
        public void Cast(Player caster, Enemy target)
        {
            target.Hp -= 30;
            Console.WriteLine($"  {caster.Name} 施放火球术，敌人受到 30 点伤害（剩余 {target.Hp}）");
        }
    }

    // ====== TODO：代理 — 技能释放代理 ======
    public class SkillProxy : ISkill
    {
        private FireballSkill _real = new();
        private int _lastCastSecond = -99;   // 上次施放的游戏秒数

        public void Cast(Player caster, Enemy target)
        {
            // TODO 1: 蓝量 < 15 → 输出 "蓝量不足"，return
            // TODO 2: 距上次施放不足 3 秒（冷却中）→ 输出 "技能冷却中"，return
            // TODO 3: 检查通过 → 扣 15 蓝，记录 _lastCastSecond，委托 _real.Cast(caster, target)
            if (caster.Mana < 15)
            {
                Console.WriteLine("蓝量不足，无法释放");
                return;
            }
            if (_lastCastSecond < 3 && _lastCastSecond >= 0)
            {
                Console.WriteLine("技能正在冷却");
                return;
            }
            caster.Mana -= 15;
            _lastCastSecond = 0;

            _real.Cast(caster, target);
        }
    }

    // ====== 验收测试（骨架，导师补）======
    // var player = new Player { Mana = 100 };
    // var enemy  = new Enemy { Hp = 100 };
    // ISkill skill = new SkillProxy();
    //
    // skill.Cast(player, enemy);   // 成功：蓝 100 → 85，敌人 100 → 70
    // skill.Cast(player, enemy);   // 冷却中被拒（假设相隔 0 秒）
    // skill.Cast(player, enemy);   // 冷却中被拒
    // player.Mana = 5;             // 蓝打空
    // skill.Cast(player, enemy);   // 蓝量不足被拒
    //
    // 预期输出（伪）：
    //   勇者 施放火球术，敌人受到 30 点伤害（剩余 70）
    //   技能冷却中
    //   技能冷却中
    //   蓝量不足
}
