using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
            Projectile.tileCollide = true;
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
            if (Projectile.timeLeft > 175)
                Projectile.velocity *= 1.025f;
            if (Projectile.timeLeft <= 175 && Projectile.timeLeft > 160)
                Projectile.velocity *= 0.85f;
            if (Projectile.timeLeft <= 160 && Projectile.velocity.Length() < 10)
                Projectile.velocity *= 1.1f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
    public class TFang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(4);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - Projectile.alpha) / 255);
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
