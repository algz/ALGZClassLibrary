using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SystemIOLibrary
{
    /// <summary>
    ///       方法一 和 方法二 都可以实现文件夹及文件的复制,两者的区别是:方法一的复制并没有包括原文件的根目录名称(要复制的文件除了根目录文件夹以外其他的都原封不动地搬到了目的地),
    ///       方法二的复制包括了原文件的根目录名称(要复制的文件原封不动的搬到目的地),
    ///       比如:要把E:/123的文件复制到F盘(123文件夹下包含其他文件夹及文件,比如包含了234文件夹和一个1.txt文档),
    ///       如果用方法一,只是把E盘123文件夹下的子文件夹及子文件复制到了F盘,如果用方法二,则是把E盘下的整个123文件夹都复制到了F盘!由此可见,
    ///       方法一 适合重命名复制,方法二 适合直接复制!
    /// </summary>
    public class FileClass
    {
        /// <summary>
        /// 复制文件夹到另一个文件夹
        /// 使用方法：
        ///bool copy = CopyDirectory("c:\\temp\\index\\", "c:\\temp\\newindex\\", true);
        ///上面的方法将把c:\temp\index目录下的所有子目录和文件复制到 c:\temp\newindex目录下。
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <param name="overwriteexisting"></param>
        /// <param name="ProcesssMethod">参数0，FileInfo；参数1，int第几个文件（从1开始）；返回值true 复制，false不复制。</param>
        /// <returns></returns>
        public static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting = true, Func<FileInfo,int, bool> ProcesssMethod=null)
        {
            bool ret = false;
            try
            {
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                int index = 1;
                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        if (ProcesssMethod == null)
                        {
                            flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                        }
                        else if (ProcesssMethod!=null&&ProcesssMethod(flinfo,index++))
                        {
                            flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
                        } 
                    }
                    foreach (string drs in Directory.GetDirectories(SourcePath))
                    {
                        DirectoryInfo drinfo = new DirectoryInfo(drs);
                        if (CopyDirectory(drs, DestinationPath + drinfo.Name, overwriteexisting) == false)
                            ret = false;
                    }
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        ///      方法一 和 方法二 都可以实现文件夹及文件的复制,两者的区别是:方法一的复制并没有包括原文件的根目录名称(要复制的文件除了根目录文件夹以外其他的都原封不动地搬到了目的地),
        ///       方法二的复制包括了原文件的根目录名称(要复制的文件原封不动的搬到目的地),
        ///       比如:要把E:/123的文件复制到F盘(123文件夹下包含其他文件夹及文件,比如包含了234文件夹和一个1.txt文档),
        ///       如果用方法一,只是把E盘123文件夹下的子文件夹及子文件复制到了F盘,如果用方法二,则是把E盘下的整个123文件夹都复制到了F盘!由此可见,
        ///       方法一 适合重命名复制,方法二 适合直接复制!
        ///       
        /// 复制文件夹及文件,不包括原文件的根目录名称(要复制的文件除了根目录文件夹以外其他的都原封不动地搬到了目的地)
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public string CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return "";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        /// <summary>
        /// 复制文件夹及文件,包括了原文件的根目录名称(要复制的文件原封不动的搬到目的地)
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public string CopyFolder2(string sourceFolder, string destFolder)
        {
            try
            {
                string folderName = System.IO.Path.GetFileName(sourceFolder);
                string destfolderdir = System.IO.Path.Combine(destFolder, folderName);
                string[] filenames = System.IO.Directory.GetFileSystemEntries(sourceFolder);
                foreach (string file in filenames)// 遍历所有的文件和目录
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        string currentdir = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(currentdir))
                        {
                            System.IO.Directory.CreateDirectory(currentdir);
                        }
                        CopyFolder2(file, destfolderdir);
                    }
                    else
                    {
                        string srcfileName = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(destfolderdir))
                        {
                            System.IO.Directory.CreateDirectory(destfolderdir);
                        }
                        System.IO.File.Copy(file, srcfileName);
                    }
                }

                return "";
            }
            catch (Exception e)
            {

                return e.Message;
            }

        }

        /// <summary>
        /// 获得文件夹大小.(字节)
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static long GetDirectorySizeMethod(string directory)
        {
            long directorySize = 0;
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
            {
                Console.WriteLine("Directory {0} is not exist!", directory);
                return 0;
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                directorySize += fi.Length;
            }
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo sondir in dirs)
            {
                directorySize += GetDirectorySizeMethod(sondir.FullName);
            }
            return directorySize;
        }

        /// <summary>
        /// 获得文件夹大小.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static long GetDirectorySizeMethod1(string directory)
        {
            long directorySize = 0;
            if (File.Exists(directory))
            {
                FileInfo fi = new FileInfo(directory);
                return fi.Length;
            }
            else
            {
                string[] strs = Directory.GetFileSystemEntries(directory);
                foreach (string str in strs)
                {
                    directorySize += GetDirectorySizeMethod1(str);
                }
            }
            return directorySize;
        }

        /// <summary>
        /// 统计指定文件夹的文件数量.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static int GetSumFileCountMethod(string directory)
        {
            int directoryFileCount = 0;
            DirectoryInfo di = new DirectoryInfo(directory);
            if (!di.Exists)
            {
                Console.WriteLine("Directory {0} is not exist!", directory);
                return 0;
            }

            directoryFileCount += di.GetFiles().Count();
            //foreach (FileInfo fi in di.GetFiles())
            //{
            //    directorySize += fi.Length;
            //}
            DirectoryInfo[] dirs = di.GetDirectories();
            foreach (DirectoryInfo sondir in dirs)
            {
                directoryFileCount += GetSumFileCountMethod(sondir.FullName);
            }
            return directoryFileCount;
        }

        /// <summary>
        /// 删除目录及目录下的文件。
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            if (System.IO.Directory.Exists(dir))
            {
                string[] fileSystemEntries = System.IO.Directory.GetFileSystemEntries(dir);
                for (int i = 0; i < fileSystemEntries.Length; i++)
                {
                    string text = fileSystemEntries[i];
                    if (System.IO.File.Exists(text))
                    {
                        System.IO.File.Delete(text);
                    }
                    else
                    {
                        DeleteFolder(text);
                    }
                }
                System.IO.Directory.Delete(dir);
            }
        }
    }
}
