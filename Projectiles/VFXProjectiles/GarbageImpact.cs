using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Utilities;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class GarbageImpact : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 50;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage() => Projectile.timeLeft > 45;
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue);
            for (int k = 0; k < 50; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Ash, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("cone2");
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 30;
            float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(rand.NextFloat(MathHelper.Pi, MathHelper.TwoPi) * (rand.NextFloatDirection() > 0 ? 1 : -1) + Projectile.ai[1]);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(new Vector2(1, rand.NextFloat(0.2f, 0.8f)));
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(Color.OrangeRed.ToVector4() * alpha * 0.5f);
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max * 2) + MathHelper.Pi;
                float scale = rand.NextFloat(0.15f, .6f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(-10, 20) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.OrangeRed * alpha * 0.5f, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max * 2) + MathHelper.Pi;
                float scale = rand.NextFloat(0.3f, .8f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(-10, 20) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.OrangeRed * alpha * 0.5f, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.15f);
            if (Projectile.ai[1] > 1)
                Projectile.Kill();
        }
    }
}
