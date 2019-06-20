using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFS
{
    public class VFile
    {
        public string FileName { get; set; }
        public Stream FileData { get; set; }
        public long FileDataLength { get; set; }
        public static VFile FromZipEntry(ZipArchiveEntry zipEntry)
        {
            var file = new VFile();
            file.FileName = zipEntry.Name;
            file.FileData = zipEntry.Open();
            file.FileDataLength = zipEntry.Length;
            return file;
        }
        public string GetExtension()
        {
            return Path.GetExtension(FileName);
        }
        public string[] ReadAllLines()
        {
            var list = new List<string>();
            using(var sr = new StreamReader(FileData))
            {
                while(!sr.EndOfStream)
                    list.Add(sr.ReadLine());
            }
            return list.ToArray();
        }
        public byte[] ReadAllBytes()
        {
            using(var mem = new MemoryStream())
            {
                FileData.CopyTo(mem);
                return mem.ToArray();
            }
        }
        public void Delete()//doesnt delete actual file, only the memory reference
        {
            FileName = "";
            FileData.Dispose();
        }
    }
}