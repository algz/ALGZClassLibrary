using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XCommon
{
    public class IniFileClass
    {
        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);

        /// <summary>
        /// ini文件格式：
        /// [section]
        ///  key=value
        /// </summary>
        /// <param name="section">要读取的段落名</param>
        /// <param name="key">要读取的键</param>
        /// <param name="def">读取异常情况下的缺省值</param>
        /// <param name="retVal">key所对应的value值，如果该key不存在则返回空值</param>
        /// <param name="size">value值允许的大小(1024)</param>
        /// <param name="filePath"> ini文件的完整路径和文件名</param>
        /// <returns></returns>
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);


        #endregion

        #region 读Ini文件

        /// <summary>
        /// ini文件其实就是一个文本文件，它有固定的格式，Section节的名字用[]括起来，然后换行说明key的值：
        /// [section]
        ///  key=value
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Key"></param>
        /// <param name="def">默认情况下的缺省值.</param>
        /// <param name="iniFilePath"></param>
        /// <returns></returns>
        public static string ReadIniData(string Section, string Key, string def, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder val = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, def, val, 1024, iniFilePath);
                return val.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region 写Ini文件

        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion


    }
}
