using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.GameEvents
{
    public interface IGameEvent
    {
        void Serialize(IBitStream stream);
        void Transmit(INetworkingClient client);
        void Process(INetworkingClient client);
        IGameEvent DeSerialize(IBitStream stream);
    }
}