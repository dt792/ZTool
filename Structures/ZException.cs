using System;
using System.Collections.Generic;
using System.Text;

namespace ZTool.Structures;
/// <summary>
/// 用户操作异常
/// </summary>
public class OpException : ZException
{
    public OpException(string message)
    {
        Message = message;
    }
    public new string Message { get; init; }
}
/// <summary>
/// 没有遵循管理
/// </summary>
public class NotFollowConventionException : SysException
{
    public NotFollowConventionException(string message) : base(message)
    {
    }
}
/// <summary>
/// 系统异常
/// </summary>
public class SysException : ZException
{
    public SysException(string message)
    {
        Message = message;
    }
    public new string Message { get; init; }
}

public class ZException:Exception
{
}
