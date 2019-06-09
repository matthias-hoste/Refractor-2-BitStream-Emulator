using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFS;

namespace Battlefield_BitStream_Common.Processors
{
    public interface IConFileProcessor
    {
        void RegisterConMethod(string name, Func<object, object, int> method);
        void ExecuteConFile(string file);
        void ExecuteConFile(VFile file);
    }
}