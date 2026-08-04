using System;
using System.Collections.Generic;

namespace ZTool.Infrastructures.StateMachine;

public class ZStateMachine<T> where T : notnull
{
	private readonly Dictionary<(T, T), Action<StateTransferContext<T>>> transferDict = new Dictionary<(T, T), Action<StateTransferContext<T>>>();

	public T State { get; set; } = default(T);

	public void Define(T oldState, T newState, Action<StateTransferContext<T>> action)
	{
		transferDict[(oldState, newState)] = action;
	}

	public void To(T newState)
	{
		StateTransferContext<T> obj = new StateTransferContext<T>
		{
			OldState = State,
			NewState = newState
		};
		if (transferDict.TryGetValue((State, newState), out Action<StateTransferContext<T>> value))
		{
			value?.Invoke(obj);
		}
		State = newState;
	}
}
