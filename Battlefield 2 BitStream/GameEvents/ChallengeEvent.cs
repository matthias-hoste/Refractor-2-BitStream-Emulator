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
    public class ChallengeEvent : IGameEvent
    {
        public string ModName { get; set; }
        public byte[] Challenge { get; set; }
        public static ChallengeEvent CreateNewChallenge()
        {
            var challengeEvent = new ChallengeEvent();
            challengeEvent.Challenge = new byte[10];
            int v4 = 0;
            var random = new Random();
            do
            {
                int v5 = random.Next();
                challengeEvent.Challenge[v4] = (byte)((((v5 - 26 * (1321528399 * v5) >> 32) >> 3) - (v5 >> 31)) + 97);
                v4++;
            }
            while (v4 <= 9);
            challengeEvent.Challenge[9] = 0x00;
            challengeEvent.ModName = "bf2";
            return challengeEvent;
        }
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(1, 7);
            stream.WriteBytes(Challenge);
            stream.WriteBits((uint)ModName.Length, 8);
            stream.WriteString(ModName, 3);
        }

        public void Transmit(INetworkingClient client)//currently only support server clients and not regular clients
        {
            client.SendEvent(this);
        }

        public void Process(INetworkingClient client)//currently only support server clients and not regular clients
        {
            client.SendEvent(this);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var challengeEvent = new ChallengeEvent();
            challengeEvent.Challenge = stream.ReadBytes(10);//actual challenge?
            var modNameLength = stream.ReadBits(8);//modname length
            challengeEvent.ModName = stream.ReadString(modNameLength);//actual modname
            return challengeEvent;
        }
    }
}