using DBLibrary;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SystemIOLibrary;
using WebLibrary.Ftp;
using DBSingletonCommon = DBLibrary.DBSingletonCommon<Oracle.ManagedDataAccess.Client.OracleConnection, Oracle.ManagedDataAccess.Client.OracleCommand>;

namespace AutoUpdate
{
    public partial class ProgressForm : Form
    {
        private string pid = "";

        private string fileName = "";

        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(string[] args):this()
        {

        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            if (this.pid != "" && this.fileName != "")
            {
                this.Close();
                return;
            }
            //this.pid = args[0];
            //this.fileName = args[1];
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            this.progressBar1.Maximum = 100;
            this.progressBar1.Minimum = 0;
            Thread thread = new Thread((method) =>
            {
                UpdateApp();
            });
            thread.Start();
        }


        public void UpdateApp()
        {
            string serverVersion = GetFileStorageDirPath("ClientAppVersion");
            string loacalVersion = AppConfigLibrary.AppConfigClass.GetValue("ClientAppVersion");
            // "SANY_WINFORM.exe";
            string startFileName = this.fileName; 
            //"D:\\Source\\c#\\三一桩机\\SANY_WINFORM20200802\\SANY_WINFORM\\AutoUpdate\\bin\\Debug\\Update\\"
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            string localPath = appPath + "Update\\";
            this.label1.Text = "开始下载";
            this.progressBar1.Value = 1;
            try
            {
                if (serverVersion != loacalVersion)
                {
                    if (this.pid!=""&&this.fileName!="")
                    {
                        //this.label1.Text = "系统开始更新";
                        //this.progressBar1.Value = 10;
                        //增加两个属性MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly，可以使弹出的MessageBox显示到最前方;
                        MessageBox.Show("准备更新主程序,自动关闭主程序进行更新。点击确定后，开始更新。", "系统提示",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning,
                          MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        //GetWindowThreadProcessId(new IntPtr(Convert.ToInt32(args[0])), out pid);

                        //如果主程序运行中，则自动关闭。
                        int pid = Convert.ToInt32(this.pid);
                        if (pid != 0)
                        {
                            Process process = Process.GetProcessById(pid);
                            process.Kill();
                        }
                    }
                    //else
                    //{
                    //    //MessageBox.Show("主程获取失败,无法更新.");
                    //    //this.Close();
                    //    //return;
                    //}


                    string msg = DownFile(appPath + "update.zip", "/Update/update.zip", null, false, true);

                    if (msg != "" && msg.Contains("失败"))
                    {
                        //失败
                        this.label1.Text = "下载失败，更新结束。";
                        this.progressBar1.Value = 100;
                        return;
                    }
                    else
                    {
                        //成功
                        this.label1.Text = "开始解压";
                        this.progressBar1.Value = 30;

                        ReadOptions options = new ReadOptions();
                        options.Encoding = Encoding.Default;
                        ZipFile zip = ZipFile.Read(appPath + "update.zip", options);
                        int i = 40 / zip.Count, j = 1;
                        foreach (ZipEntry entry in zip)
                        {
                            entry.Extract(localPath, ExtractExistingFileAction.OverwriteSilently);
                            this.label1.Text = "开始解压....." + entry.FileName;
                            this.progressBar1.Value = i * (j++) + 30;
                            Thread.Sleep(500);
                        }

                        
                        int fileCount=FileClass.GetSumFileCountMethod(localPath);
                        i = 30 / fileCount;
                        FileClass.CopyDirectory(localPath, appPath, true,(x,y)=>{
                            this.label1.Text = "文件移动....." + x.Name;
                            this.progressBar1.Value = i * y + 70;
                            Thread.Sleep(500);
                            return true;
                        }) ;
                        //DirectoryInfo di = new DirectoryInfo(localPath);
                        
                        //foreach (FileInfo fi in di.GetFiles())
                        //{
                        //    if (fi.Name.ToLower() == "update.zip")
                        //    {
                        //        continue;
                        //    }
                        //    this.label1.Text = "文件移动....." + fi.Name;
                        //    this.progressBar1.Value = i * (j++) + 70;

                        //    string destFilePath = Path.Combine(appPath, fi.Name);
                        //    if (File.Exists(destFilePath))
                        //    {
                        //        File.Delete(destFilePath);
                        //    }
                        //    fi.MoveTo(destFilePath);
                        //    Thread.Sleep(1500);

                        //}
                        //MessageBox.Show("解压完成。");

                        AppConfigLibrary.AppConfigClass.SetValue("ClientAppVersion", serverVersion);
                    }
                }
                MessageBox.Show("数据更新完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序更新失败;" + ex.Message);
            }
            finally
            {
                FileClass.DeleteFolder(localPath);
            }
            this.Close();
        }

        public string SetProgressLable
        {
            set
            {
                this.label1.Text = value;
            }
        }

        public int SetProgress
        {
            set
            {
                this.progressBar1.Value = value;
            }
        }


        public static string GetFileStorageDirPath(string name)
        {
            string sql = "select path from S_CONFIG_FILESTORAGE where name='" + name + "'";
            return DBSingletonCommon.ExecuteScalar(sql) + "";
        }


        public static string DownFile(string localFilePath, string remoteFilePath, Func<string, string> finishMethod = null, bool isTip = true, bool syncTransmissionMode = false)
        {

            if (localFilePath != "" && remoteFilePath != "")
            {
                string dirPath = Path.GetDirectoryName(localFilePath);
                if (!Directory.Exists(dirPath))
                {
                    CheckDirectoryExist(dirPath);
                }
                string ip = GetFileStorageDirPath("FileStorageDirPath");
                string username = GetFileStorageDirPath("FileStorageUsername");
                string password = GetFileStorageDirPath("FileStoragePassword");
                if (!syncTransmissionMode)
                {
                    //异步传输
                    Thread thread = new Thread((method) =>
                    {
                        FtpClient ftp = new FtpClient(ip, username, password);
                        string msg = ftp.DownloadFile(localFilePath, remoteFilePath, method as Func<string, string>);
                        if (isTip)
                        {
                            MessageBox.Show(msg);
                        }
                    });
                    thread.Start(finishMethod);
                }
                else if (syncTransmissionMode)
                {
                    //同步传输
                    FtpClient ftp = new FtpClient(ip, username, password);
                    string msg = ftp.DownloadFile(localFilePath, remoteFilePath, finishMethod);
                    if (isTip)
                    {
                        MessageBox.Show(msg);
                    }
                    return msg;
                }
            }
            return "";
        }


        /// <summary>
        /// 判断文件的目录是否存在,不存在则创建  
        /// </summary>
        /// <param name="destFilePath"></param>
        public static void CheckDirectoryExist(string destFilePath)
        {
            string fullDir = destFilePath;// Directory.CreateDirectory(destFilePath);
            string[] dirs = fullDir.Split('\\');
            string curDir = "";
            for (int i = 0; i < dirs.Length; i++)
            {

                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "\\";
                        if (i == 0)
                        {
                            continue;
                        }
                        Directory.CreateDirectory(curDir);
                        //FtpMakeDir(curDir);
                    }
                    catch (Exception)
                    { }
                }
            }
        }
    }
}
