using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class EbonianGatling : ModItem
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 2;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.shoot = ModContent.ProjectileType<WeakCursedBullet>();
            Item.shootSpeed = 8f;
            Item.rare = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item11;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTurn = false;
            //Item.autoReuse = true;
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2);
        public override Vector2? HoldoutOffset() => new(-10, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<WeakCursedBullet>();
            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
