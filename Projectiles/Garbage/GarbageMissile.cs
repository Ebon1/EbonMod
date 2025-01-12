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
using Terraria.ID;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageMissile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }
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
            Projectile.frame = Main.rand.Next(6);
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
        public override bool? CanDamage() => Projectile.timeLeft < 300;
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft > 300)
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]) * 0.99f;
            else
            {
                for (int i = 0; i < 5; i++)
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RainCloud);
                if (Projectile.velocity.Length() < 16)
                    Projectile.velocity *= 1.025f;
            }
            if (Projectile.timeLeft == 300)
                SoundEngine.PlaySound(EbonianSounds.firework, Projectile.Center);
            if (Projectile.timeLeft <= 300 && Projectile.timeLeft >= 295)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 10, 0.2f);
        }
    }
}
