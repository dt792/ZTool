namespace ZTool.Infrastructures.Command;

public class ZCommand : ZCommandBase
{
	public virtual void Do()
	{
	}
}
public abstract class ZCommand<T1> : ZCommandBase
{
	public T1 Arg1 { get; set; } = default(T1);

	public virtual void Do(T1 t1)
	{
	}
}
public abstract class ZCommand<T1, T2> : ZCommandBase
{
	public T1 Arg1 { get; set; } = default(T1);

	public T2 Arg2 { get; set; } = default(T2);

	public virtual void Do(T1 t1, T2 t2)
	{
	}
}
public abstract class ZCommand<T1, T2, T3> : ZCommandBase
{
	public T1 Arg1 { get; set; } = default(T1);

	public T2 Arg2 { get; set; } = default(T2);

	public T3 Arg3 { get; set; } = default(T3);

	public virtual void Do(T1 t1, T2 t2, T3 t3)
	{
	}
}
