using System.Text.Json;
using ZTool.Bases;

namespace ZTool.Structures;

public class FileSingleton<T> where T : new()
{
    private static T _instance;
    private static readonly object _syslock = new object();
    static string SaveDir { get; set; } = "Singletons";
    static string SavePath { get => SaveDir is not null ? $"{SaveDir}/{typeof(T).Name}.txt" : $"{typeof(T).Name}.txt"; }
    public static bool IsThrowException { get; set; } = false;
    public static T Instance
    {
        get
        {
            if (_instance == null)//两重锁结构
            {
                lock (_syslock)
                {
                    if (_instance == null)
                    {
                        string runtimeContextInJson;
                        try
                        {

                            runtimeContextInJson = File.ReadAllText(SavePath);
                            try
                            {
                                _instance = JsonTool.RefDeserialize<T>(runtimeContextInJson!);
                            }
                            catch (Exception ex)
                            {
                                _instance = new T();
                                if (IsThrowException)
                                    throw ex;
                            }
                        }
                        catch (IOException ex)
                        {
                            _instance = new T();
                            if (IsThrowException)
                                throw ex;
                        }
                    }
                }
            }
            return _instance;
        }
    }

    public static void Save()
    {
        if (!Directory.Exists(SaveDir))
            Directory.CreateDirectory(SaveDir);
        try
        {
            string json = JsonTool.RefSerialize(Instance!);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception ex)
        {
            if (IsThrowException)
                throw ex;
        }

    }
    public static void DeleteFile()
    {
        try
        {
            File.Delete(SavePath);
        }
        catch (Exception ex)
        {
            if (IsThrowException)
                throw ex;
        }

    }
}
