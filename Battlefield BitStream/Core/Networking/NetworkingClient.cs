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
            GameEventManager = new GameEventManager(this);
            BlockManager.BlockReceived += BlockReceived;
        }

        private void BlockReceived(IBlockEvent obj)
        {
            if(obj.BlockEventId == BlockEventId.ServerInfo)
            {
                Config.ServerInfo = obj;
            }
            else if(obj.BlockEventId == BlockEventId.MapList)
            {
                Config.MapList = obj;
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
                        Console.WriteLine("Received, type 1: " + Convert.ToString(clientPacketCounter) + ", type 2: " + Convert.ToString(t1) + ", type 3: " + Convert.ToString(t3));
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
                    Console.WriteLine("Sent, type 1: " + Convert.ToString(ServerPacketId) + ", type 2: " + Convert.ToString(ClientPacketId) + ", type 3: " + v54);
                    ServerPacketId++;
                    var position = stream.GetPosition();
                    stream.SkipBits(16);
                    stream.WriteBits(0, 1);//playeractionmanager
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
        private void Send(string hex)
        {
            if (ParentServer == null)
                return;
            lock (this)
            {
                IBitStream stream = new BitStream(DevelopmentHelper.ParseHexString(hex));//create new bitstream
                stream.WriteBasicHeader(15, ConnectionId);//write packet type and client id
                stream.WriteExtendedHeader(ServerPacketId, ClientPacketId, (uint)v54);//write packet identifiers
                Console.WriteLine("Sent, type 1: " + Convert.ToString(ServerPacketId) + ", type 2: " + Convert.ToString(ClientPacketId) + ", type 3: " + v54);
                ServerPacketId++;
                ParentServer.Send(stream.GetRawBuffer(), RemoteEndPoint);
                Console.WriteLine("[NetworkingClient - " + RemoteEndPoint.ToString() + "] sent packet type " + Convert.ToString(PacketType.Data) + " (as hex)");
            }
        }
        public void SetAuthenticated(bool authenticated)
        {
            IsAuthenticated = authenticated;
            if(IsAuthenticated)
            {
                Send("6f200803000000cf001204840000000041010000043f1a003d484d3d2048616c6c2d4d616e6961204d6978205365727665720100e3003d484d3d2048616c6c2d4d616e6961204d697820536572766572202870726f043f7669646564206279206266326c2e6465297c2020202020202020202020202020202020202020202020202056656869636c653a204d6f202b20446f7c202020043f2020202020202020202020496e66616e747269653a204469202b204d69202b204672202b205361202b20536f7c7c20202020202020202020486f6d6570616700");
                Send("6f3008030000008d000e08043f653a20687474703a2f2f7777772e68616c6c2d6d616e69612e64657c2020202020202020202020205465616d737065616b333a203231322e3232342e38342e043f36383a39313334cf00000080a4eca36c03000000693a06000000060000001e000080110090000000c003000000a30200001900000019000060320000001000040600003200000020");
                Send("6f400803000000cf00120c84050000000d010000043f0a00000009006b756272615f64616d060067706d5f637140000c0064616c69616e5f706c616e74060067706d5f637140001100726f61645f746f5f6a616c61043f6c61626164060067706d5f637140001000646171696e675f6f696c6669656c6473060067706d5f637140000c0067756c665f6f665f6f6d616e060067706d5f043f637140000d006d617368747575725f63697479060067706d5f637140000d007461726162615f717561727279060067706d5f6371400014006f70657261746900");
                Send("6f60100f00000023011a14043f6f6e736d6f6b6573637265656e060067706d5f6371400010006f7065726174696f6e68617276657374060067706d5f637140001100737472696b655f61745f04116b61726b616e64060067706d5f63714000840000000041010000043f1a003d484d3d2048616c6c2d4d616e6961204d6978205365727665720100e3003d484d3d2048616c6c2d4d616e6961204d697820536572766572202870726f043f7669646564206279206266326c2e6465297c2020202020202020202020202020202020202020202020202056656869636c653a204d6f202b20446f7c202020043f2020202020202020202020496e66616e747269653a204469202b204d69202b204672202b205361202b20536f7c7c20202020202020202020486f6d6570616700");
            }
        }
    }
}