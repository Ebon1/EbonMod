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
    public class HeatBlastVFX : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 300;
            Projectile.width = 300;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Projectile.ai[1] = 1;
        }
        public override void PostAI()
        {
            if (Projectile.ai[1] == 1)
                Projectile.damage = 0;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = Helper.GetExtraTexture("Extras2/light_01");
            Texture2D tex2 = Helper.GetExtraTexture("Extras2/scorch_03");
            Texture2D tex3 = Helper.GetExtraTexture("circlething");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            Vector2 scale = new Vector2(0.25f, 1);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-Main.GameUpdateCount * 0.003f);
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(Color.OrangeRed.ToVector4() * alpha);
            for (int i = 0; i < 5; i++)
            {
                float s = Projectile.ai[0] * (i * 2) * 0.2f;
                Main.spriteBatch.Draw(tex1, Projectile.Center + Projectile.velocity * i * 3 * (10 * s) - Main.screenPosition, null, Color.White * 0.4f, Projectile.velocity.ToRotation(), tex1.Size() / 2, s * 2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex2, Projectile.Center + Projectile.velocity * i * 4 * (10 * s) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex2.Size() / 2, s * 1.5f * 2, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex3, Projectile.Center + Projectile.velocity * i * 5 * (10 * s) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex3.Size() / 2, s * 0.9f * 2, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 2f)
                Projectile.Kill();
        }
    }
}
