using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Skies;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.ArchmageX
{
    public class ArchmageCutsceneMartian : ModNPC
    {
        public override string Texture => "EbonianMod/NPCs/ArchmageX/ArchmageX";
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
        float leftArmRot, rightArmRot;
        float headRotation, headYOff, headOffIncrementOffset;
        float staffAlpha = 1f, arenaVFXOffset, arenaAlpha;
        Rectangle headFrame = new Rectangle(0, AssholeFace, 36, 42);
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(50, 78);
            NPC.lifeMax = 9999;
            NPC.defense = -10;
            NPC.damage = 0;
            NPC.boss = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.lavaImmune = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            Music = MusicID.MartianMadness;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy || AITimer < 1) return false;
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D burnt = Helper.GetTexture(Texture + "_Burnt");
            Texture2D singularArm = Helper.GetTexture(Texture + "_Arm");
            Texture2D head = Helper.GetTexture(Texture + "_Head");
            Texture2D headGlow = Helper.GetTexture(Texture + "_HeadGlow");

            Texture2D fireG = Helper.GetTexture("NPCs/ArchmageX/XFire_Gray");
            Texture2D fireP = Helper.GetTexture("NPCs/ArchmageX/XFire_Purple");
            Texture2D headBloom = Helper.GetTexture(Texture + "_Head_Bloom");
            Texture2D martian = TextureAssets.Npc[NPCID.MartianSaucer].Value;
            Texture2D staff = Helper.GetTexture("Items/Weapons/Magic/StaffOfXItem");

            if (AITimer < 550)
            {
                Main.spriteBatch.Reload(BlendState.Additive);

                spriteBatch.Draw(headBloom, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, null, Color.White * 0.2f, headRotation, headBloom.Size() / 2, NPC.scale + headYOff * 0.05f, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

                Main.spriteBatch.Reload(BlendState.AlphaBlend);
                Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 0.25f;
                if (NPC.direction == 1)
                    staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -10) + (rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction)).ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -.25f;
                float staffRot = rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) + MathHelper.Pi * (NPC.direction == 1 ? .5f : 1f);

                //spriteBatch.Draw(staff, staffP - screenPos, null, Color.White * staffAlpha, staffRot, new Vector2(0, staff.Height), NPC.scale, SpriteEffects.None, 0f);

                spriteBatch.Draw(singularArm, NPC.Center - new Vector2(NPC.direction == -1 ? -14 : -6, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * -NPC.direction), new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

                spriteBatch.Draw(singularArm, NPC.Center - new Vector2(singularArm.Width - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot + (NPC.direction == -1 ? MathHelper.ToRadians(15) : 0) + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) - (NPC.direction == -1 ? MathHelper.PiOver4 * 0.5f : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

                spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(head, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

                spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(headGlow, NPC.Center + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            else
            {
                spriteBatch.Draw(burnt, NPC.Center - screenPos, null, drawColor, NPC.rotation, burnt.Size() / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
            Rectangle flameRect = new Rectangle(0, flameFrame, 32, 34);
            spriteBatch.Draw(fireP, NPC.Center - new Vector2(NPC.direction == -1 ? -44 : 20, 24 - headYOff * 2).RotatedBy(NPC.rotation) - screenPos, flameRect, Color.White * AITimer2, 0, new Vector2(16), 1 + headYOff * 0.025f, SpriteEffects.None, 0);

            spriteBatch.Draw(fireG, NPC.Center - new Vector2(singularArm.Width - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -70 : 4, 24 - headYOff * 2).RotatedBy(NPC.rotation) - screenPos, flameRect, Color.White * AITimer2, 0, new Vector2(16), 1 + headYOff * 0.025f, SpriteEffects.None, 0);



            if (beamAlpha > 0)
            {

                vfxOffset -= 0.015f;
                if (vfxOffset <= 0)
                    vfxOffset = 1;
                vfxOffset = MathHelper.Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);

                List<VertexPositionColorTexture> vertices = new();

                Vector2 startPosition = startP - new Vector2(-6, 600 - AITimer3) - screenPos;
                Vector2 endPosition = startP - new Vector2(-6, 600 - AITimer3) + new Vector2(0, 2000) - screenPos;

                for (float i = 0; i < 1; i += 0.001f)
                {
                    Vector2 pos = Vector2.Lerp(startPosition, endPosition, i);
                    float rot = Helper.FromAToB(startPosition, endPosition).ToRotation();

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + i;
                    float alphaMult = beamAlpha;
                    Color c = new Color(255, 255, 210);
                    vertices.Add(Helper.AsVertex(pos + new Vector2(MathHelper.SmoothStep((100 + MathF.Cos((Main.GlobalTimeWrappedHourly) * 100) * 4) * beamAlpha, 5 * beamAlpha, MathHelper.Lerp(1, 0, MathHelper.Clamp(i * 10, 0, 1))), 0).RotatedBy(MathHelper.PiOver2 + rot), new Vector2(_off, 0), c * alphaMult));
                    vertices.Add(Helper.AsVertex(pos + new Vector2(MathHelper.SmoothStep((100 + MathF.Cos((Main.GlobalTimeWrappedHourly) * 100) * 4) * beamAlpha, 5 * beamAlpha, MathHelper.Lerp(1, 0, MathHelper.Clamp(i * 10, 0, 1))), 0).RotatedBy(-MathHelper.PiOver2 + rot), new Vector2(_off, 1), c * alphaMult));
                }
                Main.spriteBatch.SaveCurrent();
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                if (vertices.Count > 2)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("saucerBeam"), false);
                }
                vertices.Clear();
                for (float i = 0; i < 1; i += 0.001f)
                {
                    Vector2 pos = Vector2.Lerp(startPosition, endPosition, i);
                    float rot = Helper.FromAToB(startPosition, endPosition).ToRotation();

                    float __off = vfxOffset;
                    if (__off > 1) __off = -__off + 1;
                    float _off = __off + i;
                    float alphaMult = beamAlpha * 5;
                    Color c = Color.White;
                    vertices.Add(Helper.AsVertex(pos + new Vector2(MathHelper.SmoothStep((100 + MathF.Cos((Main.GlobalTimeWrappedHourly) * 100) * 2) * beamAlpha, 5 * beamAlpha, MathHelper.Lerp(1, 0, MathHelper.Clamp(i * 10, 0, 1))), 0).RotatedBy(MathHelper.PiOver2 + rot), new Vector2(_off, 0), c * alphaMult));
                    vertices.Add(Helper.AsVertex(pos + new Vector2(MathHelper.SmoothStep((100 + MathF.Cos((Main.GlobalTimeWrappedHourly) * 100) * 2) * beamAlpha, 5 * beamAlpha, MathHelper.Lerp(1, 0, MathHelper.Clamp(i * 10, 0, 1))), 0).RotatedBy(-MathHelper.PiOver2 + rot), new Vector2(_off, 1), c * alphaMult));
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                if (vertices.Count > 2)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("Tentacle"), false);
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("FlamesSeamless"), false);
                }
                Main.spriteBatch.ApplySaved();
            }
            spriteBatch.Draw(martian, startP - new Vector2(-6, 600 - AITimer3) - screenPos, new Rectangle(0, flameFrame / 34 * 96, 210, 96), Color.White * (AITimer3 == 0 ? 0 : 1), NPC.rotation, new Vector2(210, 96) / 2, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
        int blinkInterval;
        float vfxOffset;
        int frameBeforeBlink, flameFrame;
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
        float beamAlpha;
        public void FacePlayer()
        {
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;
        }
        Vector2 startP;
        public override void OnSpawn(IEntitySource source)
        {
            if (NPC.ai[0] != -1)
            {
                NPC.active = false;
                return;
            }
        }
        public override void AI()
        {
            if (!NPC.AnyNPCs(NPCID.MartianSaucer))
            {
                if (AITimer == 0)
                {
                    int atts = 0;
                    foreach (Player _player in Main.player)
                    {
                        if (_player.active && !_player.dead)
                            _player.statLife = _player.statLifeMax2;
                    }
                    while (atts++ < 400 && (Main.tile[NPC.Center.ToTileCoordinates().X, NPC.Center.ToTileCoordinates().Y].HasTile || Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 700) < 650))
                        NPC.Center -= Vector2.UnitY * 8;
                    startP = NPC.Center;
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.active && npc.type == NPCID.MartianSaucer)
                        {
                            npc.life = 0;
                            npc.checkDead();
                        }
                        if (npc.active && !npc.friendly && npc.type != Type && npc.aiStyle != NPCAIStyleID.CelestialPillar)
                            npc.active = false;
                    }
                    foreach (Projectile p in Main.ActiveProjectiles)
                    {
                        if (p.active && !p.friendly)
                            p.active = false;
                    }
                    EbonianSystem.ChangeCameraPos(NPC.Center, 500, 1.2f);
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                    for (int i = 0; i < 20; i++)
                        Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XCloudVFXExtra>(), 0, 0);
                    for (int i = 0; i < 15; i++)
                        Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if ((!npc.friendly || npc.type == NPCType<ArchmageStaffNPC>()) && npc.type != Type)
                        npc.active = false;
                }
                if (AITimer % 5 == 0)
                {
                    if (flameFrame > 34 * 2)
                        flameFrame = 0;
                    else
                        flameFrame += 34;
                }
                NPC.DiscourageDespawn(10);
                EbonianSystem.xareusFightCooldown = 500;
                if (AITimer > 180 || AITimer < 100)
                {
                    blinkInterval++;
                    if (blinkInterval >= 170 && blinkInterval < 175)
                    {
                        if (headFrame.Y != BlinkingFace)
                        {
                            frameBeforeBlink = headFrame.Y;
                        }
                        headFrame.Y = BlinkingFace;
                    }
                    if (blinkInterval == 176)
                    {
                        headFrame.Y = frameBeforeBlink;
                        blinkInterval = Main.rand.Next(-250, 10);
                    }
                }
                Player player = Main.player[NPC.target];
                NPC.TargetClosest(false);
                if (NPC.direction != NPC.oldDirection)
                {
                    rightArmRot = 0;
                }
                if (AITimer < 550)
                    NPC.Center = startP + new Vector2(0, MathF.Sin(AITimer * 0.005f) * 25);

                float rightHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
                float leftHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver2 : MathHelper.PiOver4);
                Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
                if (NPC.direction == 1)
                    staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;

                float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : 1f);
                Vector2 staffTip = staffP + staffRot.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * 48;
                headYOff = MathHelper.Lerp(headYOff, MathF.Sin((AITimer + headOffIncrementOffset) * 0.05f) * 2, 0.2f);

                Lighting.AddLight(staffTip, TorchID.Purple);

                AITimer++;

                FacePlayer();
                // ARMS
                if (AITimer < 100)
                {
                    rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction * 0.25f + MathHelper.ToRadians(-5f - AITimer * 0.1f) * NPC.direction, 0.3f);
                    leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction * 0.25f + MathHelper.ToRadians(5f + AITimer * 0.1f) * NPC.direction, 0.3f);
                }
                if (AITimer > 100 && AITimer < 180)
                {
                    rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction * 0.15f + MathHelper.ToRadians(-5f) * NPC.direction, 0.3f);
                    leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction * 0.15f + MathHelper.ToRadians(5f) * NPC.direction, 0.3f);
                }
                if (AITimer > 180 && AITimer < 265)
                {
                    float fac = MathHelper.Lerp(0.2f, 1, (AITimer - 180) / 85f);
                    rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction * 0.5f + (MathHelper.ToRadians(-5f) - MathF.Sin(AITimer * 0.35f * fac) * .6f) * NPC.direction, 0.2f);
                    leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction * 0.5f + (MathHelper.ToRadians(5f) + MathF.Sin(AITimer * 0.35f * fac) * .6f) * NPC.direction, 0.2f);
                }
                if (AITimer >= 265)
                {
                    rightArmRot = Helper.LerpAngle(rightArmRot, -NPC.direction * 0.65f + MathHelper.ToRadians(-5f) * NPC.direction, 0.2f);
                    leftArmRot = Helper.LerpAngle(leftArmRot, NPC.direction * 0.65f + MathHelper.ToRadians(5f) * NPC.direction, 0.2f);
                }
                // DIALOGUE
                if (AITimer == 1)
                {
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "SURPRISE ATTACK!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 8);
                }
                if (AITimer == 100)
                {
                    headFrame.Y = BlinkingFace;
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "WHILE YOU'VE BEEN FOOLING AROUND...", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 6);
                }
                if (AITimer == 180)
                {
                    NPC.boss = true;
                    EbonianSystem.ChangeCameraPos(NPC.Center, 120, 1.5f);
                    headFrame.Y = SmirkFace;
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "I'VE AWOKEN MY TRUE POWER!!!", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                }
                if (AITimer == 300)
                {
                    EbonianSystem.ChangeCameraPos(NPC.Center, 300, 1.05f);
                    headFrame.Y = DisappointedFace;
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "And with this power....", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 5);
                }
                if (AITimer == 380)
                {
                    EbonianSystem.ChangeCameraPos(NPC.Center, 300, 1.05f);
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "You....", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 4);
                }
                if (AITimer == 410)
                    EbonianSystem.ChangeCameraPos(NPC.Center - new Vector2(0, 200), 300, 1.05f);
                if (AITimer == 460)
                {
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "Will....", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 4);
                }
                if (AITimer == 680)
                {
                    EbonianSystem.ChangeCameraPos(NPC.Center, 300, 1.05f);
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "...", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 4);
                }
                if (AITimer == 750)
                {
                    EbonianSystem.ChangeCameraPos(NPC.Center, 100, 1.05f);
                    DialogueSystem.NewDialogueBox(80, NPC.Center - new Vector2(0, 80), "Ow.", Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 2f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f), 4);
                }
                if (AITimer > 800)
                {
                    if (NPC.Grounded()) AITimer = 1900;
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, 25, MathHelper.Lerp(0, 0.2f, (AITimer - 800) / 300));
                    NPC.rotation += MathHelper.ToRadians((AITimer - 650) / 300);
                }
                if (AITimer == 1900)
                {
                    GetInstance<EbonianSystem>().xareusFuckingDies = true;
                    Main.musicVolume = vol;
                    NPC.life = 0; for (int i = 0; i < 80; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center, DustID.Ash, Main.rand.NextVector2Circular(10, 10), newColor: Color.Black);
                    }
                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2CircularEdge(7, 7), ProjectileID.CrossGraveMarker, 0, 0, -1);
                    NPC.checkDead();
                }
                // VFX

                if (AITimer > 265 && AITimer < 270)
                    AITimer2 = MathHelper.Lerp(AITimer2, 1, 0.35f);
                if (AITimer == 270)
                    AITimer2 = 1;
                if (AITimer == 265)
                {
                    vol = Main.musicVolume;
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                    SoundEngine.PlaySound(SoundID.Item103, NPC.Center);
                    Vector2 rPos = NPC.Center - new Vector2(NPC.direction == -1 ? -44 : 20, 24 - headYOff * 2).RotatedBy(NPC.rotation);
                    Vector2 lPos = NPC.Center - new Vector2(26 - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -70 : 4, 24 - headYOff * 2).RotatedBy(NPC.rotation);
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(rPos, DustID.DemonTorch, Main.rand.NextVector2Circular(5, 5));
                        Dust.NewDustPerfect(lPos, DustID.WhiteTorch, Main.rand.NextVector2Circular(5, 5), newColor: Color.Black);
                    }
                }
                if (AITimer == 390)
                    AITimer3 = 1;
                if (AITimer > 420 && AITimer < 500)
                    AITimer3 = MathHelper.Lerp(AITimer3, 300, 0.001f);
                if (AITimer > 650)
                {
                    beamAlpha = 0;
                    AITimer3 = MathHelper.Lerp(AITimer3, -2000, MathHelper.Lerp(0, 0.01f, (AITimer - 650) / 100));
                }

                if (AITimer == 520)
                {
                    Main.musicVolume = 0;
                    beamAlpha = 1f;
                    AITimer2 = 0;
                    SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 15, 6, 30, 1000));
                }
                if (AITimer > 600)
                    beamAlpha = MathHelper.Lerp(beamAlpha, 0, 0.3f);

            }
        }
        float vol;
    }
    public class SpawnTheThing : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/IchorGlobSmall";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Bullet);
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 55 && Projectile.ai[1] == 1 && Projectile.ai[2] == -1)
                NPC.NewNPCDirect(null, Main.player[0].Center, NPCType<ArchmageCutsceneMartian>(), 0, -1);
            Projectile.Kill();
        }
    }
}