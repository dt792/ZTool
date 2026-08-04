// ZTool 使用示例入口
using ZTool.Usages;

Console.WriteLine("=== ZTool 使用示例 ===\n");

// ==================== Bases 基础工具 ====================
Console.WriteLine(">>> Bases 基础工具 <<<");
ZBasesUsage.Run();

// ==================== Infrastructures 基础设施 ====================
Console.WriteLine("\n>>> Infrastructures 基础设施 <<<");
ZInfrastructuresUsage.AliasUsage();
ZInfrastructuresUsage.Cache();
ZInfrastructuresUsage.AOP();
ZInfrastructuresUsage.Event();
ZInfrastructuresUsage.Log();
ZInfrastructuresUsage.AutoMap();
ZInfrastructuresUsage.DI();
ZInfrastructuresUsage.CommandStack();
ZInfrastructuresUsage.StateMachine();
// ZInfrastructuresUsage.TaskQuene();   // 会阻塞等待 ReadLine，按需启用

// ==================== Structures 数据结构 ====================
Console.WriteLine("\n>>> Structures 数据结构 <<<");
ZStructuresUsage.Clone();
ZStructuresUsage.Memo();
ZStructuresUsage.RichEnum();
ZStructuresUsage.FileSingleton();

// ==================== Sugers 语法糖 ====================
Console.WriteLine("\n>>> Sugers 语法糖 <<<");
var suger = new ZSugerUsage();
suger.FunctionalStyleCall();
suger.IntEnumerator();
suger.FuncCompose();

Console.WriteLine("\n=== 所有示例执行完毕 ===");