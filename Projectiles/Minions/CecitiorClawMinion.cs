using EbonianMod.Common.Systems.Misc;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.Projectiles.Minions
{
    public class CecitiorClawMinion : ModProjectile
    {
        public override string Texture => "EbonianMod/NPCs/Crimson/Fleshformator/Fleshformator_Hook1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.Size = Vector2.One * 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        Verlet verlet;
        public override bool ShouldUpdatePosition() => false;
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new Verlet(Projectile.Center, 6, 40, -1, stiffness: 100);
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            if (player.dead)
            {
                modPlayer.cClawMinion = false;
            }
            if (modPlayer.cClawMinion)
            {
                Projectile.timeLeft = 2;
            }
            Vector2 targetPos = Projectile.position;
            Vector2 targetVel = Projectile.velocity;
            int index = -1;
            float targetDist = 400;
            bool target = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                //if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                {
                    targetDist = Vector2.Distance(Projectile.Center, targetPos);
                    targetPos = npc.Center;
                    targetVel = npc.velocity;
                    index = npc.whoAmI;
                    target = true;
                }
            }
            else
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC npc = Main.npc[k];
                    if (npc.CanBeChasedBy(this, false))
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        if ((distance < targetDist || !target))// && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            targetDist = distance;
                            targetPos = npc.Center;
                            targetVel = npc.velocity;
                            index = npc.whoAmI;
                            target = true;
                        }
                    }
                }
            }
            if (target)
            {
                UnifiedRandom rand = new(Projectile.whoAmI);
                Projectile.ai[2]++;
                if (Projectile.ai[2] < 20)
                {
                    savedP = Projectile.Center;
                    savedP2 = targetPos + targetVel;
                    Projectile.Center = Vector2.Lerp(savedP, savedP2 - new Vector2(100).RotatedBy(rand.NextFloat(MathHelper.TwoPi)), 0.2f);
                }

                if (Projectile.ai[2] == 20)
                {
                    Projectile p = Projectile.NewProjectileDirect(null, savedP + Helper.FromAToB(savedP, savedP2) * 110, Helper.FromAToB(savedP, savedP2), ModContent.ProjectileType<CecitiorClawSlash>(), 30, 0);
                    p.friendly = true;
                    p.hostile = false;
                    p.DamageType = DamageClass.Summon;
                }

                if (Projectile.ai[2] > 20 && Projectile.ai[2] < 55)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, savedP2 + Helper.FromAToB(savedP, savedP2) * 130, 0.1f);
                }
                if (Projectile.ai[2] >= 55)
                {
                    Projectile.ai[2] = Main.rand.Next(-20, 0);
                }
            }
            else
            {
                Projectile.ai[2] = -Projectile.minionPos * 20;
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center + new Vector2(100, 0).RotatedBy(player.ownedProjectileCounts[Type] > 1 ? Helper.CircleDividedEqually(Projectile.minionPos, player.ownedProjectileCounts[Type]) : -1), 0.2f);
            }
            Main.NewText("raagh");
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            if (verlet != null)
            {
                verlet.Update(player.Center, Projectile.Center);
                VerletDrawData data = new VerletDrawData(
                    "NPCs/Crimson/Fleshformator/Fleshformator_Hook0",
                    _endTex: "NPCs/Crimson/Fleshformator/Fleshformator_Hook1");
                verlet.Draw(Main.spriteBatch, data);
            }
            return false;
        }
        Vector2 savedP, savedP2;
    }
}
