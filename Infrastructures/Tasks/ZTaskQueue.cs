using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ZTool.Infrastructures.Tasks;

public class ZTaskQueue<T> where T : ZTask
{
	private readonly List<T> allList = new List<T>();

	private readonly List<T> waitingList = new List<T>();

	private readonly List<T> runningList = new List<T>();

	private readonly List<T> finishedList = new List<T>();

	private readonly List<T> crackedList = new List<T>();

	private readonly AutoResetEvent processLock = new AutoResetEvent(initialState: true);

	public Func<T> CreateTask { get; set; } = () => (T)Activator.CreateInstance(typeof(T));

	public Func<bool> CanRunNextFunc { get; set; }

	public ZTaskQueue()
	{
		CanRunNextFunc = () => waitingList.Count > 0;
	}

	public List<string> GetAllID()
	{
		return allList.Select((T t) => t.Id).ToList();
	}

	public string AddTask()
	{
		return AddTask(CreateTask());
	}

	public string AddTask(T task)
	{
		task.Id = Guid.CreateVersion7().ToString();
		task.NotifyStatusChange = delegate(ZTaskStatus _, ZTaskStatus newStatus)
		{
			if (newStatus == ZTaskStatus.Finished)
			{
				runningList.Remove(task);
				finishedList.Add(task);
				processLock.Set();
			}
			if (newStatus == ZTaskStatus.Cracked)
			{
				runningList.Remove(task);
				crackedList.Add(task);
				processLock.Set();
			}
		};
		task.Status = ZTaskStatus.Waiting;
		allList.Add(task);
		waitingList.Add(task);
		processLock.Set();
		return task.Id;
	}

	public ZTaskStatus? GetTaskStatus(string id)
	{
		return allList.FirstOrDefault((T t) => t.Id == id)?.Status;
	}

	public T? GetTask(string id)
	{
		return allList.FirstOrDefault((T t) => t.Id == id);
	}

	public void Start()
	{
		Thread thread = new Thread((ThreadStart)delegate
		{
			while (true)
			{
				Thread.Sleep(1000);
				if (CheckAndRunNext())
				{
					processLock.WaitOne();
				}
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	public virtual bool CheckAndRunNext()
	{
		if (CanRunNextFunc())
		{
			RunNext();
			return true;
		}
		return false;
	}

	public virtual void RunNext()
	{
		if (waitingList.Count != 0)
		{
			T nextTask = waitingList[0];
			waitingList.RemoveAt(0);
			nextTask.Status = ZTaskStatus.Running;
			runningList.Add(nextTask);
			Thread thread = new Thread((ThreadStart)delegate
			{
				nextTask.Run();
			});
			thread.IsBackground = true;
			thread.Start();
		}
	}
}
