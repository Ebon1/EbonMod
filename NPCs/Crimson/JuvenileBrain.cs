using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson
{
    public class JuvenileBrain : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 50;
            NPC.aiStyle = 5;
            AIType = 205;
            NPC.damage = 20;
            NPC.defense = 1;
            NPC.lifeMax = 80;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Microsoft.Xna.Framework.Color color9 = Lighting.GetColor((int)((double)NPC.position.X + (double)NPC.width * 0.5) / 16, (int)(((double)NPC.position.Y + (double)NPC.height * 0.5) / 16.0));
            Vector2 orig = new Vector2((float)(ModContent.Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Width / 2), (float)(ModContent.Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Height / Main.npcFrameCount[NPC.type] / 2));
            SpriteEffects spriteEffects2;
            Microsoft.Xna.Framework.Rectangle frame6 = NPC.frame;
            Color alpha15 = NPC.GetAlpha(color9);
            float num212 = 1f - (float)NPC.life * 2 / (float)NPC.lifeMax;
            num212 *= num212;
            alpha15 = Color.White;
            for (int num213 = 0; num213 < 4; num213++)
            {
                Vector2 position9 = NPC.position;
                float num214 = Math.Abs(NPC.Center.X - Main.player[Main.myPlayer].Center.X);
                float num215 = Math.Abs(NPC.Center.Y - Main.player[Main.myPlayer].Center.Y);
                if (num213 == 0 || num213 == 2)
                {
                    position9.X = Main.player[Main.myPlayer].Center.X + num214;
                    spriteEffects2 = SpriteEffects.None;
                }
                else
                {
                    position9.X = Main.player[Main.myPlayer].Center.X - num214;
                    spriteEffects2 = SpriteEffects.FlipHorizontally;
                }
                position9.X -= (float)(NPC.width / 2);
                Main.spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value, new Vector2(position9.X - pos.X + (float)(NPC.width / 2) - (float)ModContent.Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Width * NPC.scale / 2f + orig.X * NPC.scale, position9.Y - pos.Y + (float)NPC.height - (float)ModContent.Request<Texture2D>("EbonianMod/NPCs/Crimson/JuvenileBrain").Value.Height * NPC.scale / (float)Main.npcFrameCount[NPC.type] + 4f + orig.Y * NPC.scale + NPC.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), alpha15, NPC.rotation * .1f, orig, NPC.scale, spriteEffects2, 0);
            }
            return false;
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
        public override void FindFrame(int frameHeight)
        {
            if (AITimer <= 390 && AITimer <= 440 || NPC.IsABestiaryIconDummy)
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
                else if (NPC.frameCounter < 15)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else if (NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
            else
            {
                NPC.frameCounter++;
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
            NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
            AITimer++;
            if (AITimer >= 370)
            {
                if (AITimer >= 395 && AITimer < 410)
                {
                    NPC.velocity += Helper.FromAToB(NPC.Center, player.Center);
                    SoundEngine.PlaySound(SoundID.NPCDeath2, NPC.Center);
                }
                if (AITimer > 435)
                    NPC.velocity *= 0.95f;
                if (AITimer >= 450)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }

        }
    }
}
