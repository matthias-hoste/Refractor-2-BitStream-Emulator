using Battlefield_BitStream.Core.Game;
using Battlefield_BitStream_Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFS;

namespace Battlefield_BitStream.Core.Data
{
    public class Level : ILevel
    {
        public IMap Map { get; private set; }
        public World GameWorld { get; private set; }
        public VFileSystem LevelFileSystem { get; private set; }
        public Level(IMap map)
        {
            Map = map;
            GameWorld = new World();
            LevelFileSystem = VFileSystemManager.MountArchive("CurrentLevel", Path.Combine(Config.ModPath, "Levels", Map.MapName, "server.zip"));
        }
        public void InitLevel()
        {
            Config.ConFileProcessor.ExecuteConFile(LevelFileSystem.GetFile("CompiledRoads.con"));
            Config.ConFileProcessor.ExecuteConFile(LevelFileSystem.GetFile("AmbientObjects.con"));
            Config.ConFileProcessor.ExecuteConFile(LevelFileSystem.GetFile("StaticObjects.con"));
        }
        public static Level LoadLevel(IMap map)
        {
            var lvlFile = Path.Combine(Config.ModPath, "Levels", map.MapName);
            if (!Directory.Exists(lvlFile))
                return null;
            return new Level(map);
        }
    }
}