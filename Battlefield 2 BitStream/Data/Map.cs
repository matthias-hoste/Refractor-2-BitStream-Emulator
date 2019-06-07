using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.Data
{
    public class Map
    {
        public string MapName { get; set; }
        public string GameMode { get; set; }
        public uint MaxPlayers { get; set; }
    }
}