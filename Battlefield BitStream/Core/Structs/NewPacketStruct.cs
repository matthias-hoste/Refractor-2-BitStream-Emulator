using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Structs
{
    public class NewPacketStruct
    {
        public IPEndPoint From { get; set; }
        public byte[] Data { get; set; }
    }
}