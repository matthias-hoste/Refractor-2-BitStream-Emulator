using Battlefield_BitStream.Core.Data;
using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Managers
{
    public class PlayerActionManager
    {
        public virtual int ProcessReceivedPacket(IBitStream stream)
        {
            uint playerAction = stream.ReadBits(1);
            if (playerAction != 1)
                return 1;
            uint someVal = stream.ReadBits(4);
            uint someVal2 = stream.ReadBits(9);
            uint someVal3 = 0;
            if(someVal > 0)
            {
                uint action2 = stream.ReadBits(1);
                if(action2 == 1)
                {

                }
                else
                {
                    someVal3 = stream.ReadBits(31);
                    
                }
            }
            if (someVal <= 0)
            {
                return 1;
            }
            new PlayerAction().ReadFromStream(stream);
            return 0;
        }
    }
}