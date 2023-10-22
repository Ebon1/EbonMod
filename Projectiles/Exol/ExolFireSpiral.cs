using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    public class ExolFireSpiral : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Exol/EFireBig";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(1));
        }
    }
    public class ExolFireExplode : ModProjectile
    {
        public override string Texture => "EbonianMod/Projectiles/Exol/EFireBig";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), Projectile.damage * 2, 0);
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage * 2, 0);
            a.hostile = true;
            a.friendly = false;
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Helper.GetTexture("Projectiles/Exol/EFireBig_Bloom");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 88, 78, 88), new Color(247, 217, 49) * Projectile.ai[2], Projectile.rotation, new Vector2(80, 86) / 2 + new Vector2(2, 0), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 160;
            Projectile.tileCollide = false;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            if (Projectile.scale > 1)
                Projectile.scale -= 0.1f;
            Projectile.velocity *= 0.98f;
            Projectile.ai[2] += 0.01f;
            if (Projectile.timeLeft == 20)
                Projectile.scale = 2f;
            //  Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave2>(), 0, 0);
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}
