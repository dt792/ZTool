using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ZTool.Infrastructures.Log;

public static class ZLog
{
	public static bool ToConsole = false;

	public static List<(string, DateTime)> KeyPoints { get; set; } = new List<(string, DateTime)>();

	public static List<ZLogContent> LogContents { get; set; } = new List<ZLogContent>();

	public static Dictionary<string, Dictionary<string, List<ZLogContent>>> CateLogContents { get; set; } = new Dictionary<string, Dictionary<string, List<ZLogContent>>>();

	public static Dictionary<string, Stopwatch> NamedStopwatchs { get; set; } = new Dictionary<string, Stopwatch>();

	public static void LogKeyPoint(string content)
	{
		KeyPoints.Add((content, DateTime.Now));
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
		LogCore(ZLogLevel.Trace, content);
	}

	public static void Info(string content)
	{
		LogCore(ZLogLevel.Info, content);
	}

	public static void Warn(string content)
	{
		LogCore(ZLogLevel.Warn, content);
	}

	public static void Error(string content)
	{
		LogCore(ZLogLevel.Error, content);
	}

	public static void Fatal(string content)
	{
		LogCore(ZLogLevel.Fatal, content);
	}

	public static void StartStopwatch(string name)
	{
		if (!NamedStopwatchs.ContainsKey(name))
		{
			NamedStopwatchs.Add(name, new Stopwatch());
		}
		NamedStopwatchs[name].Start();
	}

	public static void StopStopwatch(string name)
	{
		if (NamedStopwatchs.TryGetValue(name, out Stopwatch value))
		{
			value.Stop();
		}
	}

	public static void ResetStopwatch(string name)
	{
		if (NamedStopwatchs.TryGetValue(name, out Stopwatch value))
		{
			value.Reset();
		}
	}

	public static Dictionary<string, TimeSpan> GetTimeSpans()
	{
		return NamedStopwatchs.ToDictionary<KeyValuePair<string, Stopwatch>, string, TimeSpan>((KeyValuePair<string, Stopwatch> kv) => kv.Key, (KeyValuePair<string, Stopwatch> kv) => kv.Value.Elapsed);
	}

	private static void LogCore(ZLogLevel level, string content)
	{
		(string, string) callerInfo = GetCallerInfo();
		string item = callerInfo.Item1;
		string item2 = callerInfo.Item2;
		ZLogContent zLogContent = new ZLogContent(item, item2, content, level);
		LogContents.Add(zLogContent);
		if (ToConsole)
		{
			Console.WriteLine(zLogContent);
		}
		if (!CateLogContents.TryGetValue(item, out Dictionary<string, List<ZLogContent>> value))
		{
			value = new Dictionary<string, List<ZLogContent>>();
			CateLogContents.Add(item, value);
		}
		if (!value.TryGetValue(item2, out var value2))
		{
			value2 = new List<ZLogContent>();
			value.Add(item2, value2);
		}
		value2.Add(zLogContent);
	}

	private static (string, string) GetCallerInfo()
	{
		MethodBase methodBase = new StackTrace().GetFrame(3)?.GetMethod();
		return (methodBase?.DeclaringType?.Name ?? "", methodBase?.Name ?? "");
	}
}
