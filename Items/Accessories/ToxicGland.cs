/*using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Accessories
{
    internal class ToxicGland : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Gland");
            Tooltip.SetDefault("Summons Ebon flies every 5 seconds up to 6 flies\nShoots out a blossom of cursed flames on hit.");
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            EbonianPlayer mp = player.GetModPlayer<EbonianPlayer>();
            mp.ToxicGland = true;
        }
    }
}*/