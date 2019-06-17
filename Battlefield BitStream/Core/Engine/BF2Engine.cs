using Battlefield_BitStream_Common.Data;
using Battlefield_BitStream_Common.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battlefield_BitStream.Core.Engine
{
    public class BF2Engine : IBF2Engine
    {
        public uint ServerPort { get; private set; }
        public List<IMap> MapList { get; private set; }
        public ILevel Level { get; private set; }
        public BF2Engine()
        {
            MapList = new List<IMap>();
        }
        public void InitEngine()
        {
            Console.WriteLine("[ENGINE] Initializing...");
            Console.WriteLine("[ENGINE] Loading ServerSettings...");
            Config.ConFileProcessor.ExecuteConFile(Path.Combine(Application.StartupPath, Config.ModPath, "Settings", "ServerSettings.con"));
            Console.WriteLine("[ENGINE] Loading MapList...");
            Config.ConFileProcessor.ExecuteConFile(Path.Combine(Application.StartupPath, Config.ModPath, "Settings", "maplist.con"));
            Console.WriteLine("[ENGINE] Initialized!");
        }
        public void LoadServerArchives()
        {
            var archivesPath = Path.Combine(Application.StartupPath, Config.ModName, "ServerArchives.con");
            Config.ConFileProcessor.ExecuteConFile(archivesPath);
        }
        public void LoadLevel(IMap map)
        {
            Level = Data.Level.LoadLevel(map);
            //Level.InitLevel();
        }
        public void SetServerPort(uint port)
        {
            ServerPort = port;
        }
    }
}