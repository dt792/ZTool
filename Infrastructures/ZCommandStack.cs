namespace ZTool.Infrastructures;
public abstract class ZCommandBase
{
    public virtual void Undo() { }
}
public class ZCommand : ZCommandBase
{
    public virtual void Do() { }
}
public abstract class ZCommand<T1> : ZCommandBase
{
    public T1 Arg1 { get; set; }
    public virtual void Do(T1 t1) { }
}
public abstract class ZCommand<T1, T2> : ZCommandBase
{
    public T1 Arg1 { get; set; }
    public T2 Arg2 { get; set; }
    public virtual void Do(T1 t1, T2 t2) { }
}
public abstract class ZCommand<T1, T2, T3> : ZCommandBase
{
    public T1 Arg1 { get; set; }
    public T2 Arg2 { get; set; }
    public T3 Arg3 { get; set; }
    public virtual void Do(T1 t1, T2 t2, T3 t3) { }
}
public class ZCommandStack
{
    public Stack<ZCommandBase> Stack { get; set; } = new Stack<ZCommandBase>();
    public void Do<C>(params object[] args) where C : ZCommandBase, new()
    {
        ZCommandBase command = (ZCommandBase)Activator.CreateInstance(typeof(C));
        command.GetType().GetMethod("Do").Invoke(command,args);
        //记录调用的参数
        for (int i = 0; i < args.Count(); i++)
        {
            var prop= command.GetType().GetProperty($"Arg{i + 1}");
            prop.SetValue(command, args[i]);
        }
        Stack.Push(command);
    }
    public void Undo()
    {
        var command = Stack.Pop();
        command.Undo();
    }
}
