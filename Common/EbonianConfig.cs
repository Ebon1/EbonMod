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
    public class EbonianServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Slider, DefaultValue(3600 * 12), Increment(1), Range(300, 3600 * 12)]
        public int XareusDelay { get; set; } = 3600 * 12;
    }
}
