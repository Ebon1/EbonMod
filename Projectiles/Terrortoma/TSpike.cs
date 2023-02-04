using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Terrortoma
{
    public class TSpike : ModProjectile
    {
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
            return Color.White * ((255 - Projectile.alpha) / 255);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
