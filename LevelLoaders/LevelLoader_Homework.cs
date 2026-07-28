namespace StudyNotes.LevelLoaders;

/// <summary>
/// 模板方法模式作业：关卡加载管线
/// 来源：docs/设计模式/09-模板方法-TemplateMethod.md 作业
///
/// 骨架：ShowLoading → LoadAssets → SetupEnemies → PlayBGM → [Cutscene] → HideLoading
/// </summary>

// ====== TODO 1：实现抽象基类 LevelLoader ======
public abstract class LevelLoader
{
    // ★ 模板方法 —— 你来写完整流程
    public void Load()
    {
        ShowLoading();         // ① 显示 Loading

        LoadAssets();          // ② 加载资源（子类填空）

        SetupEnemies();        // ③ 初始化敌人（子类填空）

        PlayBGM();             // ④ 播 BGM（子类填空）

        // ⑤ 过场动画 —— 钩子控制
        if (HasCutscene())
        {
            PlayCutscene();
        }

        HideLoading();         // ⑥ 隐藏 Loading
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

// ====== TODO 2：实现森林关卡（有过场动画）======
public class ForestLevel : LevelLoader
{
    protected override void LoadAssets()
    {
        // TODO: 加载森林场景资源
        Console.WriteLine("  加载森林场景资源: 树木、草地、小溪...");
    }

    protected override void SetupEnemies()
    {
        // TODO: 初始化森林关卡的敌人
        Console.WriteLine("  初始化敌人: 树精 ×5, 狼 ×3");
    }

    protected override void PlayBGM()
    {
        // TODO: 播放森林 BGM
        Console.WriteLine("  播放 BGM: Forest_Theme.mp3");
    }

    // TODO: 森林关卡有过场 → 重写钩子
    // protected override bool HasCutscene() => ...
    // protected override void PlayCutscene() { ... }
}

// ====== TODO 3：实现地牢关卡（无过场）======
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

    // 无过场 → 不需要重写钩子（HasCutscene 默认 false）
}

// ====== 思考题 ======
// 如果有一个「Boss 关卡」，它的 LoadAssets 分为两阶段：
// ① 加载场景 → ② 显示 Boss 血条 → ③ 加载 Boss 资源
// 当前模板只给了一个 LoadAssets() 插槽，不够细。
// 怎么改基类让子类可以插更多步骤，同时不破坏原有子类？
//
// 你的答案：
// _____________________________________________________________
