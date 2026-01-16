namespace ZTool.Infrastructures;

public enum ZTaskStatu
{
    Waiting,
    Running,
    Holding,
    Finished,
    Cracked,
}

/// <summary>
/// 在ZTask中修改Statu即可进入
/// </summary>
public abstract class ZTask
{
    public event Action<ZTask> OnFinished;
    public event Action<ZTask> OnRunning;
    public event Action<ZTask> OnCracked;

    public Action<ZTaskStatu, ZTaskStatu> NotifyStatuChange;
    protected void EventInvoke(ZTaskStatu oldStatu, ZTaskStatu newStatu)
    {
        if (newStatu == ZTaskStatu.Finished)
        {
            OnFinished?.Invoke(this);
        }
        if (newStatu == ZTaskStatu.Cracked)
        {
            OnCracked?.Invoke(this);
        }
        if (newStatu == ZTaskStatu.Running)
        {
            OnRunning?.Invoke(this);
        }
    }

    ZTaskStatu statu;
    public ZTaskStatu Statu { get => statu; set { EventInvoke(statu, value); NotifyStatuChange?.Invoke(statu, value); statu = value; } }
    public string Id { get; set; }
    public abstract void Run();
}

public class ZTaskQuene<T> where T : ZTask
{
    public Func<T> CreateTask { get; set; } = () => (T)Activator.CreateInstance(typeof(T));
    public Func<bool> CanRunNextFunc { get; set; }

    List<T> allList = new();
    List<T> waitingList = new();
    List<T> runningList = new();
    List<T> finishedList = new();
    List<T> crackedList = new();
    public ZTaskQuene()
    {
        CanRunNextFunc = () => waitingList.Count > 0;
    }
    /// <summary>
    /// 每当有事件将触发
    /// </summary>
    AutoResetEvent processLock = new AutoResetEvent(true);
    public List<string> GetAllID()
    {
        return allList.Select(t => t.Id).ToList();
    }
    public string AddTask()
    {
        return AddTask(CreateTask());
    }
    public string AddTask(T task)
    {
        var guid = Guid.CreateVersion7();
        task.Id = guid.ToString();
        task.Statu = ZTaskStatu.Waiting;
        task.NotifyStatuChange = (s1, s2) =>
        {
            //检查能否运行新任务，移动任务列表
            if (s2 == ZTaskStatu.Finished)
            {
                runningList.Remove(task);
                finishedList.Add(task);
                processLock.Set();
            }
            if (s2 == ZTaskStatu.Cracked)
            {
                runningList.Remove(task);
                crackedList.Add(task);
                processLock.Set();
            }
        };

        allList.Add(task);
        waitingList.Add(task);
        processLock.Set();

        return task.Id;
    }
    /// <summary>
    /// 返回空则表示id不正确
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ZTaskStatu? GetTaskStatu(string id)
    {
        return allList.Where(t => t.Id == id).FirstOrDefault()?.Statu;
    }
    public T? GetTask(string id)
    {
        return allList.FirstOrDefault(t => t.Id == id);
    }
    public void Start()
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (CheckAndRunNext())
                    processLock.WaitOne();
            }
        });
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
        var nextTask = waitingList[0];
        nextTask.Statu = ZTaskStatu.Running;
        waitingList.Remove(nextTask);
        runningList.Add(nextTask);
        Thread thread = new Thread(() =>
        {
            nextTask.Run();
        });
        thread.Start();

    }
}
