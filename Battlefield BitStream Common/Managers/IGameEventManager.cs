using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Managers
{
    public interface IGameEventManager
    {
        int ProcessReceivedPacket(IBitStream stream);
        void Transmit(IBitStream stream, List<IGameEvent> GameEvents);
        IGameEvent ReadGameEvent(IBitStream stream, bool useOld = false);
        void WriteGameEvent(IBitStream stream, IGameEvent gameEvent);
    }
}