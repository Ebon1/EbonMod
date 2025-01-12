using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Utilities;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class FlameExplosionWSprite : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Fire";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }
        public override bool? CanDamage() => Projectile.alpha < 150;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.Size = new Vector2(150);
            //Projectile.scale = 0.2f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.aiStyle = -1;
        }
        int seed;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(int.MaxValue / 2);
            EbonianSystem.ScreenShakeAmount = 4;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15) * Projectile.scale, 0, default, Main.rand.NextFloat(1, 3) * Projectile.scale).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Granite, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15) * Projectile.scale, 100, default, Main.rand.NextFloat(1, 2) * Projectile.scale).noGravity = true;
            }
        }

        public override bool PreAI()
        {
            if (Projectile.ai[2] == 0)
                Projectile.ai[2] = Projectile.scale;
            return true;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Torch);

            Projectile.scale += 0.05f;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.alpha += 10;
                if (Projectile.ai[1] < 1.1f)
                    Projectile.ai[1] += 0.07f;
            }

            if (Projectile.frameCounter++ >= 3 && Projectile.frame <= Main.projFrames[Type])
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
        }

        float magicRotation;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Color.OrangeRed * Projectile.Opacity;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale - 0.8f, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.Additive);
            color = Color.Orange * Projectile.Opacity;
            for (int i = 0; i < 2; i++)
                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale - 0.8f, SpriteEffects.None, 0);
            texture = Helper.GetTexture("Extras/vortex");

            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = sourceRectangle.Size() / 2f;
            color = Color.OrangeRed * Projectile.Opacity;

            magicRotation += 0.1f;
            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation + magicRotation * i, origin, (Projectile.scale - 0.8f) * 0.5f, SpriteEffects.None, 0);
            }

            Texture2D tex = Helper.GetExtraTexture("cone5");
            Texture2D tex2 = Helper.GetExtraTexture("Extras2/trace_04");
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 30 * Projectile.ai[2];
            float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max);
                float scale = rand.NextFloat(0.2f, 1f) * Projectile.ai[2];
                Vector2 offset = new Vector2(rand.NextFloat(50) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
                Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Red * (alpha * 0.5f), angle, new Vector2(0, tex.Height / 2), new Vector2(alpha, Projectile.ai[1] * 2) * scale, SpriteEffects.None, 0);
                for (float j = 0; j < 3; j++)
                    Main.spriteBatch.Draw(tex2, Projectile.Center + offset - Main.screenPosition, null, Color.OrangeRed * alpha, angle + MathHelper.PiOver2, new Vector2(tex2.Width / 2, 0), new Vector2(alpha, Projectile.ai[1]) * scale * .7f * 2, SpriteEffects.None, 0);
            }


            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
    public class BloodExplosionWSprite : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Fire";

        public override bool ShouldUpdatePosition() => false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.Size = new Vector2(50);
            //Projectile.scale = 0.2f;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.hide = true;
            Projectile.aiStyle = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            EbonianSystem.ScreenShakeAmount = 3;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Bone, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Crimson);

            Projectile.scale += 0.05f;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.alpha += 10;
            }

            if (Projectile.frameCounter++ >= 3 && Projectile.frame <= Main.projFrames[Type])
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
        }

        float magicRotation;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int frameY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color = Color.Red * Projectile.Opacity;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation, origin, Projectile.scale - 0.8f, SpriteEffects.None, 0);

            texture = Helper.GetTexture("Extras/vortex");

            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = sourceRectangle.Size() / 2f;
            color = Color.Red * Projectile.Opacity;

            magicRotation += 0.1f;
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                Main.EntitySpriteDraw(texture, position, sourceRectangle, color, Projectile.rotation + magicRotation * i, origin, (Projectile.scale - 0.8f) * 0.5f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
    public class CircleTelegraph : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.height = 300;
            Projectile.width = 300;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Projectile.ai[1] = 1;
        }
        public override void PostAI()
        {
            if (Projectile.ai[1] == 1)
                Projectile.damage = 0;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("Extras2/star_08");
            float scale = MathHelper.Lerp(1, 0, Projectile.ai[0]);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Black * (Projectile.ai[0] * 0.5f), Main.GameUpdateCount * 0.02f, tex.Size() / 2, scale * 2, SpriteEffects.None, 0);

            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Red * Projectile.ai[0], Main.GameUpdateCount * 0.02f, tex.Size() / 2, scale * 2, SpriteEffects.None, 0);
            tex = Helper.GetExtraTexture("Extras2/star_03");
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Red * Projectile.ai[0], Main.GameUpdateCount * -0.01f, tex.Size() / 2, scale * 4, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
