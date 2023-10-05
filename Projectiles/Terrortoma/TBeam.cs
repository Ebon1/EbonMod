using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Terrortoma
{
    public class TBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 25;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * ((1920 / 4) * Projectile.scale), Projectile.width, ref a);
        }
        bool RunOnce;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                RunOnce = false;
            }


            //Projectile.velocity = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 25, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI), 0, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Helper.Reload(Main.spriteBatch, BlendState.Additive);
            Helper.Reload(Main.spriteBatch, SpriteSortMode.Immediate);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f);
            float scale = Projectile.scale * 4 * mult;
            Texture2D bolt = Helper.GetExtraTexture("laser4");
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * ((1920 / 2) * Projectile.scale);
            float num = Vector2.Distance(start, end);
            Vector2 vector = (end - start) / num;
            Vector2 vector2 = start;
            float rotation = vector.ToRotation();
            for (int j = 0; j < 2; j++)
                for (int i = 0; i < num; i++)
                {
                    Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.LawnGreen * 0f, (float)(i / num)) * Projectile.scale, rotation, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                    vector2 = start + i * vector;
                }
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/light_01").Value;

            for (int j = 0; j < 2; j++)
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.LawnGreen * Projectile.scale, Main.GameUpdateCount * 0.03f, new Vector2(texture.Width, texture.Height) / 2, scale * 0.1f, SpriteEffects.None, 0f);

            Helper.Reload(Main.spriteBatch, BlendState.AlphaBlend);

            return false;
        }
    }
}