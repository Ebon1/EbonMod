using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Melee;
using Terraria.GameContent;
using Terraria.Audio;


namespace EbonianMod.Projectiles.Cecitior
{
    internal class BloodLaser : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/EbonianGatlingBullet";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(30, 10);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D tex = Helper.GetExtraTexture("EbonianGatlingBullet");
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Black * (1f - fadeMult * i), Projectile.rotation, tex.Size() / 2, Projectile.scale * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Black, Projectile.rotation, Projectile.Size / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);

            spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Red * 0.5f * (1f - fadeMult * i), Projectile.rotation, tex.Size() / 2, Projectile.scale * Math.Clamp((1f - fadeMult * i) * 2, 0, 1), SpriteEffects.None, 0);
            }
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Red * 0.3f, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);


            spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft > 490 && Projectile.velocity.Length() > 0.05f)
                Projectile.velocity *= 0.9f;
            if (Projectile.timeLeft < 450 && Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.15f;
        }
    }
}
