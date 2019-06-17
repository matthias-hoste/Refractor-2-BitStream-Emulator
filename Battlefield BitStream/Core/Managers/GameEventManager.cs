using Battlefield_2_BitStream.BlockEvents;
using Battlefield_2_BitStream.GameEvents;
using Battlefield_BitStream.Core.IO;
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
    public class GameEventManager : IGameEventManager
    {
        public INetworkingClient Client { get; private set; }
        private uint SomeValIdkWtfKillMe { get; set; }
        public GameEventManager(INetworkingClient client)
        {
            Client = client;
        }
        public int ProcessReceivedPacket(IBitStream stream)
        {
            uint val1 = stream.ReadBits(1);
            if (val1 != 1)
                return 1;
            uint numberOfEvents = stream.ReadBits(8);
            uint someVal2 = stream.ReadBits(5);
            uint action2 = stream.ReadBits(1);
            uint d = 0;
            while (numberOfEvents > 0)
            {
                try
                {
                    IGameEvent eventInstance = ReadGameEvent(stream, true);
                    eventInstance.Process(Client);
                }
                catch (Exception)
                {

                }
                numberOfEvents--;
            }
            return 0;
        }
        public void Transmit(IBitStream stream, List<IGameEvent> GameEvents)
        {
            stream.WriteBits((uint)GameEvents.Count, 8);
            stream.WriteBits(SomeValIdkWtfKillMe, 5);
            SomeValIdkWtfKillMe++;
            stream.WriteBits(0, 1);
            foreach (var gameEvent in GameEvents)
                WriteGameEvent(stream, gameEvent);
        }
        public IGameEvent ReadGameEvent(IBitStream stream, bool useOld = false)
        {
            uint eventId = 0;
            int v3 = 127;//adjust value depending on game, 127 is for bf2 and bf2142
            int v4 = 0;
            do
                ++v4;
            while (v3 > (1 << v4) - 1);//if the event amount defined above is 127 then the result will be 7, meaning we read 7 bits and those 7 bits are the event id
            eventId = stream.ReadBits((uint)v4);
            IGameEvent eventInstance;
            if (!useOld)
                eventInstance = Config.EventRegistry.Trigger(eventId, stream);//dont use in development
            else
            #region old
            {
                if (eventId == 1)
                {
                    eventInstance = new ChallengeEvent().DeSerialize(stream);
                }
                else if (eventId == 2)
                {
                    eventInstance = new ChallengeResponseEvent().DeSerialize(stream);
                }
                else if (eventId == 3)
                {
                    eventInstance = new ConnectionTypeEvent().DeSerialize(stream);
                }
                else if (eventId == 4)
                {
                    eventInstance = new DataBlockEvent().DeSerialize(stream);
                }
                else if (eventId == 5)
                {
                    eventInstance = new CreatePlayerEvent().DeSerialize(stream);
                }
                else if (eventId == 6)
                {
                    eventInstance = new CreateObjectEvent().DeSerialize(stream);
                }
                else if (eventId == 8)
                {
                    eventInstance = new DestroyObjectEvent().DeSerialize(stream);
                }
                else if (eventId == 11)
                {
                    eventInstance = new PostRemoteEvent().DeSerialize(stream);
                }
                else if (eventId == 16)
                {
                    eventInstance = new StringBlockEvent().DeSerialize(stream);
                }
                else if (eventId == 31)
                {
                    eventInstance = new CreateKitEvent().DeSerialize(stream);
                }
                else if (eventId == 35)//voip event
                {
                    stream.ReadBits(9);//just read so we can skip
                    eventInstance = null;
                }
                else if (eventId == 39)
                {
                    eventInstance = new VoteEvent().DeSerialize(stream);
                }
                else if (eventId == 42)
                {
                    eventInstance = new UnlockEvent().DeSerialize(stream);
                }
                else if (eventId == 46)
                {
                    eventInstance = new ContentCheckEvent().DeSerialize(stream);
                }
                else if (eventId == 50)
                {
                    eventInstance = null;
                }
                else if (eventId == 54)//voip event, we dont care
                {
                    stream.ReadBits(0x10);//just read so we can skip
                    eventInstance = null;
                }
                else if (eventId == 56)
                {
                    eventInstance = new BeginRoundEvent().DeSerialize(stream);
                }
                else if (eventId == 57)
                {
                    eventInstance = new CreateSpawnGroupEvent().DeSerialize(stream);
                }
                else
                {
                    eventInstance = null;
                }
            }
            #endregion
            return eventInstance;
        }
        public void WriteGameEvent(IBitStream stream, IGameEvent gameEvent)
        {
            gameEvent.Serialize(stream);
        }
    }
}