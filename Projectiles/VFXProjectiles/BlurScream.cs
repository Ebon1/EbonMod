using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class BlurScream : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 39;
            Projectile.scale = 0;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            EbonianMod.blurDrawCache.Add(() =>
            {
                Texture2D tex = ExtraTextures.cone2_blur;
                UnifiedRandom rand = new UnifiedRandom(seed);
                float max = 60;
                float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
                for (float i = 0; i < max; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, max);
                    float scale = rand.NextFloat(0.5f, 2f);
                    Vector2 offset = new Vector2(Main.rand.NextFloat(150, 300) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                    for (float j = 0; j < 2; j++)
                        Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.White * alpha * 3, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 2, SpriteEffects.None, 0);
                }
            });
            return false;
        }
        public override void AI()
        {
            float progress = Utils.GetLerpValue(0, 40, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi) * 0.5f, 0, 0.5f);
            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.1f);
        }
    }
}
