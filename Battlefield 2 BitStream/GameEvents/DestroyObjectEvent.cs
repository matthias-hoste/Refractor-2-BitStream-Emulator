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
    public class DestroyObjectEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {

        }

        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Process(INetworkingClient client)//currently only support server clients and not regular clients
        {
            client.SendEvent(this);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            uint v8 = stream.ReadBits(0x10);
            return null;
        }
    }
}