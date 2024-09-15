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
            Projectile.timeLeft = 1400;
            Projectile.extraUpdates = 5;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            Vector2 vel = Projectile.velocity;
            vel.SafeNormalize(-Vector2.UnitY);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + vel * 220, 20, ref a);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Projectile.ai[2] >= 0.5f;
        float riftAlpha;
        public override void AI()
        {
            if (Projectile.velocity == Vector2.Zero) Projectile.velocity = -Vector2.UnitY;
            Projectile.velocity.SafeNormalize(-Vector2.UnitY);

            if (Projectile.timeLeft == 1398)
                Projectile.Center = Helper.TRay.Cast(Projectile.Center, -Projectile.velocity, 1000);
            if (Projectile.timeLeft == 1399)
                SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, Projectile.Center);


            if (Projectile.localAI[1] >= .99f)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    SoundEngine.PlaySound(EbonianSounds.eruption.WithPitchOffset(0.3f), Projectile.Center);
                }
                Projectile.ai[2] += 0.025f;
                Projectile.ai[2] += Projectile.ai[2];
                Projectile.ai[2] = MathHelper.Clamp(Projectile.ai[2], 0, 1f);
            }
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], 1, 0.015f);

            if (Projectile.timeLeft > 200)
                riftAlpha = MathHelper.Lerp(0, 1, Projectile.localAI[1]);
            else
                riftAlpha = MathHelper.Lerp(riftAlpha, 0, 0.015f);

            if (Projectile.timeLeft % 2 == 0)
                if (Projectile.localAI[1] >= 0.25f && Projectile.timeLeft > 200)
                {
                    if (Projectile.localAI[1] >= 0.99f)
                    {
                        if (Main.rand.NextBool(Projectile.extraUpdates) && Projectile.ai[2] > 0.5f)
                            Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? ModContent.DustType<SparkleDust>() : ModContent.DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(6, 15) * Projectile.ai[2], 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                        else
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(0.1f, 15 * Projectile.ai[2]), Scale: Main.rand.NextFloat(0.5f, 0.7f));
                    }
                    if (Main.rand.NextBool(Projectile.localAI[1] < 0.5f ? 10 : 5))
                        Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? ModContent.DustType<SparkleDust>() : ModContent.DustType<LineDustFollowPoint>(), Projectile.velocity.RotatedByRandom(MathHelper.Pi) * Main.rand.NextFloat(0.1f, 4), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 1397) return false;
            Texture2D tex = Helper.GetExtraTexture("rune_alt");

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);

            Vector2 scale = new Vector2(1f, 0.25f);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-Main.GameUpdateCount * 0.035f);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(Color.DarkOrchid.ToVector4());
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * riftAlpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, riftAlpha / 4, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
