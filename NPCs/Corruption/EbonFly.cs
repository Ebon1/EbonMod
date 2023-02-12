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
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/EbonFly_Glow");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 32;
            NPC.height = 30;
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
            NPC.velocity = Main.rand.NextVector2Unit() * 5;
            if (NPC.lifeMax != 450 && NPC.lifeMax != 200)
                NPC.lifeMax = 30;
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
