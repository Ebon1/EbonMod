using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace EbonianMod.Projectiles.Garbage
{
    public class Pipebomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.width = 18;
            Projectile.timeLeft = 100;
            Projectile.height = 36;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 30)
            {
                Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CircleTelegraph>(), 0, 0);
                Projectile.velocity = Vector2.Zero;
                Projectile.aiStyle = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosionWSprite>(), Projectile.damage, 0, Projectile.owner);
        }
    }
}
