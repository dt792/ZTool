using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using Microsoft.VisualBasic;
using ZTool.Infrastructures;
using ZTool.Sugers;
using ZTools.Infrastructures;

namespace ZTool.Usages;

internal class ZInfrastructuresUsage
{
    [Ailas(["通用","一般"])]
    class Model { }
    public static void AliasUsage()
    {
        ZAilas.GetAlias<Model>();
        typeof(Model).GetAlias();
    }
    public static void Cache()
    {
        ZCacher<int, string> cacher = new ();
        cacher[10] = "a";
        Console.WriteLine(cacher[10]);
    }
    public static void AOP()
    {
        var a= ZClass.Make<Worker>();
        Worker dd = (Worker)Activator.CreateInstance(a);
        dd.Work();
    }
    public class AnyEvent:Event<int> { }
    public static void Event()
    {
        ZEvent.Let<AnyEvent>().Subscribe(Console.WriteLine);
        ZEvent.Let<AnyEvent>().Publish(10);
    }
    public static void Log()
    {
        ZLog.ToConsole = true;
        ZLog.Info("haha");
    }
    public class Any { 
        public int AnyInt{ get; set; }
        public string AnyStr="23";
        public string done="done";
        public string msg = "dddsd";
    }
    public class AnyDO { 
        public int anyint { get; set; }
        public string any_str { get; set; }
        public string mes;
        public int msglength = 0;
    }
    public static void AutoMap()
    {
        ZMapper.Define<Any, AnyDO>().MapMember(a=>a.done, b=>b.mes)
            .Final((f,t)=>t.msglength=f.msg.Length);
        Any a=new Any() { AnyInt =10};
        var q= a.To<AnyDO>();
        Console.WriteLine(q);
        AnyDO ao = ZMapper.Map<Any,AnyDO>(a);
        Console.WriteLine(ao);
    }
    public class A { }
    public class B { [DI]public A a; }
    public static void DI()
    {
        ZSingletonDI DI =new ZSingletonDI();
        DI.Set<B>();
        //检查计算依赖后再获取
        DI.Check();
        var b= DI.Get<B>();
        Console.WriteLine(b.a);
        //计算封装
        b = DI.QuickGet<B>();
        Console.WriteLine(b.a);
    }
    static int a;
    class AddOne : Command { public override void Do() { a += 1; } }
    class MulAny : Command<int> { public override void Do(int i) { a *= i; }
        public override void Undo()
        {
            a /= Arg1;
        }
    }
    public static void CommandStack()
    {
        ZCommandStack cs = new ();
        cs.Do<AddOne>();
        cs.Do<MulAny>(2);
        Console.WriteLine(a);
        cs.Undo();
        Console.WriteLine(a);
    }
    public enum State
    {
        One,Two, Three
    }
    public static void StateMachine()
    {
        ZStateMachine<State> stateMachine=new ();
        stateMachine.Define(State.One, State.Two,(stateMachine) => Console.WriteLine("2->1"));
        
        stateMachine.To(State.Two);
    }
    public class AnyTask : ZTask
    {
        public override void Run()
        {
            1000.GetAwaiter().GetResult();
            Console.WriteLine("任务完成");
        }
    }
    public static void TaskQuene()
    {
        ZTaskQuene<AnyTask> taskQuene = new();
        taskQuene.Start();
        taskQuene.AddTask();
        taskQuene.AddTask();
        Console.ReadLine();
    }
}
public class Worker
{
    [AnyAop]
    public virtual void Work()
    {
        Console.WriteLine(1);
    }
}
public class AnyAop : InvokerAttribute
{
    public override void Invoke(InvocationContext invocationContext)
    {
        Console.WriteLine(10);
        Next();
        Console.WriteLine(10);
    }
}