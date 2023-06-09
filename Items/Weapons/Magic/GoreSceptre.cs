using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Weapons.Magic
{
    public class GoreSceptre : ModItem
    {
        public override void SetStaticDefaults()
        {

            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 3;
            Item.useTime = 45;
            Item.DamageType = DamageClass.Magic;
            Item.useAnimation = 45;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Crimson.HeadGoreSceptre>();
            Item.shootSpeed = 14;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 35).AddTile(TileID.Anvils).Register();
        }
    }
}