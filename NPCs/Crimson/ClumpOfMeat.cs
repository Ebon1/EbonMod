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
using Terraria.GameContent;
using EbonianMod.Projectiles.VFXProjectiles;
namespace EbonianMod.NPCs.Crimson
{
    public class ClumpOfMeat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creatures"),
                new FlavorTextBestiaryInfoElement("Latchers are surprisingly clever entities, often bunching up in order to mimic other things, only to burst apart and bombard the enemy all at once. These usually come in the form of slimes, but a few rare cases have detailed them taking humanoid shapes."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 8;
            NPC.lifeMax = 300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        public override bool CheckDead()
        {
            for (int k = 0; k < 2; k++)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<Parasite>());
            }
            return true;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight)
            {
                return .15f;
            }
            else
            {
                return 0;
            }
        }
        Vector2 scale;
        float AITimer;
        public override void AI()
        {
            Player Player = Main.player[NPC.target];

            if (NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) < 50f)
            {
                if (AITimer == 0) AITimer++;
            }
            if (AITimer > 0)
            {
                NPC.aiStyle = -1;
                NPC.velocity.X *= 0.9f;
                AITimer++;
                scale.X = MathHelper.Clamp(MathHelper.Lerp(scale.X, MathF.Cos(4.7124f + (AITimer * 7)) * 0.3f, 0.15f + (AITimer * 0.002f)), -0.1f, 0.4f);
                scale.Y = MathHelper.Clamp(MathHelper.Lerp(scale.X, MathF.Sin(4.7124f - (AITimer * 8)) * 0.4f, 0.1f + (AITimer * 0.002f)), -0.1f, 0.4f);
                if (AITimer > 60)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        Projectile.NewProjectile(null, Main.rand.NextVector2FromRectangle(NPC.getRect()), new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-7, -4)), ProjectileType<AmbientGibs>(), 0, 0);
                    }
                    for (int k = 0; k < 5; k++)
                    {
                        Gore.NewGore(NPC.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_FromThis(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/WormyGore3").Type, NPC.scale);
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<Parasite>());
                    }
                    NPC.SimpleStrikeNPC(NPC.lifeMax * 2, 0);
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            spriteBatch.Draw(tex, NPC.Center - new Vector2(0, scale.Y * 1.1f) - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, new Vector2(NPC.scale) + scale, SpriteEffects.None, 0);
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
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
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                NPC.frame.Y = 2 * frameHeight;
            }
        }
    }
}