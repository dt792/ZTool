namespace ZTool.Infrastructures.ResponsibilityChain;

/// <summary>
/// 责任链处理者接口。允许将请求沿处理者链发送，每个处理者可处理请求或将其传递给链上的下一个处理者。
/// 当程序需要使用不同方式处理不同种类请求，且请求类型和顺序预先未知时使用。
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResult">处理结果类型</typeparam>
public interface IResponsibilityHandler<TRequest, TResult>
{
    /// <summary>
    /// 设置下一个处理者，返回该处理者以支持链式调用
    /// </summary>
    IResponsibilityHandler<TRequest, TResult> SetNext(IResponsibilityHandler<TRequest, TResult> handler);

    /// <summary>
    /// 处理请求；若本节点不处理则传递给下一节点
    /// </summary>
    TResult? Handle(TRequest request);
}

/// <summary>
/// 责任链处理者基类。子类重写 <see cref="Handle"/>，未处理时自动传递给下一处理者。
/// </summary>
public abstract class ZResponsibilityHandler<TRequest, TResult> : IResponsibilityHandler<TRequest, TResult>
{
    private IResponsibilityHandler<TRequest, TResult>? nextHandler;

    /// <summary>
    /// 子类重写此方法处理请求，不处理时调用 base.Handle 传递给下一节点
    /// </summary>
    public virtual TResult? Handle(TRequest request)
    {
        return nextHandler is not null ? nextHandler.Handle(request) : default;
    }

    public IResponsibilityHandler<TRequest, TResult> SetNext(IResponsibilityHandler<TRequest, TResult> handler)
    {
        nextHandler = handler;
        return handler;
    }
}
