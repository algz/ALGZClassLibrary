using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;


namespace XCommon
{
    /// <summary>
    /// 验证类
    /// </summary>
    public class Verification
    {
        #region  验证是否为整数

        /// <summary>
        /// 验证输入的数据是不是正整数
        /// </summary>
        /// <param name="str">输入字符串</param>
        /// <returns>返回true或者false</returns>
        public static bool IsNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg1 = new System.Text.RegularExpressions.Regex(@"^[0-9]\d*$");
            return reg1.IsMatch(str);
        }
        #endregion

        #region  验证是否为实数
        /// <summary>
        ///  验证输入的数据是否为实数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDoubleNumer(string str)
        {
            string pattern = @"^[-+]?\d+(\.\d+)?$";
            return System.Text.RegularExpressions.Regex.IsMatch(str, pattern);
        }
        #endregion

        #region  检查输入是否非法

        /// <summary>
        /// 检查输入是否非法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsCheckString(string source)
        {
            string[] strArray = new string[] { "[","~","!","@","#","$","%",
                "^","&","*","(",")","=","+","[","\\","]","{","}",
                "''\"",";",":","/","?",".",",",">","<","`","|","！","·",
                "￥",".","—","（","）","\\","-","、","；","：","。","，","》","《","]" };

            foreach (string str in strArray)
            {
                if (source.Contains(str))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查输入是否非法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsCheckSignleString(string source)
        {
            string[] strArray = new string[] { "[","~","!","@","#","$","%",
                "^","&","*","(",")","=","+","[","\\","]","{","}",
                "''\"",";",":","/","?",".",",",">","<","`","|","！","·",
                "￥",".","（","）","\\","、","；","：","。","，","》","《","]" };

            foreach (string str in strArray)
            {
                if (source.Contains(str))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查输入是否非法
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsCheckRemarkString(string source)
        {
            string[] strArray = new string[] { "[","~","@","#","$","%",
                "^","&","*","(",")","=","+","[","\\","]","{","}",
                "''\"","/",">","<","`","|",
                "￥","（","）","》","《","]" };

            foreach (string str in strArray)
            {
                if (source.Contains(str))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 检查Url地址是否正确

        public static bool IsCheckUrl(string str_url)
        {
            Regex rx = new Regex(@"[a-zA-z]+://[^s]*");
            return rx.IsMatch(str_url);
        }
        #endregion

    }
}
