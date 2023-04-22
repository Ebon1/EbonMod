using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class IchorGrenade : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 2;
            Projectile.timeLeft = 500;
            Projectile.Size = new(8);
        }
        public override void Kill(int timeLeft)
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.Gold, scaleFactor: 0.05f);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch);
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
