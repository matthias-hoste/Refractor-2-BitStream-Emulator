using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.GameEvents
{
    public class CreatePlayerEvent : IGameEvent
    {
        public string PlayerName { get; set; }
        public uint PlayerTeam { get; set; }
        //
        public uint PlayerId { get; set; }
        public uint PlayerIndex { get; set; }//unsure
        //always 0 so no point
        public bool IsAI { get; set; }
        public uint SpawnGroup { get; set; }//also 0 from what I saw
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(PlayerTeam, 3);
            stream.WriteBits(SpawnGroup, 4);
            stream.WriteBits(0, 1);
            stream.WriteBits(PlayerId, 8);
            stream.WriteBits(PlayerIndex, 0x10);
            stream.WriteBits(0, 0x10);
            stream.WriteBool(IsAI);
            stream.WriteString(PlayerName, 32);
        }

        public void Transmit(INetworkingClient client)
        {
            client.SendEvent(this);
        }

        public void Process(INetworkingClient client)//currently only support server clients and not regular clients
        {
            
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var createPlayerEvent = new CreatePlayerEvent();
            createPlayerEvent.PlayerTeam = stream.ReadBits(3);
            createPlayerEvent.SpawnGroup = stream.ReadBits(4);
            var v3 = stream.ReadBits(1);
            createPlayerEvent.PlayerId = stream.ReadBits(8);//id given by gameserver, not actual gamespy id
            createPlayerEvent.PlayerIndex = stream.ReadBits(0x10);//another server id? This one does not start at 0
            var v6 = stream.ReadBits(0x10);//always 0
            createPlayerEvent.IsAI = stream.ReadBool();
            createPlayerEvent.PlayerName = stream.ReadString(32).Replace("\0", "");
            return createPlayerEvent;
        }
    }
}