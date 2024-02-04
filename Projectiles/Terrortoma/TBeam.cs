using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;
using static EbonianMod.Helper;
using Terraria.ID;

namespace EbonianMod.Projectiles.Terrortoma
{
    public class TBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 165;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.ai[0], 60, ref a) && Projectile.scale > 0.5f;
        }
        bool RunOnce;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                RunOnce = false;
            }
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }
            if (visualOffset == 0)
                visualOffset = Main.rand.NextFloat(0.75f, 1);
            visual1 += 30 * visualOffset;
            visual2 += 35 * visualOffset;

            if (Projectile.rotation != Projectile.oldRot[1])
            {
                float len = TRay.CastLength(Projectile.Center, Projectile.velocity, 2048);
                Vector2 vel = Projectile.velocity;
                vel.Normalize();
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vel * (len), DustID.CursedTorch, Main.rand.NextVector2Circular(15, 15));
                    dust.scale = 2f;
                    dust.noGravity = true;
                }
                Projectile.ai[1] = len;
            }
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], .006f, 0.015f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center), Projectile.ai[2]);
            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], Projectile.ai[1], 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 1);
        }
        float visualOffset;
        float visual1, visual2;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("terrortomaBeam");
            Texture2D tex2 = Helper.GetExtraTexture("oracleBeamv2");
            Texture2D tex3 = Helper.GetExtraTexture("vortex3");
            //Texture2D tex3 = Helper.GetExtraTexture("spark_06");
            Vector2 pos = Projectile.Center;
            Vector2 scale = new Vector2(1f, Projectile.scale);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.ai[0]; i++)
            {
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.LawnGreen, Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                pos += Projectile.rotation.ToRotationVector2();
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            Vector2 pos2 = Projectile.Center;

            int off = 0;
            if (Projectile.ai[0] < 512)
                off = 3;
            else if (Projectile.ai[0] < 1024)
                off = 2;
            else if (Projectile.ai[0] < 1536)
                off = 1;
            for (int i = 0; i < 4 - off; i++)
            {
                int len = 512;
                if (i == 3 && Projectile.ai[0] < 2048 && Projectile.ai[0] - 1536 > 0)
                    len = (int)Projectile.ai[0] - 1536;
                if (i == 2 && Projectile.ai[0] < 1536 && Projectile.ai[0] - 1024 > 0)
                    len = (int)Projectile.ai[0] - 1024;
                if (i == 1 && Projectile.ai[0] < 1024 && Projectile.ai[0] - 512 > 0)
                    len = (int)Projectile.ai[0] - 512;
                if (i == 0 && Projectile.ai[0] < 512)
                    len = (int)Projectile.ai[0];
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual1, 0, len, 512), Color.LawnGreen, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual1, 0, len, 512), Color.White, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual2, 0, len, 512), Color.LawnGreen, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.FlipVertically, 0);
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual2, 0, len, 512), Color.White, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.FlipVertically, 0);
                pos2 += Projectile.rotation.ToRotationVector2() * len;
            }
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Main.GameUpdateCount * 0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White, Main.GameUpdateCount * -0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Main.GameUpdateCount * -0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            float alpha = MathHelper.Lerp(-1f, 1f, (Projectile.ai[0] / Projectile.ai[1]));
            Main.spriteBatch.Draw(tex3, Projectile.Center + Projectile.ai[0] * Projectile.rotation.ToRotationVector2() - Main.screenPosition, null, Color.LawnGreen * alpha, Main.GameUpdateCount * 0.003f, tex3.Size() / 2, scale.Y * 0.25f * alpha, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center + Projectile.ai[0] * Projectile.rotation.ToRotationVector2() - Main.screenPosition, null, Color.White * alpha, Main.GameUpdateCount * -0.03f, tex3.Size() / 2, scale.Y * 0.25f * alpha, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}