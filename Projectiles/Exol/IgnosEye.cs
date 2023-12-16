using EbonianMod.Dusts;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    internal class IgnosEye : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 120;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Projectile.Center = Main.LocalPlayer.Center + new Vector2(Projectile.ai[1], 0).RotatedBy(Projectile.ai[0] += MathHelper.ToRadians(Projectile.ai[2]));
            if (++Projectile.frameCounter >= 5)
            {
                Vector2 pos = Projectile.Center + 200 * Main.rand.NextVector2Unit();
                Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, Projectile.Center) * Main.rand.NextFloat(10, 20), newColor: Color.DarkOrange, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                a.noGravity = false;
                a.customData = 1;
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 50, 0);
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, 50, 0);
            a.hostile = true;
            a.friendly = false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D eye = ModContent.Request<Texture2D>(Texture + "_Extra").Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 120, Projectile.width, 120), Color.White * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 4) + 1) * 0.5f), Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Vector2 eyeOffset = new Vector2(MathHelper.Clamp(Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center, false).X * 0.01f, -10, 10), MathHelper.Clamp(Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center, false).Y * 0.01f, -6, 6));
            Main.spriteBatch.Draw(eye, Projectile.Center + new Vector2(0, 35) + eyeOffset - Main.screenPosition, null, Color.White, 0, eye.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
