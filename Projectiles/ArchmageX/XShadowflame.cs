using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using EbonianMod.Dusts;
using Terraria.Audio;
using EbonianMod.Common.Systems;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XShadowflame : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.height = 5;
            Projectile.width = 5;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 870;
            Projectile.extraUpdates = 5;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            Vector2 vel = Projectile.velocity;
            vel.SafeNormalize(-Vector2.UnitY);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + vel * 200, 20, ref a);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Projectile.ai[2] >= 1f;
        float riftAlpha;
        public override void AI()
        {
            if (Projectile.velocity == Vector2.Zero) Projectile.velocity = -Vector2.UnitY;
            Projectile.velocity.SafeNormalize(-Vector2.UnitY);

            if (Projectile.timeLeft == 868)
                Projectile.Center = Helper.TRay.Cast(Projectile.Center, -Projectile.velocity, 29 * 16);
            if (Projectile.timeLeft == 869)
                SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, Projectile.Center);


            if (Projectile.localAI[1] >= .99f)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    SoundEngine.PlaySound(EbonianSounds.eruption.WithPitchOffset(0.7f), Projectile.Center);
                }
                Projectile.ai[2] += 0.025f;
                Projectile.ai[2] += Projectile.ai[2];
                Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2], 0, 1f);
            }
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], 1, 0.0175f);

            if (Projectile.timeLeft > 100)
                riftAlpha = MathHelper.Lerp(0, 1, Projectile.localAI[1]);
            else
                riftAlpha = MathHelper.Lerp(riftAlpha, 0, 0.015f);

            if (Projectile.timeLeft % 3 == 0)
                if (Projectile.localAI[1] >= 0.1f && Projectile.timeLeft > 100)
                {
                    if (Projectile.localAI[1] >= 0.99f)
                    {
                        if (Main.rand.NextBool(Projectile.extraUpdates) && Projectile.ai[2] > 0.5f)
                            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 1), DustType<SparkleDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(6, 15) * Projectile.ai[2], 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        else
                            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10, 1), DustType<XGoopDust2>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(.1f, 15 * Projectile.ai[2]), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                    }
                    if (Main.rand.NextBool(Projectile.localAI[1] < 0.5f ? 10 : 5))
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 1), DustType<SparkleDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1f, 6), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 867) return false;
            Texture2D tex = Helper.GetExtraTexture("rune_alt");
            Texture2D bloom = Helper.GetExtraTexture("rune_alt_bloom");

            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, EbonianMod.SpriteRotation, Main.GameViewMatrix.TransformationMatrix);

            Vector2 scale = new Vector2(0.65f, 0.25f) * 1.6f;
            float alpha = riftAlpha;
            float i = 1;
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(new Vector2(scale.X * 0.75f, scale.Y / alpha.Safe() * 0.5f));
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-Main.GameUpdateCount * 0.035f * alpha);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(new Color(60, 2, 113).ToVector4() * alpha * alpha * 0.8f);
            EbonianMod.SpriteRotation.Parameters["hasPerspective"].SetValue(false);
            Main.spriteBatch.Draw(tex, Projectile.Center - Vector2.UnitY * riftAlpha * i * 2 * -Projectile.velocity.Y - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, riftAlpha, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bloom, Projectile.Center - Vector2.UnitY * riftAlpha * i * 2 * -Projectile.velocity.Y - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, riftAlpha, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(effect: null);

            Main.spriteBatch.ApplySaved();
            return false;
        }
    }
}
