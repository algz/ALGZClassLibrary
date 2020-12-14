using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZipLibrary
{
    public class ZipEntryEx
    {
        public ZipEntryEx()
        {

        }

        public ZipEntryEx(ZipEntry entry)
        {
            //TChild child = new TChild();
            var parentType = typeof(ZipEntry);
            var childType = typeof(ZipEntryEx);
            foreach (var childPropertie in childType.GetProperties())
            {
                //循环遍历属性
                if (childPropertie.CanRead && childPropertie.CanWrite)
                {
                    foreach (var parentProperty in parentType.GetProperties())
                    {
                        if (childPropertie.Name==parentProperty.Name)
                        {
                            //进行属性拷贝
                            childPropertie.SetValue(this, parentProperty.GetValue(entry, null), null);
                            break;
                        }
                    }
                        
                }
            }
        }

        public static TChild AutoCopy<TParent, TChild>(TParent parent) where TChild : TParent, new()
        {
            TChild child = new TChild();
            var ParentType = typeof(TParent);
            var Properties = ParentType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    //进行属性拷贝
                    Propertie.SetValue(child, Propertie.GetValue(parent, null), null);
                }
            }
            return child;
        }



        //////////////

        public int AESKeySize { get; set; }
        public bool CanDecompress { get; set; }
        public bool CentralHeaderRequiresZip64 { get; set; }
        public string Comment { get; set; }
        public long CompressedSize { get; set; }
        public CompressionMethod CompressionMethod { get; set; }
        public long Crc { get; set; }
        public DateTime DateTime { get; set; }
        public long DosTime { get; set; }
        public int ExternalFileAttributes { get; set; }
        public byte[] ExtraData { get; set; }
        public int Flags { get; set; }
        public bool HasCrc { get; set; }
        public int HostSystem { get; set; }
        public bool IsCrypted { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsDOSEntry { get; set; }
        public bool IsFile { get; set; }
        public bool IsUnicodeText { get; set; }
        public bool LocalHeaderRequiresZip64 { get; set; }
        public string Name { get; set; }
        public long Offset { get; set; }
        public long Size { get; set; }
        public int Version { get; set; }
        public int VersionMadeBy { get; set; }
        public long ZipFileIndex { get; set; }

        //public static string CleanName(string name);
        //public static bool IsCompressionMethodSupported(CompressionMethod method);
        //public object Clone();
        //public void ForceZip64();
        //public bool IsCompressionMethodSupported();
        //public bool IsZip64Forced();
        //public override string ToString();
    }
}
