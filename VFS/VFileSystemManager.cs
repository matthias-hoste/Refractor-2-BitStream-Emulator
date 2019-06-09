using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFS
{
    public static class VFileSystemManager
    {
        private static Dictionary<string, VFileSystem> _mounts { get; set; }
        static VFileSystemManager()
        {
            _mounts = new Dictionary<string, VFileSystem>();
        }
        public static void MountArchive(string mountPoint, string zip)
        {
            if (!_mounts.ContainsKey(mountPoint))
                _mounts.Add(mountPoint, VFileSystem.Create(zip));
            else
                _mounts[mountPoint].AddZip(zip);
        }
        public static void UnMountArchive(string mountPoint)
        {
            if(!_mounts.ContainsKey(mountPoint))
                throw new Exception("Not mounted");
            _mounts[mountPoint].DeleteFileSystem();
            _mounts.Remove(mountPoint);
        }
        public static VFile[] GetFilesByExtension(string extension)
        {
            var _fileList = new List<VFile>();
            foreach (var mount in _mounts)
            {
                _fileList.AddRange(mount.Value.GetFilesByExtension(extension));
            }
            return _fileList.ToArray();
        }
    }
}