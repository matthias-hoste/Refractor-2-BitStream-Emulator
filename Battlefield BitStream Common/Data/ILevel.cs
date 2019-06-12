using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Data
{
    public interface ILevel
    {
        IMap Map { get; }
        void InitLevel();
    }
}