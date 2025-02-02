using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Misc
{
    public class Shears : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.StylistKilLaKillScissorsIWish);
            Item.damage = 4;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.IronBar, 2).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.LeadBar, 2).AddTile(TileID.Anvils).Register();
        }
    }
}
