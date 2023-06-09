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
    public class EbonCrawler : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("The Waster is a product of the Corruption’s failures. It is an evolutionary mishap, that’s only purpose now is to be fed upon and recycled into microbes."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 38;
            NPC.damage = 15;
            NPC.defense = 3;
            NPC.lifeMax = 60;
            NPC.aiStyle = 3;
            AIType = 218;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
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
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonCrawlerGore1").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonCrawlerGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonCrawlerGore3").Type, NPC.scale);
            return true;
        }
        public float AIState;
        public float AITimer;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AIState);
            writer.Write(AITimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AIState = reader.ReadSingle();
            AITimer = reader.ReadSingle();
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        int walk = 0, attack = 1;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = NPC.direction;
            if (AIState == walk)
            {
                if (player.Center.Distance(NPC.Center) < 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100) && NPC.collideY)
                {
                    AITimer++;
                    NPC.velocity.X *= 0.8f;
                    if (AITimer >= 30)
                    {
                        AIState = attack;
                        NPC.velocity = Vector2.Zero;
                        AITimer = 0;
                    }
                }
            }
            else
            {
                NPC.direction = player.Center.X < NPC.Center.X ? -1 : 1;
                NPC.velocity.X = 0;
                NPC.aiStyle = -1;
                AIType = -1;
                AITimer++;
                if (AITimer == 120)
                {
                    NPC.velocity = new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 10, -3);
                    NPC.aiStyle = 3;
                    AIType = 218;
                    AIState = walk;
                    AITimer = 0;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.IsABestiaryIconDummy)
            {
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
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                if (AIState == walk)
                {
                    if (NPC.collideY)
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
                        else
                        {
                            NPC.frameCounter = 0;
                        }
                }
                else
                {
                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
                }
            }
        }
    }
}