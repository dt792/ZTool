namespace ZTool.Bases;

public static class StringTool
{
    /// <summary>
    /// StringList:[a,b,c,d],Separator:(" ")->"a b c d"
    /// </summary>
    public static string ComposeList<T>(IEnumerable<T> list, string separator = ",", Func<T, string> toString = null)
    {
        var result = "";
        bool first = true;
        foreach (var item in list)
        {
            if (first)
            {
                if (toString is not null)
                {
                    result += toString(item);
                }
                else
                {
                    result += item;
                }

                first = false;
            }
            else
            {
                if (toString is not null)
                {
                    result += $"{separator}{toString(item)}";
                }
                else
                {
                    result += $"{separator}{item}";
                }
            }

        }
        return result;
    }
    public static bool HasChinese(string str)
    {
        foreach (var c in str)
        {
            if (CharTool.IsChinese(c))
                return true;
        }
        return false;
    }
    public static string ConvertToBase64(this string str)
    {
        byte[] b = System.Text.Encoding.Default.GetBytes(str);
        return Convert.ToBase64String(b);
    }
    public static string ConvertFromBase64(this string str64)
    {
        byte[] b = Convert.FromBase64String(str64);
        return System.Text.Encoding.Default.GetString(b);
    }
}
