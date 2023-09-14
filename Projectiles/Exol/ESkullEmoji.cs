using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace EbonianMod.Projectiles.Exol
{
    public class ESkullEmoji : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void Kill(int timeLeft)
        {
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage * 2, 0);
            a.hostile = true;
            a.friendly = false;
        }
        public override void AI()
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            if (Projectile.timeLeft < 350)
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.active && projectile.type == Type && projectile.whoAmI != Projectile.whoAmI)
                    {
                        if (projectile.Center.Distance(Projectile.Center) < projectile.width * projectile.scale)
                        {
                            Projectile.velocity += Helper.FromAToB(Projectile.Center, projectile.Center, true, true) * 0.5f;
                        }
                        if (projectile.Center == Projectile.Center)
                        {
                            Projectile.velocity = Main.rand.NextVector2Unit() * 5;
                        }
                    }
                }
            else
                Projectile.velocity *= 0.99f;
            /*if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }*/
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.4f);
            Vector2 move = Vector2.Zero;
            float distance = 5050f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.player[k].active)
                {
                    Vector2 newMove = Main.player[k].Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (++Projectile.ai[0] % 5 == 0 && target && Projectile.timeLeft > 45 && Projectile.timeLeft < 350)
            {
                Projectile.ai[1] = Projectile.velocity.ToRotation() + MathHelper.Pi;
                AdjustMagnitude(ref move);
                Projectile.velocity = (20.5f * Projectile.velocity + move) / 20.5f;
                AdjustMagnitude(ref Projectile.velocity);
            }
            if (Projectile.timeLeft < 45)
            {
                Projectile.velocity *= 0.95f;
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20.5f)
            {
                vector *= 20.5f / magnitude;
            }
        }
    }
}
