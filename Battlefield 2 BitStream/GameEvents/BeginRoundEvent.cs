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
    public class BeginRoundEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(1, 0x20);
            stream.WriteBits(0, 0x20);
        }

        public void Process(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Transmit(INetworkingClient client)
        {
            client.SendEvent(this);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var a1 = stream.ReadBits(0x20);
            var a2 = stream.ReadBits(0x20);
            return new BeginRoundEvent();
        }
    }
}