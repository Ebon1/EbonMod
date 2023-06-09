using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    public class EBoulder : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 450;
            Projectile.hide = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 420)
            {
                return Color.Red * ((105 - Projectile.alpha) / 105f);
            }
            return Color.White * ((225 - Projectile.alpha) / 225f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Projectile.ModProjectile.GetAlpha(Color.White).Value, Projectile.rotation, a.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft > 400 && Projectile.timeLeft < 420)
                Projectile.velocity *= 0.9f;
            if (Projectile.timeLeft == 400)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 0, 0);
                for (int i = 0; i < 10; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, 10);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<EFire>(), Projectile.damage, 0);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                }
                Projectile.Kill();
            }
        }
    }
    public class EBoulder2 : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Exol/EBoulder";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 450;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = TextureAssets.Projectile[Type].Value;
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, a.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
    public class EFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.timeLeft = 190;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255f);
        }
        public override void AI()
        {
            if (Projectile.timeLeft <= 150)
            {
                Projectile.velocity *= 1.15f;
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

        }
    }
}
