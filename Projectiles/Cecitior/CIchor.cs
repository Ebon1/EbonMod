using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace EbonianMod.Projectiles.Cecitior
{
    public class CIchor : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, Main.rand.Next(100, 300));
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
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Gold * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f * mult, SpriteEffects.None, 0);
                        Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + Projectile.Size / 2 - Main.screenPosition, null, Color.Gold * mult, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.035f * mult, SpriteEffects.None, 0);
                    }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, Projectile.Center - Main.screenPosition, null, Color.Gold * alpha, 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.02f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GoldenShowerHostile);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = ProjectileID.GoldenShowerHostile;
        }

        public override bool PreKill(int timeLeft)
        {
            Projectile.active = false;
            return false;
        }
    }
}
