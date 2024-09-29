using EbonianMod.Common.Systems.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Crimson.BloodHunter
{
    public class BloodHunter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Crimson/BloodHunter/BloodHunter_Bestiary"
            });
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(94, 80);
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.45f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }
        public Vector2[] fgLegOffsets = new Vector2[2];
        public Vector2[] bgLegOffsets = new Vector2[2];
        public Vector2 stingerTarget;
        public Vector2 bodyOffset;
        public Vector2[] oldStingerPos = new Vector2[15];
        public bool stingerTrailActive = false;
        public Verlet tail;
        public override void OnSpawn(IEntitySource source)
        {
            tail = new Verlet(NPC.Center, 20, 6, -10, true, true, 60, false);
            stingerTarget = NPC.Center + new Vector2(30 * NPC.direction, -35);
        }
        public override bool? CanFallThroughPlatforms()
        {
            return Main.player[NPC.target].Center.Y > NPC.Center.Y + NPC.height;
        }
        public override bool CheckDead()
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk8").Type, NPC.scale);


                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/BloodHunter_FGLeg0").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center + fgLegOffsets[0], Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/BloodHunter_FGLeg1").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center + fgLegOffsets[1], Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/BloodHunter_FGLeg2").Type, NPC.scale);
            }
            if (tail != null)
            {
                for (int i = 0; i < tail.points.Count; i++)
                {
                    string tex = "BloodHunter_Tail0";
                    if (i > 1)
                        tex = "BloodHunter_Tail1";
                    if (i == tail.points.Count - 2)
                        tex = "BloodHunter_Tail2";
                    if (i == tail.points.Count - 1)
                        tex = "BloodHunter_Stinger";
                    Gore.NewGore(NPC.GetSource_Death(), tail.points[i].position, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/" + tex).Type, NPC.scale);
                }
            }
            return base.CheckDead();
        }
        int legUpdateT;
        public override void AI()
        {
            for (int num16 = oldStingerPos.Length - 1; num16 > 0; num16--)
            {
                oldStingerPos[num16] = oldStingerPos[num16 - 1];
            }
            oldStingerPos[0] = stingerTarget;

            Player player = Main.player[NPC.target];
            if (stingerTarget == Vector2.Zero)
                stingerTarget = NPC.Center + new Vector2(30 * NPC.direction, -35);
            NPC.TargetClosest(true);


            NPC.ai[1] = MathHelper.Lerp(NPC.ai[1], MathF.Sin(NPC.ai[0] * Main.rand.NextFloat(0.05f, 0.2f)) * 40, 0.1f);
            JumpCheck();
            if (NPC.Grounded())
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, MathHelper.Clamp(Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * (60 + NPC.ai[1]), false).X * 0.03f, -8, 8), 0.1f);
                if (NPC.Distance(player.Center) < 100)
                {
                    NPC.ai[0]++;
                    if (NPC.ai[0] > 85)
                        NPC.ai[0] = Main.rand.Next(20, 50);
                    if (NPC.ai[0] >= 80)
                    {
                        stingerTarget = Vector2.Lerp(stingerTarget, player.Center, 0.35f);
                        if (stingerTarget.Distance(player.Center) < 10)
                        {
                            player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 15, NPC.direction);
                        }
                    }
                    else if (NPC.ai[0] < 40)
                        stingerTarget = Vector2.Lerp(stingerTarget, NPC.Center + new Vector2(30 * NPC.direction, -28), 0.15f + (NPC.velocity.Length() * 0.02f));
                    else
                        stingerTarget = Vector2.Lerp(stingerTarget, NPC.Center + new Vector2(30 * NPC.direction, -50), 0.05f);

                }
            }
            UpdateLegs();
            if (tail != null)
            {
                if (NPC.Distance(player.Center) > 100)
                    stingerTarget = Vector2.Lerp(stingerTarget, NPC.Center + new Vector2(30 * NPC.direction, -28), 0.15f + (NPC.velocity.Length() * 0.02f));
                tail.Update(NPC.Center - new Vector2(32 * NPC.direction, 17), stingerTarget);
            }
        }
        void JumpCheck()
        {
            Player player = Main.player[NPC.target];
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
            if (NPC.Grounded(offsetX: 0.5f) && (NPC.collideX || Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 1000) < NPC.width || Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 1000) < NPC.width) && player.Center.Y < NPC.Center.Y + NPC.height)
                NPC.velocity.Y = -5;
            if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height) < NPC.height - 1)
            {
                NPC.noTileCollide = true;
                if (!Collision.CanHit(NPC, player))
                    NPC.Center -= Vector2.UnitY * 2;
            }
            else
                NPC.noTileCollide = false;
        }
        void UpdateLegs()
        {
            legUpdateT++;
            if (legUpdateT % 20 < 10 & NPC.Grounded())
            {
                if (!NPC.velocity.X.CloseTo(0, 0.05f))
                {
                    bodyOffset = Vector2.Lerp(bodyOffset, -Vector2.UnitY * 4, (NPC.velocity.Length() * 0.02f));
                    bgLegOffsets[0] = Vector2.Lerp(bgLegOffsets[0], new Vector2(MathHelper.Clamp(NPC.velocity.X * 5, -4, 4), -6 * (NPC.velocity.Length() * 0.02f)), 0.32f);
                    fgLegOffsets[1] = Vector2.Lerp(fgLegOffsets[1], new Vector2(MathHelper.Clamp(NPC.velocity.X * 5, -4, 4), -20 * (NPC.velocity.Length() * 0.02f)), 0.325f);
                }
                bgLegOffsets[1] = Vector2.Lerp(bgLegOffsets[1], new Vector2(0, 3), 0.45f);
                fgLegOffsets[0] = Vector2.Lerp(fgLegOffsets[0], new Vector2(0, 0), 0.45f);
            }
            else
            {
                bodyOffset = Vector2.Lerp(bodyOffset, Vector2.Zero, 0.2f);
                if (!NPC.velocity.X.CloseTo(0, 0.05f))
                {
                    bgLegOffsets[1] = Vector2.Lerp(bgLegOffsets[1], new Vector2(MathHelper.Clamp(NPC.velocity.X * 5, -4, 4), -6 * (NPC.velocity.Length() * 0.02f)), 0.325f);
                    fgLegOffsets[0] = Vector2.Lerp(fgLegOffsets[0], new Vector2(MathHelper.Clamp(NPC.velocity.X * 5, -4, 4), -20 * (NPC.velocity.Length() * 0.02f)), 0.325f);
                }

                bgLegOffsets[0] = Vector2.Lerp(bgLegOffsets[0], new Vector2(0, 0), 0.45f);
                fgLegOffsets[1] = Vector2.Lerp(fgLegOffsets[1], new Vector2(0, 0), 0.45f);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = TextureAssets.Npc[Type].Value;
            DrawBGLegs(spriteBatch, drawColor);

            DrawTail(spriteBatch, drawColor);

            SpriteEffects effect = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(tex, NPC.Center + bodyOffset + new Vector2(0, 8) - Main.screenPosition, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, effect, 0);


            DrawFGLegs(spriteBatch, drawColor);
            return false;
        }
        void DrawTail(SpriteBatch spriteBatch, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Texture2D tail0 = Helper.GetTexture(Texture + "_Tail0");
            Texture2D tail1 = Helper.GetTexture(Texture + "_Tail1");
            Texture2D tail2 = Helper.GetTexture(Texture + "_Tail2");
            Texture2D stinger = Helper.GetTexture(Texture + "_Stinger");
            if (tail != null)
            {
                for (int i = 0; i < tail.points.Count - 1; i++)
                {
                    float rot = Helper.FromAToB(tail.points[i].position, tail.points[i + 1].position).ToRotation();
                    if (i == 0)
                        rot = MathHelper.Clamp(Helper.FromAToB(tail.points[i].position, tail.points[i + 1].position).ToRotation(), -1.1f - MathHelper.PiOver2, 1.1f - MathHelper.PiOver2);
                    Texture2D tex = tail0;
                    if (i > 1)
                        tex = tail1;
                    if (i == tail.points.Count - 2)
                        tex = tail2;
                    spriteBatch.Draw(tex, tail.points[i].position - Main.screenPosition, null, drawColor, rot, tex.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                }
                if (NPC.ai[0] >= 80 && NPC.Distance(player.Center) < 100)
                    for (int i = 1; i < oldStingerPos.Length; i++)
                    {
                        var fadeMult = Helper.Safe(1f / oldStingerPos.Length);
                        float mult = (1f - fadeMult * i);
                        for (float j = 0; j < 5; j++)
                        {
                            Vector2 pos = Vector2.Lerp(oldStingerPos[i - 1], oldStingerPos[i], j / 5);
                            spriteBatch.Draw(stinger, pos - Main.screenPosition, null, Color.Gold * mult * 0.2f, 0, stinger.Size() / 2, NPC.scale * mult, SpriteEffects.None, 0);
                        }
                    }

                spriteBatch.Draw(stinger, tail.points[tail.points.Count - 1].position - Main.screenPosition, null, drawColor, 0, stinger.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            }
        }
        void DrawBGLegs(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D bgLeg0 = Helper.GetTexture(Texture + "_BGLeg0");
            Texture2D bgLeg1 = Helper.GetTexture(Texture + "_BGLeg1");
            Texture2D bgLeg2 = Helper.GetTexture(Texture + "_BGLeg2");
            SpriteEffects effect = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(bgLeg0, NPC.Center - new Vector2(2 * NPC.direction, -16) + bgLegOffsets[0] - Main.screenPosition, null, drawColor, NPC.rotation, bgLeg0.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(bgLeg1, NPC.Center - new Vector2(-10 * NPC.direction, -20) + bgLegOffsets[1] * 0.7f - Main.screenPosition, null, drawColor, NPC.rotation, bgLeg1.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(bgLeg2, NPC.Center - new Vector2(-18 * NPC.direction, -22) + bgLegOffsets[0] * 0.5f - Main.screenPosition, null, drawColor, NPC.rotation, bgLeg2.Size() / 2, NPC.scale, effect, 0);
        }
        void DrawFGLegs(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D fgLeg0 = Helper.GetTexture(Texture + "_FGLeg0");
            Texture2D fgLeg1 = Helper.GetTexture(Texture + "_FGLeg1");
            Texture2D fgLeg2 = Helper.GetTexture(Texture + "_FGLeg2");
            SpriteEffects effect = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(fgLeg0, NPC.Center - new Vector2((NPC.width / 2 - 6) * NPC.direction, -20) + fgLegOffsets[0] - Main.screenPosition, null, drawColor, NPC.rotation, fgLeg0.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(fgLeg1, NPC.Center - new Vector2(30 * NPC.direction, -25) + fgLegOffsets[1] * 0.7f - Main.screenPosition, null, drawColor, NPC.rotation, fgLeg1.Size() / 2, NPC.scale, effect, 0);
            spriteBatch.Draw(fgLeg2, NPC.Center - new Vector2(18 * NPC.direction, -29) + fgLegOffsets[0] * 0.5f - Main.screenPosition, null, drawColor, NPC.rotation, fgLeg2.Size() / 2, NPC.scale, effect, 0);
        }
    }
}
