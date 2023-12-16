using EbonianMod.NPCs.Exol;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Exol
{
    internal class ESickle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;

            Projectile.timeLeft = 200;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        Vector2 start;
        public override void AI()
        {
            if (Projectile.timeLeft == 199)
                start = Projectile.Center;
            Projectile.rotation += (float)Projectile.direction * 0.8f;
            if (Projectile.timeLeft > 150)
            {
                Projectile.ai[2] = Vector2.Distance(Projectile.Center, start);
                Projectile.velocity *= 0.98f;
            }
            if (Projectile.timeLeft < 150)
            {
                Projectile.ai[2] = MathHelper.Max(0, Projectile.ai[2] - 4f);
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<Ignos>())
                    {
                        Projectile.velocity = Helper.FromAToB(Projectile.Center, npc.Center + new Vector2(Projectile.ai[2], 0).RotatedBy(Projectile.ai[1] += MathHelper.ToRadians(1)), false) * 0.1f;
                        break;
                    }
                }
            }
        }
    }
}
