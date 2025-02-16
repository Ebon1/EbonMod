using EbonianMod.Dusts;
using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Projectiles.Conglomerate
{
    public class CGhostCeci : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 98;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Vector2 origin = new Vector2(TextureAssets.Projectile[Type].Value.Width / 2, TextureAssets.Projectile[Type].Value.Height / 4);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = InOutCubic.Invoke(1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + origin - Main.screenPosition, new Rectangle(0, Projectile.frame * (int)origin.Y * 2, (int)origin.X * 2, (int)origin.Y * 2), Color.Lerp(Color.Maroon, Color.LawnGreen, Projectile.frame) * mult * mult * 0.3f * Projectile.ai[2], Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            /*if (Projectile.velocity.Length() > 10)
                EbonianMod.blurDrawCache.Add(() =>
                {
                    //for (int i = 0; i < 5; i++)
                    Main.spriteBatch.Draw(ExtraTextures.cone2_blur, Projectile.Center - Main.screenPosition, null, Color.White * Lerp(0, 1, Projectile.velocity.Length() / 20), Projectile.rotation - PiOver2, new Vector2(0, ExtraTextures.cone2.Height / 2), new Vector2(3, 1.5f), SpriteEffects.None, 0);

                    Main.spriteBatch.Draw(ExtraTextures.seamlessNoise3, Projectile.Center - Main.screenPosition, null, Color.White * Lerp(0, 1, Projectile.velocity.Length() / 20) * 0.2f, Main.GameUpdateCount, ExtraTextures.seamlessNoise3.Size() / 2, .3f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(ExtraTextures.seamlessNoise3, Projectile.Center - Main.screenPosition, null, Color.White * Lerp(0, 1, Projectile.velocity.Length() / 20) * 0.2f, Main.GameUpdateCount * 0.3f, ExtraTextures.seamlessNoise3.Size() / 2, .3f, SpriteEffects.None, 0);
                });*/

            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * TextureAssets.Projectile[Type].Value.Height / 2, TextureAssets.Projectile[Type].Value.Width, TextureAssets.Projectile[Type].Value.Height / 2), Color.White * Projectile.ai[2] * 0.2f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.ai[2];
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            //overWiresUI.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(2);
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 100 && Projectile.velocity.Length() > 10)
            {
                for (int i = 0; i < 4; i++)
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<BlurDust>(), Projectile.velocity.X * Main.rand.NextFloat(0.5f, 1), Projectile.velocity.Y * Main.rand.NextFloat(0.5f, 1), newColor: Color.White * Lerp(0, 1, ((Projectile.velocity.Length() - 10f) / 20f)), Scale: Main.rand.NextFloat(0.1f, 0.2f));
            }
            if (Projectile.velocity.Length() > 5 && Projectile.timeLeft < 100)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<LineDustFollowPoint>(), Projectile.velocity.X * Main.rand.NextFloat(0.5f, 1), Projectile.velocity.Y * Main.rand.NextFloat(0.5f, 1), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Projectile.frame) * Lerp(0, 0.3f, ((Projectile.velocity.Length() - 5f) / 40f)), Scale: Main.rand.NextFloat(0.1f, 0.2f));
            if (Projectile.timeLeft > 40)
                Projectile.ai[2] = Lerp(Projectile.ai[2], 1, 0.05f);
            if (Projectile.timeLeft < 40)
                Projectile.ai[2] = Lerp(Projectile.ai[2], 0, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft > 140 && Projectile.velocity.Length() > 0.05f)
                Projectile.velocity *= 0.8f;
            else if (Projectile.timeLeft < 100 && Projectile.velocity.Length() < 40)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(Projectile.ai[1])) * 1.1f;
                Projectile.ai[1] = Lerp(Projectile.ai[1], 0, 0.05f);
            }
            else
                Projectile.velocity = Projectile.velocity.RotatedBy(ToRadians(Projectile.ai[1]));
        }

    }
}
