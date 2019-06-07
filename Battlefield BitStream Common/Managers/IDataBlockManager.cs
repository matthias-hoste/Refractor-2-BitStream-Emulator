using Battlefield_BitStream_Common.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Managers
{
    public interface IDataBlockManager
    {
        Action<IBlockEvent> BlockReceived { get; set; }
        IBlockEventManager BlockEventManager { get; }
        void NewBlock(uint eventId, uint size);
        void AddBlock(byte[] block);
        void QueueBlockEvent(IBlockEvent blockEvent);
    }
}