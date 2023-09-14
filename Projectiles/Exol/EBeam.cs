using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Content;

namespace EbonianMod.Projectiles.Exol
{
    public class EBeam : ModProjectile
    {
        int MAX_TIME = 60;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.scale = 0;
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
        bool collide(Rectangle targetHitbox, float offset)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.RotatedBy(offset) * MathHelper.SmoothStep(0, 300, Projectile.scale), Projectile.width, ref a);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < 8; i++)
            {
                if (collide(targetHitbox, i * MathHelper.PiOver4))
                {
                    return true;
                }
            }
            return false;
        }
        bool RunOnce;

        public override void AI()
        {
            if (!RunOnce)
            {
                Projectile.velocity.Normalize();
                Projectile.damage = 100;
                MAX_TIME = Projectile.timeLeft;
                RunOnce = true;
            }

            //Projectile.velocity = -Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * MathHelper.SmoothStep(0.25f, 1.5f, Projectile.scale), 0, 1);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(SpriteSortMode.Immediate);
            Texture2D TrailTexture1 = Mod.Assets.Request<Texture2D>("Extras/oracleBeam").Value;
            Texture2D TrailTexture2 = Mod.Assets.Request<Texture2D>("Extras/Tentacle").Value;
            Texture2D TrailTexture3 = Mod.Assets.Request<Texture2D>("Extras/laser").Value;

            float mult = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f;
            float scale = Projectile.scale * 0.5f;
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Line").Value;
            Texture2D bolt = Helper.GetExtraTexture("laser4");
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * MathHelper.SmoothStep(0, 350, Projectile.scale);
            float num = Vector2.Distance(start, end);
            Vector2 vector = (end - start) / num;
            Vector2 vector2 = start;
            float rotation = vector.ToRotation();
            for (int j = 0; j < 8; j++)
                for (int i = 0; i < num; i++)
                {
                    if (i == 0 && j != 0)
                    {
                        vector2 = start + i * vector.RotatedBy(MathHelper.PiOver4 * j);
                        continue;
                    }
                    Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.Lerp(Color.OrangeRed, Color.White * 0f, (float)i / num), rotation + MathHelper.PiOver4 * j, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(bolt, vector2 - Main.screenPosition, null, Color.Lerp(Color.White, Color.OrangeRed * 0f, (float)i / num), rotation + MathHelper.PiOver4 * j, bolt.Size() / 2, new Vector2(1, Projectile.scale), SpriteEffects.None, 0f);
                    vector2 = start + i * vector.RotatedBy(MathHelper.PiOver4 * j);
                }
            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/light_01").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed, Main.GameUpdateCount * 0.05f, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Main.GameUpdateCount * -0.05f, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);


            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/star_09").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Extras2/star_08").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Spotlight").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Main.GameUpdateCount * 0.003f, new Vector2(texture.Width, texture.Height) / 2, scale * 2, SpriteEffects.None, 0f);



            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}