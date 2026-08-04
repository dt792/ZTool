using System;

namespace ZTool.Infrastructures.DI;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DIAttribute : Attribute
{
	public Type? ActualType { get; set; }

	public DIAttribute()
	{
	}

	public DIAttribute(Type actualType)
	{
		ActualType = actualType;
	}
}
