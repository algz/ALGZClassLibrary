using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegistryKeyLibrary
{
    /// <summary>
    /// 注册自定义协议，浏览器自动调用本地 EXE.
    /// </summary>
    public class RegistryProtocol
    {

        public static void RegistryURLProtocol(string protocolName, string exeFilePath)
        {
            //[HKEY_CLASSES_ROOT\ProtocolName]
            RegistryKey root=Registry.ClassesRoot.CreateSubKey(protocolName,RegistryKeyPermissionCheck.ReadWriteSubTree);
            //@= "ProtocolNameProtocol"
            root.SetValue("", protocolName + "Protocol");
            //"URL Protocol=D:\Program Files (x86)\Tencent\QQ\Bin\Timwp.exe"
            root.SetValue("URL Protocol", exeFilePath);

            //[HKEY_CLASSES_ROOT\ProtocolName\DefaultIcon]
            RegistryKey DefaultIcon = root.CreateSubKey("DefaultIcon");
            //@= "D:\Program Files (x86)\Tencent\QQ\Bin\Timwp.exe,1"
            DefaultIcon.SetValue("", exeFilePath + ",1");

            //[HKEY_CLASSES_ROOT\ProtocolName\DefaultIcon]
            RegistryKey command = root.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
            //@= "\"D:\Program Files (x86)\Tencent\QQ\Bin\Timwp.exe\" \"%1\""
            //这个"%1"是传递给 Timwp.exe的参数。浏览器中输入 algz://123 ;Timwp.exe只接收到一个参数,args[0]="algz://123".
            command.SetValue("", "\""+ exeFilePath + "\" \"%1\"");
        }
    }
}
