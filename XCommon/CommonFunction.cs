using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Xml;
using System.Data;
using System.Data.OleDb;



namespace XCommon
{
    /// <summary>
    /// 常用函数
    /// </summary>
    public class CommonFunction
    {
        #region 错误提示信息

        private static string m_error_info = string.Empty;
        public static string mErrorInfo
        {
            get
            {
                return m_error_info;
            }
        }

        #endregion

        private CommonFunction() { }

        #region 将文件 读入到 List String 中。
        /// <summary>
        /// 将文件 读入到 List String 中。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="fileContents">读取的文件的内容</param>
        /// <returns></returns>
        public static bool mReadFileToListString(string filename, List<string> fileContents)
        {
            if (System.IO.File.Exists(filename) == false)
            {
                return false;
            }
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filename, Encoding.Default))
                {
                    while (sr.Peek() != -1)
                    {
                        fileContents.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// 将文件 读入到 List String 中。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="fileContents">读取的文件的内容</param>
        /// <returns></returns>
        public static bool mReadFileToListStringBasic(string filename, List<string> fileContents)
        {
            if (System.IO.File.Exists(filename) == false)
            {
                return false;
            }
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filename, Encoding.UTF8))
                {
                    while (sr.Peek() != -1)
                    {
                        fileContents.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                return true;
            }
            catch { return false; }
        }

        #endregion

        #region 将文件 读入到 List String 中。
        /// <summary>
        /// 将文件 读入到 List String 中。
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="fileContents">读取的文件的内容</param>
        /// <param name="remove">忽略以该字符串开头的数据行</param>
        /// <returns></returns>
        public static bool mReadFileToListString(string filename, List<string> fileContents, string remove)
        {
            if (System.IO.File.Exists(filename) == false)
            {
                m_error_info = "文件不存在：" + filename;
                return false;
            }
            try
            {
                string str;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filename, Encoding.Default))
                {
                    while (sr.Peek() != -1)
                    {
                        str = sr.ReadLine();
                        if (str.Trim().StartsWith(remove) == false)
                            fileContents.Add(str);
                    }
                    sr.Close();
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region 将List String中的数据写入到文件中
        /// <summary>
        /// 将List String中的数据写入到文件中(生成新文件)
        /// </summary>
        public static bool mWriteListStringToFile(string filename, List<string> fileContents)
        {
            try
            {
                StringBuilder s = new StringBuilder();
                foreach (string str in fileContents)
                    s.Append(str);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(s.ToString());
                StringBuilder sub = new StringBuilder();
                XmlTextWriter xw = new XmlTextWriter(new StringWriter(sub));
                xw.Formatting = Formatting.Indented;
                doc.WriteTo(xw);
                doc.Save(filename);

                //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, Encoding.Default))
                //{


                //    foreach (string str in fileContents)
                //        sw.WriteLine(str);

                //    sw.Flush();
                //    sw.Close();
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将List String中的数据写入到文件中(生成新文件)
        /// </summary>
        public static bool mWriteListStringToFileBasic(string filename, List<string> fileContents)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, false, Encoding.UTF8))
                {
                    foreach (string str in fileContents)
                        sw.WriteLine(str);
                    sw.Flush();
                    sw.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 将List String中的数据写入到文件中(可以控制生成新文件或者追加到文件)
        /// <summary>
        /// 将List String中的数据写入到文件中(可以控制生成新文件或者追加到文件)
        /// </summary>
        public static bool mWriteListStringToFile(string filename, bool append, List<string> fileContents)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filename, append, Encoding.Default))
                {
                    foreach (string str in fileContents)
                        sw.WriteLine(str);
                    sw.Flush();
                    sw.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 将数字转换为字符串
        /// <summary>
        /// 将float 转换为字符串
        /// </summary>
        public static string mDoubleToString8(float _d)
        {
            if (_d < 999999 && _d >= 99999)
            {
                return string.Format("{0:F1}", _d).PadRight(8, ' ');
            }
            else if (_d < 99999 && _d >= 9999)
            {
                return string.Format("{0:F2}", _d).PadRight(8, ' ');
            }
            else if (_d < 9999 && _d > 0.01)
            {
                return string.Format("{0:F3}", _d).PadRight(8, ' ');
            }
            else if (_d < -0.01 && _d >= -999)
            {
                return string.Format("{0:F3}", _d).PadRight(8, ' ');
            }
            else if (_d < -999 && _d > -9999)
            {
                return string.Format("{0:F2}", _d).PadRight(8, ' ');
            }
            else
            {
                string str = string.Format("{0:0.00E+00}", _d);
                if (str.Length > 8)
                    return str.Replace("E", "").PadRight(8, ' ');
                else return str.PadLeft(8, ' ');
            }
        }
        //<summary>
        //将float 转换为字符串
        //</summary>
        public static string mDoubleToString10(float _d)
        {
            return _d.ToString().PadRight(10, ' ');
        }
        //<summary>
        //将float 转换为字符串
        //</summary>
        public static string mDoubleToString16(float _d)
        {
            return _d.ToString().PadRight(16, ' ');
        }
        /// <summary>
        /// 将int 转换为字符串
        /// </summary>
        public static string mIntToString8(int _i)
        {
            if (_i == 0)
            {
                return "0       ";
            }
            else
            {
                return string.Format("{0,7:D}", _i).PadRight(8, ' ');
            }
        }

        /// <summary>
        /// 返回空格字符串，长度8
        /// </summary>
        public static string mSpaceString8()
        {
            //            12345678
            string str = "        ";
            return str;
        }
        public static string mSpaceString16()
        {
            //            1234567812345678
            string str = "                ";
            return str;
        }
        public static string mStrToString8(string _str)
        {
            return _str.PadRight(8, ' ');
        }
        public static string mStrToString10(string _str)
        {
            return _str.PadRight(10, ' ');
        }
        public static string mStrToString16(string _str)
        {
            return _str.PadRight(16, ' ');
        }

        public static string mStrModifyToString8(int count)
        {
            string str = "        ";
            string strString = string.Empty;
            for (int i = 0; i < count; i++)
            {
                strString += str;
            }
            return strString;
        }
        #endregion

        #region 将字符串截取为数组

        /// <summary>
        /// 将字符串截取为数组
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public static string[] SplitStringToArray(string strContent)
        {
            string[] strTempArray = strContent.Split(' ');
            List<string> lstContent = new List<string>();

            foreach (string str in strTempArray)
            {
                if (str != string.Empty)
                {
                    lstContent.Add(str);
                }
            }

            return lstContent.ToArray();
        }

        #endregion

 
        public static bool IsFileOpened(string file)
        {
            bool result = false;
            try
            {
                FileStream fs = File.OpenWrite(file);
                fs.Close();
            }
            catch 
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetParaType(int index)
        {
            string strType = string.Empty;

            if (index == 0)
            {
                strType = "指标参数";
            }

            if (index == 1)
            {
                strType = "构型和总体参数";
            }
            if (index == 2)
            {
                strType = "旋翼参数";
            }

            if (index == 3)
            {
                strType = "机身翼面参数";
            }

            if (index == 4)
            {
                strType = "着陆装置参数";
            }

            if (index == 5)
            {
                strType = "动力系统参数";
            }

            if (index == 6)
            {
                strType = "传动系统参数";
            }
            if (index == 7)
            {
                strType = "操纵系统参数";
            }

            if (index == 8)
            {
                strType = "人工参数";
            }

            if (index == 9)
            {
                strType = "其他类型参数";
            }

            return strType;
        }

        public static string GetStringContent(string[] strArray)
        {
            string strContent = string.Empty;
            if (strArray.Length == 3)
            {
                strContent = strArray[2];
            }
            if (strArray.Length == 2)
            {
                strContent = strArray[1];
            }

            return strContent;
        }

        public static ArrayList InitColor()
        {
            ArrayList ArColor = new ArrayList();
            //初始化颜色数组   

            ArColor.Clear();
            for (int i = 0; i < 5; i++)
            {
                ArColor.Add(Color.Gold);
                ArColor.Add(Color.Green);
                ArColor.Add(Color.Red);
                ArColor.Add(Color.Indigo);
                ArColor.Add(Color.Blue);
                ArColor.Add(Color.Brown);
                ArColor.Add(Color.Yellow);
                ArColor.Add(Color.Cornsilk);
                ArColor.Add(Color.Pink);
                ArColor.Add(Color.YellowGreen);
                ArColor.Add(Color.Firebrick);
                ArColor.Add(Color.DimGray);
                ArColor.Add(Color.Aqua);
                ArColor.Add(Color.Indigo);
                ArColor.Add(Color.DarkSeaGreen);
            }

            return ArColor;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="strDirPath"></param>
        public static void CreateDirectory(string strDirPath)
        {
            if (!Directory.Exists(strDirPath))
            {
                Directory.CreateDirectory(strDirPath);
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strContent"></param>
        public static void CreateFile(string strFilePath, string strContent)
        {
            if (!File.Exists(strFilePath))
            {
                List<string> lstContent = new List<string>();
                lstContent.Add(strContent);
                mWriteListStringToFile(strFilePath, lstContent);
            }
        }



    }
}
