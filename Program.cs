using StudyNotes.Models;
using StudyNotes.Commands;
using StudyNotes.Homework.Refactor.Level;
using StudyNotes.Homework.Refactor.Legacy;
using StudyNotes.Homework.CleanCode.Tests;

// ====== 命令模式：移动命令系统（撤销 + 重放）======
// 来源：设计模式/06-命令模式-Command.md 作业
// ===============================================

var player = new Character("勇者", new Vector2Int(0, 0));
var history = new CommandHistory();

// 移动到 (1, 0)
var move1 = new MoveCommand(player, player.Position, new Vector2Int(1, 0));
history.ExecuteCommand(move1);
Console.WriteLine($"当前位置: {player.Position}");  // 预期: (1, 0)

// 移动到 (1, 1)
var move2 = new MoveCommand(player, player.Position, new Vector2Int(1, 1));
history.ExecuteCommand(move2);
Console.WriteLine($"当前位置: {player.Position}");  // 预期: (1, 1)

// 撤销一次
history.Undo();
Console.WriteLine($"撤销后位置: {player.Position}");  // 预期: (1, 0)

// 预期输出：
// 当前位置: (1, 0)
// 当前位置: (1, 1)
// 撤销后位置: (1, 0)

// ====== 思考题 ======
// 移动命令的 Undo 是存起点坐标来回退。
// 那对敌人造成伤害的命令，要 Undo 需要存什么？
// 提示：回想你 Buff 系统的属性管道思路。
//
// 答案（写你的）：
// _____________________________________________________________

// ====== 重构 Day 14：LevelUpSystem 行为契约测试 ======
LevelUpTests.Run();

//结课测试
LegacySaveSystemTest.Run();

// ====== 代码整洁 Day 8：存档测试（F.I.R.S.T. 版）======
CleanSaveTests.Run();