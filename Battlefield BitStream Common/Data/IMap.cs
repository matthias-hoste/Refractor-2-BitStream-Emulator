using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Data
{
    public interface IMap
    {
        string MapName { get; set; }
        string GameMode { get; set; }
        uint MaxPlayers { get; set; }
    }
}