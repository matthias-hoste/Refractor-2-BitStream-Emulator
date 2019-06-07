using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.GameEvents
{
    public class DataBlockEvent : IGameEvent
    {
        public bool NewBlock { get; set; }
        public uint BlockLength { get; set; }
        public uint BlockEventId { get; set; }
        public byte[] Block { get; set; }
        public DataBlockEvent()
        {

        }
        public DataBlockEvent(uint totalSize, uint blockNumber)
        {
            NewBlock = true;
            BlockEventId = blockNumber;
            BlockLength = totalSize;
        }
        public DataBlockEvent(byte[] block, uint totalSize)
        {
            NewBlock = false;
            BlockLength = totalSize;
            Block = block;
        }
        public void Serialize(IBitStream stream)
        {
            stream.WriteBits(4, 7);//write our id
            if (NewBlock)
                stream.WriteBits(1, 1);
            else
                stream.WriteBits(0, 1);
            if(NewBlock)
            {
                stream.WriteBits(BlockEventId, 32);
                stream.WriteBits(BlockLength, 32);
            }
            else
            {
                stream.WriteBits(BlockLength, 8);
                stream.WriteBytes(Block);
            }
        }

        public void Transmit(INetworkingClient client)
        {
            client.SendEvent(this);
        }

        public void Process(INetworkingClient client)//currently only support server clients and not regular clients
        {
            if (NewBlock)
                client.BlockManager.NewBlock(BlockEventId, BlockLength);
            else
                client.BlockManager.AddBlock(Block);
        }

        public IGameEvent DeSerialize(IBitStream stream)
        {
            var block = new DataBlockEvent();
            var newBlock = stream.ReadBool();
            if(!newBlock)
            {
                uint blockLength = stream.ReadBits(8); //actual formula in the game exe: 8 * stream.ReadBits(8) & 0x7F8; we just read the bytes since this formula is used to read it as bits
                block.Block = stream.ReadBytes(blockLength);
            }
            else
            {
                block.BlockEventId = stream.ReadBits(32);//triggers specific event in EventManager, 1=ClientInfo
                block.BlockLength = stream.ReadBits(32);//full length of incoming data(in case it's sent in multiple packets
                block.NewBlock = true;
            }
            return block;
        }
    }
}