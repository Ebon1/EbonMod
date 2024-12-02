using EbonianMod.Buffs;
using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Dusts;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace EbonianMod.NPCs.ArchmageX
{
    [AutoloadBossHead]
    public class ArchmageX : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(50, 78);
            NPC.lifeMax = 11000;
            NPC.defense = 14;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = 0;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: 'Master' Magician"),
                new FlavorTextBestiaryInfoElement("The most magnificent and brilliant sorcerer to ever walk this earth... An unmatched master of wizardry! A true magician! The Archmage!!\n\nAll self-proclaimed, of course."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D singularArm = Helper.GetTexture(Texture + "_Arm");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            Texture2D headGlow = Helper.GetTexture(Texture + "_HeadGlow");
            Texture2D manaPot = TextureAssets.Projectile[ModContent.ProjectileType<XManaPotion>()].Value;
            Texture2D staff = Helper.GetTexture("Items/Weapons/Magic/StaffOfXLiteEdition");
            Texture2D heli = Helper.GetExtraTexture("Sprites/ArchmageXHeli");
            Texture2D heliGlow = Helper.GetExtraTexture("Sprites/ArchmageXHeli_Glow");

            Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;
            float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);
            //Helper.FromAToB(NPC.Center, staffP).ToRotation() + (NPC.direction == -1 ? -MathHelper.PiOver2 * 1.65f : 0) + MathHelper.PiOver4 * (NPC.direction == -1 ? 1.2f : 0.8f);

            Vector2 heliP = NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 36 : 46), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * .87f - MathHelper.PiOver4 * 0.1f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4) + MathHelper.PiOver4 * 0.5f)) - new Vector2(0, NPC.direction == 1 ? 18 : 12);
            float heliR = Helper.FromAToB(NPC.Center, heliP).ToRotation() + MathHelper.PiOver2 * (NPC.direction == -1 ? 1.1f : 0.8f);

            spriteBatch.Draw(staff, staffP - screenPos, null, Color.White * staffAlpha, staffRot, new Vector2(0, staff.Height), NPC.scale, SpriteEffects.None, 0f);

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

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(NPC.direction == -1 ? -14 : -6, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * -NPC.direction), new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(singularArm, NPC.Center - new Vector2(singularArm.Width - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) - (NPC.direction == -1 ? MathHelper.PiOver4 * 0.5f : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (GetArenaRect().Size().Length() > 1054) return;
            arenaAlpha = MathHelper.Lerp(arenaAlpha, 1, 0.1f);

            arenaVFXOffset += 0.005f;
            if (arenaVFXOffset >= 1)
                arenaVFXOffset = 0;
            arenaVFXOffset = MathHelper.Clamp(arenaVFXOffset, float.Epsilon, 1 - float.Epsilon);
            List<VertexPositionColorTexture> verticesR = new List<VertexPositionColorTexture>();
            Texture2D texture = Helper.GetExtraTexture("wavyLaser2");
            Vector2 startR = GetArenaRect().BottomRight() + Vector2.UnitY * 16 - Main.screenPosition;
            Vector2 offVert = (Helper.FromAToB(GetArenaRect().BottomRight() + Vector2.UnitY * 16, GetArenaRect().BottomRight() - Vector2.UnitY * 16 * 6, false));
            float rotVert = Helper.FromAToB(startR, startR + offVert).ToRotation();
            float s = 0f;
            float sLin = 0f;
            for (float i = 0; i < 1; i += 0.001f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                if (i < 0.5f)
                    sLin = MathHelper.Clamp(i, 0, 0.5f);
                else
                    sLin = MathHelper.Clamp((-i + 1), 0, 0.5f);

                float cA = MathHelper.Lerp(s, sLin, i);
                float vertSize = MathHelper.SmoothStep(5, 18, s);

                float __off = arenaVFXOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + i;

                Color col = new Color(60, 2, 113) * (arenaAlpha * s);
                verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert + MathHelper.PiOver2), new Vector2(_off, 0), col * 2));
                verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert - MathHelper.PiOver2), new Vector2(_off, 1), col * 2));
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (verticesR.Count >= 3)
            {
                Helper.DrawTexturedPrimitives(verticesR.ToArray(), PrimitiveType.TriangleStrip, texture, false);
            }
            Main.spriteBatch.ApplySaved();
        }
        float leftArmRot, rightArmRot;
        float headRotation, headYOff, headOffIncrementOffset;
        float manaPotAlpha, staffAlpha = 1f, heliAlpha, arenaVFXOffset, arenaAlpha;
        Rectangle headFrame = new Rectangle(0, 0, 36, 42);
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }
        public override void OnSpawn(IEntitySource source)
        {
            sCenter = NPC.Center;
        }

        public const int
            NeutralFace = 0,
            ShockedFace = 1 * 42,
            SadFace = 2 * 42,
            DisappointedFace = 3 * 42,
            AngryFace = 4 * 42,
            LightSmirkFace = 5 * 42,
            SmirkFace = 6 * 42,
            VeryShockedFace = 7 * 42,
            BlinkingFace = 8 * 42,
            AssholeFace = 9 * 42;
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
        public float AITimer3
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        public const int Phase2Transition = -5, Taunt = -4, Despawn = -3, Death = -2, Idle = -1, Spawn = 0,
            PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, SineLaser = 5, AmethystCloseIn = 6,
            AmethystBulletHell = 7, GiantAmethyst = 8, Micolash = 9, TheSheepening = 10, ManaPotion = 11, PhantasmalBlast = 12, ShadowflameRift = 13,
            HelicopterBlades = 14, AmethystStorm = 15;
        public List<int> Phase1AttackPool = new List<int>()
        {
            ManaPotion,
            PhantasmalSpirit,
            AmethystCloseIn,
            TheSheepening,
            SpectralOrbs,
            SineLaser,
            AmethystBulletHell,
            Micolash,
            ShadowflamePuddles,
            MagnificentFireballs,
            AmethystStorm,
            //adding weight to the rng
            SineLaser,
            PhantasmalSpirit,
            ShadowflamePuddles,
            SpectralOrbs,
            MagnificentFireballs
        };
        public List<int> Phase2AttackPool = new List<int>()
        {
            ManaPotion,
            PhantasmalSpirit,
            AmethystCloseIn,
            GiantAmethyst,
            TheSheepening,
            SpectralOrbs,
            AmethystBulletHell,
            Micolash,
            ShadowflameRift,
            ShadowflamePuddles,
            MagnificentFireballs,
            HelicopterBlades,
            PhantasmalBlast,
            AmethystStorm,
            //adding weight to the rng
            ShadowflameRift,
            GiantAmethyst,
            PhantasmalSpirit,
            SineLaser,
            MagnificentFireballs

        };
        public List<float> MeleeAttacks = new List<float>()
        {
            ManaPotion,
            PhantasmalSpirit,
            SineLaser,
            MagnificentFireballs,
            GiantAmethyst,
            AmethystStorm,
            HelicopterBlades,
            ShadowflamePuddles,
            SpectralOrbs,
            Micolash,
            AmethystBulletHell,
            AmethystCloseIn,
        };
        float Next = 1;
        void IdleAnimation()
        {
            if (NPC.collideY || NPC.velocity.Y.CloseTo(0))
            {
                rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.3f);
                leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
            }
            else
            {
                rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-NPC.velocity.Y * .5f), 0.1f);
                leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(NPC.velocity.Y * .5f), 0.1f);
            }
        }
        void PickAttack()
        {
            if (AIState != Taunt)
                oldAttack = AIState;
            if (doneAttacksBefore)
            {
                int attempts = 0;
                if (!phase2)
                {
                    Next = Main.rand.Next(Phase1AttackPool);
                    while (attempts++ < 10 && Next == AmethystStorm && oldAttack == Next)
                        Next = Main.rand.Next(Phase1AttackPool);
                }
                else
                {
                    Next = Main.rand.Next(Phase2AttackPool);
                    while (attempts++ < 20 && Phase2AttackPool.IndexOf((int)Next) > 11 && oldAttack == Next)
                        Next = Main.rand.Next(Phase2AttackPool);
                }
            }
            else
            {
                Next = oldAttack + 1;
                if (Next > (phase2 ? 15 : 11))
                {
                    doneAttacksBefore = true;
                    PickAttack();
                    return;
                }
            }
            AIState = Idle;
            AITimer = Main.rand.Next(-40, 20);
            if (Main.rand.NextBool((int)(8 + phaseMult * 2)) && oldAttack != Spawn)
            {
                AIState = Taunt;
                AITimer = 0;
            }
        }
        bool phase2;
        float phaseMult;
        float oldAttack = Spawn;
        bool doneAttacksBefore;
        int blinkInterval;
        int frameBeforeBlink;
        SlotId helicopterSlot;
        FloatingDialogueBox currentDialogue;
        public override bool CheckDead()
        {
            EbonianSystem.xareusFightCooldown = 3600 * 12;
            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ArchmageDeath>(), 0, 0);
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Stop();
            }
            return true;
        }
        public override void AI()
        {
            if (NPC.life < NPC.lifeMax * 0.3f)
            {
                if (phaseMult != 3)
                {
                    if (AIState == Idle)
                    {
                        doneAttacksBefore = false;
                        Next = PhantasmalSpirit;
                        phaseMult = 3;
                    }
                }
            }
            else if (phase2)
                phaseMult = 2;
            else if (NPC.life < NPC.lifeMax * 0.75f)
                phaseMult = 1;
            else
                phaseMult = 0;
            EbonianSystem.xareusFightCooldown = 500;
            blinkInterval++;
            if (blinkInterval >= 170 && blinkInterval < 175)
            {
                if (headFrame.Y != BlinkingFace)
                {
                    frameBeforeBlink = headFrame.Y;
                }
                if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                    headFrame.Y = BlinkingFace;
            }
            if (blinkInterval == 176)
            {
                if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                    headFrame.Y = frameBeforeBlink;
                blinkInterval = Main.rand.Next(-250, 10);
            }
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    AIState = Despawn;
                }
            }
            if (GetArenaRect().Size().Length() > 100)
            {
                if (Main.LocalPlayer.Distance(GetArenaRect().Center()) > 1200)
                {
                    Helper.TPNoDust(GetArenaRect().Center(), Main.LocalPlayer);
                }
                else
                {
                    while (Main.LocalPlayer.Center.X < GetArenaRect().X)
                        Main.LocalPlayer.Center += Vector2.UnitX * 2;

                    while (Main.LocalPlayer.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        Main.LocalPlayer.Center -= Vector2.UnitX * 2;

                    while (Main.LocalPlayer.Center.Y < GetArenaRect().Y)
                        Main.LocalPlayer.Center += Vector2.UnitY * 2;
                }

                if (NPC.Distance(GetArenaRect().Center()) > 1200)
                {
                    NPC.Center = GetArenaRect().Center();
                }
                else
                {
                    while (NPC.Center.X < GetArenaRect().X)
                        NPC.Center += Vector2.UnitX * 2;

                    while (NPC.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        NPC.Center -= Vector2.UnitX * 2;

                    while (NPC.Center.Y < GetArenaRect().Y)
                        NPC.Center += Vector2.UnitY * 2;
                }
            }


            if (NPC.life < NPC.lifeMax / 2 + 700)
            {
                NPC.defense = 13;
                if (!phase2 && AIState != Taunt)
                {
                    Next = 1;
                    oldAttack = 0;
                    Reset();
                    AIState = Phase2Transition; //change to a transition attack later
                    doneAttacksBefore = false;
                    phase2 = true;
                }
            }
            if (NPC.direction != NPC.oldDirection)
                rightArmRot = 0;
            float rightHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
            float leftHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver2 : MathHelper.PiOver4);
            Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
            if (NPC.direction == 1)
                staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;

            float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);
            Vector2 staffTip = staffP + staffRot.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * 48;

            Lighting.AddLight(staffTip, TorchID.Purple);
            AITimer++;
            headYOff = MathHelper.Lerp(headYOff, MathF.Sin((AITimer + headOffIncrementOffset) * 0.05f) * 2, 0.2f);
            switch (AIState)
            {
                case Despawn:
                    {
                        rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-65f), 0.1f);
                        leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(65f), 0.1f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            if (!phase2)
                            {
                                chat.Add("Your skill in losing is truly unmatched!");
                                chat.Add("Just another moth to my glorious flame!");
                                chat.Add("Hah! Another victory for the brilliant Archmage!");
                                chat.Add("Would a purple laser pointer ALSO defeat you?");
                                chat.Add("A predictable failure, THIEF!");
                                if (NPC.life > NPC.lifeMax - 500)
                                    chat.Add("Wait, you're actually THAT incompetent?! I pity you.");
                            }
                            else
                            {
                                chat.Add("Phew... I mean, not even close!");
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer >= 100)
                        {
                            NPC.active = false;
                        }
                    }
                    break;
                case Spawn:
                    {
                        FacePlayer();
                        if (AITimer == 1)
                        {
                            NPC.dontTakeDamage = true;
                            if (EbonianSystem.heardXareusIntroMonologue)
                                AITimer = 339;
                        }
                        if (AITimer < 50 || AITimer > 350)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                        }
                        else IdleAnimation();

                        if (AITimer == 1)
                        {
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionTiny>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                            //SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
                        }
                        if (AITimer == 50)
                        {
                            headFrame.Y = AngryFace;
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "GET YOUR HANDS OFF, YYYOOOOUUUU!!!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer == 150)
                        {
                            headFrame.Y = SadFace;
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "my staff... my BELOVED STAFF, SO PRISTINE UNTIL NOW!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer == 350)
                        {
                            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/xareus");
                            headFrame.Y = AngryFace;
                            if (!EbonianSystem.heardXareusIntroMonologue)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "YOU FIEND!!! I'LL DESTROY YOU IN EVERY WAY PHYSICALLY POSSIBLE!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                            {
                                WeightedRandom<string> chat = new WeightedRandom<string>();
                                chat.Add("Would you PLEASE stop that!?");
                                chat.Add("I thought the first time taught you a proper lesson!");
                                chat.Add("STOP TOUCHING THE STAFF!");
                                chat.Add("You again?!");
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            }
                        }
                        if (AITimer >= 500)
                        {
                            NPC.dontTakeDamage = false;
                            EbonianSystem.heardXareusIntroMonologue = true;
                            //AIState = Idle;
                            Reset();
                            PickAttack();
                        }
                    }
                    break;
                case Phase2Transition:
                    {
                        if (AITimer == 1)
                            disposablePos[9] = GetArenaRect().Center();
                        if (AITimer < 340 || AITimer > 551)
                        {
                            if (AITimer == 1 || AITimer > 300)
                                FacePlayer();
                            NPC.dontTakeDamage = true;
                            if (AITimer < 340)
                                headFrame.Y = AngryFace;
                            else
                                headFrame.Y = AssholeFace;

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, disposablePos[9], false).X * .03f;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            NPC.velocity *= 0.5f;
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            if (AITimer >= 400 && AITimer < 551)
                            {
                                FacePlayer();
                                rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            }
                            else
                                IdleAnimation();
                        }
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), "None of this would've happened if you just kept your grubby hands away from my things!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer == 100)
                        {
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int i = 0; i < 4; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 4) + off;
                                Vector2 vel = angle.ToRotationVector2();
                                Projectile.NewProjectile(null, staffTip + vel * 50, vel, ModContent.ProjectileType<XRift>(), 15, 0);
                            }
                        }
                        if (AITimer == 300)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 _pos = GetArenaRect().Left();
                                Vector2 __pos = GetArenaRect().Right();
                                Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 2);
                                Projectile.NewProjectileDirect(null, Helper.TRay.Cast(pos, Vector2.UnitY, GetArenaRect().Height + 10), -Vector2.UnitY, ModContent.ProjectileType<XRift>(), 15, 0);
                            }
                        }
                        if (AITimer >= 480 && AITimer < 541 && AITimer % 20 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                            Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.2f);
                            Projectile.NewProjectile(null, NPC.Center + vel * 15f, vel * 7f, ModContent.ProjectileType<XBolt>(), 15, 0);
                        }
                        if (AITimer == 570)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            for (int i = 1; i < 3; i++)
                            {
                                Vector2 pos = Main.rand.NextVector2FromRectangle(OffsetBoundaries(Vector2.One * 200));
                                disposablePos[i] = pos;
                                Projectile.NewProjectile(null, pos, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                                Projectile.NewProjectile(null, pos, Vector2.Zero, ModContent.ProjectileType<XExplosionTiny>(), 0, 0);
                            }

                        }
                        if (AITimer == 600)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            Projectile.NewProjectile(null, disposablePos[1], Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int j = 0; j < 4; j++)
                            {
                                float angle = Helper.CircleDividedEqually(j, 4) + off;
                                Projectile.NewProjectile(null, disposablePos[1], angle.ToRotationVector2() * (1 + j * 0.5f), ModContent.ProjectileType<XBolt>(), 20, 0);
                            }
                        }
                        if (AITimer == 630)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            Projectile.NewProjectile(null, disposablePos[2], Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                            float off = Main.rand.NextFloat(MathHelper.TwoPi);
                            for (int j = 0; j < 8; j++)
                            {
                                float angle = Helper.CircleDividedEqually(j, 8) + off;
                                Projectile.NewProjectile(null, disposablePos[2], angle.ToRotationVector2() * (3 + j * 0.5f), ModContent.ProjectileType<XBolt>(), 20, 0);
                            }
                        }
                        if (AITimer == 660)
                        {
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0);
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0);
                        }
                        if (AITimer == 1030)
                        {
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ModContent.ProjectileType<SheepeningOrb>(), 1, 0);
                            Projectile.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ModContent.ProjectileType<SheepeningOrb>(), 1, 0);
                        }
                        if (AITimer >= 1100)
                        {
                            Reset();
                            NPC.dontTakeDamage = false;
                            oldAttack = 0;
                            Next = 1;
                            AIState = 1;
                        }
                    }
                    break;
                case Taunt:
                    {
                        if (AITimer < 80 + (NPC.life * 0.01f))
                        {
                            headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            IdleAnimation();
                        }
                        FacePlayer();
                        AITimer2--;

                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("If I were you, I would probably try to steal my staff too!");
                            chat.Add("This is nothing but a slight inconvenience for the Grand Archmage!!");
                            chat.Add("If only you were as good at fighting as you were at opening doors!");
                            chat.Add("You're practically defeating YOURSELF!");
                            chat.Add("Are you intentionally missing? Or is my glory blinding you?");
                            chat.Add("You're unworthy of even witnessing my greatest magic!");
                            chat.Add("My greatest magics would break your mind BEFORE your body!");
                            chat.Add("I bet you can't even begin to comprehend the might of The Archmage!");
                            chat.Add("Your greatest skill is your ability to make bad decisions!");
                            chat.Add("If I were you, I'd bow and let me destroy you!");
                            chat.Add("I can't believe I'm wasting all my glory on this...");
                            chat.Add("Even if you stole it, you would never be worthy enough to wield my Magnificent Staff!");
                            chat.Add("I'm not sure whether your futile attempts to hurt me are depressing or amusing!");
                            chat.Add("I've yet to meet someone that can outsmart spells!");
                            chat.Add("You're not putting a scratch on me!");
                            chat.Add("Feel the power of my MAAAGIIIC!!!");
                            chat.Add("You truly are an amateur among amateurs!");
                            chat.Add("There is no mercy for thieves like you!");
                            chat.Add("If you surrender now, I still won't consider letting you go!");
                            chat.Add("There is no escape, weakling!");
                            chat.Add("My power is MAXIMUM!!!!! *cough*");
                            chat.Add("Unbelievable! I almost envy your persistence!");
                            chat.Add("Unlike you, I am destined for supremacy!");
                            chat.Add("My spells are my power!");
                            chat.Add("DIE!!! HUMAN!! ENEMY!!!!!! *cough, cough*");
                            if (phase2)
                                chat.Add("Hey! QUIT IT! You're scratching up my robe!");
                            if (player.HeldItem != null)
                            {
                                if (player.HeldItem.DamageType == DamageClass.Magic)
                                {
                                    chat.Add("I remember a RAT that was better with magic!");
                                    chat.Add("Even a bunny outmatches you in magic prowess!");
                                    chat.Add("Maybe hone your spells before facing the Grand Archmage!");
                                    chat.Add("You call THAT magic?! How pitiful!");
                                    if (Item.staff[player.HeldItem.type])
                                    {
                                        chat.Add("My staff is bigger than yours!");
                                        chat.Add("You'd be better off using that staff to club me!");
                                    }
                                    chat.Add("For a sorcerer, your power disappoints me...");
                                }
                                if (player.HeldItem.DamageType == DamageClass.Melee)
                                {
                                    chat.Add("A brute like you is unworthy to face someone like me!");
                                    chat.Add("Your strikes are but empty threats to one such as myself!");
                                    chat.Add("A measly blade is all you'll ever be good for!");
                                    if (phase2)
                                        chat.Add("Oh, WHAT?! Is the sword too heavy for your flabby noodle arms?!?");
                                }
                                if (player.HeldItem.DamageType == DamageClass.Ranged)
                                {
                                    chat.Add("Stay true to your aim, then maybe you'll stand a chance!");
                                    chat.Add("Your firepower is nothing compared to the might of my magic!");
                                    chat.Add("Your aim is a disgrace to a disgrace!");
                                    chat.Add("Are you TRYING to hit the wall? Or just blinded by my glory?");
                                    if (player.HeldItem.useAmmo == AmmoID.Bullet)
                                        chat.Add("You fool! I am bulletproof!");
                                    if (player.HeldItem.useAmmo == AmmoID.Arrow)
                                        chat.Add("I would make a better archer than you!");
                                    if (phase2)
                                        chat.Add("STOP GOING FOR CHEAP SHOTS, YOU LITTLE BANDIT!");
                                }
                                if (player.HeldItem.DamageType == DamageClass.SummonMeleeSpeed)
                                    chat.Add("How about I whip YOU into shape?!");
                            }
                            if (player.numMinions > 0)
                            {
                                chat.Add("Coward! Strength in numbers is futile against ME!!!");
                                chat.Add("You dare face me with your sinister spawns?!");
                                chat.Add("You attack me with MINIONS? AT LEAST FACE ME YOURSELF!");
                                chat.Add("How actually DARE you! WOULD YOU LIKE IT IF I SWARMED YOU WITH MINIONS?!");
                                if (player.ownedProjectileCounts[ProjectileID.BabySlime] > 1)
                                    chat.Add("Hold up, slimes?! You're THAT ill-prepared?!");
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer >= 160 + (NPC.life * 0.01f))
                        {
                            Reset();
                            AIState = Next;
                        }
                    }
                    break;
                case Idle:
                    {
                        IdleAnimation();
                        //
                        if (MeleeAttacks.Contains(Next))
                        {
                            FacePlayer();

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 150, false).X * .02f, 0.1f);
                        }
                        else
                        {
                            if (AITimer == 1) disposablePos[9] = Main.rand.NextVector2FromRectangle(GetArenaRect());
                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            if (NPC.Distance(pos) < 70)
                                AITimer++;

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        AITimer2--;

                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);


                        if (AITimer >= 40 + (NPC.life * 0.0065f))
                        {
                            Reset();
                            AIState = Next;
                        }
                    }
                    break;
                case PhantasmalSpirit:
                    {
                        FacePlayer();
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Phantasmal Spirits!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);



                        if (AITimer < 135) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if ((AITimer < 65 && AITimer > 50) || (AITimer < 125 && AITimer > 100))
                        {
                            headFrame.Y = ShockedFace;
                            Vector2 pos = staffTip + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        NPC.velocity.X += -NPC.direction * .05f;
                        NPC.velocity.X *= 0.95f;
                        if (phase2 ? (AITimer >= 75 && AITimer % 15 == 0 && (phaseMult == 3 ? true : AITimer != 105)) : (AITimer == 75 || AITimer == 95 || AITimer == 135 || (phaseMult == 1 ? AITimer == 160 : false)))
                        {
                            NPC.velocity.X += -NPC.direction * 5;
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, staffTip);
                            EbonianSystem.ScreenShakeAmount = 6;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(staffTip, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            if (!phase2)
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * 5f, ModContent.ProjectileType<XSpirit>(), 15, 0);
                            else
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * 6f, ModContent.ProjectileType<XSpirit>(), 15, 0);
                        }
                        if (AITimer == 95 && phaseMult == 3)
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ModContent.ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);
                        if (AITimer >= 150)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case ShadowflamePuddles:
                    {
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 1)
                            AITimer2 = Main.rand.Next(4);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowflame Eruption!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        if (AITimer < 180 && AITimer > 1)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        }
                        else
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        }


                        if (AITimer2 > 0 && phaseMult != 3)
                        {
                            if (AITimer >= 60 && AITimer <= 180 && AITimer % (phase2 ? (phaseMult == 3 ? 10 : 15) : (phaseMult == 1 ? 20 : 25)) == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                Vector2 pos = new Vector2(GetArenaRect().X + GetArenaRect().Width * Main.rand.NextFloat(), NPC.Center.Y);
                                Projectile.NewProjectile(null, pos, AITimer % (phase2 ? (phaseMult == 3 ? 20 : 30) : 50) == 0 ? Vector2.UnitY : -Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 15, 0);
                            }
                        }
                        else//phase 2
                        {
                            if (AITimer == 100)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                for (int i = 0; i < 7; i++)
                                {
                                    Vector2 _pos = GetArenaRect().Left();
                                    Vector2 __pos = GetArenaRect().Right();
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                    if (Helper.TRay.CastLength(pos, -Vector2.UnitY, GetArenaRect().Height + 10) <= GetArenaRect().Height)
                                    {
                                        Projectile p1 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 15, 0);
                                        p1.timeLeft = 868;
                                        p1.ai[1] = 2;
                                    }
                                    Projectile p2 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 15, 0);
                                    if (i > 0)
                                    {
                                        p2.timeLeft = 868;
                                        p2.ai[1] = 2;
                                    }
                                }
                            }
                            if (AITimer == 200 && phaseMult == 3)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                for (int i = 0; i < 7; i++)
                                {
                                    Vector2 _pos = GetArenaRect().Left();
                                    Vector2 __pos = GetArenaRect().Right();
                                    Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                    if (Helper.TRay.CastLength(pos, -Vector2.UnitY, GetArenaRect().Height + 10) <= GetArenaRect().Height)
                                    {
                                        Projectile p1 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 15, 0);
                                        p1.timeLeft = 868;
                                        p1.ai[1] = 2;
                                    }
                                    Projectile p2 = Projectile.NewProjectileDirect(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ModContent.ProjectileType<XShadowflame>(), 15, 0);
                                    if (i > 0)
                                    {
                                        p2.timeLeft = 868;
                                        p2.ai[1] = 2;
                                    }
                                }
                            }
                        }
                        if (AITimer >= (phaseMult == 3 ? 160 : 260))
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case SpectralOrbs:
                    {
                        FacePlayer();
                        headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f); rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        if (AITimer == 30)
                            if (oldAttack != AIState && oldAttack != AmethystBulletHell)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Glittering Amethysts!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);

                        if (AITimer == 50)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                            for (int i = 0; i < 3 + (AITimer / 60); i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                                Projectile.NewProjectileDirect(null, staffTip, Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<XAmethyst>(), 15, 0).timeLeft = 100;
                            }
                        }
                        if (AITimer >= 70 && AITimer <= (Main.expertMode ? 140 : 110) && AITimer % (phase2 ? (phaseMult == 3 ? 15 : 20) : (phaseMult == 1 ? 25 : 30)) == 0)
                        {
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                            for (int i = 0; i < 3 + (AITimer / 60); i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                                Projectile.NewProjectileDirect(null, staffTip, Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<XAmethyst>(), 15, 0).timeLeft = 100;
                            }
                        }

                        if (AITimer >= 150)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case MagnificentFireballs:
                    {
                        if (AITimer == 1)
                        {
                            disposablePos[9] = player.Center;
                            AITimer = Main.rand.Next(2, 20);
                        }
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shimmering Fusillade!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        if (AITimer == 80)
                        {
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                        if (AITimer > 55)
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        if ((AITimer <= 120 && AITimer >= 110) || AITimer == 160 || (AITimer <= 190 && AITimer >= 170) || (AITimer <= 250 && AITimer >= 240))
                        {
                            if (AITimer % (AITimer > 229 ? (phase2 ? (phaseMult == 3 ? 4 : 5) : (phaseMult == 1 ? 5 : 10)) : (phase2 ? (phaseMult == 3 ? 4 : 6) : 10)) == 0)
                            {
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(staffTip, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                                Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * MathHelper.Lerp(1.25f + phaseMult * 0.05f, 0.65f, AITimer / 300) * (AITimer == 110 ? 0 : (AITimer > 229 ? 1.2f : 0.65f)));
                                Projectile.NewProjectile(null, staffTip, vel * 7f, ModContent.ProjectileType<XBolt>(), 15, 0);
                            }
                        }

                        if (AITimer >= 300)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case SineLaser:
                    {
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phase2 ? "Helix" : "Wave") + " of Shadowflame!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        if (AITimer < 80)
                        {
                            disposablePos[0] = player.Center;
                            FacePlayer();
                        }
                        rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot + (AITimer >= 145 ? MathF.Sin(AITimer * 6) * 0.1f : 0), 0.2f);
                        Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                        if (AITimer == 85)
                        {
                            SoundStyle s = EbonianSounds.cursedToyCharge;
                            SoundEngine.PlaySound(s, NPC.Center);

                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, ModContent.DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));

                            Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: (phaseMult == 3 ? 55 : 70));
                            if (phase2)
                                Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0);
                        }
                        if (AITimer == 145)
                        {
                            SoundStyle s = EbonianSounds.eruption;
                            SoundEngine.PlaySound(s, NPC.Center);

                            Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: (phaseMult == 3 ? 55 : 70));
                            if (phase2)
                                Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, staffTip, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0);
                        }
                        if (AITimer >= 300)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case AmethystCloseIn:
                    {
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        rightArmRot = Helper.LerpAngle(rightArmRot, MathHelper.ToRadians(AITimer * 8f), 0.15f);
                        NPC.direction = NPC.spriteDirection = ((NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.PiOver4 * 0.5f) * 20).X > NPC.Center.X ? -1 : 1);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Amethysts of Empowerment!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);

                        if (AITimer % (phase2 ? (phaseMult == 3 ? 10 : 15) : (phaseMult == 1 ? 20 : 25)) == 0 && AITimer > 29 && AITimer < 91)
                        {
                            float off = (phase2 ? AITimer * 0.18f : Main.rand.NextFloat(MathHelper.TwoPi));
                            for (int i = 0; i < 5; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 5) + off;
                                Vector2 pos = NPC.Center + new Vector2(phase2 ? 450 : 300, 0).RotatedBy(angle);
                                Projectile p = Projectile.NewProjectileDirect(null, pos, Helper.FromAToB(pos, NPC.Center) * 0.1f, ModContent.ProjectileType<XAmethystCloseIn>(), 15, 0);
                                p.ai[0] = NPC.Center.X;
                                p.ai[1] = NPC.Center.Y;
                            }
                            Projectile.NewProjectileDirect(null, staffTip, Vector2.Zero, ModContent.ProjectileType<XExplosionTiny>(), 0, 0);
                        }

                        if (AITimer >= 100)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case AmethystBulletHell: //phase 2
                    {
                        headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                        rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-45f) * NPC.direction, 0.15f);
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState && oldAttack != SpectralOrbs)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Glittering Amethysts!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);

                        if (AITimer > 60 && AITimer < 170)
                        {
                            if (AITimer % 15 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
                                for (int i = 0; i < disposablePos.Length; i++)
                                {
                                    if (disposablePos[i] != Vector2.Zero)
                                        continue;
                                    disposablePos[i] = Main.rand.NextVector2FromRectangle(GetArenaRect());

                                    for (int j = 0; j < 5; j++)
                                    {
                                        Dust.NewDustPerfect(disposablePos[i], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                        Dust.NewDustPerfect(disposablePos[i], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));

                                        Dust.NewDustPerfect(staffTip, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                        Dust.NewDustPerfect(staffTip, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[i];
                                    }
                                    break;
                                }
                            }
                        }
                        if (AITimer > 60 && AITimer % 20 == 0)
                        {
                            for (int i = 0; i < disposablePos.Length; i++)
                            {
                                Projectile.NewProjectile(null, disposablePos[i], Helper.FromAToB(disposablePos[i], player.Center + Main.rand.NextVector2Circular(150, 150)) * 0.1f, ModContent.ProjectileType<XAmethystCloseIn>(), 15, 0);

                                if (phaseMult == 3)
                                {
                                    Projectile.NewProjectile(null, disposablePos[i], Helper.FromAToB(disposablePos[i], player.Center) * 0.05f, ModContent.ProjectileType<XAmethystCloseIn>(), 15, 0);
                                }
                                disposablePos[i] = Vector2.Zero;
                            }
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case GiantAmethyst:
                    {
                        headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        rightArmRot = Helper.LerpAngle(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "The Archmage's Great Amethysts!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        if (AITimer >= 60 && AITimer % 25 == 0)
                        {
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ModContent.ProjectileType<XLargeAmethyst>(), 15, 0);
                            Projectile.NewProjectile(null, staffTip, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                        if (AITimer >= (phase2 ? 151 : (phaseMult == 1 ? 100 : 76)))
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case Micolash:
                    {
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        //if (AITimer == 40)
                        //  if (oldAttack != AIState)
                        //    currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Think fast!", Color.Violet, -1, 0.Color.Indigo* 0.5f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f),3);

                        if (AITimer == 40)
                            if (oldAttack != AIState && AITimer3 == 0)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowflame Tendrils!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);

                        if (AITimer < 130)
                        {
                            headFrame.Y = AssholeFace;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.02f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - Helper.FromAToB(NPC.Center, player.Center) * 60, false).X * 0.07f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.7f;
                        }
                        if (AITimer == 130)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        if (AITimer == 150)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0);
                        if (AITimer < 130)
                            disposablePos[0] = player.Center;
                        if (AITimer > 150 && AITimer < 202 && AITimer % 4 == 0)
                        {
                            headFrame.Y = VeryShockedFace;
                            Projectile a = Projectile.NewProjectileDirect(null, NPC.Center - new Vector2(0, 6), Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedByRandom(MathHelper.PiOver4 * 0.5f), ModContent.ProjectileType<XTentacle>(), 15, 0);
                            a.ai[0] = Main.rand.Next(50, 90);
                            a.ai[1] = Main.rand.NextFloat(2.5f, 5f);
                        }

                        if (AITimer >= (phaseMult == 3 ? 220 : 250))
                        {
                            Reset();
                            AITimer3++;
                            if (AITimer3 >= (phase2 ? (phaseMult) : 0))
                            {
                                AITimer3 = 0;
                                PickAttack();
                            }
                            else
                                AITimer = (phaseMult == 3 ? 110 : 80);
                            //AIState = Idle;
                        }
                    }
                    break;
                case TheSheepening:
                    {
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            NPC.direction = disposablePos[9].X < NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        }
                        else
                            NPC.velocity.X *= 0.9f;
                        if (AITimer < (phase2 ? 160 : 70) && AITimer > 50)
                        {
                            FacePlayer();
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 40)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Bleat now!");
                            chat.Add("'Baa! Baa!'... That's you!");
                            chat.Add("The Sheepening!");
                            chat.Add("To the slaughter with you!");
                            chat.Add("Have you any wool?");
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }

                        if (AITimer == 50)
                        {

                            for (int i = 0; i < 15; i++)
                            {
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(17, 17), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        bool shouldAttack = AITimer == 70;
                        if (phase2)
                            shouldAttack = AITimer == 70 || (AITimer >= 120 && AITimer % 20 == 0 && AITimer <= 160);
                        if (shouldAttack)
                        {
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ModContent.ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);
                        }
                        if (phaseMult == 3 && AITimer == 200)
                        {
                            for (int i = -1; i < 2; i++)
                                Projectile.NewProjectileDirect(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity).RotatedBy(i * 0.65f), ModContent.ProjectileType<SheepeningOrb>(), 1, 0, -1).localAI[0] = 1;
                        }
                        if (AITimer >= (phase2 ? 260 : 160))
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case ManaPotion:
                    {
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        headFrame.Y = DisappointedFace;
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
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
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
                            if (phase2)
                                for (int i = 0; i < 2; i++)
                                    Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4, 10), ModContent.ProjectileType<XManaPotion>(), 15, 0);

                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, pos, new Vector2(Helper.FromAToB(pos, player.Center + player.velocity).X * Main.rand.NextFloat(12, 17), -10), ModContent.ProjectileType<XManaPotion>(), 15, 0);
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
                            if (Main.rand.Next((phaseMult == 3 ? 13 : 18)) > 4)
                                PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case PhantasmalBlast: //phase 2
                    {
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            FacePlayer();
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                        {
                            NPC.velocity.X *= 0.9f;
                        }
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        if (AITimer == 30)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Phantasmal Blast!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);

                        if (AITimer == 50)
                        {
                            AITimer2 = 0.5f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosionInvis>(), 0, 0);
                        }

                        if (AITimer < 205 && AITimer > 50) rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        else rightArmRot = Helper.LerpAngle(rightArmRot, 0, 0.2f);

                        if (AITimer > 80 && AITimer < 140 && AITimer % 20 < 10)
                        {
                            headFrame.Y = AngryFace;
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        if (AITimer > 80 && AITimer < 170 && AITimer % (phaseMult == 3 ? 10 : 15) == 0)
                        {
                            FacePlayer();
                            AITimer2 += 0.1f;
                            headFrame.Y = ShockedFace;
                            SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                            EbonianSystem.ScreenShakeAmount = 6;
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver2 * 0.7f * AITimer2) * Main.rand.NextFloat(5, 8), ModContent.ProjectileType<XSpiritNoHome>(), 15, 0, -1, 0.25f);
                        }
                        if (AITimer == 140)
                        {
                            disposablePos[0] = player.Center;
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ModContent.ProjectileType<SheepeningOrb>(), 1, 0, player.whoAmI);
                            else
                            {
                                Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: 7.5f);
                                Projectile.NewProjectile(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 0, 0, ai1: -7.5f);
                            }
                        }
                        if (AITimer == 190 && phaseMult != 3)
                        {
                            headFrame.Y = ShockedFace;
                            for (int i = 0; i < 20; i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                                if (i % 2 == 0)
                                    Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            }
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                            SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, NPC.Center);
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: 7.5f).localAI[0] = 1;
                            Projectile.NewProjectileDirect(null, NPC.Center + vel * 15, vel, ModContent.ProjectileType<XSineLaser>(), 15, 0, ai1: -7.5f).localAI[0] = 1;
                        }

                        if (AITimer >= 240)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case ShadowflameRift:
                    {
                        headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        //more portals in phase 2
                        if (AITimer == 80)
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>();
                            chat.Add("Hold up, I forgot to bring something...");
                            chat.Add("Don't mind me! I just forgot something...");
                            chat.Add("I seem to have forgotten something...");
                            chat.Add("Hehehehe..."); //phase 2
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        }
                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.9f;
                        }
                        if (AITimer > 60 && AITimer < 160)
                        {
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                            if (AITimer < 160)
                                disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);

                            if (AITimer < 85)
                            {
                                disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                            }
                            if (AITimer % 2 == 0)
                            {
                                Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                                Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                                Dust.NewDustPerfect(disposablePos[1], ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(disposablePos[1], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                        }
                        if (AITimer == 85)
                        {
                            Projectile.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ModContent.ProjectileType<XRift>(), 0, 0);
                        }
                        if (AITimer == 140)
                        {
                            Projectile.NewProjectile(null, disposablePos[1], Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, disposablePos[1], -Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                        }

                        if (AITimer == 200 && phase2)
                        {
                            disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);
                            Projectile.NewProjectile(null, disposablePos[1], Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                            if (phaseMult == 3)
                                Projectile.NewProjectile(null, disposablePos[1], -Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                        }

                        if (AITimer == 240 && phaseMult == 3)
                        {
                            disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);
                            Projectile.NewProjectile(null, disposablePos[1], Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                            Projectile.NewProjectile(null, disposablePos[1], -Helper.FromAToB(disposablePos[1], player.Center), ModContent.ProjectileType<XRift>(), 15, 0);
                        }

                        if (AITimer >= 320)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case HelicopterBlades: //phase 2
                    {
                        leftArmRot = Helper.LerpAngle(leftArmRot, 0, 0.3f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                        if (AITimer < 400)
                            FacePlayer();
                        if (AITimer < 405)
                            rightArmRot = Helper.LerpAngle(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);

                        if (AITimer == 40)
                            helicopterSlot = SoundEngine.PlaySound(EbonianSounds.helicopter);
                        if (AITimer >= 40 && AITimer <= 440)
                        {
                            headFrame.Y = AssholeFace;
                            if (AITimer > 80 && AITimer % 10 == 0 && AITimer < 120 || AITimer == 170)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.5f);
                                if (AITimer == 170)
                                    vel = Helper.FromAToB(NPC.Center, player.Center + player.velocity);
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(14 + (NPC.direction == -1 ? 10 : 20), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * 1.11f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(25 * -NPC.direction, 24), vel * 7f, ModContent.ProjectileType<XKnife>(), 15, 0);
                            }
                            heliAlpha = MathHelper.Lerp(heliAlpha, 1, 0.1f);
                            NPC.noGravity = true;
                            if (AITimer > 40 && AITimer < 170)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 300, true) - new Vector2(0, 150), false) / 35, 0.025f);
                            else if (AITimer >= 170 && AITimer < 240)
                                NPC.velocity *= 0.9f;
                            else if (AITimer > 240 && AITimer < 400)
                            {
                                if (AITimer % (phaseMult == 3 ? 15 : 30) == 0)
                                {
                                    SoundEngine.PlaySound(EbonianSounds.xSpirit, NPC.Center);
                                    Projectile.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * 2, ModContent.ProjectileType<XBolt>(), 15, 0);
                                }
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.TRay.Cast(player.Center, Vector2.UnitY, 300, true) - new Vector2(MathF.Sin(AITimer * 3) * 200, 150), false) / 25, 0.05f);
                            }
                            else
                                NPC.velocity *= 0.9f;
                        }
                        if (AITimer > 400 && AITimer < 490)
                        {
                            headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
                            {
                                sound.Volume = MathHelper.Lerp(sound.Volume, 0, 0.1f);
                            }
                            NPC.velocity *= 0.9f;
                            NPC.noGravity = false;
                            heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                            Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.06f, .2f));
                            d.noGravity = true;
                            d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                        }
                        if (AITimer == 470)
                        {
                            disposablePos[0] = Helper.FromAToB(NPC.Center, player.Center);
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<XExplosion>(), 0, 0, ai2: 1);
                        }
                        if (AITimer > 499 && AITimer <= 530)
                        {
                            if (AITimer % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                Projectile.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * 7f, ModContent.ProjectileType<XKnife>(), 15, 0);
                            }
                        }
                        if (AITimer >= 550)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
                case AmethystStorm:
                    {
                        if (AITimer == 40)
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Shadowflame Mist!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                            else
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "And again!", Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                        if (AITimer < 260 && AITimer > 50)
                        {
                            headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : DisappointedFace;
                            rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction + MathHelper.ToRadians(-40f) * NPC.direction, 0.15f);
                            leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction + MathHelper.ToRadians(40f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                            IdleAnimation();
                        }

                        if (AITimer == 1) disposablePos[9] = player.Center;
                        if (AITimer < 50)
                        {

                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[9].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                            NPC.direction = disposablePos[9].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.Grounded())
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        }
                        else
                        {
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                            NPC.velocity.X *= 0.9f;
                        }
                        if (AITimer == 70)
                        {
                            if (phase2)
                            {
                                Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0, player.whoAmI, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0, player.whoAmI, -20, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                                if (phaseMult == 3)
                                    Projectile.NewProjectileDirect(null, GetArenaRect().Center() + new Vector2(0, -150), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0, player.whoAmI, -30, ai1: (phaseMult == 3 ? 1 : 0)).timeLeft = 300;
                            }
                            else
                                Projectile.NewProjectile(null, new Vector2(MathHelper.Clamp(NPC.Center.X, GetArenaRect().X + 40, GetArenaRect().X + GetArenaRect().Width - 40), MathHelper.Clamp(NPC.Center.Y - 120, GetArenaRect().Y + 70, GetArenaRect().Y + GetArenaRect().Width - 70)), Vector2.Zero, ModContent.ProjectileType<XCloud>(), 0, 0, player.whoAmI);
                        }

                        if (AITimer >= 280)
                        {
                            Reset();
                            PickAttack();
                            //AIState = Idle;
                        }
                    }
                    break;
            }
        }
        // TODO: Detached head, Drinkable mana potion, Mana system, smoother movement, improved arm movement, phase 2 attack changes
        Vector2 sCenter;
        Rectangle GetArenaRect()
        {
            float LLen = Helper.TRay.CastLength(sCenter, -Vector2.UnitX, 29f * 16);
            float RLen = Helper.TRay.CastLength(sCenter, Vector2.UnitX, 29f * 16);
            Vector2 U = Helper.TRay.Cast(sCenter, -Vector2.UnitY, 380);
            Vector2 D = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 380);
            sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
            Vector2 L = sCenter;
            Vector2 R = sCenter;
            if (LLen > 400)
            {
                if (LLen > RLen)
                {
                    R = Helper.TRay.Cast(sCenter, Vector2.UnitX, 29f * 16);
                    L = Helper.TRay.Cast(R, -Vector2.UnitX, 34.5f * 32);
                }
                else
                {
                    R = Helper.TRay.Cast(L, Vector2.UnitX, 34.5f * 32);
                    L = Helper.TRay.Cast(sCenter, -Vector2.UnitX, 29f * 16);
                }
            }
            else
            {
                R = sCenter + new Vector2(495, 0);
                L = sCenter - new Vector2(464, 0);
            }
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
            return (player.Center.Y > NPC.Center.Y + 50 && (AIState == Idle || AIState == Spawn || (AIState == Micolash && AITimer < 130))) || NPC.noGravity;
        }
        void Reset()
        {
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Stop();
            }
            headOffIncrementOffset = Main.rand.NextFloat(500);
            AITimer = 0;
            AITimer2 = 0;
            NPC.noGravity = false;
            manaPotAlpha = 0;
            staffAlpha = 1;
            heliAlpha = 0;
            NPC.velocity.X = 0;
            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            NPC.damage = 0;
        }
    }
}
