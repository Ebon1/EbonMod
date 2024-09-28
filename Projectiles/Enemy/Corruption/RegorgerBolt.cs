using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.Enemy.Corruption
{
    public class RegorgerBolt : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 500;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 400;
            Projectile.Size = new(5, 5);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[1]);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i) * alpha;
                if (i > 0 && Projectile.oldPos[i] != Projectile.position)
                    for (float j = 0; j < 3; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 3));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * (0.35f + Projectile.ai[1]) * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.01f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * (0.25f + Projectile.ai[1]) * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * (1 - Projectile.ai[1]) * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.01f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[2]++ == 0)
                Projectile.timeLeft = 30 * 120;
            Projectile.velocity = Vector2.Zero;
            return false;
        }
        Vector2 velocity;
        public override void OnSpawn(IEntitySource source)
        {
            velocity = Projectile.velocity * 5;
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Projectile.velocity).noGravity = true;
            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.001f);
            }
            Projectile.ai[0]++;
            Projectile.direction = velocity.X > 0 ? 1 : -1;
            Projectile.velocity = velocity;
            Projectile.Center += new Vector2(0, MathF.Sin(Projectile.ai[0] * (1 / 3f)) * 0.1f).RotatedBy(velocity.ToRotation());
            if (Projectile.ai[0] > 550)
                Projectile.ai[0] = 0;
        }
    }
}
