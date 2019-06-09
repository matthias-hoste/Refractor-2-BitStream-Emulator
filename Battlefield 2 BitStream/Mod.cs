using Battlefield_2_BitStream.Data;
using Battlefield_2_BitStream.GameEvents;
using Battlefield_2_BitStream.Processors;
using Battlefield_BitStream_Common;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream
{
    public class Mod : IMod
    {
        public static Mod Instance { get; private set; }
        public string Name => "Battlefield 2";

        public bool RequiresChallenge => true;

        public Type ChallengeEvent => typeof(GameEvents.ChallengeEvent);

        public ServerInfo ServerSettings { get; private set; }

        public IBitStreamExtension BitStreamExtension => null;//bf2 uses the raw bitstream

        private static IConFileProcessor _instance { get; set; }

        public Mod()
        {
            Instance = this;
            ServerSettings = new ServerInfo();
        }

        public IConFileProcessor GetConFileProcessor()
        {
            if (_instance == null)
                _instance = new ConFileProcessor();
            return _instance;
        }

        public void Initialize(IEventRegistry registry)
        {
            registry.Register(1, typeof(ChallengeEvent));
            registry.Register(2, typeof(ChallengeResponseEvent));
            registry.Register(3, typeof(ConnectionTypeEvent));
            registry.Register(4, typeof(DataBlockEvent));
            registry.Register(5, typeof(CreatePlayerEvent));
            ConMethods.Initialize();
        }
    }
}