using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZTool.Infrastructures.Command;

public class ZCommandStack
{
	public Stack<ZCommandBase> Stack { get; set; } = new Stack<ZCommandBase>();

	public void Do<C>(params object[] args) where C : ZCommandBase, new()
	{
		ZCommandBase zCommandBase = new C();
		MethodInfo methodInfo = zCommandBase.GetType().GetMethod("Do") ?? throw new InvalidOperationException($"{typeof(C)} 上找不到 Do 方法");
		methodInfo.Invoke(zCommandBase, args);
		for (int i = 0; i < args.Length; i++)
		{
			zCommandBase.GetType().GetProperty($"Arg{i + 1}")?.SetValue(zCommandBase, args[i]);
		}
		Stack.Push(zCommandBase);
	}

	public void Undo()
	{
		if (Stack.Count > 0)
		{
			Stack.Pop().Undo();
		}
	}
}
