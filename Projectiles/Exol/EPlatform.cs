using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;


namespace EbonianMod.Projectiles.Exol
{
    public class EPlatform : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.hide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 420;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.velocity *= 0.975f;
            if (Projectile.timeLeft < 40 * (Projectile.ai[2] + 1))
                Projectile.velocity.Y += 1;
        }
    }
}
