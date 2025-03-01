using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Items.Misc;

namespace EbonianMod.Projectiles.Friendly.Corruption
{
    public class CorruptionBalls : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 2;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitY, Projectile.height * 1.5f) < Projectile.height)
            {
                Projectile.velocity.Y = 0;
                return false;
            }
            if ((Helper.TRay.CastLength(Projectile.Center, Vector2.UnitX, Projectile.height * 1.5f) < Projectile.width || Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitX, Projectile.height * 1.5f) < Projectile.width) && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, Projectile.height * 1.5f) > Projectile.height)
            {
                Projectile.velocity.X = 0;
                return false;
            }
            Projectile.Kill();
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
            SoundEngine.PlaySound(EbonianSounds.eggplosion, Projectile.Center);
        }
 
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame == 0) Projectile.frame++;
                else Projectile.frame = 0;
            }
            Projectile.velocity.Y += 0.25f;
        }
    }
}
