using System;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace EbonianMod.Projectiles.Terrortoma
{
    public class TFlameThrower : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 200));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = 1f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i) * alpha;
                if (i > 0)
                    for (float j = 0; j < 3; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 3));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(101);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 101;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
        public override bool PreKill(int timeLeft)
        {
            Projectile.active = false;
            return false;
        }
    }
    public class TFlameThrower2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 200));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = 1f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i) * alpha;
                if (i > 0)
                    for (float j = 0; j < 3; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 3));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(95);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 95;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
    }
    public class TFlameThrower3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = 1f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i) * alpha;
                if (i > 0)
                    for (float j = 0; j < 3; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 3));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 200));
        }
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(96);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 96;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
    }
    public class TFlameThrower4 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, Main.rand.Next(60, 200));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = 1f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i) * alpha;
                if (i > 0)
                    for (float j = 0; j < 3; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 3));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.LawnGreen * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(96);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 96;
        }
        public override bool ShouldUpdatePosition()
        {
            return ++Projectile.ai[2] > 30;
        }
        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
            if (Projectile.ai[2] > 30)
                Projectile.velocity *= 1.025f;
        }
    }
}
