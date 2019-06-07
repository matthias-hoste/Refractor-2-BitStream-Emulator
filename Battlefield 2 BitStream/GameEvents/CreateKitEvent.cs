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
    public class CreateKitEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {

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
            var v1 = stream.ReadBits(0x20);
            var v2 = stream.ReadBits(0x10);
            var v3 = stream.ReadBits(0x20);
            var v4 = stream.ReadBits(0x20);
            var v5 = stream.ReadBits(0x20);
            var v6 = stream.ReadBits(4);
            var v7 = stream.ReadBits(4);
            return null;
        }
    }
}