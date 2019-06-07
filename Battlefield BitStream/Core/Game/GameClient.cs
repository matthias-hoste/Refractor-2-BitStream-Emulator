using Battlefield_2_BitStream.GameEvents;
using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream.Core.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Game
{
    public class GameClient
    {
        private Socket _clientSocket { get; set; }
        private byte[] Buffer = new byte[8192];
        private uint slot { get; set; }
        private ManualResetEvent wsa { get; set; }
        public GameClient()
        {
            wsa = new ManualResetEvent(false);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _clientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                _clientSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref ep, ClientReadCallback, null);
            }
            catch (Exception)
            {

            }
        }

        public void Connect(uint version, string mod, IPEndPoint ep)//65552
        {
            var buff = new byte[0xffff];
            uint b = 0;
            var stream = new BitStream(buff);
            stream.WriteBasicHeader(1, 1);
            stream.WriteBits(4098, 32);
            stream.WriteBits(version, 32);
            stream.WriteBits(1, 1);//some sort of token, maybe to verify the ping request?
            stream.WriteBits(0, 32);
            stream.WriteString("", 32);
            stream.WriteString(mod, 32);
            _clientSocket.SendTo(stream.GetRawBuffer(), ep);
        }

        private void ClientReadCallback(IAsyncResult ar)
        {
            IPEndPoint remote;
            int transferred = 0;
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                transferred = _clientSocket.EndReceiveFrom(ar, ref ep);
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
            var stream = new BitStream(bytes);
            uint type = 0;
            uint connectionId = 0;
            stream.ReadBasicHeader(ref type, ref connectionId);
            Console.WriteLine("[GameClient] handling packet received from " + remote.ToString() + ", connection id: " + connectionId);
            if (type == 0)
            {
                Console.WriteLine("[GameClient] skipping invalid packet");
                goto skip;
            }
            if (type == 1)
            {
                Console.WriteLine("[GameClient] received a connection request");
            }
            else if (type == 2)
            {
                Console.WriteLine("[GameClient] " + remote.ToString() + " received connect accept");
                slot = stream.ReadBits(8);
                Console.WriteLine("Server gave us connection id " + slot);
                SendConnectAcceptAck(remote);
            }
            else if (type == 3)
            {
                Console.Write("[GameClient] " + remote.ToString() + " received connect denied");
                var err = stream.ReadBits(32);
                Console.WriteLine(", error: " + err);
            }
            else if (type == 4)
            {
                Console.WriteLine("[GameClient] " + remote.ToString() + " received connect ack");
            }
            else if (type == 5)
            {
                var error = stream.ReadBits(32);
                Console.WriteLine("[GameClient] " + remote.ToString() + " received disconnect, reason: " + error);
            }
            else if (type == 7)
            {
                Console.WriteLine("[GameClient] " + remote.ToString() + " received ping request");
                uint type1 = 0;
                uint Type2 = 0;
                uint type3 = 0;
                stream.ReadExtendedHeader(ref type1, ref Type2, ref type3);
                uint i = stream.ReadBits(1);
                uint token = stream.ReadBits(32);
                SendPingReply(remote, token);
            }
            else if (type == 8)
            {//TODO: handle
                Console.WriteLine("[GameClient] " + remote.ToString() + " received ping response");
            }
            else if (type == 15)
            {
                Console.WriteLine("[GameClient] " + remote.ToString() + " received data packet");
                uint t1 = 0;
                uint t2 = 0;
                uint t3 = 0;
                stream.ReadExtendedHeader(ref t1, ref t2, ref t3);
                var data = stream.ReadBits(16);
                new PlayerActionManager().ProcessReceivedPacket(stream);
                //new GameEventManager().ProcessReceivedPacket(stream);
                new GhostManager().ProcessReceivedPacket(stream);
                SendChallengeResponseEvent(remote);
            }
            else
            {

            }
            skip:
            try
            {
                EndPoint ep = new IPEndPoint(0x000000, 0);
                _clientSocket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None, ref ep, ClientReadCallback, null);
            }
            catch (Exception)
            {

            }
        }
        private void SendNullTrigger(IPEndPoint ep, uint slot)
        {
            try
            {
                for (uint i = 1; i < 31; i++)
                {
                    var buff = new byte[0xffff];
                    var stream = new BitStream(buff);
                    stream.WriteBasicHeader(15, 7);
                    stream.WriteExtendedHeader(i, 0, 0);
                    _clientSocket.SendTo(stream.GetRawBuffer(), ep);
                }
            }
            catch (Exception)
            {

            }
        }
        private void SendConnectionTypeEvent(IPEndPoint ep)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(15, slot);
                stream.WriteExtendedHeader(1, 0, 0);
                stream.WriteBits(17, 16);
                stream.WriteBits(0, 1);
                stream.WriteBits(1, 1);
                stream.WriteBits(1, 8);
                stream.WriteBits(0, 5);
                stream.WriteBits(0, 1);
                new ConnectionTypeEvent().Serialize(stream);
                stream.WriteBits(0, 1);
                _clientSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendChallengeResponseEvent(IPEndPoint ep)
        {
            try
            {
                var buff = new byte[0xffff];
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(15, slot);
                stream.WriteExtendedHeader(2, 1, 1);
                stream.WriteBits(88, 16);
                stream.WriteBits(0, 1);
                stream.WriteBits(1, 1);
                stream.WriteBits(1, 8);
                stream.WriteBits(0, 5);
                stream.WriteBits(0, 1);
                new ChallengeResponseEvent().Serialize(stream);
                stream.WriteBits(0, 1);
                _clientSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendConnectAcceptAck(IPEndPoint ep)
        {
            try
            {
                var buff = new byte[0xffff];
                uint b = 0;
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(4, slot);
                _clientSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
        private void SendPingReply(IPEndPoint ep, uint token)
        {
            try
            {
                var buff = new byte[0xffff];
                uint b = 0;
                var stream = new BitStream(buff);
                stream.WriteBasicHeader(8, 0);
                stream.WriteExtendedHeader(0, 0, 0);
                stream.WriteBits(0, 1);
                stream.WriteBits(token, 32);
                stream.WriteBits(0, 32);
                _clientSocket.SendTo(stream.GetRawBuffer(), ep);
            }
            catch (Exception)
            {

            }
        }
    }
}