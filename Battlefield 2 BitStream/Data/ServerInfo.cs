using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.Data
{
    public class ServerInfo
    {
        public string ServerName { get; set; }
        public string ServerPassword { get; set; }
        public uint Ranked { get; set; }
        public uint AllowFreeCam { get; set; }
        public uint AllowExternalViews { get; set; }
    }
}