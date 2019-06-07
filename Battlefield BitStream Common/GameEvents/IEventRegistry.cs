using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.GameEvents
{
    public interface IEventRegistry
    {
        void Register(uint id, Type eventType);
        IGameEvent Trigger(uint id, IBitStream stream);
    }
}