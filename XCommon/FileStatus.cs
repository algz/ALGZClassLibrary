using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace XCommon
{
   public class FileStatus
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        private const int OF_READWRITE = 2;

        private const int OF_SHARE_DENY_NONE = 0x40;

        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);

        public static int FileIsOpen(string fileFullName)
        {
            if (!File.Exists(fileFullName))
            {
                return -1;
            }

            IntPtr handle = _lopen(fileFullName, OF_READWRITE | OF_SHARE_DENY_NONE);

            if (handle == HFILE_ERROR)
            {
                return 1;
            }

            CloseHandle(handle);

            return 0;
        }
    }
}
