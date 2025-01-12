using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Items.Accessories
{
    public class XTentacleAcc : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
            Item.expert = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax += 60;
            player.GetModPlayer<EbonianPlayer>().xTent = true;
        }
    }
}
