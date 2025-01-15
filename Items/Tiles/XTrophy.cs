using EbonianMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Tiles
{
    public class XTrophy : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<XTrophyTile>());

            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 1);
        }
    }
}
