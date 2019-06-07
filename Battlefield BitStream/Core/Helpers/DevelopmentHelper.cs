using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream.Core.Managers;
using Battlefield_BitStream.Core.Networking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Helpers
{
    public static class DevelopmentHelper
    {
        public static byte[] ParseHexString(string text)
        {
            if ((text.Length % 2) != 0)
            {
                throw new ArgumentException("Invalid length: " + text.Length);
            }

            if (text.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text.Substring(2);
            }

            int arrayLength = text.Length / 2;
            byte[] byteArray = new byte[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                byteArray[i] = byte.Parse(text.Substring(i * 2, 2), NumberStyles.HexNumber);
            }

            return byteArray;
        }
        private static NetworkingClient client { get; set; }
        private static GameEventManager GameEventManager { get; set; }
        public static void DeconstructPacket(string hex)
        {
            if (client == null)
            {
                client = new NetworkingClient(null, null, 2);
            }
            var stream = new BitStream(ParseHexString(hex));
            uint type = 0;
            uint connectionId = 0;
            stream.ReadBasicHeader(ref type, ref connectionId);
            if (type == 0)
            {
            }
            if (type == 1)
            {
            }
            else if (type == 2)
            {
            }
            else if (type == 3)
            {
            }
            else if (type == 4)
            {
            }
            else if (type == 5)
            {
                var err = stream.ReadBits(0x20);
                var msgLength = stream.ReadBits(0x10);
                var msg = stream.ReadString(msgLength);
            }
            else if (type == 7)
            {
            }
            else if (type == 8)
            {
            }
            else if (type == 15)
            {
                uint t1 = 0;
                uint t2 = 0;
                uint t3 = 0;
                stream.ReadExtendedHeader(ref t1, ref t2, ref t3);
                var packetLength = stream.ReadBits(16);
                new PlayerActionManager().ProcessReceivedPacket(stream);
                client.GameEventManager.ProcessReceivedPacket(stream);
                new GhostManager().ProcessReceivedPacket(stream);
            }
            else
            {

            }
        }
    }
}