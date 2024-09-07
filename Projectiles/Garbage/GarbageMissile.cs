using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageMissile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
        }
        Color col = Color.Transparent;
        public override void OnSpawn(IEntitySource source)
        {
            col = new Color(Main.rand.Next(25, 100), Main.rand.Next(25, 100), 0);
        }
        public override bool? CanDamage() => Projectile.timeLeft < 300;
        public override Color? GetAlpha(Color lightColor) => col;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft > 300)
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]) * 0.99f;
            else
            {
                if (Projectile.velocity.Length() < 16)
                    Projectile.velocity *= 1.025f;
                for (float i = 0; i < 0.99f; i += 0.33f)
                    Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Gray * 0.1f, false, false, 0.1f, 0.125f, -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * i) * Main.rand.NextFloat(0.2f, 0.8f));
            }
            if (Projectile.timeLeft == 300)
                SoundEngine.PlaySound(EbonianSounds.firework, Projectile.Center);
            if (Projectile.timeLeft <= 300 && Projectile.timeLeft >= 295)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 10, 0.2f);
        }
    }
}
