using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZTool.Infrastructures;
public enum ZLoggerLevels
{
    Info,
    Warn,
    Trace,
    Error,
    Fatal,
}
public class ZLogContent
{
    public ZLogContent()
    {
    }
    public ZLogContent(string cate, string method, string content, ZLoggerLevels level)
    {
        Class = cate;
        Method= method;
        Content = content;
        Level = level;
    }
    public string Class { get; set; }
    public string Method { get; set; }
    public string Content { get; set; }
    public ZLoggerLevels Level { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
    public override string ToString()
    {
        return $"{Class}:{Method} [{Level}] {Content} {Time.ToString("T")}";
    }
}
public class ZLog
{
    public static bool ToConsole = false;
    public static List<(string, DateTime)> KeyPoints { get; set; } = new();
    public static void LogKeyPoint(string content)
    {
        KeyPoints.Add((content, DateTime.Now));
    }

    public static List<ZLogContent> LogContents { get; set; } = new();
    public static Dictionary<string, Dictionary<string, List<ZLogContent>>> CateLogContents { get; set; } = new();
    protected static (string, string) logExInfo([CallerMemberName] string callerMember = "")
    {
        StackTrace stackTrace = new StackTrace();
        MethodBase method = stackTrace.GetFrame(2).GetMethod();
        Type callingType = method.DeclaringType;
        return (callingType.Name, method.Name);
    }
    public static void ResetLogger()
    {
        KeyPoints.Clear();
        LogContents.Clear();
        CateLogContents.Clear();
        GC.Collect();
    }
    public static void Trace(string content)
    {
        var (type, method) = logExInfo();
        ZLogContent log = new(type, method, content, ZLoggerLevels.Trace);
        LogContents.Add(log);
        if (ToConsole)
            Console.WriteLine(log);
        if (!CateLogContents.ContainsKey(type))
        {
            CateLogContents.Add(type, new());
        }
        var c = CateLogContents[type];
        if (!c.ContainsKey(method))
        {
            c.Add(method, new List<ZLogContent>());
        }
        c[method].Add(log);
    }
    public static void Info(string content)
    {
        var (type, method) = logExInfo();
        ZLogContent log = new(type, method, content, ZLoggerLevels.Info);
        LogContents.Add(log);
        if (ToConsole)
            Console.WriteLine(log);
        if (!CateLogContents.ContainsKey(type))
        {
            CateLogContents.Add(type, new());
        }
        var c = CateLogContents[type];
        if (!c.ContainsKey(method))
        {
            c.Add(method, new List<ZLogContent>());
        }
        c[method].Add(log);
    }
    public static void Warn(string content)
    {
        var (type, method) = logExInfo();
        ZLogContent log = new(type, method, content, ZLoggerLevels.Warn);
        LogContents.Add(log);
        if (ToConsole)
            Console.WriteLine(log);
        if (!CateLogContents.ContainsKey(type))
        {
            CateLogContents.Add(type, new());
        }
        var c = CateLogContents[type];
        if (!c.ContainsKey(method))
        {
            c.Add(method, new List<ZLogContent>());
        }
        c[method].Add(log);
    }
    public static void Error(string content)
    {
        var (type, method) = logExInfo();
        ZLogContent log = new(type, method, content, ZLoggerLevels.Error);
        LogContents.Add(log);
        if (ToConsole)
            Console.WriteLine(log);
        if (!CateLogContents.ContainsKey(type))
        {
            CateLogContents.Add(type, new());
        }
        var c = CateLogContents[type];
        if (!c.ContainsKey(method))
        {
            c.Add(method, new List<ZLogContent>());
        }
        c[method].Add(log);
    }
    public static void Fatal(string content)
    {
        var (type, method) = logExInfo();
        ZLogContent log = new(type, method, content, ZLoggerLevels.Fatal);
        LogContents.Add(log);
        if (ToConsole)
            Console.WriteLine(log);
        if (!CateLogContents.ContainsKey(type))
        {
            CateLogContents.Add(type, new());
        }
        var c = CateLogContents[type];
        if (!c.ContainsKey(method))
        {
            c.Add(method, new List<ZLogContent>());
        }
        c[method].Add(log);
    }
}