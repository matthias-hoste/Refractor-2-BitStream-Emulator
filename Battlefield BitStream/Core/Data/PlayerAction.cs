using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Data
{
    public class PlayerAction
    {
        public virtual int ReadFromStream(IBitStream stream)
        {
            int v = 0;
            do
            {
                uint someVal = stream.ReadBits(1);
                if(someVal == 1)
                {

                }
                else
                {
                    uint someVal2 = stream.ReadBits(15);
                }
                v++;
            }
            while (v <= 5);
            uint vv = stream.ReadBits(32);
            uint vvv = stream.ReadBits(9);
            uint vvvv = stream.ReadBits(1);
            return 0;
        }
    }
}