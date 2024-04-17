using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.ArchmageX;
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

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageX : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(50, 78);
            NPC.lifeMax = 4500;
            NPC.defense = 5;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D arms = Helper.GetTexture(Texture + "_Arms");
            Texture2D singularArm = Helper.GetTexture(Texture + "_Arm");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            Texture2D headGlow = Helper.GetTexture(Texture + "_HeadGlow");
            //spriteBatch.Draw(arms, NPC.Center - screenPos, null, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(NPC.direction == -1 ? -14 : -2, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot, new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(singularArm.Width, 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot - (NPC.direction == -1 ? MathHelper.PiOver4 : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        float leftArmRot, rightArmRot;
        float headRotation;
        Rectangle headFrame = new Rectangle(0, 0, 36, 42);
        public const int NeutralFace = 0, ShockedFace = 1 * 42, SadFace = 2 * 42, DisappointedFace = 3 * 42, AngryFace = 4 * 42;
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
        public float Mana
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        public const int Taunt = -4, Despawn = -3, Death = -2, Idle = -1, Spawn = 0, PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, AmethystStorm = 5, HelicopterBlades = 6, AmethystBulletHell = 7, GiantAmethyst = 8, Micolash = 9, PhantasmalBlast = 10, AmethystCloseIn = 11, ExplosionAtFeet = 12, ShadowflameRift = 13, TheSheepening = 14;
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }
        float Next = SpectralOrbs;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    AIState = Despawn;
                }
            }
            AITimer++;


            switch (AIState)
            {
                case Despawn:
                    {

                    }
                    break;
                case Death:
                    {

                    }
                    break;
                case Spawn:
                    {
                        FacePlayer();
                        if (AITimer == 10)
                        {
                            AIState = Idle;
                            Reset();
                            DialogueSystem.NewDialogueBox(150, NPC.Center - new Vector2(0, 80), "Magic!", Color.Purple, -1, 0.6f, default, 2.5f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }
                    }
                    break;
                case Idle:
                    {
                        FacePlayer();
                        AITimer2--;
                        //hover vfx
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 3, 0.1f);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && AITimer2 <= 0)
                        {
                            NPC.velocity.Y = -8.5f;
                            //jump dust
                            AITimer2 = 180;
                        }
                        if (AITimer >= 100)
                        {
                            Reset();
                            AIState = Next;
                        }
                    }
                    break;
                case PhantasmalSpirit:
                    {
                        FacePlayer();
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Phantasmal Spirits!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);

                        if (AITimer < 85) rightArmRot = Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
                        else rightArmRot = MathHelper.Lerp(rightArmRot, 0, 0.05f);

                        if (AITimer < 65 && AITimer > 50)
                        {
                            headFrame.Y = ShockedFace;
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        if (AITimer == 75 || AITimer == 95 || AITimer == 135)
                        {
                            headFrame.Y = NeutralFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            EbonianSystem.ScreenShakeAmount = 6;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * 5f, ModContent.ProjectileType<XSpirit>(), 15, 0);
                        }
                        if (AITimer >= 150)
                        {
                            Reset();
                            Next = ShadowflamePuddles;
                            AIState = Idle;
                        }
                    }
                    break;
                case ShadowflamePuddles:
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 1)
                        {
                            FacePlayer();
                            AITimer2 = Main.rand.Next(4);
                        }
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowflame Eruption!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        if (AITimer < 180 && AITimer > 1)
                        {
                            rightArmRot = -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0);
                        }
                        else
                        {
                            rightArmRot = MathHelper.Lerp(rightArmRot, 0, 0.05f);
                        }
                        if (AITimer2 > 0)
                        {
                            if (AITimer >= 60 && AITimer <= 180 && AITimer % (Main.expertMode ? 20 : 40) == 0)
                            {
                                Vector2 pos = NPC.Center + new Vector2(Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 1000) * Main.rand.NextFloat(0.9f), 0);
                                if (Main.rand.NextBool()) pos = NPC.Center - new Vector2(Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 1000) * Main.rand.NextFloat(0.9f), 0);
                                Projectile.NewProjectile(null, pos, Main.rand.NextBool() ? -Vector2.UnitY : Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 30, 0);
                            }
                        }
                        else
                        {
                            if (AITimer == 100)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector2 _pos = Helper.TRay.Cast(NPC.Center, -Vector2.UnitX, 1000);
                                    Vector2 __pos = Helper.TRay.Cast(NPC.Center, Vector2.UnitX, 1000);
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 5);
                                    Projectile.NewProjectile(null, pos, Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 30, 0);
                                    Projectile.NewProjectile(null, pos, -Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 30, 0);
                                }
                            }
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            Next = SpectralOrbs;
                            AIState = Idle;
                        }
                    }
                    break;
                case SpectralOrbs:
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        rightArmRot = MathHelper.Lerp(rightArmRot, 0, 0.05f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Spectral Gems!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);

                        if (AITimer >= 60 && AITimer <= (Main.expertMode ? 180 : 120) && AITimer % 60 == 0)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                            for (int i = 0; i < 3 + (AITimer / 60); i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                                Projectile.NewProjectile(null, NPC.Center, Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<XAmethyst>(), 30, 0);
                            }
                        }

                        if (AITimer >= 300)
                        {
                            Reset();
                            Next = PhantasmalSpirit;
                            AIState = Idle;
                        }
                    }
                    break;

            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return player.Center.Y > NPC.Center.Y + 50 && AIState == Idle;
        }
        void Reset()
        {
            AITimer = 0;
            AITimer2 = 0;
            NPC.velocity.X = 0;
            NPC.damage = 0;
        }
    }
}
