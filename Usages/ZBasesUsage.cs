global using static ZTool.Bases.UlongTool;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.Marshalling;



using Newtonsoft.Json;

using ZTool.Bases;

namespace ZTool.Usages;

public static class ZBasesUsage
{
    public static void Run()
    {
        StringToolDemo();
        ListToolDemo();
        DictionaryToolDemo();
        ObjectToolDemo();
        FuncToolDemo();
        ExpressionToolDemo();
        EnumToolDemo();
        CharToolDemo();
        UlongToolDemo();
        JsonToolDemo();
    }

    // ==================== StringTool ====================
    static void StringToolDemo()
    {
        Console.WriteLine("=== StringTool ===");

        // ComposeList — 将集合拼接为字符串
        var list = new List<string> { "a", "b", "c" };
        string composed = StringTool.ComposeList(list, separator: ", ", toString: s => s.ToUpper());
        Console.WriteLine($"ComposeList: {composed}");

        // HasChinese — 判断字符串是否包含中文
        Console.WriteLine($"'你好world' 含中文: {StringTool.HasChinese("你好world")}");
        Console.WriteLine($"'hello' 含中文: {StringTool.HasChinese("hello")}");

        // ConvertToBase64 / ConvertFromBase64 — Base64 编解码 (扩展方法)
        string encoded = "Hello ZTool".ConvertToBase64();
        Console.WriteLine($"Base64: {encoded}");
        string decoded = encoded.ConvertFromBase64();
        Console.WriteLine($"解码: {decoded}");

        Console.WriteLine();
    }

    // ==================== ListTool ====================
    // 以下方法对应 ListTool.cs 中的设计，由于 ILSpy 还原时代码被注释，
    // 在此处按原始设计重新定义，作为 usage 辅助方法。
    public static List<(T Value, int Index)> Indexlize<T>(this IList<T> values)
    {
        var result = new List<(T Value, int Index)>();
        for (int i = 0; i < values.Count; i++)
            result.Add((values[i], i));
        return result;
    }

    public static int FindPrevNext<T>(this List<T> values, T obj, out T? prev, out T? next) where T : notnull
    {
        int index = values.IndexOf(obj);
        prev = index > 0 ? values[index - 1] : default;
        next = index < values.Count - 1 ? values[index + 1] : default;
        return index;
    }

    static void ListToolDemo()
    {
        Console.WriteLine("=== ListTool ===");

        var nums = new List<int> { 10, 20, 30, 40, 50 };

        // Indexlize — 为列表元素附加索引
        var indexed = nums.Indexlize();
        Console.WriteLine("Indexlize:");
        foreach (var (val, idx) in indexed)
            Console.WriteLine($"  [{idx}] = {val}");

        // FindPrevNext — 查找元素的前驱和后继
        int pos = nums.FindPrevNext(30, out var prev, out var next);
        Console.WriteLine($"FindPrevNext(30): index={pos}, prev={prev}, next={next}");

        Console.WriteLine();
    }

    // ==================== DictionaryTool ====================
    static void DictionaryToolDemo()
    {
       Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
        DictionaryTool.CategorizeAdd(dict, "fruit", "apple");
        DictionaryTool.CategorizeAdd(dict, "fruit", "banana");
        DictionaryTool.CategorizeAdd(dict, "animal", "cat");
        DictionaryTool.CategorizeAdd(dict, "animal", "dog");

    }

    // ==================== ObjectTool ====================
    class Person
    {
        public string Name { get; set; } = "Alice";
        public int Age { get; set; } = 25;
    }

    static void ObjectToolDemo()
    {
        Console.WriteLine("=== ObjectTool ===");

        var p = new Person { Name = "Alice", Age = 25 };

        // PrintProps — 打印对象属性
        Console.WriteLine("PrintProps:");
        ObjectTool.PrintProps(p);

        // ShallowClone — 浅拷贝 (扩展方法)
        Person? shallow = p.ShallowClone();
        shallow!.Name = "Bob";
        Console.WriteLine($"ShallowClone: 原={p.Name}, 浅拷贝={shallow.Name}");

        // DeepClone — 深拷贝 (扩展方法，通过 JSON 序列化)
        Person? deep = p.DeepClone();
        deep!.Age = 99;
        Console.WriteLine($"DeepClone: 原.Age={p.Age}, 深拷贝.Age={deep.Age}");

        Console.WriteLine();
    }

