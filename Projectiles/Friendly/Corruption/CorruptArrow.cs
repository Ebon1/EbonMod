using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Terrortoma;
using Terraria.GameContent;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class CorruptArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            var fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type];
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
            for (int i = 0; i < 5; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 5) + offset;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.UnitX.RotatedBy(angle) * 5 * offset, ModContent.ProjectileType<CorruptThorn>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }
    }
    public class CorruptThorn : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_7";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vilethorn");
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.aiStyle = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            if (Projectile.ai[0] == 0)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha > 0)
                {
                    return;
                }
                Projectile.alpha = 0;
                Projectile.ai[0] = 1f;
                if (Projectile.ai[1] == 0)
                {
                    Projectile.ai[1] += 1f;
                    Projectile.position += Projectile.velocity * 1f;
                }
                int num62 = ModContent.ProjectileType<CorruptThorn>();
                if (Projectile.ai[1] >= 3f)
                {
                    num62 = ModContent.ProjectileType<TerrorVilethorn2>();
                }
                int num63 = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.position.X + Projectile.velocity.X + (float)(Projectile.width / 2), Projectile.position.Y + Projectile.velocity.Y + (float)(Projectile.height / 2), Projectile.velocity.X, Projectile.velocity.Y, num62, Projectile.damage, 1, Projectile.owner);
                Main.projectile[num63].damage = Projectile.damage;
                Main.projectile[num63].hostile = false;
                Main.projectile[num63].friendly = true;
                Main.projectile[num63].ai[1] = Projectile.ai[1] + 1f;
                NetMessage.SendData(27, -1, -1, null, num63);
            }
            if (Projectile.alpha < 170 && Projectile.alpha + 5 >= 170)
            {
                for (int num73 = 0; num73 < 3; num73++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 18, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 170, default(Color), 1.2f);
                }
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 14, 0, 0, 170, default(Color), 1.1f);
            }
            Projectile.alpha += 2;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}
