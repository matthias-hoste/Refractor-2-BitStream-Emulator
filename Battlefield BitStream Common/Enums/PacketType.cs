using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Enums
{
    public enum PacketType : uint
    {
        ConnectionRequest = 1,
        ConnectionAccept = 2,
        ConnectionDenied = 3,
        ConnectionAcknowledge = 4,
        Disconnect = 5,
        PingRequest = 7,
        PingResponse = 8,
        Data = 15
    }
}