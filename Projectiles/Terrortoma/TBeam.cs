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
        int MAX_TIME = 60;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = Main.screenWidth;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.height, Projectile.width, ref a);
        }
        bool RunOnce;

        public override void AI()
        {
            if (RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                MAX_TIME = Projectile.timeLeft;
                RunOnce = false;
            }
            if (Projectile.localAI[1] != 0)
            {
                Projectile.damage = 0;
                Projectile.timeLeft = MAX_TIME;
                Projectile.localAI[1]--;
            }
            else
            {
                Projectile.damage = damage;
            }

            Vector2 end = Projectile.Center + Projectile.velocity * /*Helper.TRay.CastLength(Projectile.Center, Projectile.velocity, */Main.screenWidth/*)*/;

            //Projectile.velocity = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 5);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Helper.Reload(Main.spriteBatch, BlendState.Additive);
            Helper.Reload(Main.spriteBatch, SpriteSortMode.Immediate);
            Texture2D TrailTexture1 = Mod.Assets.Request<Texture2D>("Extras/oracleBeam").Value;
            Texture2D TrailTexture2 = Mod.Assets.Request<Texture2D>("Extras/Tentacle").Value;
            Texture2D TrailTexture3 = Mod.Assets.Request<Texture2D>("Extras/laser").Value;

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f);
            float scale = Projectile.scale * 4 * mult;
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Line").Value;
            Texture2D bolt = Helper.GetExtraTexture("laser4");
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * /*Helper.TRay.CastLength(Projectile.Center, Projectile.velocity,*/ Main.screenWidth;//);
            float num = Vector2.Distance(start, end);
            Vector2 vector = (end - start) / num;
            Vector2 vector2 = start;
            float rotation = vector.ToRotation();
            for (int i = 0; i < num; i++)
            {
                Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.LawnGreen, rotation, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.White, rotation, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                vector2 = start + i * vector;
            }
            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Spotlight").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Spotlight").Value;
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, Color.LawnGreen, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);



            Helper.Reload(Main.spriteBatch, BlendState.AlphaBlend);

            return false;
        }
    }
}