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
    public class ContentCheckEvent : IGameEvent
    {
        public void Process(INetworkingClient client)
        {
            throw new NotImplementedException();
        }
        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }
        public void Serialize(IBitStream stream)
        {
            throw new NotImplementedException();
        }
        public IGameEvent DeSerialize(IBitStream stream)
        {
            var miscHash = stream.ReadBytes(16);
            var archiveHash = stream.ReadBytes(16);
            var levelHash = stream.ReadBytes(16);
            return null;
        }
    }
}