using System;

namespace StudyNotes.Homework.MVC
{
    /// <summary>
    /// MVC 收官作业：双视图 + 可换控制器
    /// 来源：docs/设计模式/12-模型-视图-控制器-MVC.md 作业
    /// </summary>

    // ====== ① 模型（被观察者，不知道任何 UI 存在）======
    public class Player
    {
        public int Hp { get; private set; } = 100;
        public int Mp { get; private set; } = 100;

        public event Action<int> OnHpChanged;
        public event Action<int> OnMpChanged;

        public void UsePotion() { Hp = Math.Min(Hp + 30, 100); OnHpChanged?.Invoke(Hp); }
        public void CastSkill() { Mp -= 15; OnMpChanged?.Invoke(Mp); }
        public void TakeDamage(int dmg) { Hp -= dmg; OnHpChanged?.Invoke(Hp); }
    }

    // ====== ② 控制器（策略：可整体替换）======
    public interface IController
    {
        void HandleInput(Player player);
    }

    public class KeyboardController : IController
    {
        public void HandleInput(Player player)
        {
            // 模拟按键：按 H 喝药
            player.UsePotion();
        }
    }

    // ====== ③ 视图（观察者 + 组合）======
    public abstract class UIElement
    {
        public abstract void Draw();
    }

    public class UIText : UIElement
    {
        public string Text;
        public override void Draw() => Console.WriteLine($"  {Text}");
    }

    public class UIPanel : UIElement
    {
        private List<UIElement> _children = new();
        public void Add(UIElement e) => _children.Add(e);
        public override void Draw() { foreach (var c in _children) c.Draw(); }
    }

    public class PlayerStatusUI
    {
        private IController _controller;
        private UIPanel _root = new();
        private UIText _hpText = new() { Text = "HP: 100/100" };

        public PlayerStatusUI(Player player, IController controller)
        {
            _controller = controller;
            _root.Add(_hpText);
            player.OnHpChanged += hp => _hpText.Text = $"HP: {hp}/100";
        }

        public void Update(Player player)
        {
            _controller.HandleInput(player);
            _root.Draw();
        }
    }

    // ====== TODO 1：第二个视图 — 小地图迷你血条 ======
    // 订阅同一个 Player.OnHpChanged，输出一行 "迷你血条: HP=xx"
    public class MiniHpBar
    {
        private UIPanel _uiPanel = new();
        private UIMinHp _uiMinHp;
        public MiniHpBar(Player player)
        {
            // TODO: 订阅 player.OnHpChanged，每次变化打印一行迷你血条
            _uiMinHp = new UIMinHp(player);
            _uiPanel.Add(_uiMinHp);
            player.OnHpChanged += (_) =>
            {
                _uiMinHp.hp = _;
                _uiPanel.Draw();
            };
        }
    }

    public class UIMinHp : UIElement
    {
        public UIMinHp(Player player)
        {
            _player = player;
        }
        private Player _player;
        public int hp;
        public override void Draw() => Console.WriteLine($" {hp}  {_player.Hp}");
    }

    // ====== TODO 2：换控制器 — AI 自动喝药 ======
    // 每帧自动 UsePotion()（不依赖按键）
    public class AIController : IController
    {
        public void HandleInput(Player player)
        {
            // TODO: 每帧自动喝药
            player.UsePotion();
        }
    }

    public class AIPlayerManager
    {
        private Player _player;
        private IController _controller;

        public void Init(Player player)
        {
            _player = player;
            _controller = new AIController();
        }

        public void Update()
        {
            _controller.HandleInput(_player);
        }
    }

    // ====== 验收测试（骨架，导师补）======
    // var player = new Player();
    // var ui  = new PlayerStatusUI(player, new KeyboardController());
    // var mini = new MiniHpBar(player);          // 第二个视图
    //
    // player.TakeDamage(40);                     // 模型一次变化 → 两个视图各刷一次
    // ui.Update(player);                          // 键盘控制器 → 喝药
    //
    // var uiAI = new PlayerStatusUI(player, new AIController());  // 换控制器，视图代码零改动
    // uiAI.Update(player);                        // AI 自动喝药
    //
    // 预期输出（伪）：
    //   迷你血条: HP=60
    //   HP: 60/100
    //   HP: 90/100
    //   HP: 100/100   ← AI 控制器也把药喝了
}
