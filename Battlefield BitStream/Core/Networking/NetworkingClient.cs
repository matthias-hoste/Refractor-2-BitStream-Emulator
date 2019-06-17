using Battlefield_2_BitStream.BlockEvents;
using Battlefield_2_BitStream.GameEvents;
using Battlefield_BitStream.Core.Helpers;
using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream.Core.Managers;
using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Managers;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Networking
{
    public class NetworkingClient : INetworkingClient
    {
        public NetworkingServer ParentServer { get; private set; }
        public IDataBlockManager BlockManager { get; private set; }
        public IRemoteEventManager RemoteEventManager { get; private set; }
        public IGameEventManager GameEventManager { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public uint ConnectionId { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsAuthenticated { get; private set; }
        private ConcurrentQueue<IGameEvent> GameEvents { get; set; }
        private uint ServerPacketId { get; set; }
        private uint ClientPacketId { get; set; }
        private bool mustSend { get; set; }
        public NetworkingClient(NetworkingServer server, IPEndPoint endPoint, uint connectionId)
        {
            ConnectionId = connectionId;
            IsConnected = true;
            ServerPacketId = 1;
            ClientPacketId = 0;
            mustSend = true;
            ParentServer = server;
            RemoteEndPoint = endPoint;
            GameEvents = new ConcurrentQueue<IGameEvent>();
            BlockManager = new DataBlockManager(this);
            RemoteEventManager = new RemoteEventManager(this);
            GameEventManager = new GameEventManager(this);
            BlockManager.BlockReceived += BlockReceived;
        }

        private void BlockReceived(IBlockEvent obj)
        {
            if(obj.BlockEventId == BlockEventId.ClientInfo)//when we receive this we must send the player database with the current player in it and the info for the map to be loaded
            {
                var createPlayerEvent = new CreatePlayerEvent();
                createPlayerEvent.PlayerId = 1;
                createPlayerEvent.PlayerIndex = 0;
                createPlayerEvent.PlayerName = "LifeCoder";
                createPlayerEvent.IsAI = false;
                createPlayerEvent.PlayerTeam = 2;
                createPlayerEvent.SpawnGroup = 0;
                SendEvent(createPlayerEvent);
                new MapInfoEvent(true).Transmit(this);
                Send();
                //now we wait until the client finished loading the map
                //top id is 6, which is NetworkEvent, bottom id is 2 which is loadLevelComplete in NetworkEvent
                RemoteEventManager.AddEventHandler(6, 2, OnClientLoadComplete);
            }
            else if(obj.BlockEventId == BlockEventId.ServerInfo)
            {
                Config.ServerInfo = obj;
            }
            else if(obj.BlockEventId == BlockEventId.MapList)
            {
                
            }
            else
            {

            }
        }
        private int v28 = 0;
        private int v54 = 0;
        public void Handle(PacketType packetType, IBitStream stream)
        {
            lock (this)
            {
                switch (packetType)
                {
                    case PacketType.ConnectionAcknowledge:
                        break;//now we can receive and send packets
                    case PacketType.PingRequest:
                        uint type1 = 0;
                        uint Type2 = 0;
                        uint type3 = 0;
                        stream.ReadExtendedHeader(ref type1, ref Type2, ref type3);
                        uint i = stream.ReadBits(1);
                        uint v4 = 0;
                        if(i == 1)
                        {
                            if(type1 != ClientPacketId && (v4 = (type1 - ClientPacketId) & 0x3F) <= 0x1F)
                            {
                                if(v4 > 1)
                                {
                                    v28 += (int)v4 - 1;
                                    v54 <<= (int)v4 - 1;
                                }
                                v54 *= 2;
                                ++v28;
                                ClientPacketId = type1;
                            }
                        }
                        uint token = stream.ReadBits(32);
                        SendPingResponse(token);
                        break;
                    case PacketType.PingResponse:
                        break;
                    case PacketType.Data:
                        uint t1 = 0;
                        uint clientPacketCounter = 0;//a3 & 0x3F
                        uint t3 = 0;
                        stream.ReadExtendedHeader(ref clientPacketCounter, ref t1, ref t3);
                        int v11 = (int)(clientPacketCounter - ClientPacketId) & 0x3F;
                        if (v11 > 1)
                        {
                            v28 += v11 - 1;
                            v54 <<= v11 - 1;
                        }
                        v54 = 2 * v54 | 1;
                        ClientPacketId = clientPacketCounter;
                        var packetLength = stream.ReadBits(16);//payload length in bytes
                        new PlayerActionManager().ProcessReceivedPacket(stream);
                        GameEventManager.ProcessReceivedPacket(stream);
                        new GhostManager().ProcessReceivedPacket(stream);
                        if (!IsAuthenticated && Config.LoadedMod.RequiresChallenge)//create and send a challenge
                        {
                            var challenge = (IGameEvent)Activator.CreateInstance(Config.LoadedMod.ChallengeEvent);
                            challenge.Transmit(this);
                        }
                        Send();//call send to send all queued events
                        break;
                    case PacketType.Disconnect:
                        var err = stream.ReadBits(0x20);
                        //var msgLength = stream.ReadBits(0x10);
                        //var msg = stream.ReadString(msgLength);
                        break;
                }
            }
        }

        public void SendPingRequest()
        {

        }

        public void SendPingResponse(uint token)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader((uint)PacketType.PingResponse, ConnectionId);
                stream.WriteExtendedHeader(ServerPacketId, ClientPacketId, (uint)v54);
                stream.WriteBits(0, 1);
                stream.WriteBits(token, 32);
                stream.WriteBits(0, 32);//some timestamp
                ParentServer.Send(stream.GetRawBuffer(), RemoteEndPoint);
                Console.WriteLine("[NetworkingClient - " + RemoteEndPoint.ToString() + "] sent packet type " + Convert.ToString(PacketType.PingResponse));
            }
            catch (Exception)
            {

            }
        }

        public void SendEvent(IGameEvent gameEvent)
        {
            lock (GameEvents)
            {
                GameEvents.Enqueue(gameEvent);
                mustSend = true;
            }
        }
        public void Send()
        {
            if (ParentServer == null)
                return;
            if (mustSend)//only send a packet if we have data to send
            {
                lock (this)
                {
                    IBitStream stream = BitStream.Create();//create new bitstream
                    stream.WriteBasicHeader(15, ConnectionId);//write packet type and client id
                    stream.WriteExtendedHeader(ServerPacketId, ClientPacketId, (uint)v54);//write packet identifiers
                    ServerPacketId++;
                    var position = stream.GetPosition();
                    stream.SkipBits(16);
                    stream.WriteBits(0, 1);//playeractionmanager
                    if (GameEvents.Count > 0)
                    {
                        stream.WriteBits(1, 1);
                        lock (GameEvents)
                        {
                            List<IGameEvent> events = new List<IGameEvent>();
                            for (int i = 4; i > 0; i--)
                            {
                                IGameEvent gameEvent;
                                if (!GameEvents.TryDequeue(out gameEvent))
                                    break;
                                events.Add(gameEvent);
                            }
                            GameEventManager.Transmit(stream, events);
                        }
                    }
                    else
                    {
                        stream.WriteBits(0, 1);
                    }
                    stream.WriteBits(0, 1);//ghostmanager
                    var lastPosition = stream.GetPosition();
                    stream.SetPosition(position);
                    stream.WriteBits(((uint)(lastPosition - 72) / 8), 16);
                    stream.SetPosition(lastPosition);
                    ParentServer.Send(stream.GetRawBuffer(), RemoteEndPoint);
                    Console.WriteLine("[NetworkingClient - " + RemoteEndPoint.ToString() + "] sent packet type " + Convert.ToString(PacketType.Data));
                    mustSend = false;
                }
            }
            else
            {
                SendPingRequest();
            }
        }
        public void Send(string hex)
        {
            lock (this)
            {
                IBitStream stream = new BitStream(DevelopmentHelper.ParseHexString(hex));//create new bitstream
                stream.WriteBasicHeader(15, ConnectionId);//write packet type and client id
                stream.WriteExtendedHeader(ServerPacketId, ClientPacketId, (uint)v54);//write packet identifiers
                ServerPacketId++;
                ParentServer.Send(stream.GetRawBuffer(), RemoteEndPoint);
                Console.WriteLine("[NetworkingClient - " + RemoteEndPoint.ToString() + "] sent packet type " + Convert.ToString(PacketType.Data));
            }
        }
        public void SetAuthenticated(bool authenticated)
        {
            IsAuthenticated = authenticated;
            if(IsAuthenticated)
            {
                if (Config.ServerInfo != null)
                {
                    Config.ServerInfo.Transmit(this);
                    new MapListEvent().Transmit(this);//send the maplist to the client
                }
            }
        }
        public void SendDatabase()
        {

        }
        public uint OnClientLoadComplete(uint a1, uint a2)//now we must send the object database to the client
        {
            Send("0fa0141f0000003b0132242380864e07000081033c892f1ce67a5814f2a7a60866a87500002838c0c470bfe1666645218115c960867407000083030400c81a46359214020000074a662623040000000800000060507900003838c00c8266e1b8be4ba10cc275a1000018420000008000000000869507000084032c065f178eebbb146a3b1f160a008021040000000800000060607900004838c01ea582e1b8be4b21fa3e4ea100001842000000800000000086b1070000850364e5e41dd6cca617e24fc81b4e66b62d040000000800000060207b00005838c043f3e461fed478a18ff2b9e16466db42000000800000000086b207000086030400ea1efe53c9170aacfc1a4e66b62d040000000800000060107c000068385000a008a266e651210080aba10000b442000000800000000086ca07008087830455f31e7ee936146a263e2166");
            Send("0fb0141f00000032012a2886cb070000880305000a208aebf3140200d01a62f8830000a838402000f321884f4e214500cca10a0007c380967dbe824f8ebd86510800809403345d071fbedc3a14e2819c200e000000709a6c0d0c0000006018850000583940f453e4e1439570a1c13bc1e1dc2c93c2ed69cec02d7f3a3a06c008000097037c97ee1f0ec041146a41a9217e450122b4ff3bf95b54c5b963188d00008839c02194f9e139c644217abf1ce2d74a1f421fa49dbe5955143c86e80800809903aca0531fbec045141a057b205eae8fa3b34446fdeb4194c763988f0000c8394004c006a2e7e853214801dba18afb33c3910e13bfe4fb633d860c0900809d031400d81fa2abfd14f202fc1c8afe3f33ac11e3dd5b3843aa6bd0910000403a40f4ff8f2153694521f27f8ca1407506c32444803c42bf54bc06");
            Send("0fc0141f0000003e012a2c0650090000a803c4fc0f1a9ef698172a02601c2eff9f2144d750c2d39e7ad96b00950000903a40c24ade618cdd2d21d5269ba133fff94292eb2b3c29a89bbd8670090080ac03d497061ed6a56114a2244b20fe7b1be86b29d8da9b7439a26b08970000f03a40b4cfdfe1a59249a1d895b1e1275dddc259bdaabd25697aba8670090080b103fc36901bd6a55914da3dcb083e146d2a340997da2b5e17a963f0990000303c401efd922141be44a1197e86a14d39b5c26aa40740ce6b513d069f090080c40344714c1f22e4db147afab21abaa4293114ef7a00945c95d563f0990000603c401200f5215eca4d21fffcbaa1dbf03243a32d06407c6e623d069f090080c7035c24ca17b2bc4b14ea9426180a6d030ba4465f0064170cd56b989a0000083dc0057eda613d5946a19626fee1dcdce8c1b2734bc09436a24006");
            Send("0fd0141f0000003e012a3086a9090000d203ec492e1e66d063149a974c20bef64c9ba3ffe6e1e3f43cc363989a0000383d4011e8dee1b06a2da19f198da10200b44213601e3ef3d8313c86a9090000d503cc66801b66d05b14aa3b120f6edc7577439802e2c3f62bc363989a0000683d40719cf861063d43a1f67819e248cdac41164d1e3e1da9353c86a9090000d8035496dc1f26c66b174249b615be7691311c9ce6e193af54c363989a0000983d400a999ae1f0467921e2f1c2e1450bfec10de414c0f912ac3f86a9090000db032c546d1ea6549b14421cb51a1e2fd02e743d00e273e34bc363989a0000c83d40faffb161e7d744a1df7fcda1faff33c31d721e3eefa9333c86d5090000e003e408e81d0eeade12d20010189a65252b44b956effb846bd063589d0000103ec0ce08e561fe7e46a1577700e26e40a8b99171f5bea2d9063d06");
            Send("0fe0141f0000003e012a3486d5090000e2034c360520a6e56f175264fa186e241120a47856efcb8e6ed063589d0000303e40b0ddaa61df194521608ecea1ba693343dd61f3be47f7a93c86f6090000e603c4fe0f18fae94e144aff2f173a21751c2460edd383e4c2d16370a00000e83ec03d80a0a1605241a111007ea13031c4c2adc9f8bdf5be80ba86300a0000f50394fb0f1a22b614144aca3617faff3f2bdc3275da530fead16b08a30000603fc0166c8d2131ab41a1c3a177a171c96dc220d405c1cb99c64006bd0a0000df841402e81b060018149298cf1f7e6e42180c0000000800000060d0ab0000f84d487929c061008041a1e58402e2313393c1000000800000000006bd0a0000e084fca91f1c060018146a3c771ffe2691150c0000000800000060f8ab0000984e400040de61353e462100809ae179e99f41000000800000000006");
            Send("0ff0141f0000002d012e3886bf0a0000ea0404d52e1e060058148a417e1f66f8ab0000a84e400000a261d59878210040c36186bf0a0000eb04a41a191ae6a541149a44961cca49742c04000000080000006030ac0000e84e400000ab2102ab2221000014a1373432c3000000800000000006c30a0000ef0454e3d91732085c14f27c3819baa121280c000000080000006030ac0000f84e400040f7a199194d210080ada1feff3343000000800000000006c30a0000f0040400b0199a99091402002016daf727330c000000080000006040ad0000204f40aad9db6100804221b211096286d80a0080f304a41abc1a8eeb8914daceb7060e00000f04000000080000006088ad0000404f40ac6cf0e1472147a1681d05e20000b4c2000000800000000086d80a0080f40464bae21d7e939a148a96a41f66");
            Send("0f00151f00000032012a3c86d80a0000f504c41f701ef6a79a14f2d2951f6688ad0000584fc04506aee1e71b492189417aa0feff3342000000800000000086d80a0000f604e4a5b31b5e645314da78821c0a00402b0c000000080000006088ad0000684f40857b9ee1662648a174e3c4a10000b442000000800000000086dc0a0080fa043c89e51612586314bac8bc189ac633230c0000000800000060c8ad0000b04fc0ebe1f32181354ea1c6fba5a1feff33c3000000800000000086df0a0000fc04a4702518f2d2e514b29d281aeaff3f33040000000800000060f8ad0000c84f4000800022cd4c4e210080a4a1feff33c3000000800000000086df0a0000fd04c49fc819125881130a81f513caa1a1310c000000080000006038ae0000d84fd047416021000059a1c8a6b4a10000b4c2000000800000000086");
            Send("0f10151f0000003001364086e30a0000fe04b54783149a99191552e3af180a00402b0c000000080000006038ae0000e84fd08d973ba1666654a1c03aaca10000b4c2000000800000000006440b00000705040010190ad7a7140200c0196248b4000078504087d64861d5b83ca13b1f692106450b000008050400881d56b8be120200e0186258b400008850400000ae615e1a44210080c92106460b000009050400101c060050140200000b6668b40000985040ae67e4e145f648a1c8b6af6106470b00000a0534086c1a16581d18628f4b1c6678b40000a85040fabefb6100007621d9ee696106480b00000b05dca3131e06005814ca6010206698750000c050404e5ad56177fe2c214ef290a1cbcc04c2000000800000000086590700800d050400b81c7668f9170200981b1e33032d040000000800000060");
            Send("0f20151f00000032012a4486590700000e05cccbbd1c7668f9177abec31b4e66b62c04000000080000006098750000e850400080ebe1e9064621000003e2cccc8ac2000000800000000086740700800f05d4f76618e67a68122a87b8150a33432f040000000800000060487700000051c0144e702181951f210ad741e13133b74200000080000000008674070080100534332117f27cef11bac824160e00202b040000004866660c64e8740000205140088caf6181d545217969e4e0cbccecc20000008000000000864e07008012059418bb1b9e6e681432086c10eeff3f23040000000800000060e87400003051400000b761798945210000c01f864e0700801305448bba1b9e6e6814a2ef4b108e998930040000000800000060e8740000405140b24dbbe1e98646a16ed204e19a99c0c2000000800000000086");
            Send("0f30151f00000040015a48864e07008014050400d01a9697881402000009aa99092c0c0000000800000060e87400005051c070ddb06181d545215c0fc6e0feffb4420000008000000000864e0700801505f47cfb1a16585d149297760e66e8740000685140b4e8e5e1eb9146214cb7a0e1000000800000b4429899d2c2864e07000017056ce6581e16836c143a890e1a6630010000805140f5e6ae61145cbda10e7ac1e1000000800000b442000000000613000080180504000000b81eaa1902000000080000000800402b0400000060207c00009851480080f5e17288412100800ae2feff33c300000080000000002380b9002965e480404e007ac74820901b8846491608e408a24e640602b9826859d78140ce202a148720903b883ef52408e410a4bda30a02b9842858df82404e214a46ca20905b880a953408e480037f7f4202b960c1dfdf9000");
            Send("0f50193f00000034010a500b1b00000000000000005840000000000000000041000000c09c940088822d010000000000883a0c0020280400202a1c1080fc9f9de58365fb3e90037c00000000000000009a51008c4dc20200b03e5c0e312804005041fcfffd7fffffff276a9e7e13f86393b0c84b010e03000c0a2d27141057feff5f47f9ff419f27f17fff27f9bfff8bf4c04636094b4f72f130831ea3d0bc6402f1fd7ffaeffff3ffe22e0fe1ff4e96ffbfff23f07f10d401876c1236335d049e9bb4127e8d0f00ca5504e0d0e749fcdfff49feefff4ed6ffdfff01a51c30d72661315d071fbedc3a14e2819c203efcff1dfe7fff5f53e7c12661010000001818b2fc512cb7e055b1ff3f89fbffe3d9242c6666263fe17a044000002ec1fffffff7fffd7f3c86fffb3fc3fffddfc9fafffbbfff3f4ed6ffdffffdff51");//first ghostmanager packet
            Send("0f6021ff00000065020c4a098070d074027e6c12160040de61353e462100809a61c1ffff8bd3fe3ffa3c89fffb3fc9fffd5fb60736b249580a559b8729d526851047ad86bf05e77f7f040218777908ff77b2fcfffd1f81ff83bc0e146693b0a07dfe0e2f954c0ac5ae8c0d3f461600a3740270e8f324feefff24fff77f27cbffdfff11f83f10fcdfc9f2fff77fb472c05c9b8405fd1479f850255c68f04e70f894395c18324f844d9d079b84050000006060c8f247b1dc8257c5feff24eeff8f6793b0989999fc84eb11000100b804ffffffdffff7fff118feefff0cfff77f27ebffeffffeff3859ff7ffff7ff47c40764b24958924d018869f9db8514993e86cfeb7f801bd7fc17f47912fff77f92fffbbf93f5fff77f40d409f8b1495840b58b87010016856290df87ffffffeffffbffe8f324feefff24fff77f0127f0c72661f9a91f1c060018146a3c771f2efeffbfe3f1ff833e4fe2fffe4ff27fff577a82796c1216a62bdee137a949a16849fae1fffffffbfffe3f9ecdfffddffffd1f20f504f3d8242cf803cec3fe5493425ebaf2c3fffffff7fffd7f3c9bfffbbffffb3f40a10736b2495816f86987f56419855a9af887ffeedb84fda7b617777908ff77b2fcfffd1f81ff837c13f86393b00801f40d03000c0a49cce70fabfeff1f26f9ff419f27f17fff27f9bfff4bf8804c36094b6784f2307f3fa3d0ab3b00f1fdff10f0ffebff823e4fe2fffe4ff27fff77b2fefffe0f28eb406136098bbe34f0b02e0da310255902f1fdff023002fcff863e4fe2fffe4ff27fff77b2fcfffd1f81ff03c1ff9d2cff7fff47e809e6b14958b0b2c1871f851c85a27514883fedffefd3feff7836fff77ffff77f80");
            Send("0f7025ff0100007d021c4a098080d812040000000080a8c3000082420000a2c20101ccffd9593e58b6ef0339c0070000000000000020f204fcd8242c54b3b7c300008542642312c4fffffff7fffd7ff47912fff77f92fffbbf660e9c6b93b054d0a90f5fe0220a8d823d10dfffb703ff3fff2f97e4c1266101000000000000fd01000000d0ffc3bdffefffe3d9242ca01702bfc1ffce3f8c4a4ac1fffffff7fffd7f3c86fffb3fc3fffddfc9fafffbbfff3f4ed6ffdffffdffd1c301606c12165035efe1976e43a166e213e2fffffffbfffebf62f2a47ae01e9b84cd5abd109480f566eeffd37fff9fff17777908ff77b2fcfffd1f81ff83c60e846593b0d12f97fa11406fd1c7ef01b056d9ffae374fe2fffe4ff27fff77b2fcfffd1f81ff03c1ff9d2cff7fff47e7099eb149d874d40ee20e4c11a9fdffffcebbff8f67f37ffff77fff07683dc13336099bd6c8d1fa81405a3df6ff1ff0f4fff16cfeeffffeefff00c11eb8c726615b2051cfa3191385c79dfd9fc74fffc55d1ec2ff9d2cff7fff47e0ff20e0030ed9246cc38072cc09e0e0fc64db024c4ccabfa0cf93f8bfff93fcdfff9dacffbfff033a3d708f4dc296a029cfa000b0e5ffb4efbfafc4008cbb3c84ff3b59fefffe8fc0ff41d509b0b149d83c3651966cfef2f6fffffffebfff2f384fe2fffe4ff27fffd7ec817b6c123638c1a47a9b36b33db19f0bfaa2d05fdce521fcdfc9f2fff77f04fe0f923a20934dc24258c93b8cb1bb25a4da6433749c68ff0be3f9bfefc993f8bfff93fcdfff9dacffbfff03624f308f4dc2c24b6737bcc8a628b4f10439f469ff7f9ff6ffc7b3f9bffffbbfff03d49e601e9b8445e19e67b899095228dd3871e8d3feffbe92ff8f67f37ffff77fff07e8404009fe51591740");
            return 0;
        }
    }
}