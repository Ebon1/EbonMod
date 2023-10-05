using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class GreenShockwave : ModProjectile
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
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Green * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
    public class CursedFlameExplosion : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 200;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex2 = Helper.GetExtraTexture("vortex3");
            Texture2D tex3 = Helper.GetExtraTexture("Extras2/light_02");
            float alpha = MathHelper.Lerp(2, 0, Projectile.ai[0]);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f * alpha, Projectile.rotation, tex3.Size() / 2, Projectile.ai[0] * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, Projectile.rotation, tex3.Size() / 2, Projectile.ai[0] * 0.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int j = 0; j < 30; j++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 5), Scale: Main.rand.NextFloat(2));
            }
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Cursed);
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
