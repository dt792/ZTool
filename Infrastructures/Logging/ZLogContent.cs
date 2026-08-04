using System;

namespace ZTool.Infrastructures.Log;

public class ZLogContent
{
	public string Class { get; set; } = "";

	public string Method { get; set; } = "";

	public string Content { get; set; } = "";

	public ZLogLevel Level { get; set; }

	public DateTime Time { get; set; } = DateTime.Now;

	public ZLogContent()
	{
	}

	public ZLogContent(string cate, string method, string content, ZLogLevel level)
	{
		Class = cate;
		Method = method;
		Content = content;
		Level = level;
	}

	public override string ToString()
	{
		return $"{Class}:{Method} [{Level}] {Content} {Time:T}";
	}
}
