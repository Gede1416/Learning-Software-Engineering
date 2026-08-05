using System;

namespace StudyNotes.Homework.Refactor.AltClasses
{
    /// <summary>
    /// 重构 Day 12 作业：坏味道「异曲同工的类 / 纯数据类 / 被拒绝的馈赠」
    /// 任务 1：异曲同工的类——Player.AddGold / Wallet.DepositGold 同一动作两套接口
    /// 任务 2：纯数据类——PlayerStats 全 public 字段，行为全在外面
    /// 任务 3：被拒绝的馈赠——FlyingEnemy 空实现父类方法
    /// 铁律：只拆不换——行为、顺序、数值一律不变。
    /// 注意：任务 1 的方案还没定稿（概念纠错中），先按你的判断动手
    /// </summary>

    // TODO 1: 异曲同工的类——「一处实现、一套接口」（判据：有货是重复 / 没货是中间人）


    public class Player
    {
        private int _gold;
        public void AddGold(int n) => _gold += n;
    }
    
    // TODO 2: 纯数据类——把散落在外的行为搬进类里
    public class PlayerStats
    {
        public int Hp; public int MaxHp; public int Atk;
        // 行为：判死、计算伤害……全在外面，等你搬进来
        public bool IsDeath => Hp <= 0;
        public int GetDamage => Atk * 10;
        //...
    }

    // TODO 3: 被拒绝的馈赠——空实现，考虑组合替换继承
    public class FlyingEnemy : IMove
    {
        public void Move() { /* 空中飞行 */ }
    }

    public class GroundEnemy : IMove, IChase
    {
        private IMove _move = new NormalMove();
        private IChase _chase = new NormalChase();
        public virtual void Move() => _move.Move();
        public virtual void Chase() => _chase.Chase();
    }

    public interface IMove
    {
        public void Move();
    }

    public class NormalMove : IMove
    {
        public void Move() { /* 地面移动 */ }
    }

    public interface IChase
    {
        public void Chase();
    }
    public class NormalChase : IChase
    {
        public void Chase() { /* 追击玩家 */ }
    }
}
