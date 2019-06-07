using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Managers;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Managers
{
    public class BlockEventManager : IBlockEventManager
    {
        public INetworkingClient Client { get; private set; }
        private Dictionary<BlockEventId, Type> BlockEvents { get; set; }
        public BlockEventManager(INetworkingClient client)
        {
            Client = client;
            BlockEvents = new Dictionary<BlockEventId, Type>();
        }
        public void RegisterEvents()
        {
            var events = Config.ModAssembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IBlockEvent)));
            foreach(var blockEvent in events)
            {
                var block = (IBlockEvent)Activator.CreateInstance(blockEvent);
                BlockEvents.Add(block.BlockEventId, blockEvent);
            }
        }
        public IBlockEvent Trigger(BlockEventId eventId, IBitStream stream)
        {
            if(!BlockEvents.ContainsKey(eventId))
            {
                Console.WriteLine("[BLOCKEVENTMANAGER] Unknown block id: " + Convert.ToString(eventId));
                return null;
            }
            return ((IBlockEvent)Activator.CreateInstance(BlockEvents[eventId])).DeSerialize(stream);
        }
    }
}