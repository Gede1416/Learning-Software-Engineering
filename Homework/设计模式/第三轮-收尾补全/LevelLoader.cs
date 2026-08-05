using System;

namespace StudyNotes.Homework
{
    /// <summary>
    /// 模板方法模式作业：关卡加载管线
    /// 来源：docs/设计模式/09-模板方法-TemplateMethod.md 作业
    ///
    /// 骨架：ShowLoading → LoadAssets → SetupEnemies → PlayBGM → [Cutscene] → HideLoading
    /// </summary>

    // ====== 抽象基类 LevelLoader ======
    public abstract class LevelLoader
    {
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

        private void ShowLoading()
        {
            Console.WriteLine("显示 Loading 界面...");
        }

        private void HideLoading()
        {
            Console.WriteLine("隐藏 Loading 界面");
        }

        protected abstract void LoadAssets();
        protected abstract void SetupEnemies();
        protected abstract void PlayBGM();

        protected virtual bool HasCutscene() => false;

        protected virtual void PlayCutscene()
        {
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

        protected override bool HasCutscene() => true;

        protected override void PlayCutscene()
        {
            base.PlayCutscene();
            //播放视频
        }


    }

    // ====== 地牢关卡（无过场）======
    public class DungeonLevel : LevelLoader
    {
        protected override void LoadAssets()
        {
            // TODO
            Console.WriteLine("  加载地牢场景资源: 牢房 楼梯 ...");
        }

        protected override void SetupEnemies()
        {
            // TODO
            Console.WriteLine("  初始化敌人: 地牢守卫 ×5, 骷髅  ×3");
        }

        protected override void PlayBGM()
        {
            // TODO
            Console.WriteLine("  播放 BGM: Dungeon_Theme.mp3");
        }
    }

    // ====== 思考题 ======
    // ... 1. 直接修改管线 改动量大且违背 sop原则
    //      2. 创建一个管线子类 给需要细化的步骤添加子方法来进行步骤扩展
    //      3. 当需要扩展的方法过多 已经形成一个 庞大的 方法树时 使用 一个新的管线/组合模式进行抽离 
}
