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
    public class XExplosionInvis : ModProjectile
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
            Projectile.extraUpdates = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            //EbonianMod.affectedByInvisibleMaskCache.Add(() =>
            {
                float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
                Texture2D explosion = ExtraTextures.crosslight;
                Texture2D ring = ExtraTextures.crosslight;
                Main.spriteBatch.Reload(BlendState.Additive);
                for (int i = 0; i < 3; i++)
                {
                    Main.spriteBatch.Draw(explosion, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha, Projectile.rotation, explosion.Size() / 2, Projectile.ai[0] * 0.9f * 2, SpriteEffects.None, 0);
                    //Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha, Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 0.95f * 2, SpriteEffects.None, 0);
                }
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }//);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), Projectile.Center);
            //Projectile.NewProjectile(null, Projectile.Center, new Vector2(5.5f, 5.5f).RotatedByRandom(MathHelper.Pi), ProjectileType<XExplosionInvisMask>(), 0, 0, ai2: Projectile.whoAmI);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(15, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.025f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
    public class XExplosionInvisMask : ModProjectile
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
        public override bool PreDraw(ref Color lightColor)
        {
            EbonianMod.invisibleMaskCache.Add(() =>
            {
                Texture2D explosion = ExtraTextures.explosion;
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(explosion, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, explosion.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            });
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;

            Projectile proj = Main.projectile[(int)Projectile.ai[2]];
            if (proj.active && proj.whoAmI == Projectile.ai[2])
            {
                //Projectile.Center = Vector2.Lerp(Projectile.Center, proj.Center, 0.1f);
            }
            else Projectile.Kill();
        }
    }
}
