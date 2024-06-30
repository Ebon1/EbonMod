using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using EbonianMod.Dusts;
using Terraria.Audio;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XLargeAmethyst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(28, 28);
        }
        public override bool PreKill(int timeLeft)
        {
            int i = 0;
            foreach (Vector2 pos in Projectile.oldPos)
            {
                var fadeMult = 1f / Projectile.oldPos.Length;
                float mult = (1f - fadeMult * i);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), ModContent.DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);

                Dust.NewDustPerfect(pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f), ModContent.DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);
                i++;
            }
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            for (int h = 0; h < 5; h++)
            {
                Projectile.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Circular(5, 5), ModContent.ProjectileType<XAmethystShard>(), Projectile.damage, 0);
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = 1f / Projectile.oldPos.Length;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 + (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount + i * 4) * 0.03f) - Main.screenPosition, null, col * (0.35f), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - (Projectile.Size / 4).RotatedBy((Main.GameUpdateCount - i * 4) * 0.03f) - Main.screenPosition, null, col * (0.35f), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                    }
            }
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        Vector2 p;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;
            float progress = Utils.GetLerpValue(0, 250, Projectile.timeLeft);
            float vel = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 1.5f, 0, 1);
            if (Projectile.timeLeft > 70)
            {
                Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center + Helper.FromAToB(player.Center, Projectile.Center) * 50) * 3 * vel;
                if (Projectile.Distance(player.Center) < 90)
                    Projectile.timeLeft = 70;
            }
            else if (Projectile.timeLeft <= 70 && Projectile.timeLeft > 50)
            {
                Projectile.velocity *= 0.9f;
                p = Projectile.Center;
            }
            if (Projectile.timeLeft <= 50)
            {
                Projectile.ai[1] += 0.1f;
                Projectile.Center = p + Main.rand.NextVector2Circular(Projectile.ai[1], Projectile.ai[1]);
            }
        }
    }
}
