using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using ReLogic.Graphics;
using Terraria.GameContent;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.Exol;
using Terraria.Audio;
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Garbage;
using static tModPorter.ProgressUpdate;
using System.Collections.Generic;
using Terraria.UI;
using EbonianMod.Common.Achievements;
using EbonianMod.Common.Systems;
using System.Security.Cryptography.X509Certificates;

namespace EbonianMod.NPCs.Garbage
{
    [AutoloadBossHead]
    public class HotGarbage : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 13;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        public override void SetDefaults()
        {
            NPC.width = 80;
            NPC.height = 74;
            NPC.damage = 0;
            NPC.defense = 5;
            NPC.lifeMax = 4500;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = EbonianSounds.garbageDeath;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Garbage");
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: Dumpster"),
                new FlavorTextBestiaryInfoElement("Hot garbage is the magnum opus of Dr Dumbarton Gumtree, Renowned Garbagolist and homeless man. Using his ingenious skills acquired from the Stupidoodoo University of California, he created a machine purely out of scraps and shady government funding. After only a fortnight of hard work, the machine, nicknamed \"Hot Garbage\", was born. It killed Dr. Gumtree 30 seconds afterwards. May he rest in piece(s)."),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Garbage/HotGarbage");
            Texture2D glow = Helper.GetTexture("NPCs/Garbage/HotGarbage_Glow");
            Texture2D fire = Helper.GetTexture("NPCs/Garbage/HotGarbage_Fire");
            Vector2 origin = new Vector2((drawTexture.Width / 3) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 3) - (Helper.GetTexture("NPCs/Garbage/HotGarbage").Width / 3) * NPC.scale / 3f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Garbage/HotGarbage").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);
            //if (AIState != Intro)
            drawPos.Y -= 2;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            if (AIState != Intro && AIState != Idle && AIState != OpenLid && AIState != SpewFire && AIState != CloseLid && AIState != Death && AIState != ActualDeath && AIState != FallOver && AIState != SpewFire2 && AIState != BouncingBarrels && NPC.frame.X == 80)
                spriteBatch.Draw(fire, drawPos + new Vector2(NPC.width * -NPC.direction + (NPC.direction == 1 ? 9 : 0), 2).RotatedBy(NPC.rotation) * NPC.scale, new Rectangle(0, NPC.frame.Y - 76 * 3, 70, 76), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        public override void FindFrame(int f)
        {
            int frameHeight = 76;
            NPC.frame.Width = 80;
            NPC.frame.Height = 76;
            //NPC.frame.X = AIState == Intro && !NPC.IsABestiaryIconDummy ? 0 : 80;
            NPC.frameCounter++;

            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frame.X = 80;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }

            if (AIState == Intro && !NPC.IsABestiaryIconDummy)
            {
                NPC.frame.X = 0;
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
                else if (NPC.frameCounter < 25)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else if (NPC.frameCounter < 35)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                else if (NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                else if (NPC.frameCounter < 45)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                else if (NPC.frameCounter < 50)
                {
                    NPC.frame.Y = 9 * frameHeight;
                }
                else if (NPC.frameCounter < 55)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }
                else if (NPC.frameCounter < 60)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
                else if (NPC.frameCounter < 65)
                {
                    NPC.frame.Y = 12 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                    if (!NPC.IsABestiaryIconDummy)
                    {
                        NPC.Center += new Vector2(2 * NPC.direction, 0);
                        NPC.frame.X = 80;
                        NPC.frame.Y = 0;
                        AIState = Idle;
                        AITimer = 0;
                        AITimer2 = 0;
                        NextAttack = MassiveLaser;
                    }
                }
            }
            else if (AIState == Idle || (AIState == TrashBags && AITimer > 120))
            {
                NPC.frame.X = 80;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 2 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
            else if (AIState == WarningForDash || AIState == SlamPreperation || AIState == WarningForBigDash || (AIState == PipeBombAirstrike && AITimer <= 25) || (AIState == MassiveLaser && AITimer <= 25))
            {
                NPC.frame.X = 80;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else if (NPC.frame.Y >= 5 * frameHeight || NPC.frame.Y < 3 * frameHeight)
                    {
                        NPC.frame.Y = 3 * frameHeight;
                    }
                }
            }
            else if (AIState == SlamSlamSlam || AIState == Dash || AIState == BigDash || (AIState == PipeBombAirstrike && AITimer > 25) || (AIState == MassiveLaser && AITimer > 25))
            {
                NPC.frame.X = 80;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 9 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else if (NPC.frame.Y >= 9 * frameHeight || NPC.frame.Y < 6 * frameHeight)
                    {
                        NPC.frame.Y = 6 * frameHeight;
                    }
                }
            }
            else if (AIState == OpenLid)
            {
                NPC.frame.X = 160;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 3 * frameHeight)
                    {
                        NPC.frame.Y += frameHeight;
                    }
                    else
                    {
                        AITimer = 0;
                        AIState = NextAttack2;
                    }
                }
            }
            else if (AIState == CloseLid)
            {
                NPC.frame.X = 160;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frame.Y -= frameHeight;
                    }
                    else
                    {
                        AIState = Idle;
                    }
                }
            }

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
        const int ActualDeath = -2, Death = -1, Intro = 0, Idle = 1, WarningForDash = 2, Dash = 3, SlamPreperation = 4, SlamSlamSlam = 5,
            WarningForBigDash = 6, BigDash = 7, OpenLid = 8, SpewFire = 9, CloseLid = 10, FallOver = 11, SpewFire2 = 12, BouncingBarrels = 13, TrashBags = 14,
            SodaMissiles = 15, PipeBombAirstrike = 16, SateliteLightning = 17, MassiveLaser = 18, MailBoxes = 19, GiantFireball = 20;
        int NextAttack = OpenLid;
        int NextAttack2 = TrashBags;
        bool ded;
        public override bool CheckDead()
        {
            if (NPC.life <= 0 && !ded)
            {
                NPC.life = 1;
                AIState = Death;
                NPC.frameCounter = 0;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                //EbonianSystem.ChangeCameraPos(NPC.Center, 250, );
                EbonianSystem.ScreenShakeAmount = 5;
                ded = true;
                AITimer = AITimer2 = -100;
                NPC.velocity = Vector2.Zero;
                NPC.frame.X = 160;
                NPC.frame.Y = 0;
                NPC.life = 1;
                Music = 0;
                return false;
            }
            if (!EbonianAchievementSystem.acquiredAchievement[0])
                InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(0));
            return true;
        }
        Vector2 pos;
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return (NPC.Center.Y <= player.Center.Y - 100);
        }
        List<Vector2> redFrames = new List<Vector2>
        {
            new Vector2(0, 76*8),new Vector2(0, 76*10),new Vector2(0, 76*11),new Vector2(0, 76*12),

            new Vector2(80, 0),new Vector2(80, 76*1),new Vector2(80, 76*2),

            new Vector2(80*2, 0),new Vector2(80*2, 76*1),new Vector2(80*2, 76*2),new Vector2(80*2, 76*3)
        };
        List<Vector2> yellowFrames = new List<Vector2>
        {
            new Vector2(80, 76*3),new Vector2(80, 76*4),new Vector2(80, 76*5)
        };
        List<Vector2> greenFrames = new List<Vector2>
        {
            new Vector2(80, 76*6),new Vector2(80, 76*7),new Vector2(80, 76*8),new Vector2(80, 76*9)
        };
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (redFrames.Contains(new Vector2(NPC.frame.X, NPC.frame.Y)))
                Lighting.AddLight(NPC.Center, TorchID.Red);
            if (yellowFrames.Contains(new Vector2(NPC.frame.X, NPC.frame.Y)))
                Lighting.AddLight(NPC.Center, TorchID.Yellow);
            if (greenFrames.Contains(new Vector2(NPC.frame.X, NPC.frame.Y)))
                Lighting.AddLight(NPC.Center, TorchID.Green);
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    NPC.active = false;
                    return;
                }
            }
            NPC.timeLeft = 2;

            if (Main.rand.NextBool(300) && AIState != Intro && AIState != SlamSlamSlam && AIState != BigDash)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center - new Vector2(Main.rand.NextFloat(-500, 500), 200), Vector2.UnitY, 600, true), Vector2.Zero, ModContent.ProjectileType<Mailbox>(), 15, 0, player.whoAmI);
            }


            /*if (AIState != Death && AIState != BigDash)
            {
                if (NPC.Center.Y <= player.Center.Y - 100)
                    NPC.noTileCollide = true;
                else
                    NPC.noTileCollide = false;
            }
            else
                NPC.noTileCollide = false;*/
            if (AIState == Death)
            {
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (NPC.Grounded())
                {
                    AITimer++;
                    if (AITimer == -99)
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GarbageSiren");
                        EbonianSystem.ChangeCameraPos(NPC.Center, 200, 1.7f);
                    }
                }
                if (AITimer == 0)
                {
                    EbonianSystem.ScreenShakeAmount = 20;

                }
                if (AITimer % 5 == 0 && AITimer <= 21 && AITimer >= 0)
                {
                    if (NPC.frame.Y < 3 * 76)
                    {
                        NPC.frame.Y += 76;
                    }
                }
                if (AITimer == 20)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HotGarbageNuke>(), 0, 0);
                }
                if (AITimer == 664)
                    Music = 0;
                if (AITimer >= 665 && player.Distance(NPC.Center) > 4500 / 2)
                {
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;
                    NPC.StrikeInstantKill();
                    SoundEngine.PlaySound(EbonianSounds.garbageDeath, player.Center);
                }
                if (AITimer >= 665 && player.Distance(NPC.Center) < 4500 / 2)
                {
                    NPC.active = false;
                    Main.NewText("L", Color.Red);
                }
                if (++AITimer2 == 1)
                {
                    pos = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y + 1000);
                    //Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, new Vector2(0, -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                }
                if (AITimer2 == 40)
                {
                    AITimer2 = 0;
                    //Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), pos, new Vector2(0, -1), ModContent.ProjectileType<EFireBreath>(), 15, 0).localAI[0] = 650;
                }
                if (AITimer % 30 == 0 && AITimer > 120)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - Vector2.UnitY * 1000, new Vector2(i * Main.rand.NextFloat(1, 5), Main.rand.NextFloat(-2, 0)), ModContent.ProjectileType<GarbageGiantFlame>(), 15, 0);
                        fire.timeLeft = 200;
                    }
                }
            }
            else if (AIState == Intro)
            {
                if (!NPC.collideY)
                {
                    NPC.frameCounter = 0;
                }
                AITimer2++;
                if (NPC.collideY || AITimer2 > 150)
                {
                    AITimer++;
                    if (AITimer == 1)
                    {
                        player.JumpMovement();
                        player.velocity.Y = -10;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 0), ModContent.ProjectileType<FatSmash>(), 0, 0, 0, 0);
                        EbonianSystem.ChangeCameraPos(NPC.Center, 95);
                    }
                    if (AITimer == 15)
                        SoundEngine.PlaySound(EbonianSounds.garbageAwaken);
                    if (AITimer == 55)
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloodShockwave2>(), 0, 0);
                    if (AITimer < 30)
                    {

                        NPC.frameCounter = 0;
                    }
                }
            }
            else if (AIState == Idle)
            {
                AITimer++;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.35f);
                NPC.scale = MathHelper.Lerp(NPC.scale, 1, 0.35f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                if (NPC.Grounded(offsetX: 0.5f) && (NPC.collideX || Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 1000) < NPC.width || Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 1000) < NPC.width))
                    NPC.velocity.Y = -10;
                if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 300)
                    NPC.velocity.Y = -20;
                else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 100)
                    NPC.velocity.Y = -10;
                NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * 3;
                if (AITimer >= 100) //change this back to 300 once testing is over.
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AIState = NextAttack;
                    if (NextAttack == OpenLid)
                        NPC.frame.Y = 0;
                }
            }
            else if (AIState == WarningForDash)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer >= 100)
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AIState = Dash;
                }

            }
            else if (AIState == Dash)
            {
                //old code i refuse to even look at, it works.
                NPC.damage = 30;
                AITimer++;
                int num899 = 80;
                int num900 = 80;
                Vector2 position5 = new Vector2(NPC.Center.X - (float)(num899 / 2), NPC.position.Y + (float)NPC.height - (float)num900);
                if (AITimer3 == 7)
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                if (Collision.SolidCollision(position5, num899, num900))
                {
                    NPC.velocity.Y = -5.75f;
                }
                if (AITimer3 < 22)
                {
                    NPC.velocity.X = 13.5f * NPC.direction;
                }
                if (AITimer3 >= 22)
                {
                    NPC.velocity *= 0.96f;
                }
                if (AITimer3 >= 22 && AITimer3 < 40 && AITimer3 % 2 == 0)
                {
                    for (int i = -1; i < 1; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 2), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer3 >= 40 && AITimer % 5 == 0)
                {
                    NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                    NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                }
                if (++AITimer3 >= 65)
                {
                    AITimer3 = 0;
                }
                if (AITimer >= 65 * 3)
                {
                    NPC.velocity = Vector2.Zero;
                    NextAttack = OpenLid;
                    NextAttack2 = SpewFire;
                    AIState = Idle;
                    AITimer = 0;
                    AITimer2 = 0;
                    NPC.damage = 0;
                    AITimer3 = 0;
                    NPC.direction = 1;
                }
            }
            else if (AIState == SlamPreperation)
            {
                AITimer++;
                NPC.rotation += MathHelper.ToRadians(-0.9f * 4 * NPC.direction);
                if (AITimer >= 25)
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AIState = SlamSlamSlam;
                }
            }
            else if (AIState == SlamSlamSlam)
            {
                AITimer++;
                NPC.noGravity = true;
                NPC.damage = 30;
                if (AITimer < 50)
                    NPC.velocity.Y--;
                if (AITimer >= 50 && AITimer < 200)
                {
                    NPC.direction = NPC.spriteDirection = 1;
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.ToRadians(90), 0.15f);
                    if (AITimer % 5 == 0)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 500)) * 10;
                }
                if (AITimer == 2)
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                if (AITimer == 175)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<GarbageTelegraph>(), 0, 0);
                if (AITimer == 200)
                {
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                    for (int i = -4; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(3 * i * Main.rand.NextFloat(0.7f, 1.2f), 5), ModContent.ProjectileType<GarbageFlame>(), 15, 0, ai0: 1);
                    }
                    NPC.velocity = new Vector2(0, 35);
                }
                if (AITimer > 200 && !NPC.collideY)
                {
                    NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                    NPC.velocity.Y += 0.015f;
                }
                if ((NPC.collideY || NPC.Grounded(offsetX: 0.25f)) && AITimer2 == 0 && AITimer >= 200 && NPC.Center.Y >= player.Center.Y - 100)
                {
                    SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                    NPC.velocity = -Vector2.UnitY * 3;
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosionWSprite>(), 0, 0);
                    AITimer2 = 1;
                }
                if (AITimer2 >= 1)
                {
                    NPC.velocity.Y += 0.1f;
                    AITimer2++;
                }
                if (AITimer2 >= 50)
                {
                    NPC.noGravity = false;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = WarningForBigDash;
                    AIState = Idle;
                }
            }
            else if (AIState == WarningForBigDash)
            {
                AITimer++;
                NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * -2;
                if (AITimer == 10)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CircleTelegraph>(), 0, 0);
                NPC.rotation += MathHelper.ToRadians(-0.2f * 4 * NPC.direction);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer >= 25)
                {
                    NPC.velocity.X = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    AIState = BigDash;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (AIState == BigDash)
            {
                AITimer++;
                NPC.damage = 30;
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.35f);
                if (AITimer == 2)
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                if (AITimer < 12)
                {
                    NPC.velocity += new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 2, -1.2f);
                }
                if (AITimer % 5 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer >= 100)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = -250;
                    NPC.damage = 0;
                    NextAttack = OpenLid;
                    NextAttack2 = FallOver;
                    NPC.velocity = Vector2.Zero;
                    AIState = Idle;
                }
            }
            else if (AIState == OpenLid)
            {
                AITimer++;
                if (NextAttack2 == FallOver)
                    NPC.rotation -= MathHelper.ToRadians(-0.9f * 5 * NPC.direction);
                if (AITimer == 1)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GreenShockwave>(), 0, 0);

            }
            else if (AIState == SpewFire)
            {
                AITimer++;
                if (AITimer % 6 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 7), -4 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer >= 100)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = SlamPreperation;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }
            else if (AIState == FallOver)
            {
                AITimer++;
                if (AITimer == 1)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction, 0), ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                if (AITimer == 40)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction, 0), ModContent.ProjectileType<EFireBreath>(), 15, 0).localAI[0] = 650;
                }
                if (AITimer >= 80)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack2 = TrashBags;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }
            else if (AIState == SpewFire2)
            {
                AITimer++;
                if (AITimer % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer >= 50)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NPC.velocity = Vector2.Zero;
                    NextAttack2 = SateliteLightning;
                    NextAttack = OpenLid;
                    AIState = CloseLid;
                }
            }
            else if (AIState == BouncingBarrels)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer % 50 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item10, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 4, -3), ModContent.ProjectileType<GarbageBarrel>(), 15, 0, player.whoAmI);
                }

                if (AITimer >= 250)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = OpenLid;
                    NextAttack2 = GiantFireball;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }
            else if (AIState == GiantFireball)
            {
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                AITimer++;
                if (AITimer == 10)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer == 30)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer == 60)
                {
                    EbonianSystem.ScreenShakeAmount = 10;
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-0.4f).WithVolumeScale(1.1f), NPC.Center);
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -7 - Main.rand.NextFloat(4, 7)), ModContent.ProjectileType<GarbageGiantFlame>(), 15, 0, ai2: 1);
                }
                if (AITimer >= 80)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NPC.velocity = Vector2.Zero;
                    NextAttack = MassiveLaser;
                    AIState = CloseLid;
                }
            }
            else if (AIState == TrashBags)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer <= 60 && AITimer % 5 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)), ModContent.ProjectileType<GarbageBag>(), 15, 0, player.whoAmI).timeLeft = 200;
                }
                if (AITimer % 5 == 0 && AITimer > 100)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y), new Vector2(Main.rand.NextFloat(-1, 1), 2), ModContent.ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
                }
                if (AITimer >= 100 && AITimer < 120)
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y > 0)
                        {
                            NPC.frame.Y -= 76;
                        }
                    }
                }
                if (AITimer >= 250)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NPC.velocity = Vector2.Zero;
                    NextAttack2 = SodaMissiles;
                    NextAttack = OpenLid;
                    AIState = Idle;
                }
            }
            else if (AIState == SodaMissiles)
            {
                AITimer++;
                if (AITimer % 5 == 0 && AITimer < 40)
                {
                    SoundEngine.PlaySound(SoundID.Item156, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4, 4), -7), ModContent.ProjectileType<GarbageMissile>(), 15, 0, player.whoAmI, MathHelper.ToRadians(Main.rand.NextFloat(-3, 3)));
                }
                if (AITimer >= 60)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NPC.velocity = Vector2.Zero;
                    NextAttack = MailBoxes;
                    AIState = CloseLid;
                }
            }
            else if (AIState == MailBoxes)
            {
                AITimer++;
                if (AITimer == 20)
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                if (AITimer >= 20 && AITimer <= 40 && AITimer % 10 == 0)
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ModContent.ProjectileType<GreenShockwave>(), 0, 0);

                if (AITimer > 60 && AITimer < 80)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center - new Vector2(Main.rand.NextFloat(-1000, 1000), 200), Vector2.UnitY, 600, true), Vector2.Zero, ModContent.ProjectileType<Mailbox>(), 15, 0, player.whoAmI);
                }
                if (AITimer >= 120)
                {
                    AITimer = -80;
                    NPC.damage = 0;
                    NPC.velocity = Vector2.Zero;
                    NextAttack2 = SpewFire2;
                    NextAttack = OpenLid;
                    AIState = Idle;
                }
            }
            else if (AIState == SateliteLightning)
            {
                AITimer++;
                if (AITimer == 20)
                {
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<GarbageDrone>(), 20, 0, ai2: Main.rand.NextFloat(0.02f, 0.035f));
                }
                if (AITimer > 20 && AITimer < 90 && AITimer % 10 == 0)
                {
                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<GarbageDrone>(), 20, 0, ai1: Main.rand.NextFloat(-700, 700), ai2: Main.rand.NextFloat(0.02f, 0.035f));
                }
                if (AITimer >= 180)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    AIState = CloseLid;
                    NextAttack = PipeBombAirstrike;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (AIState == PipeBombAirstrike)
            {
                AITimer++;
                if (AITimer == 2)
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                if (AITimer <= 25)
                    NPC.rotation += MathHelper.ToRadians(-0.9f * 4 * NPC.direction);
                if (AITimer < 75 && AITimer > 25)
                    NPC.velocity.Y--;
                if (AITimer >= 75 && AITimer < 150)
                {
                    if (AITimer < 150)
                        pos = player.Center;
                    NPC.direction = NPC.spriteDirection = 1;
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, MathHelper.ToRadians(90), 0.15f);
                    if (AITimer % 5 == 0)
                        NPC.velocity = Helper.FromAToB(NPC.Center, pos - new Vector2(0, 700)) * 10;
                }
                if (AITimer == 150)
                {
                    NPC.velocity = Vector2.Zero;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<GarbageTelegraph>(), 0, 0);
                }
                if (AITimer > 170 && AITimer <= 200 && AITimer % 5 == 0)
                {
                    Projectile.NewProjectile(null, Main.rand.NextVector2FromRectangle(NPC.getRect()), Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(5, 10), ModContent.ProjectileType<Pipebomb>(), 15, 0);
                }
                if (AITimer == 200)
                {
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                    NPC.velocity = new Vector2(0, 50);
                }
                if (AITimer > 200 && !NPC.collideY)
                {
                    NPC.position.Y += NPC.velocity.Y;
                }
                if ((NPC.collideY || NPC.Grounded(offsetX: 0.25f)) && AITimer2 == 0 && AITimer >= 200 && NPC.Center.Y >= player.Center.Y - 100)
                {
                    SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                    NPC.velocity = -Vector2.UnitY * 3;
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosionWSprite>(), 0, 0);
                    AITimer2 = 1;
                }
                if (AITimer2 >= 1)
                {
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.1f);
                    NPC.velocity.Y += 0.1f;
                    AITimer2++;
                }
                if (AITimer2 >= 50)
                {
                    AITimer2 = 0;
                    AITimer = 0;
                    NPC.damage = 0;
                    AIState = Idle;
                    NextAttack = OpenLid;
                    NextAttack2 = BouncingBarrels;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (AIState == MassiveLaser)
            {
                AITimer++;
                if (AITimer <= 25)
                {
                    NPC.velocity = Vector2.Zero;
                    NPC.rotation += MathHelper.ToRadians(-0.9f * 4 * NPC.direction);
                }
                if (AITimer < 50 && AITimer > 25)
                {
                    pos = NPC.Center;
                    NPC.velocity.Y--;
                }
                if (AITimer > 50)
                {
                    NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 500)).X * (2 + AITimer2), 0.2f);
                    NPC.velocity.Y = MathHelper.Lerp(NPC.velocity.Y, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 500)).Y * (5 * AITimer2), 0.2f);
                }
                if (AITimer == 100)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<GarbageTelegraph>(), 0, 0);

                if (AITimer == 120)
                {
                    EbonianSystem.ScreenShakeAmount = 5;
                    AITimer2 = 1;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(NPC.velocity.X, NPC.height * 0.75f), Vector2.UnitY, ModContent.ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(10, NPC.height * -0.75f), Vector2.UnitY, ModContent.ProjectileType<GarbageLaser1>(), 100, 0, ai0: NPC.whoAmI);
                }
                if (AITimer == 240)
                {
                    EbonianSystem.ScreenShakeAmount = 15;
                    AITimer2 = 2;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(NPC.velocity.X, NPC.height * 0.75f), Vector2.UnitY, ModContent.ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(10, NPC.height * -0.75f), Vector2.UnitY, ModContent.ProjectileType<GarbageLaser2>(), 100, 0, ai0: NPC.whoAmI);
                }
                if (AITimer == 360)
                {
                    EbonianSystem.ScreenShakeAmount = 30;
                    AITimer2 = 3;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(NPC.velocity.X, NPC.height * 0.75f), Vector2.UnitY, ModContent.ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + new Vector2(10, NPC.height * -0.75f), Vector2.UnitY, ModContent.ProjectileType<GarbageLaser3>(), 100, 0, ai0: NPC.whoAmI);
                }
            }
        }
    }
    public class HotGarbageNuke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 35;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.hide = true;
        }
        Vector2 targetPos;
        float waveTimer;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
            behindProjectiles.Add(index);
        }
        public override void PostDraw(Color lightColor)
        {
            Texture2D pulse = Helper.GetExtraTexture("PulseCircle2");
            Texture2D chevron = Helper.GetExtraTexture("chevron");
            Texture2D hazard = Helper.GetExtraTexture("hazardUnblurred");
            Texture2D textGlow = Helper.GetExtraTexture("textGlow");
            Texture2D circle = Helper.GetExtraTexture("explosion2");
            float alpha = Utils.GetLerpValue(0, 2, waveTimer);
            float alpha2 = MathHelper.Clamp((float)Math.Sin(alpha * Math.PI) * 1, 0, 1f);
            if (targetPos != Vector2.Zero)
            {
                Main.spriteBatch.Draw(circle, targetPos - Main.screenPosition, null, Color.Black * Projectile.ai[2] * 0.5f, 0, circle.Size() / 2, 4.8f, SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(pulse, targetPos - Main.screenPosition, null, Color.DarkRed * Projectile.ai[2], 0, pulse.Size() / 2, 4.5f, SpriteEffects.None, 0);
                //Main.spriteBatch.Draw(pulse, targetPos - Main.screenPosition, null, Color.Maroon * alpha2, 0, pulse.Size() / 2, waveTimer * 2, SpriteEffects.None, 0);
                for (int i = 0; i < 16; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, 16);
                    Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height / 2) * Vector2.One, waveTimer);
                    Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, Color.DarkRed * alpha2, angle + MathHelper.PiOver4, chevron.Size() / 2, 0.5f, SpriteEffects.None, 0);
                }
                for (int i = 0; i < 16; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, 16);
                    Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height / 2) * Vector2.One, waveTimer) + 100 * Vector2.One;
                    Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, Color.DarkRed * alpha2 * 0.75f, angle + MathHelper.PiOver4, chevron.Size() / 2, 0.5f, SpriteEffects.None, 0);
                }
                for (int i = 0; i < 16; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, 16);
                    Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height / 2) * Vector2.One, waveTimer) + 200 * Vector2.One;
                    Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, Color.DarkRed * alpha2 * 0.5f, angle + MathHelper.PiOver4, chevron.Size() / 2, 0.5f, SpriteEffects.None, 0);
                }
                for (int i = 0; i < 16; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, 16);
                    Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height / 2) * Vector2.One, waveTimer) + 300 * Vector2.One;
                    Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, Color.DarkRed * alpha2 * 0.25f, angle + MathHelper.PiOver4, chevron.Size() / 2, 0.5f, SpriteEffects.None, 0);
                }
            }
            string strin = "" + (int)(Projectile.ai[1] / 60);
            if (extraString != "NUKE DETONATION IN: ")
                strin = "";

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.UIScaleMatrix);
            Main.spriteBatch.Draw(textGlow, new Vector2(Main.screenWidth / 2, Main.screenHeight * 0.05f), null, Color.Black, 0, new Vector2(textGlow.Width / 2, textGlow.Height / 2), 10, SpriteEffects.None, 0);

            for (int i = -(int)(Main.screenWidth / hazard.Width); i < (int)(Main.screenWidth / hazard.Width); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Main.spriteBatch.Draw(hazard, new Vector2(Main.screenWidth / 2 + (i * hazard.Width) - waveTimer * hazard.Width, Main.screenHeight * 0.0325f), null, Color.Red, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(hazard, new Vector2(Main.screenWidth / 2 + (i * hazard.Width) + waveTimer * hazard.Width, Main.screenHeight * 0.122f), null, Color.Red, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);
                }
            }
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, extraString + strin, new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString((extraString + strin).ToString()).X / 2, Main.screenHeight * 0.05f), Color.Red);

            Main.spriteBatch.ApplySaved();
        }
        string extraString = "NUKE DETONATION IN: ";
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = 600;
            targetPos = Projectile.Center;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(EbonianSounds.nuke);
            foreach (Player player in Main.player)
            {
                if (player.active && player.Center.Distance(targetPos) < 4500 / 2)
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " advocated for the legalization of nuclear bombs."), 999999, 0);
            }
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.Center.Distance(targetPos) < 4500 / 2 && npc.type != ModContent.NPCType<HotGarbage>())
                {
                    npc.life = 0;
                    npc.checkDead();
                }
            }
            EbonianMod.FlashAlpha = 1;
            //Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ScreenFlash>(), 0, 0);
        }
        public override void AI()
        {
            foreach (Player player in Main.player)
            {
                if (player.active)
                    if ((player.HeldItem.type == ItemID.MagicMirror || player.HeldItem.type == ItemID.RecallPotion) && player.itemAnimation > 2)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " thought they were smart."), 12345, 0);
                        Projectile.active = false;
                    }
            }
            if (Projectile.ai[2] < 1f)
                Projectile.ai[2] += 0.05f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[2] > 0)
                Helper.DustExplosion(Projectile.Center - new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation), Vector2.One, 2, Color.Transparent, false);
            if (Projectile.ai[1] > 0 && Projectile.ai[0] > 50)
                Projectile.ai[1]--;
            if (Projectile.ai[1] <= 0 && Projectile.ai[0] > 50)
            {
                Projectile.Kill();
            }
            extraString = "NUKE DETONATION IN: ";
            Projectile.timeLeft = 2;
            float alpha = Utils.GetLerpValue(0, 2, waveTimer);
            float alpha2 = MathHelper.Clamp((float)Math.Sin(alpha * Math.PI) * 3, 0, 1f);
            waveTimer += 0.025f * (waveTimer.Safe() + (alpha2.Safe() * 0.5f));
            if (waveTimer > 2)
                waveTimer = 0;

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 50)
                Projectile.velocity.Y -= 0.5f;
            else if (Projectile.ai[0] > 50 && Projectile.ai[0] < 450)
            {
                Projectile.velocity *= 0.9f;
            }
            else if (Projectile.ai[0] > 450)
            {
                Projectile.velocity.Y += 0.1f;
            }
        }
    }
}