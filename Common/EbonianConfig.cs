using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace EbonianMod.Common
{
    public class EbonianClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [ReloadRequired, DefaultValue(1), Increment(0.1f), DrawTicks]
        public float BowPullVolume { get; set; } = 1f;
    }
}
