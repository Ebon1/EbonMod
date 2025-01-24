using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace EbonianMod.NPCs.Crimson
{
    public class Mosquito : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("A once nuisance turned into another monster. These creatures' bloodsucking ability makes them annoying alone, but dangerous in swarms, or for what lasting side effects they may bring."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("NPCs/Crimson/Mosquito_Glow");
            Texture2D tex2 = Helper.GetTexture("NPCs/Crimson/Mosquito");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Bleeding, Main.rand.Next(100, 500));
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCrimson)
            {
                return .45f;
            }
            else
            {
                return 0;
            }
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 24;
            NPC.height = 24;
            NPC.npcSlots = 0.1f;
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
                NPC.frame.Y = 0 * frameHeight;
            else if (NPC.frameCounter < 10)
                NPC.frame.Y = 1 * frameHeight;
            else if (NPC.frameCounter < 15)
                NPC.frame.Y = 2 * frameHeight;
            else
                NPC.frameCounter = 0;
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
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                    {
                        NPC.velocity += NPC.Center.FromAToB(npc.Center, true, true) * 0.5f;
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
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WormyGore3").Type, NPC.scale);
            return true;
        }
    }
}
