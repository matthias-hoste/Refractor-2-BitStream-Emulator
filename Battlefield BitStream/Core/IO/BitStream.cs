using Battlefield_BitStream_Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.IO
{
    public class BitStream : IBitStream
    {
        private byte[] _buffer { get; set; }
        public uint Position { get; private set; }
        public uint Length { get; private set; }
        public int BitLength { get; private set; }
        public BitStream() : this(new byte[8192])
        {
        }
        public BitStream(byte[] data)
        {
            _buffer = data;
            Position = 0;
            Length = 0;
            BitLength = _buffer.Length * 8;
        }
        public static IBitStream Create()
        {
            return new BitStream();
        }
        public void ReadBasicHeader(ref uint type, ref uint type2)
        {
            type = ReadBits(4);
            type2 = ReadBits(8);
        }
        public void ReadExtendedHeader(ref uint type1, ref uint dataId, ref uint type3)
        {
            type1 = ReadBits(6);
            dataId = ReadBits(6);
            type3 = ReadBits(32);
        }
        public string ReadString(uint len)
        {
            int i;
            string str = "";
            for (i = 0; i < len; i++)
            {
                str += (char)ReadBits(8);
            }
            return str;
        }
        public byte[] ReadBytes(uint bytes)
        {
            var data = new byte[bytes];
            int i = 0;
            while(i < bytes)
            {
                data[i] = (byte)ReadBits(8);
                i++;
            }
            return data;
        }
        public bool ReadBool()
        {
            return ReadBits(1) == 1;
        }
        public uint ReadBits(uint bits)
        {
            return ReadBits(bits, (uint)Position);
        }
        public uint ReadBits(uint bits, uint in_bits)
        {
            return ReadBits(bits, _buffer, in_bits);
        }
        public uint ReadBits(uint bits, byte[] i, uint in_bits)
        {
            uint seek_bits;
            uint rem;
            uint seek = 0;
            uint ret = 0;
            uint mask = 0xffffffff;
            Position += bits;
            if (bits > 32)
            {
                return (0);
            }
            if (bits < 32)
            {
                mask = (uint)((1 << (int)bits) - 1);
            }
            for (; ; )
            {
                seek_bits = in_bits & 7;
                ret |= (uint)((i[in_bits >> 3] >> (int)seek_bits) & mask) << (int)seek;
                rem = (uint)(8 - seek_bits);
                if (rem >= bits)
                {
                    break;
                }
                bits -= rem;
                in_bits += rem;
                seek += rem;
                mask = (uint)((1 << (int)bits) - 1);
            }
            return (ret);
        }
        public void WriteBasicHeader(uint type, uint type2)
        {
            Position = WriteBits(type, 4, _buffer, Position);
            Position = WriteBits(type2, 8, _buffer, Position);
        }
        public void WriteExtendedHeader(uint type, uint type2, uint type3)
        {
            Position = WriteBits(type, 6, _buffer, Position);
            Position = WriteBits(type2, 6, _buffer, Position);
            Position = WriteBits(type3, 32, _buffer, Position);
        }
        public void WriteString(string data, uint len)
        {
            int i;

            for (i = 0; i < len; i++)
            {
                if (string.IsNullOrEmpty(data) || data.Length <= i)
                    break;
                Position = WriteBits(data[i], 8, _buffer, Position);
            }
            for (; i < len; i++)
            {
                Position = WriteBits(0, 8, _buffer, Position);
            }
        }
        public void WriteBytes(byte[] bytes)
        {
            int i;
            for (i = 0; i < bytes.Length; i++)
            {
                Position = WriteBits(bytes[i], 8, _buffer, Position);
            }
        }
        public void WriteBool(bool b)
        {
            uint v = 0;
            if (b)
                v = 1;
            WriteBits(v, 1);
        }
        public void WriteBits(uint data, uint bits)
        {
            Position = WriteBits(data, bits, _buffer, Position);
        }
        public uint WriteBits(uint data, uint bits, byte[] @out, uint out_bits)
        {
            uint seek_bits;
            uint rem;
            uint mask;

            if (bits > 32)
            {
                return (out_bits);
            }
            if (bits < 32)
            {
                data &= (uint)(1 << (int)bits) - 1;
            }
            for (; ; )
            {
                seek_bits = out_bits & (uint)7;
                mask = (uint)((1 << (int)seek_bits) - 1);
                if ((bits + seek_bits) < 8)
                {
                    mask |= (uint)~(((1 << (int)bits) << (int)seek_bits) - 1);
                }
                @out[out_bits >> 3] &= (byte)mask; // zero
                @out[out_bits >> 3] |= (byte)(data << (int)seek_bits);
                rem = (8 - seek_bits);
                if (rem >= bits)
                {
                    break;
                }
                out_bits += rem;
                bits -= rem;
                data = (data >> (int)rem);
            }
            Length += bits;
            return (out_bits + bits);
        }
        public byte[] GetRawBuffer()
        {
            var len = ((((Position) + 7) & ~7) >> 3);
            var sendArray = new byte[len];
            Array.Copy(_buffer, sendArray, len);
            return sendArray;
        }
        public uint GetPosition()
        {
            return Position;
        }
        public void SetPosition(uint position)
        {
            Position = position;
        }
        public uint GetLength()
        {
            return Length;
        }
        public bool SkipBits(uint bits)
        {
            if ((Position + bits) > BitLength)
                return false;
            Position += bits;
            return true;
        }
    }
}