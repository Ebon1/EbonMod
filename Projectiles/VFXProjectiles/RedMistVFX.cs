using EbonianMod.Dusts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class RedMistVFX : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 5;
            Projectile.width = 5;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 340;
        }
        int seed, baseDir;
        public override void OnSpawn(IEntitySource source)
        {
            seed = Main.rand.Next(10000);
            baseDir = Projectile.velocity.X > 0 ? 1 : -1;
        }
        public override void AI()
        {
            UnifiedRandom rand = new UnifiedRandom(seed);
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rand.NextFloat(1, 2) * 0.5f * baseDir));
            //Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2Dark>(), Vector2.Zero, Scale: Projectile.timeLeft * 0.015f);
            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustType<RedGoopDust>(), Projectile.velocity.RotatedByRandom(PiOver4 / 4) * MathHelper.Lerp(2, 0.5f, (float)Projectile.timeLeft / 340), 0, Color.White * 0.7f, Clamp(MathF.Sin(Utils.GetLerpValue(340, 0, Projectile.timeLeft) * Pi) * 1.5f, 0, 1f) * rand.NextFloat(0.5f, 1.2f));
        }
    }
}
