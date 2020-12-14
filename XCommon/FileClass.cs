using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XCommon
{
    /// <summary>
    /// 在.Net中，对文件（File）和文件夹（Folder）的操作可以使用File类和Directory类，也可以使用FileInfo类和DirectoryInfo类.
    /// File类和Directory类都是静态类。使用它们的好处是不需要初始化对象。
    /// 如果你对某一个文件或文件夹只进行一次操作，那你最好使用该静态类的静态方法，比如File.Move，File.Delete等等。
    /// 如果你需要对一个文件或文件夹进行多次操作，那最好还是使用FileInfo和DirectoryInfo类。
    /// 因为File类和Directory是静态类，所以你每次对一个文件或文件夹进行操作之前，它们都需要对该文件或文件夹进行一些检查，比如authentication。如果使用FileInfo类和DirectoryInfo类，只在初始化类的对象时进行相关的检查工作，也就是说只需要做一次，所以如果你需要对某个文件或文件夹进行多次操作，那最好使用FileInfo类和DirectoryInfo类。 
    /// </summary>
    public class FileClass
    {
        /// <summary>
        /// 下面的这段代码演示了如何获得文件夹的信息，包括获得文件夹下的子文件夹，以及文件夹下的文件。这里使用了DirectoryInfo 类来完成，当然你也可以使用Directory静态类。
        /// </summary>
        public void DisplayFolder()
        {
            string folderFullName = @"c:\temp";
            DirectoryInfo theFolder = new DirectoryInfo(folderFullName);
            if (!theFolder.Exists)
                throw new DirectoryNotFoundException("Folder not found: " + folderFullName);
            // list all subfolders in folder 
            Console.WriteLine("Subfolders:");
            foreach (DirectoryInfo subFolder in theFolder.GetDirectories())
            {
                Console.WriteLine(subFolder.Name);
            }
            // list all files in folder 
            Console.WriteLine();
            Console.WriteLine("Files:");
            foreach (FileInfo file in theFolder.GetFiles())
            {
                Console.WriteLine(file.Name);
            }
        }

        /// <summary>
        /// 下面演示了如何使用FileInfo类来获得文件的相关信息，包括文件的创建日期，文件的大小等等。当然你同样也可以使用File静态类来完成。
        /// </summary>
        public void DisplayFileInfo()
        {
            string folderFullName = @"c:\temp";
            string fileName = "New Text Document.txt";
            string fileFullName = Path.Combine(folderFullName, fileName);
            FileInfo theFile = new FileInfo(fileFullName);
            if (!theFile.Exists)
                throw new FileNotFoundException("File not found: " + fileFullName);
            Console.WriteLine(string.Format("Creation time: {0}", theFile.CreationTime.ToString()));
            Console.WriteLine(string.Format("Size: {0} bytes", theFile.Length.ToString()));
        }

        /// <summary>
        /// 下面的代码分别使用了File类来演示如何删除文件。
        /// </summary>
        public void DeleteFileForFile()
        {
            string fileToBeDeleted = @"c:\temp\New Text~ Document (3).txt";
            if (File.Exists(fileToBeDeleted))
            {
                File.Delete(fileToBeDeleted);
            }
        }

        /// <summary>
        /// 下面的代码使用FileInfo类来演示如何删除文件。
        /// </summary>
        public void DeleteFileForFileInfo()
        {
            string fileToBeDeleted = @"c:\temp\New Text~ Document (3).txt";
            FileInfo file = new FileInfo(fileToBeDeleted);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        /// <summary>
        /// 下面的代码分别使用了Directory类来演示如何删除文件夹。
        /// </summary>
        public void DeleteFolder1()
        {
            string folderToBeDeleted = @"c:\temp\test";
            if (Directory.Exists(folderToBeDeleted))
            {
                // true is recursive delete: 
                Directory.Delete(folderToBeDeleted, true);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了DirectoryInfo类来演示如何删除文件夹。
        /// </summary>
        public void DeleteFolder2()
        {
            string folderToBeDeleted = @"c:\temp\test";
            DirectoryInfo folder = new DirectoryInfo(folderToBeDeleted);
            if (folder.Exists)
            {
                folder.Delete(true);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了File类来演示如何移动文件。
        /// </summary>
        void MoveFile1()
        {
            string fileToMove = @"c:\temp\New Text Document.txt";
            string fileNewDestination = @"c:\temp\test.txt";
            if (File.Exists(fileToMove) && !File.Exists(fileNewDestination))
            {
                File.Move(fileToMove, fileNewDestination);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了FileInfo类来演示如何移动文件。
        /// </summary>
        void MoveFile2()
        {
            string fileToMove = @"c:\temp\New Text Document.txt";
            string fileNewDestination = @"c:\temp\test.txt";
            FileInfo file = new FileInfo(fileToMove);
            if (file.Exists)
            {
                file.MoveTo(fileNewDestination);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了Directory类来演示如何移动文件夹。
        /// </summary>
        void MoveFolder1()
        {
            string folderToMove = @"c:\temp\test";
            string folderNewDestination = @"c:\temp\test2";
            if (Directory.Exists(folderToMove))
            {
                Directory.Move(folderToMove, folderNewDestination);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了DirectoryInfo类来演示如何移动文件夹。
        /// </summary>
        void MoveFolder2()
        {
            string folderToMove = @"c:\temp\test";
            string folderNewDestination = @"c:\temp\test2";
            DirectoryInfo folder = new DirectoryInfo(folderToMove);
            if (folder.Exists)
            {
                folder.MoveTo(folderNewDestination);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了File类来演示如何复制文件。
        /// </summary>
        public void CopyFile1()
        {
            string sourceFile = @"c:\temp\New Text Document.txt";
            string destinationFile = @"c:\temp\test.txt";
            if (File.Exists(sourceFile))
            {
                // true is overwrite 
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        /// <summary>
        /// 下面的代码分别使用了FileInfo类来演示如何复制文件。
        /// </summary>
        public void CopyFile2()
        {
            string sourceFile = @"c:\temp\New Text Document.txt";
            string destinationFile = @"c:\temp\test.txt";
            FileInfo file = new FileInfo(sourceFile);
            if (file.Exists)
            {
                // true is overwrite 
                file.CopyTo(destinationFile, true);
            }
        }

        /// <summary>
        /// 使用方法：
        /// bool copy = CopyDirectory("c:\\temp\\index\\", "c:\\temp\\newindex\\", true);
        /// 上面的方法将把c:\temp\index目录下的所有子目录和文件复制到 c:\temp\newindex目录下。
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="DestinationPath"></param>
        /// <param name="overwriteexisting"></param>
        /// <returns></returns>
        public static bool CopyDirectory(string SourcePath, string DestinationPath, bool overwriteexisting)
        {
            bool ret = false;
            try
            {
                if (SourcePath.Equals(DestinationPath))
                {
                    return true;
                }
                SourcePath = SourcePath.EndsWith(@"\") ? SourcePath : SourcePath + @"\";
                DestinationPath = DestinationPath.EndsWith(@"\") ? DestinationPath : DestinationPath + @"\";

                if (Directory.Exists(SourcePath))
                {
                    if (Directory.Exists(DestinationPath) == false)
                        Directory.CreateDirectory(DestinationPath);

                    foreach (string fls in Directory.GetFiles(SourcePath))
                    {
                        FileInfo flinfo = new FileInfo(fls);
                        flinfo.CopyTo(DestinationPath + flinfo.Name, overwriteexisting);
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
            catch
            {
                ret = false;
            }
            return ret;
        }

    }
}
