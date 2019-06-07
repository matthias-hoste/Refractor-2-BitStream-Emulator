using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common
{
    public interface IMod
    {
        string Name { get; }
        bool RequiresChallenge { get; }
        IBitStreamExtension BitStreamExtension { get; }
        void Initialize(IEventRegistry registry);
        IConFileProcessor GetConFileProcessor();
    }
}