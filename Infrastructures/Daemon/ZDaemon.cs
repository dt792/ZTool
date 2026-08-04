using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ZTool.Infrastructures.Daemon;

public class ZDaemon
{
	public static void Daemon(int id)
	{
		ManualResetEventSlim manualLock = new ManualResetEventSlim(initialState: false);
		Process processById;
		string path;
		try
		{
			processById = Process.GetProcessById(id);
			path = processById.MainModule?.FileName ?? throw new InvalidOperationException("无法获取进程主模块路径");
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Console.WriteLine("分析被守护进程出错->退出");
			return;
		}
		string disk = Path.GetPathRoot(path).Replace("\\", "");
		string dirPath = Path.GetDirectoryName(path);
		string exe = Path.GetFileName(path);
		processById.EnableRaisingEvents = true;
		processById.Exited += OnProcessExited;
		manualLock.Wait();
		void OnProcessExited(object? sender, EventArgs e)
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			process.StandardInput.WriteLine("cd\\");
			process.StandardInput.WriteLine(disk);
			process.StandardInput.WriteLine("cd " + dirPath);
			process.StandardInput.WriteLine("start " + exe);
			manualLock.Set();
		}
	}
}
