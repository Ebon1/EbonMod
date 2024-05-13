using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Dusts;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Projectiles.ArchmageX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

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
            Texture2D manaPot = TextureAssets.Projectile[ModContent.ProjectileType<XManaPotion>()].Value;
            Texture2D staff = Helper.GetTexture("Items/Weapons/Magic/StaffOfXLiteEdition");
            Texture2D heli = Helper.GetExtraTexture("Sprites/ArchmageXHeli");
            Texture2D heliGlow = Helper.GetExtraTexture("Sprites/ArchmageXHeli_Glow");

            Vector2 staffP = NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 10 : 20), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * 1.11f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 14);
            float staffRot = Helper.FromAToB(NPC.Center, staffP).ToRotation() + MathHelper.PiOver4 * (NPC.direction == -1 ? 1.2f : 0.8f);

            Vector2 heliP = NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 36 : 46), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * .87f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
            float heliR = Helper.FromAToB(NPC.Center, heliP).ToRotation() + MathHelper.PiOver2 * (NPC.direction == -1 ? 1.1f : 0.8f);
            if (NPC.direction == 1)
            {
                staffP.X += 14;
                heliP.X += 6;
                staffP.Y += 2;
            }
            else
            {
                staffP.X -= 4;
                staffP.Y -= 14;
                heliP.Y -= 8;
                heliP.X -= 12;
            }
            staffP.Y -= 6;

            spriteBatch.Draw(staff, staffP - screenPos, null, Color.White * staffAlpha, staffRot, staff.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            Vector2 scale = new Vector2(1f, 0.25f);
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            Vector4 col = (Color.White * heliAlpha).ToVector4();
            col.W = heliAlpha * 0.25f;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);


            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heliGlow, pos - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale);
            col.W = heliAlpha;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heli, pos - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);


            spriteBatch.Draw(manaPot, NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18) - screenPos, null, Color.White * 0.9f * manaPotAlpha, 0, manaPot.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(NPC.direction == -1 ? -14 : -2, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot, new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(singularArm.Width, 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot - (NPC.direction == -1 ? MathHelper.PiOver4 : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        float leftArmRot, rightArmRot;
        float headRotation;
        float manaPotAlpha, staffAlpha = 1f, heliAlpha;
        Rectangle headFrame = new Rectangle(0, 0, 36, 42);
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }

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
        public const int Taunt = -4, Despawn = -3, Death = -2, Idle = -1, Spawn = 0, PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, SineLaser = 5, AmethystCloseIn = 6,
            AmethystBulletHell = 7, GiantAmethyst = 8, Micolash = 9, TheSheepening = 10, ManaPotion = 11, PhantasmalBlast = 12, ShadowflameRift = 13, HelicopterBlades = 14, AmethystStorm = 15, ExplosionAtFeet = 16;

        float Next = HelicopterBlades;
        public override void AI()
        {
            if (NPC.direction != NPC.oldDirection)
                rightArmRot = 0;
            float rightHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
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
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.3f);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (rightArmRot.CloseTo(0, 0.25f))
                            FacePlayer();
                        AITimer2--;
                        //hover vfx
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.75f, 0.1f);
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

                        if (AITimer < 135) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if ((AITimer < 65 && AITimer > 50) || (AITimer < 125 && AITimer > 100))
                        {
                            headFrame.Y = ShockedFace;
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        NPC.velocity.X *= 0.95f;
                        if (AITimer == 75 || AITimer == 95 || AITimer == 135)
                        {
                            NPC.velocity.X += -NPC.direction * 5;
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
                            AITimer2 = Main.rand.Next(4);
                        }
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowflame Eruption!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        if (AITimer < 180 && AITimer > 1)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        }
                        else
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        }

                        if (AITimer < 50)
                        {
                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X, NPC.Center.Y);
                            if (player.Center.X < GetArenaRect().X + GetArenaRect().Width / 2)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + OffsetBoundaries(NPC.Size * 1.5f).Width, NPC.Center.Y);
                            NPC.direction = player.Center.X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, pos).X * 3.75f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;

                        if (AITimer2 > 0)
                        {
                            if (AITimer >= 60 && AITimer <= 180 && AITimer % (Main.expertMode ? 20 : 40) == 0)
                            {
                                Vector2 pos = NPC.Center + new Vector2(Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 900) * Main.rand.NextFloat(0.9f), 0);
                                if (Main.rand.NextBool()) pos = NPC.Center - new Vector2(Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 900) * Main.rand.NextFloat(0.9f), 0);
                                Projectile.NewProjectile(null, pos, Main.rand.NextBool() && Helper.TRay.CastLength(pos, -Vector2.UnitY, 1000) < 900 ? Vector2.UnitY : -Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 30, 0);
                            }
                        }
                        else//phase 2
                        {
                            if (AITimer == 100)
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector2 _pos = Helper.TRay.Cast(NPC.Center, -Vector2.UnitX, 900);
                                    Vector2 __pos = Helper.TRay.Cast(NPC.Center, Vector2.UnitX, 900);
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 5);
                                    if (Helper.TRay.CastLength(pos, -Vector2.UnitY, 1000) < 900)
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
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Spectral Jewels!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);

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
                            Next = MagnificentFireballs;
                            AIState = Idle;
                        }
                    }
                    break;
                case MagnificentFireballs:
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                        {
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shimmering Fusillade!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }
                        if ((AITimer <= 70 && AITimer >= 50) || AITimer == 110 || (AITimer <= 180 && AITimer >= 160) || (AITimer <= 240 && AITimer >= 230))
                        {
                            FacePlayer();
                            if (AITimer % (AITimer > 229 ? 5 : 10) == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 * (AITimer == 110 ? 0 : (AITimer > 229 ? 1.2f : 0.65f)));
                                Projectile.NewProjectile(null, NPC.Center + vel * 15f, vel * 7f, ModContent.ProjectileType<XBolt>(), 15, 0);
                            }
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }

                        if (AITimer >= 300)
                        {
                            Reset();
                            Next = SineLaser;
                            AIState = Idle;
                        }
                    }
                    break;
                case SineLaser:
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowey Helix of Flame!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        if (AITimer < 80)
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        Vector2 vel = -(rightArmRot - MathHelper.Pi + (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2)).ToRotationVector2();
                        if (AITimer == 85)
                        {
                            SoundStyle s = s = EbonianSounds.cursedToyCharge;
                            SoundEngine.PlaySound(s, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: 100);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: -100);
                        }
                        if (AITimer == 120)
                        {
                            SoundStyle s = s = EbonianSounds.eruption;
                            SoundEngine.PlaySound(s, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: 100);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: -100);
                        }
                        if (AITimer >= 300)
                        {
                            Reset();
                            Next = AmethystCloseIn;
                            AIState = Idle;
                        }
                    }
                    break;
                case AmethystCloseIn:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Amethysts of Empowerment!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);

                        if (AITimer % 30 == 0 && AITimer > 59 && AITimer < 151)
                        {
                            float off = Main.rand.NextFloat(MathHelper.PiOver2);
                            for (int i = 0; i < 5; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 5) + off;
                                Vector2 pos = NPC.Center + new Vector2(300, 0).RotatedBy(angle);
                                Projectile p = Projectile.NewProjectileDirect(null, pos, Helper.FromAToB(pos, NPC.Center) * 0.1f, ModContent.ProjectileType<XAmethystCloseIn>(), 15, 0);
                                p.ai[0] = NPC.Center.X;
                                p.ai[1] = NPC.Center.Y;
                            }
                        }

                        if (AITimer >= 240)
                        {
                            Reset();
                            Next = AmethystBulletHell;
                            AIState = Idle;
                        }
                    }
                    break;
                case AmethystBulletHell: //phase 2
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "FOCUS!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);

                        if (AITimer > 60 && AITimer < 170)
                        {
                            if (AITimer % 10 == 0)
                            {
                                for (int i = 0; i < disposablePos.Length; i++)
                                {
                                    if (disposablePos[i] != Vector2.Zero)
                                        continue;
                                    disposablePos[i] = Main.rand.NextVector2FromRectangle(GetArenaRect());

                                    for (int j = 0; j < 5; j++)
                                    {
                                        Dust.NewDustPerfect(disposablePos[i], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                        Dust.NewDustPerfect(disposablePos[i], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                                    }
                                    break;
                                }
                            }
                        }
                        if (AITimer == 180)
                        {
                            SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
                            for (int i = 0; i < disposablePos.Length; i++)
                            {
                                Projectile.NewProjectile(null, disposablePos[i], Helper.FromAToB(disposablePos[i], player.Center + Main.rand.NextVector2Circular(150, 150)) * 0.1f, ModContent.ProjectileType<XAmethystCloseIn>(), 0, 0);
                            }
                        }
                        if (AITimer >= 240)
                        {
                            Reset();
                            Next = GiantAmethyst;
                            AIState = Idle;
                        }
                    }
                    break;
                case GiantAmethyst:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "big gem lmfao", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        if (AITimer == 70)
                        {
                            Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 80), Vector2.Zero, ModContent.ProjectileType<XLargeAmethyst>(), 30, 0);
                            Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 80), Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                        if (AITimer >= 180)
                        {
                            Reset();
                            Next = Micolash;
                            AIState = Idle;
                        }
                    }
                    break;
                case Micolash:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        if (AITimer == 40)
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Think fast!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        if (AITimer < 130)
                        {
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 7, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.7f;
                        }
                        if (AITimer == 130)
                        {
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                        if (AITimer == 145)
                            disposablePos[0] = player.Center;
                        if (AITimer > 170 && AITimer < 202 && AITimer % 4 == 0)
                        {
                            Projectile a = Projectile.NewProjectileDirect(null, NPC.Center - new Vector2(0, 6), Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedByRandom(MathHelper.PiOver4 * 0.5f), ModContent.ProjectileType<XTentacle>(), 15, 0);
                            a.ai[0] = Main.rand.Next(50, 90);
                            a.ai[1] = Main.rand.NextFloat(0.5f, 2f);
                        }

                        if (AITimer >= 260)
                        {
                            Reset();
                            Next = TheSheepening;
                            AIState = Idle;
                        }
                    }
                    break;
                case TheSheepening:
                    {
                        if (AITimer < 70)
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Bleat now!");
                            chat.Add("'Baa! Baa!'... That's you!");
                            chat.Add("The Sheepening!");
                            chat.Add("To the slaughter with you!");
                            chat.Add("Have you any wool?");
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }

                        if (AITimer == 50)
                        {

                            for (int i = 0; i < 15; i++)
                            {
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(17, 17), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer == 70)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity * 2) * 0.05f, ModContent.ProjectileType<SheepeningOrb>(), 1, 0);
                        }
                        if (AITimer >= 120)
                        {
                            Reset();
                            Next = ManaPotion;
                            AIState = Idle;
                        }
                    }
                    break;
                case ManaPotion:
                    {
                        FacePlayer();
                        if (AITimer < 40)
                        {
                            if (AITimer > 20)
                                manaPotAlpha = MathHelper.Lerp(0, 1, (AITimer - 20) / 20);
                            else
                                staffAlpha = MathHelper.Lerp(1, 0, AITimer / 19);
                            if (AITimer == 25 || AITimer == 5)
                            {
                                Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                                for (int i = 0; i < 10; i++)
                                {
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                                }
                            }
                            AITimer2 = MathHelper.Lerp(AITimer2, MathHelper.PiOver4, 0.1f);
                            disposablePos[0] = player.Center;
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                        }
                        if (AITimer == 60)
                        {
                            Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity) * Main.rand.NextFloat(12, 17), ModContent.ProjectileType<XManaPotion>(), 15, 0);
                        }
                        if (AITimer > 60)
                        {
                            AITimer2 = MathHelper.Lerp(AITimer2, -(MathHelper.PiOver2 * 0.75f), 0.25f);
                            if (AITimer > 75)
                                staffAlpha = MathHelper.Lerp(0, 1, 0.1f);
                            manaPotAlpha = 0;
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                        }
                        if (AITimer >= 100)
                        {
                            Reset();
                            Next = PhantasmalBlast;
                            AIState = Idle;
                        }
                    }
                    break;
                case PhantasmalBlast: //phase 2
                    {
                        if (AITimer == 40)
                        {
                            AITimer2 = 0.5f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Phantasmal Blast!", Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }

                        if (AITimer < 145) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if (AITimer > 60 && AITimer < 120 && AITimer % 20 < 10)
                        {
                            headFrame.Y = NeutralFace;
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        if (AITimer > 60 && AITimer < 150 && AITimer % 10 == 0)
                        {
                            AITimer2 += 0.1f;
                            headFrame.Y = ShockedFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            EbonianSystem.ScreenShakeAmount = 6;
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver2 * 0.7f * AITimer2) * 5f, ModContent.ProjectileType<XSpiritNoHome>(), 15, 0, -1, 0.25f);
                        }
                        if (AITimer == 110)
                        {
                            disposablePos[0] = player.Center;
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: 7.5f);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: -7.5f);
                        }
                        if (AITimer == 140)
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: 7.5f).localAI[0] = 1;
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: -7.5f).localAI[0] = 1;
                        }

                        if (AITimer >= 210)
                        {
                            Reset();
                            Next = ShadowflameRift;
                            AIState = Idle;
                        }
                    }
                    break;
                case ShadowflameRift:
                    {
                        //more portals in phase 2
                        if (AITimer == 80)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Hold up, I forgot to bring something...");
                            chat.Add("Don't mind me! I just forgot something...");
                            chat.Add("I seem to have forgotten something...");
                            chat.Add("Hehehehe..."); //phase 2
                            DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Purple, -1, 0.6f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite);
                        }
                        if (AITimer < 50)
                        {
                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X, NPC.Center.Y);
                            if (player.Center.X < GetArenaRect().X + GetArenaRect().Width / 2)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + OffsetBoundaries(NPC.Size * 1.5f).Width, NPC.Center.Y);
                            NPC.direction = player.Center.X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, pos).X * 3.75f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        if (AITimer > 60 && AITimer < 160)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            if (AITimer < 140)
                                disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100);

                            if (AITimer < 85)
                            {
                                disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                            }
                            if (AITimer % 2 == 0)
                            {
                                Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                                Dust.NewDustPerfect(disposablePos[1], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[1], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                        }
                        if (AITimer == 85)
                        {
                            Projectile.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ModContent.ProjectileType<XRift>(), 0, 0);
                        }
                        if (AITimer == 140)
                        {
                            Projectile.NewProjectile(null, disposablePos[1], Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                        }

                        if (AITimer >= 310)
                        {
                            Reset();
                            Next = HelicopterBlades;
                            AIState = Idle;
                        }
                    }
                    break;
                case HelicopterBlades: //phase 2
                    {
                        FacePlayer();
                        if (AITimer < 170)
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else if (AITimer >= 170 && AITimer < 300)
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else
                            rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        if (AITimer >= 40 && AITimer <= 240)
                        {
                            if (AITimer >= 180 && AITimer % 20 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center);
                                Projectile.NewProjectile(null, NPC.Center + vel * 25f, vel * 7f, ModContent.ProjectileType<XKnife>(), 15, 0);
                            }
                            heliAlpha = MathHelper.Lerp(heliAlpha, 1, 0.1f);
                            NPC.noGravity = true;
                            if (AITimer < 180)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 1000, true) - new Vector2(0, 150), false) / 35, 0.025f);
                            else
                                NPC.velocity *= 0.9f;
                        }
                        if (AITimer > 250 && AITimer < 290)
                        {
                            NPC.noGravity = false;
                            heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                            Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.DarkOrchid, Main.rand.NextFloat(0.06f, .2f));
                            d.noGravity = true;
                            d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                        }
                        if (AITimer == 270)
                        {
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0, ai2: 1);
                        }
                        if (AITimer == 310)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.2f), NPC.Center);
                            Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 40, Helper.FromAToB(NPC.Center, player.Center), ModContent.ProjectileType<PhantasmalGreatswordP2>(), 20, 0);
                            a.friendly = false;
                            a.hostile = true;
                        }
                        if (AITimer >= 400)
                        {
                            Reset();
                            Next = PhantasmalSpirit;
                            AIState = Idle;
                        }
                    }
                    break;
            }
        }
        // TODO: Detached head, Drinkable mana potion, Mana system, smoother movement, improved arm movement, phase 2 attack changes
        Rectangle GetArenaRect()
        {
            Vector2 L = Helper.TRay.Cast(NPC.Center, -Vector2.UnitX, 900);
            Vector2 R = Helper.TRay.Cast(NPC.Center, Vector2.UnitX, 900);
            Vector2 U = Helper.TRay.Cast(NPC.Center, -Vector2.UnitY, 350);
            Vector2 D = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 350);
            Vector2 TopLeft = new Vector2(L.X, U.Y);
            Vector2 BottomRight = new Vector2(R.X, D.Y);
            Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
            return rect;
        }
        Rectangle OffsetBoundaries(Vector2 offset)
        {
            Rectangle rect = GetArenaRect();
            rect.Width -= (int)offset.X;
            rect.Height -= (int)offset.Y;
            rect.X += (int)offset.X / 2;
            rect.Y += (int)offset.Y / 2;
            return rect;
        }
        Vector2[] disposablePos = new Vector2[10];
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return (player.Center.Y > NPC.Center.Y + 50 && AIState == Idle) || AIState == ExplosionAtFeet;
        }
        void Reset()
        {
            AITimer = 0;
            AITimer2 = 0;
            NPC.noGravity = false;
            manaPotAlpha = 0;
            staffAlpha = 1;
            heliAlpha = 0;
            NPC.velocity.X = 0;
            headFrame.Y = NeutralFace;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            NPC.damage = 0;
        }
    }
}
