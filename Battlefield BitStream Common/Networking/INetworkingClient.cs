using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_BitStream_Common.Networking
{
    public interface INetworkingClient
    {
        IRemoteEventManager RemoteEventManager { get; }
        IDataBlockManager BlockManager { get; }
        bool IsConnected { get; }
        void Handle(PacketType packetType, IBitStream stream);
        void SendPingRequest();
        void SendPingResponse(uint token);
        void SendEvent(IGameEvent gameEvent);
        void Send();
        void Send(string hex);
        void SetAuthenticated(bool authenticated);
    }
}