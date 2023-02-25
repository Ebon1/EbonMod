using Terraria;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.NPCs.Corruption
{
    public class EbonianStrider : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regurgitator");
            Main.npcFrameCount[NPC.type] = 16;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Organism"),
                new FlavorTextBestiaryInfoElement("A barrel shaped monster with confusing anatomy. Most of the creature is actually hollow, raising questions about how it manifests cursed flame at all."),
            });
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
        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 15;
            NPC.defense = 8;
            NPC.lifeMax = 70;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        /*public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            if (Main.rand.Next(35) == 0)
            {
                Item.NewItem(NPC.getRect(), ModContent.ItemType<Items.Weapons.Summoner.EaterStaff>());
            }
        }*/
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore1").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore4").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore5").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonianStriderGore6").Type, NPC.scale);
            return true;
        }
        private const int AISlot = 0;
        private const int TimerSlot = 1;

        private const int Idle = 0;
        private const int Walk = 1;
        private const int Attack = 2;
        public float rotation = 0;
        public float AIState
        {
            get => NPC.ai[AISlot];
            set => NPC.ai[AISlot] = value;
        }

        public float AITimer
        {
            get => NPC.ai[TimerSlot];
            set => NPC.ai[TimerSlot] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
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
                NPC.aiStyle = 0;
                if (AITimer2 % 5 == 0 && player.Center.Distance(NPC.Center) > 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100))
                    NPC.velocity.X = NPC.direction * 1.7f;
                if (player.Center.Distance(NPC.Center) < 100)
                {
                    AITimer++;
                    NPC.velocity.X *= 0.8f;
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if ((player.Center.Y - NPC.Center.Y > 100 || player.Center.Y - NPC.Center.Y < -100))
                {
                    AITimer--;
                    NPC.velocity.X = 0;
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if (NPC.collideY && NPC.collideX)
                {
                    NPC.velocity.Y = -8;
                }


                if (AITimer >= 100)
                {
                    NPC.velocity.X = 0;
                    NPC.frameCounter = 0;
                    AIState = Attack;
                    AITimer = 0;
                    NPC.velocity.Y = 0;
                }
            }
            else if (AIState == Attack)
            {
                AITimer++;
                rotation += 180;
                NPC.aiStyle = -1;
                Vector2 velocity = new Vector2(4, 0).RotatedBy(MathHelper.ToRadians(rotation));
                Vector2 cen = new Vector2(NPC.Center.X, NPC.Center.Y);
                if (velocity.Length() < 3) velocity = Vector2.Normalize(velocity) * 3f;
                {
                    int damage = ((Main.expertMode) ? 5 : 6);
                    int projInt = Projectile.NewProjectile(NPC.GetSource_FromAI(), cen, velocity, 101, damage, 0, 0, 1);
                    Main.projectile[projInt].tileCollide = false;
                }
                if (AITimer == 120)
                    NPC.frameCounter = 15;
                if (AITimer >= 130)
                {
                    AIState = Walk;
                    AITimer = 0;
                    NPC.frameCounter = 0;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y < 0)
                NPC.frame.Y = 0;
            else if (AIState == Walk || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else if (AIState == Attack)
            {
                if (AITimer < 120)
                    NPC.frameCounter++;
                else
                    NPC.frameCounter--;
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 13 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 14 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 15 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 15;
                }
            }
        }
    }
}