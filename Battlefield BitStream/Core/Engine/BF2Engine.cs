using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battlefield_BitStream.Core.Engine
{
    public class BF2Engine
    {
        public void InitEngine()
        {
            
        }
        public void LoadServerArchives()
        {
            var archivesPath = Path.Combine(Application.StartupPath, Config.ModName, "ServerArchives.con");
            Config.ConFileProcessor.ExecuteConFile(archivesPath);
        }
    }
}