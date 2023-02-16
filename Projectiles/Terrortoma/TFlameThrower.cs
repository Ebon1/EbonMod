using System;
using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace EbonianMod.Projectiles.Terrortoma
{
    public class TFlameThrower : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(101);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 101;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
        public override bool PreKill(int timeLeft)
        {
            Projectile.active = false;
            return false;
        }
    }
    public class TFlameThrower2 : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(95);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 95;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
    }
    public class TFlameThrower3 : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(96);
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            AIType = 96;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.LawnGreen, false, false, 0.04f);
        }
    }
}
