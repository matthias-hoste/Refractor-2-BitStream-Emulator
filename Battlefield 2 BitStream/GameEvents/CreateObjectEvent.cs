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
    public class CreateObjectEvent : IGameEvent
    {
        public void Serialize(IBitStream stream)
        {

        }

        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Process(INetworkingClient client)
        {
            
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var templateId = stream.ReadBits(0x20);
            var objectId = stream.ReadBits(0x10);
            var v3 = stream.ReadBits(2);
            var isMultiSpawn = stream.ReadBits(1);//if true means spawn the same object multiple times
            if(isMultiSpawn == 1)
            {
                var spawnAmount = stream.ReadBits(8);
            }
            else
            {
                var v5 = stream.ReadBits(1);
                if(v5 == 1)
                {
                    var v6 = stream.ReadBits(0x20);
                    var v7 = stream.ReadBits(0x20);
                    var v8 = stream.ReadBits(0x20);
                    var v222 = 0;
                }
                else
                {

                }
                var hasVector = stream.ReadBool();
                if(hasVector)
                {
                    var v6 = stream.ReadBits(0x20);
                    var v7 = stream.ReadBits(0x20);
                    var v8 = stream.ReadBits(0x20);
                    var v222 = 0;
                }
                else
                {

                }
            }
            return null;
        }
    }
}