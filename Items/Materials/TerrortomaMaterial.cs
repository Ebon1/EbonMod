using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Materials
{
    public class TerrortomaMaterial : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SoulofNight);
        }
    }
}
