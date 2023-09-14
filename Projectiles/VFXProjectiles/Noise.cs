using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class Noise : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;
            Texture2D tex = Helper.GetExtraTexture("seamlessNoise2");
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[2] > 0)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[0] * Projectile.ai[2] * 0.5f, Main.GameUpdateCount * 0.1f * Projectile.ai[0], tex.Size() / 2, Projectile.ai[2] * 0.5f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[0], -Main.GameUpdateCount * 0.1f * Projectile.ai[0], tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            
            if (Projectile.ai[2] < 1 && Projectile.timeLeft > 100)
                Projectile.ai[2] += 0.01f;
            if (Projectile.timeLeft < 50 && Projectile.ai[2] > 0)
                Projectile.ai[2] -= 0.1f;
            if (Projectile.timeLeft < 50 && Projectile.ai[2] < 0.1f)
                Projectile.ai[2] = 0;
            float progress = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
            Projectile.ai[0] = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
    }
    public class NoiseOverlay : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("Extras2/light_03");
            Texture2D OrigTex = Helper.GetExtraTexture("seamlessNoise2");
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[2] > 0) { 
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed  * 0.5f* Projectile.ai[2] * Projectile.ai[0], Main.GameUpdateCount * 0.1f, OrigTex.Size() / 2, Projectile.ai[2] * 1.3f * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f* Projectile.ai[2] * Projectile.ai[0], Main.GameUpdateCount * -0.1f, OrigTex.Size() / 2, Projectile.ai[2] * 1.3f * 0.5f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[0], -Main.GameUpdateCount * 0.1f * Projectile.ai[0], tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            
            if (Projectile.ai[2] < 1 && Projectile.timeLeft > 100)
                Projectile.ai[2] += 0.01f;
            if (Projectile.timeLeft < 50 && Projectile.ai[2] > 0)
                Projectile.ai[2] -= 0.1f;
            if (Projectile.timeLeft < 50 && Projectile.ai[2] < 0.1f)
                Projectile.ai[2] = 0;
            float progress = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
            Projectile.ai[0] = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
    }
}
