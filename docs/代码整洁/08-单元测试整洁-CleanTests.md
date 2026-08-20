# 单元测试整洁（Clean Tests）

> 来源：《代码整洁之道》Robert C. Martin 第 9 章
> 跨书联动：重构 Day 14 测试工作流；Day 15 特征测试

---

## 一、坏代码场景

存档系统的测试——它坏在哪？

```csharp
public void TestSave()
{
    var p = new Player();
    p.Hp = 100;
    SaveGame(p);
    var p2 = LoadGame();
    var ok = p2.Hp == 100;
    Console.WriteLine(ok ? "pass" : "fail");    // 输出靠人眼
}
```

## 二、问题（2026-08-20 布置）

1. 这个测试坏在哪？（命名 / 断言方式 / 独立性——逐条说）
2. 测试跑完输出 "pass"——你真的放心吗？**测试失败时你能立刻知道哪个行为坏了吗**？（提示：自足验证 Self-Validating——测试应该自己说 PASS/FAIL，不是靠人眼读输出）
3. F.I.R.S.T. 是什么？逐条对上这个测试，它违背了哪几条？

## 三、你的回答（待填写）

（等你回答）

## 四、标准解（待给出）

（回答后给出）

---

`[进度：阶段四-代码整洁 → Day 8「单元测试整洁」苏格拉底问答中]`
