using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFS
{
    public class VFileSystem
    {
        private List<string> _directories { get; set; }
        private Dictionary<string, ZipArchiveEntry> _files { get; set; }
        public VFileSystem()
        {
            _directories = new List<string>();
            _files = new Dictionary<string, ZipArchiveEntry>();
        }
        public static VFileSystem Create(string zip)
        {
            Console.WriteLine("[VFS] Loading " + zip + "...");
            ZipArchive zipData = new ZipArchive(new MemoryStream(File.ReadAllBytes(zip)));
            var fileSystem = new VFileSystem();
            foreach(var zipEntry in zipData.Entries)
            {
                fileSystem.AddFile(zipEntry);
            }
            return fileSystem;
        }
        public void AddFile(ZipArchiveEntry file)
        {
            if (_files.ContainsKey(file.FullName))
                return;
            _files.Add(file.FullName, file);
        }
        public void AddZip(string zip)
        {
            Console.WriteLine("[VFS] Loading " + zip + "...");
            ZipArchive zipData = new ZipArchive(new MemoryStream(File.ReadAllBytes(zip)));
            foreach (var zipEntry in zipData.Entries)
            {
                AddFile(zipEntry);
            }
        }
        public VFile GetFile(string file)
        {
            if (!_files.ContainsKey(file))
                return null;
            return VFile.FromZipEntry(_files[file]);
        }
        public VFile[] GetFilesByExtension(string extension)
        {
            var fileList = new List<VFile>();
            foreach(var vfile in _files)
            {
                if (vfile.Key.EndsWith(extension))
                    fileList.Add(VFile.FromZipEntry(vfile.Value));
            }
            return fileList.ToArray();
        }
        public void DeleteFileSystem()
        {

        }
    }
}