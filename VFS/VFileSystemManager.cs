using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFS.Core.Extensions;

namespace VFS
{
    public static class VFileSystemManager
    {
        private static Dictionary<string, VFileSystem> _mounts { get; set; }
        static VFileSystemManager()
        {
            _mounts = new Dictionary<string, VFileSystem>();
        }
        public static VFileSystem MountArchive(string mountPoint, string zip)
        {
            if (!_mounts.ContainsKey(mountPoint))
                _mounts.Add(mountPoint, VFileSystem.Create(zip));
            else
                _mounts[mountPoint].AddZip(zip);
            return _mounts[mountPoint];
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
        public static VFile GetFileByName(string file)
        {
            var ind = file.IndexOf('.');
            var files = GetFilesByExtension(file.Substring(ind));
            foreach(var vfile in files)
            {
                if (vfile.FileName.Contains(file))
                    return vfile;
            }
            return null;
        }
        public static VFile GetFile(string file)
        {
            var ind = file.IndexOf('/');
            if (ind == -1)
                return GetFileByName(file);//works since only 1 level is loaded
            var mount = file.Substring(0, ind);
            mount = mount.FirstCharToUpper();//hacky fix
            if (!_mounts.ContainsKey(mount))
                return null;
            return _mounts[mount].GetFile(file.Substring(ind + 1));
        }
    }
}