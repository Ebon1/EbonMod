using EbonianMod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace EbonianMod.Items.Tiles
{
    public class EbonHiveI : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.rare = 0;
            Item.useTurn = true;
            Item.rare = 0;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.rare = 2;
            Item.useTime = 10;
            Item.useStyle = 1;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<EbonHiveBlock>();
        }
    }
}
