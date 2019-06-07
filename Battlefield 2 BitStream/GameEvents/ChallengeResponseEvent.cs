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
    public class ChallengeResponseEvent : IGameEvent
    {
        public byte[] Challenge { get; set; }
        public uint NetVersionNumber { get; set; }
        public uint GameId { get; set; }
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(2, 7);
            var bytes = new byte[73];
            stream.WriteBytes(bytes);
            stream.WriteBits(1768123489, 32);
            stream.WriteBits(353128704, 32);//Battlefield 2 latest net build
            stream.WriteBits(0, 1);
            stream.WriteBits(1059, 31);
        }

        public void Transmit(INetworkingClient client)
        {
            throw new NotImplementedException();
        }

        public void Process(INetworkingClient client)
        {
            client.SetAuthenticated(true);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var challengeResponseEvent = new ChallengeResponseEvent();
            challengeResponseEvent.Challenge = stream.ReadBytes(73);
            var ss = stream.ReadBits(32);//1768123489
            challengeResponseEvent.NetVersionNumber = stream.ReadBits(32);//Network Build Number
            var ssss = stream.ReadBits(1);
            challengeResponseEvent.GameId = stream.ReadBits(31);
            return challengeResponseEvent;
        }
    }
}