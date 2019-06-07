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
    public class ClientInfoEvent : IBlockEvent
    {
        public string Username { get; set; }
        public string SomeString { get; set; }
        public BlockEventId BlockEventId => BlockEventId.ClientInfo;
        public void Process(INetworkingClient client)
        {

        }
        public void Transmit(INetworkingClient client)
        {
            client.BlockManager.QueueBlockEvent(this);
        }
        public void Serialize(IBitStream stream)//datablock events are different from game events
        {

        }
        public IBlockEvent DeSerialize(IBitStream stream)
        {
            var clientInfo = new ClientInfoEvent();
            var userNameLength = stream.ReadBits(0x10);
            if(userNameLength > 0)
            {
                clientInfo.Username = stream.ReadString(userNameLength);
            }
            var v1 = stream.ReadBits(32);
            var v2 = stream.ReadBits(1);
            var v3 = stream.ReadBits(31);
            var v4 = stream.ReadBits(0x10);
            if (v4 > 0)
            {
                clientInfo.Username = stream.ReadString(v4);
            }
            var v5 = stream.ReadBits(0x10);
            if (v5 > 0)
            {
                clientInfo.SomeString = stream.ReadString(v5);
            }
            var v6 = stream.ReadBits(1);
            return clientInfo;
        }
    }
}