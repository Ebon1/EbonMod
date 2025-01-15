using Terraria;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.NPCs.Crimson
{
    public class Parasite : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("These remnants of other crimson creatures are put to better use as small vessels to bring blood to more deserving crimson entities."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 26;
            AIType = 47;
            NPC.damage = 0;
            NPC.defense = 1;
            NPC.lifeMax = 80;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 3;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore3").Type, NPC.scale);
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight)
            {
                return .5f;
            }
            else
            {
                return 0;
            }
        }
        private bool IsLeeching = false;
        float AITimer, AITimer2;
        Vector2 off;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if ((player.position - NPC.Center).LengthSquared() < 20 * 20)
            {
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " is severely anemic."), 20, 0, cooldownCounter: 0, dodgeable: false);
                Leech();
            }
            if (player.dead || !player.active)
            {
                IsLeeching = false;
            }
            if (AITimer == 0)
            {
                NPC.velocity.X = Main.rand.NextFloat(-9f, 9f);
                NPC.velocity.Y = Main.rand.NextFloat(-9f, -1.5f);
                AITimer = 1;
            }
            if (++AITimer2 >= 60)
            {
                NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                AITimer2 = 0;
            }
            if (IsLeeching)
            {
                LookAtPlayer();
                NPC.Center = new Vector2(player.Center.X, player.position.Y) + off;
                player.immuneNoBlink = true;
            }
            else
            {
                off = Helper.FromAToB(player.Center, NPC.Center).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * 18;
                NPC.rotation = 0;
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            Leech();
        }
        private void Leech()
        {
            IsLeeching = true;
        }

        private void LookAtPlayer()
        {
            Vector2 look = Main.player[NPC.target].Center - NPC.Center;
            LookInDirection(look);
        }
        private void LookInDirection(Vector2 look)
        {
            float angle = 0.5f * (float)Math.PI;
            if (look.X != 0)
            {
                angle = (float)Math.Atan(look.Y / look.X);
            }
            else if (look.Y < 0)
            {
                angle += (float)Math.PI;
            }
            if (look.X < 0)
            {
                angle += (float)Math.PI;
            }
            NPC.rotation = angle;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.velocity.Y != 0 && !IsLeeching)
            {
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 6 * frameHeight;
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
        }
    }
}