using EbonianMod.Dusts;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XKnife : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/AmethystShard";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
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
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.velocity.ToRotation().ToRotationVector2() * 15 + Projectile.Size / 2 - Main.screenPosition, null, col * (0.4f + Projectile.ai[1]), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult + (((MathF.Sin(Main.GlobalTimeWrappedHourly * 3) + 1) / 2) * 0.005f), SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.velocity.ToRotation().ToRotationVector2() * 15 + Projectile.Size / 2 - Main.screenPosition, null, col * (0.4f + Projectile.ai[1]), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.05f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.Indigo, Projectile.rotation, glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(30, 30);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), -Projectile.velocity * 0.5f, 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
        }
    }
}
