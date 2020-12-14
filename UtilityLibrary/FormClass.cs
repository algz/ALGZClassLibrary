using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UtilityLibrary
{
    public class FormClass
    {
        public static void CloseForm(FormClosingEventArgs e)
        {
            //switch (e.CloseReason)
            //{
            //    //应用程序要求关闭窗口
            //    case CloseReason.ApplicationExitCall:
            //        e.Cancel = false; //不拦截，响应操作
            //        break;
            //    //自身窗口上的关闭按钮
            //    case CloseReason.FormOwnerClosing:
            //        e.Cancel = true;//拦截，不响应操作
            //        break;
            //    //MDI窗体关闭事件
            //    case CloseReason.MdiFormClosing:
            //        e.Cancel = true;//拦截，不响应操作
            //        break;
            //    //不明原因的关闭
            //    case CloseReason.None:
            //        break;
            //    //任务管理器关闭进程
            //    case CloseReason.TaskManagerClosing:
            //        e.Cancel = false;//不拦截，响应操作
            //        break;
            //    //用户通过UI关闭窗口或者通过Alt+F4关闭窗口
            //    case CloseReason.UserClosing:
            //        if (Monitor.MonitorClass.State == 1)
            //        {
            //            e.Cancel = true;//拦截，不响应操作
            //            MessageBox.Show("静力学监控中,有静力学任务在计算,不允许用户关闭主程序.");
            //        }

            //        break;
            //    //操作系统准备关机
            //    case CloseReason.WindowsShutDown:
            //        e.Cancel = false;//不拦截，响应操作
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
