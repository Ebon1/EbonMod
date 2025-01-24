using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace EbonianMod.Projectiles.Garbage
{
    public class GarbageBag : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 22;
            Projectile.timeLeft = 500;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.height = 24;
            Projectile.Opacity = 1;
        }
        public override bool? CanDamage() => Projectile.Opacity > 0.5f;
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.frame == 0 && Projectile.timeLeft > 100)
            {
                SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);
                Projectile.timeLeft = 100;
            }
            Projectile.velocity.Y *= 0.5f;
            Projectile.velocity.X = 0;
            Projectile.frame = 1;
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void AI()
        {
            Projectile.tileCollide = Projectile.Center.Y > Main.player[Projectile.owner].Center.Y - 20;
            Projectile.velocity *= 1.025f;
            if (Projectile.velocity.Y > 0)
                Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0, 0.03f);
            if (Projectile.frame == 0)
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            else
            {
                Projectile.rotation = 0;
                if (Projectile.timeLeft < 40)
                    Projectile.Opacity -= 0.03f;
            }

        }
    }
}
