namespace ZTool.Bases;

public static class CharExtensions
{
    // 判断是否为常见汉字（可按需扩展其它汉字区间）
    public static bool IsChinese(this char c)
    {
        return (c >= '\u4E00' && c <= '\u9FFF')  // CJK 统一汉字
            || (c >= '\u3400' && c <= '\u4DBF')  // 扩展A
            || (c >= '\uF900' && c <= '\uFAFF'); // 兼容汉字
    }

    // 是否大写字母
    public static bool IsCapital(this char c) => char.IsUpper(c);

    // 是否数字（无参数）
    public static bool IsDigit(this char c) => char.IsDigit(c);
}
