using DBLibrary;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebLibrary.Ftp;
using DBSingletonCommon = DBLibrary.DBSingletonCommon<Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess.Client.OracleCommand>;


namespace AutoUpdate
{
    public class Program
    {
        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        public static void Main(string[] args)
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ProgressForm(args));
            //Form f = new ProgressForm();
            //f.Show();



            //try
            //{
            //    System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            //    string serverVersion = GetFileStorageDirPath("ClientAppVersion");
            //    string loacalVersion = AppConfigLibrary.AppConfigClass.GetValue("ClientAppVersion");
            //    if (serverVersion != loacalVersion)
            //    {
            //        if (args.Length > 1)
            //        {
            //            string startFileName = args[1]; // "SANY_WINFORM.exe";
            //            int pid = Convert.ToInt32(args[0]);
            //            增加两个属性MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly，可以使弹出的MessageBox显示到最前方;
            //            MessageBox.Show("准备更新主程序,自动关闭主程序进行更新。", "系统提示",
            //              MessageBoxButtons.OK, MessageBoxIcon.Warning,
            //              MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            //            GetWindowThreadProcessId(new IntPtr(Convert.ToInt32(args[0])), out pid);


            //            string localPath = AppDomain.CurrentDomain.BaseDirectory + "Update\\";
            //            MessageBox.Show("准备下载。");
            //            string msg = DownFile(localPath + "update.zip", "/Update/update.zip", null, false, true);
            //            if (msg != "" && msg.Contains("失败"))
            //            {
            //                失败
            //                    MessageBox.Show(msg);
            //                return;
            //            }
            //            else
            //            {
            //                成功

            //                如果主程序运行中，则自动关闭。
            //                    if (pid != 0)
            //                {
            //                    Process process = Process.GetProcessById(pid);
            //                    process.Kill();
            //                }
            //                MessageBox.Show("开始解压。");
            //                ReadOptions options = new ReadOptions();
            //                options.Encoding = Encoding.Default;
            //                ZipFile zip = ZipFile.Read(localPath + "update.zip", options);
            //                foreach (ZipEntry entry in zip)
            //                {
            //                    entry.Extract(localPath, ExtractExistingFileAction.OverwriteSilently);
            //                }
            //                MessageBox.Show("解压完成。");



            //                AppConfigLibrary.AppConfigClass.SetValue("ClientAppVersion", serverVersion);
            //            }




            //        }
            //        else
            //        {
            //            MessageBox.Show("主程获取失败,无法更新.");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("程序更新失败;" + ex.Message);
            //}

            //MessageBox.Show("数据更新完成！");
        }


        private static void UpdateApplication(string localPath, string startFileName, string serverVersion)
        {
            //获取目录下的所有文件
            //string[] files = Directory.GetFiles(localPath);// dir =new Directory();// Path.GetDirectoryName();
            //if (files.Length > 0)
            //{
            //    //判断目录下是否有.zip文件
            //    string[] arr = files.Where(x => Path.GetFileName(x) == "update.zip").ToArray();
            //    if (arr.Length > 0)
            //    {
            //        //解压.zip文件.
            //        ZipLibrary.ZipClass zip = new ZipLibrary.ZipClass();
            //        List<string> unzipFiles = zip.UnZip(arr[0], Path.GetDirectoryName(arr[0]), null, (x) =>
            //        {
            //            foreach (string filePath in x)
            //            {
            //                File.Copy(filePath, System.AppDomain.CurrentDomain.BaseDirectory + Path.GetFileName(filePath), true);
            //                //File.Delete(filePath);
            //            }

            //            //删除更新文件夹
            //            Directory.Delete(Path.GetDirectoryName(arr[0]), true);

            //            AppConfigLibrary.AppConfigClass.SetValue("ClientAppVersion", serverVersion);
            //            MessageBox.Show("程序更新完成,准备开启主程序.", "系统提示",
            //                  MessageBoxButtons.OK, MessageBoxIcon.Warning,
            //                  MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

            //            Process pro = new Process();
            //            pro.StartInfo.FileName = startFileName;
            //            pro.Start();
            //        });


            //    }
            //}
        }

       
    }

}
