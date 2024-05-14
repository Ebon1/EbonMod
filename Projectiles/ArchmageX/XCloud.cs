using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.ArchmageX
{
    public class XCloud : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.height = 120;
            Projectile.width = 200;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 340;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(7, 2), 0, Color.White * 0.5f, Scale: Main.rand.NextFloat(2, 3)).customData = 1;
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(7, 2), 0, Color.White, Scale: Main.rand.NextFloat(1, 2));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(15, 15), Scale: Main.rand.NextFloat(0.5f, 1));
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.DarkOrchid, Scale: Main.rand.NextFloat(0.1f, .15f));
            }
        }
        Vector2 savedDir, savedP;
        public override void AI() // REWORK VFX, https://discord.com/channels/1068976079131390053/1068976080746193050/1240006628917379073
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(7, 2), 0, Color.White * 0.5f, Scale: Main.rand.NextFloat(2, 3)).customData = 1;
            Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(7, 2), 0, Color.White, Scale: Main.rand.NextFloat(1, 2));
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(15, 15), Scale: Main.rand.NextFloat(0.5f, 1));

            if (Projectile.timeLeft % 2 == 0)
                Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.DarkOrchid, Scale: Main.rand.NextFloat(0.1f, .15f));
            if (Projectile.timeLeft <= 320)
                Projectile.ai[0]++;
            if (Projectile.ai[0] == 40)
            {
                savedDir = new Vector2(Main.rand.NextFloat(-1, 1), 1);
                savedP = Main.rand.NextVector2FromRectangle(Projectile.getRect());
                Projectile.NewProjectile(null, savedP, savedDir, ModContent.ProjectileType<XTelegraphLine>(), 0, 0);
            }
            if (Projectile.ai[0] > 75)
            {
                Projectile.NewProjectile(null, savedP, savedDir, ModContent.ProjectileType<XLightningBolt>(), 20, 0);
                Projectile.ai[0] = 0;
            }
        }
    }
}
