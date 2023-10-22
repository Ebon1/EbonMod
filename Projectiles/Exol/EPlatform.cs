using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace EbonianMod.Projectiles.Exol
{
    public class EPlatform : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1] = Projectile.Center.X;
                Projectile.ai[2] = Projectile.Center.Y;
            }
            else
            {
                float value = MathHelper.Lerp(0, 1, Projectile.ai[0] / 120);
                if (Projectile.velocity == Vector2.Zero)
                    Projectile.Center = new Vector2(Projectile.ai[1], Projectile.ai[2]) + Main.rand.NextVector2Circular(3 * value, 3 * value);
            }
            if (Projectile.ai[0] < 120)
                Projectile.timeLeft = 2;
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] > 0)
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), Projectile.damage * 2, 0);
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage * 2, 0);
            a.hostile = true;
            a.friendly = false;
        }
    }
}
