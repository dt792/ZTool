using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZTools.Infrastructures;
public class StateTransferContext<T>
{
    public T OldState {  get; set; }
    public T NewState { get; set; }
}
//State 如果把状态转移改为真的判定
public class ZStateMachine<T>
{
    /// <summary>
    /// 不允许空状态
    /// </summary>
    public T State{ get; set; }
    Dictionary<(T, T), Action<StateTransferContext<T>>> transferDict = new();
    public void Define(T oldState, T newState, Action<StateTransferContext<T>> action)
    {
        transferDict.Add((oldState, newState), action);
    }
    public void To(T newState)
    {
        StateTransferContext<T> context = new StateTransferContext<T>();
        context.OldState = State;
        context.NewState = newState;
        var transfer = transferDict.FirstOrDefault(kv =>
        {
            var b1 = kv.Key.Item1.Equals(State) ;
            var b2 = kv.Key.Item2.Equals(newState);
            return b1 && b2;
        });
        transfer.Value?.Invoke(context);
    }
}