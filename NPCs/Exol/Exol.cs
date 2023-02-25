using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles;
using EbonianMod;
using EbonianMod.Projectiles.ExolOld;

namespace EbonianMod.NPCs.Exol
{
    [AutoloadBossHead]
    public class Exol : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Demonic Construct"),
                new FlavorTextBestiaryInfoElement("A boiling, blazing core sealed within a cold shell of stone. Its purpose and origin are unclear and most are content not knowing."),
            });
        }
        private static int hellLayer => Main.maxTilesY - 200;
        public float angle = 0;
        public float multiplier = 1f;
        private bool multiplierGoBack = false;
        private bool beamAttack;
        private Projectile laser;
        public bool Minus;
        private int beamTime = 300;
        private int numberProjectiles = 3;
        private int realllll = 0;

        public static readonly int arenaWidth = (int)(0.85f * NPC.sWidth);
        public static Vector2 centerOfExol;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exol");
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override string Texture => "EbonianMod/NPCs/Exol/Exol";

        public override bool CheckDead()
        {
            EbonianSystem.ScreenShakeAmount = 30f;
            return true;
        }
        public int damage = ((Main.expertMode) ? 20 : 40);
        public bool goBack;
        SoundStyle DashSound = new SoundStyle("EbonianMod/Sounds/ExolDash");
        SoundStyle RoarSound = new SoundStyle("EbonianMod/Sounds/ExolRoar");
        private int num103 = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(ModContent.Request<Texture2D>("EbonianMod/NPCs/Exol/Exol").Value.Width * 0.5f, NPC.height * 0.5f);
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - pos + drawOrigin + new Vector2(0, NPC.gfxOffY);
                spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Exol/ExolBoss_Pulse").Value, drawPos, NPC.frame, Color.White * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Exol/ExolBoss_Pulse").Value, NPC.Center - pos, NPC.frame, Color.White, NPC.rotation, drawOrigin, NPC.scale * multiplier, SpriteEffects.None, 0);
            spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Exol/Exol").Value, NPC.Center - pos, NPC.frame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("NPCs/InfernoMagePortal").Value;
            Texture2D textureStun = Mod.Assets.Request<Texture2D>("Buffs/Stunned").Value;
            for (int i = 0; i < 5; i++)
            {
                float angle = 2f * (float)Math.PI / 5f * i;
                Vector2 poss = NPC.Center + 120f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                if (AIState == CirclesAroundExol)
                    spriteBatch.Draw(texture, poss - pos, null, Color.White, Main.GameUpdateCount * 0.314f, new Vector2(16, 16), Vector2.One, SpriteEffects.None, 0);
            }
            if (NPC.HasBuff(ModContent.BuffType<Buffs.ExolStun>()))
            {
                spriteBatch.Draw(textureStun, new Vector2(NPC.Center.X, NPC.Center.Y - 90) - pos, null, Color.White, 0, new Vector2(16, 16), Vector2.One, SpriteEffects.None, 0);
            }
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 27500;
            NPC.damage = 0;
            NPC.defense = 48;
            NPC.knockBackResist = 0;
            NPC.width = 156;
            NPC.height = 150;
            NPC.npcSlots = 1f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = new SoundStyle("EbonianMod/Sounds/NPCHit/EDead");
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Exol");
        }
        private const int FrameIdle = 0;
        private const int FrameIdle2 = 1;
        private const int FrameIdle3 = 2;
        private const int FrameIdle4 = 3;
        private const int FrameIdle5 = 4;
        private const int FrameIdle6 = 5;
        private const int FrameIdle7 = 6;
        private const int FrameIdle8 = 7;
        private const int FrameAttack = 8;
        private const int FrameAttack2 = 9;
        private const int FrameDash = 10;
        private const int FrameDash2 = 11;
        public Vector2 ech;
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * ((255 - NPC.alpha) / 255f);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 3)
            {
                NPC.frame.Y = FrameIdle * frameHeight;
            }
            else if (NPC.frameCounter < 6)
            {
                NPC.frame.Y = FrameIdle2 * frameHeight;
            }
            else if (NPC.frameCounter < 12)
            {
                NPC.frame.Y = FrameIdle3 * frameHeight;
            }
            else if (NPC.frameCounter < 18)
            {
                NPC.frame.Y = FrameIdle4 * frameHeight;
            }
            else if (NPC.frameCounter < 24)
            {
                NPC.frame.Y = FrameIdle5 * frameHeight;
            }
            else if (NPC.frameCounter < 32)
            {
                NPC.frame.Y = FrameIdle6 * frameHeight;
            }
            else if (NPC.frameCounter < 38)
            {
                NPC.frame.Y = FrameIdle7 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
        public float increaser = 0;
        public static void DoAFunny()
        {
            CombatText.NewText(new Rectangle((int)Exol.projectileSpawnPoint.X, (int)Exol.projectileSpawnPoint.Y, 156, 156), Color.Red, "The hell is up with that guy");
            Main.NewText("<Exol> The hell is up with that guy", Color.Red);
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (AIState == HugeCharge || AIState == CardinalDirectionDash)
                NPC.AddBuff(ModContent.BuffType<Buffs.ExolStun>(), 90, true);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

        private const int AISlot = 0;
        private const int TimerSlot = 1;
        private const int TimerSlot2 = 2;
        private const int TimerSlot3 = 3;

        public float AIState
        {
            get => NPC.ai[AISlot];
            set => NPC.ai[AISlot] = value;
        }

        public float AITimer
        {
            get => NPC.ai[TimerSlot];
            set => NPC.ai[TimerSlot] = value;
        }

        public float AITimer2
        {
            get => NPC.ai[TimerSlot2];
            set => NPC.ai[TimerSlot2] = value;
        }
        public float AITimer3
        {
            get => NPC.ai[TimerSlot2];
            set => NPC.ai[TimerSlot2] = value;
        }
        public const int Intro = 0;
        private const int Spew = 3;
        private const int Rings = 4;
        private const int SethAttackRipoff = 5;
        private const int HugeCharge = 6;
        private const int CirclePlayer = 7;
        private const int RocksFallingAttack = 8;
        private const int CardinalDirectionDash = 9;
        private const int CirclesAroundExol = 10;
        private const int HomingFlameAttack = 11;
        private const int DemonScytheWall = 12;


        public float lastRotation;
        public Vector2 lastPos;
        public static Vector2 projectileSpawnPoint;
        public float rotation = 0;
        public float rotation2 = 0;
        public float rotation3 = 0;
        public int Deathtimer = 0;
        public bool HaveAlreadyDoneRedLasers = false;
        public override void AI()
        {
            if (!multiplierGoBack)
            {
                multiplier += 0.005f;
            }
            else
            {
                multiplier -= 0.005f;
            }
            if (multiplier >= 1.15f)
            {
                multiplierGoBack = true;
            }
            if (multiplier <= 0.9f)
            {
                multiplierGoBack = false;
            }
            projectileSpawnPoint = new Vector2(NPC.Center.X + 10, NPC.Center.Y + 2);
            int buffIndex = NPC.FindBuffIndex(ModContent.BuffType<Buffs.ExolStun>());
            if (AIState != HugeCharge && AIState != CardinalDirectionDash)
            {
                if (buffIndex != -1)
                {
                    NPC.DelBuff(buffIndex);
                }
            }
            centerOfExol = NPC.Center;
            {
                Lighting.AddLight(NPC.position, 0.25f, 0, 0.5f);
            }
            Player player = Main.player[NPC.target];
            if (Main.player[NPC.target].Distance(NPC.Center) > arenaWidth * 2.5f)
            {
                player.AddBuff(163, 2, true);
            }
            else
            {
                Deathtimer = 180;
            }
            if ((player.position - NPC.Center).LengthSquared() < 74 * 74)
            {
                if (AIState == CardinalDirectionDash || AIState == HugeCharge)
                    player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " got destroyed!"), 55, 0);
            }

            if (!player.active || player.dead || player.position.Y < hellLayer * 16)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead || player.position.Y < hellLayer * 16)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    NPC.timeLeft = 1;
                    return;
                }
            }
            if (AIState == Intro)
            {
                AITimer++;
                NPC.dontTakeDamage = true;
                NPC.damage = 0;
                rotation += MathHelper.ToRadians(46);
                if (AITimer == 1)
                {
                    NPC.Center = new Vector2(player.position.X, player.position.Y - 200);
                }
                if (AITimer == 10)
                {
                    Helper.SetBossTitle(90, "Exol", Color.OrangeRed);
                    EbonianSystem.ChangeCameraPos(projectileSpawnPoint, 90);
                }
                if (AITimer >= 10 && AITimer <= 60)
                {
                    Dust dust = Dust.NewDustDirect(player.Center, 0, 0, ModContent.DustType<Dusts.ExolIntro>(), Scale: 2);
                    dust.rotation = rotation;
                    dust.noLight = false;
                    dust.position = NPC.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 100;
                    dust.fadeIn = 1f;
                }
                else if (AITimer == 85)
                {
                    EbonianSystem.ScreenShakeAmount = 10f;
                    ArenaSetup(1, true);
                }
                if (AITimer >= 170)
                {
                    NPC.dontTakeDamage = false;
                    AIState = Spew;
                    rotation = 0;

                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == Spew)
            {
                AITimer++;
                NPC.damage = 0;
                if (AITimer >= 30)
                {
                    rotation += Main.rand.Next(100, 360);
                    Vector2 velocity = new Vector2(1.5f, 1.5f).RotatedBy(MathHelper.ToRadians(rotation));

                    if (AITimer == 40)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                        NPC.Center = new Vector2(player.position.X, player.position.Y - 150);
                    }
                    if (AITimer == 41)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);

                    }
                    else if (AITimer >= 41)
                    {
                        Vector2 pos = new Vector2(player.position.X, player.position.Y - 155);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.18f;

                        if (velocity.Length() < 3 && AIState == Spew) velocity = Vector2.Normalize(velocity) * 3f;
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, velocity, ModContent.ProjectileType<ExolFire>(), damage, 0, 0, 1);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, velocity, ModContent.ProjectileType<ExolFire>(), damage, 0, 0, 1);
                            if (AITimer == 100)
                            {
                                NPC.velocity.X = 0;
                                NPC.velocity.Y = 0;

                                if (!HaveAlreadyDoneRedLasers)
                                {
                                    AIState = Rings;
                                }

                                else
                                {
                                    switch (Main.rand.Next(1, 11))
                                    {
                                        case 1:
                                            AIState = Rings;
                                            break;
                                        case 2:
                                            AIState = Rings;
                                            break;
                                        case 3:
                                            AIState = SethAttackRipoff;
                                            break;
                                        case 4:
                                            AIState = HugeCharge;
                                            break;
                                        case 5:
                                            AIState = CirclePlayer;
                                            break;
                                        case 6:
                                            AIState = RocksFallingAttack;
                                            break;
                                        case 7:
                                            AIState = CardinalDirectionDash;
                                            break;
                                        case 8:
                                            AIState = CirclesAroundExol;
                                            break;
                                        case 9:
                                            AIState = HomingFlameAttack;
                                            break;
                                        case 10:
                                            AIState = DemonScytheWall;
                                            break;
                                    }
                                }
                                AITimer = 0;
                                AITimer2 = 0;
                            }
                        }
                    }

                }
            }


            else if (AIState == Rings)
            {
                NPC.damage = 0;
                AITimer++;
                if (AITimer == 1)
                {
                    FlameAttack2(arenaWidth / 12, false);
                }
                if (AITimer <= 180)
                {
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 280);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.18f;
                }
                if (AITimer >= 40)
                {

                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                }
                if (AITimer >= 210)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;


                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = SethAttackRipoff;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = SethAttackRipoff;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                }
            }
            else if (AIState == SethAttackRipoff)
            {
                NPC.damage = 0;
                AITimer++;
                AITimer2++;
                if (AITimer == 1)
                {
                    EbonianSystem.ScreenShakeAmount = 20f;
                    SoundEngine.PlaySound(RoarSound);
                }
                if (AITimer2 <= 70 && AITimer2 >= 20)
                {
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 335);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.18f;
                }
                if (AITimer2 == 15)
                {
                    ech = player.Center;
                    NPC.velocity = Vector2.Zero;
                    Vector2 vector16 = NPC.DirectionTo(player.Center) * 7f;
                    for (int i = (numberProjectiles - numberProjectiles - numberProjectiles); i <= numberProjectiles; i++)
                    {
                        Projectile projectile2 = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, 7.5f * Utils.RotatedBy(NPC.DirectionTo(ech), (double)(MathHelper.ToRadians(16f) * (float)i), default(Vector2)), ModContent.ProjectileType<TelegraphLine>(), 0, 1f, Main.myPlayer)];
                        projectile2.timeLeft = 20;
                    }
                }
                if (AITimer2 == 35)
                {
                    NPC.velocity = Vector2.Zero;
                    Vector2 vector16 = NPC.DirectionTo(player.Center) * 7f;
                    for (int i = (numberProjectiles - numberProjectiles - numberProjectiles); i <= numberProjectiles; i++)
                    {

                        Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, 9.5f * Utils.RotatedBy(NPC.DirectionTo(ech), (double)(MathHelper.ToRadians(16f) * (float)i), default(Vector2)), ProjectileID.EyeBeam, damage, 1f, Main.myPlayer)];
                        projectile.tileCollide = false;
                        projectile.friendly = false;
                        projectile.hostile = true;
                        projectile.timeLeft = 230;
                    }
                }
                if (AITimer2 >= 35)
                {
                    numberProjectiles += 2;
                    AITimer2 = 0;
                }

                if (AITimer >= 210)
                {
                    NPC.rotation = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    numberProjectiles = 3;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = CirclePlayer;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = HugeCharge;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                }
            }
            else if (AIState == CirclePlayer)
            {
                NPC.damage = 0;
                AITimer++;
                AITimer2++;
                if (AITimer <= 250)
                {
                    angle += 0.01f + increaser;
                    increaser += 0.0002f;
                    if (AITimer2 >= 9)
                    {
                        float Speed3 = 1f;
                        Vector2 vector1 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                        int type3 = ModContent.ProjectileType<ExolSickle>();
                        float rotation3 = (float)Math.Atan2(vector1.Y - (player.position.Y + (player.height * 0.5f)), vector1.X - (player.position.X + (player.width * 0.5f)));
                        int Proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), type3, damage, 0, 0);
                        int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                        Main.projectile[Proj].tileCollide = false;
                        Main.projectile[Proj2].timeLeft = 30;
                        Main.projectile[Proj].timeLeft = 230;
                        AITimer2 = 0;
                    }
                    NPC.Center = player.Center + Vector2.One.RotatedBy(angle) * 350;
                }
                else
                {
                    EbonianSystem.ScreenShakeAmount = 2f;
                }
                if (AITimer >= 280)
                {
                    AITimer = 0;
                    increaser = 0;
                    AITimer2 = 0;
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    angle = 0;
                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = RocksFallingAttack;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = RocksFallingAttack;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                }
            }
            else if (AIState == RocksFallingAttack)
            {
                NPC.damage = 0;
                AITimer++;
                AITimer2++;
                if (AITimer == 1)
                {
                    beamAttack = false;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                    NPC.Center = new Vector2(player.Center.X, player.Center.Y - 230);
                }
                else if (AITimer == 42)
                {
                    SoundEngine.PlaySound(RoarSound);
                    EbonianSystem.ScreenShakeAmount = 25f;
                }
                else if (AITimer == 62)
                {
                    NPC.velocity = Vector2.Zero;
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                    NPC.Center = new Vector2(player.Center.X, player.Center.Y - 230);
                }
                else if (AITimer >= 62)
                {
                    if (AITimer2 >= 5)
                    {
                        Vector2 rainPos = new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y);
                        int projInt = Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, 0), ModContent.ProjectileType<ExolRockFall>(), damage, 0, 0, 1);
                        int projInt2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), Main.projectile[projInt].Center + new Vector2(0, -40), new Vector2(0, 1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0, 1);
                        Main.projectile[projInt].tileCollide = false;
                        Main.projectile[projInt].friendly = false;
                        Main.projectile[projInt].hostile = true;
                        Main.projectile[projInt2].timeLeft = 20;
                        AITimer2 = 0;
                    }

                }

                if (AITimer >= 210)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = CirclesAroundExol;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = CardinalDirectionDash;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == CirclesAroundExol)
            {
                NPC.damage = 0;
                AITimer++;
                if (AITimer == 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                }
                else if (AITimer == 2)
                {
                    NPC.Center = new Vector2(player.Center.X, player.Center.Y - 300);
                }
                else if (AITimer == 3)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                }
                if (AITimer >= 30)
                {
                    if (++AITimer2 >= 30)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = 2f * (float)Math.PI / 5f * i;
                            Vector2 pos = NPC.Center + 120f * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                            int type3 = ModContent.ProjectileType<DustSpawnerExol>();
                            Vector2 velocity = Vector2.Normalize(pos - Main.player[NPC.target].position) * -20f;
                            int Proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velocity, type3, damage, 0, 0);
                            int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                            Main.projectile[Proj].tileCollide = false;
                            Main.projectile[Proj].friendly = false;
                            Main.projectile[Proj].hostile = true;
                            Main.projectile[Proj].timeLeft = 400;
                            Main.projectile[Proj2].timeLeft = 15;
                        }
                        AITimer2 = 0;
                    }
                }

                if (AITimer >= 155)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = HomingFlameAttack;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = Spew;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == HomingFlameAttack)
            {
                NPC.damage = 0;
                AITimer++;
                if (AITimer >= 60)
                {
                    AITimer2++;
                    if (!goBack)
                    {
                        increaser += 10;
                    }
                    else
                    {
                        increaser -= 10;
                    }
                    if (increaser >= 400)
                    {
                        goBack = true;
                    }
                    else if (increaser <= 0)
                    {
                        goBack = false;
                    }
                    if (AITimer2 <= 15)
                    {
                        Vector2 pos = new Vector2(player.position.X + 200 - increaser, player.position.Y - 335);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.18f;
                    }

                    if (AITimer2 >= 20)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            float angle = 2f * (float)Math.PI / 3f * k;
                            Vector2 velocity = new Vector2(10, 10).RotatedBy(angle);
                            int projInt = Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, velocity, ModContent.ProjectileType<HomingDust>(), 0, 0, 0, 1);
                            Main.projectile[projInt].tileCollide = false;
                        }
                        AITimer2 = 0;
                    }
                }
                if (AITimer >= 260)
                {
                    NPC.velocity.X = 0;
                    increaser = 0;
                    NPC.velocity.Y = 0;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = DemonScytheWall;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = CirclesAroundExol;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == DemonScytheWall)
            {
                NPC.damage = 0;
                AITimer++;
                if (AITimer == 1)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                }
                else if (AITimer == 2)
                {
                    NPC.Center = new Vector2(player.Center.X, player.Center.Y - 300);
                }
                else if (AITimer == 3)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                }

                int projAmount = 0;

                if (AITimer % 40 == 0)
                {
                    projAmount = 13 + (int)(AITimer / 40);
                }
                float num53 = 2200f;
                float num54 = num53 / (float)projAmount;

                for (int num56 = 0; num56 < projAmount; num56++)
                {
                    Vector2 vector17a = new Vector2(player.Center.X + (Main.screenWidth / 2), player.Center.Y - num53 / 2f + num54 * (float)num56);
                    Vector2 vector17 = new Vector2(player.Center.X + 1500f, player.Center.Y - num53 / 2f + num54 * (float)num56);
                    Projectile projectileDemon = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), vector17.X, vector17.Y, -12f, 0, ModContent.ProjectileType<ExolSickle>(), damage, 0, Main.myPlayer, 0, 0)];
                    Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), vector17a.X, vector17a.Y, -17f, 0, ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0, 0, 0)];
                    projectile.timeLeft = 25;
                    projectileDemon.tileCollide = false;
                    projectile.tileCollide = false;
                    projectileDemon.timeLeft = 130;
                }
                if (AITimer >= 300)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = CardinalDirectionDash;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CardinalDirectionDash;
                                break;
                            case 8:
                                AIState = Spew;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = CirclesAroundExol;
                                break;
                        }
                    }
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == CardinalDirectionDash)
            {
                //npc.damage = 35;
                AITimer++;
                AITimer2++;
                if (AITimer == 1)
                {
                    realllll = Main.rand.Next(2);
                    for (int k = 0; k < 4; k++)
                    {
                        float angle = 2f * (float)Math.PI / 4f * k;
                        Vector2 pos = player.Center + 350 * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos.X, pos.Y, 0, 0, ModContent.ProjectileType<ExolPortalTelegraph>(), 0, 0, Main.myPlayer, NPC.whoAmI, angle);
                        Main.projectile[proj].localAI[0] = realllll == 0 ? 5 : -5;
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                    }
                }
                if (AITimer2 == 30)
                {
                    int portal = -1;
                    List<int> portals = new List<int>();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        var projectilee = Main.projectile[i];
                        if (projectilee.type == ModContent.ProjectileType<ExolPortalTelegraph>() && projectilee.active)
                        {
                            portals.Add(i);
                        }
                    }
                    portal = portals[Main.rand.Next(portals.Count)];
                    if (portal != -1)
                    {
                        var proj = Main.projectile[portal];
                        NPC.Center = proj.Center;
                        proj.Kill();
                        if (ExolPortalTelegraph.dir == 5)
                        {
                            ExolPortalTelegraph.dir = -5;
                        }
                        else
                        {
                            ExolPortalTelegraph.dir = 5;
                        }
                        lastPos = player.Center;
                        float Speed3 = 1f;
                        Vector2 vector1 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                        float rotation3 = (float)Math.Atan2(vector1.Y - (player.position.Y + (player.height * 0.5f)), vector1.X - (player.position.X + (player.width * 0.5f)));
                        int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                        Main.projectile[Proj2].timeLeft = 25;
                    }
                }
                else if (AITimer2 == 55)
                {
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y *= 0.98f;
                    Vector2 vector9 = new Vector2(NPC.position.X + (NPC.width * 0.5f), NPC.position.Y + (NPC.height * 0.5f));
                    {
                        float rotation2 = (float)Math.Atan2((vector9.Y) - (lastPos.Y), (vector9.X) - (lastPos.X));
                        NPC.velocity.X = (float)(Math.Cos(rotation2) * 45) * -1;
                        NPC.velocity.Y = (float)(Math.Sin(rotation2) * 45) * -1;
                        SoundEngine.PlaySound(DashSound);
                    }
                }
                else if (AITimer2 == 90)
                {
                    if (buffIndex != -1)
                    {
                        NPC.DelBuff(buffIndex);
                    }
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;
                    AITimer2 = 0;
                }

                if (AITimer >= 370)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    if (!HaveAlreadyDoneRedLasers)
                    {
                        AIState = HugeCharge;
                    }

                    else
                    {
                        switch (Main.rand.Next(1, 11))
                        {
                            case 1:
                                AIState = Spew;
                                break;
                            case 2:
                                AIState = Rings;
                                break;
                            case 3:
                                AIState = SethAttackRipoff;
                                break;
                            case 4:
                                AIState = HugeCharge;
                                break;
                            case 5:
                                AIState = CirclePlayer;
                                break;
                            case 6:
                                AIState = RocksFallingAttack;
                                break;
                            case 7:
                                AIState = CirclesAroundExol;
                                break;
                            case 8:
                                AIState = CirclesAroundExol;
                                break;
                            case 9:
                                AIState = HomingFlameAttack;
                                break;
                            case 10:
                                AIState = DemonScytheWall;
                                break;
                        }
                    }
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == HugeCharge)
            {
                //npc.damage = 36;
                AITimer++;
                rotation += MathHelper.ToRadians(46);
                if (buffIndex == -1)
                {
                    if (AITimer == 1)
                    {
                        SoundEngine.PlaySound(new("EbonianMod/Sounds/ExolSummon"));
                        Minus = Main.rand.NextBool();
                    }
                    if (AITimer == 2)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                    }
                    if (AITimer <= 20 && AITimer >= 1)
                    {

                        Dust dust = Dust.NewDustDirect(player.Center, 0, 0, ModContent.DustType<Dusts.ExolIntro>(), Scale: 2);
                        dust.rotation = rotation;
                        dust.noLight = false;
                        dust.position = NPC.Center + Vector2.UnitX.RotatedBy(dust.rotation, Vector2.Zero) * dust.scale * 100;
                        dust.fadeIn = 1f;
                    }
                    if (AITimer == 35)
                    {
                        ArenaSetup(arenaWidth / 20, true);
                        EbonianSystem.ScreenShakeAmount = 20f;
                    }
                    if (AITimer <= 25)
                    {
                        lastPos = player.Center;
                        if (Minus)
                        {
                            NPC.Center = new Vector2(player.position.X - 600, player.Center.Y);
                        }
                        else if (!Minus)
                        {
                            NPC.Center = new Vector2(player.position.X + 600, player.Center.Y);
                        }
                    }
                    if (AITimer == 25)
                    {
                        float Speed3 = 1f;
                        Vector2 vector1 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                        float rotation3 = (float)Math.Atan2(vector1.Y - (player.position.Y + (player.height * 0.5f)), vector1.X - (player.position.X + (player.width * 0.5f)));
                        int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                        Main.projectile[Proj2].timeLeft = 10;
                    }
                    if (AITimer == 35)
                    {
                        LookAtPlayer();
                        SoundEngine.PlaySound(DashSound);
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y *= 0.98f;
                        Vector2 vector9 = NPC.Center;
                        {
                            float rotation2 = (float)Math.Atan2((vector9.Y) - (lastPos.Y), (vector9.X) - (lastPos.X));
                            NPC.velocity.X = (float)(Math.Cos(rotation2) * 36) * -1;
                            NPC.velocity.Y = (float)(Math.Sin(rotation2) * 36) * -1;
                        }
                        Vector2 offset = new Vector2((float)Math.Cos(NPC.ai[0]), (float)Math.Sin(NPC.ai[0]));





                    }
                    if (AITimer == 80)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 0;
                        AITimer2 = 0;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                    }
                    if (AITimer == 90)
                    {
                        lastPos = player.Center;
                        float Speed3 = 1f;
                        Vector2 vector1 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                        float rotation3 = (float)Math.Atan2(vector1.Y - (player.position.Y + (player.height * 0.5f)), vector1.X - (player.position.X + (player.width * 0.5f)));
                        int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                        Main.projectile[Proj2].timeLeft = 10;
                    }
                    if (AITimer == 100)
                    {
                        LookAtPlayer();
                        EbonianSystem.ScreenShakeAmount = 20f;
                        SoundEngine.PlaySound(DashSound);
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y *= 0.98f;
                        Vector2 vector9 = NPC.Center;
                        {
                            float rotation2 = (float)Math.Atan2((vector9.Y) - (lastPos.Y), (vector9.X) - (lastPos.X));
                            NPC.velocity.X = (float)(Math.Cos(rotation2) * 36) * -1;
                            NPC.velocity.Y = (float)(Math.Sin(rotation2) * 36) * -1;
                        }
                        Vector2 offset = new Vector2((float)Math.Cos(NPC.ai[0]), (float)Math.Sin(NPC.ai[0]));
                    }
                    if (AITimer == 154)
                    {
                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 0;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnPoint, Vector2.Zero, ModContent.ProjectileType<ExolPortal>(), 0, 0, 0, 0);
                    }
                    if (AITimer == 155)
                    {
                        lastPos = player.Center;
                        float Speed3 = 1f;
                        Vector2 vector1 = new Vector2(NPC.position.X + (NPC.width / 2), NPC.position.Y + (NPC.height / 2));
                        float rotation3 = (float)Math.Atan2(vector1.Y - (player.position.Y + (player.height * 0.5f)), vector1.X - (player.position.X + (player.width * 0.5f)));
                        int Proj2 = Projectile.NewProjectile(NPC.GetSource_FromAI(), vector1.X, vector1.Y, (float)((Math.Cos(rotation3) * Speed3) * -1), (float)((Math.Sin(rotation3) * Speed3) * -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0, 0);
                        Main.projectile[Proj2].timeLeft = 11;
                    }
                    if (AITimer == 165)
                    {
                        LookAtPlayer();
                        EbonianSystem.ScreenShakeAmount = 20f;
                        SoundEngine.PlaySound(DashSound);
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y *= 0.98f;
                        Vector2 vector9 = NPC.Center;
                        {
                            float rotation2 = (float)Math.Atan2((vector9.Y) - (lastPos.Y), (vector9.X) - (lastPos.X));
                            NPC.velocity.X = (float)(Math.Cos(rotation2) * 36) * -1;
                            NPC.velocity.Y = (float)(Math.Sin(rotation2) * 36) * -1;
                        }
                        Vector2 offset = new Vector2((float)Math.Cos(NPC.ai[0]), (float)Math.Sin(NPC.ai[0]));
                    }
                }
                if (AITimer >= 215)
                {
                    NPC.velocity.X = 0;
                    NPC.velocity.Y = 0;

                    switch (Main.rand.Next(1, 11))
                    {
                        case 1:
                            AIState = Spew;
                            break;
                        case 2:
                            AIState = Rings;
                            break;
                        case 3:
                            AIState = SethAttackRipoff;
                            break;
                        case 4:
                            AIState = CirclePlayer;
                            break;
                        case 5:
                            AIState = CirclePlayer;
                            break;
                        case 6:
                            AIState = RocksFallingAttack;
                            break;
                        case 7:
                            AIState = CardinalDirectionDash;
                            break;
                        case 8:
                            AIState = CirclesAroundExol;
                            break;
                        case 9:
                            AIState = HomingFlameAttack;
                            break;
                        case 10:
                            AIState = DemonScytheWall;
                            break;
                    }
                    HaveAlreadyDoneRedLasers = true;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
        }
        private void FlameAttack(int radius, bool clockwise)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Vector2 center = NPC.Center;
            for (int k = 0; k < 10; k++)
            {
                float angle = 2f * (float)Math.PI / 10f * k;
                Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos.X, pos.Y, 0, 0, ModContent.ProjectileType<ExolSmallFlame>(), damage, 0, Main.myPlayer, NPC.whoAmI, angle);
                Main.projectile[proj].localAI[0] = radius;
                Main.projectile[proj].localAI[1] = clockwise ? 1 : -1;
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
            }
        }
        private void ArenaSetup(int radius, bool clockwise)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Vector2 center = projectileSpawnPoint;
            for (int k = 0; k < 35; k++)
            {
                float angle = 2f * (float)Math.PI / 35f * k;
                Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 velocity = new Vector2(15, 15).RotatedBy(angle);
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<DustSpawnerExol>(), 0, 0, Main.myPlayer, NPC.whoAmI, angle);
            }
        }
        private void FlameAttack2(int radius, bool clockwise)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }
            Vector2 center = NPC.Center;
            for (int k = 0; k < 3; k++)
            {
                float angle = 2f * (float)Math.PI / 3f * k;
                Vector2 pos = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), pos.X, pos.Y, 0, 0, ModContent.ProjectileType<ExolRing1>(), 0, 0, Main.myPlayer, NPC.whoAmI, angle);
                Main.projectile[proj].localAI[0] = radius;
                Main.projectile[proj].localAI[1] = 45 * (k + 1);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / NPC.lifeMax * 100.0; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 5, hitDirection, -1f, 0, default(Color), 1f);
                if (Main.netMode != NetmodeID.MultiplayerClient && NPC.life <= 0)
                {
                }
            }
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (Main.expertMode || Main.rand.NextBool())
            {
                player.AddBuff(BuffID.OnFire, 600, true);
            }
        }
        private void LookAtPlayer()
        {
            Vector2 look = lastPos - NPC.Center;
            LookInDirection(look);
        }
        public float anglee = 0.5f * (float)Math.PI;
        private void LookInDirection(Vector2 look)
        {
            if (look.X != 0)
            {
                anglee = (float)Math.Atan(look.Y / look.X);
            }
            else if (look.Y < 0)
            {
                anglee += (float)Math.PI;
            }
            if (look.X < 0)
            {
                anglee += (float)Math.PI;
            }
        }
    }
}








