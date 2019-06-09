using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Data
{
    public class RemoteEvent
    {
        public uint TopId { get; set; }
        public uint BottomId { get; set; }
        public Func<uint, uint, uint> EventHandler { get; set; }
    }
}