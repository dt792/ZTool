namespace ZTool.Infrastructures.StateMachine;

public class StateTransferContext<T>
{
	public T? OldState { get; set; }

	public T? NewState { get; set; }
}
