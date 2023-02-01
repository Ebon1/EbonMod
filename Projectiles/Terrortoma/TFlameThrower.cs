using System;
using ExolRebirth.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace ExolRebirth.Projectiles.Terrortoma
{
    public class TFlameThrower : ModProjectile
    {
        public override string Texture => "ExolRebirth/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(101);
            Projectile.friendly = false;
            Projectile.hostile = true;
            AIType = 101;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size, true, Color.LawnGreen, false, false);
        }
    }
    public class TFlameThrower2 : ModProjectile
    {
        public override string Texture => "ExolRebirth/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(95);
            Projectile.friendly = false;
            Projectile.hostile = true;
            AIType = 95;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size / 2, true, Color.LawnGreen, false, false, 0.15f);
        }
    }
    public class TFlameThrower3 : ModProjectile
    {
        public override string Texture => "ExolRebirth/Extras/Empty";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(96);
            Projectile.friendly = false;
            Projectile.hostile = true;
            AIType = 96;
        }

        public override void PostAI()
        {
            Helper.DustExplosion(Projectile.Center, Projectile.Size / 2, true, Color.LawnGreen, false, false, 0.15f);
        }
    }
}
