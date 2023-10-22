using EbonianMod.Items.Accessories;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Projectiles.Friendly.Crimson
{
    public class CrimCannonP : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 34;
            Projectile.aiStyle = 0;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 290;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit") { PitchVariance = 0.2f }, Projectile.Center);
            for (int i = 0; i < 19; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextVector2Unit());
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
            {
                Projectile.ai[2] = 2;
                Projectile.ai[1] = target.whoAmI;
                Projectile.ai[0] = target.type;
                Projectile.localAI[0] = Helper.FromAToB(target.Center, Projectile.Center, false).X;
                Projectile.localAI[1] = Helper.FromAToB(target.Center, Projectile.Center, false).Y;
                SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit") { PitchVariance = 0.2f }, Projectile.Center);
                for (int i = 0; i < 7; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(Main.rand.NextFloat(5, 10), 0).RotatedBy(Projectile.velocity.RotatedByRandom(MathHelper.PiOver2).ToRotation()), ModContent.ProjectileType<Gibs>(), Projectile.damage / 2, 0, Projectile.owner);
                }
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[2] == 0;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[2] == 0)
            {
                for (int i = 0; i < 7; i++)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, -2)), ModContent.ProjectileType<Gibs>(), Projectile.damage / 2, 0, Projectile.owner);
                }
                Projectile.velocity = Vector2.Zero;
                Projectile.Center += oldVelocity;
                Projectile.ai[2] = 1;
                SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit"), Projectile.Center);
            }
            return false;
        }
        public override void AI()
        {
            if (Projectile.ai[2] == 2)
            {
                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (target != null && target.active && target.type == Projectile.ai[0])
                {
                    Projectile.Center = target.Center + new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                    if (Projectile.timeLeft % 10 == 0)
                        target.SimpleStrikeNPC(5, 0, false);
                }
            }
        }
        public override void PostAI()
        {
            if (Projectile.ai[2] == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
