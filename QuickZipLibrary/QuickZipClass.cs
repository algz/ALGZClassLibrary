using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QuickZipLibrary
{
    public class QuickZipClass
    {
        /// <summary>  
        /// 所有文件缓存  
        /// </summary>  
        List<string> files = new List<string>();

        /// <summary>  
        /// 所有空目录缓存  
        /// </summary>  
        List<string> paths = new List<string>();

        /// <summary>
        /// 解压密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>  
        /// 压缩单个文件  
        /// </summary>  
        /// <param name="fileToZip">要压缩的文件</param>  
        /// <param name="zipedFile">压缩后的文件全名</param>  
        /// <param name="compressionLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>  
        /// <param name="blockSize">分块大小</param>  
        public void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            if (!System.IO.File.Exists(fileToZip))//如果文件没有找到，则报错  
            {
                throw new FileNotFoundException("The specified file " + fileToZip + " could not be found. Zipping aborderd");
            }

            FileStream streamToZip = new FileStream(fileToZip, FileMode.Open, FileAccess.Read);
            FileStream zipFile = File.Create(zipedFile);
            ZipOutputStream zipStream = new ZipOutputStream(zipFile);
            ZipEntry zipEntry = new ZipEntry(fileToZip);
            zipStream.PutNextEntry(zipEntry);
            zipStream.SetLevel(compressionLevel);
            byte[] buffer = new byte[blockSize];
            int size = streamToZip.Read(buffer, 0, buffer.Length);
            zipStream.Write(buffer, 0, size);

            try
            {
                while (size < streamToZip.Length)
                {
                    int sizeRead = streamToZip.Read(buffer, 0, buffer.Length);
                    zipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                throw ex;
            }

            zipStream.Finish();
            zipStream.Close();
            streamToZip.Close();
            GC.Collect();
        }

        /// <summary>  
        /// 压缩目录（包括子目录及所有文件）  
        /// </summary>  
        /// <param name="originDirPath">要压缩的源目录路径,目录以“\”结束。</param>  
        /// <param name="destinationFilePath">保存文件路径</param>  
        /// <param name="compressLevel">压缩程度，范围0-9，数值越大，压缩程序越高</param>  
        public void ZipFileFromDirectory(string originDirPath, string destinationFilePath, int compressLevel = 9, Action<FileStream> progressFun = null)
        {
            GetAllDirectories(originDirPath, destinationFilePath);

            /* while (rootPath.LastIndexOf("\\") + 1 == rootPath.Length)//检查路径是否以"\"结尾 
            { 

            rootPath = rootPath.Substring(0, rootPath.Length - 1);//如果是则去掉末尾的"\" 

            } 
            */

            //string rootMark = rootPath.Substring(0, rootPath.LastIndexOf("\\") + 1);//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。  
            string rootMark = originDirPath;//得到当前路径的位置，以备压缩时将所压缩内容转变成相对路径。  
            //Crc32 crc = new Crc32();
            using (Stream stream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
            {
                using (ZipOutputStream outPutStream = new ZipOutputStream(stream))
                {
                    if (this.Password != null)
                    {
                        outPutStream.Password = this.Password;
                    }
                    outPutStream.SetLevel(compressLevel); // 0 - store only to 9 - means best compression  
                    foreach (string file in files)
                    {
                        //不能把目标文件压缩到压缩文件中.
                        if (file == destinationFilePath)
                        {
                            continue;
                        }

                        using (FileStream fileStream = File.OpenRead(file))
                        {
                            //打开压缩文件  
                            byte[] buffer = new byte[fileStream.Length];
                            fileStream.Read(buffer, 0, buffer.Length);
                            DirectoryInfo di = new DirectoryInfo(rootMark);
                            //ZipEntry entry = new ZipEntry(file.Replace(rootMark, string.Empty));
                            ZipEntry entry = new ZipEntry(file.Replace(di.Parent.FullName + "\\", string.Empty));
                            //entry.Name = "";
                            entry.DateTime = DateTime.Now;


                            progressFun?.Invoke(fileStream);

                            entry.Size = fileStream.Length;
                            //fileStream.Close();
                            //crc.Reset();
                            //crc.Update(buffer);
                            //entry.Crc = crc.Value;
                            outPutStream.PutNextEntry(entry);
                            outPutStream.Write(buffer, 0, buffer.Length);
                        }
                    }

                    this.files.Clear();

                    foreach (string emptyPath in paths)
                    {
                        ZipEntry entry = new ZipEntry(emptyPath.Replace(rootMark, string.Empty) + "/");
                        outPutStream.PutNextEntry(entry);
                    }

                    this.paths.Clear();
                    outPutStream.Finish();
                    outPutStream.Close();
                }
            }
            GC.Collect();
        }

        /// <summary>  
        /// 取得目录下所有文件及文件夹，分别存入files及paths  
        /// </summary>  
        /// <param name="rootPath">根目录</param>  
        private void GetAllDirectories(string rootPath, params string[] exclusiveFiles)
        {
            string[] subPaths = Directory.GetDirectories(rootPath);//得到所有子目录  
            foreach (string path in subPaths)
            {
                GetAllDirectories(path);//对每一个字目录做与根目录相同的操作：即找到子目录并将当前目录的文件名存入List  
            }
            string[] files = Directory.GetFiles(rootPath);
            foreach (string file in files)
            {
                //不包含本地文件
                if (exclusiveFiles.Contains(file))
                {
                    continue;
                }
                this.files.Add(file);//将当前目录中的所有文件全名存入文件List  
            }
            if (subPaths.Length == files.Length && files.Length == 0)//如果是空目录  
            {
                this.paths.Add(rootPath);//记录空目录  
            }
        }

        /// <summary>  
        /// 解压缩文件(压缩文件中含有子目录)  
        /// </summary>  
        /// <param name="zipfilepath">待解压缩的文件路径.(目录必须以"\\"结尾.)</param>  
        /// <param name="unzippath">解压缩到指定目录</param>
        /// <param name="selectFileNames">解压的文件(集)</param>
        /// <returns>解压后的文件列表</returns>  
        public List<string> UnZip(string zipfilepath, string unzippath, Func<ZipEntryEx, string> method = null, Action<List<string>> completeMethod = null)
        {
            //解压出来的文件列表  
            List<string> unzipFiles = new List<string>();

            //检查输出目录是否以“\\”结尾  
            //if (unzippath.EndsWith("\\") == false || unzippath.EndsWith(":") == true)
            //{
            //    unzippath += "\\";
            //}

            using (ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(zipfilepath)))
            {
                if (this.Password != null)
                {
                    zipInputStream.Password = this.Password;
                }

                ZipEntry theEntry;
                while ((theEntry = zipInputStream.GetNextEntry()) != null)
                {
                    //获取目录,即去掉结尾"\\".
                    if (unzippath.Last() != '\\')
                    {
                        unzippath += "\\";
                    }
                    string directoryName = Path.GetDirectoryName(unzippath);
                    string fileName = Path.GetFileName(theEntry.Name);
                    string destFilePath = unzippath + theEntry.Name;

                    ZipEntryEx zipEx = new ZipEntryEx(theEntry);
                    //zipEx = ZipEntryEx.AutoCopy<ZipEntry, ZipEntryEx>(theEntry);


                    //生成解压目录【用户解压到硬盘根目录时，不需要创建】  
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        //如果文件的压缩后大小为0那么说明这个文件是空的,因此不需要进行读出写入  
                        //if (theEntry.CompressedSize == 0)
                        //    continue;


                        //回调方法,提取需要的文件.
                        if (method != null)
                        {
                            string tem = method(zipEx);
                            if (tem != null && tem != "")
                            {
                                destFilePath = tem;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        //解压文件到指定的目录，并建立指定的目录和子目录  
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));


                        //记录导出的文件  
                        unzipFiles.Add(destFilePath);



                        using (FileStream streamWriter = File.Create(destFilePath))
                        {
                            int size = 2048;
                            byte[] buffer = new byte[2048];
                            do
                            {
                                size = zipInputStream.Read(buffer, 0, buffer.Length); //返回读取缓冲区的字节数。如果读取到流的末尾，则返回0。
                                streamWriter.Write(buffer, 0, size);
                            } while (size > 0);
                        }
                    }
                }
            }

            //zipInputStream.Close();
            GC.Collect();
            completeMethod?.Invoke(unzipFiles);
            return unzipFiles;
        }

        public string GetZipFileExtention(string fileFullName)
        {
            int index = fileFullName.LastIndexOf(".");
            if (index <= 0)
            {
                throw new Exception("源包文件不是压缩文件");
            }

            //extension string
            string ext = fileFullName.Substring(index);

            if (ext == ".rar" || ext == ".zip")
            {
                return ext;
            }
            else
            {
                //The source package file is not a compress file
                throw new Exception("源包文件不是压缩文件");
            }
        }

        /// <summary>
        /// 根据压缩包路径读取此压缩包内文件个数
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <returns></returns>
        public static int CountZipFile(string zipFilePath)
        {
            ZipFile zipFile = null;
            try
            {
                using (zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(zipFilePath))
                {
                    long l_New = zipFile.Count;
                    return Convert.ToInt32(l_New);
                }
            }
            catch
            {
                return 0;
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.Close();
                }
            }
        }

        class md5
        {
            #region "MD5加密"
            /// <summary>
            ///32位 MD5加密
            /// </summary>
            /// <param name="str">加密字符</param>
            /// <returns></returns>
            public static string encrypt(string str)
            {
                string cl = str;
                string pwd = "";
                MD5 md5 = MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                for (int i = 0; i < s.Length; i++)
                {
                    pwd = pwd + s[i].ToString("X");
                }
                return pwd;
            }
            #endregion
        }
    }
}
