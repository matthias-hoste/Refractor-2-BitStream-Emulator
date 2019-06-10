using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFS;

namespace Battlefield_BitStream.Core.Data
{
    public class Level
    {
        //get map info from here
        public VFileSystem LevelFileSystem { get; private set; }
        public Level(string lvlPath)
        {
            LevelFileSystem = VFileSystemManager.MountArchive("CurrentLevel", Path.Combine(lvlPath, "server.zip"));
        }
        public void InitLevel()
        {
            Config.ConFileProcessor.ExecuteConFile(LevelFileSystem.GetFile("Init.con"));
        }
        public static Level LoadLevel(string name)
        {
            var lvlFile = Path.Combine(Config.ModPath, "Levels", name);
            if (!Directory.Exists(lvlFile))
                return null;
            return new Level(lvlFile);
        }
    }
}