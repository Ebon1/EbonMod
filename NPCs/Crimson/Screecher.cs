using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Projectiles.Cecitior;

namespace EbonianMod.NPCs.Crimson
{
    public class Screecher : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("Medium sized, skinless creatures that roam the crimson as walking alarms of a sort. Their nerves are nearly exposed, allowing them to sense even the slightest disturbance and alert the crimson to it."),
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson)
            {
                return .07f;
            }
            else
            {
                return 0;
            }
        }
        public override void SetDefaults()
        {
            NPC.width = 44;
            NPC.height = 72;
            NPC.damage = 15;
            NPC.defense = 8;
            NPC.lifeMax = 75;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WalkerGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WalkerGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WalkerGore3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WalkerGore4").Type, NPC.scale);
            return true;
        }

        private const int Idle = 0;
        private const int Walk = 1;
        private const int Attack = 2;
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (AIState == Walk)
            {
                if (!NPC.velocity.X.CloseTo(0, 0.5f) && NPC.velocity.Y.CloseTo(0))
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y < 7 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                }
                else
                    NPC.frame.Y = 7 * frameHeight;
            }
            else
            {
                if (AITimer < 100)
                {
                    if (NPC.frame.Y < 7 * frameHeight)
                        NPC.frame.Y = 7 * frameHeight;
                    if (NPC.frameCounter % 5 == 0)
                    {

                        if (NPC.frame.Y < 10 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 9 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.frameCounter % 5 == 0)
                    {

                        if (NPC.frame.Y > 7 * frameHeight)
                            NPC.frame.Y -= frameHeight;
                    }
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (AIState == Idle)
            {
                NPC.TargetClosest(true);
                if (NPC.HasValidTarget)
                {
                    AIState = Walk;
                    AITimer = 0;
                }
            }
            else if (AIState == Walk)
            {
                AITimer2++;
                AITimer = MathHelper.Clamp(AITimer, 0, 500);
                if (AITimer2 % 7 == 0)
                    NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

                NPC.spriteDirection = NPC.direction;

                if (player.Center.Distance(NPC.Center) > 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100))
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 1.7f, 0.1f);
                if (player.Center.Distance(NPC.Center) < 100)
                {
                    AITimer += 4;
                    NPC.velocity.X *= 0.9f;
                    NPC.frame.Y = 7 * 72;
                    NPC.frameCounter = 1;
                }
                if ((player.Center.Y - NPC.Center.Y > 100 || player.Center.Y - NPC.Center.Y < -100))
                {
                    AITimer--;
                }

                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

                if (NPC.collideY && NPC.collideX)
                {
                    NPC.velocity.Y = -6;
                }


                if (AITimer >= 100)
                {
                    NPC.velocity.X = 0;
                    AIState = Attack;
                    AITimer = 0;
                }
            }
            else if (AIState == Attack)
            {
                AITimer++;
                if (AITimer == 15)
                {
                    SoundEngine.PlaySound(EbonianSounds.shriek, NPC.Center);
                }
                if (AITimer % 5 == 0 && AITimer > 15 && AITimer < 100)
                {
                    Projectile.NewProjectileDirect(null, NPC.Center - new Vector2(NPC.direction * -10, 8), new Vector2(NPC.direction * Main.rand.NextFloat(2, 6), 0).RotatedByRandom(MathHelper.PiOver4), ProjectileID.BloodNautilusShot, 17, 0).tileCollide = true;
                }
                if (player.Center.Distance(NPC.Center) > 100 && AITimer < 100)
                    AITimer += 2;

                if (AITimer >= 120)
                {
                    AIState = Walk;
                    AITimer = 0;
                }
            }
        }
    }
}
