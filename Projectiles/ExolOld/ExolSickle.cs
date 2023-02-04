using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Buffs;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolSickle : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lava scythe");
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.timeLeft = 200;
        }
        public override void Kill(int timeLeft)
        {
            for (int num686 = 0; num686 < 30; num686++)
            {
                int num687 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 1.7f);
                Main.dust[num687].noGravity = true;
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, Projectile.velocity.X, Projectile.velocity.Y, 100);
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }
        public override void AI()
        {
            Projectile.rotation += (float)Projectile.direction * 0.8f;
            Projectile.velocity *= 1.06f;
            {
                Lighting.AddLight(Projectile.position, 0.25f, 0, 0.5f);
            }
            if (Main.rand.Next(5) == 0)
                for (int dustNumber = 0; dustNumber < 3; dustNumber++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 6, 0, 0, 0, default(Color), 1f)];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                    dust.velocity.X = Main.rand.NextFloat(-0.5f, 0.5f);
                    dust.velocity.Y = -2f;
                    dust.noGravity = true;
                    dust.scale = 1.25f;
                }
        }
    }
}
