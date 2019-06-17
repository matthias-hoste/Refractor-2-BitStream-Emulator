using Battlefield_BitStream_Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Engine
{
    public interface IBF2Engine
    {
        uint ServerPort { get; }
        List<IMap> MapList { get; }
        ILevel Level { get; }
        void InitEngine();
        void LoadServerArchives();
        void LoadKitData();
        void LoadLevel(IMap map);
        void SetServerPort(uint port);
    }
}