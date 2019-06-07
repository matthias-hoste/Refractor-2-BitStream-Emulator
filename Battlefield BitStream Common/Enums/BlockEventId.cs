using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Enums
{
    public enum BlockEventId : uint
    {
        ServerInfo = 0,
        ClientInfo = 1,
        MapInfo = 2,
        MapList = 5
    }
}