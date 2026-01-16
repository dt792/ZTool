using ZTool.Sugers;
namespace ZTool.Usages;

internal class ZSugerUsage
{
    public void FunctionalStyleCall()
    {
        int AddThree(int a) => a + 3;
        string ToStr(int a) => a.ToString();
        var c = 2 >> AddThree >> ToStr;
        
    }
    public async void WaitMilliSecond()
    {
        await 10;
    }
    public void IntEnumerator()
    {
        foreach (var i in 10)
        {
            Console.WriteLine(i);
        }
    }
    public void FuncCompose()
    {
        int AddThree(int a) => a + 3;
        string ToStr(int a) => a.ToString();
        var addThree = AddThree;
        var c = addThree + ToStr;
        Console.WriteLine(c(3));
    }
}
