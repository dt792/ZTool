using System;
using System.Collections.Generic;
using System.Text;

namespace ZTool.Structures;

public class ZPair<T1,T2>
{
    public T1 First { get; set; }
    public T2 Second { get; set; }
    public static implicit operator ZPair<T1, T2>((T1, T2) tuple)
    {
        return new ZPair<T1, T2> { First = tuple.Item1, Second = tuple.Item2 };
    }
}
