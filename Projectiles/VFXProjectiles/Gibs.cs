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
using EbonianMod.Dusts;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class Gibs : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void OnKill(int timeLeft)
        {
            foreach (Vector2 pos in Projectile.oldPos)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(pos + Projectile.Size / 2, Projectile.ai[2] == 0 ? DustID.Blood : DustID.Torch, Main.rand.NextVector2Circular(3, 3));
        }
        public override string Texture => "EbonianMod/Extras/explosion";
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            Main.spriteBatch.Reload(BlendState.Additive);
            if (Projectile.ai[2] == 0)
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Black * 0.25f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.025f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Black, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.025f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, (Projectile.ai[2] == 0 ? Color.Maroon : Color.OrangeRed) * 0.25f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.025f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Projectile.ai[2] == 0 ? Color.Maroon : Color.OrangeRed, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.025f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 150;
            Projectile.penetrate = -1;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0, 0);
            if (Projectile.ai[1] == 1)
            {
                AIType = -1;
                Projectile.aiStyle = 0;
            }
            if (Projectile.timeLeft < 50)
                Projectile.velocity.X *= 0.975f;
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[2] == 0 ? DustID.Blood : DustID.Torch, Projectile.velocity).noGravity = true;
            else
                Dust.NewDustPerfect(Projectile.Center, Projectile.ai[2] == 0 ? DustID.Blood : DustID.Torch, Main.rand.NextVector2Circular(1.5f, 1.5f));
        }
    }
    public class AmbientGibs : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void OnKill(int timeLeft)
        {
            foreach (Vector2 pos in Projectile.oldPos)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(pos + Projectile.Size / 2, DustID.Blood, Main.rand.NextVector2Circular(3, 3));
        }
        public override string Texture => "EbonianMod/Extras/explosion";
        public override bool PreDraw(ref Color lightColor)
        {
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 5; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 5));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Black * 0.25f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.01f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Black, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.01f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = (1f - fadeMult * i);
                if (i > 0)
                    for (float j = 0; j < 10; j++)
                    {
                        Vector2 pos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[i - 1], (float)(j / 10));
                        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Maroon * 0.25f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.01f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Maroon, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.01f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 150;
            Projectile.penetrate = -1;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 1)
            {
                AIType = -1;
                Projectile.aiStyle = 0;
            }
            if (Projectile.timeLeft < 50)
                Projectile.velocity.X *= 0.975f;
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Projectile.velocity).noGravity = true;
            else
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextVector2Circular(1.5f, 1.5f));
        }
    }
}
