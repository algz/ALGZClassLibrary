using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace XCommon
{
    public class XDebug
    {
        private static string _logPath;
        public static string LogPath
        {
            get
            {
                return _logPath;
            }
        }

        static XDebug()
        {
            _logPath = Application.StartupPath + "\\record.log";
        }

        public static void Show(Exception ex)
        {
#if DEBUG
            MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
#else

#endif

            try
            {
                AppendLogFile(ex);
            }
            catch
            {
            }
        }

        public static void AppendLogFile(Exception ex)
        {
            //string strLogPath = Application.StartupPath + "\\record.log";

            string msg = "Time: " + DateTime.Now.ToString() + Environment.NewLine
                + "Message: " + ex.Message + Environment.NewLine
                + "StackTrace: " + ex.StackTrace + Environment.NewLine
                + Environment.NewLine;

            AppendLogFile(msg, LogPath);
        }

        public static void WriteLogFile(string str)
        {
            string strMessage = "Time:" + DateTime.Now.ToString() + Environment.NewLine
                    + "Messgage:" + str + Environment.NewLine;

            AppendLogFile(strMessage,LogPath);
        }

        public static void AppendLogFile(string strLog)
        {
            //string strLogPath = Application.StartupPath + "\\record.log";
            AppendLogFile(strLog, LogPath);
        }

        public static void AppendLogFile(string strLog, string strFilePath)
        {
            File.AppendAllText(strFilePath, strLog);
        }
    }
}
