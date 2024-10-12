using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Friendly.Corruption;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Conglomerate
{
    public class CIchorBomb : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/swirlyNoise";
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(64);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = 2;
        }
        public override void OnKill(int timeLeft)
        {
            EbonianSystem.ScreenShakeAmount = 5;
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CFlareExplosion>(), 0, 0);
            for (int i = 0; i < 3 + Projectile.ai[2] * 2; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 3 + Projectile.ai[2] * 2);
                Projectile a = Projectile.NewProjectileDirect(null, Projectile.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<CecitiorTeeth>(), 30, 0, 0);
                a.friendly = false;
                a.hostile = true;
            }
            if (Projectile.ai[2] < 2)
                Projectile.NewProjectile(null, Projectile.Center, new Vector2(Projectile.velocity.X, -Projectile.velocity.Y * 0.9f), Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1] - 0.15f, Projectile.ai[2] + 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            EbonianMod.spherize.CurrentTechnique.Passes[0].Apply();
            EbonianMod.spherize.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 20);
            EbonianMod.spherize.Parameters["alpha"].SetValue(1);
            EbonianMod.spherize.Parameters["noShadow"].SetValue(true);

            EbonianMod.spherize.Parameters["lightDirection"].SetValue(Projectile.velocity);
            EbonianMod.spherize.Parameters["insideColor"].SetValue(Color.Lerp(Color.Maroon, Color.LawnGreen, 0.5f).ToVector4());
            EbonianMod.spherize.Parameters["outsideColor"].SetValue(new Vector4(0, 0, 0, 0));

            sb.Draw(Helper.GetExtraTexture("swirlyNoise"), Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), Helper.GetExtraTexture("swirlyNoise").Size() / 2, (128f / Helper.GetExtraTexture("swirlyNoise").Size().Length()) * (1 + Projectile.ai[1]), SpriteEffects.None, 0);
            sb.ApplySaved();
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            EbonianMod.spherize.CurrentTechnique.Passes[0].Apply();
            EbonianMod.spherize.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 15f);
            for (int i = 0; i < 2; i++)
                sb.Draw(Helper.GetExtraTexture("seamlessNoise"), Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), Helper.GetExtraTexture("seamlessNoise").Size() / 2, (128f / Helper.GetExtraTexture("seamlessNoise").Size().Length()) * (1 + Projectile.ai[1]), SpriteEffects.None, 0);
            sb.ApplySaved();

            sb.Reload(BlendState.Additive);
            sb.Draw(Helper.GetExtraTexture("explosion"), Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.LawnGreen, 0.4f), Projectile.rotation, Helper.GetExtraTexture("explosion").Size() / 2, 128f / (float)Helper.GetExtraTexture("explosion").Size().Length() * 2 * (1 + Projectile.ai[1]), SpriteEffects.None, 0);

            sb.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}
