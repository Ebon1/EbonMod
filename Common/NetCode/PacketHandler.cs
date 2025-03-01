using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace EbonianMod.Common.NetCode
{
    internal abstract class PacketHandler
    {
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader);

        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        protected ModPacket GetPacket(byte packetType)
        {
            var p = EbonianMod.Instance.GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}
