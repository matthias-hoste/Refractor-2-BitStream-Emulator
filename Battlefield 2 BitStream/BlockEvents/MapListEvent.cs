using Battlefield_2_BitStream.Data;
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
        public List<Map> Maps { get; set; }
        public BlockEventId BlockEventId => BlockEventId.MapList;
        public MapListEvent()
        {
            Maps = new List<Map>();
        }
        public MapListEvent(bool addMap = false)
        {
            Maps = new List<Map>();
            if (addMap)
            {
                Maps.Add(new Map() { GameMode = "gpm_cq", MaxPlayers = 0, MapName = "strike_at_karkand" });
                Maps.Add(new Map() { GameMode = "gpm_cq", MaxPlayers = 64, MapName = "kubra_dam" });
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
            stream.WriteBits((uint)Maps.Count, 16);
            foreach(var map in Maps)
            {
                stream.WriteBits(map.MaxPlayers, 16);
                stream.WriteBits((uint)map.MapName.Length, 16);
                stream.WriteString(map.MapName, (uint)map.MapName.Length);
                stream.WriteBits((uint)map.GameMode.Length, 16);
                stream.WriteString(map.GameMode, (uint)map.GameMode.Length);
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