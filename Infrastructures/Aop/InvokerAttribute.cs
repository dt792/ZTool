using System;

namespace ZTool.Infrastructures.Aop;

[AttributeUsage(AttributeTargets.Method)]
public abstract class InvokerAttribute : Attribute
{
	public Action Next = null;

	public abstract void Invoke(InvocationContext invocationContext);
}
