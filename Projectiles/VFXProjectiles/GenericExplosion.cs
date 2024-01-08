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

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class GenericExplosion : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 200;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
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
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Helper.DustExplosion(Projectile.Center, Vector2.One, 0, sound: false);
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
    public class FlameExplosion : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 200;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Texture2D tex2 = Helper.GetExtraTexture("flameEye");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(2, 0, Projectile.ai[0]);
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Desert);
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
    public class FlameExplosion2 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
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
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Texture2D tex2 = Helper.GetExtraTexture("flameEye");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0] * 2);
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //Helper.DustExplosion(Projectile.Center, Vector2.One, 0, sound: false);
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 0.5f)
                Projectile.Kill();
        }
    }
    public class FlameExplosion3 : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
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
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Texture2D tex2 = Helper.GetExtraTexture("flameEye");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(2, 0, Projectile.ai[0] * 2);
            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex2.Size() / 2, Projectile.ai[0] * 0.3f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //Helper.DustExplosion(Projectile.Center, Vector2.One, 0, sound: false);
        }
        public override void AI()
        {
            Projectile.ai[0] += 0.025f;
            if (Projectile.ai[0] > 0.5f)
                Projectile.Kill();
        }
    }
    public class FlameExplosionWSprite : ModProjectile
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

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            EbonianSystem.ScreenShakeAmount = 5;

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            Projectile.rotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.Granite, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 15), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
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

            texture = Helper.GetTexture("Extras/vortex");

            sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            origin = sourceRectangle.Size() / 2f;
            color = Color.OrangeRed * Projectile.Opacity;

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
        public override void SetDefaults()
        {
            Projectile.height = 300;
            Projectile.width = 300;
            Projectile.hostile = true;
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
            Texture2D tex = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            float scale = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Maroon * Projectile.ai[0] * 0.75f, Projectile.rotation, tex.Size() / 2, scale, SpriteEffects.None, 0);
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
