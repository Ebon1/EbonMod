using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class EyeVFX : ModProjectile
    {
        //public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Projectile.ai[1] = 1;
        }
        public override void PostAI()
        {
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = 1;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * Projectile.ai[1], SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
