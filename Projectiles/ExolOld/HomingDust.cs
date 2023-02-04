using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics;

namespace EbonianMod.Projectiles.ExolOld
{
    public class ExolDecoyFire : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flame");

        }

        public override void SetDefaults()
        {

            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[1] += 0.01f;
            Projectile.scale = Projectile.ai[1];

            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 3 * Main.projFrames[Projectile.type])
            {
                Projectile.Kill();
                return;
            }
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.hide = true;
                }
            }

            Projectile.alpha -= 63;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.Damage();

            int dusts = 15;
            Vector2 velocityyyy = Vector2.Zero;
            Helper.DustExplosion(Projectile.Center, Projectile.Size, true, Color.OrangeRed);
            /*Dust dust3 = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, velocityyyy, 150, Color.White, 1.5f);
            dust3.noGravity = true;
            dust3.scale = 3.5f;
            for (int i = 0; i < dusts; i++)
            {
                if (Main.rand.NextBool(3))
                {
                    float speed = 6f;
                    Vector2 velocity = new Vector2(0, -speed * Main.rand.NextFloat(0.5f, 1.2f)).RotatedBy(MathHelper.ToRadians(360f / i * dusts + Main.rand.NextFloat(-50f, 50f)));
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center, 6, velocity, 150, Color.White, 1.5f);
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, velocity, 150, Color.White, 1.5f);
                    dust1.noGravity = true;
                    dust2.noGravity = true;
                    dust1.scale = 2.5f;
                    dust2.scale = 1.5f;
                }
            }*/
        }
    }
    public class HomingDust : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiteful Flame");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 45;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            default(FlameLashDrawer).Draw(Main.projectile[Projectile.whoAmI]);
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;

            Projectile.alpha = 255;
            Projectile.timeLeft = 70;
        }
        public int KillTimer = 0;

        public override void Kill(int timeLeft)
        {
            int damage = ((Main.expertMode) ? 12 : 20);
            Projectile projectile69 = Main.projectile[Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ExolDecoyFire>(), damage, 0, 0, 0)];
            projectile69.friendly = false;
            projectile69.hostile = true;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (++KillTimer >= 10 && Projectile.timeLeft >= 20)
            {

                if (Projectile.localAI[0] == 0)
                {
                    AdjustMagnitude(ref Projectile.velocity);
                    Projectile.localAI[0] = 1f;
                }
                Vector2 move = Vector2.Zero;
                float distance = 8000f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.player[k].active && !Main.player[k].dead)
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
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 10f)
            {
                vector *= 10f / magnitude;
            }
        }
    }
}