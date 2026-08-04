using System;

namespace ZTool.Infrastructures.Tasks;

public abstract class ZTask
{
	public Action<ZTaskStatus, ZTaskStatus>? NotifyStatusChange;

	private ZTaskStatus status;

	public ZTaskStatus Status
	{
		get
		{
			return status;
		}
		set
		{
			EventInvoke(value);
			NotifyStatusChange?.Invoke(status, value);
			status = value;
		}
	}

	public string Id { get; set; } = "";

	public event Action<ZTask>? OnFinished;

	public event Action<ZTask>? OnRunning;

	public event Action<ZTask>? OnCracked;

	protected void EventInvoke(ZTaskStatus newStatus)
	{
		switch (newStatus)
		{
		case ZTaskStatus.Finished:
			this.OnFinished?.Invoke(this);
			break;
		case ZTaskStatus.Cracked:
			this.OnCracked?.Invoke(this);
			break;
		case ZTaskStatus.Running:
			this.OnRunning?.Invoke(this);
			break;
		case ZTaskStatus.Holding:
			break;
		}
	}

	public abstract void Run();
}
