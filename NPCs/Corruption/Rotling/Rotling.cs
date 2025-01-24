using EbonianMod.Items.Consumables.Food;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.Rotling
{
    public class RotlingHead : WormHead
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/Rotling/Rotling",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<VileNoodleBox>(), 50, 1, 3));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt ? 0.22f : 0;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("The Rotling is a small worm birthed from the rotting residual energy left by long dead corruption creatures."),
            });
        }
        public override void OnKill()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override int BodyType => NPCType<RotlingBody>();
        public override int TailType => NPCType<RotlingTail>();
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(10, 14);
            NPC.damage = 0;
            NPC.aiStyle = -1;
            NPC.lifeMax = 15;
            NPC.value = Item.buyPrice(0, 0, 0, 1);
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
        }
        public override void Init()
        {
            MinSegmentLength = 5;
            MaxSegmentLength = 10;

            MoveSpeed = 13.5f;
            Acceleration = 0.1f;
        }
    }
    public class RotlingBody : WormBody
    {
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(10, 8);
            NPC.damage = 1;
            NPC.aiStyle = -1;
            NPC.lifeMax = 15;
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
        }
        public override void Init()
        {
            MoveSpeed = 13.5f;
            Acceleration = 0.1f;
        }
    }
    public class RotlingTail : WormTail
    {
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(10, 8);
            NPC.damage = 1;
            NPC.aiStyle = -1;
            NPC.lifeMax = 15;
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
        }
        public override void Init()
        {
            MoveSpeed = 13.5f;
            Acceleration = 0.1f;
        }
    }
}
