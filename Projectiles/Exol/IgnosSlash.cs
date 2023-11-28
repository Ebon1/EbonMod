using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;

namespace EbonianMod.Projectiles.Exol
{
    internal class IgnosSlash : ModProjectile
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
            return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0], 0, 1f)), 29, ref a) || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + -Projectile.velocity.SafeNormalize(Vector2.UnitX) * MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)), 29, ref a)) && Projectile.scale > 0.5f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundStyle a = SoundID.DD2_SonicBoomBladeSlash;
            a.PitchVariance = 0.2f;
            a.MaxInstances = 30;
            SoundEngine.PlaySound(a, Projectile.Center);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, max, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * (Projectile.scale + 0.5f), 0, 1);
            Projectile.ai[0] += 0.1f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("laser4");
            Vector2 pos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 1080;
            for (int i = 0; i < MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0], 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(0, 4, (float)(i / MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), MathHelper.Lerp(0, 1.5f, (float)(i / MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0], 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.OrangeRed, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2, SpriteEffects.None, 0);
                pos += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Vector2 pos2 = Projectile.Center;
            for (int i = 0; i < MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)); i++)
            {
                Vector2 scale = new Vector2(0.5f, 0.5f * Projectile.scale * MathHelper.Lerp(MathHelper.Lerp(4, 0, (float)(i / MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), MathHelper.Lerp(1.5f, 0, (float)(i / MathHelper.Lerp(0, 1080, MathHelper.Clamp(Projectile.ai[0] - 1, 0, 1f)))), Projectile.scale));
                Main.spriteBatch.Draw(tex, pos2 - Main.screenPosition, null, Color.OrangeRed, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, pos2 - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tex.Height / 2), scale * 2, SpriteEffects.None, 0);
                pos2 += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
    internal class IgnosSlashTelegraph : ModProjectile
    {
        public override string Texture => Helper.Empty;
        const int max = 60;
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
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            float progress = Utils.GetLerpValue(0, max, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
            if (Projectile.timeLeft == 20)
                Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity, ModContent.ProjectileType<IgnosSlash>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("laser4");
            Vector2 pos = Projectile.Center;
            Vector2 scale = new Vector2(1, 0.5f * Projectile.scale);
            for (int i = 0; i < 1080; i++)
            {
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                pos += Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Vector2 pos2 = Projectile.Center + -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 1080; i++)
            {
                Main.spriteBatch.Draw(tex, pos2 - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                pos2 += -Projectile.velocity.SafeNormalize(Vector2.UnitX);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
