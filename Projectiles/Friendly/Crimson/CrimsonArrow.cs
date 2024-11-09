using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Terrortoma;
using Terraria.GameContent;
using Terraria.Map;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class CrimsonArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, lightColor * (1f - fadeMult * i), Projectile.rotation, a.Size() / 2, Projectile.scale * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
            Projectile.width = 18;
            Projectile.height = 42;
        }
        public override void Kill(int timeLeft)
        {
            float offset = Main.rand.NextFloat(MathHelper.Pi * 2);
            for (int i = 0; i < 2; i++)
            {
                float angle = (i + 1) * offset;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(angle) * 4, ModContent.ProjectileType<Gibs>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
            }
        }
    }
}
