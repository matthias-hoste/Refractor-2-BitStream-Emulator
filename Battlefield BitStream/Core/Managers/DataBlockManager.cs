using Battlefield_2_BitStream.BlockEvents;
using Battlefield_2_BitStream.GameEvents;
using Battlefield_BitStream.Core.Extensions;
using Battlefield_BitStream.Core.IO;
using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.Managers;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Battlefield_BitStream.Core.Managers
{
    public class DataBlockManager : IDataBlockManager
    {
        public Action<IBlockEvent> BlockReceived { get; set; }
        public INetworkingClient Client { get; private set; }
        public IBlockEventManager BlockEventManager { get; private set; }
        private static byte[] FullDataBlock { get; set; }
        private static uint DataBlockEventId { get; set; }
        private static uint DataBlockPosition { get; set; }
        private ConcurrentQueue<IBlockEvent> SendBlockQueue { get; set; }
        public DataBlockManager(INetworkingClient client)
        {
            Client = client;
            BlockEventManager = new BlockEventManager(Client);
            BlockEventManager.RegisterEvents();
            SendBlockQueue = new ConcurrentQueue<IBlockEvent>();
            Task.Factory.StartNew(() => Update());
        }
        private void Update()
        {
            while(Client.IsConnected)
            {
                lock (SendBlockQueue)
                {
                    IBlockEvent blockEvent;
                    if (SendBlockQueue.TryDequeue(out blockEvent))
                    {
                        var serializer = BitStream.Create();
                        blockEvent.Serialize(serializer);
                        var block = serializer.GetRawBuffer();
                        var newBlockEvent = new DataBlockEvent((uint)block.Length, (uint)blockEvent.BlockEventId);//first we send info about a new block
                        Client.SendEvent(newBlockEvent);//this doesnt send yet, it queues the event
                        foreach (var chunk in block.GetChunks(63))
                        {
                            var sendBlockEvent = new DataBlockEvent(chunk, (uint)chunk.Length);
                            Client.SendEvent(sendBlockEvent);//queue the chunks
                        }
                        Client.Send();//send the block data
                    }
                }
                Thread.Sleep(500);
            }
        }
        public void NewBlock(uint eventId, uint size)
        {
            FullDataBlock = new byte[size];//should probably sanitize this to prevent a buffer overflow
            DataBlockPosition = 0;
            DataBlockEventId = eventId;
        }
        public void AddBlock(byte[] block)
        {
            Array.Copy(block, 0, FullDataBlock, DataBlockPosition, block.Length);
            DataBlockPosition += (uint)block.Length;
            if(DataBlockPosition == FullDataBlock.Length)//we got a full block
            {
                var eventInstance = BlockEventManager.Trigger((BlockEventId)DataBlockEventId, new BitStream(FullDataBlock));
                if (BlockReceived != null)
                    BlockReceived(eventInstance);
            }
        }
        public void QueueBlockEvent(IBlockEvent blockEvent)
        {
            lock(SendBlockQueue)
                SendBlockQueue.Enqueue(blockEvent);
        }
    }
}