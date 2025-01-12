using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles
{
    public class CursedFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(16, 16);
            Projectile.scale = 1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 10;
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            AI();
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.CursedTorch, Main.rand.NextVector2Circular(1, 1), 0, default, 2).noGravity = true;

            if (++Projectile.ai[0] >= 30)
            {
                if (Projectile.velocity.Length() < 20)
                    Projectile.velocity *= 1.1f;
            }
            else
                Projectile.velocity *= 0.9f;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;

                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            for (int k = 0; k < 20; k++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), DustID.CursedTorch, Projectile.velocity.SafeNormalize(Vector2.Zero) * 10 * Main.rand.NextFloat(), 0, default, 3).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawGlow();
            DrawTrail();

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Color color = Projectile.GetAlpha(new Color(255, 255, 255, 128));

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

            return false;
        }

        void DrawGlow()
        {
            Texture2D texture = ModContent.Request<Texture2D>("EbonianMod/Extras/Bloom").Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Rectangle sourceRectangle = texture.Frame();

            Color color = Projectile.GetAlpha(new Color(96, 248, 2, 0));

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.08f * 2, spriteEffects, 0);
        }

        void DrawTrail()
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);

            Vector2 drawOrigin = sourceRectangle.Size() / 2;

            SpriteEffects spriteEffects = SpriteEffects.None;

            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 drawPosition = (Projectile.oldPos[k] - Main.screenPosition) + (drawOrigin / 2) + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 255, 255, 128), new Color(0, 0, 0, 0), k / (float)Projectile.oldPos.Length));

                Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            }
        }
    }
}