using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ProcessLibrary
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
    public class ProcessCommon
    {
        public const int WM_CLOSE = 0x10;
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32", EntryPoint = "GetWindowThreadProcessId")] 
        private static extern int GetWindowThreadProcessId(IntPtr hwnd, out int pid);

        [DllImport("user32", EntryPoint = "IsWindowVisible")]
        private static extern int IsWindowVisible(IntPtr hwnd);

        public static int IsWindowVisible1(IntPtr hwnd)
        {
            return IsWindowVisible(hwnd);
        }

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
        /// 关闭窗口。向指定句柄的窗口，发送关闭消息。
        /// </summary>
        /// <param name="hWnd"></param>
        public static void KillWindow(IntPtr hWnd)
        {
            GetProcessForHwnd(hWnd).Kill();
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

        #region ShellExecute

        /// <summary>
        /// 显示方式
        /// </summary>
        public enum ShowCommands : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        /// <summary>
        /// WinApi 中 ShellExecute 的功能是运行一个外部程序（或者是打开一个已注册的文件、打开一个目录、打印一个文件等等），并对外部程序有一定的控制。函数如下：
        /// </summary>
        /// <param name="hwnd">指定父窗口句柄，未指定时可以为 null 或者为 0</param>
        /// <param name="lpOperation">
        ///lpOperation：指定操作, 值可以为【open】、【print】、【explore】，释义如下：
        ///open   ：执行由 lpFile 参数指定的程序，或打开由 lpFile 参数指定的文件或文件夹；
        ///print  ：打印由 lpFile 参数指定的文件；
        ///explore：浏览由 lpFile 参数指定的文件夹。
        ///
        ///当参数设为 null 时，默认为 open。
        /// </param>
        /// <param name="lpFile">指定要打开的文件或程序</param>
        /// <param name="lpParameters">给要打开的程序指定参数; 如果打开的是文件，值为 null</param>
        /// <param name="lpDirectory">默认目录</param>
        /// <param name="nShowCmd">
        ///SW_HIDE            = 0;  //隐藏
        ///SW_SHOWNORMAL      = 1;  //用最近的大小和位置显示, 激活
        ///SW_NORMAL          = 1;  //同 SW_SHOWNORMAL
        ///SW_SHOWMINIMIZED   = 2;  //最小化, 激活
        ///SW_SHOWMAXIMIZED   = 3;  //最大化, 激活
        ///SW_MAXIMIZE        = 3;  //同 SW_SHOWMAXIMIZED
        ///SW_SHOWNOACTIVATE  = 4;  //用最近的大小和位置显示, 不激活
        ///SW_SHOW            = 5;  //同 SW_SHOWNORMAL
        ///SW_MINIMIZE        = 6;  //最小化, 不激活
        ///SW_SHOWMINNOACTIVE = 7;  //同 SW_MINIMIZE
        ///SW_SHOWNA          = 8;  //同 SW_SHOWNOACTIVATE
        ///SW_RESTORE         = 9;  //同 SW_SHOWNORMAL
        ///SW_SHOWDEFAULT     = 10; //同 SW_SHOWNORMAL
        ///SW_MAX             = 10; //同 SW_SHOWNORMAL
        /// </param>
        /// <returns>
        /// 返回值大于 32 时，即执行成功。执行失败的返回值具体意义如下：
        /// 0                      = 0   //内存不足
        ///ERROR_FILE_NOT_FOUND   = 2;  //文件名错误
        ///ERROR_PATH_NOT_FOUND   = 3;  //路径名错误
        ///ERROR_BAD_FORMAT       = 11; //EXE 文件无效
        ///SE_ERR_SHARE           = 26; //发生共享错误
        ///SE_ERR_ASSOCINCOMPLETE = 27; //文件名不完全或无效
        ///SE_ERR_DDETIMEOUT      = 28; //超时
        ///SE_ERR_DDEFAIL         = 29; //DDE 事务失败
        ///SE_ERR_DDEBUSY         = 30; //正在处理其他 DDE 事务而不能完成该 DDE 事务
        ///SE_ERR_NOASSOC         = 31; //没有相关联的应用程序
        /// </returns>
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, ShowCommands nShowCmd);

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        //private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);//

        //[DllImport("user32", CharSet = CharSet.Ansi, EntryPoint = "FindWindowA", ExactSpelling = false, SetLastError = true)]
        //public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        //[DllImport("Oleacc.dll")]
        //public static extern int AccessibleObjectFromWindow(int hwnd, uint dwObjectID, byte[] riid, ref Microsoft.Office.Interop.Excel.Window ptr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="lpOperation">
        ///lpOperation：指定操作, 值可以为【open】、【print】、【explore】，释义如下：
        ///open   ：执行由 lpFile 参数指定的程序，或打开由 lpFile 参数指定的文件或文件夹；
        ///print  ：打印由 lpFile 参数指定的文件；
        ///explore：浏览由 lpFile 参数指定的文件夹。
        /// </param>
        /// <returns></returns>
        public static string ShellExecute(string filepath,string lpOperation= "open")
        {
            IntPtr result = ShellExecute(IntPtr.Zero, lpOperation, filepath, "", "", ShowCommands.SW_SHOWNORMAL);

            if (result.ToInt32() <= 32)
            {
                return "打开失败";
            }
            return result.ToString();
        }

        #endregion

    }
}
