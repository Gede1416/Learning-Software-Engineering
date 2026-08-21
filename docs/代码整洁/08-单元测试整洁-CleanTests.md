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

## 三、你的回答（2026-08-20，同步自 00-我的回答.md）

1. 测试输出不清楚，单是 PASS/FAIL 搞成测试名称+通过或失败；流程耦合 save load 应该是两个不同的测试，不然不好定位是 save 还是 load 的问题导致的错误
2. （并入 1）
3. 不清楚 F.I.R.S.T.

## 三·五、验收与补课（2026-08-20）

- ✅ Q1 两个核心全抓：命名不清 + **save/load 拆分成独立测试**（定位问题源）
- ⚠️ Q2 半对：输出要带测试名 ✓，但没到「断言自动判断」——`Console.WriteLine` + 人眼读 = 不自足验证（Self-Validating）
- Q3 请求讲解 → 标准解直接给出

## 四、标准解（2026-08-20）

### F.I.R.S.T.（《代码整洁之道》第 9 章）——逐条对上坏场景

| 字母 | 含义 | 坏场景怎么违背 |
|------|------|----------------|
| F | Fast 快速 | SaveGame 写真实磁盘——慢；测试应该毫秒级 |
| I | Independent 独立 | save+load 耦合在一个测试——改 Save 影响 Load 的结果 |
| R | Repeatable 可重复 | 依赖真实文件/全局状态——换机器、换顺序结果可能不同 |
| S | Self-Validating 自足验证 | `Console.WriteLine("pass")` 靠人眼读——测试必须自动说红/绿 |
| T | Timely 及时 | 测试应该和生产代码同步写（TDD 三定律） |

### 干净版（标准答案）

```csharp
public void SaveGame_保存后玩家数据可完整读回_血量一致()
{
    // Given：造一个 100 血的玩家
    var p = new Player { Hp = 100 };
    // When：保存再读取
    SaveGame(p);
    var p2 = LoadGame();
    // Then：断言——自动判断，红/绿
    Assert(p2.Hp == 100, "血量往返一致");
}

public void LoadGame_文件不存在时_返回默认玩家()
{
    Assert(LoadGame("/nonexist") == null, "缺失存档返回空");
}
```

要点：一个测试一个行为；Given-When-Then 结构；`Assert` 自动判断（你 Day 14 的 LevelUpTests 就是这种模式）

## 五、作业验收（2026-08-20，纠错第 1 轮）

- ✅ 结构全对：Save/Load/独立三个测试分开、命名带行为、Assert 自动判断
- ✅ 测试数据有区分度（playerTest1/playerTest2 数值不同）——这是能抓出 bug 的前提
- ⚠️ 全 FAIL 的根源：`Player.Equals` 第 25 行 `var maxHpEquals = this.MaxHp.Equals(other.Hp);`——**MaxHp 和 other.Hp 比**，复制粘贴错误（应 `other.MaxHp`）。无 ReferenceEquals 短路，同名对象比较也逐字段硬比 → 必 FAIL
- 教学点：测试当场抓到生产 bug——这正是 F.I.R.S.T. 的意义；若用 `==` 直接比引用，这个 bug 永远静默
- 小提示：Player 加 Equals 属测试辅助改动（骨架铁律是别改 Player）——可接受，但更贴近铁律的做法是测试里逐字段断言

### 第 2 轮（2026-08-20）✅ 通过
- `MaxHp.Equals(other.Hp)` → `other.MaxHp` 修正
- dotnet run：存档测试 3/3 PASS；顺带回补 Day 1 遗留债（RenameSkill.cs `DropLoot` → `e.DropLoot(e)`，构建恢复）

---

`[进度：阶段四-代码整洁 → Day 8「单元测试整洁」已完成，下一步 Day 9「类与系统」]`
