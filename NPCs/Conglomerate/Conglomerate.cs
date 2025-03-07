﻿using System;
using System.Linq;
using ReLogic.Utilities;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.NPCs.Corruption.Ebonflies;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.Enemy.Corruption;
using EbonianMod.Projectiles.Conglomerate;
using EbonianMod.Dusts;
using Terraria.Graphics.CameraModifiers;
using Terraria.GameContent.RGB;
using EbonianMod.Common.CameraModifiers;
using Terraria.WorldBuilding;
using EbonianMod.Common.Systems.Verlets;

namespace EbonianMod.NPCs.Conglomerate
{
    public class Conglomerate : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
        }
        public override void SetDefaults()
        {
            NPC.width = 124;
            NPC.height = 108;
            NPC.lifeMax = 36000;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.defense = 23;
            NPC.knockBackResist = 0;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = true;
            NPC.aiStyle = -1;
            SoundStyle death = EbonianSounds.cecitiorDie;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = death;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Ichor] = true;
            NPC.netAlways = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/conglomerate");
        }
        bool isLasering, isScreaming;
        void StartLasering()
        {
            NPC.frame.X = 126 * 2;
            NPC.frame.Y = 7 * 110;
            isLasering = true;
            NPC.frameCounter = 1;
        }
        void StartScreaming()
        {
            for (int i = 0; i < verletRotSeeds.Length; i++)
                verletRotSeeds[i] = Main.rand.Next(ushort.MaxValue);
            NPC.frame.X = 126;
            NPC.frame.Y = 5 * 110;
            isScreaming = true;
            NPC.frameCounter = 1;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 126;
            NPC.frameCounter++;
            if (isLasering)
            {
                NPC.frame.X = 126 * 2;
                if (NPC.frameCounter % 3 == 0)
                {
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y > frameHeight * 9)
                    NPC.frame.Y = frameHeight * 6;
            }
            else if (isScreaming)
            {
                NPC.frame.X = 126;
                if (NPC.frameCounter % 3 == 0)
                {
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y > frameHeight * 6)
                    NPC.frame.Y = frameHeight * 5;
            }
            else
            {
                NPC.frame.X = 0;
                if (NPC.frameCounter % 5 == 0)
                {
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y > frameHeight * 4)
                    NPC.frame.Y = 0;
            }

            if (NPC.frameCounter % 5 == 0)
            {

                hookFrame++;
                if (hookFrame > 7 || hookFrame < 1)
                    hookFrame = 1;
            }
        }
        Vector2 openOffset;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return false;
            Vector2 shakeOffset = Main.rand.NextVector2Circular(shakeVal, shakeVal);
            Texture2D tex = Helper.GetTexture(Texture);
            Texture2D glow = Helper.GetTexture(Texture + "_Glow");

            Texture2D part = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT");
            Texture2D part2 = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC");
            Texture2D partTeeth = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT_Teeth");
            Texture2D part2Teeth = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC_Teeth");
            Texture2D partGlow = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartT_Glow");
            Texture2D part2Glow = Helper.GetTexture("NPCs/Conglomerate/Conglomerate_PartC_Glow");
            if (tendonVerlet[0] != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = Vector2.Zero;
                    Vector2 offset2 = Vector2.Zero;
                    float scale = 2;
                    switch (i)
                    {
                        case 1:
                            scale = 4;
                            offset = new Vector2(4, 28);
                            offset2 = new Vector2(-7, -10);
                            break;
                        case 2:
                            scale = 2;
                            offset = new Vector2(10, -20);
                            offset2 = new Vector2(-5, 20);
                            break;
                        case 3:
                            scale = 3;
                            offset = new Vector2(-15, -10);
                            offset2 = new Vector2(-15, -20);
                            break;
                        case 4:
                            scale = 1;
                            offset = new Vector2(1, 20);
                            offset2 = new Vector2(-10, 32);
                            break;
                    }
                    tendonVerlet[i].Update(NPC.Center + offset2.RotatedBy(NPC.rotation) - openOffset, NPC.Center + openOffset + new Vector2(30, 4) + offset.RotatedBy(openRotation));
                    if (tendonVerlet[i].segments[10].cut)
                        tendonVerlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Lerp(Color.Maroon, Color.DarkGreen, (float)i / 5) * 0.25f, scale: scale);
                }
            }
            DrawVerlets(spriteBatch);
            if (openOffset.Length() > 1 || !Helper.CloseTo(openRotation, 0, ToRadians(5)))
            {
                spriteBatch.Draw(partTeeth, NPC.Center + shakeOffset + new Vector2(30, 0) + openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center + new Vector2(30, 4) + openOffset) * 1.25f), openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2Teeth, NPC.Center + shakeOffset - openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center - openOffset) * 1.25f), NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                if (tendonVerlet[0] != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float scale = 2;
                        switch (i)
                        {
                            case 1:
                                scale = 4;
                                break;
                            case 2:
                                scale = 2;
                                break;
                            case 3:
                                scale = 3;
                                break;
                            case 4:
                                scale = 1;
                                break;
                        }
                        if (!tendonVerlet[i].segments[10].cut)
                            tendonVerlet[i].Draw(spriteBatch, "Extras/Line", useColor: true, color: Color.Lerp(Color.Maroon, Color.DarkGreen, (float)i / 5) * 0.25f, scale: scale);
                    }
                }
                spriteBatch.Draw(part, NPC.Center + shakeOffset + new Vector2(30, 0) + openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center + new Vector2(30, 4) + openOffset) * 1.25f), openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2, NPC.Center + shakeOffset - openOffset - screenPos, null, new Color(Lighting.GetSubLight(NPC.Center - openOffset) * 1.25f), NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(partGlow, NPC.Center + shakeOffset + new Vector2(30, 0) + openOffset - screenPos, null, Color.White, openRotation, part.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(part2Glow, NPC.Center + shakeOffset - openOffset - screenPos, null, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                switch (deathSprite)
                {
                    case 0:
                        spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
                        break;
                    case 1:
                        spriteBatch.Draw(TextureAssets.Npc[NPCType<Cecitior.Cecitior>()].Value, NPC.Center - screenPos, new Rectangle(0, 0, 118, 100), drawColor, NPC.rotation, new Vector2(118, 100) / 2, NPC.scale, SpriteEffects.None, 0);
                        spriteBatch.Draw(Helper.GetTexture("NPCs/Cecitior/Cecitior_Glow"), NPC.Center - screenPos, new Rectangle(0, 0, 118, 100), Color.White, NPC.rotation, new Vector2(118, 110) / 2, NPC.scale, SpriteEffects.None, 0);
                        break;
                    case 2:
                        spriteBatch.Draw(TextureAssets.Npc[NPCType<Terrortoma.Terrortoma>()].Value, NPC.Center - screenPos, new Rectangle(0, 0, 118, 106), drawColor, NPC.rotation, new Vector2(118, 106) / 2, NPC.scale, SpriteEffects.None, 0);
                        break;
                }

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
        const int Intro = 0, Idle = 1,
            // Generic Attacks
            BloodAndWormSpit = 2, HomingEyeAndVilethornVomit = 3, BloatedEbonflies = 4, CursedFlameRain = 5, Chomp = 6, ExplodingBalls = 7,
            BayleLaser = 8, SpinAroundThePlayerAndDash = 9, SpikesCloseIn = 10, BothHalvesRainCloseIn = 11, DashSpam = 12,
            OstertagiExplosion = 13, IchorBomb = 14, InstantRays = 15, ClawAtFlesh = 16, GrabAttack = 17, ClingerHipfire = 18,
            EyeBeamPlusHomingEyes = 19, ClawTantrum = 20, CecitiorCloneDash = 21, GeyserSweep = 22, BitesEyeToRainBlood = 23,
            PowerOfFriendship = 24, ClawGrindTheFloorAndSpawnBlood = 25, ClawTriangle = 26, ClingerComboType1 = 27,
            EyeSpin = 28, SmallDeathRay = 29, OpenAndShootStuff = 30, HoldsUpOrbThatShootsLasers = 31,
            ClingerComboType2 = 32, SlamSpineAndEyeTogether = 33, BodySlamTantrum = 34, ClingerComboType3 = 35,
            SpineChargedFlame = 36, CecitiorClonesChompGrid = 37, LaserBallRalken = 38, CecitiorCloneChompCircular = 39,

            Death = -2, PhaseTransition = -1;
        SlotId savedSound, laserSound, openSound;
        Vector2[] disposablePos = new Vector2[10];
        float rotation, openRotation;
        bool phase2, open, hasDied;
        float shakeVal;
        float Next;
        int deathSprite;
        FocusCameraModifier deathCamModifier;
        public override bool CheckDead()
        {
            if (!hasDied)
            {
                NPC.life = 1;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                Reset();
                AIState = Death;
                hasDied = true;
                return false;
            }
            return true;
        }
        public override void AI()
        {
            bool t = true;
            if (NPC.life < NPC.lifeMax / 2 + NPC.lifeMax / 4 && !phase2)
            {
                //Reset();
                /*foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.hostile && projectile.active)
                        projectile.Kill();
                }*/
                phase2 = true;
                //Next = PhaseTransition;
            }
            if (Main.dayTime)
            {
                Main.time = 31400;
                Main.UpdateTime_StartNight(ref t);
            }
            MiscFX();
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)// || !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    if (AIState != Intro)
                        AIState = Idle;
                    AITimer = 0;
                }
                if (!player.active || player.dead)// || !player.ZoneCrimson)
                {
                    DefaultVerletAnimation();
                    AIState = -12124;
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 60)
                    {
                        NPC.timeLeft = 60;
                    }
                    return;
                }
            }

            if (open)
            {
                NPC.damage = 0;
            }
            else
            {
                openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.5f);
                if ((openOffset.Length() < 2.5f && openOffset.Length() > 1f) || (openOffset.Length() > -2.5f && openOffset.Length() < -1f))
                {
                    if (SoundEngine.TryGetActiveSound(openSound, out var sound) && AITimer > 60)
                    {
                        sound.Stop();
                    }
                    SoundEngine.PlaySound(EbonianSounds.cecitiorClose, NPC.Center);
                    CameraSystem.ScreenShakeAmount = 5;
                }
                if (openOffset != Vector2.Zero)
                    if (player.Center.Distance(NPC.Center - openOffset) < 75 || player.Center.Distance(NPC.Center + openOffset) < 75)
                        player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 150, 0);
            }

            NPC.rotation = Helper.LerpAngle(NPC.rotation, rotation, 0.05f);
            AITimer++;
            switch (AIState)
            {
                case Death:
                    {
                        if (AITimer2 == 0 && AITimer < 350)
                        {
                            deathCamModifier = new FocusCameraModifier(NPC.Center, 650, 1.5f, InOutCirc);
                            Main.instance.CameraModifiers.Add(deathCamModifier);
                            AITimer2 = 30;
                            StartScreaming();
                        }
                        if (AITimer == 50)
                        {
                            spineVerlet.verlet.gravity = 10;
                            clingerVerlet.verlet.gravity = 10;
                            eyeVerlet.verlet.gravity = 10;
                            armVerlet.verlet.gravity = 10;
                            for (int i = 0; i < 3; i++)
                                clawVerlet[i].verlet.gravity = 10;
                        }

                        if (AITimer > 50 && AITimer < 350)
                        {
                            MoveVerlet(ref spineVerlet, NPC.Center + new Vector2(30, -50));
                            MoveVerlet(ref clingerVerlet, NPC.Center + new Vector2(40, -20));
                            MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(-30, 50));
                            MoveVerlet(ref armVerlet, NPC.Center + new Vector2(-50, -50));
                            for (int i = 0; i < 3; i++)
                            {
                                Vector2 off = new Vector2(-50, -40);
                                if (i == 1) off = new Vector2(-50, -20);
                                if (i == 2) off = new Vector2(-50, 50);
                                MoveVerlet(ref clawVerlet[i], NPC.Center + off);
                            }
                            if (++AITimer3 > AITimer2 * 4)
                            {
                                EbonianSystem.conglomerateSkyFlash = 10;
                                GetInstance<EbonianSystem>().conglomerateSkyColorOverride = new Color(Main.rand.Next(255), Main.rand.Next(255), Main.rand.Next(255));
                                AITimer3 = 0;
                            }
                            AITimer2 = Lerp(AITimer2, 5, 0.01f);
                            if (AITimer % (int)AITimer2 == 0)
                            {
                                spineVerlet.verlet.gravityDirection = Main.rand.NextVector2Unit();
                                clingerVerlet.verlet.gravityDirection = Main.rand.NextVector2Unit();
                                eyeVerlet.verlet.gravityDirection = Main.rand.NextVector2Unit();
                                armVerlet.verlet.gravityDirection = Main.rand.NextVector2Unit();
                                for (int i = 0; i < 3; i++)
                                    clawVerlet[i].verlet.gravityDirection = Main.rand.NextVector2Unit();
                                MoveVerlet(ref spineVerlet, NPC.Center + new Vector2(30, -50) + Main.rand.NextVector2Circular(50, 50), 1);
                                MoveVerlet(ref clingerVerlet, NPC.Center + new Vector2(40, -20) + Main.rand.NextVector2Circular(50, 50), 1);
                                MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(-30, 50) + Main.rand.NextVector2Circular(50, 50), 1);
                                MoveVerlet(ref armVerlet, NPC.Center + new Vector2(-50, -50) + Main.rand.NextVector2Circular(50, 50), 1);
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 off = new Vector2(-50, -40);
                                    if (i == 1) off = new Vector2(-50, -20);
                                    if (i == 2) off = new Vector2(-50, 50);
                                    MoveVerlet(ref clawVerlet[i], NPC.Center + off + Main.rand.NextVector2Circular(50, 50), 1);
                                }

                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<BlurScream>(), 0, 0);
                                NPC.rotation = rotation + Main.rand.NextFloat(-PiOver4, PiOver4);
                                for (int i = 0; i < 5; i++)
                                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(500, 500) + Main.rand.NextVector2Circular(300, 300), DustType<IntenseDustFollowPoint>(), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()), Scale: 0.1f).customData = NPC.Center - new Vector2(-4, 16);
                                if (AITimer < 250)
                                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit(), ProjectileType<CBeamInstant>(), 20, 0);
                                deathSprite++;
                                if (deathSprite > 2)
                                    deathSprite = 0;
                            }
                        }
                        else if (AITimer <= 50)
                        {
                            DefaultVerletAnimation();
                            if (AITimer == 2)
                            {
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        }
                        else
                        {
                            DefaultVerletAnimation();
                            deathSprite = 0;
                            if (AITimer == 350)
                            {
                                isScreaming = false;
                                NPC.rotation = 0;
                                spineVerlet.verlet.gravity = 0;
                                clingerVerlet.verlet.gravity = 0;
                                eyeVerlet.verlet.gravity = 0;
                                armVerlet.verlet.gravity = 0;
                                for (int i = 0; i < 3; i++)
                                    clawVerlet[i].verlet.gravity = 0;
                                AITimer2 = 0;
                                AITimer3 = 0;
                                CameraSystem.ChangeZoom(300, new ZoomInfo(1.25f, 2, InOutElastic, InOutCirc));
                                for (int i = 0; i < 30; i++)
                                    Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(500, 500) + Main.rand.NextVector2Circular(300, 300), DustType<IntenseDustFollowPoint>(), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()), Scale: 0.1f).customData = NPC.Center;
                            }
                            AITimer3++;
                            if (AITimer3 < 90)
                            {
                                deathCamModifier._pos = Vector2.Lerp(deathCamModifier._pos, NPC.Center + new Vector2(0, 200) - new Vector2(Main.screenWidth, Main.screenHeight) / 2, 0.1f);
                                Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedBy(NPC.rotation).RotatedByRandom(PiOver2 * 0.7f);
                                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10), newColor: Color.Lerp(Color.LawnGreen, Color.Maroon, Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.06f, 0.2f)).customData = NPC.Center + new Vector2(0, 20);

                                Dust.NewDustPerfect(pos, DustID.CursedTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                                Dust.NewDustPerfect(pos, DustID.IchorTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                            }
                            int amt = 3;

                            if (AITimer3 == 90)
                            {
                                StartLasering();
                                if (!(AITimer > 350 + 350 && AITimer < 350 + 350 * 2))
                                {
                                    CameraSystem.ScreenShakeAmount = 20f;
                                    SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                                    laserSound = SoundEngine.PlaySound(s.WithPitchOffset(0.25f), NPC.Center);

                                    CachedSlotIdsSystem.loopedSounds.Add("CBeam3", new(SoundEngine.PlaySound(EbonianSounds.deathrayLoop0.WithPitchOffset(-0.4f).WithVolumeScale(2.5f), NPC.Center), Type));

                                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                                    Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);

                                }
                                for (int i = -amt; i < amt + 1; i++)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, (rotation + PiOver2 + i * 0.2f).ToRotationVector2(), ProjectileType<CBeam>(), 50, 0, -1, i * 0.2f, 0.5f, NPC.whoAmI);
                                }
                            }
                            if (AITimer3 > 90 && AITimer3 < 235)
                            {
                                rotation += AITimer2;
                                NPC.rotation = rotation;
                                AITimer2 = Lerp(0, ToRadians(Clamp(20f / amt, 0, 5)), InOutCirc.Invoke((AITimer3 - 80) / 155));
                                deathCamModifier._pos = Vector2.Lerp(deathCamModifier._pos, NPC.Center + new Vector2(0, 200).RotatedBy(NPC.rotation) - new Vector2(Main.screenWidth, Main.screenHeight) / 2, 0.5f);
                                EbonianSystem.conglomerateSkyFlash = 5f + MathF.Sin(AITimer3 * 0.4f) * 0.5f;
                                if (CachedSlotIdsSystem.loopedSounds.Any())
                                    if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CBeam3"))
                                    {
                                        SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CBeam3"].slotId;
                                        bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                        if (playing)
                                        {
                                            activeSound.Pitch = Lerp(0.6f, 3f, InOutCirc.Invoke((AITimer3 - 80) / 155));

                                            if (AITimer3 > 195)
                                                activeSound.Volume = Lerp(1, 0, InOutCirc.Invoke((AITimer3 - 195) / 40));
                                        }
                                        if (AITimer3 > 193)
                                            CachedSlotIdsSystem.ClearSound("CBeam3");
                                    }
                            }
                            else isLasering = false;

                            if (AITimer > 360 + 240 && AITimer < 360 + 320)
                            {
                                rotation = 0;
                                if (AITimer == 360 + 241)
                                {
                                    SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                                    StartScreaming();
                                }
                                if (AITimer % 10 == 0)
                                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                                if (AITimer % 20 == 0)
                                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                                if (AITimer < 360 + 270)
                                {
                                    Vector2 pos = NPC.Center + new Vector2(Main.rand.NextFloat(-700, 700), 0);
                                    if (AITimer % 2 == 0)
                                    {
                                        Projectile.NewProjectile(null, pos, Vector2.UnitY, ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                                        Projectile.NewProjectile(null, pos, -Vector2.UnitY, ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                                    }
                                    else
                                    {
                                        pos = Main.rand.NextVector2Unit(); // man i sure do love variable names
                                        Projectile.NewProjectile(null, NPC.Center, pos, ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                                    }
                                }
                            }

                            if (AITimer > 360 + 320)
                            {
                                if (AITimer == 360 + 321)
                                {
                                    CameraSystem.ChangeCameraPos(NPC.Center, 200, new ZoomInfo(2, 2, InOutElastic, InOutSine), 2, InOutSine);
                                    isScreaming = false;
                                }
                                MoveVerlet(ref spineVerlet, NPC.Center + new Vector2(30, -50));
                                MoveVerlet(ref clingerVerlet, NPC.Center + new Vector2(40, -20));
                                MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(-30, 50));
                                MoveVerlet(ref armVerlet, NPC.Center + new Vector2(-50, -50));
                                for (int i = 0; i < 3; i++)
                                {
                                    Vector2 off = new Vector2(-50, -40);
                                    if (i == 1) off = new Vector2(-50, -20);
                                    if (i == 2) off = new Vector2(-50, 50);
                                    MoveVerlet(ref clawVerlet[i], NPC.Center + off);
                                }
                                rotation = 0;

                                if (AITimer == 360 + 351)
                                {
                                    SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                                    SoundEngine.PlaySound(s.WithPitchOffset(0.25f), NPC.Center);
                                    Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 500), Vector2.UnitY, ProjectileType<CBeam>(), 50, 0, -1, 0.5f);
                                }
                                if (AITimer == 360 + 354)
                                {
                                    NPC.dontTakeDamage = false;
                                    NPC.immortal = false;
                                    NPC.StrikeInstantKill();
                                    NPC.life = 0;
                                    NPC.checkDead();
                                }
                            }
                        }
                    }
                    break;
                case PhaseTransition:
                    {
                    }
                    break;
                case Intro:
                    {
                        if (AITimer2 == 0)
                        {
                            for (int i = 0; i < verletRotSeeds.Length; i++)
                                verletRotSeeds[i] = Main.rand.Next(ushort.MaxValue);
                            AITimer = -100;
                            AITimer2 = 1;
                        }
                        if (AITimer == -30)
                            CameraSystem.ChangeCameraPos(NPC.Center, 200, new ZoomInfo(1.8f, 1.3f, InOutElastic, InOutCubic), 2, InOutCirc);
                        if (AITimer < 180 && AITimer > 1)
                        {
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                            if (AITimer == 2)
                            {
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        }
                        else isScreaming = false;
                        if (AITimer == 1)
                        {
                            StartScreaming();
                            disposablePos[0] = NPC.Center;
                        }
                        else if (AITimer > 1 && AITimer < 100)
                            NPC.Center = disposablePos[0] + Main.rand.NextVector2Circular(AITimer * 0.1f, AITimer * 0.1f);
                        else if (AITimer == 100)
                        {
                            SpawnVerlets();
                            SoundEngine.PlaySound(EbonianSounds.fleshHit with { Pitch = -0.3f, PitchVariance = 0.2f }, player.Center);

                            for (int k = 0; k < 25; k++)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustID.Blood, Vector2.UnitX.RotatedByRandom(1) * Main.rand.NextFloat(7, 15), 0, default, Main.rand.NextFloat(1, 2));
                            }
                            for (int k = 0; k < 25; k++)
                            {
                                Dust.NewDustPerfect(NPC.Center, DustID.Blood, -Vector2.UnitX.RotatedBy(0.25f).RotatedByRandom(0.5f) * Main.rand.NextFloat(7, 15), 0, default, Main.rand.NextFloat(1, 2));
                            }
                        }
                        else
                        {
                            DefaultVerletAnimation();
                        }
                        if (AITimer >= 200)
                        {
                            Next = GeyserSweep;
                            Reset();

                            AIState = Death;
                        }
                    }
                    break;
                case Idle:
                    {
                        DefaultVerletAnimation();
                        if (AITimer < NPC.life / 1200 && NPC.Distance(player.Center) > 200)
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.15f);
                        else NPC.velocity *= 0.9f;

                        if (AITimer >= NPC.life / 1200 + 20)
                        {
                            AIState = Next;
                            NPC.velocity = Vector2.Zero;
                            AITimer = 0;
                            AITimer2 = 0;
                        }
                    }
                    break;
                case BloodAndWormSpit:
                    {
                        DefaultVerletAnimation();
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.2f);
                        Vector2 vel = NPC.rotation.ToRotationVector2().RotatedBy(PiOver2).RotatedByRandom(PiOver2 * 0.7f);
                        if (AITimer == 20)
                        {
                            AITimer2 = Main.rand.Next(2);
                        }
                        if (AITimer == 30)
                        {
                            if (AITimer2 == 0)
                            {
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                                NPC.velocity = -vel * 5;
                                Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + vel * 20, vel.RotatedByRandom(PiOver4) * Main.rand.NextFloat(8, 12), ProjectileType<CWorm>(), 30, 0, 0, Main.rand.NextFloat(0.15f, 0.5f));
                                a.friendly = false;
                                a.hostile = true;
                            }
                            else
                                Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 80 && AITimer % (phase2 ? 5 : 7) == 0 && AITimer < 216)
                        {
                            NPC.velocity = -vel * Main.rand.NextFloat(2f, 5);

                            SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                            Projectile a = Projectile.NewProjectileDirect(null, NPC.Center + vel * 20, vel * Main.rand.NextFloat(4, 6), ProjectileType<CWorm>(), 30, 0, 0, Main.rand.NextFloat(0.15f, 0.5f));
                            a.friendly = false;
                            a.hostile = true;
                            Projectile.NewProjectile(null, NPC.Center + vel * 20, vel * 6, ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 240)
                        {
                            Next = HomingEyeAndVilethornVomit;
                            Reset();
                        }
                    }
                    break;
                case HomingEyeAndVilethornVomit:
                    {
                        DefaultVerletAnimation();
                        if (AITimer < 200 && AITimer > 20)
                        {
                            if (AITimer == 21)
                            {
                                StartScreaming();
                                openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                            open = true;
                            if (openOffset.X < 25)
                            {
                                openOffset.X += 3.5f;
                            }
                            openRotation = ToRadians(MathF.Sin((AITimer + 50) * 4) * 10);

                            rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            NPC.rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                        }
                        else
                        {
                            isScreaming = false;
                            open = false;
                            rotation = Helper.LerpAngle(rotation, 0, 0.2f);
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.2f);
                        }
                        if (AITimer > 50 && AITimer % (phase2 ? 10 : 15) == 0 && AITimer < 200)
                        {
                            Vector2 pos = player.Center + new Vector2(700 * (Main.rand.NextBool() ? 1 : -1), 0).RotatedByRandom(PiOver4 * 0.5f);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ProjectileType<CecitiorEyeP>(), 25, 0);
                            pos = player.Center + new Vector2(0, 700 * (Main.rand.NextBool() ? 1 : -1)).RotatedByRandom(PiOver4);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center), ProjectileType<TerrorVilethorn1>(), 25, 0);
                        }
                        if (AITimer >= 200)
                        {
                            Next = BloatedEbonflies;
                            Reset();
                        }
                    }
                    break;
                case BloatedEbonflies: // come back to this later lol
                    {
                        DefaultVerletAnimation();
                        if (AITimer == 21)
                        {
                            StartScreaming();
                            openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                            SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        }
                        if (AITimer % 20 == 0)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        open = true;
                        if (openOffset.X < 25)
                        {
                            openOffset.X += 3.5f;
                        }
                        openRotation = ToRadians(MathF.Sin((AITimer + 50) * 4) * 10);

                        rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                        NPC.rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                        if (AITimer % 10 == 0)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                        if (AITimer == 30 || AITimer == 40 || AITimer == 50)
                        {
                            CameraSystem.ScreenShakeAmount = 10f;
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                            for (int i = -3; i < 4; i++)
                                NPC.NewNPCDirect(null, NPC.Center + Vector2.One.RotatedBy(i) * 30, NPCType<BloatedEbonfly>());
                        }

                        if (AITimer >= 60)
                        {
                            Next = CursedFlameRain;
                            Reset();
                        }
                    }
                    break;
                case CursedFlameRain:
                    {
                        DefaultVerletAnimation();
                        rotation = -Vector2.UnitY.ToRotation() - PiOver2;
                        if (AITimer <= 160 && AITimer > 30)
                        {
                            Vector2 pos = new Vector2(player.position.X, player.position.Y - 75);
                            Vector2 target = pos;
                            Vector2 moveTo = target - NPC.Center;
                            NPC.velocity = (moveTo) * 0.0445f;
                            if (++AITimer2 % 60 == 0)
                            {
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                                for (int i = 0; i <= 5; i++)
                                {
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-20f, 20f), -15), ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                                    projectile.tileCollide = false;
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.timeLeft = 230;
                                }
                                Projectile projectileb = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15), ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                                projectileb.tileCollide = false;
                                projectileb.hostile = true;
                                projectileb.friendly = false;
                                projectileb.timeLeft = 230;
                            }
                            else if (AITimer2 % 60 == 30)
                            {
                                for (int i = 0; i <= 5; i++)
                                {
                                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -5), ProjectileType<CIchor>(), 30, 1f, Main.myPlayer)];
                                    projectile.tileCollide = false;
                                    projectile.hostile = true;
                                    projectile.friendly = false;
                                    projectile.timeLeft = 230;
                                }
                                Projectile projectilec = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5), ProjectileType<CIchor>(), 30, 1f, Main.myPlayer)];
                                projectilec.tileCollide = false;
                                projectilec.hostile = true;
                                projectilec.friendly = false;
                                projectilec.timeLeft = 230;
                            }
                        }
                        else NPC.velocity *= 0.9f;

                        if (AITimer >= 180)
                        {
                            Next = Chomp;
                            Reset();
                        }
                    }
                    break;
                case Chomp:
                    {
                        /*if (AITimer >= 50 || AITimer2 > 0)
                            EbonianMod.DarkAlpha = 0.5f;
                        else if (AITimer < 50)
                            EbonianMod.DarkAlpha = Lerp(0, 0.5f, InOutCirc.Invoke(AITimer / 50));*/
                        DefaultVerletAnimation();
                        //do thing
                        if (AITimer == 1)
                            openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                        if (open)
                        {
                            if (disposablePos[5].X == 0)
                                openOffset += Vector2.UnitY * 5;
                            else
                                openOffset += Vector2.UnitX * 6;
                        }
                        if (AITimer < 25)
                        {
                            NPC.dontTakeDamage = true;
                            open = true;
                            if (disposablePos[5].X == 0)
                            {
                                openRotation = Lerp(openRotation, ToRadians(90), 0.5f);
                                rotation = Lerp(rotation, ToRadians(90), 0.5f);
                            }
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center, false) / 10f;
                        }
                        if (AITimer >= 25 && AITimer < (50 + (phase2 ? 7 : 0)))
                        {
                            shakeVal = Lerp(shakeVal, (phase2 ? 30 : 15), 0.1f);
                            if (AITimer < 53)
                                disposablePos[0] = player.Center + player.velocity * 5;
                            if (disposablePos[5].X == 0)
                                NPC.velocity = Helper.FromAToB(NPC.Center, disposablePos[0], false) / 5f;
                            else
                                NPC.velocity = Helper.FromAToB(NPC.Center, disposablePos[0], false) / 5f;
                        }
                        if (AITimer == 50 + (phase2 ? 7 : 0))
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);


                            for (int i = -1; i < 2; i++)
                            {
                                float off = Main.rand.NextFloat(-PiOver4 * 0.3f, PiOver4 * 0.3f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - openOffset.RotatedBy(PiOver2) * 3 + openOffset * 0.25f * i, Helper.FromAToB(NPC.Center - openOffset.RotatedBy(PiOver2) + openOffset * 0.25f * i, player.Center + player.velocity).RotatedBy(off) * 20, ProjectileType<CGhostCeci>(), 20, 0);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + openOffset.RotatedBy(PiOver2) * 3 + openOffset * 0.25f * i, Helper.FromAToB(NPC.Center + openOffset.RotatedBy(PiOver2) + openOffset * 0.25f * i, player.Center + player.velocity).RotatedBy(off) * 20, ProjectileType<CGhostCeci>(), 20, 0);
                            }

                            //SoundEngine.PlaySound(SoundID.ForceRoar, NPC.Center + openOffset);
                            NPC.velocity = Vector2.Zero;
                        }
                        if (AITimer == 65)
                        {
                            NPC.dontTakeDamage = false;
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;

                                for (int j = -2; j < 3; j++)
                                {
                                    Vector2 pos = NPC.Center + openOffset * i + new Vector2(0, j * 10).RotatedBy(Helper.FromAToB(NPC.Center + openOffset * i, NPC.Center).ToRotation());
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Helper.FromAToB(pos, NPC.Center + new Vector2(0, j * 200).RotatedBy(Helper.FromAToB(NPC.Center + openOffset * i, NPC.Center).ToRotation())) * 10, ProjectileType<TFlameThrower4>(), 0, 0);
                                }
                            }

                            shakeVal = 0;
                            open = false;
                        }
                        if (AITimer > 65)
                        {
                            openOffset.Y = Lerp(openOffset.Y, 0, 0.3f);
                            //NPC.Center = Vector2.Lerp(NPC.Center, NPC.Center + openOffset, 0.6f);
                        }
                        if (disposablePos[5].X == 0)
                        {
                            if (openOffset.Y < 50 && AITimer > 25)
                            {
                                if (openOffset != Vector2.Zero)
                                {
                                    SoundEngine.PlaySound(EbonianSounds.cecitiorClose, NPC.Center);
                                    CameraSystem.ScreenShakeAmount = 5;
                                }
                                openRotation = 0;
                                //rotation = 0;
                                open = false;
                                NPC.frame.Y = 0;
                                openOffset = Vector2.Zero;
                            }
                        }
                        if (AITimer >= 75)
                        {
                            openRotation = 0;
                            rotation = 0;
                            if (phase2)
                                NPC.dontTakeDamage = false;
                            int num = 3;
                            disposablePos[5] = new Vector2(Main.rand.Next(2), 0);
                            if (AITimer2 < num)
                            {
                                AITimer2++;
                                AITimer = 0;
                                NPC.velocity = Vector2.Zero;
                            }
                            else
                            {
                                Next = ExplodingBalls;
                                Reset();
                            }
                        }
                    }
                    break;
                case ExplodingBalls:
                    {
                        DefaultVerletAnimation();
                        rotation = -Vector2.UnitY.ToRotation() - PiOver2;
                        Vector2 pos = player.Center + new Vector2(player.velocity.X, -75);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.0445f;
                        if (AITimer > 60 && AITimer % (phase2 ? 2 : 3) == 0 && AITimer < 90)
                        {
                            Projectile.NewProjectile(null, NPC.Center + -Vector2.UnitY.RotatedByRandom(PiOver4) * 30, -Vector2.UnitY.RotatedByRandom(PiOver4 * 0.3f) * Main.rand.NextFloat(4, 10), (Main.rand.NextBool() ? ProjectileType<TerrorStaffPEvil>() : ProjectileType<CecitiorBombThing>()), 20, 0);
                        }
                        if (AITimer >= 90)
                        {
                            Next = BayleLaser;
                            Reset();
                        }
                    }
                    break;
                case BayleLaser:
                    {
                        DefaultVerletAnimation();
                        if (AITimer > 30)
                        {
                            if (!(AITimer < 165 && AITimer > 140) && AITimer < 320)
                            {
                                if (AITimer > 165)
                                    AITimer2 = Lerp(AITimer2, ToRadians(3), 0.1f);
                                disposablePos[0] = Vector2.Lerp(disposablePos[0], player.Center, 0.05f);
                            }
                            if (AITimer < 320)
                            {
                                if (AITimer < 140)
                                    rotation = Helper.FromAToB(NPC.Center, disposablePos[0]).ToRotation() - PiOver2;
                                else
                                    rotation += AITimer2;
                            }
                            else
                                rotation = Vector2.UnitY.ToRotation() - PiOver2 + ToRadians(20);
                        }
                        if (AITimer == 30)
                        {
                            disposablePos[0] = player.Center;
                        }
                        if (AITimer == 45)
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                        if (AITimer == 100)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0);
                        if (AITimer == 110)
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);

                        if (AITimer == 140)
                        {
                            SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                            Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + PiOver2).ToRotationVector2(), ProjectileType<VileTearTelegraph>(), 0, 0);
                        }
                        if (AITimer == 165)
                        {
                            StartLasering();
                            CameraSystem.ScreenShakeAmount = 10f;
                            SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                            SoundEngine.PlaySound(s.WithPitchOffset(0.2f), NPC.Center);

                            CachedSlotIdsSystem.loopedSounds.Add("CBeam1", new(SoundEngine.PlaySound(EbonianSounds.deathrayLoop0.WithPitchOffset(0.2f).WithVolumeScale(2), NPC.Center), Type));

                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, disposablePos[0]), ProjectileType<CBeam>(), 50, 0, -1, 0, -.5f, NPC.whoAmI);
                        }
                        if (AITimer > 165 && AITimer < 320)
                        {
                            EbonianSystem.conglomerateSkyFlash = 3f + MathF.Sin(AITimer * 0.4f) * 0.5f;
                            if (CachedSlotIdsSystem.loopedSounds.Any())
                                if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CBeam1"))
                                {
                                    SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CBeam1"].slotId;
                                    bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                    if (playing)
                                    {
                                        activeSound.Pitch = Lerp(1.2f, 2, InOutCirc.Invoke((AITimer - 165) / 175));

                                        if (AITimer > 280)
                                            activeSound.Volume = Lerp(1, 0, InOutCirc.Invoke((AITimer - 280) / 40));
                                    }
                                    if (AITimer > 318)
                                        CachedSlotIdsSystem.ClearSound("CBeam1");
                                }
                        }
                        if (AITimer > 160 && AITimer < 220)
                            NPC.velocity = Helper.FromAToB(disposablePos[0], NPC.Center) * 0.5f;
                        if ((AITimer > 220 && AITimer < 240) || (AITimer > 340 && AITimer < 360))
                            NPC.velocity *= 0.9f;
                        if (AITimer == 320)
                        {
                            isLasering = false;
                            NPC.velocity = -Vector2.UnitY.RotatedBy(-PiOver4) * 8f;
                        }
                        if (AITimer > 320 && AITimer < 390)
                        {
                            for (int i = 0; i < 1 + (AITimer * 0.025f); i++)
                            {
                                Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, NPC.Center + (NPC.rotation + PiOver2).ToRotationVector2() * 10).RotatedByRandom(PiOver2) * Main.rand.NextFloat(100, 600);
                                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center).RotatedByRandom(PiOver2) * Main.rand.NextFloat(15, 35), 0, Color.Lerp(Color.LimeGreen, Color.Maroon, Main.rand.NextFloat()), Main.rand.NextFloat(0.06f, .15f)).customData = NPC.Center;
                            }
                        }
                        if (AITimer == 360)
                        {
                            SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                            Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + PiOver2).ToRotationVector2(), ProjectileType<VileTearTelegraph>(), 0, 0);
                        }
                        if (AITimer == 385)
                        {
                            StartLasering();
                            CameraSystem.ScreenShakeAmount = 20f;
                            SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                            laserSound = SoundEngine.PlaySound(s.WithPitchOffset(0.25f), NPC.Center);

                            CachedSlotIdsSystem.loopedSounds.Add("CBeam2", new(SoundEngine.PlaySound(EbonianSounds.deathrayLoop0.WithPitchOffset(-0.4f).WithVolumeScale(2.5f), NPC.Center), Type));

                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, (rotation + PiOver2).ToRotationVector2(), ProjectileType<CBeam>(), 50, 0, -1, 0, 0.3f, NPC.whoAmI);
                        }
                        if (AITimer > 385 && AITimer < 540)
                        {
                            EbonianSystem.conglomerateSkyFlash = 5f + MathF.Sin(AITimer * 0.4f) * 0.5f;
                            if (CachedSlotIdsSystem.loopedSounds.Any())
                                if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CBeam2"))
                                {
                                    SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CBeam2"].slotId;
                                    bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                    if (playing)
                                    {
                                        activeSound.Pitch = Lerp(0.6f, 3f, InOutCirc.Invoke((AITimer - 385) / 155));

                                        if (AITimer > 500)
                                            activeSound.Volume = Lerp(1, 0, InOutCirc.Invoke((AITimer - 500) / 40));
                                    }
                                    if (AITimer > 538)
                                        CachedSlotIdsSystem.ClearSound("CBeam2");
                                }
                        }
                        if (SoundEngine.TryGetActiveSound(laserSound, out var _laserSound))
                            _laserSound.Position = NPC.Center;
                        if (AITimer > 385 && AITimer < 470)
                        {
                            isLasering = false;
                            disposablePos[0] = new Vector2((player.Center.X < NPC.Center.X ? 1 : -1), 0);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(30, 0), 0.015f);
                        }
                        if (AITimer > 470 && AITimer < 520)
                        {
                            NPC.rotation += ToRadians(SmoothStep(0.25f, 11f, (AITimer - 470) / 80) * disposablePos[0].X);
                            NPC.velocity *= 0.9f;
                        }

                        if (AITimer >= 520)
                        {
                            Next = SpinAroundThePlayerAndDash;
                            Reset();
                        }
                    }
                    break;
                case SpinAroundThePlayerAndDash:
                    {
                        /*if (AITimer < 100 && AITimer > 20)
                            EbonianMod.DarkAlpha = Lerp(0, 0.5f, InOutCirc.Invoke((AITimer - 20) / 80));
                        else if (AITimer >= 100)
                            EbonianMod.DarkAlpha = 0.5f;*/
                        DefaultVerletAnimation();
                        if (NPC.velocity.Length() > .5f)
                            rotation = NPC.velocity.ToRotation() - PiOver2;
                        else
                            rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;

                        if (AITimer < 233)
                        {
                            AITimer2++;

                            if (AITimer2 == 1)
                                disposablePos[0] = Main.rand.NextVector2Unit();
                            //else if (AITimer2 < 60)
                            else
                            {
                                if (AITimer2 % 9 == 0 && AITimer > 40)
                                    Projectile.NewProjectile(null, NPC.Center + Helper.FromAToB(NPC.Center, player.Center, false) * 3.4f + Main.rand.NextVector2CircularEdge(5, 5), Helper.FromAToB(NPC.Center, player.Center, reverse: true) * Main.rand.NextFloat(5, 15), ProjectileType<CGhostCeci>(), 25, 0);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(800, 0).RotatedBy(disposablePos[0].ToRotation() + ToRadians(AITimer2 * 6)), false) * 0.04f, 0.5f);
                            }
                        }
                        else
                            NPC.velocity *= 0.8f;
                        if (AITimer >= 270)
                        {
                            Next = SpikesCloseIn;
                            Reset();
                        }
                    }
                    break;
                case SpikesCloseIn:
                    {
                        DefaultVerletAnimation();
                        Vector2 pos = new Vector2(player.position.X, player.position.Y - 175);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.0445f;

                        if (AITimer < 180 && AITimer > 1)
                        {
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                            if (AITimer == 2)
                            {
                                StartScreaming();
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        }
                        else isScreaming = false;

                        if (AITimer > 70 && AITimer < 85 && AITimer % 3 == 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Vector2 _pos = player.Center + Main.rand.NextVector2CircularEdge(1200, 1200) + Main.rand.NextVector2Circular(100, 100);
                                Vector2 vel = Helper.FromAToB(_pos, player.Center).RotatedByRandom(PiOver4 / 2f);
                                Projectile.NewProjectile(null, _pos, vel, ProjectileType<CBeamSmall>(), 0, 0, ai1: -0.7f);
                            }
                        }
                        if (AITimer >= 140)
                        {
                            Next = BothHalvesRainCloseIn;
                            Reset();
                        }
                    }
                    break;
                case BothHalvesRainCloseIn:
                    {
                        DefaultVerletAnimation();
                        Vector2 pos = new Vector2(player.position.X + 100, player.position.Y - 175);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.0745f;
                        if (AITimer == 1)
                            openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                        if (AITimer < 50)
                        {
                            NPC.dontTakeDamage = true;
                            open = true;
                            rotation = -PiOver2;
                            openRotation = Helper.LerpAngle(openRotation, PiOver2, 0.05f);
                            openOffset = Vector2.Lerp(openOffset, new Vector2(600, 0), 0.1f);
                        }
                        if (AITimer > 50 && AITimer < 150)
                        {
                            openOffset = Vector2.Lerp(new Vector2(600, 0), new Vector2(50, 0), InOutCirc.Invoke((AITimer - 50) / 100));

                            if (AITimer > 60)
                            {
                                if (AITimer % 10 == 0)
                                {
                                    Projectile.NewProjectile(null, NPC.Center + openOffset, -Vector2.UnitY.RotatedByRandom(PiOver2) * 15, ProjectileType<CFlesh>(), 30, 0);
                                }
                                if (AITimer % 10 == 5)
                                {
                                    Projectile.NewProjectile(null, NPC.Center - openOffset, -Vector2.UnitY.RotatedByRandom(PiOver2) * 15, ProjectileType<CFlesh>(), 30, 0);
                                }
                                if (AITimer % 15 == 0)
                                {
                                    Projectile.NewProjectile(null, NPC.Center + openOffset, -Vector2.UnitY.RotatedBy(-PiOver2 * 0.7f).RotatedByRandom(PiOver2) * 5, ProjectileType<TerrorVilethorn1>(), 30, 0);
                                    Projectile.NewProjectile(null, NPC.Center - openOffset + Main.rand.NextVector2CircularEdge(5, 5), -Vector2.UnitY.RotatedBy(PiOver2 * 0.7f).RotatedByRandom(PiOver2) * 5, ProjectileType<CecitiorEyeP>(), 30, 0);
                                }
                            }
                        }
                        if (AITimer >= 150)
                        {
                            NPC.dontTakeDamage = false;
                            rotation = 0;
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.25f);
                        }
                        if (AITimer == 170)
                            open = false;
                        if (AITimer >= 200)
                        {
                            Next = DashSpam;
                            Reset();
                        }
                    }
                    break;
                case DashSpam:
                    {
                        DefaultVerletAnimation();
                        /*if (AITimer < 100 && AITimer > 20)
                            EbonianMod.DarkAlpha = Lerp(0, 0.5f, InOutCirc.Invoke((AITimer - 20) / 80));
                        else if (AITimer >= 100)
                            EbonianMod.DarkAlpha = 0.5f;*/
                        if (AITimer < 250 && AITimer > 20)
                        {
                            NPC.damage = 150;
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                            if (AITimer == 21)
                            {
                                StartScreaming();
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        }
                        else isScreaming = false;

                        if (AITimer < 300 && AITimer >= 100)
                        {
                            AITimer2++;
                            if (AITimer2 == 14)
                            {
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Helper.FromAToB(NPC.Center, disposablePos[0]), 30, 10, 30, 2000));
                                SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                                for (int i = 0; i < 40; i++)
                                {
                                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IchorTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                                }
                            }
                            if (AITimer2 < 10)
                            {
                                if (AITimer2 < 4)
                                    disposablePos[0] = player.Center;
                                /*if (AITimer2 == 4)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, disposablePos[0]) * 10, ProjectileType<CGhostCeci>(), 30, 0);
                                }*/
                                rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;
                                NPC.velocity -= Helper.FromAToB(NPC.Center, player.Center) * (AITimer2 % 50) * 0.6f;
                            }
                            if (AITimer2 < 30 && AITimer2 > 10)
                            {
                                if (AITimer2 > 20)
                                    rotation = NPC.velocity.ToRotation() - PiOver2;
                                if (AITimer2 == 14)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<BlurScream>(), 0, 0);
                                    for (int i = 0; i < 50; i++)
                                    {
                                        Dust.NewDustPerfect(NPC.Center, DustType<BlurDust>(), Helper.CircleDividedEqually(i, 50).ToRotationVector2() * 20);
                                        Dust.NewDustPerfect(NPC.Center, DustType<BlurDust>(), Helper.CircleDividedEqually(i, 50).ToRotationVector2() * 25);
                                    }
                                    NPC.velocity = Helper.FromAToB(NPC.Center, disposablePos[0]) * 40;
                                }
                                if (AITimer2 > 12 && AITimer2 % 3 == 0)
                                {
                                    int type = (AITimer2 % 6 == 0 ? ProjectileType<CIchor>() : ProjectileType<TFlameThrower>());
                                    Projectile.NewProjectile(null, NPC.Center, NPC.velocity.RotatedBy(PiOver2 * (AITimer2 % 2 == 0 ? 1 : -1)).RotatedByRandom(PiOver4) * 0.1f, type, 30, 0);
                                }
                            }
                            if (AITimer2 > 35)
                                NPC.velocity *= 0.8f;

                            if (AITimer2 > 50) AITimer2 = 0;
                        }

                        if (AITimer > 300)
                            NPC.velocity *= 0.9f;
                        if (AITimer >= 330)
                        {
                            Next = OstertagiExplosion;
                            Reset();
                        }
                    }
                    break;
                case OstertagiExplosion:
                    {
                        DefaultVerletAnimation();
                        if (AITimer == 1)
                        {
                            SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                        }
                        if (AITimer == 55)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                        if (AITimer == 100)
                        {
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 10; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 10);
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * 6, ProjectileType<CWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                            CameraSystem.ScreenShakeAmount = 10;
                        }
                        if (AITimer == 120)
                        {
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 10; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 10) + PiOver4;
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * 8, ProjectileType<CWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                            CameraSystem.ScreenShakeAmount = 10;
                        }
                        if (AITimer == 140)
                        {
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 10; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 10) + PiOver2;
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * 10, ProjectileType<CWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                            CameraSystem.ScreenShakeAmount = 10;
                        }

                        if (AITimer >= 140)
                        {
                            Next = IchorBomb;
                            Reset();
                        }

                    }
                    break;
                case IchorBomb:
                    {
                        DefaultVerletAnimation();
                        if (AITimer2 == 0)
                        {
                            if (AITimer == 1)
                                openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                            if (AITimer < 35)
                            {
                                open = true;
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200), false) / 20f;
                                openOffset.X += 3;
                                if (AITimer > 5)
                                {
                                    openRotation += ToRadians(Lerp(0, 4, InOutExpo.Invoke(AITimer / 35)));
                                    rotation -= ToRadians(Lerp(0, 4, InOutExpo.Invoke(AITimer / 35)));
                                }
                                NPC.rotation = rotation;
                            }
                            if (AITimer == 35)
                            {
                                NPC.velocity = Vector2.Zero;
                            }
                            if (AITimer >= 50 && AITimer <= 70 && AITimer % 5 == 0)
                            {
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                                for (int i = -1; i < 2; i++)
                                {
                                    if (i == 0) continue;
                                    Projectile.NewProjectile(null, NPC.Center + openOffset * i, new Vector2(-i * (10 + (AITimer - 50) * 0.5f), -7), ProjectileType<CIchorBomb>(), 40, 0);
                                }
                            }
                            if (AITimer >= 70)
                            {
                                openRotation = Helper.LerpAngle(openRotation, 0, 0.35f);
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.35f);
                                rotation = Helper.LerpAngle(rotation, 0, 0.35f);
                            }
                            if (AITimer >= 85)
                            {
                                open = false;
                            }
                        }
                        else
                        {

                            if (AITimer == 1)
                                openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                            if (AITimer < 25)
                            {
                                open = true;
                                openOffset += Vector2.UnitX * 8;
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 360), false) / 10f;
                            }
                            else NPC.velocity = Vector2.Zero;
                            if (AITimer == 40)
                                Projectile.NewProjectile(null, NPC.Center - openOffset + Main.rand.NextVector2Circular(20, 20), Vector2.UnitY.RotatedByRandom(PiOver2 * 0.7f) * 10, ProjectileType<CIchor>(), 30, 0);

                            if (AITimer > 50 && AITimer < 100 && AITimer % 4 == 0)
                            {
                                if (AITimer % 8 == 0)
                                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                                Projectile.NewProjectile(null, NPC.Center + openOffset + Main.rand.NextVector2Circular(20, 20), Vector2.UnitY.RotatedByRandom(PiOver2 * 0.7f) * 10, ProjectileType<TFlameThrower>(), 30, 0);
                                Projectile.NewProjectile(null, NPC.Center - openOffset + Main.rand.NextVector2Circular(20, 20), Vector2.UnitY.RotatedByRandom(PiOver2 * 0.7f) * 10, ProjectileType<CIchor>(), 30, 0);
                            }
                            if (AITimer < 100)
                            {
                                openRotation = Helper.LerpAngle(openRotation, -PiOver4, 0.25f);
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, PiOver4, 0.25f);
                                rotation = Helper.LerpAngle(rotation, PiOver4, 0.25f);
                            }
                            if (AITimer >= 120)
                            {
                                open = false;
                                openRotation = Helper.LerpAngle(openRotation, 0, 0.25f);
                                rotation = NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.25f);
                            }
                        }

                        if (AITimer >= 110 + AITimer2 * 80)
                        {
                            Next = InstantRays;
                            if (AITimer2 == 0)
                            {
                                AITimer = 0;
                                AITimer2 = 1;
                            }
                            else
                                Reset();
                        }
                    }
                    break;
                case InstantRays:
                    {
                        DefaultVerletAnimation();

                        if (AITimer == 1)
                        {
                            StartScreaming();
                            openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                            SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        }
                        if (AITimer % 20 == 0)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        if (AITimer % 10 == 0)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                        int _dir = AITimer % 2 == 0 ? 1 : -1;
                        Vector2 _pos = player.Center - new Vector2(750 - Main.rand.NextFloat(750 * 2), (Main.screenHeight / 2 + 60) * _dir);
                        Dust.NewDustPerfect(_pos, DustType<LineDustFollowPoint>(), Vector2.UnitY.RotatedByRandom(PiOver4) * _dir * Main.rand.NextFloat(10, 20), 0, Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.05f, 0.15f));

                        if (AITimer > 40 && AITimer % 15 == 0)
                        {
                            int dir = AITimer % 30 == 0 ? 1 : -1;
                            Vector2 pos = player.Center - new Vector2(750 - (AITimer - 20) * 7.5f, 700 * dir);
                            Vector2 vel = Vector2.UnitY.RotatedByRandom(PiOver4 * 0.5f) * dir;
                            Projectile.NewProjectile(null, pos, vel, ProjectileType<CBeamInstant>(), 0, 0);

                        }
                        if (AITimer >= 200)
                        {
                            Next = ClawAtFlesh;
                            Reset();
                        }
                    }
                    break;
                case ClawAtFlesh:
                    {
                        DefaultSpineBehaviour();
                        DefaultClingerBehaviour();
                        DefaultEyeBehaviour();

                        AITimer2++;
                        if (clawVerlet[0].verlet != null && armVerlet.verlet != null)
                        {
                            if (AITimer2 == 1)
                                disposablePos[5] = new Vector2(Main.rand.NextFloat(Pi * 2), 0);
                            if (AITimer2 < 20)
                            {
                                NPC.velocity *= 0.9f;
                                MoveVerlet(ref armVerlet, NPC.Center + new Vector2(0, 250).RotatedBy(disposablePos[5].X), 0.2f);
                                clawVerlet[0].position = Vector2.Lerp(clawVerlet[0].position, NPC.Center + new Vector2(45, -185).RotatedBy(disposablePos[5].X), 0.3f);
                                clawVerlet[1].position = Vector2.Lerp(clawVerlet[1].position, NPC.Center + new Vector2(0, -200).RotatedBy(disposablePos[5].X), 0.3f);
                                clawVerlet[2].position = Vector2.Lerp(clawVerlet[2].position, NPC.Center + new Vector2(-45, -185).RotatedBy(disposablePos[5].X), 0.3f);
                            }
                            if (AITimer2 == 20)
                            {
                                for (int i = 0; i < clawVerlet.Length; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), clawVerlet[i].position + Vector2.UnitY.RotatedBy(disposablePos[5].X) * 110, -Vector2.UnitY.RotatedBy(disposablePos[5].X), ProjectileType<CecitiorClawSlash>(), 30, 0, ai2: 1);
                                }
                            }
                            if (AITimer2 >= 20)
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 20, 0.05f);
                                if (AITimer2 == 23)
                                {
                                    NPC.SimpleStrikeNPC(50, 0);
                                    Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextVector2Circular(17, 17), ProjectileType<CFlesh>(), 30, 0);
                                    Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextVector2Circular(7, 7), ProjectileType<CIchor>(), 30, 0);
                                    Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextVector2Circular(7, 7), ProjectileType<CecitiorEyeP>(), 30, 0);
                                    Projectile.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(40, 40), Main.rand.NextVector2Circular(17, 17), ProjectileType<TFlameThrower2>(), 30, 0);
                                }
                                for (int i = 0; i < clawVerlet.Length; i++)
                                {
                                    clawVerlet[0].position = Vector2.Lerp(clawVerlet[0].position, NPC.Center - new Vector2(45, -170).RotatedBy(disposablePos[5].X), 0.05f);
                                    clawVerlet[1].position = Vector2.Lerp(clawVerlet[1].position, NPC.Center - new Vector2(0, -200).RotatedBy(disposablePos[5].X), 0.05f);
                                    clawVerlet[2].position = Vector2.Lerp(clawVerlet[2].position, NPC.Center - new Vector2(-45, -170).RotatedBy(disposablePos[5].X), 0.05f);
                                }
                            }
                        }
                        if (AITimer2 >= 26 && AITimer < 150)
                            AITimer2 = 0;
                        if (AITimer >= 170)
                        {
                            Next = GrabAttack;
                            Reset();
                        }
                    }
                    break;
                case GrabAttack:
                    {
                        if (AITimer2 < 1)
                            DefaultArmBehaviour();
                        DefaultClingerBehaviour();
                        DefaultEyeBehaviour();

                        if (AITimer > 50)
                        {
                            if (AITimer < 80)
                            {
                                if (AITimer2 == 0)
                                {
                                    if (AITimer == 41)
                                        SoundEngine.PlaySound(EbonianSounds.cecitiorSlice, NPC.Center);
                                    DefaultClawBehaviour();
                                    MoveVerlet(ref spineVerlet, disposablePos[0] + Helper.FromAToB(disposablePos[1], disposablePos[0]) * (100), 0.25f);
                                }
                            }
                        }
                        else
                        {
                            if (AITimer2 == 0)
                            {
                                if (AITimer == 50 || AITimer == 10)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spineVerlet.position, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                                disposablePos[1] = spineVerlet.position;
                                disposablePos[0] = spineVerlet.position + Vector2.Clamp(Helper.FromAToB(spineVerlet.position, player.Center + player.velocity, false), -Vector2.One * 520, Vector2.One * 420);
                            }
                        }
                        if (spineVerlet.position.Distance(player.Center) < 37 && AITimer2 == 0 && AITimer > 53)
                        {
                            AITimer2 = 1;
                            SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            AITimer = 40;
                        }
                        if (AITimer2 > 0)
                        {
                            if (AITimer < 100)
                                MoveVerlet(ref spineVerlet, disposablePos[0] + Helper.FromAToB(disposablePos[1], disposablePos[0]) * (200) * SmoothStep(1, 0, (AITimer - 40) / 60), 0.25f);
                            MoveVerlet(ref armVerlet, player.Center, 0.15f);
                            rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;
                            if (AITimer == 45)
                                SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                            if (AITimer == 100)
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0);
                            if (AITimer == 110)
                            {
                                CameraSystem.ChangeZoom(250, new(1.7f, 1.2f, InOutCirc, InOutSine));
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                            }
                            if (AITimer == 165)
                            {
                                StartLasering();
                                CameraSystem.ScreenShakeAmount = 10f;
                                SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                                SoundEngine.PlaySound(s.WithPitchOffset(0.2f), NPC.Center);

                                CachedSlotIdsSystem.loopedSounds.Add("CBeam1", new(SoundEngine.PlaySound(EbonianSounds.deathrayLoop0.WithPitchOffset(0.2f).WithVolumeScale(2), NPC.Center), Type));

                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, disposablePos[0]), ProjectileType<CBeam>(), 0, 0, -1, 0, 1, NPC.whoAmI);
                            }
                            if (AITimer > 165 && AITimer < 320)
                            {
                                EbonianSystem.conglomerateSkyFlash = 5f + MathF.Sin(AITimer * 0.4f) * 0.5f;
                                if (CachedSlotIdsSystem.loopedSounds.Any())
                                    if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CBeam1"))
                                    {
                                        SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CBeam1"].slotId;
                                        bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                        if (playing)
                                        {
                                            activeSound.Pitch = Lerp(1.2f, 2, InOutCirc.Invoke((AITimer - 165) / 175));

                                            if (AITimer > 280)
                                                activeSound.Volume = Lerp(1, 0, InOutCirc.Invoke((AITimer - 280) / 40));
                                        }
                                        if (AITimer > 318)
                                            CachedSlotIdsSystem.ClearSound("CBeam1");
                                    }
                            }
                            else isLasering = false;

                            AITimer2++;
                            if (clawVerlet[0].verlet != null && armVerlet.verlet != null)
                            {
                                if (AITimer % 10 == 0)
                                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                                if (AITimer % 20 == 0)
                                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                                player.Center = spineVerlet.position;

                                if (AITimer % 20 == 10 && AITimer < 280)
                                    Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(eyeVerlet.position, player.Center) * Main.rand.NextFloat(8, 16), ProjectileType<CIchor>(), 10, 0);
                                if (AITimer % 20 == 0 && AITimer < 280)
                                    Projectile.NewProjectile(null, clingerVerlet.position, Helper.FromAToB(clingerVerlet.position, player.Center).RotatedByRandom(PiOver4 * 0.3f) * 5, ProjectileType<TFlameThrower4>(), 20, 0);

                                if (AITimer2 < 20)
                                {
                                    NPC.velocity *= 0.9f;
                                    clawVerlet[0].position = Vector2.Lerp(clawVerlet[0].position, player.Center + new Vector2(45, -185).RotatedBy(disposablePos[5].X), 0.3f);
                                    clawVerlet[1].position = Vector2.Lerp(clawVerlet[1].position, player.Center + new Vector2(0, -200).RotatedBy(disposablePos[5].X), 0.3f);
                                    clawVerlet[2].position = Vector2.Lerp(clawVerlet[2].position, player.Center + new Vector2(-45, -185).RotatedBy(disposablePos[5].X), 0.3f);
                                }
                                if (AITimer2 == 20)
                                {
                                    StartScreaming();
                                    for (int i = 0; i < clawVerlet.Length; i++)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), clawVerlet[i].position + Vector2.UnitY.RotatedBy(disposablePos[5].X) * 110, -Vector2.UnitY.RotatedBy(disposablePos[5].X), ProjectileType<CecitiorClawSlash>(), 15, 0, ai2: 1);
                                    }
                                }
                                if (AITimer2 >= 20)
                                {
                                    for (int i = 0; i < clawVerlet.Length; i++)
                                    {
                                        clawVerlet[0].position = Vector2.Lerp(clawVerlet[0].position, player.Center - new Vector2(45, -170).RotatedBy(disposablePos[5].X), 0.05f);
                                        clawVerlet[1].position = Vector2.Lerp(clawVerlet[1].position, player.Center - new Vector2(0, -200).RotatedBy(disposablePos[5].X), 0.05f);
                                        clawVerlet[2].position = Vector2.Lerp(clawVerlet[2].position, player.Center - new Vector2(-45, -170).RotatedBy(disposablePos[5].X), 0.05f);
                                    }
                                }
                            }
                            if (AITimer2 >= 26 && AITimer < 280)
                            {
                                disposablePos[5] = new Vector2(Main.rand.NextFloat(Pi * 2), 0);
                                AITimer2 = 1;
                            }
                        }
                        else if (AITimer > 80)
                        {
                            AITimer += 30;
                        }
                        if (AITimer >= 320)
                        {
                            Next = ClingerHipfire;
                            Reset();
                        }
                    }
                    break;
                case ClingerHipfire:
                    {
                        DefaultArmBehaviour();
                        DefaultSpineBehaviour();
                        DefaultEyeBehaviour();
                        DefaultClawBehaviour();

                        if (AITimer > 50)
                        {
                            NPC.velocity *= 0.97f;
                            MoveVerlet(ref clingerVerlet, NPC.Center + new Vector2(0, 300).RotatedBy(MathF.Sin(AITimer * 0.1f) * 1.5f + NPC.rotation));
                            //MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(0, 300).RotatedBy(MathF.Sin(-AITimer * 0.1f) * 1.5f + NPC.rotation));
                            if (AITimer % 5 == 0)
                            {
                                if (AITimer % 15 == 0)
                                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                                if (AITimer < 110)
                                    Projectile.NewProjectile(null, clingerVerlet.position, Helper.FromAToB(clingerVerlet.position, player.Center), ProjectileType<TFlameThrowerHoming>(), 30, 0);
                            }
                        }
                        else
                        {
                            DefaultClingerBehaviour();
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 300) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.15f);

                        }
                        if (AITimer >= 150)
                        {
                            Next = EyeBeamPlusHomingEyes;
                            Reset();
                        }
                    }
                    break;
                case EyeBeamPlusHomingEyes:
                    {
                        DefaultClingerBehaviour();
                        DefaultArmBehaviour();
                        DefaultSpineBehaviour();
                        DefaultEyeBehaviour();
                        DefaultClawBehaviour();


                        MoveVerlet(ref eyeVerlet, NPC.Center + Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -200).RotatedBy(MathF.Sin(AITimer * 0.05f)), false), -Vector2.One * 500, Vector2.One * 500));

                        if (AITimer > 50)
                        {
                            if (AITimer % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                                Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(eyeVerlet.position, player.Center).RotatedByRandom(PiOver4), ProjectileType<CecitiorEyeP>(), 30, 0);
                                Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(eyeVerlet.position, player.Center).RotatedByRandom(PiOver4), ProjectileType<EyeVFX>(), 0, 0);
                            }
                            if (AITimer % 20 == 10 && AITimer > 80)
                            {
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, NPC.Center);
                                Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(eyeVerlet.position, player.Center).RotatedByRandom(PiOver4) * 4, ProjectileType<CIchor>(), 30, 0);
                            }
                        }
                        if (AITimer >= 150)
                        {
                            Next = ClawTantrum;
                            Reset();
                        }
                    }
                    break;
                case ClawTantrum:
                    {
                        DefaultClingerBehaviour();
                        DefaultSpineBehaviour();
                        DefaultEyeBehaviour();

                        if (AITimer < 100)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                DefaultArmBehaviour();
                            }
                            sawAlpha = Lerp(0, 1, InOutCirc.Invoke(AITimer / 100));
                            if (AITimer == 35)
                                SoundEngine.PlaySound(EbonianSounds.clawSwipe.WithVolumeScale(2), NPC.Center);
                            if (AITimer == 50)
                            {
                                CachedSlotIdsSystem.loopedSounds.Add("CSaw", new(SoundEngine.PlaySound(EbonianSounds.ghizasWheel.WithPitchOffset(0.2f).WithVolumeScale(2), NPC.Center), Type));
                            }
                            for (int i = 0; i < 3; i++)
                            {
                                MoveVerlet(ref clawVerlet[i], armVerlet.position + new Vector2(0, 120).RotatedBy(Helper.CircleDividedEqually(i, 3) + ToRadians(AITimer * 15)), Lerp(0, 1, InOutCirc.Invoke(AITimer / 100)));
                            }
                        }
                        else
                        {
                            if (NPC.Distance(player.Center) > 200)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.025f);
                            else NPC.velocity *= 0.97f;

                            if (player.Distance(armVerlet.position) < 130 && sawAlpha > 0.5f)
                                player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 100, 0);
                            for (int i = 0; i < 3; i++)
                            {
                                MoveVerlet(ref clawVerlet[i], armVerlet.position + new Vector2(0, 120).RotatedBy(Helper.CircleDividedEqually(i, 3) + ToRadians(AITimer * 15)), sawAlpha);
                            }
                            if (AITimer2 < 5 || disposablePos[0] == Vector2.Zero)
                                disposablePos[0] = armVerlet.position + Helper.FromAToB(armVerlet.position, player.Center, false) * 1.4f;
                            else if (AITimer < 280)
                            {
                                //if (AITimer2 % 2 == 0 && AITimer2 > 7 && disposablePos[0].Distance(armVerlet.position) > 200)
                                //{
                                //for (int i = 0; i < 3; i++)
                                //  Projectile.NewProjectile(null, clawVerlet[i].position, Helper.FromAToB(armVerlet.position, disposablePos[0]), ProjectileType<CecitiorClawSlash>(), 20, 0);
                                //}
                                MoveVerlet(ref armVerlet, disposablePos[0], Lerp(0, 1, InOutCirc.Invoke(AITimer2 / 60)));
                            }
                            if (++AITimer2 > 60)
                                AITimer2 = 0;

                            if (AITimer > 250 && AITimer < 300)
                                sawAlpha = Lerp(1, 0, InOutCirc.Invoke((AITimer - 250) / 50));
                            if (CachedSlotIdsSystem.loopedSounds.Any())
                                if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CSaw"))
                                {
                                    SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CSaw"].slotId;
                                    bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                    if (playing)
                                    {
                                        activeSound.Volume = sawAlpha * 2;
                                        activeSound.Position = armVerlet.position;
                                    }
                                    if (AITimer > 288)
                                        CachedSlotIdsSystem.ClearSound("CSaw");
                                }
                        }
                        if (AITimer >= 300)
                        {
                            Next = CecitiorCloneDash;
                            Reset();
                        }
                    }
                    break;
                case CecitiorCloneDash:
                    {
                        DefaultVerletAnimation();
                        if (AITimer < 200 && AITimer > 20)
                        {
                            if (AITimer < 100)
                                EbonianSystem.DarkAlpha = Lerp(0, 0.5f, InOutCirc.Invoke((AITimer - 20) / 80));
                            if (AITimer == 21)
                            {
                                StartScreaming();
                                openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                            open = true;
                            if (openOffset.X < 25)
                            {
                                openOffset.X += 3.5f;
                            }
                            openRotation = ToRadians(MathF.Sin((AITimer + 50) * 4) * 10);

                            rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            NPC.rotation = ToRadians(MathF.Sin(-(AITimer + 50) * 4) * 10);
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                        }
                        else
                        {
                            isScreaming = false;
                            open = false;
                            openOffset = Vector2.Lerp(openOffset, Vector2.Zero, 0.2f);
                            rotation = Helper.LerpAngle(rotation, 0, 0.2f);
                            openRotation = Helper.LerpAngle(openRotation, 0, 0.2f);
                        }

                        if (AITimer >= 100)
                        {
                            EbonianSystem.DarkAlpha = 0.5f;
                            if (AITimer == 150)
                            {
                                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                            }
                            if (AITimer > 160 && AITimer < 260 && AITimer % 5 == 0)
                            {
                                Vector2 pos = player.Center + new Vector2(500).RotatedByRandom(TwoPi);
                                SoundEngine.PlaySound(SoundID.Item8, pos);
                                Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity * 4) * 5, ProjectileType<CGhostCeci>(), 30, 0);
                            }
                        }

                        if (AITimer >= 350)
                        {
                            Next = GeyserSweep;
                            Reset();
                        }
                    }
                    break;
                case GeyserSweep:
                    {
                        DefaultVerletAnimation();
                        if (AITimer < 90)
                        {
                            rotation = -PiOver4 * 0.4f;
                            if (AITimer < 20)
                                disposablePos[0] = player.Center;
                            Vector2 to = Helper.TRay.Cast(disposablePos[0] + new Vector2(850, -200), Vector2.UnitY, 800, true) - new Vector2(200, 400);
                            if (NPC.Distance(to) > 200)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 40, 0.3f);
                            else NPC.velocity *= 0.9f;
                            Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedByRandom(PiOver2 * 0.7f);
                            Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10), newColor: Color.Lerp(Color.LawnGreen, Color.Maroon, Main.rand.NextFloat()), Scale: Main.rand.NextFloat(0.06f, 0.2f)).customData = NPC.Center + new Vector2(0, 20);

                            Dust.NewDustPerfect(pos, DustID.CursedTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                            Dust.NewDustPerfect(pos, DustID.IchorTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                        }
                        if (AITimer == 90)
                        {
                            StartLasering();
                            CameraSystem.ScreenShakeAmount = 20f;
                            SoundStyle s = EbonianSounds.chargedBeamImpactOnly;
                            laserSound = SoundEngine.PlaySound(s.WithPitchOffset(0.25f), NPC.Center);

                            CachedSlotIdsSystem.loopedSounds.Add("CBeam3", new(SoundEngine.PlaySound(EbonianSounds.deathrayLoop0.WithPitchOffset(-0.4f).WithVolumeScale(2.5f), NPC.Center), Type));

                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center + new Vector2(7, -14).RotatedBy(NPC.rotation), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                            Projectile.NewProjectile(null, NPC.Center, (rotation + PiOver2).ToRotationVector2(), ProjectileType<CBeam>(), 50, 0, -1, 0, 0.3f, NPC.whoAmI);
                        }
                        if (AITimer > 90 && AITimer < 235)
                        {
                            EbonianSystem.conglomerateSkyFlash = 5f + MathF.Sin(AITimer * 0.4f) * 0.5f;
                            if (CachedSlotIdsSystem.loopedSounds.Any())
                                if (CachedSlotIdsSystem.loopedSounds.ContainsKey("CBeam3"))
                                {
                                    SlotId cachedSound = CachedSlotIdsSystem.loopedSounds["CBeam3"].slotId;
                                    bool playing = SoundEngine.TryGetActiveSound(cachedSound, out var activeSound);
                                    if (playing)
                                    {
                                        activeSound.Pitch = Lerp(0.6f, 3f, InOutCirc.Invoke((AITimer - 80) / 155));

                                        if (AITimer > 195)
                                            activeSound.Volume = Lerp(1, 0, InOutCirc.Invoke((AITimer - 195) / 40));
                                    }
                                    if (AITimer > 193)
                                        CachedSlotIdsSystem.ClearSound("CBeam3");
                                }
                        }
                        else isLasering = false;
                        if (SoundEngine.TryGetActiveSound(laserSound, out var _laserSound))
                            _laserSound.Position = NPC.Center;
                        if (AITimer > 100)
                        {
                            Vector2 to = Helper.TRay.Cast(disposablePos[0] + new Vector2(-1050, -400), Vector2.UnitY, 800, true) - new Vector2(200, 400);
                            if (NPC.Distance(to) > 200)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.02f);
                            else
                            {
                                AITimer++;
                                NPC.velocity *= 0.9f;
                            }


                            if (AITimer % 9 == 0 && AITimer > 120 && NPC.Distance(to) > 200)
                            {
                                Vector2 vel = Vector2.UnitY.RotatedByRandom(PiOver4 * 0.2f);
                                Projectile.NewProjectile(null, Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 300, true) + new Vector2(0, 40), -vel, ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                                Projectile.NewProjectile(null, Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 300, true) - new Vector2(0, 40), vel, ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                            }
                        }
                        if (AITimer >= 260)
                        {
                            Next = BitesEyeToRainBlood;
                            Reset();
                        }
                    }
                    break;

                case BitesEyeToRainBlood:
                    {
                        DefaultClingerBehaviour();
                        DefaultSpineBehaviour();
                        DefaultArmBehaviour();
                        DefaultClawBehaviour();
                        if (AITimer < 40)
                        {
                            open = true;
                            openOffset = Vector2.Lerp(Vector2.Zero, new Vector2(300, 0), SmoothStep(0, 1, AITimer / 40));
                        }
                        if (AITimer > 20 && AITimer < 90)
                        {
                            float speed = Lerp(0, 30, InOutCirc.Invoke((AITimer - 20) / 70));
                            MoveVerlet(ref eyeVerlet,
                                NPC.Center + Main.rand.NextVector2Circular(speed, speed),
                                Lerp(0.2f, 1, InOutCirc.Invoke((AITimer - 20) / 70)));
                            openRotation = 0 + Lerp(0, Main.rand.NextFloat(-.5f, .5f), InOutCirc.Invoke((AITimer - 20) / 70));
                            NPC.rotation = rotation = 0 + Lerp(0, Main.rand.NextFloat(-.5f, .5f), InOutCirc.Invoke((AITimer - 20) / 70));
                        }
                        if (AITimer > 90)
                        {
                            openRotation = Lerp(openRotation, 0, 0.3f);
                            rotation = Lerp(rotation, 0, 0.3f);
                            NPC.rotation = Lerp(NPC.rotation, 0, 0.3f);
                            open = false;
                            if ((openOffset.Length() < 2.5f && openOffset.Length() > 1f) || (openOffset.Length() > -2.5f && openOffset.Length() < -1f))
                            {
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 15, 6, 30, 1000));
                                EbonianSystem.conglomerateSkyFlash = 10f;
                                for (int i = 0; i < 15; i++)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, Helper.CircleDividedEqually(i, 15).ToRotationVector2() * Main.rand.NextFloat(8, 18), ProjectileType<HostileGibs>(), 30, 0);
                                }
                            }
                        }

                        if (AITimer >= 130)
                        {
                            Next = PowerOfFriendship;
                            Reset();
                        }
                    }
                    break;

                case PowerOfFriendship:
                    {
                        DefaultArmBehaviour();
                        DefaultClawBehaviour();
                        DefaultClingerBehaviour();

                        /*if (AITimer < 50)
                            EbonianMod.DarkAlpha = Lerp(0, 0.5f, InOutCirc.Invoke(AITimer / 50));
                        else
                            EbonianMod.DarkAlpha = 0.5f;*/
                        if (AITimer < 60)
                            disposablePos[0] = player.Center;
                        if (AITimer < 20)
                        {
                            MoveVerlet(ref spineVerlet, NPC.Center + new Vector2(20, 100).RotatedBy(Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2));
                            MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(-20, 100).RotatedBy(Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2));
                        }
                        if (AITimer == 10)
                        {
                            Projectile.NewProjectile(null, spineVerlet.position, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 4, 6, 30, 1000));
                            EbonianSystem.conglomerateSkyFlash = 30f;
                        }
                        if (AITimer == 30)
                        {
                            Projectile.NewProjectile(null, eyeVerlet.position, Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0);
                            Projectile.NewProjectile(null, spineVerlet.position, Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                        }
                        if (AITimer > 30 && AITimer < 120 && AITimer % 10 == 0)
                        {
                            CameraSystem.ScreenShakeAmount = (AITimer - 50) * 0.2f;
                            Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedBy(ToRadians((AITimer - 70) * 3));
                            Projectile.NewProjectile(null, spineVerlet.position, vel, ProjectileType<CBeamInstant>(), 0, 0);

                            vel = Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedBy(-ToRadians((AITimer - 70) * 3) + PiOver4);
                            Projectile.NewProjectile(null, spineVerlet.position, vel, ProjectileType<CBeamInstant>(), 0, 0);
                        }
                        if (AITimer >= 200)
                        {
                            Next = ClawGrindTheFloorAndSpawnBlood;
                            Reset();
                        }
                    }
                    break;

                case ClawGrindTheFloorAndSpawnBlood:
                    {
                        DefaultClingerBehaviour();
                        DefaultSpineBehaviour();
                        DefaultEyeBehaviour();

                        if (AITimer < 40)
                        {
                            if (AITimer < 20)
                                disposablePos[0] = player.Center;
                            Vector2 to = Helper.TRay.Cast(disposablePos[0] + new Vector2(-850, -200), Vector2.UnitY, 800, true) - new Vector2(200, 400);
                            if (NPC.Distance(to) > 200)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 40, 0.4f);
                            else NPC.velocity *= 0.9f;
                            rotation = ToRadians(NPC.velocity.X * 2);
                            Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedByRandom(PiOver2 * 0.7f);
                        }


                        if (AITimer > 50)
                        {
                            rotation = ToRadians(NPC.velocity.X);
                            Vector2 to = Helper.TRay.Cast(disposablePos[0] + new Vector2(1050, -400), Vector2.UnitY, 800, true) - new Vector2(200, 400);
                            if (NPC.Distance(to) > 200)
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.02f);
                            else
                            {
                                AITimer += 3;
                                NPC.velocity *= 0.9f;
                            }


                            if (AITimer % 3 == 0 && AITimer > 80 && NPC.Distance(to) > 200)
                            {
                                Vector2 vel = Vector2.UnitY.RotatedByRandom(PiOver4 * 0.2f);
                            }
                            if (AITimer > 80 && NPC.Distance(to) > 200)
                            {

                                if (AITimer % 5 == 0)
                                {
                                    for (int i = 1; i < 4; i++)
                                    {
                                        Projectile.NewProjectile(null, armVerlet.position + NPC.velocity * 2, Helper.FromAToB(disposablePos[i], -disposablePos[i]), ProjectileType<CecitiorClawSlash>(), 20, 0);
                                        Projectile.NewProjectile(null, armVerlet.position + NPC.velocity * 2, -Vector2.UnitY.RotatedByRandom(PiOver4) * Main.rand.NextFloat(5, 15), ProjectileType<HostileGibs>(), 20, 0);
                                    }
                                    for (int i = 1; i < 4; i++)
                                        disposablePos[i] = Main.rand.NextVector2CircularEdge(130, 130);
                                }

                                MoveVerlet(ref armVerlet, Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 500) - new Vector2(0, 80), 0.3f);
                                for (int i = 0; i < 3; i++)
                                    MoveVerlet(ref clawVerlet[i], armVerlet.position + disposablePos[i + 1], 0.5f);
                            }
                            else
                            {
                                DefaultArmBehaviour();
                                DefaultClawBehaviour();
                            }
                        }
                        else
                        {
                            DefaultArmBehaviour();
                            DefaultClawBehaviour();
                        }
                        if (AITimer >= 180)
                        {
                            Next = ClawTriangle;
                            Reset();
                        }
                    }
                    break;
                case ClawTriangle:
                    {
                        DefaultClingerBehaviour();
                        DefaultSpineBehaviour();
                        DefaultEyeBehaviour();

                        if (AITimer2 <= 1)
                        {
                            if (AITimer == 1 && AITimer2 == 0)
                            {
                                disposablePos[1] = new Vector2(Main.rand.NextFloat(Pi), 0);
                                StartScreaming();
                                openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                                SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                            }
                            if (AITimer % 20 == 0)
                                Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                        }
                        else isScreaming = false;

                        if (NPC.Distance(player.Center) > 200)
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.15f);
                        else NPC.velocity *= 0.9f;
                        if (disposablePos[0] != Vector2.Zero)
                            MoveVerlet(ref armVerlet, disposablePos[0], 0.4f);
                        if (AITimer < 80)
                        {
                            disposablePos[0] = player.Center;
                            AITimer3 = Lerp(0, 160, InOutCirc.Invoke(AITimer / 80f));
                            for (int i = 0; i < clawVerlet.Length; i++)
                            {
                                MoveVerlet(ref clawVerlet[i], armVerlet.position + (Helper.CircleDividedEqually(i, clawVerlet.Length) + disposablePos[1].X).ToRotationVector2() * (150 + AITimer3), 0.5f);
                                disposablePos[2 + i] = clawVerlet[i].position;
                                //Dust.NewDustPerfect(clawVerlet[i].position, DustType<LineDustFollowPoint>(), Helper.FromAToB(disposablePos[2 + i], disposablePos[0]).RotatedByRandom(PiOver4 / 8f) * Main.rand.NextFloat(10, 20), 0, Color.Maroon, Main.rand.NextFloat(0.05f, 0.15f));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < clawVerlet.Length; i++)
                            {
                                if (AITimer == 81)
                                    Projectile.NewProjectile(null, clawVerlet[i].position, Helper.FromAToB(disposablePos[2 + i], disposablePos[0]), ProjectileType<CecitiorClawSlash>(), 20, 0, ai1: 1);
                                MoveVerlet(ref clawVerlet[i], disposablePos[0], 0.4f);
                            }
                        }
                        if (AITimer >= 100)
                        {
                            if (AITimer2 > Main.rand.Next(2))
                            {
                                Next = ClingerComboType1;
                                Reset();
                                AITimer = 50;
                            }
                            else
                            {
                                AITimer = 0;
                                AITimer3 = 0;
                                AITimer2++;
                            }
                        }
                    }
                    break;
                case ClingerComboType1:
                    {
                        if (NPC.Distance(player.Center) > 200)
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 100) + Helper.FromAToB(player.Center, NPC.Center) * 100) * 15, 0.15f);
                        else NPC.velocity *= 0.9f;
                        if (AITimer < 160)
                        {
                            DefaultSpineBehaviour();
                            DefaultArmBehaviour();
                            DefaultClawBehaviour();
                            AITimer3 = 100 + MathF.Sin(AITimer * 0.1f) * 60;
                            MoveVerlet(ref clingerVerlet, player.Center + new Vector2(200 + AITimer3, 0).RotatedBy(ToRadians(AITimer * 4)));
                            MoveVerlet(ref eyeVerlet, player.Center + new Vector2(-200 - AITimer3, 0).RotatedBy(ToRadians(AITimer * 4)));
                        }
                        else DefaultVerletAnimation();
                        if (AITimer < 150)
                        {
                            if (AITimer % 20 == 10)
                            {
                                for (int i = 0; i < 30; i++)
                                    Dust.NewDustPerfect(eyeVerlet.position, DustID.IchorTorch, Helper.FromAToB(eyeVerlet.position, player.Center).RotatedByRandom(PiOver4) * Main.rand.NextFloat(5, 15), Scale: 1.5f).noGravity = true;
                                SoundEngine.PlaySound(EbonianSounds.cecitiorSpit, eyeVerlet.position);
                                Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(eyeVerlet.position, player.Center), ProjectileType<CecitiorTeeth>(), 20, 0);
                            }
                            else if (AITimer % 20 == 0)
                                Projectile.NewProjectile(null, clingerVerlet.position, Helper.FromAToB(clingerVerlet.position, player.Center), ProjectileType<TFlameThrower4>(), 20, 0);
                        }
                        if (AITimer >= 180)
                        {
                            Next = EyeSpin;
                            Reset();
                        }
                    }
                    break;
                case EyeSpin:
                    {
                        if (AITimer == 1 && AITimer2 == 0)
                        {
                            disposablePos[1] = new Vector2(Main.rand.NextFloat(Pi), 0);
                            StartScreaming();
                            openSound = SoundEngine.PlaySound(EbonianSounds.cecitiorOpen, NPC.Center);
                            SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        }
                        if (AITimer % 20 == 0)
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                        if (AITimer % 10 == 0)
                            Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ConglomerateScream>(), 0, 0);

                        DefaultArmBehaviour();
                        DefaultClawBehaviour();
                        DefaultClingerBehaviour();
                        DefaultSpineBehaviour();
                        AITimer3 = 100 + MathF.Sin(AITimer * 0.1f) * 60;
                        MoveVerlet(ref eyeVerlet, NPC.Center + new Vector2(-200 - AITimer3, 0).RotatedBy(ToRadians(AITimer * 10)));
                        if (AITimer > 50 && AITimer % 7 == 0 && AITimer < 140)
                        {
                            Projectile.NewProjectile(null, eyeVerlet.position, Helper.FromAToB(NPC.Center, eyeVerlet.position) * Main.rand.NextFloat(1, 5), ProjectileType<CecitiorEyeP>(), 20, 0);
                        }
                        if (AITimer >= 160)
                        {
                            Next = SmallDeathRay;
                            Reset();
                        }
                    }
                    break;
                case SmallDeathRay:
                    {
                        for (int i = 0; i < 2; i++)
                            DefaultVerletAnimation();
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;

                        if (AITimer < 100)
                        {
                            AITimer2++;
                            AITimer3 = Lerp(0, 0.2f, InOutCirc.Invoke(AITimer / 100));
                            if (AITimer2 == 1)
                                disposablePos[0] = Main.rand.NextVector2Unit();
                            else
                            {
                                if (AITimer2 % 5 == 0 && AITimer > 30)
                                    Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center), ProjectileType<CBeamSmall>(), 0, 0, ai1: -.9f);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -700).RotatedBy(MathF.Sin(AITimer2 * 0.1f) * 2), false) * 0.1f, AITimer3);
                            }
                        }
                        else
                            NPC.velocity *= 0.8f;

                        if (AITimer >= 160)
                        {
                            Next = OpenAndShootStuff;
                            Reset();
                        }
                    }
                    break;
                case OpenAndShootStuff:
                    {
                        if (AITimer >= 200)
                        {
                            Next = BloodAndWormSpit;
                            Reset();
                        }
                    }
                    break;
            }
        }
        float sawAlpha;
        void Reset()
        {
            CachedSlotIdsSystem.UnloadSounds();
            sawAlpha = 0;
            AITimer = 0;
            AITimer2 = 0;
            open = false;
            isScreaming = false;
            isLasering = false;
            openOffset = Vector2.Zero;
            openRotation = 0;
            AITimer3 = 0;
            rotation = 0;
            NPC.velocity = Vector2.Zero;
            for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
            AIState = Idle;
            //Next = OpenAndShootStuff;
            NPC.damage = 0;
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 5; i++)
            {
                float scale = 2;
                switch (i)
                {
                    case 1:
                        scale = 4;
                        break;
                    case 2:
                        scale = 2;
                        break;
                    case 3:
                        scale = 3;
                        break;
                    case 4:
                        scale = 1;
                        break;
                }
                tendonVerlet[i] = new(NPC.Center, 2, 18, 1 * scale, true, true, (int)(2.5f * scale));
            }
        }
        public struct VerletStruct
        {
            public Vector2 position;
            public Vector2[] oldPosition = new Vector2[25];
            public float[] oldRotation = new float[25];
            public Verlet verlet;
            public VerletStruct(Vector2 _position, Verlet _verlet)
            {
                position = _position;
                verlet = _verlet;
            }
        }
        Verlet[] tendonVerlet = new Verlet[5];
        public VerletStruct spineVerlet, clingerVerlet, eyeVerlet, armVerlet;
        public VerletStruct[] clawVerlet = new VerletStruct[3];
        public VerletStruct[] gutVerlets = new VerletStruct[10];
        int hookFrame = 1;
        void SpawnVerlets()
        {
            spineVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 14, 30, -0.25f, stiffness: 70));
            clingerVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 10, 20, 0.25f, stiffness: 70));
            eyeVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 10, 20, 0.15f, stiffness: 70));
            armVerlet = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 12, 30, 0f, stiffness: 70));
            for (int i = 0; i < 3; i++)
                clawVerlet[i] = new VerletStruct(NPC.Center, new Verlet(NPC.Center, 12, 30, 0f, stiffness: 50));
        }
        void MoveVerlet(ref VerletStruct verlet, Vector2 pos, float t = 0.2f)
        {
            if (verlet.verlet != null)
                verlet.position = Vector2.Lerp(verlet.position, pos, t);
        }
        void DefaultVerletAnimation()
        {
            DefaultSpineBehaviour();

            DefaultClingerBehaviour();

            DefaultEyeBehaviour();

            DefaultArmBehaviour();

            DefaultClawBehaviour();
        }
        int[] verletRotSeeds = new int[4];
        void DefaultSpineBehaviour()
        {
            UnifiedRandom rand = new(verletRotSeeds[0]);
            float rotOff = MathF.Sin((0.6f + GetInstance<EbonianSystem>().constantTimer) * (isScreaming ? 0.16f : 0.015f)) * rand.NextFloat(0.15f, 0.5f);
            if (spineVerlet.verlet != null)
                spineVerlet.position = Vector2.Lerp(spineVerlet.position, NPC.Center + openOffset + new Vector2(260 * (rotOff * 0.4f + 1), 60).RotatedBy(NPC.rotation + rotOff), 0.2f);
        }
        void DefaultClingerBehaviour()
        {
            UnifiedRandom rand = new(verletRotSeeds[1]);
            float rotOff = MathF.Sin((0.23f + GetInstance<EbonianSystem>().constantTimer) * (isScreaming ? 0.2f : 0.005f)) * rand.NextFloat(0.15f, 0.5f);
            if (clingerVerlet.verlet != null)
                clingerVerlet.position = Vector2.Lerp(clingerVerlet.position, NPC.Center + openOffset + new Vector2(200 * (rotOff * 0.4f + 1), -20).RotatedBy(NPC.rotation - rotOff), 0.2f);
        }
        void DefaultEyeBehaviour()
        {
            UnifiedRandom rand = new(verletRotSeeds[2]);
            float rotOff = MathF.Sin((GetInstance<EbonianSystem>().constantTimer) * (isScreaming ? 0.15f : 0.01f)) * rand.NextFloat(0.15f, 0.5f);
            if (eyeVerlet.verlet != null)
                eyeVerlet.position = Vector2.Lerp(eyeVerlet.position, NPC.Center - openOffset + new Vector2(-150 * (rotOff * 0.4f + 1), 60).RotatedBy(NPC.rotation + rotOff), 0.2f);
        }
        void DefaultArmBehaviour()
        {
            UnifiedRandom rand = new(verletRotSeeds[3]);
            float rotOff = MathF.Sin((1.6f + GetInstance<EbonianSystem>().constantTimer) * (isScreaming ? 0.23f : 0.015f)) * rand.NextFloat(0.15f, 0.5f);
            if (armVerlet.verlet != null)
                armVerlet.position = Vector2.Lerp(armVerlet.position, NPC.Center - openOffset + new Vector2(-250 * (rotOff * 0.4f + 1), -100).RotatedBy(NPC.rotation - rotOff), 0.2f);
        }
        void DefaultClawBehaviour()
        {
            if (armVerlet.verlet != null)
                for (int i = 0; i < 3; i++)
                {
                    if (clawVerlet[i].verlet != null)
                    {
                        clawVerlet[i].position = Vector2.Lerp(clawVerlet[i].position, armVerlet.position + Helper.FromAToB(NPC.Center - openOffset, armVerlet.position) * 100 + Helper.CircleDividedEqually(i, 3).ToRotationVector2() * 60, 0.2f);
                    }
                }
        }
        void DrawVerlets(SpriteBatch spriteBatch)
        {
            if (spineVerlet.verlet != null)
            {
                spineVerlet.verlet.Update(NPC.Center + new Vector2(22, -10) + openOffset, spineVerlet.position);
                spineVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_ClingerSegment", null, Helper.Empty));
                Texture2D tex = Helper.GetTexture(Texture + "_Spine");
                Main.spriteBatch.Draw(tex, spineVerlet.position - Main.screenPosition, null, new Color(Lighting.GetSubLight(spineVerlet.position)) * 1.25f, Pi, tex.Size() / 2, 1, SpriteEffects.None, 0);
            }
            if (clingerVerlet.verlet != null)
            {
                clingerVerlet.verlet.Update(NPC.Center + new Vector2(22, 10) + openOffset, clingerVerlet.position);
                clingerVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_ClingerSegment", null, Texture + "_Clinger", _useRotEnd: true, _endRot: Pi));
            }
            if (eyeVerlet.verlet != null)
            {
                eyeVerlet.verlet.Update(NPC.Center + new Vector2(-22, -10) - openOffset, eyeVerlet.position);
                eyeVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_EyeSegment", null, Helper.Empty));

                Texture2D eye = Helper.GetTexture(Texture + "_Eye");
                Texture2D eyeball = Helper.GetTexture(Texture + "_Eyeball");
                Texture2D eyeGlow = Helper.GetTexture(Texture + "_Eye_Glow");
                Texture2D eyeballGlow = Helper.GetTexture(Texture + "_Eyeball_Glow");
                Texture2D eyeShell = Helper.GetTexture(Texture + "_Eye_Shell");

                Main.EntitySpriteDraw(eye, eyeVerlet.position - Main.screenPosition, null, new Color(Lighting.GetSubLight(eyeVerlet.position)) * 1.25f, 0, eye.Size() / 2, 1, SpriteEffects.None);

                Main.EntitySpriteDraw(eyeball, eyeVerlet.position + new Vector2(2, -2) - Main.screenPosition, null, new Color(Lighting.GetSubLight(eyeVerlet.position)) * 1.25f, Helper.FromAToB(eyeVerlet.position, Main.LocalPlayer.Center).ToRotation() + PiOver2, eyeball.Size() / 2, 1, SpriteEffects.None);
                Main.EntitySpriteDraw(eyeballGlow, eyeVerlet.position + new Vector2(2, -2) - Main.screenPosition, null, Color.White, Helper.FromAToB(eyeVerlet.position, Main.LocalPlayer.Center).ToRotation() + PiOver2, eyeball.Size() / 2, 1, SpriteEffects.None);

                Main.EntitySpriteDraw(eyeShell, eyeVerlet.position - Main.screenPosition, null, new Color(Lighting.GetSubLight(eyeVerlet.position)) * 1.25f, 0, eye.Size() / 2, 1, SpriteEffects.None);
                Main.EntitySpriteDraw(eyeGlow, eyeVerlet.position - Main.screenPosition, null, Color.White, 0, eye.Size() / 2, 1, SpriteEffects.None);


            }
            if (armVerlet.verlet != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (clawVerlet[i].verlet != null)
                    {
                        clawVerlet[i].verlet.Update(armVerlet.position, clawVerlet[i].position);
                        clawVerlet[i].verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_EyeSegment", null, "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame));
                        clawVerlet[i].verlet.Draw(spriteBatch, new VerletDrawData(Helper.Empty, null, "NPCs/Cecitior/Hook/CecitiorHook_" + hookFrame + "_Glow", _useColor: true, _color: Color.White));
                    }
                }
                armVerlet.verlet.Update(NPC.Center + new Vector2(-22, -10) - openOffset, armVerlet.position);
                armVerlet.verlet.Draw(spriteBatch, new VerletDrawData(Texture + "_EyeSegment", null, "Gores/Crimorrhage3"));
                Lighting.AddLight(armVerlet.position, TorchID.Ichor);

                Texture2D spin = ExtraTextures2.slash_06;
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(spin, armVerlet.position - Main.screenPosition, null, Color.Maroon * sawAlpha, Main.GameUpdateCount * 0.4f, spin.Size() / 2, 1.4f, SpriteEffects.None, 0); ;
                Main.spriteBatch.Draw(spin, armVerlet.position - Main.screenPosition, null, Color.Maroon * sawAlpha, Main.GameUpdateCount * 0.4f + Pi, spin.Size() / 2, 1.4f, SpriteEffects.None, 0); ;
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        void MiscFX()
        {
            if (isScreaming && !open && AIState != GrabAttack && AIState != BayleLaser)
            {
                NPC.rotation = rotation + Main.rand.NextFloat(-0.4f, 0.4f);
            }
            Lighting.AddLight(NPC.TopLeft - openOffset, TorchID.Ichor);
            Lighting.AddLight(NPC.BottomRight + openOffset, TorchID.Cursed);
            SoundStyle selected = EbonianSounds.flesh0;
            switch (Main.rand.Next(3))
            {
                case 0:
                    selected = EbonianSounds.flesh1;
                    break;
                case 1:
                    selected = EbonianSounds.flesh2;
                    break;
            }
            if (!savedSound.IsValid || !SoundEngine.TryGetActiveSound(savedSound, out var activeSound) || !activeSound.IsPlaying)
            {
                savedSound = SoundEngine.PlaySound(selected, NPC.Center);
            }
        }
    }
}
