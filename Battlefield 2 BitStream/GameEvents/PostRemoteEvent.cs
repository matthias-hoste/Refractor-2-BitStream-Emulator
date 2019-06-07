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
    public class PostRemoteEvent : IGameEvent
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
            var p1 = stream.ReadBits(4);
            var p2 = stream.ReadBits(32);
            var p3 = stream.ReadBits(32);
            var p4 = stream.ReadBits(8);
            return new PostRemoteEvent();
        }
    }
}