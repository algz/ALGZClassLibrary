using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCommon
{
    public class ReflectionClass
    {
        /// <summary>
        /// 通过控件名称查找指定窗体的控件。
        /// </summary>
        /// <param name="form">指定窗体</param>
        /// <param name="controlName">控件名称</param>
        /// <returns></returns>
        public static object getControlForName(Form form,string controlName)
        {
            object obj = form.GetType().GetField(controlName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.IgnoreCase).GetValue(form);
            return obj;
        }
    }
}
