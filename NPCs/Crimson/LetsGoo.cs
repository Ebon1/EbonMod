using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using Terraria.GameContent.ItemDropRules;

namespace EbonianMod.NPCs.Crimson
{
    public class LetsGoo : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 78;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 120;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 100f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.buffImmune[BuffID.Ichor] = true;
            NPC.noTileCollide = true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<Items.Weapons.Melee.SanguineSlasher>(), 50));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCrimson ? 0.05f : 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("The Gnasher is nothing but muscle, jaws, and the instinctual desire to devour prey of any kind."),
            });
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (AIState == 0)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < frameHeight * 9)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            else if (AIState == 1)
            {
                if (NPC.frameCounter % 10 == 0)
                {
                    NPC.frame.Y = frameHeight * 10;
                }
                if (NPC.frameCounter % 10 == 5)
                {
                    NPC.frame.Y = frameHeight * 11;
                }
            }
            else
                NPC.frame.Y = frameHeight * 12;
        }
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
        public override bool CheckDead()
        {
            for (int i = 0; i < 6; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/Gnasher" + i).Type, NPC.scale);
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            switch (AIState)
            {
                case 0:
                    NPC.damage = 0;
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
                    AITimer++;
                    if (AITimer < 300)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
                    if (AITimer > 300)
                        NPC.velocity *= 0.9f;
                    if (AITimer >= 330)
                    {
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    NPC.damage = 40;
                    if (AITimer < 10)
                        NPC.rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
                    else
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
                    if (AITimer == 10)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 15;
                    if (AITimer > 30)
                    {
                        SoundEngine.PlaySound(EbonianSounds.chomp2.WithVolumeScale(0.5f), NPC.Center);
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 2:
                    NPC.damage = 0;
                    NPC.velocity *= 0.95f;
                    AITimer++;
                    if (AITimer > 25)
                    {
                        AIState = 0;
                        AITimer = -200;
                    }
                    break;
            }
        }
    }
}
