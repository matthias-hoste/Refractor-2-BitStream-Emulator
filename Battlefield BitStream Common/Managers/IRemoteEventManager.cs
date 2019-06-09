using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Managers
{
    public interface IRemoteEventManager
    {
        void AddEventHandler(uint topId, uint bottomId, Func<uint, uint, uint> handler);
        void TriggerEvent(uint topId, uint bottomId);
    }
}