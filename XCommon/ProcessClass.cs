using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace XCommon
{
    /// <summary>
    /*
     1、Process基本属性和方法
        Id　　　　　　　　　　//进程的Id
        ProcessName  　　　　//进程的名称
        PriorityClass　　　　　　//进程的优先级
        HandleCount　　　　　　//进程句柄数
        PrivateMemorySize64      //关联的进程分配的专用内存量
        WorkingSet64        //工作集，关联的进程分配的物理内存量
        StartInfo　　　　　　//进程启动的相关属性
        GetProcesses()   //获取当前系统的所有进程
        Start()     //启动进程
        Kill();　　　　//强行杀死进程
         */
    /// </summary>
    public class ProcessClass
    {
        public const int WM_CLOSE = 0x10;
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")] 
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);

        /// <summary>
        /// 通过句柄获得进程标识符。
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static Process GetProcessForHwnd(IntPtr hwnd)
        {
            int pid;
            Process process=null;
            try
            {
                GetWindowThreadProcessId(hwnd, out pid);
                if (pid != 0)
                {
                    process = Process.GetProcessById(pid);
                }
                return process;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 关闭窗口。向指定句柄的窗口，发送关闭消息。
        /// </summary>
        /// <param name="hWnd"></param>
        public static void CloseWindow(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_CLOSE, 0, 0);
        }

        /// <summary>
        /// 封装到 Process 对象。
        /// 启用进程
        /// GetProcesses()   //获取当前系统的所有进程
        ///Start()     //启动进程
        ///Kill();　　　　//强行杀死进程
        /// </summary>
        public static Process process(string filePath)
        {
            Process p;//实例化一个Process对象
            p = Process.Start(filePath);//要开启的进程（或 要启用的程序），括号内为绝对路径
            //p.Kill();//结束进程
            return p;
        }

        /// <summary>
        /// 执行Cmd
        /// </summary>
        /// <param name="argument">cmd命令</param>
        /// <param name="msg">返回信息</param>
        /// <param name="directoryPath">路径</param>
        /// <param name="closed">是否关闭</param>
        public static void RunCmd(string argument, out string msg, string directoryPath = "", bool redirect = false)
        {
            msg = string.Empty;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = redirect ? @"/c " + argument : @"/k " + argument;
            startInfo.UseShellExecute = false;                        //是否需要启动windows shell
            startInfo.CreateNoWindow = false;
            startInfo.RedirectStandardError = redirect;    //是否重定向错误
            startInfo.RedirectStandardInput = redirect;    //是否重定向输入   是则不能在cmd命令行中输入
            startInfo.RedirectStandardOutput = redirect;      //是否重定向输出,是则不会在cmd命令行中输出
            startInfo.WorkingDirectory = directoryPath;       //指定当前命令所在文件位置，
            process.StartInfo = startInfo;
            process.Start();
            if (redirect)
            {
                process.StandardInput.Close();
                msg = process.StandardOutput.ReadToEnd();  //在重定向输出时才能获取
            }
            //else
            //{
            //    process.WaitForExit();//等待进程退出
            //}
        }

        /// <summary>
        /// 启动exe
        /// 这里需要注意三点，1、是重定向，重定向为true时方可获取返回值，并且要求命令行退出。2、WorkingDirectory，在调用命令行时需注意指定工作目录，默认都是当前程序启动目录。(特别是那些需要在指定目录运行的注册exe，当然他算exe范畴了，但会弹出命令行显示进度，所以再此提一下)3、当程序为控制台时，系统为默认在控制台上执行命令行命令。
        /// </summary>
        /// <param name="filePath">程序路径</param>
        /// <param name="argument">参数</param>
        /// <param name="waitTime">等待时间，毫秒计</param>
        public void RunExe(string filePath, string argument, int waitTime = 0)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("filePath is empty");
            }
            if (!File.Exists(filePath))
            {
                throw new Exception(filePath + " is not exist");
            }
            string directory = Path.GetDirectoryName(filePath);

            try
            {
                Process p = new Process();
                p.StartInfo.FileName = filePath;
                p.StartInfo.WorkingDirectory = directory;
                p.StartInfo.Arguments = argument;
                p.StartInfo.ErrorDialog = false;
                //p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;//与CreateNoWindow联合使用可以隐藏进程运行的窗体
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.EnableRaisingEvents = true;                      // 启用Exited事件
                //p.Exited += p_Exited;
                p.Start();
                if (waitTime > 0)
                {
                    p.WaitForExit(waitTime);
                }

                if (p.ExitCode == 0)//正常退出
                {
                    //TODO记录日志
                    System.Console.WriteLine("执行完毕！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("系统错误：", ex);
            }

        }

        /// <summary>
        /// 这里要求指定执行文件的绝对地址，当需要执行系统环境配置好的执行文件时就不好了，还要知道他的详细路径。如若这样，可以考虑下面简单的方式.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="argument"></param>
        public void RunSysExe(string filePath, string argument)
        {
            Process p = new Process();

            p.StartInfo.FileName = filePath;        // "iexplore.exe";   //IE
            p.StartInfo.Arguments = argument;// "http://www.baidu.com";
            p.Start();
        }
    }
}
