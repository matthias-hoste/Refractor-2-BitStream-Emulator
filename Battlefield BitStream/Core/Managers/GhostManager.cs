using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Managers
{
    public class GhostManager
    {
        public virtual int ProcessReceivedPacket(IBitStream stream)
        {
            uint val1 = stream.ReadBits(1);
            if (val1 != 1)
                return 1;
            uint someVal = stream.ReadBits(32);
            uint someVal2 = stream.ReadBits(8);
            uint someVal3 = stream.ReadBits(1);
            ReadControlObjectState(stream);
            return 0;
        }
        public virtual int ReadData(IBitStream stream)
        {
            uint someVal = stream.ReadBits(2);
            uint someVal2 = stream.ReadBits(16);
            return 0;
        }
        public virtual int ReadControlObjectState(IBitStream stream)
        {
            uint someVal = stream.ReadBits(12);
            uint i = stream.ReadBits(1);
            if(i == 1)
            {

            }
            else
            {
                var ii = stream.ReadBits(31);
            }
            return 0;
        }
    }
}