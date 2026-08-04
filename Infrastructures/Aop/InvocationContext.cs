using System;
using System.Collections.Generic;

namespace ZTool.Infrastructures.Aop;

public class InvocationContext
{
	public object[] Parameters = Array.Empty<object>();

	public object? ReturnValue;

	public Dictionary<string, object> Context { get; set; } = new Dictionary<string, object>();
}
