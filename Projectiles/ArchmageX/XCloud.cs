using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
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
            Projectile.timeLeft = 386;
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
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(.5f, .5f);
                float s = Main.rand.NextFloat(3, 5);
                //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White * 0.15f, Scale: s);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), vel, 0, Color.White * 0.3f, Scale: s).customData = 1;
                //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White, Scale: s * 1.1f);
                //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDust>(), vel, 0, Color.White, Scale: s);

                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
            }
        }
        Vector2 savedDir, savedP;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Purple);
            Vector2 vel = Main.rand.NextVector2Circular(2, 2);
            float s = Main.rand.NextFloat(1.5f, 4.25f);
            if (Projectile.timeLeft > 30 && Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), vel, 0, Color.White * 0.1f, Scale: s).customData = 1;
            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), vel, 0, Color.White * 0.15f, Scale: s * 1.1f);
            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), vel, 0, Color.White * 0.15f, Scale: s);

            if (Projectile.ai[0] > 40)
            {
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.2f);
                if (Projectile.ai[0] % 6 == 0 && Projectile.ai[0] < 70)
                {
                    Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(150, 150) * Main.rand.NextFloat(1, 2);
                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, Projectile.Center) * Main.rand.NextFloat(1, 2), 0, Color.Lerp(Color.Purple, Color.Indigo, Projectile.ai[2]), Scale: Main.rand.NextFloat(0.15f, .4f)).customData = Projectile.Center;
                }
            }
            else
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.2f);

            if (Projectile.timeLeft % 8 == 0)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 4), ModContent.ProjectileType<XCloudVFXExtra>(), 0, 0);
            }

            if (Projectile.timeLeft % 6 == 0)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
            if (Projectile.timeLeft <= 345)
                Projectile.ai[0]++;
            if (Projectile.ai[0] == 20)
            {
                savedDir = Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center);
                if (Projectile.ai[1] != 1)
                    Projectile.NewProjectile(null, Projectile.Center, savedDir, ModContent.ProjectileType<XTelegraphLine>(), 0, 0);
                else
                    Projectile.NewProjectile(null, Projectile.Center, savedDir, ModContent.ProjectileType<SheepeningOrb>(), 20, 0, Projectile.owner);
            }
            if (Projectile.ai[0] > (Projectile.ai[1] != 1 ? 55 : 80))
            {
                if (Projectile.ai[1] != 1)
                    Projectile.NewProjectile(null, Projectile.Center, savedDir, ModContent.ProjectileType<XLightningBolt>(), 20, 0);
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                Projectile.ai[0] = 0;
            }

            /*if (Projectile.ai[1] != 0)
            {
                if (Projectile.timeLeft == 260)
                {
                    savedP = Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center);
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(null, Projectile.Center, savedDir.RotatedBy(MathHelper.ToRadians(i * 30)), ModContent.ProjectileType<XTelegraphLine>(), 0, 0);
                    }
                }
                if (Projectile.timeLeft == 225)
                {
                    Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(null, Projectile.Center, savedDir.RotatedBy(MathHelper.ToRadians(i * 30)), ModContent.ProjectileType<XLightningBolt>(), 20, 0);
                    }
                }
            }*/
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
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rand.NextFloat(1.5f, 3.5f)) * (rand.NextFloatDirection() > 0 ? 1 : -1));
            //Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDustDark>(), Vector2.Zero, Scale: Projectile.timeLeft * 0.015f);
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Projectile.velocity * MathHelper.Lerp(0.05f, 0, (float)Projectile.timeLeft / 60), 0, Color.White * 0.7f, Projectile.timeLeft * 0.015f).customData = 1;
        }
    }
}
