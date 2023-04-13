using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EbonianMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class ExolWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Wings");
            Tooltip.SetDefault("Allows flight and slow fall");
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(145, 9f, 1.5f);
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = 10000;
            Item.rare = 4;
            Item.accessory = true;
            Item.expert = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override void UpdateVanity(Player player)
        {
            Lighting.AddLight(player.Center, TorchID.Torch);
        }
        public override void UpdateEquip(Player player)
        {
            Lighting.AddLight(player.Center, TorchID.Torch);
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.95f;
            ascentWhenRising = 0.09f;
            maxCanAscendMultiplier = 0.9f;
            maxAscentMultiplier = 1.9f;
            constantAscend = 0.085f;
        }
    }
}