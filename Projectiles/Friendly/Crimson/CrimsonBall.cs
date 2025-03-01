using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Conglomerate;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class CrimsonBall : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Cecitior/CecitiorBombThing";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 2;
        }
        public override void OnKill(int timeLeft)
        {
            CameraSystem.ScreenShakeAmount = 5;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0);
            for (int i = 0; i < 5; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 5 + Projectile.ai[2] * 2) + Main.rand.NextFloat(Pi);
                Projectile projectile = Projectile.NewProjectileDirect(null, Projectile.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<CecitiorTeethFriendly>(), Projectile.damage/3, 0, 0);
                projectile.usesLocalNPCImmunity = true;
                projectile.tileCollide = false;
                projectile.velocity /= 50;
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.Reload(BlendState.Additive);

            float fadeMult = 1f / Projectile.oldPos.Length;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                sb.Draw(Helper.GetExtraTexture("explosion"), Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.LawnGreen, 0.4f) * mult * 0.6f, Projectile.rotation, Helper.GetExtraTexture("explosion").Size() / 2, (32 / (float)Helper.GetExtraTexture("explosion").Size().Length() * 2 * (1 + Projectile.ai[1])) * mult, SpriteEffects.None, 0);
            }

            sb.Draw(Helper.GetExtraTexture("explosion"), Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.LawnGreen, 0.4f), Projectile.rotation, Helper.GetExtraTexture("explosion").Size() / 2, 32 / (float)Helper.GetExtraTexture("explosion").Size().Length() * 2.2f * (1 + Projectile.ai[1]), SpriteEffects.None, 0);
            sb.Draw(Helper.GetExtraTexture("Extras2/circle_04"), Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.LawnGreen, 0.4f), Projectile.rotation, Helper.GetExtraTexture("Extras2/circle_04").Size() / 2, 32 / (float)Helper.GetExtraTexture("Extras2/circle_04").Size().Length() * 1.5f * (1 + Projectile.ai[1]), SpriteEffects.None, 0);

            sb.Reload(BlendState.AlphaBlend);
            return true;
        }
    }
}
