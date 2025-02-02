using EbonianMod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Tiles.Furniture;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Tiles
{
    public class WoolWoodI : ModItem
    {
        public override void SetDefaults() => Item.DefaultToPlaceableTile(TileType<WoolWood>());
        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemType<Wool>(), 1).AddIngredient(ItemID.Wood, 1).Register();
            CreateRecipe(1).AddIngredient(ItemType<WoolWoodWallI>(), 4).AddTile(TileID.WorkBenches).Register();
            CreateRecipe(1).AddIngredient(ItemType<WoolWoodPlatformI>(), 2).Register();
        }
    }
    public class WoolWoodWallI : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<WoolWoodWall>());
        }
        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ItemType<WoolWoodI>(), 1).AddTile(TileID.WorkBenches).Register();
        }
    }
    public class WoolWoodPlatformI : ModItem
    {
        public override void SetDefaults() => Item.DefaultToPlaceableTile(TileType<WoolWoodPlatform>());
        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ItemType<WoolWoodI>(), 1).Register();
        }
    }
}

