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
    public class UnlockEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {

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
            int v2 = 0;
            do
                ++v2;
            while (((1 << v2) - 1) < 2);
            uint v10 = 0;
            v10 = stream.ReadBits((uint)v2);
            uint v9 = 0;
            v9 = stream.ReadBits(8);
            uint v7 = 0;
            uint v6 = 0;
            do
            {
                uint v3 = 0;
                do
                {
                    uint v8 = 0;
                    uint v4 = v3++ + v6;
                    v8 = stream.ReadBits(4);
                }
                while (v3 <= 6);
                ++v7;
                v6 += 7;
            }
            while (v7 <= 1);
            return new UnlockEvent();
        }
    }
}