    // ==================== FuncTool ====================
    static void FuncToolDemo()
    {
        Console.WriteLine("=== FuncTool ===");

        // Pack — 将多参数函数部分应用，锁定部分参数
        Func<int, int, int> add = (a, b) => a + b;
        Func<int,int> add5 = add.Pack(5);        // 锁定第一个参数为 5
        Console.WriteLine($"Pack: add(3,4)={add(3, 4)}, add5(10)={add5(10)}");


        Console.WriteLine();
    }

    // ==================== ExpressionTool ====================
    class DemoModel
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    static void ExpressionToolDemo()
    {
        Console.WriteLine("=== ExpressionTool ===");

        // GetMember — 从 Expression 中提取 MemberInfo
        var member = ExpressionTool.GetMember<DemoModel>(x => x.Name);
        Console.WriteLine($"GetMember<T>: {member?.Name}");

        var member2 = ExpressionTool.GetMember<DemoModel, int>(x => x.Age);
        Console.WriteLine($"GetMember<I,P>: {member2?.Name}");

        // GetOper — 提取二元操作信息
        Expression<Func<int, bool>> expr = (a) =>true;
        //var oper = ExpressionTool.GetOper(expr);
        //Console.WriteLine($"GetOper: {oper.Operator}, Left={oper.Left}, Right={oper.Right}");

        Console.WriteLine();
    }

    // ==================== EnumTool ====================
    static void EnumToolDemo()
    {
        Console.WriteLine("=== EnumTool ===");

        // GetEnumList — 获取枚举所有值
        IEnumerable<DayOfWeek> allDays = EnumTool.GetEnumList<DayOfWeek>();
        Console.WriteLine("GetEnumList<DayOfWeek>: " + string.Join(", ", allDays));

        Console.WriteLine();
    }

    // ==================== CharTool ====================
    static void CharToolDemo()
    {
        Console.WriteLine("=== CharTool ===");

        // IsChinese / IsCapital / IsDigit — 字符类型判断 (扩展方法)
        char c1 = '中';
        char c2 = 'A';
        char c3 = '5';
        Console.WriteLine($"'{c1}' IsChinese={c1.IsChinese()}, IsCapital={c1.IsCapital()}, IsDigit={c1.IsDigit()}");
        Console.WriteLine($"'{c2}' IsChinese={c2.IsChinese()}, IsCapital={c2.IsCapital()}, IsDigit={c2.IsDigit()}");
        Console.WriteLine($"'{c3}' IsChinese={c3.IsChinese()}, IsCapital={c3.IsCapital()}, IsDigit={c3.IsDigit()}");

        // GetCharPositions — 查找字符在字符串中的所有位置
        var positions = CharTool.GetCharPositions('l', "hello world");
        Console.WriteLine($"GetCharPositions('l', \"hello world\"): [{string.Join(", ", positions.SelectMany(l => l.ToString()))}]");

        Console.WriteLine();
    }

    // ==================== UlongTool ====================
    static void UlongToolDemo()
    {
        Console.WriteLine("=== UlongTool ===");

        // UlongToStr — 将 ulong 转为指定进制字符串 (默认 36 进制)
        ulong value = 123456789UL;
       
        string str36 = UlongTool.UlongToStr(value);
        Console.WriteLine($"UlongToStr({value}, 36进制): {str36}");

        string str16 = UlongTool.UlongToStr(value);
        Console.WriteLine($"UlongToStr({value}, 16进制): {str16}");

        Console.WriteLine();
    }

    // ==================== JsonTool ====================
    class Item
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    static void JsonToolDemo()
    {
        Console.WriteLine("=== JsonTool ===");

        var item = new Item { Name = "Sword", Count = 10 };

        // ToJson — 对象转 JSON 字符串 (扩展方法)
        string json = item.ToJson();
        Console.WriteLine($"ToJson:\n{json}");

        // ToObj — JSON 字符串转对象 (扩展方法)
        string raw = "{\"Name\":\"Shield\",\"Count\":5}";
        Item? parsed = raw.ToObj<Item>();
        Console.WriteLine($"ToObj: Name={parsed?.Name}, Count={parsed?.Count}");

        // RefSerialize — 引用序列化 (保留 $type 信息)
        string refJson = item.RefSerialize();
        Console.WriteLine($"RefSerialize: {refJson[..Math.Min(refJson.Length, 80)]}...");

        // RefDeserialize — 引用反序列化
        Item? restored = refJson.RefDeserialize<Item>();
        Console.WriteLine($"RefDeserialize: Name={restored?.Name}, Count={restored?.Count}");

        Console.WriteLine();
    }
}
