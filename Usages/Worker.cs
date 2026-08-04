using System;

namespace ZTool.Usages;

public class Worker
{
	[AnyAop]
	public virtual void Work()
	{
		Console.WriteLine(1);
	}
}
