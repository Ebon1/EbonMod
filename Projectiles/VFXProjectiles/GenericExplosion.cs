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

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class GenericExplosion : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 300;
            Projectile.width = 300;
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
            float alpha = MathHelper.Lerp(2, 0, Projectile.ai[0]);
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
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
    public class FlameExplosion2 : ModProjectile
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
            Texture2D tex2 = Helper.GetExtraTexture("flameEye");
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
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
