using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using EbonianMod.Buffs;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Projectiles.Minions
{
    public class EbonFlyMinion : ModProjectile //this is literally ExampleMinion and i refuse to change anything
    {
        public override string Texture => "EbonianMod/NPCs/Corruption/EbonFly";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;

        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = Main.rand.NextFloat(0.8f, 1.2f);
        }

        public override void Kill(int timeLeft)
        {
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, Projectile.scale);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, Projectile.scale);
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool MinionContactDamage()
        {
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                Projectile.Kill();
            }

            EbonianPlayer mp = player.GetModPlayer<EbonianPlayer>();
            if (mp.ToxicGland)
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            #region General behavior
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f;
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX;
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();
            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            float overlapVelocity = 0.04f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
                    else Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
                    else Projectile.velocity.Y += overlapVelocity;
                }
            }
            #endregion

            #region Find target
            float distanceFromTarget = 700f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        bool closeThroughWall = between < 100f;
                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            Projectile.friendly = foundTarget;
            #endregion

            #region Movement

            float speed = 8f;
            float inertia = 20f;

            if (foundTarget)
            {
                Projectile.spriteDirection = targetCenter.X > Projectile.Center.X ? -1 : 1;
                Projectile.direction = targetCenter.X > Projectile.Center.X ? -1 : 1;
                if (distanceFromTarget > 40f)
                {
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {

                Projectile.spriteDirection = Main.player[Projectile.owner].Center.X > Projectile.Center.X ? -1 : 1;
                Projectile.direction = Main.player[Projectile.owner].Center.X > Projectile.Center.X ? -1 : 1;
                if (distanceToIdlePosition > 600f)
                {
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    speed = 4f;
                    inertia = 80f;
                }
                if (distanceToIdlePosition > 20f)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            #endregion

            #region Animation and visuals

            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
            #endregion
        }
    }
}