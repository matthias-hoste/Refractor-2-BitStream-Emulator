using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream.Core.Structs;
using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Networking
{
    public class NetworkingServer
    {
        public Dictionary<IPEndPoint, INetworkingClient> ConnectedClients { get; private set; }
        private uint ConnectionId { get; set; }
        private Socket _serverSocket { get; set; }
        private byte[] Buffer = new byte[8192];
        public ConcurrentQueue<NewPacketStruct> _newPackets { get; set; }
        private bool _stopping { get; set; }
        public NetworkingServer()
        {
            ConnectionId = 1;
            ConnectedClients = new Dictionary<IPEndPoint, INetworkingClient>();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            _serverSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            _newPackets = new ConcurrentQueue<NewPacketStruct>();
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            Console.WriteLine("[NetworkingServer] bound to " + _serverSocket.LocalEndPoint.ToString());
            for (int i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() => PacketHandlerLoop());
            }
            Console.WriteLine("[NetworkingServer] started handler threads");
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                _serverSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref ep, ClientReadCallback, null);
            }
            catch (Exception)
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
            catch (Exception)
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
            while (!_stopping)
            {
                NewPacketStruct packetStruct;
                while (_newPackets.TryDequeue(out packetStruct))
                {
                    var stream = new BitStream(packetStruct.Data);
                    uint type = 0;
                    uint connectionId = 0;
                    stream.ReadBasicHeader(ref type, ref connectionId);
                    Console.WriteLine("[NetworkingServer] handling packet received from " + packetStruct.From.ToString() + ", connection id: " + connectionId + ", packet type: " + Convert.ToString((PacketType)type));
                    if (type == 0)
                    {
                        Console.WriteLine("[NetworkingServer] skipping invalid packet");
                        continue;
                    }
                    if (type == 1)
                    {
                        var par2 = stream.ReadBits(32);
                        var version = stream.ReadBits(32);
                        var punkbuster = stream.ReadBits(1);
                        var unk = stream.ReadBits(32);
                        var pass = stream.ReadString(32).Replace("\0", "");
                        var mod = stream.ReadString(32).Replace("\0", "");
                        if(mod != Config.ModName)
                            SendConnectDenied(packetStruct.From, 0x24);
                        ConnectionId++;
                        var client = new NetworkingClient(this, packetStruct.From, ConnectionId);
                        if (ConnectedClients.ContainsKey(packetStruct.From))
                            ConnectedClients.Remove(packetStruct.From);
                        ConnectedClients.Add(packetStruct.From, client);
                        SendConnectAccept(packetStruct.From, ConnectionId);
                        continue;
                    }
                    if (!ConnectedClients.ContainsKey(packetStruct.From))
                        continue;
                    ConnectedClients[packetStruct.From].Handle((PacketType)type, stream);
                }
                Thread.Sleep(1);
            }
        }
        public void Send(byte[] packet, IPEndPoint endPoint)
        {
            try
            {
                _serverSocket.SendTo(packet, endPoint);
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
            catch (Exception)
            {

            }
        }
    }
}