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
    public class VoteEvent : IGameEvent
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
            var v1 = stream.ReadBits(8);//target, if mapvote this is a map index
            var v2 = stream.ReadBits(8);
            var v3 = stream.ReadBits(8);
            var v4 = stream.ReadBits(8);//target, if mapvote this is a map index
            var v5 = stream.ReadBits(8);
            var v6 = stream.ReadBits(8);
            var voteCaster = stream.ReadBits(8);
            var updateVoteType = stream.ReadBits(8);//8 = updateparticipant status, 9 = updateclientstate
            var voteType = stream.ReadBits(8);//0 = updatePendingVote, 1 = map, 2 = kick, 3 = mutiny
            var v10 = stream.ReadBits(32);
            return new VoteEvent();
        }
    }
}