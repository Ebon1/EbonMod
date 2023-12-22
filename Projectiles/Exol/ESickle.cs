using EbonianMod.NPCs.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
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
        public override void OnKill(int timeLeft)
        {

            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, Projectile.damage, Projectile.knockBack);

            a.hostile = true;
            a.friendly = false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 199)
                start = Projectile.Center;
            Projectile.rotation += (float)Projectile.direction * 0.8f;
            if (Projectile.timeLeft > 150)
            {
                Projectile.ai[0] = 2f;
                Projectile.ai[2] = Vector2.Distance(Projectile.Center, start);
            }
            if (Projectile.timeLeft > 175)
                Projectile.velocity *= 1.1f;
            if (Projectile.timeLeft > 150 && Projectile.timeLeft < 175)
                Projectile.velocity *= 0.98f;
            if (Projectile.timeLeft < 150 && Projectile.timeLeft > 75)
            {
                Projectile.ai[0] *= 0.98f;
                //Projectile.ai[2] = MathHelper.Max(0, Projectile.ai[2] - 4f);
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<Ignos>())
                    {
                        start = npc.Center;
                        Projectile.velocity = Helper.FromAToB(Projectile.Center, npc.Center + new Vector2(Projectile.ai[2], 0).RotatedBy(Projectile.ai[1] += MathHelper.ToRadians(2f * Projectile.ai[0])), false) * 0.2f;
                        break;
                    }
                }
            }

            if (Projectile.timeLeft == 80)
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave2>(), 0, Projectile.knockBack);
            if (Projectile.timeLeft < 75)
            {
                Projectile.timeLeft--;
                Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 20, 0.15f);
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == ModContent.NPCType<Ignos>())
                    {
                        Projectile.velocity = Helper.FromAToB(Projectile.Center, start, true) * Projectile.ai[0];
                        break;
                    }
                }
            }
        }
    }
}
