using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace XCommon
{
    class WindowsFormClass
    {
        /// <summary>
        /// 获取窗体的句柄函数
        /// </summary>
        /// <param name="lpClassName">窗口类名</param>
        /// <param name="lpWindowName">窗口标题名</param>
        /// <returns>返回句柄</returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 通过句柄，窗体显示函数
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="cmdShow">显示方式</param>
        /// <returns>返工成功与否</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindowAsync", SetLastError = true)]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);


        /// <summary>
        /// 通过句柄设置方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWindowHander_Click(object sender, EventArgs e)
        {
            // 获取查找窗体句柄(通过窗体标题名)
            IntPtr mainHandle = FindWindow(null, "演示窗体");
            if (mainHandle != IntPtr.Zero)
            {
                //通过句柄设置当前窗体最大化（0：隐藏窗体，1：默认窗体，2：最小化窗体，3：最大化窗体，....）
                bool result = ShowWindowAsync(mainHandle, 3);
            }
        }
    }
}
