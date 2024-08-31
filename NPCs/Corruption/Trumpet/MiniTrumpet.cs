/*using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Common.Systems;

namespace EbonianMod.NPCs.Corruption.Trumpet
{
    public class MiniTrumpetHead : WormHead
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/Trumpet/MiniTrumpetBestiary",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("brumpet"),
            });
        }
        public override bool byHeight => true;
        public override int BodyType => ModContent.NPCType<MiniTrumpetBody>();
        public override int TailType => ModContent.NPCType<MiniTrumpetTail>();
        public override bool useNormalMovement => NPC.ai[3] < 300;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 36);
            NPC.damage = 0;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.behindTiles = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override void OnKill()
        {
            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            Player player = Main.player[NPC.target];
            NPC.soundDelay = 10;
            if (NPC.ai[3] == 300)
            {
                SoundEngine.PlaySound(EbonianSounds.trumpet.WithPitchOffset(0.5f).WithVolumeScale(0.7f), NPC.Center);
            }
            if (++NPC.ai[3] >= 300 && NPC.ai[3] < 400)
            {
                NPC.velocity.Y = NPC.Center.FromAToB(player.Center - new Vector2(0, 250)).Y * MoveSpeed;
                NPC.velocity.X = NPC.direction * MoveSpeed;
                if (NPC.Center.X - player.Center.X > 500)
                    NPC.direction = NPC.spriteDirection = -1;
                if (NPC.Center.X - player.Center.X < 500)
                    NPC.direction = NPC.spriteDirection = 1;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            }
            if (NPC.ai[3] > 400)
                NPC.ai[3] = -Main.rand.Next(100, 200);
        }
        public override void Init()
        {
            MinSegmentLength = 6;
            MaxSegmentLength = 10;

            MoveSpeed = 5.5f;
            Acceleration = 0.2f;

            CanFly = true;
        }
    }
    public class MiniTrumpetBody : WormBody
    {
        public override bool byHeight => true;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 36);
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.behindTiles = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life)
            {
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
            }
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
            NPC.direction = NPC.spriteDirection = HeadSegment.direction;
            if (HeadSegment.ai[3] % 30 == 0 && HeadSegment.ai[3] > 300 && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * 5, ProjectileID.CursedFlameHostile, 30, 0);
            }
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.2f;
        }
    }
    public class MiniTrumpetTail : WormTail
    {
        public override bool byHeight => true;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 36);
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.behindTiles = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }

        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life)
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
            NPC.direction = NPC.spriteDirection = HeadSegment.direction;
        }
        public override void Init()
        {
            MoveSpeed = 5.5f;
            Acceleration = 0.2f;
        }
    }
}*/
