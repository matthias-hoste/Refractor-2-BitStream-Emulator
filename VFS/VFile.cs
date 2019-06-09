using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFS
{
    public class VFile
    {
        public string FileName { get; set; }
        public static VFile FromZipEntry(ZipArchiveEntry zipEntry)
        {
            var file = new VFile();
            file.FileName = zipEntry.Name;
            return file;
        }
    }
}