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
using static tModPorter.ProgressUpdate;
using System.Collections.Generic;

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
            NPC.HitSound = SoundID.NPCHit4;
            NPC.knockBackResist = 0f;
            NPC.DeathSound = new Terraria.Audio.SoundStyle("EbonianMod/Sounds/NPCHit/GarbageDeath");
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
            if (AIState != Intro && AIState != Idle && AIState != OpenLid && AIState != SpewFire && AIState != CloseLid && AIState != Death && AIState != ActualDeath && AIState != FallOver && AIState != SpewFire2 && AIState != trash && NPC.frame.X == 80)
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
                        NextAttack = 2;
                    }
                }
            }
            else if (AIState == Idle)
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
            else if (AIState == WarningForDash || AIState == SlamPreperation || AIState == WarningForBigDash)
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
            else if (AIState == SlamSlamSlam || AIState == Dash || AIState == BigDash)
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
        const int ActualDeath = -2, Death = -1, Intro = 0, Idle = 1, WarningForDash = 2, Dash = 3, SlamPreperation = 4, SlamSlamSlam = 5, WarningForBigDash = 6, BigDash = 7, OpenLid = 8, SpewFire = 9, CloseLid = 10, FallOver = 11, SpewFire2 = 12, trash = 13, trash2 = 14;
        int NextAttack = 2;
        int NextAttack2 = 9;
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
                EbonianSystem.ScreenShakeAmount = 20;
                ded = true;
                AITimer = AITimer2 = -100;
                NPC.velocity = Vector2.Zero;
                NPC.frame.X = 160;
                NPC.frame.Y = 0;
                NPC.life = 1;
                Music = -1;
                return false;
            }
            return true;
        }
        Vector2 pos;
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return (NPC.Center.Y <= player.Center.Y - 100);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
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

            if (Main.rand.NextBool(300) && AIState != Intro)
            {
                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - new Vector2(Main.rand.NextFloat(-500, 500), 300), Vector2.Zero, ModContent.ProjectileType<Mailbox>(), 15, 0, player.whoAmI);
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
                        EbonianSystem.ChangeCameraPos(NPC.Center, 200, 1.2f);
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
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GarbageSiren");
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HotGarbageNuke>(), 0, 0);
                }
                if (AITimer >= 650)
                    Music = 0;
                if (AITimer >= 665 && player.Distance(NPC.Center) > 4500 / 2)
                {
                    NPC.life = 0;
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;
                    NPC.checkDead();
                    SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("EbonianMod/Sounds/NPCHit/GarbageDeath"), player.Center);
                }
                if (AITimer >= 665 && player.Distance(NPC.Center) < 4500 / 2)
                {
                    NPC.active = false;
                    Main.NewText("L", Color.Red);
                }
                if (++AITimer2 == 1)
                {
                    pos = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y + 1000);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, new Vector2(0, -1), ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                }
                if (AITimer2 == 40)
                {
                    AITimer2 = 0;
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), pos, new Vector2(0, -1), ModContent.ProjectileType<EFireBreath>(), 15, 0).localAI[0] = 650;
                }
                if (AITimer % 120 == 0 && AITimer > 20)
                {
                    for (int i = -6; i < 6; i++)
                    {
                        Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center - Vector2.UnitY * 1000, new Vector2(3 * i * Main.rand.NextFloat(1, 2), 10), ModContent.ProjectileType<GarbageFlame>(), 15, 0, ai0: 1);
                        fire.timeLeft = 200;
                        fire.tileCollide = false;
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
                        SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("EbonianMod/Sounds/GarbageAwaken"));
                    if (AITimer == 55)
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BloodShockwave>(), 0, 0);
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
                if (AITimer3 >= 22 && AITimer3 < 40)
                {
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 2), new Vector2(-NPC.direction, -2), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
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
                if (AITimer == 175)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CircleTelegraph>(), 0, 0);
                if (AITimer == 200)
                {

                    for (int i = -3; i < 3; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(3 * i, 10), ModContent.ProjectileType<GarbageFlame>(), 15, 0, ai0: 1);
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
                    NPC.velocity = Vector2.Zero;
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 0, 0);
                    AITimer2 = 1;
                }
                if (AITimer2 >= 1)
                {
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
                if (AITimer == 1)
                {
                    NPC.velocity = new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 22, -15);
                }
                if (AITimer % 7 == 0)
                {

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-NPC.direction, -2), ModContent.ProjectileType<GarbageFlame>(), 15, 0);

                }
                if (AITimer >= 100)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = -350;
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
                if (AITimer % 3 == 0)
                {
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction * Main.rand.NextFloat(5, 7), -4 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
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
                if (AITimer == 50)
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction, 0), ModContent.ProjectileType<EFireBreath>(), 15, 0).localAI[0] = 650;
                if (AITimer >= 100)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack2 = trash2;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }
            else if (AIState == SpewFire2)
            {
                AITimer++;
                if (AITimer % 10 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer >= 50)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    AIState = CloseLid;
                    NextAttack = OpenLid;
                    NextAttack2 = trash;
                    NPC.velocity = Vector2.Zero;
                    //AIState = CloseLid;
                }
            }
            else if (AIState == trash)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer % 100 == 0)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4, 4), -7), ModContent.ProjectileType<Soder>(), 15, 0, player.whoAmI);
                if (AITimer % 50 == 0)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 4, -3), ModContent.ProjectileType<MetalCircle>(), 15, 0, player.whoAmI);

                if (AITimer >= 250)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = WarningForDash;
                    NPC.velocity = Vector2.Zero;
                    AIState = Idle;
                }
            }
            else if (AIState == trash2)
            {
                AITimer++;
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer <= 60 && AITimer % 5 == 0)
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -5), ModContent.ProjectileType<Garbage>(), 15, 0, player.whoAmI).timeLeft = 200;
                if (AITimer % 15 == 0 && AITimer > 100)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y), new Vector2(Main.rand.NextFloat(-1, 1), 2), ModContent.ProjectileType<Garbage>(), 15, 0, player.whoAmI);

                if (AITimer >= 450)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = OpenLid;
                    NextAttack2 = SpewFire2;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }
        }
    }
    public class Soder : ModProjectile
    {
        //public override string Texture => "Terraria/Images/Item_" + ItemID.CreamSoda;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Projectile.timeLeft > 300)
                Projectile.velocity *= 0.99f;
            else
            {
                Projectile.velocity *= 1.025f;
                Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Gray * 0.1f, false, false, 0.1f, 0.125f, -Projectile.velocity);
            }
            if (Projectile.timeLeft == 300)
                Projectile.velocity = Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 5;
        }
    }
    public class MetalCircle : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.BouncyGlowstick;
            Projectile.hostile = true;
            Projectile.timeLeft = 400;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity.Y = -5;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        int dir;
        public override void OnSpawn(IEntitySource source)
        {
            dir = Projectile.velocity.X > 0 ? 1 : -1;
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y + 100)
                Projectile.velocity.Y -= (Projectile.Center.Y - Main.LocalPlayer.Center.Y) * 0.035f;
        }
        public override void AI()
        {
            Projectile.direction = dir;
            Projectile.velocity.X = Projectile.direction * 5;

            Projectile.rotation += MathHelper.ToRadians(3);
        }
    }
    public class GarbageFlame : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/explosion";
        public override void SetStaticDefaults()
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Orange, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.035f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 350;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            if (Projectile.velocity.Y > 2.8f && Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.87f;
            }
            if (Main.rand.Next(2) == 0)
            {
                for (int dustNumber = 0; dustNumber < 5; dustNumber++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 6, 0, 0, 0, default(Color), 1f)];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(Projectile.width / 2, Projectile.height / 2) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                    dust.velocity.X = Main.rand.NextFloat(-0.5f, 0.5f);
                    dust.velocity.Y = -2f;
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.65f, 1.25f);
                }
            }
        }
    }
    public class Mailbox : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 44;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200;
            Projectile.scale = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            float scale = Math.Clamp(MathHelper.Lerp(0, 1, Projectile.scale * 2), 0, 1);
            Rectangle frame = new Rectangle(0, Projectile.frame * 46, 30, 46);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, new Vector2(tex.Width / 2, Projectile.height), new Vector2(Projectile.scale, 1), SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.Center = Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, 1000) - new Vector2(0, 44);
                Projectile.ai[0] = 1;
            }
            if (Projectile.timeLeft == 150)
                Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CircleTelegraph>(), 0, 0);
            if (Projectile.timeLeft == 100)
            {
                Projectile.frame = 1;
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Helper.FromAToB(Projectile.Center, Main.player[Projectile.owner].Center) * 10, ModContent.ProjectileType<Pipebomb>(), Projectile.damage, 0, Projectile.owner);
            }

            float progress = Utils.GetLerpValue(0, 200, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 3, 0, 1);
        }
    }
    public class Garbage : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 22;
            Projectile.timeLeft = 500;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.height = 24;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.frame == 0 && Projectile.timeLeft > 100)
                Projectile.timeLeft = 100;
            Projectile.velocity *= 0.5f;
            Projectile.frame = 1;
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            Projectile.velocity *= 1.025f;
            if (Projectile.frame == 0)
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            else
                Projectile.rotation = 0;

        }
    }
    public class Pipebomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = 14;
            AIType = ProjectileID.StickyGlowstick;
            Projectile.width = 18;
            Projectile.timeLeft = 100;
            Projectile.height = 36;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
                fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 30)
            {
                Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CircleTelegraph>(), 0, 0);
                Projectile.velocity = Vector2.Zero;
                Projectile.aiStyle = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), Projectile.damage, 0, Projectile.owner);
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
            SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("EbonianMod/Sounds/Nuke"));
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
            foreach (Player player in Main.player)
            {
                if (player.active && player.Center.Distance(targetPos) < 4500 / 2)
                {
                    if (Projectile.ai[1] < 121)
                    {
                        extraString = "cry about it";
                    }
                    else if (Projectile.ai[1] < 181)
                        extraString = "lol";
                }
                if (player.active && player.Center.Distance(targetPos) > 4500 / 2)
                    extraString = "NUKE DETONATION IN: ";
            }
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