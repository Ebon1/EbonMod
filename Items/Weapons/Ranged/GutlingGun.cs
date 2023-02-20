using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class GutlingGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 3;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.reuseDelay = 25;
            Item.shoot = ModContent.ProjectileType<IchorGrenade>();
            Item.shootSpeed = 8f;
            Item.rare = 2;
            Item.useStyle = 5;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<IchorGrenade>();
            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)) * Main.rand.NextFloat(1, 2), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class IchorGrenade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 500;
            Projectile.Size = new(8);
        }
        public override void Kill(int timeLeft)
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.Gold, scaleFactor: 0.05f);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
