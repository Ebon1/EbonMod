using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Common.Systems;
using Terraria.GameContent;

namespace EbonianMod.Projectiles.Cecitior
{
    internal class CecitiorClawSlash : ModProjectile
    {
        public override string Texture => Helper.Empty;
        const int max = 40;
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = max;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)), 29, ref a) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + -Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)), 29, ref a)) && Projectile.scale > 0.5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle a = SoundID.DD2_SonicBoomBladeSlash;
            a.PitchVariance = 0.2f;
            a.MaxInstances = 30;
            SoundEngine.PlaySound(a, Projectile.Center);
            SoundEngine.PlaySound(EbonianSounds.clawSwipe, Projectile.Center);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, max, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * (Projectile.scale + 0.5f), 0, 1);
            Projectile.ai[0] += 0.17f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("laser4");
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, Main.Rasterizer);
            Vector2 pos3 = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 200;
            for (int i = 0; i < MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(0, 4, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), MathHelper.Lerp(0, 1.5f, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos3 - Main.screenPosition, null, Color.Black, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                pos3 += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Vector2 pos4 = Projectile.Center;
            for (int i = 0; i < MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(4, 0, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), MathHelper.Lerp(1.5f, 0, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos4 - Main.screenPosition, null, Color.Black, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                pos4 += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Main.spriteBatch.Reload(BlendState.Additive);
            Vector2 pos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 200;
            for (int i = 0; i < MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(0, 4, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), MathHelper.Lerp(0, 1.5f, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Maroon, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White * 0.35f, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                pos += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Vector2 pos2 = Projectile.Center;
            for (int i = 0; i < MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(4, 0, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), MathHelper.Lerp(1.5f, 0, (float)(i / MathHelper.Lerp(0, 200, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos2 - Main.screenPosition, null, Color.Maroon, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, pos2 - Main.screenPosition, null, Color.White * 0.35f, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2.5f, SpriteEffects.None, 0);
                pos2 += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer);
            return false;
        }
    }
}
