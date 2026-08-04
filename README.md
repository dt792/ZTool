# ZTool — C# 个人工具库

[![Target](https://img.shields.io/badge/.NET-10-blue)](https://dotnet.microsoft.com/)
[![Lang](https://img.shields.io/badge/C%23-14-purple)]()

个人积累的 C# 基础设施工具集，涵盖日志、事件总线、DI、AOP、命令栈、状态机、缓存、映射等常用模块，以及基础工具类、数据结构、语法糖和算法工具。

---

## 快速开始

```bash
git clone https://github.com/<your-name>/ZTool.git
cd ZTool
dotnet run
```

或作为 NuGet 包引用：

```bash
dotnet add package ZTool --version 1.1.0
```

---

## 功能模块

### 1. Bases — 基础工具（10 个）

| 类 | 功能 | 核心 API |
|---|---|---|
| `CharTool` | 字符生成 | `AZ(), az(), Zero2Nine(), All()` |
| `StringTool` | 字符串 | `AllIndex(string, pattern)`, `SplitByLineNSpace()`, 各种格式转换 (`ToUnicode`, `ToHex`, `FromHex` 等) |
| `ListTool` | 集合操作 | `FindPrevNext<T>()`, `ToLinkedList<T>()`, `ReplaceAll<T>()`, `TryGetFirst`, `DistinctBy` |
| `DictionaryTool` | 字典操作 | `Merge<TKey,TVal>()`, `GetOrAdd`, `Concat`, `TryGetValue` 带默认值 |
| `ObjectTool` | 对象工具 | `ShallowClone<T>()`, `DeepClone<T>()`（支持循环引用、数组、集合） |
| `ExpressionTool` | 表达式树 | `GetOper()`, `GetFieldName()`, `GetNames()` — 获取方法/属性/字段名 |
| `FuncTool` | 函数式 | `Delay<T>()`, `TryCatch(Action)`, `DoNothing`, `Repeat`, 计时包装 `WrapTime` |
| `EnumTool` | 枚举 | `GetAll<TEnum>()`, `ToCombo()` |
| `UlongTool` | 无符号长整形 | 位运算辅助 |
| `JsonTool` | JSON | `ToJson()`, `ToObj<T>()` 扩展方法（基于 Newtonsoft.Json） |

**使用示例：**

```csharp
using Bases;

// 深拷贝（支持循环引用）
var copy = original.DeepClone();

// 列表找前后相邻元素
var (prev, next) = list.FindPrevNext(x => x.Id == 42);

// 字典合并
var merged = dict1.Merge(dict2);

// JSON
var json = obj.ToJson();
var obj  = json.ToObj<MyClass>();
```

---

### 2. Infrastructures — 基础设施（12 个）

#### 2.1 Log — 日志

自动捕获调用者类名与方法名，支持按类别/方法名索引查询。

```csharp
using Infrastructures.Log;

ZLog.ToConsole = true;

ZLog.Trace("详情日志");
ZLog.Info("普通信息");
ZLog.Warn("警告");
ZLog.Error("错误");
ZLog.Fatal("致命");

// 计时
ZLog.StartStopwatch("算法A");
// ... do work ...
ZLog.StopStopwatch("算法A");
var elapsed = ZLog.GetTimeSpans();

// 关键节点
ZLog.LogKeyPoint("初始化完成");
```

#### 2.2 Event — 事件总线

支持 0~3 个参数的强类型事件 `ZEvent`、`ZEvent<T>`、`ZEvent<T1,T2>`、`ZEvent<T1,T2,T3>`。

```csharp
using Infrastructures.Event;

// 定义
public class PlayerDied : ZEvent<int> { }  // int = playerId
public class GameStart : ZEvent { }

// 使用
var e = new PlayerDied();
e.Subscribe(id => Console.WriteLine($"玩家 {id} 死亡"));
e.Publish(1001);
```

#### 2.3 DI — 单例注入容器

轻量 DI 容器，支持接口绑定、自动装配 `[DIAttribute]` 标记的依赖。

```csharp
using Infrastructures.DI;

var di = new ZSingletonDI();
di.Set<MyService, IMyService>();    // 接口绑定
di.Set<AnotherService>();           // 具体类型
di.Check();                         // 自动装配依赖

var svc = di.Get<IMyService>();
var all  = di.GetAll<IMyService>();
var q    = di.QuickGet<MyService>(); // Set + Check + Get
```

#### 2.4 Command — 命令栈（撤销/重做）

```csharp
using Infrastructures.Command;

var stack = new ZCommandStack();
stack.Do<AddCommand>(item, quantity);  // 自动记录参数
stack.Do<DeleteCommand>(itemId);
stack.Undo();  // 撤销 DeleteCommand
stack.Undo();  // 撤销 AddCommand
```

> 命令需实现 `ZCommandBase`：定义 `Do()` 和 `Undo()` 方法，以及 `Arg1`、`Arg2` 等属性接收参数。

#### 2.5 StateMachine — 状态机

定义状态间的转换行为。

```csharp
using Infrastructures.StateMachine;

var sm = new ZStateMachine<string>();
sm.State = "Idle";

sm.Define("Idle", "Running", ctx => Console.WriteLine($"{ctx.OldState} -> {ctx.NewState}"));
sm.Define("Running", "Stopped", ctx => stopTimer());

sm.To("Running");   // 输出: Idle -> Running
sm.To("Stopped");   // 调用 stopTimer
```

#### 2.6 Aop — 动态代理 AOP

通过 `[InvokerAttribute]` 在方法上添加拦截器，配合 `ZClass` / `MethodBuildTool` 动态生成代理类。

```csharp
using Infrastructures.Aop;

public class LogAttribute : InvokerAttribute
{
    public override void Invoke(InvocationContext ctx)
    {
        Console.WriteLine($"调用前: {ctx.Method.Name}");
        Next?.Invoke(ctx);  // 执行原方法
        Console.WriteLine($"调用后: {ctx.Method.Name}");
    }
}

public class MyClass
{
    [Log]
    public virtual void DoWork() { /* ... */ }
}

// 通过 ZClass 生成代理
var proxy = new ZClass<MyClass>();
proxy.Instance.DoWork();  // 自动触发 Log 拦截器
```

#### 2.7 Mapper — 对象映射

自动按同名属性/字段映射对象。

```csharp
using Infrastructures.Mapper;

ZMapper.Map(source, target);
var target = ZMapper.AutoMap<Source, Target>(source);
```

#### 2.8 Cache — 结果缓存

保存计算结果，避免重复计算。支持 1~3 个参数的缓存键。

```csharp
using Infrastructures.Cache;

var cache = new ZCacher<int, string>();
cache[42] = "答案";
var result = cache[42];  // "答案"
```

#### 2.9 Memo — 备忘录

记录并回滚对象状态。

```csharp
using Infrastructures.Memo;

var memo = new ZMemo<MyState>();
memo.Save(state);           // 记录快照
state.Modify();             // 修改
memo.Restore();             // 回滚到上一快照
```

#### 2.10 Daemon — 进程守护

监控指定进程，当进程退出时自动重启。

```csharp
using Infrastructures.Daemon;

// 找到目标进程 ID 后，传入守护
ZDaemon.Daemon(processId);

// 被守护的进程退出后，daemon 会自动在当前目录重新拉起
```

#### 2.11 Alias — 别名

为类绑定别名，按别名查找类型。

```csharp
using Infrastructures.Alias;

[Alias("玩家控制器")]
public class PlayerController { }

var type = ZAlias.GetAlias("玩家控制器");  // typeof(PlayerController)
```

#### 2.12 ResponsibilityChain — 责任链

请求沿处理者链传递，直到某个节点处理。

```csharp
using Infrastructures.ResponsibilityChain;

var h1 = new AuthHandler();
var h2 = new LogHandler();
var h3 = new RateLimitHandler();

h1.SetNext(h2).SetNext(h3);

var result = h1.Handle(request);
```

#### 2.13 Tasks — 任务队列

可暂停、可撤销的任务队列。

```csharp
using Infrastructures.Tasks;

var queue = new ZTaskQueue();
queue.Enqueue(new MyTask());
queue.Start();
queue.Pause();
```

---

### 3. Structures — 数据结构

| 类 | 功能 |
|---|---|
| `ZDict<K,V>` | 字典：键不存在时自动创建默认值（无参构造）或返回 `default` |
| `ZListDict<K,V>` | 列表字典：一个键对应多个值 |
| `ZPair<T>` | 简单的配对结构 |
| `RichEnum<TEnum,Content>` | Java 风格的富枚举：带内容的枚举，支持 `FromName()` 查询 |
| `FileSingleton<T>` | 文件单例：序列化到磁盘的单例 |
| `ZFlyweight<T>` / `ZFlyweightFactory<T>` | 享元模式：共享状态复用 |
| `ZException` | 基础异常类 |

**富枚举示例：**

```csharp
public class Color : RichEnum<Color, (byte R, byte G, byte B)>
{
    public static readonly Color Red   = new("Red",   (255, 0, 0));
    public static readonly Color Green = new("Green", (0, 255, 0));
    public static readonly Color Blue  = new("Blue",  (0, 0, 255));

    private Color(string name, (byte, byte, byte) value) : base(name, value) { }
}

var color = Color.FromName("Red");
Console.WriteLine(color.Value);  // (255, 0, 0)
foreach (var c in Color.Enumerations) { /* ... */ }
```

**享元示例：**

```csharp
using Structures;

var factory = new ZFlyweightFactory<(string, int)>(
    keySelector: s => s.Item1,
    ("A", 1), ("B", 2));

var f1 = factory.GetFlyweight(("A", 1));  // 返回已有享元
var f2 = factory.GetFlyweight(("C", 3));  // 创建新享元
Console.WriteLine(f1.SharedState);        // (A, 1)
Console.WriteLine(factory.Count);         // 3
```

---

### 4. Sugers — 语法糖

基于 C# 14 扩展成员（`extension`）实现。

```csharp
using Sugers;

// 泛型管道操作符（>>）：将数据流式传入函数链
string result = 100 >> (x => x + 1) >> (x => x.ToString());  // "101"

// 函数组合操作符（+）：f1 + f2 等价于 f2(f1(x))
Func<int, int>    f1 = x => x + 1;
Func<int, string> f2 = x => x.ToString();
Console.WriteLine((f1 + f2)(3));  // "4"

// Compose 扩展方法（+ 的替代方案）
var composed = f1.Compose(f2);

// int 可等待 (毫秒)：await 一个整数等同于 Task.Delay
await 1000;  // 等待 1 秒

// int 可枚举：foreach 遍历 0 到 n-1
foreach (var i in 5)    // 0, 1, 2, 3, 4
    Console.WriteLine(i);
```

---

### 5. Algorithms — 算法

```csharp
using Algorithms;

var items = new List<int> { 1, 2, 3 };

// 组合
var c2 = PermutationCombinationTool.GetCombination(items, 2);
// [[1,2], [1,3], [2,3]]

var allC = PermutationCombinationTool.GetAllCombination(items);
// [[], [1], [2], [3], [1,2], [1,3], [2,3], [1,2,3]]

// 排列
var a2 = PermutationCombinationTool.GetArrangement(items, 2);
// [[1,2], [1,3], [2,1], [2,3], [3,1], [3,2]]

// 笛卡尔积
var cp = PermutationCombinationTool.CartesianProduct(
    new() { new() { 1, 2 }, new() { "A", "B" } });
// [[1,A], [1,B], [2,A], [2,B]]
```

---

## 目录结构

```
ZTool/
├── ZTool.csproj
├── Program.cs               # 使用示例入口
├── Algorithms/              排列组合 / 笛卡尔积
├── Bases/                   10 个基础工具
├── Infrastructures/
│   ├── Alias/               别名
│   ├── Aop/                 AOP 动态代理
│   ├── Cache/               缓存
│   ├── Command/             命令栈（撤销/重做）
│   ├── Daemon/              守护任务
│   ├── DI/                  单例注入容器
│   ├── Event/               事件总线
│   ├── Log/                 日志
│   ├── Mapper/              对象映射
│   ├── Memo/                备忘录
│   ├── ResponsibilityChain/ 责任链
│   ├── StateMachine/        状态机
│   └── Tasks/               任务队列
├── Structures/              数据结构 / 享元
├── Sugers/                  语法糖
├── Usages/                  使用示例
│   ├── ZBasesUsage.cs       基础工具用法
│   ├── ZAlgorithmsUsage.cs  算法用法
│   ├── ZSugerUsage.cs       语法糖用法
│   ├── ZStructuresUsage.cs  数据结构用法
│   ├── ZInfrastructuresUsage.cs  基础设施用法
│   ├── AnyAop.cs            AOP 拦截器示例
│   └── Worker.cs            AOP 被代理类示例
└── README.md
```

---

## 环境要求

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- C# 14（`Sugers` 使用了扩展成员语法）

## 许可证

MIT
