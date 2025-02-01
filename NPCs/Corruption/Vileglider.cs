using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.NPCs.Corruption
{
    public class Vileglider : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Vileglider.Bestiary"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 2, 1, 4));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt)
            {
                return .08f;
            }
            else
            {
                return 0;
            }
        }
        public override bool? CanFallThroughPlatforms() => true;

        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 56;
            NPC.height = 52;
            NPC.npcSlots = 0.1f;
            NPC.lifeMax = 100;
            NPC.damage = 12;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.7f;
            NPC.buffImmune[24] = true;
            NPC.noTileCollide = false;
            NPC.defense = 4;
            NPC.value = Item.buyPrice(0, 0, 10);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/Vileglider_Glow");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/Vileglider");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 5 == 0)
            {
                if ((NPC.frame.Y += frameHeight) > 7 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[3] == 0)
            {
                NPC.scale = Main.rand.NextFloat(0.8f, 1.2f);
                NPC.velocity = Main.rand.NextVector2Unit();
            }
        }
        public override void PostAI()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                    {
                        NPC.velocity += NPC.Center.FromAToB(npc.Center, true, true) * 0.1f;
                    }
                    if (npc.Center == NPC.Center)
                    {
                        NPC.velocity = Main.rand.NextVector2Unit() * 5;
                    }
                }
            }
            NPC.position += NPC.velocity * 0.5f;
        }
        public override bool CheckDead()
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                for (int j = 0; j < 3; j++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
            }
            return true;
        }
    }
}
