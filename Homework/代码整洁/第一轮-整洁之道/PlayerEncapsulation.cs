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
        private int _hp;
        private int _maxHp;
        private int _mana;
        private int _maxMana;

        public void TakeDamage(uint damage)
        {
            _hp -= (int)damage;
            if (IsDead())
            {
                _hp = 0;
                // 死亡消息
            }
        }

        public void Heal(uint heal)
        {
            if (_hp == _maxHp)
            {
                Console.WriteLine("player hp is Max Cant beheal");
                return;
            }
            if (IsDead())
            {
                Console.WriteLine("player is dead can`t heal");
                return;
            }
            _hp += (int)heal;
            if (_hp > _maxHp)
            {
                _hp = _maxHp;
            }
        }

        public void UseMana(uint castMana)
        {
            if (_mana == 0)
            {
                Console.WriteLine("mana is empty can`t be use");
                return;
            }
            bool flowControl = IsManaEnough(castMana);
            if (!flowControl)
            {
                return;

            }
            _mana -= (int)castMana;
        }

        private bool IsManaEnough(uint castMana)
        {
            bool res = true;
            if (_mana < castMana)
            {
                res = false;
            }
            return res;
        }


        private bool IsDead()
        {
            bool res = false;
            if (_hp <= 0)
            {
                res = true;
            }
            return res;
        }



        // ……无脑 getter/setter（待改造）

        // TODO: 私有化字段 + 行为接口 + 规则集中

    }
}
