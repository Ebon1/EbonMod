using EbonianMod.Common.Systems;
using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XExplosion : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.height = 200;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 200;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            float alpha2 = MathHelper.Lerp(0.5f, 0, Projectile.ai[0]);
            Texture2D ring = Helper.GetExtraTexture("ring");
            Texture2D explosion = Helper.GetExtraTexture("explosion");
            Texture2D flameEye2 = Helper.GetExtraTexture("flameEye2");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(explosion, Projectile.Center - Main.screenPosition, null, Color.DarkOrchid * alpha2 * 2, Projectile.rotation, explosion.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(flameEye2, Projectile.Center - Main.screenPosition, null, Color.Magenta * alpha2 * 2, Projectile.rotation, flameEye2.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.DarkOrchid * alpha, Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.DarkOrchid * alpha * 0.5f, Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(15, 15), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150 * Projectile.ai[0], 150 * Projectile.ai[0]), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
            Projectile.ai[0] += 0.075f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
