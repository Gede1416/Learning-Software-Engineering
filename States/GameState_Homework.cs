namespace StudyNotes.States;

/// <summary>
/// 状态模式作业：游戏主菜单状态机
/// 来源：docs/设计模式/07-状态模式-State.md 作业
/// 状态流转：MainMenu → Playing ⇄ Paused → MainMenu
///           Playing → GameOver → MainMenu
/// </summary>

// ====== 状态接口 ======
public interface IGameState
{
    void Enter(GameManager gm);
    void Update(GameManager gm);
    void Exit(GameManager gm);
}

// ====== 四个状态类 ======

// 主菜单——点击"开始游戏"→ Playing
public class MainMenuState : IGameState
{
    public void Enter(GameManager gm)
    {
        Console.WriteLine("进入 MainMenu 状态");
    }

    public void Update(GameManager gm)
    {
        var playingBtnFlag = true; // 模拟点击"开始游戏"
        if (playingBtnFlag)
        {
            gm.SetState(new PlayingState());
        }
    }

    public void Exit(GameManager gm)
    {
        Console.WriteLine("退出 MainMenu 状态");
    }
}

// 游戏中——按 ESC → Paused / 角色死亡 → GameOver
public class PlayingState : IGameState
{
    public void Enter(GameManager gm)
    {
        Console.WriteLine("进入 Playing 状态");
    }

    public void Update(GameManager gm)
    {
        var playerDead = true;  // 模拟角色死亡
        var ESCBtn = true;      // 模拟 ESC 键

        if (playerDead)
        {
            gm.SetState(new GameOverState());
            return;
        }
        if (ESCBtn)
        {
            gm.SetState(new PausedState());
            return;
        }
    }

    public void Exit(GameManager gm)
    {
        Console.WriteLine("退出 Playing 状态");
    }
}

// 暂停——按 ESC → Playing / 点击"退出"→ MainMenu
public class PausedState : IGameState
{
    public void Enter(GameManager gm)
    {
        Console.WriteLine("进入 Paused 状态");
    }

    public void Update(GameManager gm)
    {
        var ESCBtn = true;    // 模拟 ESC 键
        var exitBtn = true;   // 模拟点击"退出"

        if (exitBtn)
        {
            gm.SetState(new MainMenuState());
            return;
        }
        if (ESCBtn)
        {
            gm.SetState(new PlayingState());
            return;
        }
    }

    public void Exit(GameManager gm)
    {
        Console.WriteLine("退出 Paused 状态");
    }
}

// 游戏结束——点击"返回主菜单"→ MainMenu
public class GameOverState : IGameState
{
    public void Enter(GameManager gm)
    {
        Console.WriteLine("进入 GameOver 状态");
    }

    public void Update(GameManager gm)
    {
        var backBtn = true; // 模拟点击"返回主菜单"
        if (backBtn)
        {
            gm.SetState(new MainMenuState());
        }
    }

    public void Exit(GameManager gm)
    {
        Console.WriteLine("退出 GameOver 状态");
    }
}

// ====== GameManager（Context）=====
public class GameManager
{
    private IGameState _currentState;

    public GameManager()
    {
        _currentState = new MainMenuState();
        _currentState.Enter(this);
    }

    public void SetState(IGameState newState)
    {
        _currentState.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }

    public void Update()
    {
        _currentState.Update(this);
    }

    // 运行当前状态一次更新（模拟帧循环）
    public void Tick()
    {
        Update();
    }
}

// ====== 思考题 ======
// 状态模式的 Update(ctx) 参数里传 Context，
// 会不会让状态和 Context 耦合过紧？
// 有没有更好的办法？（提示：接口隔离原则 ISP）
//
// 答案：抽象出 ISetState 接口，在 Update 中切换时调用 ISetState.SetState(...);
//       在初始化时注入 ISetState 接口，避免 State 依赖整个 GameManager。
//       状态实例也可以通过抽象工厂去创建，来避免或减少 GC。
// _____________________________________________________________
