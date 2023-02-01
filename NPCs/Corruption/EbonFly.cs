using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace ExolRebirth.NPCs.Corruption
{
    public class EbonFly : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebon Fly");
            Main.npcFrameCount[NPC.type] = 2;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.width = 32;
            NPC.height = 32;
            NPC.lifeMax = 30;
            NPC.damage = 12;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[24] = true;
            NPC.noTileCollide = true;
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
        public override void PostAI()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == NPC.type && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < NPC.width)
                    {
                        NPC.velocity += Helper.FromAToB(NPC.Center, npc.Center, true, true) * 0.5f;
                    }
                }
            }
            if (NPC.lifeMax == 450 || NPC.lifeMax == 200)
                NPC.life--;
            NPC.checkDead();
        }
        public override bool CheckDead()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
            return true;
        }
    }
}
