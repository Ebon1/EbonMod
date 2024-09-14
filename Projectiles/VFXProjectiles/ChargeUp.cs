using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class ChargeUp : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        int seed;
        public override bool PreDraw(ref Color lightColor)
        {

            if (seed == 0) seed = Main.rand.Next(int.MaxValue / 2);
            Texture2D tex = Helper.GetExtraTexture("Extras2/scratch_03");
            float max = 40;
            Main.spriteBatch.Reload(BlendState.Additive);
            UnifiedRandom rand = new UnifiedRandom(seed);
            float ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[2] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.01f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(300, 400) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.DarkRed, Color.Maroon, Projectile.ai[2] * 2) * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[2] * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[1] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.025f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(400, 600) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Maroon, Color.Red, Projectile.ai[1] * 2) * (ringScale * 0.7f), angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[1] * 6.5f, 0, 1.1f), ringScale) * scale * 0.3f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1.2f, 0, MathHelper.Clamp(Projectile.ai[0] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.04f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(350, 500) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.White, Projectile.ai[0] * 2) * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[0] * 6.5f, 0, 1.2f), ringScale) * scale * 0.3f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 50)
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1f, 0.007f);
            if (Projectile.timeLeft < 40)
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1f, 0.011f);
            if (Projectile.timeLeft < 30)
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1f, 0.015f);
        }
    }
    public class GreenChargeUp : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 50;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        int seed;
        public override bool PreDraw(ref Color lightColor)
        {

            if (seed == 0) seed = Main.rand.Next(int.MaxValue / 2);
            Texture2D tex = Helper.GetExtraTexture("Extras2/scratch_03");
            float max = 40;
            Main.spriteBatch.Reload(BlendState.Additive);
            UnifiedRandom rand = new UnifiedRandom(seed);
            float ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[2] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.03f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(300, 400) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Olive, Color.Green, Projectile.ai[2] * 2) * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[2] * 6.5f, 0, 1), ringScale) * scale * 0.2f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1, 0, MathHelper.Clamp(Projectile.ai[1] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.06f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(400, 600) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Green, Color.LawnGreen, Projectile.ai[1] * 2) * (ringScale * 0.7f), angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[1] * 6.5f, 0, 1.1f), ringScale) * scale * 0.3f, SpriteEffects.None, 0);
                }
            }
            rand = new UnifiedRandom(seed + 1);
            ringScale = MathHelper.Lerp(1.2f, 0, MathHelper.Clamp(Projectile.ai[0] * 3.5f, 0, 1));
            if (ringScale > 0.01f)
            {
                for (float i = 0; i < max; i++)
                {
                    UnifiedRandom rand2 = new UnifiedRandom(seed + (int)i);
                    float angle = Helper.CircleDividedEqually(i, max) + Main.GameUpdateCount * 0.09f * rand.NextFloat(0.1f, 2f);
                    float scale = rand.NextFloat(0.1f, .5f);
                    Vector2 offset = new Vector2(rand2.NextFloat(350, 500) * (ringScale + rand2.NextFloat(-0.2f, 0.5f)) * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.White, Projectile.ai[0] * 2) * ringScale, angle, tex.Size() / 2, new Vector2(MathHelper.Clamp(Projectile.ai[0] * 6.5f, 0, 1.2f), ringScale) * scale * 0.3f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return true;
        }
        public override void AI()
        {
            if (Projectile.timeLeft < 50)
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1f, 0.007f);
            if (Projectile.timeLeft < 40)
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1f, 0.011f);
            if (Projectile.timeLeft < 30)
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1f, 0.015f);
        }
    }
}
