using System.Collections.Generic;

namespace ZTool.Infrastructures.Cache;

public class ZCacher<T, R> : ZCacherBase where T : notnull
{
	private readonly Dictionary<T, R> historyResults = new Dictionary<T, R>();

	public R this[T index]
	{
		get
		{
			if (historyResults.ContainsKey(index))
			{
				return historyResults[index];
			}
			else
				return default(R)	;
		}
		set
		{
			historyResults[index] = value;
		}
	}
}
public class ZCacher<T1, T2, R> : ZCacherBase
{
	private readonly Dictionary<(T1, T2), R> historyResults = new Dictionary<(T1, T2), R>();

	public R this[T1 index1, T2 index2]
	{
		get
		{
			return historyResults[(index1, index2)];
		}
		set
		{
			historyResults[(index1, index2)] = value;
		}
	}
}
public class ZCacher<T1, T2, T3, R> : ZCacherBase
{
	private readonly Dictionary<(T1, T2, T3), R> historyResults = new Dictionary<(T1, T2, T3), R>();

	public R this[T1 index1, T2 index2, T3 index3]
	{
		get
		{
			return historyResults[(index1, index2, index3)];
		}
		set
		{
			historyResults[(index1, index2, index3)] = value;
		}
	}
}
