using System.Collections;
using ZTool.Bases;

namespace ZTool.Structures;

public class ZDict
{
    /// <summary>
    /// 计数列表中的元素与出现次数
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static ZDict<K, int> CountItemNum<K>(IEnumerable<K> list)
    {
        ZDict<K, int> dict = new ZDict<K, int>();
        foreach (var item in list)
        {
            if (dict.ContainsKey(item))
            {
                dict[item]++;
            }
            else
            {
                dict.Add(item, 1);
            }
        }
        return dict;
    }
}
public class ZDict<Key, Value> : Dictionary<Key, Value>
{
    public new Value this[Key index]
    {
        get
        {
            if (!ContainsKey(index))
            {
                var ctor = typeof(Value).GetConstructor(Type.EmptyTypes);
                if (ctor != null)
                {
                    base[index] = (Value)ctor.Invoke(null);
                }
                else
                {
                    return default(Value);
                }
            }
            return base[index];
        }
        set
        {
            if (!ContainsKey(index))
            {
                Add(index, value);
            }
            else
            {
                base[index] = value;
            }
        }
    }


}
public class ZListDict<Key, Value> : IEnumerable<KeyValuePair<Key, List<Value>>>
{
    public class ZListDictEnumerator : IEnumerator<KeyValuePair<Key, List<Value>>>
    {
        List<KeyValuePair<Key, List<Value>>> Data;
        public ZListDictEnumerator(Dictionary<Key, List<Value>> data)
        {
            Data = data.ToList();
        }
        int index = 0;
        public KeyValuePair<Key, List<Value>> Current => Data[index++];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (index < Data.Count)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void Reset()
        {
            index = 0;
        }
    }
    Dictionary<Key, List<Value>> Datas = new Dictionary<Key, List<Value>>();
    public List<Value> this[Key index]
    {
        get
        {
            if (!Datas.ContainsKey(index))
            {
                List<Value> newList = new List<Value>();
                Datas.Add(index, newList);
            }
            return Datas[index];
        }
        set
        {
            if (Datas.ContainsKey(index))
            {
                Datas[index] = value;
            }
            else
            {
                Datas.Add(index, value);
            }
        }
    }

    public IEnumerator<KeyValuePair<Key, List<Value>>> GetEnumerator()
    {
        return new ZListDictEnumerator(Datas);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        string result = "{";
        foreach (var item in Datas)
        {
            result += item.Key;
            result += ":";
            result += StringTool.ComposeList(item.Value);
        }
        result += "}";
        return result;
    }
}