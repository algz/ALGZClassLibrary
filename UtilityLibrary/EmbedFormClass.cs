using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UtilityLibrary
{

    public class EmbedFormClass
    {
        [DllImport("User32.dll", EntryPoint = "SetParent")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        const int WS_THICKFRAME = 262144;
        const int WS_BORDER = 8388608;
        const int GWL_STYLE = -16;

        /// <summary>
        /// 把需要嵌入的EXE,嵌入到主窗体中的Panel.
        /// </summary>
        /// <param name="mainPanel">主窗体中的嵌入者Panel</param>
        /// <param name="embedProcess">被嵌入的EXE</param>
        /// <returns></returns>
        public static string EmbedPanel(Panel mainPanel, Process embedProcess)
        {
            try
            {
                //Process proApp = new Process();
                //string fileName = Application.StartupPath + "\\CallTest.exe";
                //fileName = @"C:\Users\algz\Downloads\Compressed\abaqus-online-0.5.2_2\abaqus-online-0.5.2\client\abaqus-online.exe";
                //embedProcess.StartInfo.FileName = fileName;
                embedProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                embedProcess.Start();
                embedProcess.WaitForInputIdle();

                while (embedProcess.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                    embedProcess.Refresh();
                }
                IntPtr wnd = embedProcess.MainWindowHandle;

                Int32 wndStyle = GetWindowLong(wnd, GWL_STYLE);
                wndStyle &= ~WS_BORDER;
                wndStyle &= ~WS_THICKFRAME;
                SetWindowLong(wnd, GWL_STYLE, wndStyle);

                SetParent(wnd, mainPanel.Handle);
                ShowWindow(wnd, (int)ProcessWindowStyle.Maximized);

                //mainPanel.Tag = embedProcess;
                return "";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }

    }
}
