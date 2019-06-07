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
    public class PostRemoteEvent : IGameEvent
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
            client.Send("9ff1151f00000032013e78a38c9f090600008081a4780c62e6c485218fd11222888f05030040d0409c448db03ef3ad101b88cbb0c4278501000078a0cf6471f89a77640856db71c8e693c20000103c100ba31f1cd8112cb41e073664f1616100000820680f551c9eb2251992cf7a1ca2f8ec31000000141c0ca6097199f30add3b6a0d8b7cf6180000020a8808f48465658285c23c0f87443e7f0c0000410524df38c4aebfcf42315e2844331f40060080c0029dc71c62d7df6721da3d14228b8f200300007081db52cb505e0fae50ddc2e530c54790010020b84019c77018ce4f5c0824386b88c2b0cc000010648049f118c4cc890b431ea325440ccc0c00007c064844a0272c2b132c14e67938f4587c531800000010000000c0c0cc0000e06780711235c2faccb7426c202ec333b32b8600000000010000000c");
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var eventId = stream.ReadBits(4);
            var subEventId = stream.ReadBits(32);
            var p3 = stream.ReadBits(32);
            var p4 = stream.ReadBits(8);
            return new PostRemoteEvent();
        }
    }
}