namespace StudyNotes.States;

/// <summary>
/// 状态模式作业：游戏主菜单状态机
/// 来源：docs/设计模式/07-状态模式-State.md 作业
/// 状态流转：MainMenu → Playing ⇄ Paused → MainMenu
///           Playing → GameOver → MainMenu
/// </summary>

// ====== TODO 1：实现状态接口 ======
public interface IGameState
{
    void Enter(GameManager gm);
    void Update(GameManager gm);
    void Exit(GameManager gm);
}

// ====== TODO 2：实现四个状态类 ======

// 主菜单——点击"开始游戏"→ Playing
public class MainMenuState : IGameState
{
    public void Enter(GameManager gm)
    {
        // TODO: 打印 "进入 MainMenu 状态"
    }

    public void Update(GameManager gm)
    {
        // TODO: 模拟按键 → 切换到 PlayingState
        // gm.SetState(new PlayingState());
    }

    public void Exit(GameManager gm)
    {
        // TODO
    }
}

// 游戏中——按 ESC → Paused / 角色死亡 → GameOver
public class PlayingState : IGameState
{
    public void Enter(GameManager gm)
    {
        // TODO
    }

    public void Update(GameManager gm)
    {
        // TODO: 模拟 ESC 键 → Paused
        // TODO: 模拟死亡 → GameOver
    }

    public void Exit(GameManager gm)
    {
        // TODO
    }
}

// 暂停——按 ESC → Playing / 点击"退出"→ MainMenu
public class PausedState : IGameState
{
    public void Enter(GameManager gm)
    {
        // TODO
    }

    public void Update(GameManager gm)
    {
        // TODO: ESC → Playing, 退出 → MainMenu
    }

    public void Exit(GameManager gm)
    {
        // TODO
    }
}

// 游戏结束——点击"返回主菜单"→ MainMenu
public class GameOverState : IGameState
{
    public void Enter(GameManager gm)
    {
        // TODO
    }

    public void Update(GameManager gm)
    {
        // TODO: 返回 → MainMenu
    }

    public void Exit(GameManager gm)
    {
        // TODO
    }
}

// ====== TODO 3：实现 GameManager（Context）=====
public class GameManager
{
    private IGameState _currentState;

    public GameManager()
    {
        // TODO: 初始状态 = MainMenu
    }

    public void SetState(IGameState newState)
    {
        // TODO: Exit 旧状态 → 替换 → Enter 新状态
    }

    public void Update()
    {
        _currentState?.Update(this);
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
// 答案：
// _____________________________________________________________
