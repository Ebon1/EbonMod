using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.NPCs.Corruption
{
    public class VileSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Slime");
            Main.npcFrameCount[NPC.type] = 20;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("This infected slime-like creature can split open and reveal a tentacle made of cursed flames."),
            });
        }

        public override void SetDefaults()
        {
            NPC.width = 96;
            NPC.height = 64;
            NPC.aiStyle = 0;
            NPC.damage = 7;
            NPC.defense = 5;
            NPC.lifeMax = 75;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 25f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Confused] = false;

        }


        /// <summary>
        /// Whether to target the player and jump towards it. Defaults to how most slimes aggro (when its night time, or if it has taken damage, if its underground, or its a slime rain)
        /// </summary>
        /// <returns></returns>
        public virtual bool Aggro() => !Main.dayTime || NPC.life != NPC.lifeMax || NPC.position.Y > Main.worldSurface * 16.0 || Main.slimeRain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The speed in the X direction</param>
        /// <param name="y">The speed in the Y direction</param>
        public virtual void SetSpeed(ref float x, ref float y, int phase)
        {
        }

        /// <summary>
        /// How much to add the the jump timer. Defaults to 1f when not aggro, and 2f when aggro
        /// </summary>
        /// <param name="aggro"></param>
        /// <returns></returns>
        public virtual float JumpTimerAdd(bool aggro) => aggro ? 4f : 4f;

        private const int AIStateSlot = 0;
        private const int AITimerSlot = 1;


        private const int Asleep = 0;
        private const int Notice = 1;
        private const int Jump = 2;
        private const int Attack = 3;
        private const int JumpAgain = 4;
        private const int Fall = 5;
        private const int FallAgain = 6;
        public float rotation = 0;
        public int timer = 0;
        public float int1 = 0;
        public float int2 = 0;
        public float int3 = 0;
        public float AIState
        {
            get => NPC.ai[AIStateSlot];
            set => NPC.ai[AIStateSlot] = value;
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/VileSlimeGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/VileSlimeGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/VileSlimeGore3").Type, NPC.scale);
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt)
            {
                return .35f;
            }
            else
            {
                return 0;
            }
        }
        public float AITimer
        {
            get => NPC.ai[AITimerSlot];
            set => NPC.ai[AITimerSlot] = value;
        }
        /*public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            if (Main.rand.Next(30) == 0)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Ranged.EbonianGatling>());
            }
        }*/
        public override void AI()
        {
            if (AIState == Asleep)
            {
                NPC.TargetClosest(true);
                if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 1000f)
                {
                    AIState = Notice;
                    AITimer = 0;
                }
            }
            else if (AIState == Notice)
            {
                if (Main.player[NPC.target].Distance(NPC.Center) < 2500f)
                {
                    AITimer++;
                    if (AITimer >= 20)
                    {
                        AIState = Jump;
                        AITimer = 0;
                    }
                }
                else
                {
                    NPC.TargetClosest(true);
                    if (!NPC.HasValidTarget || Main.player[NPC.target].Distance(NPC.Center) > 500f)
                    {
                        AIState = Asleep;
                        AITimer = 0;
                    }
                }
            }
            else if (AIState == Jump)
            {
                AITimer++; bool aggro = Aggro();
                if (int2 > 1f)
                    int2 -= 1f;
                if (NPC.wet)
                {
                    if (NPC.collideY)
                        NPC.velocity.Y = -2f;
                    if (NPC.velocity.Y < 0 && int3 == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        int2 = 200f;
                    }
                    if (NPC.velocity.Y > 0)
                        int3 = NPC.position.X;
                    {
                        if (NPC.velocity.Y > 2f)
                            NPC.velocity.Y *= 0.9f;
                        NPC.velocity.Y -= 0.5f;
                        if (NPC.velocity.Y < -4f)
                            NPC.velocity.Y = -4f;
                    }
                    if (int2 == 1f && aggro)
                        NPC.TargetClosest();
                }
                NPC.aiAction = 0;
                if (int2 == 0)
                {
                    int1 = -100f;
                    int2 = 1f;
                    NPC.TargetClosest();
                }
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.collideY && NPC.oldVelocity.Y != 0 && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + NPC.direction;
                    if (int3 == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        int2 = 200f;
                    }
                    int3 = 0;
                    NPC.velocity.X *= 0.8f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0;
                    int1 += JumpTimerAdd(aggro);
                    int jumpPhase = 0;
                    if (int1 >= 0)
                        jumpPhase = 1;
                    if (int1 >= -1000f && int1 <= -500f)
                        jumpPhase = 2;
                    if (int1 >= -2000f && int1 <= -1500f)
                        jumpPhase = 3;
                    if (jumpPhase > 0)
                    {
                        NPC.netUpdate = true;
                        if (aggro && int2 == 1f)
                            NPC.TargetClosest();
                        if (jumpPhase == 3)
                        {
                            float x = 2f;
                            float y = 8f;
                            SetSpeed(ref x, ref y, jumpPhase);
                            NPC.velocity.Y = -y;
                            NPC.velocity.X += x * NPC.direction;
                            int1 = -200f;
                            int3 = NPC.position.X;
                        }
                        else
                        {
                            float x = 2f;
                            float y = 8f;
                            SetSpeed(ref x, ref y, jumpPhase);
                            NPC.velocity.Y = -y;
                            NPC.velocity.X += x * NPC.direction;
                            int1 = -120f;
                            if (jumpPhase == 1)
                                int1 -= 1000f;
                            else
                            {
                                int1 -= 2000f;
                            }
                        }
                    }
                    else if (int1 >= -30f)
                    {
                        NPC.aiAction = 1;
                    }
                }
                else if (NPC.target < 255 && (NPC.direction == 1 && NPC.velocity.X < 3f || NPC.direction == -1 && NPC.velocity.X > -3f))
                {
                    if (NPC.collideX && Math.Abs(NPC.velocity.X) == 0.2f)
                        NPC.position.X -= 1.4f * NPC.direction;
                    if (NPC.collideY && NPC.oldVelocity.Y != 0 && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + NPC.direction;
                    if (NPC.direction == -1 && NPC.velocity.X < 0.01 || NPC.direction == 1 && NPC.velocity.X > -0.01)
                        NPC.velocity.X += 0.2f * NPC.direction;
                    else
                    {
                        NPC.velocity.X *= 0.93f;
                    }
                }
                if (AITimer > 400)
                {
                    AIState = Attack;
                    AITimer = 0;
                }
            }
            else if (AIState == Attack)
            {
                AITimer++;
                NPC.aiStyle = 0;
                rotation += 22.5f;
                timer++;
                Vector2 velocity = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(rotation));
                if (timer >= 10)
                {
                    if (velocity.Length() < 3) velocity = Vector2.Normalize(velocity) * 3f;
                    {
                        int damage = ((Main.expertMode) ? 5 : 7);
                        int projInt = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity, 95, damage, 0, 0, 1);
                        Main.projectile[projInt].tileCollide = false;
                        Main.projectile[projInt].friendly = false;
                        Main.projectile[projInt].hostile = true;
                        Main.projectile[projInt].timeLeft = 203;
                    }
                    timer = 0;
                }

                if (AITimer >= 215)
                    NPC.frame.Y = 19 * 66;
                if (AITimer >= 220)
                {
                    AIState = Jump;
                    AITimer = 0;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            if (AIState == Asleep)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else if (AIState == Notice)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else if (AIState == Jump)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else if (AIState == Attack)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.frameCounter < 50)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else if (NPC.frameCounter < 55)
                {
                    NPC.frame.Y = 13 * frameHeight;
                }
                else if (NPC.frameCounter < 60)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (NPC.frameCounter < 65)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else if (NPC.frameCounter < 70)
                {
                    NPC.frame.Y = 16 * frameHeight;
                }
                else if (NPC.frameCounter < 75)
                {
                    NPC.frame.Y = 17 * frameHeight;
                }
                else if (NPC.frameCounter < 80)
                {
                    NPC.frame.Y = 18 * frameHeight;
                }
                /*else if (NPC.frameCounter < 85)
                {
                    NPC.frame.Y = 19 * frameHeight;
                }*/
                else
                {
                    NPC.frameCounter = 20;
                }
            }
            else if (AIState == Fall)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
    }
}