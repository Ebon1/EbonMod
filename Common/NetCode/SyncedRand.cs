using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common.NetCode
{
    public class SyncedRand : ModSystem
    {
        public static int seed;
        public static UnifiedRandom rand;
        public override void OnWorldLoad() => SetRand();
        public override void PreUpdateTime() => SetRand();
        public void SetRand()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                seed = Main.rand.Next(int.MaxValue);
            }
            rand = new UnifiedRandom(seed);
        }
    }
}
