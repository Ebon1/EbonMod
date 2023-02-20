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
    public class EbonFly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebon Fly");
            Main.npcFrameCount[NPC.type] = 2;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("These flies are pretty harmless on their own but when they're in huge packs, combined with other enemies, they can create some intense situations."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D tex = Helper.GetTexture("NPCs/Corruption/EbonFly_Glow");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/EbonFly");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, null, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 28;
            NPC.height = 24;
            NPC.lifeMax = 30;
            NPC.damage = 12;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[24] = true;
            NPC.noTileCollide = false;
            NPC.defense = 0;
        }
        public override void FindFrame(int frameHeight)
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
        public override void OnSpawn(IEntitySource source)
        {
            NPC.scale = Main.rand.NextFloat(0.8f, 1.2f);
            NPC.velocity = Main.rand.NextVector2Unit() * 5;
        }
        public override void PostAI()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                    {
                        NPC.velocity += Helper.FromAToB(NPC.Center, npc.Center, true, true) * 0.5f;
                    }
                    if (npc.Center == NPC.Center)
                    {
                        NPC.velocity = Main.rand.NextVector2Unit() * 5;
                    }
                }
            }
            if (NPC.lifeMax == 450 || NPC.lifeMax == 200)
                NPC.life--;
            NPC.checkDead();
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
            return true;
        }
    }
}
