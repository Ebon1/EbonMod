using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Dusts;
using Terraria.ID;
using EbonianMod.Common.Systems;
using Terraria.Audio;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XAmethyst : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(18, 18);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = -oldVelocity;
            return false;
        }
        public override bool? CanDamage() => Projectile.velocity.Length() > 2.5f;
        public override bool PreKill(int timeLeft)
        {
            int i = 0;
            foreach (Vector2 pos in Projectile.oldPos)
            {
                var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
                float mult = (1f - fadeMult * i);
                Dust.NewDustPerfect(pos + Projectile.Size / 2, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f) * mult);
                Dust.NewDustPerfect(pos + Projectile.Size / 2, ModContent.DustType<SparkleDust>(), Vector2.Zero, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.15f) * mult);
                i++;
            }
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, col * (0.5f + Projectile.ai[1]), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, col * (0.5f + Projectile.ai[1]), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.05f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Indigo, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.05f;
        }
    }
    public class XAmethystCloseIn : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/ArchmageX/XAmethyst";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 250;
            Projectile.Size = new(18, 18);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");
            float alpha = Projectile.ai[2];
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Color col = Color.Lerp(Color.Indigo * 0.5f, Color.Gray, (float)(i / Projectile.oldPos.Length)) * alpha;
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, col * (0.5f), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, col * (0.5f), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.05f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override bool? CanDamage() => Projectile.velocity.Length() > .5f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.05f;
            Vector2 pos = new(Projectile.ai[0], Projectile.ai[1]);
            if (Projectile.timeLeft > 230 && Projectile.ai[2] < 1)
                Projectile.ai[2] += 0.1f;
            if (Projectile.timeLeft < 15)
            {
                Projectile.ai[2] -= 0.1f;
                if (Projectile.ai[2] <= 0)
                    Projectile.Kill();
            }
            if (pos != Vector2.Zero)
            {
                if (Projectile.Distance(pos) < 100)
                {
                    Projectile.ai[2] -= 0.1f;
                    if (Projectile.ai[2] <= 0)
                        Projectile.Kill();
                }
            }
        }
    }
}
