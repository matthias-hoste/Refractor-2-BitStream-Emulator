using Battlefield_2_BitStream.GameEvents;
using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream.Core.Managers;
using Battlefield_BitStream.Core.Structs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Game
{
    public class GameServer
    {
        private Socket _serverSocket { get; set; }
        private byte[] Buffer = new byte[8192];
        public ConcurrentQueue<NewPacketStruct> _newPackets { get; set; }
        private bool _stopping { get; set; }
        private static int pppp = 1;
        public GameServer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _newPackets = new ConcurrentQueue<NewPacketStruct>();
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            Console.WriteLine("[GameServer] bound to " + _serverSocket.LocalEndPoint.ToString());
            for(int i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() => PacketHandlerLoop());
            }
            Console.WriteLine("[GameServer] started handler threads");
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                _serverSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref ep, ClientReadCallback, null);
            }
            catch(Exception)
            {

            }
        }

        private void ClientReadCallback(IAsyncResult ar)
        {
            IPEndPoint remote;
            int transferred = 0;
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                transferred = _serverSocket.EndReceiveFrom(ar, ref ep);
                if (transferred <= 0)
                    throw new Exception();
                remote = (IPEndPoint)ep;
            }
            catch(Exception)
            {
                return;
            }
            var bytes = new byte[transferred];
            Array.Copy(Buffer, bytes, transferred);
            var newPacket = new NewPacketStruct();
            newPacket.From = remote;
            newPacket.Data = bytes;
            _newPackets.Enqueue(newPacket);
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                _serverSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref ep, ClientReadCallback, null);
            }
            catch (Exception)
            {

            }
        }
        private void PacketHandlerLoop()
        {
            while(!_stopping)
            {
                NewPacketStruct packetStruct;
                while(_newPackets.TryDequeue(out packetStruct))
                {
                    var stream = new BitStream(packetStruct.Data);
                    uint type = 0;
                    uint connectionId = 0;
                    stream.ReadBasicHeader(ref type, ref connectionId);
                    Console.WriteLine("[GameServer] handling packet received from " + packetStruct.From.ToString() + ", connection id: " + connectionId);
                    if (type == 0)
                    {
                        Console.WriteLine("[GameServer] skipping invalid packet");
                        continue;
                    }
                    if(type == 1)
                    {
                        Console.WriteLine("[GameServer] received a connection request");
                        var par2 = stream.ReadBits(32);
                        var version = stream.ReadBits(32);
                        var punkbuster = stream.ReadBits(1);//should always be 0
                        var unk = stream.ReadBits(32);
                        var pass = stream.ReadString(32);
                        var mod = stream.ReadString(32);
                        //SendConnectDenied(packetStruct.From, 29);
                        SendConnectAccept(packetStruct.From, 7);
                        continue;
                    }
                    else if(type == 2)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received connect accept");
                    }
                    else if (type == 3)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received connect denied");
                    }
                    else if(type == 4)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received connect ack");
                        SendPingRequest(packetStruct.From);
                    }
                    else if(type == 5)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received disconnect");
                    }
                    else if(type == 7)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received ping request");
                        uint type1 = 0;
                        uint Type2 = 0;
                        uint type3 = 0;
                        stream.ReadExtendedHeader(ref type1, ref Type2, ref type3);
                        uint i = stream.ReadBits(1);
                        uint token = stream.ReadBits(32);
                        SendPingReply(packetStruct.From, token);
                    }
                    else if (type == 8)
                    {//TODO: handle
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received ping response");
                    }
                    else if(type == 15)
                    {
                        Console.WriteLine("[GameServer] " + packetStruct.From.ToString() + " received data packet");
                        uint t1 = 0;
                        uint t2 = 0;
                        uint t3 = 0;
                        stream.ReadExtendedHeader(ref t1, ref t2, ref t3);
                        var packetLength = stream.ReadBits(16);//full packet length in bytes
                        new PlayerActionManager().ProcessReceivedPacket(stream);
                        //new GameEventManager().ProcessReceivedPacket(stream);
                        new GhostManager().ProcessReceivedPacket(stream);
                        SendChallengeEvent(packetStruct.From);
                    }
                    else
                    {

                    }
                }
                Thread.Sleep(1);
            }
        }
        private void SendChallengeEvent(IPEndPoint ep)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(15, 7);
                stream.WriteExtendedHeader(1, 0, 0);//first value should always increment
                stream.WriteBits(14, 16);
                stream.WriteBits(0, 1);
                stream.WriteBits(1, 1);
                stream.WriteBits(1, 8);
                stream.WriteBits(0, 5);
                stream.WriteBits(0, 1);
                var challengeEvent =  ChallengeEvent.CreateNewChallenge();
                challengeEvent.ModName = "";//bf2 for bf2 and empty for bf2142
                challengeEvent.Serialize(stream);
                stream.WriteBits(0, 1);
                _serverSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendPingRequest(IPEndPoint ep)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(7, 7);
                stream.WriteExtendedHeader(1, 0, 0);
                stream.WriteBits(1, 1);
                stream.WriteBits(7, 32);
                _serverSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch(Exception)
            {

            }
        }
        private void SendPingReply(IPEndPoint ep, uint token)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(8, 7);
                stream.WriteExtendedHeader(63, 0, 0);
                stream.WriteBits(0, 1);
                stream.WriteBits(token, 32);
                stream.WriteBits(0, 32);
                _serverSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendConnectAccept(IPEndPoint ep, uint slot)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(2, 0);
                stream.WriteBits(slot, 8);//connection id, unique per connection and is always sent as second param in the basic header
                stream.WriteBits(36, 32);//weird time calculation shit
                stream.WriteBits(0, 1);
                _serverSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendConnectDenied(IPEndPoint ep, uint err = 36)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(3, 0);
                stream.WriteBits(err, 32);
                _serverSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch(Exception)
            {

            }
        }
    }
}