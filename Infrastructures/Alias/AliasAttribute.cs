using System;

namespace ZTool.Infrastructures.Alias;

[AttributeUsage(AttributeTargets.All)]
public class AliasAttribute : Attribute
{
	public string[] Alias { get; set; }

	public AliasAttribute(params string[] alias)
	{
		Alias = alias;
	}
}
