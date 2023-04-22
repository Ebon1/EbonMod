using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;

namespace EbonianMod.Projectiles.Friendly.Underworld
{
    public class CorebreakerP : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(30);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 2;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), Projectile.damage * 2, 0);
            a.friendly = true;
            a.hostile = false;
        }
        public override void AI()
        {
        }
    }
}
