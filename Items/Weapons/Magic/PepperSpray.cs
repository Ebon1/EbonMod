﻿using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace EbonianMod.Items.Weapons.Magic
{
    public class PepperSpray : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 1;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 5;
            Item.useAnimation = 10;
            Item.useStyle = 5;
            Item.value = Item.buyPrice(0, 1);
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 0;
            Item.rare = 2;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PepperSprayProjectile>();
            Item.shootSpeed = 11f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            position.Y = position.Y - 12;
            position.X = position.X + 2;
            Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(30));
            if (Main.rand.NextBool(3))
                Dust.NewDustPerfect(position, DustID.YellowStarDust, perturbedSpeed.RotatedByRandom(0.2f));
            Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);

            return false;
        }
    }
    public class PepperSprayProjectile : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;

            Projectile.timeLeft = 55;
            Projectile.scale = 1f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Helper.DustExplosion(Projectile.Center, new Vector2(1), 2, new Color(210, 183, 82), false, false, 0, 1, Projectile.velocity);
        }
        public override void AI()
        {
            Projectile.velocity *= 0.95f;
        }
    }
}
