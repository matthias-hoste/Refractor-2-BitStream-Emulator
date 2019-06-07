using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Managers
{
    public interface IBlockEventManager
    {
        void RegisterEvents();
        IBlockEvent Trigger(BlockEventId eventId, IBitStream stream);
    }
}