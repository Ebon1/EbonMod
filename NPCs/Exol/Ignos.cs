using EbonianMod.Projectiles;
using EbonianMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Exol;
using System.IO;
using FullSerializer.Internal;
using System.Reflection.Metadata;
using Terraria.Map;
using EbonianMod.Projectiles.Friendly.Underworld;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;

namespace EbonianMod.NPCs.Exol
{
    public class Ignos : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Soul"),
                new FlavorTextBestiaryInfoElement("One of the greatest warriors known, and one of the few able to go toe to toe with Inferos. Even in death, his very soul yearns for a battle as grand as that of Inferos, if not grander, before he can finally rest."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 134;
            NPC.height = 148;
            NPC.damage = 0;
            NPC.defense = 30;
            NPC.lifeMax = 35000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Ignos");
            }
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[Type] = 100;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public float AIState { get => NPC.ai[0]; set => NPC.ai[0] = value; }
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
        public float IdleState
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }
        const int Spawn = 0, Idle = -1;
        //BIG BOY ATTACKS
        const int SwordSlashes = 1, RykardSpiral = 2, SwordSlashesVariant2 = 3, HoldSwordUpAndChannelSoulVortex = 4, idkwhatthisshouldbe = 5, InfernalEye = 6, GreatArrowRain = 7, SoulStorm = 8, BigSwordCombo = 9,
            GaelDiscus = 10, LavaApocalypse = 11, SwordWaveSpam = 12;
        //IDLE ATTACKS
        const int QuickSlashes = 0, SwordWave = 1, MinorArrowHail = 2, SkullBurst = 3, SethKnife = 4, MeleeComboToCloseDistance = 5, GiantExplosiveArrow = 6, RenallaHomingBeams = 7, FireGiantLavaExplosion = 8, SwordThrustDash = 9;
        //max idles: 3
        public int IdleAttacks;
        public int NextAttack = 1;
        Vector2 lastPos;
        public float Ease(float f) => 1 - (float)Math.Pow(2, 10 * f - 10);
        public float ScaleFunction(float progress) => 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        SoundStyle pull = new("EbonianMod/Sounds/bowPull")
        {
            PitchVariance = 0.25f,
        };
        SoundStyle release = new("EbonianMod/Sounds/bowRelease")
        {
            PitchVariance = 0.25f,
        };
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            PlayerDetection();
            AITimer++;
            if (turningSword)
            {
                swingTime--;
                int direction = (int)swordDirection;
                float swingProgress = Ease(Utils.GetLerpValue(0f, maxSwingTime, swingTime));
                float defRot = swordRotationFocus;
                float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
                Vector2 position = NPC.Center + rotation.ToRotationVector2() * swordPositionOffset * ScaleFunction(swingProgress);
                swordRotation = (position - NPC.Center).ToRotation() + MathHelper.PiOver4 * 0.5f;
                if (swingTime <= 0)
                {
                    turningSword = false;
                }
            }
            if (trailAlpha <= 0f)
            {
                for (int i = 0; i < NPC.oldPos.Length; i++)
                    NPC.oldPos[i] = NPC.position;
            }
            switch (AIState)
            {
                case Spawn:
                    {
                        Reset();
                        AIState = Idle;
                    }
                    break;
                case Idle:
                    {
                        switch (IdleState)
                        {
                            case QuickSlashes:
                                IdleMovement(new Vector2(0, -200));
                                swordRotationFocus = MathHelper.Lerp(swordRotationFocus, -Vector2.UnitY.ToRotation(), 0.1f);
                                swordRotation = MathHelper.Lerp(swordRotation, -Vector2.UnitY.ToRotation(), 0.1f);
                                if (AITimer % 10 == 0 && AITimer < 50)
                                {
                                    Projectile.NewProjectile(null, player.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                                }
                                if (AITimer > 75)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case SwordWave:
                                IdleMovement();
                                if (AITimer == 30)
                                {
                                    PerformBasicSwordSlash(40);
                                    Vector2 vel = Helper.FromAToB(NPC.Center, player.Center);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 30, vel * 15, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                                }
                                if (AITimer > 50)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case MinorArrowHail:
                                bowRotation = -MathHelper.PiOver2;
                                if (AITimer < 30)
                                {
                                    bowAlpha = Math.Min(bowAlpha + 0.04f, 1);
                                    swordAlpha = Math.Max(swordAlpha - 0.04f, 0);
                                }
                                if (AITimer == 30)
                                    SoundEngine.PlaySound(pull, NPC.Center);
                                if (AITimer == 60)
                                    SoundEngine.PlaySound(release, NPC.Center);
                                if (AITimer > 30 && AITimer < 60)
                                {
                                    IdleMovement(new Vector2(200 * Helper.FromAToB(NPC.Center, player.Center).X, -100));
                                    bowString += 0.65f;
                                }
                                else NPC.velocity *= 0.98f;
                                if (AITimer > 30 && AITimer < 60)
                                    for (int i = 0; i < 1 + MathHelper.Clamp((AITimer - 30) / 7.5f, 0, 4); i++)
                                    {
                                        arrowAlpha[i] = MathHelper.Lerp(arrowAlpha[i], 1f, 0.2f);
                                    }
                                if (AITimer > 60 && AITimer < 67)
                                {
                                    bowString = MathHelper.Lerp(bowString, -5, 0.4f);
                                    for (int i = 0; i < arrowAlpha.Length; i++)
                                    {
                                        arrowAlpha[i] = 0;
                                    }
                                    for (int i = 0; i < 5; i++)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation), new Vector2(Helper.FromAToB(NPC.Center, player.Center, false).X * 0.01f + Main.rand.NextFloat(-1, 1), -5), ModContent.ProjectileType<MagmaArrowHostile>(), 30, 0);
                                }
                                if (AITimer > 67)
                                {
                                    swordAlpha = Math.Min(swordAlpha + 0.04f, 1);
                                    bowAlpha = Math.Max(bowAlpha - 0.04f, 0);
                                    bowString = MathHelper.Lerp(bowString, 0, 0.2f);
                                }

                                if (AITimer >= 80)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case SkullBurst:
                                if (AITimer < 30)
                                {
                                    Vector2 pos = NPC.Center + 200 * Main.rand.NextVector2Unit();
                                    Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                                    a.noGravity = false;
                                    a.customData = 1;
                                }
                                if (AITimer == 40)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave>(), 0, 0);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Vector2 vel = Main.rand.NextVector2Unit() * 5;
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ESkullEmoji>(), 15, 0);
                                    }
                                }
                                if (AITimer > 80)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case SethKnife:
                                IdleMovement(new Vector2(0, -200));
                                if (AITimer > 30 && AITimer % 5 == 0 && AITimer < 60)
                                {
                                    NPC.velocity *= 0.75f;
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, 50).RotatedBy(MathHelper.ToRadians((AITimer - 45) * 3.5f)), new Vector2(0, 1).RotatedBy(MathHelper.ToRadians((AITimer - 45) * 3.5f)) * 4, ModContent.ProjectileType<EFire>(), 15, 0, player.whoAmI).timeLeft = 170 + (int)AITimer;
                                }
                                if (AITimer > 60 && AITimer % 5 == 0 && AITimer < 90)
                                {
                                    NPC.velocity *= 0.75f;
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, 50).RotatedBy(MathHelper.ToRadians((AITimer - 75) * 2.5f)), new Vector2(0, 1).RotatedBy(MathHelper.ToRadians((AITimer - 75) * 2.5f)) * 4, ModContent.ProjectileType<EFire>(), 15, 0, player.whoAmI).timeLeft = 170 + (int)AITimer;
                                }
                                if (AITimer > 130)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case MeleeComboToCloseDistance:
                                if ((AITimer % 40 == 0 && AITimer <= 80) || AITimer == 1)
                                {
                                    PerformBasicSwordSlash(40);
                                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 10;
                                    Vector2 vel = Helper.FromAToB(NPC.Center, player.Center);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 30, vel * 10, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                                }
                                if (AITimer > 100)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                else
                                    NPC.damage = 50;
                                break;
                            case GiantExplosiveArrow:
                                if (AITimer < 60)
                                    bowRotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation();
                                if (AITimer < 30)
                                {
                                    bowAlpha = Math.Min(bowAlpha + 0.04f, 1);
                                    swordAlpha = Math.Max(swordAlpha - 0.04f, 0);
                                }
                                if (AITimer == 30)
                                    SoundEngine.PlaySound(pull, NPC.Center);
                                if (AITimer > 30 && AITimer < 60)
                                {
                                    IdleMovement(new Vector2(200 * Helper.FromAToB(NPC.Center, player.Center).X, -200));
                                    bowString += 0.65f;
                                }
                                else NPC.velocity *= 0.95f;
                                if (AITimer > 30 && AITimer < 60)
                                    arrowAlpha[2] = MathHelper.Lerp(arrowAlpha[2], 1f, 0.2f);
                                if (AITimer > 80 && AITimer < 87)
                                {
                                    bowString = MathHelper.Lerp(bowString, -5, 0.4f);
                                    for (int i = 0; i < arrowAlpha.Length; i++)
                                    {
                                        arrowAlpha[i] = 0;
                                    }
                                }
                                if (AITimer == 80)
                                {
                                    SoundEngine.PlaySound(release, NPC.Center);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation), new Vector2(20, 0).RotatedBy(bowRotation), ModContent.ProjectileType<ExolGreatArrow>(), 40, 0);
                                }
                                if (AITimer > 87)
                                {
                                    swordAlpha = Math.Min(swordAlpha + 0.04f, 1);
                                    bowAlpha = Math.Max(bowAlpha - 0.04f, 0);
                                    bowString = MathHelper.Lerp(bowString, 0, 0.2f);
                                }
                                if (AITimer > 120)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case RenallaHomingBeams:
                                if (AITimer % 5 == 0 && AITimer < 50)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2CircularEdge(20, 10), ModContent.ProjectileType<EHomingBeam>(), 40, 0);
                                }
                                if (AITimer > 50)
                                    IdleMovement(new Vector2(0, -200));
                                if (AITimer > 120)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case FireGiantLavaExplosion:
                                if (AITimer < 30)
                                    IdleMovement(new Vector2(0, -100));
                                else
                                    NPC.velocity *= 0.8f;

                                if (AITimer % 30 == 0 && AITimer <= 120)
                                {
                                    Vector2 vel = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), AITimer % 60 == 0 ? -1 : 1).RotatedBy(MathHelper.ToRadians(AITimer2));
                                    NPC.velocity = -vel * 8;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 0.5f, ModContent.ProjectileType<EMine>(), 0, 0);
                                    EbonianSystem.ScreenShakeAmount = 5f;
                                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                                    for (int i = 0; i < 8; i++)
                                    {
                                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, -4)), ModContent.ProjectileType<Gibs>(), 30, 0, ai2: 1);
                                        a.friendly = false;
                                        a.hostile = true;
                                    }
                                }
                                if (AITimer >= 180)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                            case SwordThrustDash:
                                if (AITimer < 35f)
                                {
                                    swordRotationFocus = MathHelper.Lerp(swordRotationFocus, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver4 * 0.5f, 0.25f);
                                    swordRotation = MathHelper.Lerp(swordRotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver4 * 0.5f, 0.25f);
                                }
                                if (AITimer >= 40 && AITimer <= 50)
                                {
                                    NPC.damage = 50;
                                    trailAlpha = Math.Min(trailAlpha + 0.1f, 1);
                                    NPC.velocity += Vector2.UnitX.RotatedBy(swordRotation - MathHelper.PiOver4 * 0.5f) * 3.5f;
                                }
                                else NPC.velocity *= 0.95f;
                                if (AITimer > 70)
                                {
                                    trailAlpha = Math.Max(trailAlpha - 0.04f, 0);
                                }
                                if (AITimer > 85)
                                {
                                    if (--IdleAttacks <= 0)
                                        AIState = NextAttack;
                                    Reset(true);
                                }
                                break;
                        }
                    }
                    break;
                case SwordSlashes:
                    {
                        IdleMovement(new Vector2(0, -200));
                        swordRotationFocus = MathHelper.Lerp(swordRotationFocus, -Vector2.UnitY.ToRotation(), 0.1f);
                        swordRotation = MathHelper.Lerp(swordRotation, -Vector2.UnitY.ToRotation(), 0.1f);
                        if (AITimer % 3 == 0 && AITimer >= 30 && AITimer <= 100)
                        {
                            Projectile.NewProjectile(null, player.Center - new Vector2((AITimer - 30 - 50) * 40, 0), new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                        if (AITimer == 75 || AITimer == 145)
                            Projectile.NewProjectile(null, player.Center, new Vector2(0, -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        if (AITimer % 3 == 0 && AITimer >= 90 && AITimer <= 160)
                        {
                            Projectile.NewProjectile(null, player.Center + new Vector2((AITimer - 90 - 50) * 40, 0), new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), -1), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = RykardSpiral;
                        }
                    }
                    break;
                case RykardSpiral:
                    {
                        if (AITimer < 40)
                        {
                            IdleMovement();
                            Vector2 pos = NPC.Center + 300 * Main.rand.NextVector2Unit();
                            Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                            a.noGravity = false;
                            a.customData = 1;
                        }
                        if (AITimer == 45)
                        {
                            NPC.velocity = Vector2.Zero;
                            Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ModContent.ProjectileType<RykardSkullSpiral>(), 30, 0);
                            }
                        }
                        if (AITimer >= 250)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = SwordSlashesVariant2;
                        }
                    }
                    break;
                case SwordSlashesVariant2:
                    {
                        if (AITimer < 15)
                            IdleMovement();
                        else
                            NPC.velocity *= 0.9f;
                        if (AITimer > 15 && AITimer % 3 == 0 && AITimer2 < 15)
                        {
                            AITimer2++;
                            float angle = Helper.CircleDividedEqually(AITimer2, 15);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 200).RotatedBy(angle), Vector2.One.RotatedBy(angle), ModContent.ProjectileType<IgnosSlashTelegraph>(), 30, 0);
                        }
                        if (AITimer >= 150)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = HoldSwordUpAndChannelSoulVortex;
                        }
                    }
                    break;
                case HoldSwordUpAndChannelSoulVortex:
                    {
                        if (AITimer < 30)
                        {
                            IdleMovement(speedLimit: 20);
                            vortexAlpha = Math.Min(vortexAlpha + 0.04f, 1);
                        }
                        else
                            NPC.velocity *= 0.85f;
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<IgArena>(), 0, 0);
                        }
                        if (AITimer > 40 && AITimer % 10 == 0 && AITimer < 80)
                        {
                            Vector2 vel = Main.rand.NextVector2Circular(5, 5);
                            Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 100), vel, ModContent.ProjectileType<ESkullEmoji>(), 30, 0);
                        }
                        if (AITimer == 150)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center - new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                        if (AITimer == 160)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 4);
                                Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 100), Main.rand.NextFloat(5, 10) * Vector2.UnitX.RotatedBy(angle), ModContent.ProjectileType<ESkullEmoji>(), 30, 0);
                            }
                        }
                        if (AITimer > 200)
                        {
                            vortexAlpha = Math.Max(vortexAlpha - 0.04f, 0);
                        }
                        if (AITimer >= 370)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = idkwhatthisshouldbe;
                        }
                    }
                    break;
                case idkwhatthisshouldbe:
                    {
                        if (AITimer >= 1)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = InfernalEye;
                        }
                    }
                    break;
                case InfernalEye:
                    {
                        if (AITimer == 1)
                            for (int i = 0; i < 5; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 7);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center + new Vector2(250, 0).RotatedBy(angle), Vector2.Zero, ModContent.ProjectileType<IgnosEye>(), 0, 0, ai0: angle, ai1: 350, ai2: 3);
                            }
                        if (AITimer % 40 == 0 && AITimer <= 200)
                        {
                            foreach (Projectile projectile in Main.projectile)
                            {
                                if (projectile.type == ModContent.ProjectileType<IgnosEye>() && projectile.active)
                                {
                                    NPC.Center = projectile.Center;
                                    PerformBasicSwordSlash();
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center) * 15, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                                    projectile.Kill();
                                    break;
                                }
                            }
                        }
                        if (AITimer >= 340)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = GreatArrowRain;
                        }
                    }
                    break;
                case GreatArrowRain:
                    {
                        bowRotation = -MathHelper.PiOver2;
                        if (AITimer < 30)
                            bowAlpha = Math.Min(bowAlpha + 0.04f, 1);
                        if (AITimer > 30 && AITimer < 60)
                        {
                            bowString += 0.65f;
                        }
                        if (AITimer > 30 && AITimer < 100)
                            for (int i = 0; i < 1 + MathHelper.Clamp((AITimer - 30) / 7.5f, 0, 4); i++)
                            {
                                arrowAlpha[i] = MathHelper.Lerp(arrowAlpha[i], 1f, 0.1f);
                            }
                        if (AITimer > 100 && AITimer < 107)
                        {
                            bowString = MathHelper.Lerp(bowString, -5, 0.4f);
                            for (int i = 0; i < arrowAlpha.Length; i++)
                            {
                                arrowAlpha[i] = 0;
                            }
                            for (int i = 0; i < 5; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation), new Vector2(Helper.FromAToB(NPC.Center, player.Center, false).X * 0.01f + Main.rand.NextFloat(-1, 1), -25), ModContent.ProjectileType<MagmaArrowHostile>(), 30, 0);
                        }
                        if (AITimer > 107)
                            bowString = MathHelper.Lerp(bowString, 0, 0.2f);
                        if (AITimer > 155 && AITimer < 165)
                        {
                            float off = AITimer % 10 == 0 ? 20 : 10;
                            for (int i = 0; i < 2; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), Helper.TRay.Cast(new Vector2(player.Center.X, (Main.maxTilesY * 16) / 2), -Vector2.UnitY, 1200) - new Vector2(0, 100), new Vector2(Main.rand.NextFloat(-off, off), Main.rand.NextFloat(5, 10)), ModContent.ProjectileType<MagmaArrowHostile>(), 30, 0);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Helper.TRay.Cast(new Vector2(player.Center.X, (Main.maxTilesY * 16) / 2), -Vector2.UnitY, 1200) - new Vector2(0, 100), new Vector2(player.velocity.X, Main.rand.NextFloat(5, 10)), ModContent.ProjectileType<MagmaArrowHostile>(), 30, 0);
                        }
                        if (AITimer > 155 && AITimer < 250 && AITimer % 5 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), Helper.TRay.Cast(new Vector2(player.Center.X + 920 * Main.rand.NextFloat(-1, 1f), (Main.maxTilesY * 16) / 2), -Vector2.UnitY, 1200) - new Vector2(0, 100), new Vector2(player.velocity.X, Main.rand.NextFloat(5, 10)), ModContent.ProjectileType<MagmaArrowHostile>(), 30, 0);
                        }
                        if (AITimer >= 285)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = SoulStorm;
                        }

                    }
                    break;
                case SoulStorm:
                    {
                        if (AITimer == 1)
                        {
                            EbonianSystem.ScreenShakeAmount = 3;
                        }
                        if (AITimer > 30 && AITimer < 90 && AITimer % 5 == 0)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X + 1080, (Main.maxTilesY * Main.rand.NextFloat(4, 28)) / 2), -Vector2.UnitX.RotatedByRandom(MathHelper.PiOver4 / 2) * 15, ModContent.ProjectileType<TinyRykardSkull>(), 30, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X - 1080, (Main.maxTilesY * Main.rand.NextFloat(4, 28)) / 2), Vector2.UnitX.RotatedByRandom(MathHelper.PiOver4 / 2) * 15, ModContent.ProjectileType<TinyRykardSkull>(), 30, 0);
                        }
                        if (AITimer >= 170)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = BigSwordCombo;
                        }
                    }
                    break;
                case BigSwordCombo:
                    {
                        NPC.damage = 50;
                        if ((AITimer % 30 == 0 && AITimer <= 120) || AITimer == 1 || AITimer == 200)
                        {
                            PerformBasicSwordSlash(30);
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 15;
                        }
                        if (AITimer >= 120 && AITimer < 200)
                        {
                            AITimer2++;
                            if (AITimer2 < 35f)
                            {
                                swordRotationFocus = MathHelper.Lerp(swordRotationFocus, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver4 * 0.5f, 0.25f);
                                swordRotation = MathHelper.Lerp(swordRotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.PiOver4 * 0.5f, 0.25f);
                            }
                            if (AITimer2 >= 40 && AITimer2 <= 50)
                            {
                                NPC.damage = 50;
                                trailAlpha = Math.Min(trailAlpha + 0.1f, 1);
                                NPC.velocity += Vector2.UnitX.RotatedBy(swordRotation - MathHelper.PiOver4 * 0.5f) * 3.5f;
                            }
                            else NPC.velocity *= 0.95f;
                        }
                        if (AITimer2 > 70)
                        {
                            trailAlpha = Math.Max(trailAlpha - 0.1f, 0);
                        }
                        if (AITimer >= 250)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = GaelDiscus;
                        }
                    }
                    break;
                case GaelDiscus:
                    {
                        IdleMovement(new Vector2(0, -100));
                        if (AITimer == 30)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosion>(), 30, 0);
                        if (AITimer == 50)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitX.RotatedBy(angle) * 2, ModContent.ProjectileType<ESickle>(), 30, 0, ai1: angle);
                            }
                        }
                        if (AITimer >= 270)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = LavaApocalypse;
                        }
                    }
                    break;
                case LavaApocalypse:
                    {
                        if (AITimer % 5 == 0 && AITimer <= 50)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X - 1000, (Main.maxTilesY * 16) / 2 + Main.rand.NextFloat(-175, 175)), Vector2.UnitX * (AITimer), ModContent.ProjectileType<EPlatform>(), 30, 0);
                        }
                        if (AITimer == 75)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EGroundHazard>(), 0, 0);
                        if (AITimer > 80 && AITimer < 360)
                        {
                            if (AITimer % 20 == 0)
                            {
                                Projectile.NewProjectileDirect(NPC.InheritSource(NPC), Helper.TRay.Cast(NPC.Center + new Vector2(920 * Main.rand.NextFloat(-1, 1), -20), Vector2.UnitY, 1000) + Vector2.UnitY * 50, new Vector2(Main.rand.NextFloat(-1, 1), -Main.rand.NextFloat(5, 15)), ModContent.ProjectileType<ExolFireExplode>(), 30, 1);
                            }
                        }
                        if (AITimer >= 470)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = SwordWaveSpam;
                        }
                    }
                    break;
                case SwordWaveSpam:
                    {
                        IdleMovement(new Vector2(0, -100));
                        if (AITimer % 30 == 0 && AITimer <= 180)
                        {
                            PerformBasicSwordSlash();
                            if (AITimer % 90 == 0)
                                for (int i = -2; i < 3; i++)
                                {
                                    if (i == 0) continue;
                                    Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedBy(i * Main.rand.NextFloat());
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 30, vel * 15, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                                }
                            else if (AITimer % 60 == 0)
                                for (int i = -1; i < 2; i++)
                                {
                                    if (i != -1 && i != 1) continue;
                                    Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedBy(i * Main.rand.NextFloat());
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 30, vel * 15, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                                }
                            else
                            {
                                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vel * 30, vel * 15, ModContent.ProjectileType<ESwordWave>(), 30, 0);
                            }
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            AIState = Idle;
                            NextAttack = SwordSlashes;
                        }
                    }
                    break;
            }
        }
        void PerformBasicSwordSlash(int time = 40)
        {
            Player player = Main.player[NPC.target];
            swordRotationFocus = Helper.FromAToB(NPC.Center, player.Center).ToRotation();
            swordDirection = -swordDirection;
            swingTime = time;
            maxSwingTime = time;
            turningSword = true;
        }
        float rot;
        float vortexAlpha;
        float bowAlpha, bowRotation, bowString;
        float[] arrowAlpha = new float[5];
        float trailAlpha;
        bool turningSword;
        float swordAlpha = 1f, swordRotation, swordRotationFocus, swordDirection = 1, swingTime = 40, maxSwingTime = 40, swordPositionOffset = 40;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D vortex = Helper.GetExtraTexture("vortex3");
            spriteBatch.Reload(BlendState.Additive);
            rot -= 0.025f;
            Vector2 scale = new Vector2(1f, 0.25f);
            spriteBatch.Reload(EbonianMod.SpriteRotation);
            EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-rot);
            EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
            Vector4 col = (Color.OrangeRed * vortexAlpha).ToVector4();
            col.W = vortexAlpha;
            EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
            spriteBatch.Draw(vortex, NPC.Center - new Vector2(0, 100) - Main.screenPosition, null, Color.White, 0, vortex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(vortex, NPC.Center - new Vector2(0, 100) - Main.screenPosition, null, Color.White, 0, vortex.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Reload(effect: null);
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture("Items/Weapons/Ranged/MagmaBowP");
            float off = MathHelper.Lerp(2, 0, bowString / 30);
            if (NPC.ai[2] > 30)
                off = 0;
            Utils.DrawLine(Main.spriteBatch, NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) - new Vector2(8, 20).RotatedBy(bowRotation), NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) - new Vector2(8 + bowString, -off).RotatedBy(bowRotation), new Color(162, 44, 31) * bowAlpha, new Color(162, 44, 31) * bowAlpha, 2);
            Utils.DrawLine(Main.spriteBatch, NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) - new Vector2(10, -20).RotatedBy(bowRotation), NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) - new Vector2(8 + off + bowString, off).RotatedBy(bowRotation), new Color(162, 44, 31) * bowAlpha, new Color(162, 44, 31) * bowAlpha, 2);
            spriteBatch.Draw(tex, NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) - Main.screenPosition, null, Color.White * bowAlpha, bowRotation, tex.Size() / 2, 1, SpriteEffects.None, 0);

            Texture2D tex2 = Helper.GetTexture("Projectiles/Friendly/Underworld/MagmaArrow");
            for (int i = -2; i < 3; i++)
            {
                if (i == 0)
                    continue;
                Main.spriteBatch.Draw(tex2, NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) + Vector2.Lerp(new Vector2(20, 0).RotatedBy(bowRotation), Vector2.Zero, arrowAlpha[i + 2]) - Main.screenPosition, null, Color.White * arrowAlpha[i + 2], bowRotation + (i * 0.25f) - MathHelper.PiOver2, tex2.Size() / 2, 1, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex2, NPC.Center + new Vector2(25, 0).RotatedBy(bowRotation) + Vector2.Lerp(new Vector2(20, 0).RotatedBy(bowRotation), Vector2.Zero, arrowAlpha[2]) - Main.screenPosition, null, Color.White * arrowAlpha[2], bowRotation - MathHelper.PiOver2, tex2.Size() / 2, 1, SpriteEffects.None, 0);

            if (trailAlpha > 0f)
            {
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[Type];
                Main.spriteBatch.Reload(BlendState.Additive);
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    float mult = (1f - fadeMult * i);
                    if (i > 0)
                        for (float j = 0; j < 10; j++)
                        {
                            Vector2 pos = Vector2.Lerp(NPC.oldPos[i], NPC.oldPos[i - 1], (float)(j / 10));
                            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + NPC.Size / 2 + new Vector2(swordPositionOffset, 0).RotatedBy(swordRotation) - Main.screenPosition, null, Color.OrangeRed * (0.35f * trailAlpha), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.025f * mult, SpriteEffects.None, 0);
                            Main.spriteBatch.Draw(TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value, pos + NPC.Size / 2 + new Vector2(swordPositionOffset, 0).RotatedBy(swordRotation) - Main.screenPosition, null, Color.OrangeRed * (0.25f * trailAlpha), 0, TextureAssets.Projectile[ModContent.ProjectileType<Gibs>()].Value.Size() / 2, 0.1f * mult, SpriteEffects.None, 0);
                        }
                }
            }

            Texture2D tex3 = Helper.GetTexture("Items/Weapons/Melee/IgnosSword");
            spriteBatch.Draw(tex3, NPC.Center + new Vector2(swordPositionOffset, 0).RotatedBy(swordRotation) - Main.screenPosition, null, Color.White * swordAlpha, swordRotation + (swordDirection == -1 ? 0 : MathHelper.PiOver2 * 3), tex3.Size() / 2, 1, (swordDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);

            if (turningSword)
            {
                float mult = Ease(Utils.GetLerpValue(0f, maxSwingTime, swingTime));
                Texture2D slash = Helper.GetExtraTexture("Extras2/slash_02");
                float alpha = (float)Math.Sin(mult * Math.PI);
                Vector2 pos = NPC.Center + new Vector2(swordPositionOffset, 0).RotatedBy(swordRotation);
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.OrangeRed * alpha * 0.5f, swordRotationFocus - MathHelper.PiOver2, slash.Size() / 2, 1 / 3f, SpriteEffects.None, 0f);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        void Reset(bool idleReset = false)
        {
            int maxIdles = 1;
            if (NPC.life < NPC.lifeMax / 4)
                maxIdles = 2;
            else if (NPC.life < NPC.lifeMax / 2)
                maxIdles = 3;
            else if (NPC.life < NPC.lifeMax - NPC.lifeMax / 4)
                maxIdles = 2;
            if (!idleReset)
                IdleAttacks = maxIdles;
            NPC.noTileCollide = true;
            NPC.rotation = 0;
            IdleState = Main.rand.Next(10);
            //IdleState++;
            //if (IdleState > SwordThrustDash)
            //  IdleState = 0;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            trailAlpha = 0;
            bowAlpha = 0;
            swordAlpha = 1;
            vortexAlpha = 0;
            for (int i = 0; i < arrowAlpha.Length; i++)
            {
                arrowAlpha[i] = 0;
            }
            AITimer = 0;
            AITimer2 = 0;
            turningSword = false;
            lastPos = Vector2.Zero;
            NPC.damage = 0;
        }
        void IdleMovement(Vector2 offset = default, float speedLimit = 10)
        {
            Player player = Main.player[NPC.target];
            //NPC.velocity *= 0.975f;
            NPC.velocity = Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 150) + offset, false) * (speedLimit * 0.01f), -Vector2.One * speedLimit, Vector2.One * speedLimit);
        }
        void PlayerDetection()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.dead || !player.active || !player.ZoneUnderworldHeight)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Spawn;
                    AITimer = 0;
                }
                if (player.dead || !player.active || !player.ZoneUnderworldHeight)
                {
                    NPC.velocity.Y = 30;
                    NPC.timeLeft = 10;
                    NPC.active = false;
                }
                return;
            }
        }
    }
}
