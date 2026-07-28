namespace StudyNotes.Homework;

/// <summary>
/// 模板方法模式作业：关卡加载管线
/// 来源：docs/设计模式/09-模板方法-TemplateMethod.md 作业
///
/// 骨架：ShowLoading → LoadAssets → SetupEnemies → PlayBGM → [Cutscene] → HideLoading
/// </summary>

// ====== 抽象基类 LevelLoader ======
public abstract class LevelLoader
{
    // ★ 模板方法
    public void Load()
    {
        ShowLoading();

        LoadAssets();

        SetupEnemies();

        PlayBGM();

        if (HasCutscene())
        {
            PlayCutscene();
        }

        HideLoading();
    }

    // ==== 固定步骤（基类实现，子类不改）====
    private void ShowLoading()
    {
        Console.WriteLine("显示 Loading 界面...");
    }

    private void HideLoading()
    {
        Console.WriteLine("隐藏 Loading 界面");
    }

    // ==== 抽象步骤 —— 子类必须填空 ====
    protected abstract void LoadAssets();
    protected abstract void SetupEnemies();
    protected abstract void PlayBGM();

    // ==== 钩子方法 —— 子类可选重写 ====
    protected virtual bool HasCutscene() => false;

    protected virtual void PlayCutscene()
    {
        // 默认空实现，有过场的子类重写
    }
}

// ====== 森林关卡（有过场动画）======
public class ForestLevel : LevelLoader
{
    protected override void LoadAssets()
    {
        Console.WriteLine("  加载森林场景资源: 树木、草地、小溪...");
    }

    protected override void SetupEnemies()
    {
        Console.WriteLine("  初始化敌人: 树精 ×5, 狼 ×3");
    }

    protected override void PlayBGM()
    {
        Console.WriteLine("  播放 BGM: Forest_Theme.mp3");
    }

    // TODO: 森林关卡有过场 → 重写钩子
    // protected override bool HasCutscene() => true;
    // protected override void PlayCutscene() { Console.WriteLine("  播放森林过场动画..."); }
}

// ====== 地牢关卡（无过场）======
public class DungeonLevel : LevelLoader
{
    protected override void LoadAssets()
    {
        // TODO
    }

    protected override void SetupEnemies()
    {
        // TODO
    }

    protected override void PlayBGM()
    {
        // TODO
    }
}

// ====== 思考题 ======
// Boss 关卡 LoadAssets 分两阶段，当前模板插槽不够细。
// 怎么改基类让子类可以插更多步骤，同时不破坏原有子类？
//
// 你的答案：
// _____________________________________________________________
