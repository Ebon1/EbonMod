using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace EbonianMod.Common.NetCode.Packets
{
    internal class SyncedRandPacket : PacketHandler
    {
        public SyncedRandPacket(byte handlerType) : base(handlerType) { }
        public void Send(int seed, int toWho = -1)
        {
            ModPacket packet = GetPacket(HandlerType);
            packet.Write(seed);
            packet.Send(toWho);
        }
        public override void HandlePacket(BinaryReader reader)
        {
            int seed = reader.ReadInt32();
            if (Main.dedServ)
                Send(seed);
            else
                SyncedRand.seed = seed;
        }
    }
}
