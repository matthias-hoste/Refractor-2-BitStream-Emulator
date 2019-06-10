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
        public uint EventId { get; set; }
        public uint FunctionId { get; set; }

        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(EventId, 4);
            stream.WriteBits(FunctionId, 32);
            stream.WriteBits(0, 32);
            stream.WriteBits(0, 8);
        }

        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Process(INetworkingClient client)
        {
            client.RemoteEventManager.TriggerEvent(EventId, FunctionId);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var remoteEvent = new PostRemoteEvent();
            remoteEvent.EventId = stream.ReadBits(4);
            remoteEvent.FunctionId = stream.ReadBits(32);
            var p3 = stream.ReadBits(32);
            var p4 = stream.ReadBits(8);
            return remoteEvent;
        }
    }
}