using System;
using System.Collections.Generic;
using System.Text;
using ZTool.Bases;
using ZTool.Infrastructures;
using ZTool.Structures;

namespace ZTool.Usages;

public static class ZStructuresUsage
{
    class Model
    {
        public string Name { get; set; }
    }
    public static void Clone()
    {
        Model model= new Model();
        Model model2 = model.ShowllaClone();
        Console.WriteLine(model2);
        Model model3 = model.DeepClone();
        Console.WriteLine(model3);
    }
    public static void Memo()
    {
        Model model = new Model();
        model.Name = "1000";
        model.Save();
        model.Name = "90";
        model.Load();
        Console.WriteLine(model.Name);
    }
    public class CCardT : RichEnum<CCardT, Func<object>>
    {
        public static readonly CCardT Standard = new CCardT(() => "maomao", nameof(Standard));
        public static readonly CCardT Higher = new CCardT(() => 3, nameof(Higher));
        public static readonly CCardT Plus = new CCardT(() => new List<int>() { 6 }, nameof(Plus));
        private CCardT(Func<object> value, string name) : base(name, value) { }
    }
    public static void RichEnum()
    {
        Console.WriteLine(CCardT.Higher== CCardT.Higher);
        Console.WriteLine(CCardT.Higher.Value());
    }
}
