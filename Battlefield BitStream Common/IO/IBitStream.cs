using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.IO
{
    public interface IBitStream
    {
        void ReadBasicHeader(ref uint type, ref uint type2);
        void ReadExtendedHeader(ref uint type1, ref uint dataId, ref uint type3);
        string ReadString(uint len);
        byte[] ReadBytes(uint bytes);
        bool ReadBool();
        uint ReadBits(uint bits);
        uint ReadBits(uint bits, uint in_bits);
        uint ReadBits(uint bits, byte[] i, uint in_bits);
        void WriteBasicHeader(uint type, uint type2);
        void WriteExtendedHeader(uint type, uint type2, uint type3);
        void WriteString(string data, uint len);
        void WriteBytes(byte[] bytes);
        void WriteBool(bool b);
        void WriteBits(uint data, uint bits);
        uint WriteBits(uint data, uint bits, byte[] @out, uint out_bits);
        byte[] GetRawBuffer();
        uint GetPosition();
        void SetPosition(uint position);
        bool SkipBits(uint bits);
        uint GetLength();
    }
}