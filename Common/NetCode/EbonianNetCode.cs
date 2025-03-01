using EbonianMod.Common.NetCode.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common.NetCode
{
    internal class EbonianNetCode
    {
        internal static SyncedRandPacket syncedRand = new((byte)EbonianMessages.SyncedRand);
        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch ((EbonianMessages)reader.ReadByte())
            {
                case EbonianMessages.SyncedRand:
                    syncedRand.HandlePacket(reader);
                    break;
            }
        }
    }
    internal enum EbonianMessages : byte
    {
        None,
        SyncedRand
    }
}
