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
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
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
            NPC.defense = 30;
            NPC.lifeMax = 35000;
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
                Main.EntitySpriteDraw(b, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, b.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.Additive);
                Main.EntitySpriteDraw(c, NPC.Center - Main.screenPosition, null, Color.White * 0.25f, NPC.rotation, c.Size() / 2, 1, SpriteEffects.None, 0);
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
        public float AITimer4;
        SoundStyle dash = EbonianSounds.exolDash with
        {
            PitchVariance = 0.25f,
        };
        const int Death = -1, Spawn = 0, Geyser = 1, RockFall = 2, DashFireSpiral = 3, ThePowerOfTheSun = 4, EyeSpin = 5, OffScreenMeteorDash = 6, HomingSkulls = 7, LaserWalls = 8, Pentagram = 9, SeethingChaos = 10, SolarRays = 11;
        Vector2 lastPos;
        void LookAtPlayer() => pointOfInterest = Vector2.SmoothStep(pointOfInterest, Main.player[NPC.target].Center, 0.5f);
        /// <summary>
        /// Default0: Geyser, Spirals, Eye Spin, Rocks, Sun, Dash, Seethe, Skulls, Lasers, Pentagram, Solar Rays.
        /// Variant1: Spirals, Pentagram, Rocks, Geyser, Eye Spin, Skulls, Dash, Seethe, Sun, Solar Rays, Lasers.  
        /// Variant2: Dash, Skulls, Pentagram, Geyser, Solar Rays, Eye Spin, Sun, Spirals, Seethe, Lasers, Rocks.
        /// Variant3: Eye Spin, Dash, Pentagram, Skulls, Solar Rays, Rocks, Geyser, Sun, Seethe, Spirals, Lasers. 
        /// </summary>
        public int AttackPattern;
        void StartNewPattern(int pattern)
        {
            AttackPattern = pattern;
            switch (pattern)
            {
                case 0:
                    AIState = Geyser;
                    break;
                case 1:
                    AIState = DashFireSpiral;
                    break;
                case 2:
                    AIState = OffScreenMeteorDash;
                    break;
                case 3:
                    AIState = EyeSpin;
                    break;
            }
        }
        void NextAttack()
        {
            switch (AIState)
            {
                case 1:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = DashFireSpiral;
                            break;
                        case 1:
                            AIState = EyeSpin;
                            break;
                        case 2:
                            AIState = SolarRays;
                            break;
                        case 3:
                            AIState = ThePowerOfTheSun;
                            break;
                    }
                    break;
                case 2:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = ThePowerOfTheSun;
                            break;
                        case 1:
                            AIState = Geyser;
                            break;
                        case 2:
                            StartNewPattern(3);
                            break;
                        case 3:
                            AIState = Geyser;
                            break;
                    }
                    break;
                case 3:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = EyeSpin;
                            break;
                        case 1:
                            AIState = Pentagram;
                            break;
                        case 2:
                            AIState = SeethingChaos;
                            break;
                        case 3:
                            AIState = LaserWalls;
                            break;
                    }
                    break;
                case 4:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = OffScreenMeteorDash;
                            break;
                        case 1:
                            AIState = SolarRays;
                            break;
                        case 2:
                            AIState = DashFireSpiral;
                            break;
                        case 3:
                            AIState = SeethingChaos;
                            break;
                    }
                    break;
                case 5:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = RockFall;
                            break;
                        case 1:
                            AIState = HomingSkulls;
                            break;
                        case 2:
                            AIState = ThePowerOfTheSun;
                            break;
                        case 3:
                            AIState = OffScreenMeteorDash;
                            break;
                    }
                    break;
                case 6:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = SeethingChaos;
                            break;
                        case 1:
                            AIState = SeethingChaos;
                            break;
                        case 2:
                            AIState = HomingSkulls;
                            break;
                        case 3:
                            AIState = Pentagram;
                            break;
                    }
                    break;
                case 7:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = LaserWalls;
                            break;
                        case 1:
                            AIState = OffScreenMeteorDash;
                            break;
                        case 2:
                            AIState = Pentagram;
                            break;
                        case 3:
                            AIState = SolarRays;
                            break;
                    }
                    break;
                case 8:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = Pentagram;
                            break;
                        case 1:
                            StartNewPattern(2);
                            break;
                        case 2:
                            AIState = RockFall;
                            break;
                        case 3:
                            StartNewPattern(0);
                            break;
                    }
                    break;
                case 9:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = SolarRays;
                            break;
                        case 1:
                            AIState = RockFall;
                            break;
                        case 2:
                            AIState = Geyser;
                            break;
                        case 3:
                            AIState = HomingSkulls;
                            break;
                    }
                    break;
                case 10:
                    switch (AttackPattern)
                    {
                        case 0:
                            AIState = HomingSkulls;
                            break;
                        case 1:
                            AIState = ThePowerOfTheSun;
                            break;
                        case 2:
                            AIState = LaserWalls;
                            break;
                        case 3:
                            AIState = DashFireSpiral;
                            break;
                    }
                    break;
                case 11:
                    switch (AttackPattern)
                    {
                        case 0:
                            StartNewPattern(1);
                            break;
                        case 1:
                            AIState = LaserWalls;
                            break;
                        case 2:
                            AIState = EyeSpin;
                            break;
                        case 3:
                            AIState = RockFall;
                            break;
                    }
                    break;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            CombatText.NewText(Main.LocalPlayer.getRect(), Color.OrangeRed, "Infinite Flight!", true);
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            PlayerDetectionAndSteamVFX();
            player.wingTime = player.wingTimeMax;
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
                        if (AITimer2 == 1 && AITimer < 50)
                            NPC.velocity *= 0.9f;
                        LookAtPlayer();
                        if (AITimer < 20 || AITimer > 50 && AITimer2 == 1)
                        {
                            NPC.noTileCollide = true;
                            IdleMovement(true);
                        }
                        /*if (AITimer % 20 == 0 && AITimer > 40 && AITimer2 == 1)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y + Main.screenHeight), -Vector2.UnitY * Main.rand.NextFloat(4, 8), ModContent.ProjectileType<EBoulder>(), 30, 0);
                        }*/
                        if (AITimer >= 80)
                        {
                            Reset();
                            //AIState = DashFireSpiral;
                            NextAttack();
                        }
                    }
                    break;
                case RockFall:
                    {
                        if (AITimer < 50)
                        {
                            pointOfInterest = Vector2.SmoothStep(pointOfInterest, NPC.Center - Vector2.UnitY * 500, 0.5f);
                        }
                        if (AITimer == 30)
                        {
                            NPC.noTileCollide = false;
                            NPC.velocity = new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 5, -30);
                        }
                        if (NPC.Center.Y < Main.maxTilesY * 8 && (NPC.collideY || NPC.collideX || NPC.Center.Y < Main.UnderworldLayer * 16 || Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height * 2) < NPC.height) && AITimer2 == 0)
                        {
                            AITimer2 = 1;
                            EbonianSystem.ScreenShakeAmount = 10;
                            NPC.velocity = Vector2.UnitY * 7.5f;
                        }
                        if (AITimer > 60)
                        {
                            LookAtPlayer();
                            IdleMovement();
                        }
                        if (AITimer % 7 == 0 && AITimer > 60 && AITimer < 200 && AITimer2 == 1)
                        {
                            Vector2 pos = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y - 300);
                            if (Main.rand.NextBool(8))
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X, pos.Y), Vector2.UnitY * 2, ModContent.ProjectileType<EBoulder2>(), 15, 0);
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X, pos.Y), Vector2.UnitY * 1, ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(1, 2)), ModContent.ProjectileType<EBoulder2>(), 15, 0);
                                //Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY * 1, ModContent.ProjectileType<TelegraphLine>(), 0, 0);
                            }
                        }
                        if (AITimer >= 250)
                        {
                            Reset();
                            //AIState = ThePowerOfTheSun;
                            NextAttack();
                        }
                    }
                    break;
                case DashFireSpiral:
                    {
                        if (AITimer < 50)
                        {
                            IdleMovement(true);
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 1, 0.3f);
                        }
                        NPC.noTileCollide = true;
                        if (AITimer > 40)
                            AITimer2++;
                        if (AITimer3 < 4)
                        {
                            pointOfInterest = NPC.Center + NPC.velocity * 10;
                            if (AITimer2 == 39)
                                NPC.velocity = Vector2.Zero;
                            if (AITimer2 == 40)
                            {
                                NPC.damage = 50;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave>(), 0, 0);
                                lastPos = Helper.FromAToB(NPC.Center, player.Center);
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
                                NPC.velocity += lastPos * 1.9f;
                            if (AITimer2 >= 80)
                            {
                                AITimer4 = 0;
                                AITimer2 = 0;
                                AITimer3++;
                            }
                            if (AITimer2 > 70 && AITimer4 == 1)
                                NPC.velocity *= 0.99f;
                            bool collision = false;
                            if (NPC.velocity.Y > 0)
                                collision = NPC.Center.Y > (Main.maxTilesY - 45) * 16;
                            else
                                collision = NPC.Center.Y < 45 * 16;
                            if (collision && AITimer4 == 0)
                            {
                                NPC.velocity = -NPC.velocity.ToRotation().ToRotationVector2() * 10;
                                AITimer4 = 1;
                            }
                        }
                        else
                        {
                            AITimer3++;
                            LookAtPlayer();
                            NPC.velocity *= 0.95f;
                        }
                        if (AITimer3 > 5)
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 0, 0.1f);
                        if (AITimer3 >= 40)
                        {
                            Reset();
                            //AIState = EyeSpin;
                            NextAttack();
                        }
                    }
                    break;
                case ThePowerOfTheSun:
                    {
                        if (AITimer < 20)
                        {
                            NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(NPC.Center.X, (Main.maxTilesY * 16) / 2), 0.2f);
                        }
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
                        if (AITimer > (AttackPattern == 0 ? 300 : 270))
                        {
                            Reset();
                            //AIState = OffScreenMeteorDash;
                            NextAttack();
                        }
                    }
                    break;
                case EyeSpin:
                    {
                        AITimer3 += MathHelper.ToRadians(5 + (AITimer * 0.1f));
                        if (AITimer < 60)
                        {
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 1, 0.3f);
                            if (flameAlpha > 0.75f)
                                NPC.damage = 50;
                            if (AITimer < 20)
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 3;
                            else
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * -6;
                            pointOfInterest = NPC.Center + new Vector2(0, 100).RotatedBy(AITimer3);
                        }
                        else
                        {
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 0, 0.3f);
                            NPC.velocity *= 0.97f;
                            LookAtPlayer();
                        }

                        if (AITimer % 2 == 0 && AITimer < 60 && AITimer > 30)
                        {
                            if (AITimer % 10 == 0)
                                Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, 100).RotatedBy(AITimer3), Helper.FromAToB(NPC.Center, player.Center) * 3, ModContent.ProjectileType<EFire2>(), 15, 0, player.whoAmI).timeLeft = 290 + (int)AITimer;

                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center + new Vector2(0, 50).RotatedBy(AITimer3), new Vector2(0, 1).RotatedBy(AITimer3) * 4, ModContent.ProjectileType<EFire>(), 15, 0, player.whoAmI).timeLeft = 290 + (int)AITimer;
                        }

                        if (AITimer > 80)
                        {
                            Reset();
                            //AIState = RockFall;
                            NextAttack();
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
                                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center + new Vector2(1200 * AITimer2, 0), false) * 0.04f;
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
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Helper.FromAToB(NPC.Center, pos) * 5, ModContent.ProjectileType<ExolFireExplode>(), 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<ExolFireExplode>(), 0, 0);
                            }
                            if (AITimer < 135)
                            {
                                if (AITimer == 80)
                                    SoundEngine.PlaySound(dash, NPC.Center);
                                if (NPC.velocity.Length() < 35)
                                    NPC.velocity += Helper.FromAToB(NPC.Center, lastPos + (AITimer3 == 0 ? new Vector2(1000 * -AITimer2, 0) : Vector2.Zero)) * (AITimer3 == 2 ? 2 : 1);
                            }
                            else
                            {
                                lastPos = player.Center;
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
                            //AIState = SeethingChaos;
                            NextAttack();
                        }
                    }
                    break;
                case (SeethingChaos):
                    {
                        AITimer2 = Main.rand.NextFloat(-60, 60);
                        if (AITimer < 20)
                        {
                            NPC.Center = Vector2.Lerp(NPC.Center, new Vector2(NPC.Center.X, (Main.maxTilesY * 16) / 2), 0.2f);
                        }
                        if (AITimer == 1)
                        {
                            Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<QuickFlare>(), 0, 0);
                        }
                        LookAtPlayer();
                        if (AITimer % 5 == 0 && (AITimer >= 20 && AITimer <= 80))
                        {
                            Vector2 vel = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), AITimer % 10 == 0 ? -1 : 1).RotatedBy(MathHelper.ToRadians(AITimer2));
                            NPC.velocity = -vel * 8;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel * 0.5f, ModContent.ProjectileType<EMine>(), 0, 0);
                        }
                        NPC.velocity *= 0.9f;
                        if (AITimer > 110)
                        {
                            Reset();
                            //AIState = HomingSkulls;
                            NextAttack();
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
                            pointOfInterest = NPC.Center;
                        }
                        if (AITimer > 60)
                        {
                            LookAtPlayer();
                            IdleMovement();
                        }
                        if (AITimer == 40)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<InferosShockwave>(), 0, 0);
                            for (int i = 0; i < 8; i++)
                            {
                                Vector2 vel = Main.rand.NextVector2Unit() * 5;
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel, ModContent.ProjectileType<ESkullEmoji>(), 15, 0);
                            }
                        }
                        if (AITimer > 290)
                        {
                            Reset();
                            //AIState = LaserWalls;
                            NextAttack();
                        }
                    }
                    break;
                case LaserWalls:
                    {
                        LookAtPlayer();
                        AITimer2 *= 0.85f;
                        if (AITimer == 1 || AITimer == 100)
                            lastPos = player.Center;
                        if (AITimer < 100 && AITimer > 1)
                            NPC.velocity = Helper.FromAToB(NPC.Center, lastPos + new Vector2(-1200 + (AITimer * 20), 150 + AITimer2), false) * 0.2f;
                        else if (AITimer > 100 && AITimer < 200)
                            NPC.velocity = Helper.FromAToB(NPC.Center, lastPos + new Vector2(1200 - ((AITimer - 100) * 20), -150 - AITimer2), false) * 0.2f;
                        else
                            NPC.velocity *= 0.95f;
                        if (AITimer > 25 && AITimer <= 200 && AITimer % 5 == 0)
                        {
                            if (AITimer % 15 == 0)
                                AITimer2 = 50;
                            if (AITimer > 25 && AITimer <= 100)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -Vector2.UnitY, ModContent.ProjectileType<EFire>(), 20, 0);
                            if (AITimer > 125 && AITimer <= 200)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(60, 0), Vector2.UnitY, ModContent.ProjectileType<EFire>(), 20, 0);
                        }
                        if (AITimer > 200)
                            IdleMovement();
                        if (AITimer > 240)
                        {
                            Reset();
                            //AIState = Pentagram;
                            NextAttack();
                        }
                    }
                    break;
                case Pentagram:
                    {
                        LookAtPlayer();
                        if (AITimer > 40)
                            flameAlpha = MathHelper.SmoothStep(flameAlpha, 1, 0.3f);
                        if (flameAlpha > 0.5f)
                            NPC.damage = 40;
                        if (AITimer < 100)
                            NPC.velocity += Helper.FromAToB(NPC.Center, new Vector2(player.Center.X, (Main.maxTilesY * 16) / 2)) / 2;
                        if (AITimer > 100)
                        {
                            NPC.velocity = Vector2.Clamp(Helper.FromAToB(NPC.Center, new Vector2(player.Center.X, (Main.maxTilesY * 16) / 2), false) * 0.02f, -Vector2.One * 4, Vector2.One * 4);
                        }
                        if (AITimer > 50 && AITimer < 270 && AITimer % 25 == 0)
                        {
                            Vector2 pos = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y - 300);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<RykardSkull>(), 30, 0);
                            if (AITimer % 100 == 0)
                            {
                                Vector2 vel = player.velocity;
                                vel.SafeNormalize(Vector2.UnitX);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(player.Center.X, pos.Y), new Vector2(vel.X * 0.1f, Main.rand.NextFloat(2, 4)), ModContent.ProjectileType<RykardSkull>(), 30, 0);
                            }
                        }
                        if (AITimer == 100)
                        {
                            NPC.velocity = Vector2.Zero;
                            Vector2 pos = NPC.Center;
                            bool hasReflected = false;
                            bool outside = false;
                            int times = 0;
                            int times2 = 0;
                            Vector2 vel = Vector2.UnitY * 30;
                            while (times < 6)
                            {
                                if (Vector2.Distance(NPC.Center, pos) > 550)
                                {
                                    if (!outside)
                                    {
                                        times++;
                                        outside = true;
                                        if (!hasReflected)
                                        {
                                            vel = -vel.RotatedBy((MathHelper.ToRadians(18)));
                                            hasReflected = true;
                                        }
                                        else
                                        {
                                            vel = -vel.RotatedBy((MathHelper.ToRadians(36)));
                                        }
                                    }
                                }
                                else
                                    outside = false;
                                Vector2 blorg = Helper.FromAToB(NPC.Center, pos, false);
                                if (hasReflected)
                                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<EPentagramAsh>(), 30, 0, -1, times2, blorg.X, blorg.Y).localAI[0] = NPC.whoAmI;
                                times2++;
                                pos += vel;
                            }
                        }
                        if (AITimer > 360)
                        {
                            Reset();
                            //AIState = SolarRays;
                            NextAttack();
                        }
                    }
                    break;
                case (SolarRays):
                    {
                        if (AITimer == 1)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ETelegraph>(), 0, 0);
                        }
                        if (AITimer == 40)
                            for (int i = 0; i < 4; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 4);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(256).RotatedBy(angle), Vector2.Zero, ModContent.ProjectileType<ETelegraph>(), 0, 0);
                            }
                        if (AITimer == 80)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(512).RotatedBy(angle), Vector2.Zero, ModContent.ProjectileType<ETelegraph>(), 0, 0);
                            }
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY, ModContent.ProjectileType<EBeam>(), 30, 0);
                        }
                        if (AITimer == 90)
                        {
                            EbonianMod.FlashAlpha = 0.05f;
                            EbonianMod.FlashAlphaDecrement = 0.0025f;
                            EbonianPlayer.Instance.FlashScreen(NPC.Center, 20, 0.05f);
                        }
                        if (AITimer == 120)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 4);
                                Vector2 pos = NPC.Center + new Vector2(256).RotatedBy(angle);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Helper.FromAToB(pos, NPC.Center), ModContent.ProjectileType<EBeam>(), 30, 0);
                            }
                        }
                        if (AITimer == 130)
                        {
                            EbonianMod.FlashAlpha = 0.05f;
                            EbonianMod.FlashAlphaDecrement = 0.0025f;
                            EbonianPlayer.Instance.FlashScreen(NPC.Center, 20, 0.1f);
                        }
                        if (AITimer == 160)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8);
                                Vector2 pos = NPC.Center + new Vector2(512).RotatedBy(angle);
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), pos, Helper.FromAToB(pos, NPC.Center), ModContent.ProjectileType<EBeam>(), 30, 0);
                            }
                        }
                        if (AITimer == 170)
                        {
                            EbonianMod.FlashAlpha = 0.075f;
                            EbonianMod.FlashAlphaDecrement = 0.0025f;
                            EbonianPlayer.Instance.FlashScreen(NPC.Center, 20, 0.15f);
                        }
                        if (AITimer > 180)
                        {
                            Reset();
                            //AIState = Geyser;
                            NextAttack();
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
            AITimer4 = 0;
            sunAlpha = 0f;
            flameAlpha = 0f;
            NPC.damage = 0;
        }
        void IdleMovement(bool over = false)
        {
            Player player = Main.player[NPC.target];
            NPC.velocity *= 0.975f;
            if (!over)
                NPC.velocity += Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200).RotatedBy(MathHelper.ToRadians(AITimer * 4)));
            else
                NPC.velocity += Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200).RotatedBy(MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4, (float)(Math.Sin(AITimer / 25) / 2) + 0.5f)));
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