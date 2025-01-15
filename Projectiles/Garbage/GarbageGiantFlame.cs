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

using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageGiantFlame : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/fireball";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreKill(int timeLeft)
        {
            int b = 0;
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
            foreach (Vector2 pos in Projectile.oldPos)
            {
                b++;
                float s = MathHelper.SmoothStep(0, 1, (1f - fadeMult * b * 2));
                if (b % 6 == 0 && b < Projectile.oldPos.Length / 2 - 6)
                    Projectile.NewProjectileDirect(null, pos + Projectile.Size / 2, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), Projectile.damage, 0).scale = s;
                float Y = MathHelper.Lerp(60, 0, (float)(MathHelper.Clamp(Projectile.velocity.Length(), -10, 10) + 10) / 20);
                Vector2 oldpos = Vector2.SmoothStep(pos, pos - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)b / Projectile.oldPos.Length);
                if (b < Projectile.oldPos.Length / 2)
                    for (int i = 0; i < 15; i++)
                        Dust.NewDustPerfect(oldpos + Projectile.Size / 2 + Main.rand.NextVector2Circular(15, 15), DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f) * (3 + (1f - fadeMult * b)), Scale: 1 + fadeMult * 1.5f).noGravity = true;
            }
            if (Projectile.ai[2] != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(null, Projectile.Center, new Vector2(Main.rand.NextFloat(-3, 3), Main.rand.NextFloat(-8, -0.1f)), ProjectileType<GarbageFlame>(), Projectile.damage, 0);
                }
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;

            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
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
                    Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + new Vector2(5, 10 / Main.GameZoomTarget) + Projectile.Size / 2 - Main.screenPosition, null, Color.Lerp(Color.DarkRed * 0.1f, Color.DarkOrange, mult) * mult * 0.2f, Helper.FromAToB(oldpos, olderpos).ToRotation() + MathHelper.PiOver2, TextureAssets.Projectile[Type].Value.Size() / 2, 0.35f * mult * 4f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center + new Vector2(5, 10 / Main.GameZoomTarget) - Main.screenPosition, null, Color.OrangeRed * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, TextureAssets.Projectile[Type].Value.Size() / 2, 0.35f * 4, SpriteEffects.None, 0);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = 2;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {

        }
    }
}
