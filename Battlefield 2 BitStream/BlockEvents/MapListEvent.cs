using Battlefield_2_BitStream.Data;
using Battlefield_BitStream_Common.Data;
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
    public class MapListEvent : IBlockEvent
    {
        public List<IMap> Maps { get; set; }
        public BlockEventId BlockEventId => BlockEventId.MapList;
        public MapListEvent()
        {
            Maps = new List<IMap>();
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
            if(Mod.Instance == null)
            {
                stream.WriteBits(1, 16);
                stream.WriteBits(64, 16);
                stream.WriteBits((uint)"kubra_dam".Length, 16);
                stream.WriteString("kubra_dam", (uint)"kubra_dam".Length);
                stream.WriteBits((uint)"gpm_cq".Length, 16);
                stream.WriteString("gpm_cq", (uint)"gpm_cq".Length);
            }
            else
            {
                stream.WriteBits((uint)Mod.Instance.BF2Engine.MapList.Count, 16);
                foreach(var map in Mod.Instance.BF2Engine.MapList)
                {
                    stream.WriteBits(map.MaxPlayers, 16);
                    stream.WriteBits((uint)map.MapName.Length, 16);
                    stream.WriteString(map.MapName, (uint)map.MapName.Length);
                    stream.WriteBits((uint)map.GameMode.Length, 16);
                    stream.WriteString(map.GameMode, (uint)map.GameMode.Length);
                }
            }
        }
        public IBlockEvent DeSerialize(IBitStream stream)
        {
            var mapListEvent = new MapListEvent();
            var mapCount = stream.ReadBits(16);
            while (mapCount > 0)
            {
                var map = new Map();
                map.MaxPlayers = stream.ReadBits(16);
                var mapNameLen = stream.ReadBits(16);
                map.MapName = stream.ReadString(mapNameLen);
                var gameModeLen = stream.ReadBits(16);
                map.GameMode = stream.ReadString(gameModeLen);
                mapListEvent.Maps.Add(map);
                mapCount--;
            }
            return mapListEvent;
        }
    }
}