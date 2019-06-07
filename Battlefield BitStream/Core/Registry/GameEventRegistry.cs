using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Registry
{
    public class GameEventRegistry : IEventRegistry
    {
        private Dictionary<uint, Type> registeredEvents { get; set; }
        public GameEventRegistry()
        {
            registeredEvents = new Dictionary<uint, Type>();
        }
        public void Register(uint id, Type eventType)
        {
            if(eventType.GetInterfaces().Contains(typeof(IGameEvent)))
            {
                if (registeredEvents.ContainsKey(id))
                    return;
                registeredEvents.Add(id, eventType);
            }
        }
        public IGameEvent Trigger(uint id, IBitStream stream)
        {
            if (!registeredEvents.ContainsKey(id))
                return null;
            var ev = ((IGameEvent)Activator.CreateInstance(registeredEvents[id])).DeSerialize(stream);
            if(ev == null)
            {

            }
            return ev;
        }
    }
}