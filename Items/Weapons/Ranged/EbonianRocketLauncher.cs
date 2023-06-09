using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class EbonianRocketLauncher : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 40;
            Item.height = 40;
            Item.reuseDelay = 25;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = 8;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<EbonianRocket>();
            Item.shootSpeed = 14;
            Item.useAmmo = AmmoID.Rocket;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<EbonianRocket>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}