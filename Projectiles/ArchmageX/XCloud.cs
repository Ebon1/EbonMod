﻿using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XCloud : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.height = 120;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 340;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("flare");
            float alpha = Projectile.ai[2];
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Indigo, Color.MediumSlateBlue * 0.7f, alpha) * alpha * 0.75f, Main.GameUpdateCount * 0.04f, tex.Size() / 2, alpha * 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.MediumSlateBlue * 0.7f, Color.Indigo, alpha) * alpha * 0.75f, -Main.GameUpdateCount * 0.04f, tex.Size() / 2, alpha * 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
            for (int i = 0; i < 10; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(7, 2);
                float s = Main.rand.NextFloat(2, 3);
                //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White * 0.15f, Scale: s);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), vel, 0, Color.White * 0.15f, Scale: s);
                s = Main.rand.NextFloat(1, 2);
                //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White, Scale: s * 1.1f);
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDust>(), vel, 0, Color.White, Scale: s);

                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
            }
        }
        Vector2 savedDir, savedP;
        public override void AI()
        {
            Vector2 vel = Main.rand.NextVector2Circular(5, 2);
            float s = Main.rand.NextFloat(1.5f, 2.25f);
            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White * 0.15f, Scale: s * 1.1f);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), vel, 0, Color.White * 0.15f, Scale: s);

            if (Projectile.ai[0] > 40)
            {
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.2f);
                if (Projectile.ai[0] % 4 == 0 && Projectile.ai[0] < 70)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(150, 150) * Main.rand.NextFloat(1, 2);
                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, Projectile.Center) * Main.rand.NextFloat(1, 2), 0, Color.Lerp(Color.Purple, Color.Indigo, Projectile.ai[2]), Scale: Main.rand.NextFloat(0.15f, .4f)).customData = Projectile.Center;
                }
            }
            else
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.2f);

            if (Projectile.timeLeft % 5 == 0)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 4), ModContent.ProjectileType<XCloudVFXExtra>(), 0, 0);
            }

            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
            if (Projectile.timeLeft <= 320)
                Projectile.ai[0]++;
            if (Projectile.ai[0] == 40)
            {
                savedDir = new Vector2(Main.rand.NextFloat(-1, 1), 1);
                Projectile.NewProjectile(null, Projectile.Center, savedDir, ModContent.ProjectileType<XTelegraphLine>(), 0, 0);
            }
            if (Projectile.ai[0] > 75)
            {
                Projectile.NewProjectile(null, Projectile.Center, savedDir, ModContent.ProjectileType<XLightningBolt>(), 20, 0);
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                Projectile.ai[0] = 0;
            }
        }
    }
    public class XCloudVFXExtra : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 5;
            Projectile.width = 5;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(10000);
        }
        public override void AI()
        {
            UnifiedRandom rand = new UnifiedRandom(seed);
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rand.NextFloat(1.5f, 2.5f)) * (rand.NextFloatDirection() > 0 ? 1 : -1));
            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), Vector2.Zero, Scale: Projectile.timeLeft * 0.015f);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Vector2.Zero, Scale: Projectile.timeLeft * 0.015f);
        }
    }
}
