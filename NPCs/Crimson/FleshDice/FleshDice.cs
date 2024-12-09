using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.FleshDice
{
    public class FleshDice : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 23;
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(170, 118);
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.lifeMax = 1000;
            NPC.value = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.45f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }
        public override void FindFrame(int frameHeight)
        {
            if (AIState == Roll)
            {
                NPC.frame.X = 170;

                if (NPC.frameCounter % 5 == 0 && AITimer != 0)
                {
                    if (NPC.frame.Y < 22 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                    {
                        AIState = Walk;
                        AITimer = 0;
                    }
                }
            }
            else
            {
                NPC.frame.X = 0;
                if (!NPC.velocity.X.CloseTo(0, 0.5f) && NPC.velocity.Y.CloseTo(0))
                {
                    if (++NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y < 7 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                }
                else
                    NPC.frame.Y = 5 * frameHeight;
            }
        }
        public float AIState { get => NPC.ai[0]; set => NPC.ai[0] = value; }
        public float AITimer { get => NPC.ai[1]; set => NPC.ai[1] = value; }
        public float AITimer2 { get => NPC.ai[2]; set => NPC.ai[2] = value; }
        public float Rolled { get => NPC.ai[3]; set => NPC.ai[3] = value; }
        const int Walk = 1, Roll = 2;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");

            Texture2D rolled1 = Helper.GetTexture(Texture + "_1");
            Texture2D rolled2 = Helper.GetTexture(Texture + "_2");
            Texture2D rolled3 = Helper.GetTexture(Texture + "_3");
            Texture2D rolled4 = Helper.GetTexture(Texture + "_4");
            Texture2D rolled5 = Helper.GetTexture(Texture + "_5");
            Texture2D rolled6 = Helper.GetTexture(Texture + "_6");

            Vector2 origin = NPC.Size / 2;
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (AIState == 0)
            {
                NPC.TargetClosest(true);
                if (NPC.HasValidTarget)
                {
                    AIState = Walk;
                    AITimer = 0;
                }
            }
            AITimer++;
            if (AIState == Walk)
            {
                if (++AITimer2 % 7 == 0)
                    NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

                NPC.spriteDirection = NPC.direction;

                if (player.Center.Distance(NPC.Center) > 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100))
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 1.7f, 0.1f);
                if (player.Center.Distance(NPC.Center) < 100)
                {
                    AITimer += 4;
                    NPC.velocity.X *= 0.9f;
                    NPC.frame.Y = 7 * 72;
                    NPC.frameCounter = 1;
                }
                if ((player.Center.Y - NPC.Center.Y > 100 || player.Center.Y - NPC.Center.Y < -100))
                {
                    AITimer--;
                }

                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

                if (NPC.collideY && NPC.collideX)
                {
                    NPC.velocity.Y = -6;
                }


                if (AITimer >= 300)
                {
                    NPC.velocity.X = 0;
                    NPC.frame.Y = 0;
                    NPC.frame.X = 170;
                    AIState = Roll;
                    Rolled = Main.rand.Next(6);
                    for (int i = 0; i < 10; i++)
                    {
                        Gore.NewGorePerfect(null, NPC.Center, Main.rand.NextVector2Circular(8, 8), GoreID.Smoke1);
                    }
                    NPC.velocity = new Vector2(Main.rand.NextFloatDirection() * 17, Main.rand.NextFloat(-12, -8));
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == Roll)
            {
                if (NPC.Grounded(0.5f, 0.5f))
                    NPC.velocity = -NPC.velocity * 0.7f;

                NPC.velocity.X *= 0.991f;
                AITimer2 += MathHelper.ToRadians(NPC.velocity.Length() * 0.5f * (NPC.velocity.X > 0 ? 1 : -1));

                if (NPC.velocity.Length() < 2)
                {
                    AITimer++;
                    NPC.velocity.X *= 0.9f;
                }
            }
        }
    }
}
