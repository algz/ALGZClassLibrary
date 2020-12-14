using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UtilityLibrary
{
    /// <summary>
    ///  程序中调用，如下：

//    [csharp]
//    view plaincopy


//ConsoleHelper.hideConsole();  

//3. 注意：如果程序是只能启动一个，则可以用上面的方法控制控制台的显示与隐藏；否则需要在初始化时对控制台的标题赋值，如下：

//[csharp]
//    view plaincopy


//Console.Title = Guid.NewGuid().ToString();
    /// </summary>
    class ConsoleClass
    {
        /// <summary>  
        /// 获取窗口句柄  
        /// </summary>  
        /// <param name="lpClassName"></param>  
        /// <param name="lpWindowName"></param>  
        /// <returns></returns>  
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>  
        /// 设置窗体的显示与隐藏  
        /// </summary>  
        /// <param name="hWnd"></param>  
        /// <param name="nCmdShow"></param>  
        /// <returns></returns>  
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        /// <summary>  
        /// 隐藏控制台  
        /// </summary>  
        /// <param name="ConsoleTitle">控制台标题(可为空,为空则取默认值)</param>  
        public static void hideConsole(string ConsoleTitle = "")
        {
            ConsoleTitle = String.IsNullOrEmpty(ConsoleTitle) ? Console.Title : ConsoleTitle;
            IntPtr hWnd = FindWindow("ConsoleWindowClass", ConsoleTitle);
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 0);
            }
        }

        /// <summary>  
        /// 显示控制台  
        /// </summary>  
        /// <param name="ConsoleTitle">控制台标题(可为空,为空则去默认值)</param>  
        public static void showConsole(string ConsoleTitle = "")
        {
            ConsoleTitle = String.IsNullOrEmpty(ConsoleTitle) ? Console.Title : ConsoleTitle;
            IntPtr hWnd = FindWindow("ConsoleWindowClass", ConsoleTitle);
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 1);
            }
        }
    }
}
