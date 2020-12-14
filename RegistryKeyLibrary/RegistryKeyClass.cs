using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegistryKeyLibrary
{
    /// <summary>
    /// 如果提示注册表权限问题.
    /// 在后缀为manifest的文件中,
    /// <requestedExecutionLevel  level="asInvoker" uiAccess="false" /> 
    /// 更改为
    /// <requestedExecutionLevel  level="requireAdministrator" uiAccess="false" /> 
    /// 这样程序在运行时,都会弹出UAC提示是否以管理员身份运行.
    /// 
    /// OpenSubKey ( string name )方法主要是打开指定的子键。
    //    GetSubKeyNames()方法是获得主键下面的所有子键的名称，它的返回值是一个字符串数组。
    //GetValueNames()方法是获得当前子键中的所有的键名称，它的返回值也是一个字符串数组。
    //GetValue(string name)方法是指定键的键值。
    /// </summary>
    public class RegistryKeyClass
    {
        /// <summary>
        /// Win10 读写LocalMachine权限，没有访问权限
        /// Registry.ClassesRoot     对应于HKEY_CLASSES_ROOT主键
        /// egistry.CurrentUser 对应于HKEY_CURRENT_USER主键
        //Registry.LocalMachine 对应于 HKEY_LOCAL_MACHINE主键
        //Registry.User             对应于 HKEY_USER主键
        //Registry.CurrentConfig 对应于HEKY_CURRENT_CONFIG主键
        //Registry.DynDa 对应于HKEY_DYN_DATA主键
        //Registry.PerformanceData 对应于HKEY_PERFORMANCE_DATA主键
        /// </summary>
        public static RegistryKey RegKeyRoot = Registry.CurrentUser;

        /// <summary>
        /// 创建子项,如果已经存在则不影响.
        /// </summary>
        /// <param name="subName"></param>
        /// <returns></returns>
        public static RegistryKey CreateSub(string sub = "ALGZ")
        {
            RegistryKey software = RegKeyRoot.OpenSubKey("SOFTWARE", true);
            return software.CreateSubKey(sub);//如果已经存在则不影响.
        }

        public static void SetRegKey(string sub, string key, string val)
        {
            //string RegeditPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\";  
            //RegistryKey software = RegKeyRoot.OpenSubKey(RegeditPath, true);
            RegistryKey software = RegKeyRoot.OpenSubKey(sub, true);
            software.SetValue(key, val);
            RegKeyRoot.Close();
        }

        public static string GetRegKey(string sub, string key, string val)
        {
            //注意该方法后面还可以有一个布尔型的参数，true表示可以写入。
            //注意，如果该注册表项不存在，这调用这个方法会抛出异常,sub该项必须已存在
            RegistryKey software = RegKeyRoot.OpenSubKey(sub, true);
            return software.GetValue(key, "") + "";
        }

        /// <summary>
        /// 注意，如果该注册表项不存在，这调用这个方法会抛出异常
        /// </summary>
        /// <returns></returns>
        public static void DelRegKey(string key)
        {
            RegKeyRoot.DeleteSubKey(key, true); //该方法无返回值，直接调用即可  
            RegKeyRoot.Close();
        }

        /// <summary>
        /// 判断注册表项是否存在
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="cSub"></param>
        /// <returns></returns>
        private bool IsRegeditItemExist(string sub, string cSub)
        {
            string[] subkeyNames;
            RegistryKey software = RegKeyRoot.OpenSubKey(sub, true);
            subkeyNames = software.GetSubKeyNames();
            //取得该项下所有子项的名称的序列，并传递给预定的数组中
            foreach (string keyName in subkeyNames) //遍历整个数组
            {
                if (keyName == cSub) //判断子项的名称
                {
                    RegKeyRoot.Close();
                    return true;
                }
            }
            RegKeyRoot.Close();
            return false;
        }

        /// <summary>
        /// 判断键值是否存在
        /// </summary>
        /// <returns></returns>
        private bool IsRegeditKeyExit(string sub, string key)
        {
            string[] subkeyNames;
            RegistryKey software = RegKeyRoot.OpenSubKey(sub, true);
            subkeyNames = software.GetValueNames();
            //取得该项下所有键值的名称的序列，并传递给预定的数组中  
            foreach (string keyName in subkeyNames)
            {
                if (keyName == key) //判断键值的名称
                {
                    RegKeyRoot.Close();
                    return true;
                }
            }
            RegKeyRoot.Close();
            return false;
        }



    }
}
