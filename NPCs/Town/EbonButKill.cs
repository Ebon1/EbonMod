using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using EbonianMod.Common.Systems;

namespace EbonianMod.NPCs.Town
{
    [AutoloadHead]
    public class EbonButKill : ModNPC
    {
        public override string Texture => "EbonianMod/NPCs/Town/EbonHimself";
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
            NPC.Happiness.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Love);
            NPC.Happiness.SetNPCAffection(NPCID.Wizard, AffectionLevel.Like);
            NPC.Happiness.SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Dislike);
            NPC.Happiness.SetNPCAffection(NPCID.BestiaryGirl, AffectionLevel.Hate);
            NPC.Happiness.SetBiomeAffection<DesertBiome>(AffectionLevel.Love);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.friendly = false;
            NPC.width = 38;
            NPC.height = 30;
            NPC.aiStyle = 7;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = int.MaxValue;
            NPC.HitSound = EbonianSounds.None;
            NPC.DeathSound = EbonianSounds.None;
            NPC.knockBackResist = 0.5f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Type: Ebon"),
                new FlavorTextBestiaryInfoElement("Someone once decided to put an Agal on a cat, and it all went downhill from that point on")
            });
        }
        public override ITownNPCProfile TownNPCProfile()
        {
            return new EbonProfile();
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>
            {
                "Ebon"
            };
        }
        public override void AI()
        {
            NPC.direction = NPC.spriteDirection = Main.LocalPlayer.Center.X > NPC.Center.X ? 1 : -1;
        }
        public override bool CheckDead()
        {
            CombatText.NewText(NPC.getRect(), Color.Red, "ouch owie oof my bones", true);
            NPC.life = NPC.lifeMax;
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y < -1 || NPC.velocity.Y > 1)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            else if (NPC.velocity.X > 0 || NPC.velocity.X < 0)
            {
                if (++NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else if (NPC.frame.Y >= 9 * frameHeight || NPC.frame.Y < 2 * frameHeight)
                        NPC.frame.Y = 2 * frameHeight;
                }
            }
            else
                NPC.frame.Y = 0;
        }
    }
}