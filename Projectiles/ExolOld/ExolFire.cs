using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flame");
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 14;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.timeLeft = 1000;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            if (Main.rand.Next(5) == 0)
                for (int dustNumber = 0; dustNumber < 3; dustNumber++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 6, 0, 0, 0, default(Color), 1f)];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                    dust.velocity.X = Main.rand.NextFloat(-0.5f, 0.5f);
                    dust.velocity.Y = -2f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.65f, 1.25f);
                }
        }
    }
}