# 模型-视图-控制器（MVC）

> 来源：《Head First 设计模式》第 14 章 复合模式（MVC 是多种模式的组合）
> 本章为设计模式阶段收官章——前面所有模式在这里汇合

---

## 一、坏代码场景

你的游戏有个人物状态栏：左上角显示 HP/MP，玩家按 **H** 键喝药、按 **J** 键放技能，HP 归零显示「你死了」。你写了个类，一肩挑：

```csharp
public class PlayerStatusUI
{
    private Player _player;

    public void Update()
    {
        // ① 读数据 —— 从模型拿状态
        int hp = _player.Hp;
        int mp = _player.Mp;

        // ② 处理输入 —— 键盘快捷键直接改数据
        if (Input.GetKeyDown(KeyCode.H)) _player.UsePotion();
        if (Input.GetKeyDown(KeyCode.J)) _player.CastSkill();

        // ③ 画界面 —— 把状态画到屏幕上
        DrawText($"HP: {hp}/{_player.MaxHp}", new Vector2(10, 10));
        DrawText($"MP: {mp}/{_player.MaxMp}", new Vector2(10, 30));
        if (_player.Hp <= 0) DrawText("你死了", new Vector2(10, 50));
    }
}
```

## 二、问题

1. 这段代码有什么问题？当需求变化时——比如加第二个 UI（小地图上的迷你血条）、把键盘操作换成触屏按钮、或者加一段「复活提示」动画——**具体会在哪里崩盘**？

2. 「读数据」「处理输入」「画界面」这三件事挤在同一个类里——它们的**变化频率**一样吗？谁最常改？

3. 你已经学过：观察者模式（②）、策略模式（①）、组合模式（⑩）——如果分别把这三件事抽出去，**每个模式恰好负责拆哪一件**？想想「数据变化了谁要知道」「输入来了该问谁」「界面元素怎么组织」三个问题。

---

## 三、你的回答（日期）

（待填）

---

## 四、标准解

（待填）

---

## 五、作业

（待填）

---

`[进度：设计模式-①策略 ✓ / ②观察者 ✓ / ③装饰器 ✓ / ④工厂 ✓ / ⑤单例 ✓ / ⑥命令 ✓ / ⑦状态 ✓ / ⑧适配器+外观 ✓ / ⑨模板方法 ✓ / ⑩迭代器+组合 ✓ 核心完成 / ⑪代理模式 ✓ / ⑫MVC → 提问中]`
