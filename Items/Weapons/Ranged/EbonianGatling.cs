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
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 2;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.shoot = ProjectileType<WeakCursedBullet>();
            Item.shootSpeed = 8f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item11;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.useAmmo = AmmoID.Bullet;
            Item.useTurn = false;
            //Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ShadowScale, 20).AddIngredient(ItemID.Minishark).AddTile(TileID.Anvils).Register();
        }
        public override bool? CanAutoReuseItem(Player player)
        {
            return true;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2);
        public override Vector2? HoldoutOffset() => new(-10, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ProjectileType<WeakCursedBullet>();
            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
