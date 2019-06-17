using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.BlockEvents
{
    public class MapInfoEvent : IBlockEvent
    {
        public string MapName { get; set; }
        public string MapPath { get; set; }
        public string GameMode { get; set; }
        public uint MaxPlayers { get; set; }
        public uint CommanderEnabled { get; set; }
        public uint ChallengeOrdinal { get; set; }
        public BlockEventId BlockEventId => BlockEventId.MapInfo;
        public MapInfoEvent()
        {

        }
        public MapInfoEvent(bool make = false)
        {
            if(make)
            {
                MapName = "kubra_dam";
                MapPath = "Levels/";
                GameMode = "gpm_cq";
                MaxPlayers = 64;
                CommanderEnabled = 1;
                ChallengeOrdinal = 1;
            }
        }
        public void Process(INetworkingClient client)
        {

        }
        public void Transmit(INetworkingClient client)
        {
            client.BlockManager.QueueBlockEvent(this);
        }
        public void Serialize(IBitStream stream)//datablock events are different from game events
        {
            stream.WriteBits((uint)GameMode.Length, 0x10);
            stream.WriteString(GameMode, (uint)GameMode.Length);
            stream.WriteBits((uint)MapPath.Length, 0x10);
            stream.WriteString(MapPath, (uint)MapPath.Length);
            stream.WriteBits((uint)MapName.Length, 0x10);
            stream.WriteString(MapName, (uint)MapName.Length);
            stream.WriteBits(0, 1);
            stream.WriteBits(MaxPlayers, 31);
            stream.WriteBits(CommanderEnabled, 1);
            stream.WriteBits(0, 1);
            stream.WriteBits(ChallengeOrdinal, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(0, 1);
        }
        public IBlockEvent DeSerialize(IBitStream stream)
        {
            var mapInfoEvent = new MapInfoEvent();
            var gameModeLen = stream.ReadBits(0x10);
            mapInfoEvent.GameMode = stream.ReadString(gameModeLen);
            var pathLen = stream.ReadBits(0x10);
            mapInfoEvent.MapPath = stream.ReadString(pathLen);
            var levelNameLen = stream.ReadBits(0x10);
            mapInfoEvent.MapName = stream.ReadString(levelNameLen);
            var v4 = stream.ReadBits(1);
            mapInfoEvent.MaxPlayers = stream.ReadBits(31);
            mapInfoEvent.CommanderEnabled = stream.ReadBits(1);
            var v7 = stream.ReadBits(1);
            mapInfoEvent.ChallengeOrdinal = stream.ReadBits(31);
            var v9 = stream.ReadBits(1);
            var v10 = stream.ReadBits(1);
            return mapInfoEvent;
        }
    }
}