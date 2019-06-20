using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Objects
{
    public interface IObject
    {
        ITemplate Template { get; set; }
        int ObjectId { get; set; }
        string ObjectName { get; set; }
        Vector3 ObjectPosition { get; set; }
        Vector3 ObjectRotation { get; set; }
    }
}