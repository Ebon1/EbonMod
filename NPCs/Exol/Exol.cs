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
using System.Collections.Generic;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.PlayerDrawLayer;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Dusts;
using EbonianMod.Projectiles.Exol;
using System.Net.Http.Headers;
using EbonianMod.NPCs.Corruption;
using System.IO;

namespace EbonianMod.NPCs.Exol
{
    [AutoloadBossHead]
    public class Exol : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            //Main.npcFrameCount[Type] = 8;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "EbonianMod/NPCs/Exol/Exol_bestiary",
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Type: Unknown"),
                new FlavorTextBestiaryInfoElement("A boiling, blazing monster who terrorizes hell through sheer spite and rage. Its purpose and origin are unclear and most are content not knowing."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 134;
            NPC.height = 148;
            NPC.damage = 0;
            NPC.defense = 20;
            NPC.lifeMax = 25000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Exol");
            }
        }
        float sunAlpha = 0, sunScale = 1, flameAlpha = 0, flameScale = 1;
        int flameY;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D sun = Helper.GetExtraTexture("Sun");
            Texture2D flame = Helper.GetExtraTexture("flameEyeSheet");
            spriteBatch.Reload(BlendState.Additive);
            if (sunAlpha > 0)
                spriteBatch.Draw(sun, NPC.Center + new Vector2(4, 5) - screenPos, null, Color.White * sunAlpha, Main.GameUpdateCount * 0.03f, sun.Size() / 2, 0.5f * sunScale, SpriteEffects.None, 0);

            if (flameAlpha > 0)
            {
                if (AITimer % 2 == 0 && !Main.gameInactive)
                    flameY++;
                if (flameY > 14)
                    flameY = 0;
                spriteBatch.Draw(flame, NPC.Center + new Vector2(4, 5) - screenPos, new Rectangle(0, 512 * flameY, 512, 512), Color.White * flameAlpha, -Main.GameUpdateCount * 0.01f, new Vector2(512, 512) / 2, 0.65f * flameScale, SpriteEffects.None, 0);
            }
            spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D a = Helper.GetTexture("NPCs/Exol/Exol_Pulse");
            Texture2D b = TextureAssets.Npc[Type].Value;
            Texture2D c = Helper.GetExtraTexture("Sprites/Exol_Glow");
            Texture2D d = Helper.GetTexture("NPCs/Exol/Exol_eye");
            Texture2D e = Helper.GetTexture("NPCs/Exol/Exol_bestiary");
            if (!NPC.hide)
            {
                Player player = Main.player[NPC.target];
                var fadeMult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[NPC.type]; i++)
                {
                    //Color color = Color.Lerp(Color.OrangeRed, Color.White, 1f - fadeMult * i);
                    Main.spriteBatch.Draw(b, NPC.oldPos[i] - Main.screenPosition + new Vector2(NPC.width / 2f, NPC.height / 2f), null, drawColor * ((1f - fadeMult * i) * 0.75f), NPC.rotation, b.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
                }
                Main.EntitySpriteDraw(b, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, b.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.Additive);
                Main.EntitySpriteDraw(c, NPC.Center - Main.screenPosition, null, Color.White * 0.75f, NPC.rotation, c.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.AlphaBlend);
                Vector2 ogEyePos = NPC.Center + new Vector2(4, 5);
                Vector2 eyePos = ogEyePos;
                Vector2 fromTo = Helper.FromAToB(ogEyePos, pointOfInterest);

                //forced point of interest, remove later once ai is finished.
                //if (AIState != Death)


                float dist = MathHelper.Clamp(Helper.FromAToB(ogEyePos, pointOfInterest, false).Length() * 0.1f, 0, 11);
                float distY = MathHelper.Clamp(Helper.FromAToB(ogEyePos, pointOfInterest, false).Length() * 0.1f, 0, 2);
                //if (AIState != Death)
                {
                    eyePos.X += dist * fromTo.X;
                    eyePos.Y += distY * fromTo.Y;
                }
                /*else
                {
                    eyePos += new Vector2(Main.rand.NextFloat(-11, 11), Main.rand.NextFloat(-2, 2));
                }*/
                Main.EntitySpriteDraw(d, eyePos - Main.screenPosition, null, Color.White, NPC.rotation, d.Size() / 2, 1, SpriteEffects.None, 0);
            }
            if (AIState == Death)
            {
                spriteBatch.Reload(BlendState.Additive);
                if (!NPC.hide)
                    for (int i = 0; i < 5; i++)
                        Main.EntitySpriteDraw(a, NPC.Center - Main.screenPosition, null, Color.White * deathAlpha, NPC.rotation, a.Size() / 2, 1, SpriteEffects.None, 0);

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            return false;
        }
        float deathAlpha;
        bool ded;
        Vector2 pointOfInterest;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(pointOfInterest);
            writer.Write(sunAlpha);
            writer.Write(sunScale);
            writer.Write(flameAlpha);
            writer.Write(flameScale);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            pointOfInterest = reader.ReadVector2();
            sunAlpha = reader.ReadSingle();
            sunScale = reader.ReadSingle();
            flameAlpha = reader.ReadSingle();
            flameScale = reader.ReadSingle();
        }
        public override bool CheckDead()
        {
            if (NPC.life <= 0 && !ded)
            {
                NPC.life = 1;
                AIState = Death;
                NPC.frameCounter = 0;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                EbonianSystem.ChangeCameraPos(NPC.Center, 300);
                EbonianSystem.ScreenShakeAmount = 20;
                ded = true;
                AITimer = AITimer2 = 0;
                NPC.velocity = Vector2.Zero;
                NPC.life = 1;
                return false;
            }
            return true;
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
        const int Death = -1, Spawn = 0, Geyser = 1, RockFall = 2, DashFireSpiral = 3, ThePowerOfTheSun = 4, EyeSpin = 5, OffScreenMeteorDash = 6, HomingSkulls = 7;
        Vector2 lastPos;
        SoundStyle summon = new("EbonianMod/Sounds/ExolSummon");
        SoundStyle roar = new("EbonianMod/Sounds/ExolRoar")
        {
            PitchVariance = 0.25f,
        };
        SoundStyle dash = new("EbonianMod/Sounds/ExolDash")
        {
            PitchVariance = 0.25f,
        };
        void LookAtPlayer() => pointOfInterest = Vector2.SmoothStep(pointOfInterest, Main.player[NPC.target].Center, 0.5f);
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            PlayerDetectionAndSteamVFX();
            AITimer++;
            switch (AIState)
            {
                case Death:
                    {

                        if (AITimer == 180)
                        {

                            Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 0, 0);
                        }
                        if (AITimer == 200)
                        {

                            EbonianMod.FlashAlpha = 1;
                            //Projectile a = Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ScreenFlash>(), 0, 0);
                            //a.ai[0] = 1;
                        }
                        if (AITimer < 40)
                        {
                            pointOfInterest = NPC.Center;
                        }
                        else if (AITimer < 150 & AITimer > 40)
                        {
                            pointOfInterest = NPC.Center + Main.rand.NextVector2CircularEdge(250, 250);
                        }
                        else
                        {

                            pointOfInterest = NPC.Center + Vector2.UnitY * 100;
                        }
                        if (AITimer > 212)
                        {
                            NPC.hide = true;
                        }
                        if (AITimer == 312)
                        {

                            NPC.immortal = false;
                            NPC.life = 0;
                            NPC.checkDead();
                        }
                        if (AITimer > 50)
                        {
                            if (deathAlpha < 1)
                                deathAlpha += 0.005f;
                            if (Main.rand.NextBool(deathAlpha < 0.5 ? 5 : 2) && !NPC.hide)
                            {
                                Vector2 pos = NPC.Center + 300 * Main.rand.NextVector2Unit();
                                Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.Gold * deathAlpha, Scale: Main.rand.NextFloat(0.15f, 0.35f));
                                a.noGravity = false;
                                a.customData = 1;
                            }
                        }
                        //if (AITimer == 190)
                        //  EbonianPlayer.Instance.FlashScreen(NPC.Center, 35);
                    }
                    break;
                case Spawn:
                    {

                        pointOfInterest = NPC.Center;
                        if (AITimer == 1)
                        {
                            //Helper.SetBossTitle(180, "Exol", Color.OrangeRed);
                            EbonianSystem.ChangeCameraPos(NPC.Center, 100);
                            SoundEngine.PlaySound(summon);
                            EbonianSystem.ScreenShakeAmount = 15f;
                        }
                        if (AITimer >= 150)
                        {
                            Reset();
                            AIState = Geyser;
                        }
                    }
                    break;
                case Geyser:
                    {
                        if (AITimer == 20)
                        {
                            NPC.noTileCollide = false;
                            NPC.velocity = Vector2.UnitY * 30;
                        }
                        if ((NPC.collideY || NPC.collideX || NPC.Center.Y > Main.maxTilesX * 16 || Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, NPC.height * 2) < NPC.height) && AITimer2 == 0)
                        {
                            AITimer2 = 1;
                            EbonianSystem.ScreenShakeAmount = 10;
                            NPC.velocity = -Vector2.UnitY * 3.5f;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EGeyser>(), 30, 0);
                        }
                        if (AITimer2 == 1 && AITimer < 100)
                            NPC.velocity *= 0.9f;
                        LookAtPlayer();
                        if (AITimer == 1)
                            SoundEngine.PlaySound(roar);
                        if (AITimer < 20 || AITimer > 100)
                        {
                            NPC.noTileCollide = true;
                            IdleMovement();
                        }
                        /*if (AITimer % 20 == 0 && AITimer > 40 && AITimer2 == 1)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y + Main.screenHeight), -Vector2.UnitY * Main.rand.NextFloat(4, 8), ModContent.ProjectileType<EBoulder>(), 30, 0);
                        }*/
                        if (AITimer >= 159)
                        {
                            Reset();
                            AIState = DashFireSpiral;
                        }
                    }
                    break;
                case RockFall:
                    {
                        if (AITimer == 1)
                            SoundEngine.PlaySound(roar);
                        if (AITimer < 50)
                        {
                            pointOfInterest = Vector2.SmoothStep(pointOfInterest, NPC.Center - Vector2.UnitY * 500, 0.5f);
                        }
                        if (AITimer == 30)
                        {
                            NPC.noTileCollide = false;
                            NPC.velocity = new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 5, -30);
                        }
                        if ((NPC.collideY || NPC.collideX || NPC.Center.Y < Main.UnderworldLayer * 16 || Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height * 2) < NPC.height) && AITimer2 == 0)
                        {
                            AITimer2 = 1;
                            EbonianSystem.ScreenShakeAmount = 10;
                            NPC.velocity = Vector2.UnitY * 7.5f;
                        }
                        if (AITimer > 60)
                        {
                            LookAtPlayer();
                            NPC.velocity *= 0.9f;
                        }
                        if (AITimer % 6 == 0 && AITimer > 60 && AITimer < 180 && AITimer2 == 1)
                        {
                            Vector2 pos = new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y - 300);
                            if (Main.rand.NextBool(8))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X, pos.Y), Vector2.UnitY * 5f, ModContent.ProjectileType<EBoulder2>(), 30, 0);
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X, pos.Y), Vector2.UnitY * 1, ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY * 2f, ModContent.ProjectileType<EBoulder2>(), 30, 0);
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY * 1, ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                            }
                        }
                        if (AITimer >= 200)
                        {
                            Reset();
                            AIState = ThePowerOfTheSun;
                        }
                    }
                    break;
                case DashFireSpiral:
                    {
                        if (AITimer < 50)
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 1, 0.3f);
                        NPC.noTileCollide = false;
                        if (AITimer > 40)
                            AITimer2++;
                        if (AITimer3 < 3)
                        {
                            pointOfInterest = NPC.Center + NPC.velocity * 10;
                            if (AITimer2 == 39)
                                NPC.velocity = Vector2.Zero;
                            if (AITimer2 == 40)
                            {
                                NPC.damage = 50;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave>(), 0, 0);
                                lastPos = player.Center;
                            }
                            if ((AITimer2 > 50 && AITimer2 < 55) && AITimer2 <= 40)
                                NPC.velocity *= 0.95f;
                            if (AITimer2 == 55)
                            {
                                SoundEngine.PlaySound(dash, NPC.Center);
                                for (int i = 0; i < 6 + AITimer3; i++)
                                {
                                    float angle = Helper.CircleDividedEqually(i, 6 + AITimer3);
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(7, 0).RotatedBy(angle), ModContent.ProjectileType<ExolFireSpiral>(), 25, 0);
                                }
                            }
                            if (AITimer2 > 55 && AITimer2 < 75)
                                NPC.velocity += Helper.FromAToB(NPC.Center, lastPos) * 1.9f;
                            if (AITimer2 >= 80)
                            {
                                AITimer2 = 0;
                                AITimer3++;
                            }
                            if (NPC.Center.Y > (Main.maxTilesY - 45) * 16 || NPC.Center.Y < 45 * 16 && AITimer2 > 70 || NPC.collideX || NPC.collideY)
                                NPC.velocity = -NPC.velocity * 0.2f;
                        }
                        else
                        {
                            AITimer3++;
                            LookAtPlayer();
                            NPC.velocity *= 0.95f;
                        }
                        if (AITimer3 > 5)
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 0, 0.1f);
                        if (AITimer3 >= 80)
                        {
                            Reset();
                            AIState = EyeSpin;
                        }
                    }
                    break;
                case ThePowerOfTheSun:
                    {
                        if (AITimer < 60)
                            pointOfInterest = NPC.Center;
                        else
                            LookAtPlayer();
                        if (AITimer < 20)
                            IdleMovement();
                        if (AITimer == 30)
                        {
                            NPC.velocity = Vector2.Zero;
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center) * 15, ModContent.ProjectileType<ESun>(), 30, 0, player.whoAmI);
                        }
                        if (AITimer > 400)
                        {
                            Reset();
                            AIState = OffScreenMeteorDash;
                        }
                    }
                    break;
                case EyeSpin:
                    {
                        AITimer3 += MathHelper.ToRadians(5);
                        if (AITimer < 100)
                        {
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 6;
                            pointOfInterest = NPC.Center + new Vector2(0, 100).RotatedBy(AITimer3);
                        }
                        else
                        {
                            NPC.velocity *= 0.97f;
                            LookAtPlayer();
                        }

                        if (AITimer % 10 == 0 && AITimer < 100)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, 100).RotatedBy(AITimer3), Helper.FromAToB(NPC.Center, player.Center) * 3, ModContent.ProjectileType<EFire2>(), 30, 0, player.whoAmI).timeLeft = 290 + (int)AITimer;

                        if (AITimer > 130)
                        {
                            Reset();
                            AIState = RockFall;
                        }
                    }
                    break;
                case OffScreenMeteorDash:
                    {
                        if (AITimer3 == 2)
                            AITimer++;
                        if (AITimer == 1)
                        {
                            if (player.Center.X < (Main.maxTilesX * 16) / 2)
                                AITimer2 = 1;
                            else
                                AITimer2 = -1;
                        }

                        if (AITimer > 50 && AITimer < 100)
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 1, 0.3f);
                        if (AITimer < 50)
                        {
                            LookAtPlayer();
                            if (AITimer3 == 0)
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center + new Vector2(1200 * AITimer2, 0)) * 25;
                            else
                            {
                                NPC.velocity *= 0.9f;
                            }
                        }
                        if (AITimer == 60)
                        {
                            NPC.damage = 50;
                            NPC.velocity = Vector2.Zero;
                            lastPos = player.Center;
                        }
                        if (AITimer > 70)
                        {
                            pointOfInterest = NPC.Center + NPC.velocity * 10;
                            if (AITimer % 5 == 0 && AITimer3 < 30)
                            {
                                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2);
                                if (Main.rand.NextBool(2))
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Helper.FromAToB(NPC.Center, pos) * 5, ModContent.ProjectileType<ExolFireExplode>(), 30, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<ExolFireExplode>(), 30, 0);
                            }
                            if (AITimer < 135)
                            {
                                if (NPC.velocity.Length() < 35)
                                    NPC.velocity += Helper.FromAToB(NPC.Center, lastPos + (AITimer3 == 0 ? new Vector2(1000 * -AITimer2, 0) : Vector2.Zero)) * (AITimer3 == 2 ? 2 : 1);
                            }
                            else
                            {
                                AITimer3++;
                                if (AITimer3 < 3)
                                    AITimer = -1;
                                flameAlpha = MathHelper.SmoothStep(flameAlpha, 0, 0.1f);
                            }
                        }
                        if (AITimer3 >= 3)
                        {
                            NPC.velocity *= 0.9f;
                            AITimer3++;
                        }
                        if (AITimer > 200 && AITimer3 > 60 && flameAlpha < 0.05f)
                        {
                            IdleMovement();
                            LookAtPlayer();
                        }
                        if (AITimer > 230)
                        {
                            Reset();
                            AIState = HomingSkulls;
                        }
                    }
                    break;
                case HomingSkulls:
                    {
                        if (AITimer < 30)
                        {
                            Vector2 pos = NPC.Center + 300 * Main.rand.NextVector2Unit();
                            Dust a = Dust.NewDustPerfect(pos, ModContent.DustType<GenericAdditiveDust>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.05f, 0.15f));
                            a.noGravity = false;
                            a.customData = 1;
                        }
                        if (AITimer > 60)
                            IdleMovement();
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave>(), 0, 0);
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 vel = Main.rand.NextVector2Unit() * 5;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ESkullEmoji>(), 30, 0);
                            }
                        }
                        if (AITimer > 240)
                        {
                            //Reset();
                            //AIState = HomingSkulls;
                        }
                    }
                    break;
            }
        }
        void Reset()
        {
            NPC.noTileCollide = true;
            NPC.rotation = 0;
            NPC.velocity.X = 0;
            NPC.velocity.Y = 0;
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            sunAlpha = 0f;
            flameAlpha = 0f;
            NPC.damage = 0;
        }
        void IdleMovement()
        {
            Player player = Main.player[NPC.target];
            NPC.velocity *= 0.975f;
            NPC.velocity += Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200).RotatedBy(MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, (float)(Math.Sin(AITimer / 25) / 2) + 0.5f)));
        }
        void PlayerDetectionAndSteamVFX()
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

            if (Main.rand.NextBool(5))
                if (NPC.life < NPC.lifeMax / 4)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.45f, false, false, 0.6f, 0.5f, new(Main.rand.NextFloat(-4, 4), -10));
                }
                else if (NPC.life < NPC.lifeMax / 3)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.35f, false, false, 0.4f, 0.5f, -Vector2.UnitY * Main.rand.NextFloat(6, 8));
                }
                else if (NPC.life < NPC.lifeMax / 2)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.25f, false, false, 0.2f, 0.5f, -Vector2.UnitY * Main.rand.NextFloat(4, 8));
                }
        }
    }
}