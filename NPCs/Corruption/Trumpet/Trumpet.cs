﻿using EbonianMod.Common.Systems;
using EbonianMod.NPCs.Crimson.CrimsonWorm;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.Trumpet
{
    public class TrumpetHead : WormHead
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/Trumpet/TrumpetBestiary",
                PortraitPositionXOverride = -200
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("A trumpet's job within the corruption macroorganism is to tend to the fauna below it. Unlike most infected creatures, they seem completely unbothered by your presence."),
            });
        }
        public override bool CheckDead()
        {
            for (int j = 0; j < 3; j++)
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs" + i).Type, NPC.scale);
            return true;
        }
        public override int TailType => ModContent.NPCType<TrumpetTail>();
        public override int BodyType => ModContent.NPCType<TrumpetBody>();
        public override bool extraAiAsIndex => true;
        public override bool useNormalMovement => false;
        public override void ExtraAI()
        {
            if (NPC.ai[2] == 0)
                NPC.ai[2] = NPC.Center.X - Main.player[NPC.target].Center.X > 0 ? -1 : 1;
            NPC.timeLeft = 10;
            NPC.direction = NPC.spriteDirection = NPC.ai[2] < 0 ? 1 : -1;
            NPC.ai[3] += 0.05f;
            if (NPC.Center.X - Main.player[NPC.target].Center.X > 3000)
            {
                NPC.ai[2] = MathHelper.Lerp(NPC.ai[2], -1f, Acceleration);
            }
            if (NPC.Center.X - Main.player[NPC.target].Center.X < -3000)
            {
                NPC.ai[2] = MathHelper.Lerp(NPC.ai[2], 1f, Acceleration);
            }
            NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            /*if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 1200) > 1000)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(NPC.ai[2] * MoveSpeed, MoveSpeed), Acceleration);
            }
            else */
            if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 1200) < 200)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(NPC.ai[2] * MoveSpeed, -MoveSpeed), Acceleration);
            }
            else
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(NPC.ai[2] * MoveSpeed, MathF.Sin(NPC.ai[3]) * MoveSpeed * NPC.ai[2]), Acceleration);
        }
        public override void SetDefaults()
        {
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 1500;
            NPC.defense = 20;
            NPC.Size = new Vector2(58, 78);
            NPC.aiStyle = -1;
        }
        public override void Init()
        {
            MinSegmentLength = 15;
            MaxSegmentLength = 15;
            CanFly = true;
            MoveSpeed = 5.5f;
            Acceleration = 0.15f;
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt ? 0.05f : 0;
        }
    }
    public class TrumpetBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override void SetDefaults()
        {
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 1500;
            NPC.defense = 20;
            NPC.Size = new Vector2(58, 78);
            NPC.aiStyle = -1;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 || !NPC.active)
                for (int j = 0; j < 3; j++)
                    for (int i = 0; i < 5; i++)
                        Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs" + i).Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            NPC.timeLeft = 10;
            NPC.direction = NPC.spriteDirection = HeadSegment.ai[2] < 0 ? 1 : -1;
            if (++NPC.ai[2] % 200 == 100 + NPC.ai[3] * 2)
            {
                if (NPC.ai[3] == 0)
                {
                    SoundEngine.PlaySound(EbonianSounds.trumpet, NPC.Center);
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * 5, ProjectileID.CursedFlameHostile, 30, 0);
            }
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.15f;
        }
    }
    public class TrumpetTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 1500;
            NPC.defense = 20;
            NPC.Size = new Vector2(58, 78);
            NPC.aiStyle = -1;
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 || !NPC.active)
                for (int j = 0; j < 3; j++)
                    for (int i = 0; i < 5; i++)
                        Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs" + i).Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            NPC.timeLeft = 10;
            NPC.direction = NPC.spriteDirection = HeadSegment.ai[2] < 0 ? 1 : -1;
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.15f;
        }
    }
}
