using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageFlame : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Extras2/fire_01";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreKill(int timeLeft)
        {
            int b = 0;
            var fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            foreach (Vector2 pos in Projectile.oldPos)
            {
                b++;
                float Y = MathHelper.Lerp(60, 0, (float)(MathHelper.Clamp(Projectile.velocity.Length(), -10, 10) + 10) / 20);
                Vector2 oldpos = Vector2.SmoothStep(pos, pos - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)b / Projectile.oldPos.Length);
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(oldpos + Projectile.Size / 2, DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f) * (3 + (1f - fadeMult * b)), Scale: 1 + fadeMult * 1.5f).noGravity = true;
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;

            var fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float Y = MathHelper.Lerp(60, 0, (float)(MathHelper.Clamp(Projectile.velocity.Length(), -10, 10) + 10) / 20);
                Vector2 oldpos = Vector2.SmoothStep(Projectile.oldPos[i], Projectile.oldPos[i] - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)i / Projectile.oldPos.Length);
                Vector2 olderpos = Vector2.SmoothStep(Projectile.oldPos[i - 1], Projectile.oldPos[i - 1] - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)i / Projectile.oldPos.Length);
                if (oldpos == Vector2.Zero || oldpos == Projectile.position) continue;
                float mult = (1f - fadeMult * i);
                for (float j = 0; j < 5; j++)
                {
                    Vector2 pos = Vector2.Lerp(oldpos, olderpos, (float)(j / 5));
                    Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + new Vector2(5, 10 / Main.GameZoomTarget) + Projectile.Size / 2 - Main.screenPosition, null, Color.Lerp(Color.DarkOrange, Color.OrangeRed, mult) * mult * 0.4f, Main.GameUpdateCount * 0.03f * i, TextureAssets.Projectile[Type].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center + new Vector2(5, 10 / Main.GameZoomTarget) - Main.screenPosition, null, Color.Yellow, Main.GameUpdateCount * 0.03f, TextureAssets.Projectile[Type].Value.Size() / 2, 0.035f, SpriteEffects.None, 0);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Yellow, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.035f, SpriteEffects.None, 0);
            //Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.03f, SpriteEffects.None, 0);
            Main.spriteBatch.ApplySaved();
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
        }
        float savedP;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= savedP - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            if (savedP == 0)
                savedP = Main.LocalPlayer.Center.Y;
            if (Projectile.velocity.Y > 2.8f && Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.87f;
            }
            if (Main.rand.Next(2) == 0)
            {
                /*for (int dustNumber = 0; dustNumber < 5; dustNumber++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 6, 0, 0, 0, default(Color), 1f)];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                    dust.velocity.X = Main.rand.NextFloat(-0.5f, 0.5f);
                    dust.velocity.Y = -2f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.65f, 1.25f);
                }*/
            }
        }
    }
}
