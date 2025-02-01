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
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.DankDigger
{
    public class DankDiggerHead : WormHead
    {
        public override void SetStaticDefaults()
        {

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/DankDigger/DankDigger",
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
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.DankDiggerHead.Bestiary"),
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt && spawnInfo.Player.ZoneDirtLayerHeight ? 0.08f : 0;
        }
        public override bool byHeight => true;
        public override int BodyType => NPCType<DankDiggerBody>();
        public override int TailType => NPCType<DankDiggerTail>();
        public override bool useNormalMovement => NPC.ai[3] < 300;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 32);
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.defense = 15;
            NPC.value = Item.buyPrice(0, 0, 2);
        }
        public override void OnKill()
        {
            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            Player player = Main.player[NPC.target];
            NPC.soundDelay = 10;
            if (++NPC.ai[3] >= 300 && NPC.ai[3] < 304)
            {
                NPC.velocity += NPC.Center.FromAToB(player.Center) * 1.15f;
                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
                SoundEngine.PlaySound(SoundID.Zombie38, NPC.Center);
            }
            if (NPC.ai[3] > 340)
                NPC.ai[3] = -Main.rand.Next(100, 200);
        }
        public override void Init()
        {
            MinSegmentLength = 6;
            MaxSegmentLength = 10;

            MoveSpeed = 2.5f;
            Acceleration = 0.1f;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 2, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.WormTooth, 2, 1, 2));
        }
    }
    public class DankDiggerBody : WormBody
    {
        public override bool byHeight => true;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 18);
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
        }
        public override void Init()
        {
            MoveSpeed = 2.5f;
            Acceleration = 0.1f;
        }
    }
    public class DankDiggerTail : WormTail
    {
        public override bool byHeight => true;
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.Size = new Vector2(30, 20);
            NPC.damage = 40;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        }
        public override void ExtraAI()
        {
            NPC.soundDelay = 10;
        }
        public override void Init()
        {
            MoveSpeed = 2.5f;
            Acceleration = 0.1f;
        }
    }
}
