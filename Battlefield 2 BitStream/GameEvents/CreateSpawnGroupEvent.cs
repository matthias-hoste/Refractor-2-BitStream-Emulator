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
    public class CreateSpawnGroupEvent : IGameEvent
    {
        public CreateSpawnGroupEvent()
        {

        }
        public void Serialize(IBitStream stream)
        {

        }

        public void Transmit(INetworkingClient client)//currently only support server clients and not regular clients
        {

        }

        public void Process(INetworkingClient client)//currently only support server clients and not regular clients
        {

        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            stream.ReadBits(8);
            stream.ReadBits(4);
            stream.ReadBits(1);
            stream.ReadBits(1);
            stream.ReadBits(1);
            stream.ReadBits(8);
            stream.ReadBits(8);
            stream.ReadBits(0x10);
            return new CreateSpawnGroupEvent();
        }
    }
}