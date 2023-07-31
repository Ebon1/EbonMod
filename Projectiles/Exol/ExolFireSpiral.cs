using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
}
