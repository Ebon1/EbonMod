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

using Terraria.Audio;
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Garbage;
using static tModPorter.ProgressUpdate;
using System.Collections.Generic;
using Terraria.UI;
using EbonianMod.Common.Achievements;
using EbonianMod.Common.Systems;
using System.Security.Cryptography.X509Certificates;
using ReLogic.Utilities;
using EbonianMod.Dusts;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Items.Weapons.Ranged;
using Terraria.GameContent.ItemDropRules;
using EbonianMod.Items.BossTreasure;
using EbonianMod.Items.Pets.Hightoma;
using EbonianMod.Items.Pets;
using EbonianMod.Items.Weapons.Summoner;
using EbonianMod.Items.Tiles.Trophies;
using Terraria.GameContent.UI;
using Terraria.Graphics.CameraModifiers;
using EbonianMod.NPCs.ArchmageX;
using System.Linq;

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
            NPC.defense = 16;
            NPC.lifeMax = 5250;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.buffImmune[BuffID.OnFire] = true;
            //NPC.DeathSound = EbonianSounds.garbageDeath;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Garbage");
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(new CommonDrop(ItemType<Chainsword>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<DoomsdayRemote>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<MailboxStaff>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<SalvagedThruster>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<GarbageFlail>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<NastySnapper>(), 4));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<PipebombI>(), 1, 20, 100));
            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.Common(ItemType<GarbageTrophy>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<GarbagePet>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<XRelic>()));

            npcLoot.Add(ItemDropRule.BossBag(ItemType<GarbageBagI>()));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: Dumpster"),
                new FlavorTextBestiaryInfoElement("A heavily armed and armored dumpster, possibly for combat purposes, that has somehow been on fire since its conception.\n\nA note on its side reads: \"Please return to GenCorp Headquarters if lost.\""),
            });
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Garbage/HotGarbage");
            Texture2D glow = Helper.GetTexture("NPCs/Garbage/HotGarbage_Glow");
            Texture2D fire = Helper.GetTexture("NPCs/Garbage/HotGarbage_Fire");
            Texture2D fireball = Helper.GetExtraTexture("fireball");
            Vector2 origin = new Vector2((drawTexture.Width / 3) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 3) - (Helper.GetTexture("NPCs/Garbage/HotGarbage").Width / 3) * NPC.scale / 3f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Garbage/HotGarbage").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);
            //if (AIState != Intro)
            drawPos.Y -= 2;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            if (AIState != Intro && AIState != Idle && AIState != OpenLid && AIState != SpewFire && AIState != CloseLid && AIState != ActualDeath && AIState != FallOver && AIState != SpewFire2 && AIState != BouncingBarrels && NPC.frame.X == 80)
                spriteBatch.Draw(fire, drawPos + new Vector2(NPC.width * -NPC.direction + (NPC.direction == 1 ? 9 : 0), 2).RotatedBy(NPC.rotation) * NPC.scale, new Rectangle(0, NPC.frame.Y - 76 * 3, 70, 76), Color.White, NPC.rotation, origin, NPC.scale, effects, 0);

            spriteBatch.Reload(BlendState.Additive);

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(drawTexture, drawPos, NPC.frame, Color.White * flameAlpha, NPC.rotation, origin, NPC.scale, effects, 0);
                if (AIState != Intro && AIState != Idle && AIState != OpenLid && AIState != SpewFire && AIState != CloseLid && AIState != Death && AIState != ActualDeath && AIState != FallOver && AIState != SpewFire2 && AIState != BouncingBarrels && NPC.frame.X == 80)
                    spriteBatch.Draw(fire, drawPos + new Vector2(NPC.width * -NPC.direction + (NPC.direction == 1 ? 9 : 0), 2).RotatedBy(NPC.rotation) * NPC.scale, new Rectangle(0, NPC.frame.Y - 76 * 3, 70, 76), Color.White * flameAlpha, NPC.rotation, origin, NPC.scale, effects, 0);
            }

            //spriteBatch.Draw(fireball, drawPos + new Vector2(3 * NPC.direction, 0), null, Color.OrangeRed * flameAlpha, NPC.rotation + PiOver2 + (NPC.direction == -1 ? Pi : 0), new Vector2(fireball.Width / 2, fireball.Height * 0.475f), NPC.scale * 2.2f, SpriteEffects.None, 0);
            //spriteBatch.Draw(fireball, drawPos + new Vector2(3 * NPC.direction, 0), null, Color.Gold * flameAlpha, NPC.rotation + PiOver2 + (NPC.direction == -1 ? Pi : 0), new Vector2(fireball.Width / 2, fireball.Height * 0.475f), NPC.scale * 2.15f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
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
                        NextAttack = WarningForDash;
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
            else if (AIState == WarningForDash || (AIState == Dash && (AITimer3 >= 22)) || AIState == SlamPreperation || AIState == WarningForBigDash || (AIState == PipeBombAirstrike && AITimer <= 25) || (AIState == MassiveLaser && AITimer <= 25))
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
            else if ((AIState == Death && AITimer > 40) || AIState == SlamSlamSlam || (AIState == Dash && !(AITimer3 >= 22)) || AIState == BigDash || (AIState == PipeBombAirstrike && AITimer > 25) || (AIState == MassiveLaser && AITimer > 25))
            {
                if ((AIState == PipeBombAirstrike || AIState == SlamSlamSlam ? AITimer > 200 : NPC.velocity.Length() > 4))
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
                else
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
                    if (NPC.frame.Y == frameHeight)
                        SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
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
        bool didAttacks;
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
                AITimer = -75;
                AITimer2 = -110;
                NPC.velocity = Vector2.Zero;
                NPC.frame.X = 160;
                NPC.frame.Y = 0;
                NPC.life = 1;
                Music = 0;
                return false;
            }
            //if (!EbonianAchievementSystem.acquiredAchievement[0])
            //  InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(0));
            return true;
        }
        Vector2 pos;
        public override bool? CanFallThroughPlatforms()
        {
            Player player = Main.player[NPC.target];
            return (NPC.Center.Y <= player.Center.Y - 100) || AIState == MassiveLaser;
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
        void JumpCheck()
        {
            Player player = Main.player[NPC.target];
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
            if (NPC.Grounded(offsetX: 0.5f) && (NPC.collideX || Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 1000) < NPC.width || Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 1000) < NPC.width))
                NPC.velocity.Y = -10;
            if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 300)
                NPC.velocity.Y = -20;
            else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 200)
                NPC.velocity.Y = -15;
            else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 100)
                NPC.velocity.Y = -10;
            if (AIState == Idle)
            {
                if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height) < NPC.height - 1 && !Collision.CanHit(NPC, player))
                {
                    NPC.noTileCollide = true;
                    if (player.Center.Y < NPC.Center.Y)
                        NPC.Center -= Vector2.UnitY * 2;
                    else
                        NPC.Center += Vector2.UnitY * 2;

                    NPC.Center += new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 2, 0);
                }
                else if ((!Collision.CanHit(NPC, player) || !Collision.CanHitLine(NPC.TopLeft, 10, 10, player.position, player.width, player.height) || !Collision.CanHitLine(NPC.TopRight, 10, 10, player.position, player.width, player.height)) && player.Center.X.CloseTo(NPC.Center.X, NPC.width))
                {
                    NPC.noTileCollide = true;
                    if (player.Center.Y < NPC.Center.Y)
                        NPC.Center -= Vector2.UnitY * 2;
                    else
                        NPC.Center += Vector2.UnitY * 2;
                }
                else
                    NPC.noTileCollide = false;
            }
        }
        public override void AI()
        {
            if (AIState != Idle && AIState != SlamSlamSlam && AIState != PipeBombAirstrike)
                NPC.noTileCollide = false;
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

                NPC.rotation = Lerp(NPC.rotation, 0, 0.1f);
                if (NPC.Grounded())
                {
                    AITimer++;
                    if (AITimer == -74)
                    {
                        pos = NPC.Center;
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GarbageSiren");
                        EbonianSystem.ChangeCameraPos(NPC.Center, 100, 1.7f);
                    }
                    if (AITimer == -30)
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0);
                    if (AITimer > -30)
                        AITimer2++;
                }
                else
                    NPC.velocity.Y++;
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
                if (AITimer >= 40 && AITimer <= 20)
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y == 76)
                            SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
                        if (NPC.frame.Y > 0)
                        {
                            NPC.frame.Y -= 76;
                        }
                    }
                }
                if (AITimer2 >= 22 && AITimer2 < 40 && AITimer2 % 2 == 0)
                {
                    for (int i = -1; i < 1; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 8), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0).timeLeft = 170;
                    }
                }
                if (AITimer == 20)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center - new Vector2(Main.rand.NextFloat(-30, 30), 20), DustID.Smoke, -Vector2.UnitY.RotatedByRandom(PiOver4) * Main.rand.NextFloat(5, 20));
                    }
                    SoundEngine.PlaySound(EbonianSounds.firework.WithVolumeScale(2), NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 50), Vector2.Zero, ProjectileType<HotGarbageNuke>(), 0, 0);
                }
                Main.musicNoCrossFade[MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GarbageSiren")] = true;
                Main.musicNoCrossFade[0] = true;
                if (AITimer >= 634)
                {
                    Main.musicFade[MusicLoader.GetMusicSlot(Mod, "Sounds/Music/GarbageSiren")] = 1;
                    Music = 0;
                }
                if (AITimer >= 665 && player.Distance(NPC.Center) > 4500 / 2)
                {
                    NPC.immortal = false;
                    GetInstance<DownedBossSystem>().downedGarbage = true;
                    NPC.dontTakeDamage = false;
                    NPC.StrikeInstantKill();
                }
                if (AITimer >= 665 && player.Distance(NPC.Center) < 4500 / 2)
                {
                    NPC.active = false;
                }
                int num899 = 130;
                int num900 = 130;
                Vector2 position5 = new Vector2(NPC.Center.X - (float)(num899 / 2), NPC.position.Y + (float)NPC.height - (float)num900);
                if (AITimer2 == 7)
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                if (Collision.SolidCollision(position5, num899, num900))
                {
                    NPC.velocity.Y = -5.75f;
                }
                if (AITimer2 < 22 && AITimer2 >= 0)
                {
                    NPC.velocity.X = Lerp(NPC.velocity.X, 20f * NPC.direction, 0.15f);
                }
                if (AITimer2 >= 22)
                {
                    NPC.velocity *= 0.96f;
                }
                if (AITimer2 == 40 || AITimer2 < 0)
                {
                    if (player.Center.Distance(pos) < 650)
                    {
                        NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                        NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                    }
                    else
                    {
                        NPC.spriteDirection = pos.X > NPC.Center.X ? 1 : -1;
                        NPC.direction = pos.X > NPC.Center.X ? 1 : -1;
                    }
                }
                if (AITimer2 >= 65)
                {
                    AITimer2 = -50;
                }
                if (AITimer % 20 == 0 && AITimer > 30 && AITimer < 630)
                {
                    Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), pos - Vector2.UnitY * 1000, new Vector2(Main.rand.NextFloat(-30, 30) * Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(-5, 1)), ProjectileType<GarbageGiantFlame>(), 15, 0);
                    fire.timeLeft = 200;
                }
            }
            else if (AIState == Intro)
            {
                if (!NPC.collideY && AITimer2 < 150)
                {
                    if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 80) > 50)
                        NPC.position.Y += NPC.velocity.Y * 0.5f;
                    NPC.frameCounter = 0;
                }
                NPC.dontTakeDamage = true;
                AITimer2++;
                if (NPC.collideY || AITimer2 > 150)
                {
                    AITimer++;
                    if (AITimer == 1)
                    {
                        NPC.position.Y -= NPC.velocity.Y;
                        player.JumpMovement();
                        player.velocity.Y = -10;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.height * 0.5f * Vector2.UnitY, new Vector2(0, 0), ProjectileType<GarbageImpact>(), 0, 0, 0, 0);
                        EbonianSystem.ChangeCameraPos(NPC.Center, 70, 1f);
                    }
                    if (AITimer == 15)
                    {
                        SoundEngine.PlaySound(EbonianSounds.garbageAwaken);
                    }
                    if (AITimer == 45)
                    {
                        EbonianSystem.ChangeCameraPos(NPC.Center, 40, 2f);
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
                    }
                    if (AITimer < 30)
                    {

                        NPC.frameCounter = 0;
                    }
                }
            }
            else if (AIState == Idle)
            {
                NPC.dontTakeDamage = false;
                NPC.damage = 0;
                AITimer++;
                NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
                NPC.scale = Lerp(NPC.scale, 1, 0.35f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                JumpCheck();
                if (AITimer == 50 && Main.rand.NextBool() && NextAttack2 != SpewFire)
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center - new Vector2(Main.rand.NextFloat(-500, 500), 200), Vector2.UnitY, 600, true), Vector2.Zero, ProjectileType<Mailbox>(), 15, 0, player.whoAmI);
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
                if (player.Distance(NPC.Center) < 70)
                    AITimer += 1;
                if (player.Distance(NPC.Center) < 40)
                    AITimer += 1;
                if (AITimer >= 150)
                {
                    if (NextAttack != WarningForDash)
                        NPC.velocity.X = 0;
                    AITimer = 0;
                    if (didAttacks)
                    {
                        List<int> attacks = new List<int>()
                        { WarningForDash, WarningForBigDash,  SlamPreperation, MailBoxes, PipeBombAirstrike, MassiveLaser,
                            OpenLid, OpenLid, OpenLid, OpenLid, OpenLid, OpenLid, };
                        List<int> openAttacks = new List<int>()
                        { SpewFire, SpewFire2, GiantFireball, TrashBags, SodaMissiles, SateliteLightning };
                        NextAttack = Main.rand.Next(attacks);
                        if (NextAttack == OpenLid)
                            NextAttack2 = Main.rand.Next(openAttacks);
                    }
                    AIState = NextAttack;
                    if (NextAttack == OpenLid)
                        NPC.frame.Y = 0;
                }
            }
            else if (AIState == WarningForDash)
            {
                NPC.velocity.X *= 0.99f;
                AITimer++;
                if (AITimer == 20)
                {
                    SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
                }
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer >= 55)
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
                NPC.damage = 60;
                AITimer++;
                int num899 = 130;
                int num900 = 130;
                Vector2 position5 = new Vector2(NPC.Center.X - (float)(num899 / 2), NPC.position.Y + (float)NPC.height - (float)num900);
                if (AITimer3 == 7)
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                if (Collision.SolidCollision(position5, num899, num900))
                {
                    NPC.velocity.Y = -5.75f;
                }
                if (AITimer3 < 22)
                {
                    NPC.velocity.X = Lerp(NPC.velocity.X, 20f * NPC.direction, 0.15f);
                }
                if (AITimer3 >= 22)
                {
                    NPC.velocity *= 0.96f;
                }
                if (AITimer3 >= 22 && AITimer3 < 40 && AITimer3 % 2 == 0)
                {
                    for (int i = -1; i < 1; i++)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 8), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0).timeLeft = 170;
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
                NPC.rotation += ToRadians(-0.9f * 4 * NPC.direction);
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
                NPC.damage = 60;
                if (AITimer < 50)
                    NPC.velocity.Y--;
                if (AITimer < 200)
                    NPC.noTileCollide = true;
                if (AITimer >= 50 && AITimer < 200)
                {
                    if (AITimer < 176)
                        pos = player.Center - new Vector2(-player.velocity.X * 20, 500);
                    NPC.direction = NPC.spriteDirection = 1;
                    NPC.rotation = Lerp(NPC.rotation, ToRadians(90), 0.15f);
                    if (AITimer % 8 == 0)
                        NPC.velocity = Helper.FromAToB(NPC.Center, pos, false) * 0.056f;
                }
                if (AITimer == 2)
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                if (AITimer == 181)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
                if (AITimer == 200)
                {
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                    for (int i = -4; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(6 * i * Main.rand.NextFloat(0.7f, 1.2f), 3), ProjectileType<GarbageGiantFlame>(), 15, 0, ai0: 1);
                    }
                    NPC.velocity = new Vector2(0, 35);
                }
                if (AITimer > 200 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                {
                    NPC.noTileCollide = false;
                }
                if (AITimer > 200 && !NPC.collideY && NPC.noTileCollide)
                {
                    NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                    NPC.velocity.Y += 0.015f;
                }
                if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer >= 200)
                {
                    SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                    NPC.velocity = -Vector2.UnitY * 3;
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
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
                NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * -1;
                if (AITimer == 10)
                {
                    SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
                }
                NPC.rotation += ToRadians(-0.2f * 2 * NPC.direction);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer >= 50)
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
                NPC.damage = 90;
                NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
                if (AITimer == 2)
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                if (AITimer < 12)
                {
                    NPC.velocity += new Vector2(((Helper.FromAToB(NPC.Center, player.Center).X < 0) ? -3 : 3) * 1.6f, -1.2f);
                }
                if (AITimer % 6 == 0)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer >= 110)
                {
                    NPC.velocity = Vector2.Zero;
                    AITimer = -50;
                    NPC.damage = 0;
                    NextAttack = OpenLid;
                    NextAttack2 = TrashBags;
                    NPC.velocity = Vector2.Zero;
                    AIState = Idle;
                }
            }
            else if (AIState == OpenLid)
            {
                AITimer++;
                if (NextAttack2 == FallOver)
                    NPC.rotation -= ToRadians(-0.9f * 5 * NPC.direction);
                if (AITimer == 1)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0);
                }

            }
            else if (AIState == SpewFire)
            {
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
                AITimer++;
                if (AITimer % 6 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    for (int i = -2; i < 2; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
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
            /*else if (AIState == FallOver)
            {
                AITimer++;
                if (AITimer == 1)
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center, Vector2.UnitY, NPC.velocity.Length() * 40), new Vector2(NPC.direction, 0), ProjectileType<GarbageTelegraphSmall>(), 0, 0, -1, 800);
                if (AITimer == 40)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(NPC.direction, 0), ProjectileType<EFireBreath>(), 15, 0).localAI[0] = 650;
                }
                if (AITimer >= 60)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack2 = TrashBags;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }
            }*/
            else if (AIState == SpewFire2)
            {
                AITimer++;
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
                if (AITimer % 6 == 0 && AITimer > 30)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                    }
                }
                if (AITimer >= 70)
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
                AIState = GiantFireball;
                /*AITimer++;
                JumpCheck();
                if (AITimer == 10)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(NPC.getRect()), DustID.Poop, Main.rand.NextVector2Circular(10, 10));
                    }
                    SoundEngine.PlaySound(SoundID.Item177, NPC.Center);
                }
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 4, 0.15f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer % 20 == 0 && AITimer > 40)
                {
                    SoundEngine.PlaySound(SoundID.Item10, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * (10 + AITimer * 0.2f), -3), ProjectileType<GarbageBarrel>(), 15, 0, player.whoAmI);
                }

                if (AITimer >= 160)
                {
                    AITimer = 0;
                    NPC.damage = 0;
                    NextAttack = OpenLid;
                    NextAttack2 = GiantFireball;
                    NPC.velocity = Vector2.Zero;
                    AIState = CloseLid;
                }*/
            }
            else if (AIState == GiantFireball)
            {
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.01f, 0.15f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                AITimer++;
                if (AITimer == 10)
                {
                    SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer == 20)
                {
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer == 80)
                {
                    EbonianSystem.ScreenShakeAmount = 12;
                    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-0.4f).WithVolumeScale(1.1f), NPC.Center);
                    for (int i = 0; i < 5; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -7 - Main.rand.NextFloat(4, 7)), ProjectileType<GarbageGiantFlame>(), 15, 0, ai2: 1);
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
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                if (AITimer <= 60 && AITimer % 5 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)), ProjectileType<GarbageBag>(), 15, 0, player.whoAmI).timeLeft = 200;
                }
                if (AITimer % 3 == 0 && AITimer > 100)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(player.Center.X + 600 * Main.rand.NextFloat(-1, 1), player.Center.Y - 600), new Vector2(Main.rand.NextFloat(-1, 1), 2), ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
                }
                if (AITimer >= 100 && AITimer < 120)
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y == 76)
                            SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
                        if (NPC.frame.Y > 0)
                        {
                            NPC.frame.Y -= 76;
                        }
                    }
                }
                if (AITimer >= 150)
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
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 4, 0.15f);
                if (AITimer % 5 == 0 && AITimer < 60 && AITimer > 20)
                {
                    SoundEngine.PlaySound(SoundID.Item156, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-4, 4), -7), ProjectileType<GarbageMissile>(), 15, 0, player.whoAmI, ToRadians(Main.rand.NextFloat(-3, 3)));
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
                JumpCheck();
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
                AITimer++;
                if (AITimer == 20)
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                if (AITimer >= 20 && AITimer <= 40 && AITimer % 10 == 0)
                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0);

                if (AITimer > 60 && AITimer < 82)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), Helper.TRay.Cast(NPC.Center - new Vector2(Main.rand.NextFloat(-2000, 2000), 200), Vector2.UnitY, 600, true), Vector2.Zero, ProjectileType<Mailbox>(), 15, 0, player.whoAmI);
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
                if (AITimer >= 20 && AITimer % 20 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ProjectileType<GarbageDrone>(), 20, 0, ai1: Helper.FromAToB(NPC.Center, player.Center + player.velocity * 2, false).X, ai2: Main.rand.NextFloat(0.02f, 0.035f));
                }
                if (AITimer > 20 && AITimer % 5 == 0)
                {
                    Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ProjectileType<GarbageDrone>(), 20, 0, ai1: Main.rand.NextFloat(-1500, 1500), ai2: Main.rand.NextFloat(0.02f, 0.035f));
                }
                if (AITimer >= 100)
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
                    NPC.rotation += ToRadians(-0.9f * 4 * NPC.direction);
                if (AITimer < 75 && AITimer > 25)
                {
                    NPC.noTileCollide = true;
                    NPC.velocity.Y--;
                }
                if (AITimer >= 75 && AITimer < 150)
                {
                    NPC.damage = 60;
                    if (AITimer < 150)
                        pos = player.Center;
                    NPC.direction = NPC.spriteDirection = 1;
                    NPC.rotation = Lerp(NPC.rotation, ToRadians(90), 0.15f);
                    NPC.velocity = Helper.FromAToB(NPC.Center, pos - new Vector2(-player.velocity.X * 10, 700), false) * 0.05f;
                }
                if (AITimer == 150)
                {
                    NPC.velocity = Vector2.Zero;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
                }
                if (AITimer > 170 && AITimer <= 200 && AITimer % 3 == 0)
                {
                    Projectile.NewProjectile(null, Main.rand.NextVector2FromRectangle(NPC.getRect()), Vector2.UnitY.RotatedByRandom(PiOver2) * Main.rand.NextFloat(5, 10), ProjectileType<Pipebomb>(), 15, 0);
                }
                if (AITimer == 200)
                {
                    SoundEngine.PlaySound(EbonianSounds.exolDash, NPC.Center);
                    NPC.velocity = new Vector2(0, 50);
                }
                if (AITimer > 200 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                    NPC.noTileCollide = false;
                if (AITimer > 200 && !NPC.collideY && NPC.noTileCollide)
                {
                    NPC.position.Y += NPC.velocity.Y;
                }
                if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer >= 200)
                {
                    SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                    NPC.velocity = -Vector2.UnitY * 3;
                    Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 16, 0);
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
                if (AITimer < 60)
                {
                    NPC.velocity.X = 0;
                    if (AITimer < 40)
                        NPC.velocity.Y = Lerp(NPC.velocity.Y, -30, 0.1f);
                    else
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.3f);
                    if (AITimer > 40)
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, PiOver2 * NPC.direction, 0.05f);
                }
                if (AITimer == 60)
                    NPC.rotation = PiOver2 * NPC.direction;
                if (AITimer > 60 && AITimer3 != 3)
                {
                    NPC.damage = 60;
                    flameAlpha = Lerp(flameAlpha, 1, 0.1f);
                    pos = NPC.Center;
                    if (NPC.velocity.Length() < 20)
                        NPC.velocity.Y += 1 + NPC.velocity.Y;
                    NPC.Center += Vector2.UnitY * NPC.velocity.Y;
                }
                else
                    flameAlpha = Lerp(flameAlpha, 0, 0.2f);
                bool colliding = Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, NPC.width * 0.6f) < NPC.width * 0.3f ||
                    Helper.TRay.CastLength(NPC.BottomRight, Vector2.UnitY, NPC.width * 0.6f) < NPC.width * 0.3f ||
                    Helper.TRay.CastLength(NPC.BottomLeft, Vector2.UnitY, NPC.width * 0.6f) < NPC.width * 0.3f;
                if (colliding && AITimer > 60 && AITimer < 320)
                {
                    if (AITimer3 != 3)
                    {
                        pos = NPC.Center + new Vector2(0, NPC.height * 0.5f);
                        AITimer3 = 3;
                        for (int i = 0; i < 4; i++)
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center + Main.rand.NextVector2Circular(15, 15), Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
                        Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FatSmash>(), 0, 0).scale = Main.rand.NextFloat(0.4f, 0.7f);
                    }
                    else
                    {
                        NPC.Center = Vector2.Lerp(NPC.Center, pos + Main.rand.NextVector2Circular(AITimer2 * 10f, AITimer2), 0.2f);
                        NPC.velocity = Vector2.Zero;
                    }
                    if (AITimer % 3 - (int)AITimer2 == 0)
                    {
                        Vector2 pos = NPC.Center + new Vector2(Main.rand.NextFloat(-NPC.width, NPC.width) * 0.7f, NPC.height * 0.3f);
                        Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15),
                            newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.105f, 0.25f)).customData = NPC.Center - new Vector2(0, 100);
                    }
                }
                if (AITimer == 300)
                    Projectile.NewProjectile(null, NPC.Center - new Vector2(6 * NPC.direction, 40), -Vector2.UnitY * 10, ProjectileType<GarbageGiantFlame>(), 20, 0, ai2: 1);
                if (AITimer > 100 && AITimer < 300 && AITimer % 20 == 0)
                {
                    EbonianSystem.ScreenShakeAmount = 5 * AITimer2;
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), -40), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), -40), new Vector2(NPC.direction * Main.rand.NextFloat(-6, 6) * AITimer2, -6 - Main.rand.NextFloat(3, 5) * AITimer2), ProjectileType<GarbageFlame>(), 15, 0);
                }
                if (AITimer == 5)
                {
                    SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
                }
                if (AITimer == 40)
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);

                if (AITimer == 60)
                {
                    SoundEngine.PlaySound(EbonianSounds.eruption.WithVolumeScale(0.8f), NPC.Center);
                    laserSlot = SoundEngine.PlaySound(EbonianSounds.garbageLaser.WithVolumeScale(1.35f), NPC.Center);
                    EbonianSystem.ScreenShakeAmount = 5;
                    AITimer2 = 1;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall1>(), 100, 0, ai0: NPC.whoAmI);
                }
                if (AITimer == 140)
                {
                    if (SoundEngine.TryGetActiveSound(laserSlot, out var sound))
                    {
                        sound.Pitch += 0.3f;
                        sound.Volume += 0.3f;
                    }
                    EbonianSystem.ScreenShakeAmount = 10;
                    AITimer2 = 1.5f;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall2>(), 100, 0, ai0: NPC.whoAmI);
                }
                if (AITimer == 200)
                {
                    if (SoundEngine.TryGetActiveSound(laserSlot, out var sound))
                    {
                        sound.Pitch += 0.4f;
                        sound.Volume += 0.4f;
                    }
                    EbonianSystem.ScreenShakeAmount = 15;
                    AITimer2 = 2.25f;
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall3>(), 100, 0, ai0: NPC.whoAmI);
                }
                if (AITimer > 200 && AITimer < 320)
                {
                    for (float i = 0; i < 0.99f; i += 0.33f)
                        Helper.DustExplosion(NPC.Center - new Vector2(6, NPC.height * 0.2f), Vector2.One, 2, Color.Gray * 0.1f, false, false, 0.1f, 0.125f, -Vector2.UnitY.RotatedByRandom(PiOver4 * i) * Main.rand.NextFloat(2f, 8f));
                }
                if (AITimer >= 360)
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.1f);
                if (AITimer >= 400)
                {

                    if (SoundEngine.TryGetActiveSound(laserSlot, out var sound))
                    {
                        sound.Stop();
                    }
                    AITimer3 = 0;
                    AITimer2 = 0;
                    AITimer = 0;
                    NPC.damage = 0;
                    AIState = Idle;
                    flameAlpha = 0;
                    didAttacks = true;
                    NextAttack = WarningForDash;
                    NPC.velocity = Vector2.Zero;
                }
            }
        }
        SlotId laserSlot;
        float flameAlpha;
    }
    public class HotGarbageNuke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            EbonianMod.projectileFinalDrawList.Add(Type);
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
        }
        Vector2 targetPos;
        float waveTimer, waveTimer2, vfxOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 497) return false;
            List<VertexPositionColorTexture> vertices = new();

            vfxOffset -= 0.015f;
            if (vfxOffset <= 0)
                vfxOffset = 1;
            vfxOffset = Clamp(vfxOffset, float.Epsilon, 1 - float.Epsilon);

            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float _mult = (1f - fadeMult * i);
                float mult = Lerp(1, 0, _mult);
                Color c = Color.Lerp(Color.Red, Color.Gray * _mult, mult * mult) * _mult * alpha;

                float rot = Projectile.oldPos[i - 1].FromAToB(Projectile.oldPos[i]).ToRotation();
                float __off = vfxOffset;
                if (__off > 1) __off = -__off + 1;
                float _off = __off + mult;
                if (Projectile.oldPos[i] == Vector2.Zero) break;
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + new Vector2(-14, 0).RotatedBy(Projectile.velocity.ToRotation()) + new Vector2(SmoothStep(20, 0, mult), 0).RotatedBy(rot + PiOver2) - Main.screenPosition, c, new Vector2(_off, 1)));
                vertices.Add(Helper.AsVertex(Projectile.oldPos[i] + Projectile.Size / 2 + new Vector2(-14, 0).RotatedBy(Projectile.velocity.ToRotation()) + new Vector2(SmoothStep(20, 0, mult), 0).RotatedBy(rot - PiOver2) - Main.screenPosition, c, new Vector2(_off, 0)));
            }
            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (vertices.Count > 2)
            {
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Helper.GetExtraTexture("wavyLaser2"), false);
            }
            Main.spriteBatch.ApplySaved();
            Texture2D pulse = Helper.GetExtraTexture("PulseCircle2");
            Texture2D ring = Helper.GetExtraTexture("crosslight");
            Texture2D ring2 = Helper.GetExtraTexture("Extras2/slash_06");
            Texture2D chevron = Helper.GetExtraTexture("chevron");
            Texture2D hazard = Helper.GetExtraTexture("hazardUnblurred");
            Texture2D textGlow = Helper.GetExtraTexture("textGlow");
            Texture2D circle = Helper.GetExtraTexture("explosion2");
            Texture2D exclamation = Helper.GetExtraTexture("exclamation");
            float _alpha = Utils.GetLerpValue(0, 2, waveTimer);
            float alpha2 = Clamp((float)Math.Sin(_alpha * Math.PI) * 1, 0, 1f);

            float chevron_alpha = Utils.GetLerpValue(0, 1, waveTimer2);
            float chevron_alpha2 = Clamp((float)Math.Sin(chevron_alpha * Math.PI) * 1, 0, 1f);
            if (Projectile.ai[0] > 60)
                waveTimer += 0.02f * (waveTimer.Safe() + (alpha2.Safe()));
            if (waveTimer > 2)
                waveTimer = 0;

            waveTimer2 += 0.019f * (waveTimer2.Safe());
            if (waveTimer2 > 1)
                waveTimer2 = 0;

            Color color = Color.Lerp(Color.Maroon, Color.Red, Lerp(0, 1, Clamp((Projectile.ai[0]) / 600, 0, 1)));
            Color color2 = Color.Lerp(Color.Black, Color.DarkRed, Lerp(0, 1, Clamp((Projectile.ai[0]) / 600, 0, 1)));
            if (targetPos != Vector2.Zero)
            {
                Main.spriteBatch.Draw(circle, targetPos - Main.screenPosition, null, Color.Black * Projectile.ai[2] * 0.4f, 0, circle.Size() / 2, 4.8f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(circle, targetPos - Main.screenPosition, null, color * chevron_alpha2 * 0.125f, Main.GameUpdateCount * -0.01f, circle.Size() / 2, 4.7f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(circle, targetPos - Main.screenPosition, null, color * chevron_alpha2 * 0.125f, Main.GameUpdateCount * 0.01f, circle.Size() / 2, 4.7f, SpriteEffects.None, 0);


                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Red * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 600) / 60f, 0, 1)));

                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(pulse, targetPos - Main.screenPosition, null, Color.DarkRed * Projectile.ai[2], 0, pulse.Size() / 2, 4.5f, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(ring, targetPos - Main.screenPosition, null, color2 * ((chevron_alpha2 * 0.5f) + SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))), 0, ring.Size() / 2, chevron_alpha * 10 + SmoothStep(0, 4, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1)), SpriteEffects.None, 0);

                //Main.spriteBatch.Draw(pulse, targetPos - Main.screenPosition, null, Color.Maroon * alpha2, 0, pulse.Size() / 2, waveTimer * 2, SpriteEffects.None, 0);
                if (chevronTimer2++ % 15 == 0)
                    chevronTimer++;
                for (int j = 1; j < 10; j++)
                {
                    chevronAlphas[j] = Lerp(chevronAlphas[j], 0f, 0.01f);
                    if (chevronAlphas[j].CloseTo(0, 0.05f)) chevronAlphas[j] = 0f;
                    if ((chevronTimer % 20 == j) && chevronTimer2 % 15 == 0)
                    {
                        chevronAlphas[j] = 0.6f;
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 16);
                        float scaleOff = MathF.Cos((j - Main.GlobalTimeWrappedHourly) * 20) * 0.1f;
                        Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height * 2) * Vector2.One, 0.01f) + ((j + 1) * 150) * Vector2.One;
                        Vector2 scale = new Vector2(Clamp(chevron_alpha2 + (float)(j - 1) / 5, 0, 1), MathF.Pow(Clamp(chevron_alpha2 + (float)(j - 1) / 5, 0, 1), 2)) * (0.5f + scaleOff);
                        Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, color2 * chevronAlphas[j] * Lerp(1, 0, Clamp(targetPos.Distance(targetPos + offset.RotatedBy(angle)) / (pulse.Height * 2.4f), 0, 1)), angle + PiOver4, chevron.Size() / 2, 0.75f, SpriteEffects.None, 0);
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 16) + Helper.CircleDividedEqually(1, 32);
                        float scaleOff = MathF.Cos((j - Main.GlobalTimeWrappedHourly) * 20) * 0.1f;
                        Vector2 offset = Vector2.Lerp(Vector2.Zero, (pulse.Height * 2) * Vector2.One, waveTimer2) + ((j - 1.5f) * 150) * Vector2.One;
                        Vector2 scale = new Vector2(Clamp(chevron_alpha2 + (float)(j - 1) / 5, 0, 1), MathF.Pow(Clamp(chevron_alpha2 + (float)(j - 1) / 5, 0, 1), 2)) * (0.5f + scaleOff);
                        Main.spriteBatch.Draw(chevron, targetPos + offset.RotatedBy(angle) - Main.screenPosition, null, color2 * chevron_alpha2 * Lerp(1, 0, Clamp(targetPos.Distance(targetPos + offset.RotatedBy(angle)) / (pulse.Height * 2.4f), 0, 2)), angle + PiOver4, chevron.Size() / 2, scale, SpriteEffects.None, 0);
                    }
                }
            }
            string num = MathF.Round(Projectile.ai[1] / 60, 2).ToString();
            switch (num.Length)
            {
                case 1:
                    num = MathF.Round(Projectile.ai[1] / 60, 2).ToString() + ".0";
                    break;
            }
            string strin = "" + num;
            if (extraString != "NUKE DETONATION IN: ")
                strin = "";

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Main.spriteBatch.SaveCurrent();


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            Main.spriteBatch.Draw(textGlow, new Vector2(Main.screenWidth / 2, Main.screenHeight * 0.05f), null, Color.Black * textAlpha, 0, new Vector2(textGlow.Width / 2, textGlow.Height / 2), 10, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(textGlow, new Vector2(Main.screenWidth / 2, Main.screenHeight - Main.screenHeight * 0.05f), null, Color.Black * textAlpha, 0, new Vector2(textGlow.Width / 2, textGlow.Height / 2), 10, SpriteEffects.None, 0);

            for (int i = -(int)(Main.screenWidth / hazard.Width); i < (int)(Main.screenWidth / hazard.Width); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Main.spriteBatch.Draw(hazard, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * hazard.Width) - Lerp(0, waveTimer, Clamp(Projectile.ai[0] / 120, 0, 1)) * hazard.Width - Main.screenWidth * hazardDistanceMult * 2, Main.screenHeight * 0.0325f), null, color * 2 * textAlpha, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(hazard, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * hazard.Width) + Lerp(0, waveTimer, Clamp(Projectile.ai[0] / 120, 0, 1)) * hazard.Width + Main.screenWidth * hazardDistanceMult * 2, Main.screenHeight * 0.122f), null, color * 2 * textAlpha, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);

                    Vector2 exPos = (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * exclamation.Width) + waveTimer * exclamation.Width * (i < 0 ? 1 : -1), Main.screenHeight * 0.078f);
                    Main.spriteBatch.Draw(exclamation, exPos, null, color * 2 * textAlpha * Lerp(0, 2, Clamp(MathF.Abs(exPos.X - Main.screenWidth / 2) / 5000, 0, 1)), 0, new Vector2(exclamation.Width / 2, exclamation.Height / 2), 0.1f, SpriteEffects.None, 0);
                }
            }

            for (int i = -(int)(Main.screenWidth / hazard.Width); i < (int)(Main.screenWidth / hazard.Width); i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Main.spriteBatch.Draw(hazard, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * hazard.Width) + Lerp(0, waveTimer, Clamp(Projectile.ai[0] / 120, 0, 1)) * hazard.Width + Main.screenWidth * hazardDistanceMult * 2, Main.screenHeight - Main.screenHeight * 0.0325f - 100), null, color * 2 * textAlpha, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(hazard, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * hazard.Width) - Lerp(0, waveTimer, Clamp(Projectile.ai[0] / 120, 0, 1)) * hazard.Width - Main.screenWidth * hazardDistanceMult * 2, Main.screenHeight - Main.screenHeight * 0.122f - 100), null, color * 2 * textAlpha, 0, new Vector2(hazard.Width / 2, hazard.Height / 2), 1, SpriteEffects.None, 0);

                    Vector2 exPos = (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 + (i * exclamation.Width) + waveTimer * exclamation.Width * (i < 0 ? 1 : -1), Main.screenHeight - Main.screenHeight * 0.078f - 100);
                    Main.spriteBatch.Draw(exclamation, exPos, null, color * 2 * textAlpha * Lerp(0, 2, Clamp(MathF.Abs(exPos.X - Main.screenWidth / 2) / 5000, 0, 1)), 0, new Vector2(exclamation.Width / 2, exclamation.Height / 2), 0.1f, SpriteEffects.None, 0);
                }
            }
            string warningText = "EVACUATE IMMEDIATELY";
            bool run = false;
            if (Main.LocalPlayer.Center.Distance(targetPos) < 4500 / 2 - 100)
            {
                if (leftAgain)
                    warningText = "This is NOT funny PLEASE GET OUT!";
                else if (left)
                {
                    warningText = "WHAT ARE YOU DOING!?";
                }
                else if (Projectile.ai[1] < 180)
                {
                    run = true;
                    warningText = "RUN";
                    for (int i = 0; i < (int)((Projectile.ai[0] - 480) / 4); i++)
                    {
                        warningText += " RUN";
                    }
                }
            }
            else
            {
                if (reentered)
                    warningText = "Phew...";
                else
                    warningText = "GOOD JOB!";
            }

            for (int j = 0; j < 2; j++)
            {
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, warningText, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(warningText).X / 2, Main.screenHeight - Main.screenHeight * 0.1f - 100), color * textAlpha);
                if (run)
                {
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, warningText, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(-Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(warningText).X / 2, Main.screenHeight * 0.055f), color * textAlpha);
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, warningText, (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth + Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString(warningText).X / 2, Main.screenHeight * 0.055f), color * textAlpha);
                }
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, extraString + strin, (Main.rand.NextVector2Circular(15, 15) * onNumberShakeMult * (numberScaleOff * 0.2f + 1)) + (Main.rand.NextVector2Circular(50, 50) * SmoothStep(0, 1, Clamp((Projectile.ai[0] - 500) / 180f, 0, 1))) + new Vector2(Main.screenWidth / 2 - FontAssets.DeathText.Value.MeasureString((extraString + "0.00").ToString()).X / 2, Main.screenHeight * 0.055f), color * textAlpha);
            }
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(ring2, Main.ScreenSize.ToVector2() / 2 - new Vector2(0, 100 + numberScaleOff * 30) + Main.rand.NextVector2Circular(60, 60) * numberAlpha, null, Color.Maroon * numberAlpha * 0.25f, 0, ring2.Size() / 2, 2.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(ring2, Main.ScreenSize.ToVector2() / 2 - new Vector2(0, 100 + numberScaleOff * 30) + Main.rand.NextVector2Circular(60, 60) * numberAlpha, null, Color.Maroon * numberAlpha * 0.25f, 0, ring2.Size() / 2, 2.5f, SpriteEffects.FlipHorizontally, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, number.ToString(), Main.ScreenSize.ToVector2() / 2 - new Vector2(0, 100 + numberScaleOff * 30) + Main.rand.NextVector2Circular(50, 50) * numberAlpha - new Vector2(FontAssets.DeathText.Value.MeasureString((number).ToString()).X / 2 * (15 / 2 + numberScaleOff), FontAssets.DeathText.Value.MeasureString((number).ToString()).Y / 2 * (10 / 2 + numberScaleOff)), Color.Red * numberAlpha * 0.05f, 0, new Vector2(0.5f), 15 / 2 + numberScaleOff, SpriteEffects.None, 0);

            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, number.ToString(), Main.ScreenSize.ToVector2() / 2 - new Vector2(0, 100 + numberScaleOff * 30) + Main.rand.NextVector2Circular(30, 30) * numberAlpha - new Vector2(FontAssets.DeathText.Value.MeasureString((number).ToString()).X / 2 * (15 / 4 + numberScaleOff), FontAssets.DeathText.Value.MeasureString((number).ToString()).Y / 2 * (10 / 4 + numberScaleOff)), Color.Red * numberAlpha, 0, new Vector2(0.5f), 15 / 4 + numberScaleOff, SpriteEffects.None, 0);


            Main.spriteBatch.ApplySaved();
            Main.EntitySpriteDraw(Helper.GetTexture(Texture), Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
        string extraString = "NUKE DETONATION IN: ";
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = 600;
            Projectile.rotation = -Vector2.UnitY.ToRotation();
            targetPos = Projectile.Center;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * alpha;
        }
        public override void Kill(int timeLeft)
        {
            EbonianSystem.TemporarilySetMusicTo0(600);
            SoundEngine.PlaySound(EbonianSounds.nuke);
            SoundEngine.PlaySound(EbonianSounds.garbageDeath);
            foreach (Player player in Main.player)
            {
                if (player.active)
                {
                    if (player.Center.Distance(targetPos) < 4500 / 2 - 100)
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " advocated for the legalization of nuclear bombs."), 999999, 0);
                }

            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.Center.Distance(targetPos) < 4500 / 2 - 100 && npc.type != NPCType<HotGarbage>() && npc.type != NPCType<ArchmageStaffNPC>())
                {
                    npc.life = 0;
                    npc.checkDead();
                }
                if (npc.active && npc.type == NPCType<HotGarbage>())
                {
                    npc.immortal = false;
                    npc.dontTakeDamage = false;
                    npc.StrikeInstantKill();
                }
            }
            EbonianMod.FlashAlpha = 1;
            //Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<ScreenFlash>(), 0, 0);
        }
        public override void PostDraw(Color lightColor)
        {

        }
        float alpha = 1, numberAlpha = 0, number, numberTimer, textAlpha, chevronTimer, chevronTimer2, numberScaleOff = -1f, hazardDistanceMult = 1, onNumberShakeMult;
        float[] chevronAlphas = new float[10];
        bool changedCam, left, reentered, leftAgain;
        public override void AI()
        {
            if (Projectile.ai[0] < 60)
                hazardDistanceMult = Lerp(1, 0, (Projectile.ai[0]) / 60);
            foreach (Player player in Main.player)
            {
                if (player.active)
                    if ((player.HeldItem.type == ItemID.MagicMirror || player.HeldItem.type == ItemID.RecallPotion) && player.itemAnimation > 2)
                    {
                        player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " thought they were smart."), 12345, 0);
                        Projectile.active = false;
                    }
            }
            textAlpha = Lerp(textAlpha, 1, 0.15f);
            if (--numberTimer < 0)
                numberAlpha = Lerp(numberAlpha, 0, 0.15f);

            onNumberShakeMult = Lerp(onNumberShakeMult, 0, 0.15f);
            if (Projectile.ai[1] % 60 == 0 && Projectile.ai[0] > 60)
            {
                numberScaleOff += 0.5f;
                onNumberShakeMult = 1;
                if (Projectile.ai[1] < 6 * 60)
                {
                    SoundEngine.PlaySound(EbonianSounds.buzz.WithPitchOffset((numberScaleOff + 1) * 0.04f));
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(targetPos, Helper.FromAToB(targetPos, Main.LocalPlayer.Center), 23, 10, 60, 2000));
                    numberAlpha = 1;
                    number = Projectile.ai[1] / 60;
                    numberTimer = 20;
                }
            }

            if (Projectile.ai[1] < 180 && Projectile.ai[1] > 60 && !changedCam && Main.LocalPlayer.Center.Distance(targetPos) > 4500 / 2)
            {
                EbonianSystem.ChangeCameraPos(targetPos, (int)Projectile.ai[1] + 10);
                changedCam = true;
            }
            if (Main.LocalPlayer.Center.Distance(targetPos) > 4500 / 2 - 100)
            {
                if (reentered) leftAgain = true;
                left = true;
            }
            else if (left)
                reentered = true;
            if (Projectile.ai[2] < 1f)
                Projectile.ai[2] += 0.05f;
            if (alpha < 0.1f)
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                    Projectile.oldPos[i] = Projectile.position;

            if (Projectile.ai[1] > 0 && Projectile.ai[0] > 50)
                Projectile.ai[1]--;
            if (Projectile.ai[1] <= 0 && Projectile.ai[0] > 50)
            {
                Projectile.Kill();
            }
            extraString = "NUKE DETONATION IN: ";
            Projectile.timeLeft = 2;
            float _alpha = Utils.GetLerpValue(0, 2, waveTimer);
            float alpha2 = Clamp((float)Math.Sin(_alpha * Math.PI) * 3, 0, 1f);

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 50)
            {
                Projectile.rotation = -Vector2.UnitY.ToRotation();
                Projectile.velocity.Y -= 0.5f + Projectile.velocity.Y * 0.01f;
            }
            else if (Projectile.ai[0] > 50 && Projectile.ai[0] < 540)
            {
                alpha = Lerp(alpha, 0, 0.1f);
                Projectile.velocity *= 0.9f;
            }
            else if (Projectile.ai[0] > 540)
            {
                if (Projectile.ai[0] == 481)
                    Projectile.Center = targetPos - new Vector2(0, 800);
                alpha = Lerp(alpha, 1, 0.1f);
                Projectile.velocity.Y += 0.3f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }
    }
}