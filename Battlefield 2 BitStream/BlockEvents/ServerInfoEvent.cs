using Battlefield_BitStream_Common.Enums;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.IO;
using Battlefield_BitStream_Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battlefield_2_BitStream.BlockEvents
{
    public class ServerInfoEvent : IBlockEvent
    {
        public string GameName { get; set; }
        public string GameDescription { get; set; }
        public uint AllowNoseCam { get; set; }
        public uint HitIndicator { get; set; }
        public uint TeamKillPunishByDefault { get; set; }
        public uint AllowFreeCam { get; set; }
        public uint AllowExternalViews { get; set; }
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }
        public uint Unknown5 { get; set; }
        public uint Unknown6 { get; set; }
        public uint Unknown7 { get; set; }
        public uint GameSpyPort { get; set; }
        public uint RadioSpamInterval { get; set; }
        public uint RadioMaxSpamFlagCount { get; set; }
        public uint RadioBlockedDurationTime { get; set; }
        public uint VotingEnabled { get; set; }
        public uint FriendlyFireWithMines { get; set; }
        public uint TeamVoteOnly { get; set; }
        public uint NoVehicles { get; set; }
        public uint RoundsPerMap { get; set; }
        public uint SpawnTime { get; set; }
        public uint TimeLimit { get; set; }
        public uint TicketRatio { get; set; }
        public uint TeamRatioPercent { get; set; }
        public uint AutoBalanceTeam { get; set; }
        public uint UseGlobalUnlocks { get; set; }
        public uint CoopBotRatio { get; set; }
        public uint CoopBotCount { get; set; }
        public uint CoopBotDifficulty { get; set; }
        public BlockEventId BlockEventId => BlockEventId.ServerInfo;
        public void Process(INetworkingClient client)
        {

        }
        public void Transmit(INetworkingClient client)
        {
            client.BlockManager.QueueBlockEvent(this);
        }
        public void Serialize(IBitStream stream)//datablock events are different from game events, but not much
        {
            GameName = "LifeCoder's";
            GameDescription = "I did it biiiitch";
            stream.WriteBits((uint)GameName.Length, 0x10);
            stream.WriteString(GameName, (uint)GameName.Length);
            stream.WriteBits(1, 0x10);
            stream.WriteBits((uint)GameDescription.Length, 0x10);
            stream.WriteString(GameDescription, (uint)GameDescription.Length);
            stream.WriteBits(1, 1);
            stream.WriteBits(Unknown1, 1);//unknown
            stream.WriteBits(AllowNoseCam, 1);
            stream.WriteBits(HitIndicator, 1);
            stream.WriteBits(TeamKillPunishByDefault, 1);
            stream.WriteBits(AllowFreeCam, 1);
            stream.WriteBits(AllowExternalViews, 1);
            stream.WriteBits(Unknown2, 32);//unknown
            stream.WriteBits(Unknown3, 0x10);//unknown
            stream.WriteBits(Unknown4, 0x10);//unknown
            stream.WriteBits(0, 1);
            stream.WriteBits(Unknown5, 31);//unknown
            stream.WriteBits(GameSpyPort, 0x10);
            stream.WriteBits(0, 1);
            stream.WriteBits(RadioSpamInterval, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(RadioMaxSpamFlagCount, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(RadioBlockedDurationTime, 31);
            stream.WriteBits(1, 1);
            stream.WriteBits(VotingEnabled, 31);
            stream.WriteBits(Unknown6, 0x10);//unknown
            stream.WriteBits(FriendlyFireWithMines, 1);
            stream.WriteBits(TeamVoteOnly, 1);
            stream.WriteBits(NoVehicles, 1);
            stream.WriteBits(Unknown7, 1);//unknown
            stream.WriteBits(0, 1);
            stream.WriteBits(RoundsPerMap, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(SpawnTime, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(TimeLimit, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(TicketRatio, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(TeamRatioPercent, 31);
            stream.WriteBits(AutoBalanceTeam, 1);
            stream.WriteBits(UseGlobalUnlocks, 1);
            stream.WriteBits(0, 1);
            stream.WriteBits(CoopBotRatio, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(CoopBotCount, 31);
            stream.WriteBits(0, 1);
            stream.WriteBits(CoopBotDifficulty, 31);
        }
        public IBlockEvent DeSerialize(IBitStream stream)
        {
            var serverInfo = new ServerInfoEvent();
            uint gameNameLength = stream.ReadBits(0x10);
            serverInfo.GameName = stream.ReadString(gameNameLength);
            uint v10 = 0;
            uint v11 = stream.ReadBits(0x10);
            var description = "";
            do
            {
                uint descriptionPartLength = stream.ReadBits(0x10);
                if(descriptionPartLength != 0)
                {
                    description += stream.ReadString(descriptionPartLength);
                }
                v10++;
            }
            while (v11 > v10);
            serverInfo.GameDescription = description;
            var v12 = stream.ReadBits(1);//20//some value that is always 1
            serverInfo.Unknown1 = stream.ReadBits(1);//21
            serverInfo.AllowNoseCam = stream.ReadBits(1);//22
            serverInfo.HitIndicator = stream.ReadBits(1);//23
            serverInfo.TeamKillPunishByDefault = stream.ReadBits(1);//24
            serverInfo.AllowFreeCam = stream.ReadBits(1);//25
            serverInfo.AllowExternalViews = stream.ReadBits(1);//26
            serverInfo.Unknown2 = stream.ReadBits(32);//7
            serverInfo.Unknown3 = stream.ReadBits(0x10);//16
            serverInfo.Unknown4 = stream.ReadBits(0x10);//17
            var v22 = stream.ReadBits(1);
            serverInfo.Unknown5 = stream.ReadBits(31);//9
            serverInfo.GameSpyPort = stream.ReadBits(0x10);//26
            var v25 = stream.ReadBits(1);
            serverInfo.RadioSpamInterval = stream.ReadBits(31);//10
            var v27 = stream.ReadBits(1);
            serverInfo.RadioMaxSpamFlagCount = stream.ReadBits(31);//11
            var v29 = stream.ReadBits(1);
            serverInfo.RadioBlockedDurationTime = stream.ReadBits(31);//12
            var v31 = stream.ReadBits(1);//54
            serverInfo.VotingEnabled = stream.ReadBits(1);//55
            serverInfo.Unknown6 = stream.ReadBits(0x10);//28
            serverInfo.FriendlyFireWithMines = stream.ReadBits(1);//58
            serverInfo.TeamVoteOnly = stream.ReadBits(1);//59
            serverInfo.NoVehicles = stream.ReadBits(1);//60
            serverInfo.Unknown7 = stream.ReadBits(1);//61
            var v38 = stream.ReadBits(1);
            serverInfo.RoundsPerMap = stream.ReadBits(31);//16
            var v40 = stream.ReadBits(1);
            serverInfo.SpawnTime = stream.ReadBits(31);//17
            var v42 = stream.ReadBits(1);
            serverInfo.TimeLimit = stream.ReadBits(31);//18
            var v44 = stream.ReadBits(1);
            serverInfo.TicketRatio = stream.ReadBits(31);//19
            var v46 = stream.ReadBits(1);
            serverInfo.TeamRatioPercent = stream.ReadBits(31);//20
            serverInfo.AutoBalanceTeam = stream.ReadBits(1);//84
            serverInfo.UseGlobalUnlocks = stream.ReadBits(1);//85
            var v50 = stream.ReadBits(1);
            serverInfo.CoopBotRatio = stream.ReadBits(31);//22
            var v52 = stream.ReadBits(1);
            serverInfo.CoopBotCount = stream.ReadBits(31);//23
            var v54 = stream.ReadBits(1);
            serverInfo.CoopBotDifficulty = stream.ReadBits(31);//24
            return serverInfo;
        }
    }
}