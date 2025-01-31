using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Enemy.Corruption
{
    public class RegorgerTelegraph : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = ExtraTextures.laser4;
                Vector2 pos = Projectile.Center;
                Vector2 scale = new Vector2(1f, Projectile.ai[1]);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(30, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.DarkGreen, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    pos += Projectile.rotation.ToRotationVector2();

                    for (int j = -1; j < 2; j++)
                    {
                        if (j == 0) continue;

                        Main.spriteBatch.Draw(tex, pos + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2] * j) - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.DarkGreen, i / Projectile.ai[0]) * (eAlpha * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.ai[1] = MathHelper.Clamp(MathF.Sin(progress * MathF.PI) * 0.5f, 0, 1);
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = Helper.TRay.CastLength(Projectile.Center, Projectile.rotation.ToRotationVector2(), 2000);
        }
    }
    public class VileTearTelegraph : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(5, 5);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 40;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[0] > 0)
            {
                Texture2D tex = ExtraTextures.laser4;
                Texture2D chevron = ExtraTextures.chevron_single;
                Vector2 pos = Projectile.Center;
                Vector2 scale = new Vector2(1f, Projectile.ai[1]);
                float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
                float eAlpha = MathHelper.Lerp(1, 0, Projectile.ai[2]);
                for (float i = 0; i < Projectile.ai[0]; i++)
                {
                    float x = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, (i / Projectile.ai[0]) * 5), 0, 1);
                    float f = MathHelper.Lerp(40, 0, x);
                    float alpha = MathHelper.Lerp(1, 0, x);
                    float chevS = MathHelper.Clamp(MathHelper.Lerp(0.5f, 3, i / Projectile.ai[0]), 0, 1);
                    //Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.DarkGreen, i / Projectile.ai[0]) * (Projectile.ai[1] * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    pos += Projectile.rotation.ToRotationVector2();

                    if (i % 80 == 0)
                    {
                        Vector2 chevP = pos + Projectile.rotation.ToRotationVector2() * ((Projectile.ai[2] * 240) + (progress * 10));
                        Main.spriteBatch.Draw(chevron, chevP - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.DarkGreen, i / Projectile.ai[0]) * (Projectile.ai[1] * eAlpha * 4 * MathHelper.Clamp(alpha + 0.5f, 0, 1)), Projectile.rotation, new Vector2(0, chevron.Height / 2), Projectile.ai[2] * chevS * 0.65f, SpriteEffects.None, 0);
                    }

                    for (int j = -4; j < 5; j += 2)
                    {
                        if (j == 0) continue;

                        Main.spriteBatch.Draw(tex, pos + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * (f * Projectile.ai[2] * j) - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.DarkGreen, i / Projectile.ai[0]) * (eAlpha * alpha), Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                    }
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.ai[1] = MathHelper.Clamp(MathF.Sin(progress * MathF.PI) * 0.5f, 0, 1);
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0] == 0)
                Projectile.ai[0] = 2000;
        }
    }
}