using System;
using ZTool.Infrastructures.Aop;

namespace ZTool.Usages;

public class AnyAop : InvokerAttribute
{
	public override void Invoke(InvocationContext invocationContext)
	{
		Console.WriteLine(10);
		Next();
		Console.WriteLine(10);
	}
}
