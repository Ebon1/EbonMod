using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EbonianMod.Helper;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    internal class SerrationSpike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 44;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tex.Width / 2, tex.Height), new Vector2(Projectile.ai[2], MathHelper.Clamp(Projectile.ai[2] * 2, 0, (Projectile.ai[0] + 1) * 0.2f)), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.position);
            for (int num495 = 0; num495 < 15; num495++)
            {
                int num496 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(0, 101), default(Color), 1f + (float)Main.rand.Next(40) * 0.01f);
                Main.dust[num496].noGravity = true;
                Dust dust2 = Main.dust[num496];
                dust2.velocity *= 2f;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            if (Projectile.ai[2] < (Projectile.ai[0] + 1) * 0.2f)
                Projectile.ai[2] += 0.1f;
            if (Projectile.timeLeft == 38)
            {
                SoundStyle style = SoundID.NPCHit18;
                style.Volume = 0.5f;
                SoundEngine.PlaySound(style, Projectile.Center);
                for (int num495 = 0; num495 < 15; num495++)
                {
                    int num496 = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(0, 101), default(Color), 1f + (float)Main.rand.Next(40) * 0.01f);
                    Main.dust[num496].noGravity = true;
                    Dust dust2 = Main.dust[num496];
                    dust2.velocity *= 2f;
                }

                if (Projectile.ai[0] < 7)
                {
                    Projectile a = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), ((Projectile.ai[0] + 1) * 4) * Projectile.velocity + (TRay.Cast(Projectile.Center - Vector2.UnitY * 30, Vector2.UnitY, 500, true)) + new Vector2(0, (Projectile.ai[0] + 1)), Projectile.velocity, ModContent.ProjectileType<SerrationSpike>(), (int)(Projectile.damage * 0.7f), Projectile.knockBack, Projectile.owner, Projectile.ai[0]);
                    a.ai[0] = Projectile.ai[0] + 1;
                }
            }
            Projectile.velocity.Normalize();
            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
            Projectile.rotation = MathHelper.ToRadians(20 * Projectile.direction);
        }
    }
}