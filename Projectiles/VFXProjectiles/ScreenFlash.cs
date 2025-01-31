using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace EbonianMod.Projectiles.VFXProjectiles
{
    public class ScreenFlash : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 300;
            Projectile.width = 300;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
        }
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Projectile.ai[1] = 1;
        }
        public override void PostAI()
        {
            if (Projectile.ai[1] == 1)
                Projectile.damage = 0;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(ExtraTextures.Line, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * (Projectile.ai[0] - Projectile.ai[2]) * 2);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = 1;
        }
        public override void AI()
        {
            Projectile.ai[0] -= Projectile.ai[1] + 0.01f;
            if (Projectile.ai[0] <= 0)
                Projectile.Kill();
        }
    }
}
