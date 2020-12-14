using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WebLibrary.Ftp
{
    public enum TransmissionMode
    {
        Synchronous, Asynchronous
    }

    public class FtpClient
    {
        private FtpWebRequest ftpWebRequest;

        public FtpWebRequest FtpWebRequest
        {
            get
            {
                return ftpWebRequest;
            }
        }

        private string ftpServerName { get; set; }

        private string loginName { get; set; }

        private string password { get; set; }

        public FtpClient(string ftpServerName, string loginName, string password)
        {
            this.ftpServerName = ftpServerName;
            this.loginName = loginName;
            this.password = password;

        }

        /// <summary>
        /// 测试FTP服务器是否连接成功
        /// </summary>
        /// <param name="path">FTP服务器.例:ftp://algz-pc</param>
        /// <param name="Login">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="useBinary">指定数据传输类型,true 二进制传输（默认）;false 文本传输。</param>
        /// <returns></returns>
        public string Connect(bool useBinary=true)
        {
            try
            {
                // 根据uri创建FtpWebRequest对象
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + this.ftpServerName));
                //指定命令
                ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                // 指定数据传输类型
                ftpWebRequest.UseBinary = useBinary;
                // ftp用户名和密码
                ftpWebRequest.Credentials = new NetworkCredential(this.loginName, this.password);
                //
                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();
                return "FTP连接成功."+response.BannerMessage;
            }
            catch (System.Net.WebException ex)
            {
                return   ex.Message;
            }
        }


        /// <summary>
        /// 从FTP服务器下载文件. 同步。
        /// </summary>
        /// <param name="localFilePath">本地存储文件路径(全路径,含目录和文件名)</param>
        /// <param name="remoteFilePath">远程FTP服务器文件路径(包括目录和文件名)</param>
        /// <param name="Login">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns></returns>
        public string DownloadFile(string localFilePath, string remoteFilePath, Func<string, string> finishMethod = null, Action<long, long> progressMethod = null)
        {
            string msg = "";
            try
            {
                //下载目录+文件名
                //string newFileName = localfilePath + Path.GetFileName(remoteFileName);
                //string remoteFileName = Path.GetFileName(remoteFilePath);
                //if (File.Exists(localFilePath))
                //{
                //    string errorinfo = string.Format("文件{0}在该目录下已存在,无法下载", remoteFileName);
                //    //return errorinfo;
                //}

                // 必须放在前面获取，不然可能会出现无法获取文件大小，超时。
                long cl = this.GetFileSize(remoteFilePath);

                string uri = "ftp://" + this.ftpServerName + remoteFilePath;

                // 根据uri创建FtpWebRequest对象
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));

                // 指定数据传输类型
                ftpWebRequest.UseBinary = true;

                ftpWebRequest.UsePassive = true;

                // 上传文件时通知服务器文件的大小
                //ftpWebRequest.ContentLength = this.GetFileSize(remoteFilePath);

                // ftp用户名和密码
                ftpWebRequest.Credentials = new NetworkCredential(this.loginName, this.password);

                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();


                int bufferSize = 2048;
                int readCount;
                long curLen = 0;
                byte[] buffer = new byte[bufferSize];
                using (Stream ftpStream = response.GetResponseStream())
                {

                    readCount = ftpStream.Read(buffer, 0, bufferSize);

                    using (FileStream outputStream = new FileStream(localFilePath, FileMode.Create))
                    {
                        while (readCount > 0)
                        {
                            outputStream.Write(buffer, 0, readCount);
                            readCount = ftpStream.Read(buffer, 0, bufferSize);

                            curLen += bufferSize;
                            progressMethod?.Invoke(curLen, cl);
                        }
                    }
                }
                response.Close();
                msg = string.Format("服务器文件{0}已成功下载", Path.GetFileName(localFilePath));
                return msg;
            }
            catch (Exception ex)
            {
                //errorinfo
                msg = string.Format("因{0},无法下载", ex.Message);
                return msg;
            }
            finally
            {
                //下载完成时调用.
                finishMethod?.Invoke(msg);
            }
        }

        /// <summary>
        /// 上传文件到FTP服务器.(同步)
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFilePath"></param>
        /// <returns></returns>
        public string UploadFile(string localFilePath, string remoteFilePath, Action<string> finishMethod = null, Action<long, long> progressMethod = null)
        {
            string msg = "";
            try
            {
                //检查目录是否存在，不存在创建  
                FtpCheckDirectoryExist(remoteFilePath);
                FileInfo fileInf = new FileInfo(localFilePath);
                //判断是否有上级目录
                string uri = "ftp://" + ftpServerName + remoteFilePath;
                // 根据uri创建FtpWebRequest对象
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));

                // 指定数据传输类型
                ftpWebRequest.UseBinary = true;

                // ftp用户名和密码
                ftpWebRequest.Credentials = new NetworkCredential(this.loginName, password);

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行
                ftpWebRequest.KeepAlive = false;

                // 指定执行什么命令
                ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                // 上传文件时通知服务器文件的大小
                ftpWebRequest.ContentLength = fileInf.Length;

                // 缓冲大小设置为kb 
                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;
                long curLen = 0;

                // 打开一个本地文件(System.IO.FileStream) 去读上传的文件
                using (FileStream fs = fileInf.OpenRead())
                {
                    // 把上传的文件写入本地文件
                    using (Stream strm = ftpWebRequest.GetRequestStream())
                    {
                        // 每次读文件流的kb 
                        contentLen = fs.Read(buff, 0, buffLength);

                        // 流内容没有结束
                        while (contentLen != 0)
                        {
                            // 把内容从file stream 写入upload stream 
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                            curLen += buffLength;
                            progressMethod?.Invoke(curLen, fileInf.Length);
                        }
                    }
                }
                //// 关闭两个流
                //strm.Close();

                //fs.Close();
                //Successinfo
                msg = string.Format("本地文件{0}已成功上传", fileInf.Name);
                return msg;
            }

            catch (Exception ex)
            {
                //ErrorInfo
                msg = "上传失败" + ex.Message;
                return msg;
            }
            finally
            {
                //下载完成时调用.
                finishMethod?.Invoke(msg);
            }
        }

        /// <summary>
        ///  改名
        /// </summary>
        /// <param name="sourceRemoteFilePath">本地文件及目录名(全路径)</param>
        /// <param name="destRemoteFilePath">
        /// FTP文件路径(含文件名).
        /// 语法:/路径/文件
        /// 例:/test/abc.txt</param>
        /// <param name="login">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public string Rename(string sourceRemoteFilePath, string destRemoteFilePath)
        {
            try
            {
                //检查目录是否存在，不存在创建  
                FtpCheckDirectoryExist(destRemoteFilePath);
                //FileInfo fileInf = new FileInfo(sourceRemoteFilePath);
                //判断是否有上级目录
                string uri = "ftp://" + ftpServerName + destRemoteFilePath;
                // 根据uri创建FtpWebRequest对象
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));

                // 指定数据传输类型
                ftpWebRequest.UseBinary = true;

                // ftp用户名和密码
                ftpWebRequest.Credentials = new NetworkCredential(this.loginName, password);

                // 默认为true，连接不会被关闭.在一个命令之后被执行
                ftpWebRequest.KeepAlive = false;

                // 指定执行什么命令
                ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;

                ftpWebRequest.RenameTo = sourceRemoteFilePath;

                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();
                Stream strm = response.GetResponseStream();

                // 关闭两个流
                strm.Close();

                response.Close();
                //Successinfo
                return string.Format("文件{0}已更改为{1}.", sourceRemoteFilePath, destRemoteFilePath);
            }

            catch (Exception ex)
            {
                //ErrorInfo
                return "更名失败" + ex.Message;
            }
        }

        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="sourceRemoteFilePath"></param>
        /// <param name="destRemoteFilePath"></param>
        /// <returns></returns>
        public string CopyFile(string sourceRemoteFilePath, string destRemoteFilePath)
        {
            try
            {
                FtpCheckDirectoryExist(destRemoteFilePath);
                // 根据uri创建FtpWebRequest对象
                FtpWebRequest sourceFtpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + this.ftpServerName + sourceRemoteFilePath));
                FtpWebRequest destFtpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + this.ftpServerName + destRemoteFilePath));

                // 指定数据传输类型
                sourceFtpWebRequest.UseBinary = true;
                destFtpWebRequest.UseBinary = true;
                // ftp用户名和密码
                sourceFtpWebRequest.Credentials = new NetworkCredential(this.loginName, this.password);
                destFtpWebRequest.Credentials = new NetworkCredential(this.loginName, this.password);
                sourceFtpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                destFtpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

                FtpWebResponse response = (FtpWebResponse)sourceFtpWebRequest.GetResponse();
                using (Stream outFtpStream = response.GetResponseStream())
                {
                    //long cl = response.ContentLength;
                    //int bufferSize = 2048;
                    byte[] buffer = new byte[2048];
                    int readCount = outFtpStream.Read(buffer, 0, buffer.Length);

                    // 把上传的文件写入流
                    using (Stream inFtpStream = destFtpWebRequest.GetRequestStream())
                    {
                        while (readCount > 0)
                        {
                            inFtpStream.Write(buffer, 0, readCount);
                            readCount = outFtpStream.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                response.Close();
                return string.Format("文件{0}已成功复制到{1}.", sourceRemoteFilePath, destRemoteFilePath);
            }

            catch (Exception ex)
            {
                //ErrorInfo
                return "上传失败" + ex.Message;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="FtpPath"></param>
        /// <returns></returns>
        public string DeleteFile(string fileName, string FtpPath)
        {
            try
            {
                FileInfo fileInf = new FileInfo(fileName);
                string uri = "ftp://" + Path.Combine(FtpPath, fileInf.Name);

                // 根据uri创建FtpWebRequest对象
                ftpWebRequest = (FtpWebRequest)WebRequest.Create(new Uri(uri));

                // 指定数据传输类型
                ftpWebRequest.UseBinary = true;

                // ftp用户名和密码
                ftpWebRequest.Credentials = new NetworkCredential(this.loginName, this.password);

                // 默认为true，连接不会被关闭

                // 在一个命令之后被执行
                ftpWebRequest.KeepAlive = false;

                // 指定执行什么命令
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();

                response.Close();

                //Successinfo
                return string.Format("文件{0}已成功删除", fileInf.Name);
            }

            catch (Exception ex)
            {

                //ErrorInfo 
                return string.Format("文件因{0},无法删除", ex.Message);
            }
        }

        //判断文件的目录是否存在,不存在则创建  
        public void FtpCheckDirectoryExist(string destFilePath)
        {
            string fullDir = FtpParseDirectory(destFilePath);
            string[] dirs = fullDir.Split('/');
            string curDir = "/";
            for (int i = 0; i < dirs.Length; i++)
            {
                string dir = dirs[i];
                //如果是以/开始的路径,第一个为空    
                if (dir != null && dir.Length > 0)
                {
                    try
                    {
                        curDir += dir + "/";
                        FtpMakeDir(curDir);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        public string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        //创建目录  
        public string FtpMakeDir(string remoteDir)
        {
            string uri = "ftp://" + this.ftpServerName + remoteDir;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri);

            req.Credentials = new NetworkCredential(this.loginName, this.password);
            req.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                req.Abort();
                return ex.Message;
            }
            req.Abort();
            return "";
        }


        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file">ip服务器下的相对路径</param>
        /// <returns>文件大小</returns>
        public long GetFileSize(string file)
        {
            StringBuilder result = new StringBuilder();
            //FtpWebRequest request;
            try
            {
                string uri = "ftp://" + this.ftpServerName + file;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);

                //request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path + file));
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(this.loginName, this.password);//设置用户名和密码
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                long dataLength = request.GetResponse().ContentLength;//(单位：字节)
                Console.Out.WriteLine(file + ":" + dataLength + "字节");
                return dataLength;
            }
            catch (Exception ex)
            {
                Console.WriteLine("获取文件大小出错：" + ex.Message);
                return -1;
            }
        }
    }
}
