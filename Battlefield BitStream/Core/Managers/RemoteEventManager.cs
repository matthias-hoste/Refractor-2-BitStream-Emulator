using Battlefield_BitStream.Core.Data;
using Battlefield_BitStream_Common.Managers;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Managers
{
    public class RemoteEventManager : IRemoteEventManager
    {
        public INetworkingClient Client { get; private set; }
        private List<RemoteEvent> RemoteEvents { get; set; }
        public RemoteEventManager(INetworkingClient client)
        {
            Client = client;
            RemoteEvents = new List<RemoteEvent>();
        }
        public void AddEventHandler(uint topId, uint bottomId, Func<uint, uint, uint> handler)
        {
            var remoteEvent = new RemoteEvent()
            {
                TopId = topId,
                BottomId = bottomId,
                EventHandler = handler,
            };
            RemoteEvents.Add(remoteEvent);
        }

        public void TriggerEvent(uint topId, uint bottomId)
        {
            try
            {
                var trigger = RemoteEvents.Where(x => x.TopId == topId && x.BottomId == bottomId).First();
                trigger.EventHandler(0, 0);//later on the real arguments should be passed here
            }
            catch(Exception)
            {

            }
        }
    }
}