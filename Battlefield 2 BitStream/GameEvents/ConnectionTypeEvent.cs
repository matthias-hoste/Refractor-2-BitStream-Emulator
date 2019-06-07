using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.GameEvents
{
    public class ConnectionTypeEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(3, 7);
            stream.WriteBits(0, 3);
        }

        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Process(INetworkingClient client)
        {

        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            int v2 = 0;
            do
                ++v2;
            while (((1 << v2) - 1) < 7);
            uint somthing = stream.ReadBits((uint)v2);
            return new ConnectionTypeEvent();
        }
    }
}