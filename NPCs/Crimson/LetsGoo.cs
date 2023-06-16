using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace EbonianMod.NPCs.Crimson
{
    public class LetsGoo : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 78;
            NPC.damage = 0;
            NPC.defense = 1;
            NPC.lifeMax = 60;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (AIState == 0)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < frameHeight * 9)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            else if (AIState == 1)
            {
                if (NPC.frameCounter % 10 == 0)
                {
                    NPC.frame.Y = frameHeight * 10;
                }
                if (NPC.frameCounter % 10 == 5)
                {
                    NPC.frame.Y = frameHeight * 11;
                }
            }
            else
                NPC.frame.Y = frameHeight * 12;
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
            NPC.spriteDirection = NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            switch (AIState)
            {
                case 0:
                    AITimer++;
                    if (AITimer < 300)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 5, 0.05f);
                    if (AITimer > 300)
                        NPC.velocity *= 0.9f;
                    if (AITimer >= 330)
                    {
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer == 10)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 15;
                    if (AITimer > 30)
                    {
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 2:
                    NPC.velocity *= 0.95f;
                    AITimer++;
                    if (AITimer > 25)
                    {
                        AIState = 0;
                        AITimer = -200;
                    }
                    break;
            }
        }
    }
}
