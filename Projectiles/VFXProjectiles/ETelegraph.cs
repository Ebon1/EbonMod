using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class ETelegraph : ModProjectile
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
            Projectile.timeLeft = 100;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("Extras2/circle_02");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[1]);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f * Projectile.ai[0], Main.GameUpdateCount * 0.1f, tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * Projectile.ai[0], Main.GameUpdateCount * 0.1f, tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            tex = Helper.GetExtraTexture("Extras2/circle_02");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f * alpha * Projectile.ai[0], Projectile.rotation, tex.Size() / 2, Projectile.ai[1] * Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * alpha * Projectile.ai[0], Projectile.rotation, tex.Size() / 2, Projectile.ai[1] * Projectile.ai[0], SpriteEffects.None, 0);
            tex = Helper.GetExtraTexture("Extras2/flare_01");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.5f * Projectile.ai[0], 0, tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.ai[0], 0, tex.Size() / 2, 1.65f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[1] += 0.05f;
            if (Projectile.ai[1] > 1.5f)
                Projectile.ai[1] = 0;

            float progress = Utils.GetLerpValue(0, 100, Projectile.timeLeft);
            Projectile.ai[0] = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }
    }
}
