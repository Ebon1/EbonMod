using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles;
using System.Reflection.Metadata;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.Bossbars;
using System.Linq;
using Terraria.UI;
using EbonianMod.Common.Achievements;
using EbonianMod.Common.Systems;
using EbonianMod.NPCs.Corruption;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using Terraria.GameContent.Golf;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.NPCs.Corruption.Ebonflies;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles.Enemy.Corruption;

namespace EbonianMod.NPCs.Terrortoma
{
    [AutoloadBossHead]
    public class Terrortoma : ModNPC
    {
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("An unusual conglomerate of corrupted entities with the strange capability to show emotion. It is malformed, and not the intended result of the metamorphosis."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrorStaff>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrorFlail>()));
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            Main.npcFrameCount[NPC.type] = 14;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            /*NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                //CustomTexturePath = "EbonianMod/NPCs/Terrortoma/Terrortoma_Bosschecklist",
                PortraitScale = 0.6f,
                PortraitPositionYOverride = 0f,
            };*/
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 14000;
            NPC.boss = true;
            NPC.damage = 100;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 118;
            NPC.height = 106;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.BossBar = ModContent.GetInstance<TerrortomaBar>();
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss");
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }
        Rectangle introFrame = new Rectangle(0, 0, 118, 108), laughFrame = new Rectangle(0, 0, 118, 108);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Player player = Main.player[NPC.target];
            Vector2 drawOrigin = new Vector2(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value.Width * 0.5f, NPC.height * 0.5f);
            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value, NPC.Center - pos, NPC.frame, NPC.IsABestiaryIconDummy ? Color.White : lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                if ((!isLaughing && AIState != -12124 && AIState != Intro))
                {
                    Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Bloom").Value;
                    spriteBatch.Reload(BlendState.Additive);
                    spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 - new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale, SpriteEffects.None, 0);
                    spriteBatch.Reload(BlendState.AlphaBlend);

                    spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value, NPC.Center - pos, NPC.frame, NPC.IsABestiaryIconDummy ? Color.White : lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (isLaughing)
                {
                    if (AITimer % 5 == 0)
                    {
                        if (laughFrame.Y < laughFrame.Height * 2)
                            laughFrame.Y += laughFrame.Height;
                        else
                            laughFrame.Y = 0;
                    }
                    spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaLaughing").Value, NPC.Center - pos, laughFrame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (AIState == Intro)
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaSpawn").Value, NPC.Center - pos, introFrame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            //else
            //{
            //spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value, NPC.Center - pos, null, lightColor, NPC.rotation, ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            //}
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Texture2D tex = Helper.GetTexture("NPCs/Terrortoma/TerrorEye");
            Vector2 eyeOGPosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
            Vector2 eyePosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
            Vector2 fromTo = Helper.FromAToB(eyeOGPosition, player.Center);
            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(tex, eyeOGPosition - screenPos, null, Color.White, 0, Vector2.One * 2, 1, SpriteEffects.None, 0);
            }
            if (AIState != -12124 && AIState != Intro && !isLaughing)
            {
                float dist = MathHelper.Clamp(Helper.FromAToB(eyeOGPosition, player.Center, false).Length() * 0.1f, 0, 6);
                if (AIState == Death)
                {
                    Vector2 vel = NPC.velocity;
                    vel.Normalize();
                    if (NPC.velocity == Vector2.Zero)
                        eyePosition += Main.rand.NextVector2Unit() * Main.rand.NextFloat(3);
                    else
                        eyePosition += vel * 5;
                }
                else
                    eyePosition += dist * fromTo;
                spriteBatch.Draw(tex, eyePosition - screenPos, null, Color.White, 0, Vector2.One * 2, 1, SpriteEffects.None, 0);

                Texture2D tex2 = Helper.GetExtraTexture("crosslight");
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(tex2, eyePosition - Main.screenPosition, null, Color.LawnGreen * glareAlpha, 0, tex2.Size() / 2, glareAlpha * 0.2f, SpriteEffects.None, 0);
                if (AIState == Death)
                {
                    Texture2D tex3 = Helper.GetExtraTexture("Extras2/flare_01");
                    Texture2D tex4 = Helper.GetExtraTexture("Extras2/star_02");
                    Main.spriteBatch.Draw(tex2, eyePosition - Main.screenPosition, null, Color.Olive * (glareAlpha - 1), Main.GameUpdateCount * 0.03f, tex2.Size() / 2, (glareAlpha - 1) * 0.25f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex3, eyePosition - Main.screenPosition, null, Color.Green * (glareAlpha - 2), Main.GameUpdateCount * -0.03f, tex3.Size() / 2, (glareAlpha - 2) * 0.45f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex4, eyePosition - Main.screenPosition, null, Color.Green * (glareAlpha - 3), Main.GameUpdateCount * -0.03f, tex4.Size() / 2, (glareAlpha - 3) * 0.75f, SpriteEffects.None, 0);
                }
                Main.spriteBatch.Reload(BlendState.AlphaBlend);

            }
        }
        //npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
        //Vector2 toPlayer = player.Center - npc.Center;
        //npc.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
        public override void FindFrame(int frameHeight)
        {
            if (AIState == Death)
            {
                if (++NPC.frameCounter < 5)
                    NPC.frame.Y = 12 * frameHeight;
                else if (NPC.frameCounter < 10)
                    NPC.frame.Y = 13 * frameHeight;
                else
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 12 * frameHeight;
                }
            }
            else
            {
                if (++NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < frameHeight * 11)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
        }
        public override bool CheckDead()
        {
            if (NPC.life <= 0 && !ded)
            {
                NPC.life = 1;
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.hostile && projectile.active)
                        projectile.Kill();
                }
                foreach (NPC npc in Main.npc)
                {
                    if (npc.type == ModContent.NPCType<BloatedEbonfly>() && npc.active)
                    {
                        npc.life = 0;
                        npc.checkDead();
                    }
                }
                AIState = Death;
                NPC.frameCounter = 0;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                EbonianSystem.ChangeCameraPos(NPC.Center, 150);
                EbonianSystem.ScreenShakeAmount = 20;
                ded = true;
                AITimer = AITimer2 = 0;
                NPC.velocity = Vector2.Zero;
                NPC.life = 1;
                return false;
            }
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, ModContent.Find<ModGore>("EbonianMod/Terrortoma1").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, ModContent.Find<ModGore>("EbonianMod/Terrortoma2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, ModContent.Find<ModGore>("EbonianMod/Terrortoma3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, ModContent.Find<ModGore>("EbonianMod/Terrortoma4").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, ModContent.Find<ModGore>("EbonianMod/Terrortoma5").Type, NPC.scale);
            for (int i = 0; i < 40; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
            }
            return true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotation);
            writer.Write(ded);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotation = reader.ReadByte();
            ded = reader.ReadBoolean();
        }
        float glareAlpha;
        private bool isLaughing;
        private bool HasSummonedClingers = false;
        private const int AISlot = 0;
        private const int TimerSlot = 1;
        float bloomAlpha;
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
        public float SelectedClinger
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private float AITimer2 = 0;
        private float angle = 0;
        private bool hasDonePhase2ApeShitMode = false;

        public const int ApeShitMode = 999, Idle = -2, Death = -1, Intro = 0, Vilethorn = 1, DifferentClingerAttacks = 2, HeadSlam = 3, CursedFlamesRain = 4, Pendulum = 5, ThrowUpVilethorns = 6, DoubleDash = 7, Ostertagi = 8;
        float rotation;
        bool ded;
        float Next = 1;
        float OldState;
        const int attackNum = 8;
        public float[] pattern = new float[attackNum];
        public float[] oldPattern = new float[attackNum];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < attackNum; i++)
            {
                pattern[i] = Main.rand.Next(1, attackNum + 1);
            }
        }
        public void GenerateNewPattern()
        {
            for (int i = 0; i < attackNum; i++)
            {
                pattern[i] = Main.rand.Next(1, attackNum + 1);
            }
            for (int i = 0; i < attackNum; i++)
            {
                int attempts = 0;
                while (++attempts < 100 && (pattern.Count(p => p == pattern[i]) != 1 || pattern[i] == 0) || oldPattern.Last() == pattern.First())
                {
                    pattern[i] = Main.rand.Next(1, attackNum + 1);
                }
            }
        }
        public void SwitchToRandom()
        {
            if (pattern.Any())
            {
                if (AIState == pattern[attackNum - 1])
                {
                    GenerateNewPattern();
                    Next = pattern.First();
                }
                else if (AIState == Intro)
                {
                    GenerateNewPattern();
                    Next = pattern.First();
                }
                else
                {
                    oldPattern = pattern;
                    Next = pattern[pattern.ToList().IndexOf((int)OldState) + 1];
                }
            }
        }
        public override void AI()
        {
            if (pattern.Contains(Intro) && AIState == Idle)
                GenerateNewPattern();
            if (AIState != Idle && AIState != Intro)
                OldState = AIState;
            AITimer++;
            if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
            NPC.rotation = rotation - NPC.rotation > MathHelper.Pi || rotation - NPC.rotation < -MathHelper.Pi ? rotation : MathHelper.SmoothStep(NPC.rotation, rotation, 0.25f);
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead)//|| !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead)//|| !player.ZoneCorrupt)
                {
                    AIState = -12124;
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 60)
                    {
                        NPC.timeLeft = 60;
                    }
                    return;
                }
            }
            if (glareAlpha > 0) glareAlpha -= 0.025f;
            if (NPC.life <= NPC.lifeMax / 2 && !hasDonePhase2ApeShitMode)
            {
                glareAlpha = 1f;
                AIState = ApeShitMode;
                hasDonePhase2ApeShitMode = true;
                AITimer = 0;
            }
            /*if (NPC.alpha >= 255)
            {
                NPC.Center = new Vector2(player.Center.X, player.Center.Y - 230);
            }*/
            if (NPC.alpha <= 0 && AIState != Death)
            {
                /*if (!HasSummonedClingers)
                {
                    NPC clinger = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerMelee>())];
                    NPC clinger2 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerSummoner>())];
                    NPC clinger3 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerRanged>())];
                    clinger.ai[0] = NPC.whoAmI;
                    clinger2.ai[0] = NPC.whoAmI;
                    clinger3.ai[0] = NPC.whoAmI;
                    HasSummonedClingers = true;
                }*/
                if (!NPC.AnyNPCs(ModContent.NPCType<TerrorClingerMelee>()))
                {
                    NPC clinger = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerMelee>())];
                    clinger.ai[0] = NPC.whoAmI;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<TerrorClingerSummoner>()))
                {
                    NPC clinger2 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerSummoner>())];
                    clinger2.ai[0] = NPC.whoAmI;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<TerrorClingerRanged>()))
                {
                    NPC clinger3 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TerrorClingerRanged>())];
                    clinger3.ai[0] = NPC.whoAmI;
                }
            }
            if (AIState == -12124)
            {

                NPC.velocity = new Vector2(0, 10f);
                if (player.ZoneCorrupt)
                {
                    AITimer = 0;
                    NPC.velocity = Vector2.Zero;
                    if (NPC.life > NPC.lifeMax / 2)
                        AIState = Vilethorn;
                }
            }
            else if (AIState == Death)
            {
                SelectedClinger = 4;
                NPC.damage = 0;
                NPC.timeLeft = 2;
                if (AITimer < 250)
                {
                    NPC.velocity = Vector2.Zero;
                    rotation = 0;
                }
                if (AITimer >= 250 && AITimer < 450)
                {
                    if (AITimer % 10 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver2) * 0.5f, ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);

                    }
                    if (AITimer % 25 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            if (i == 0) continue;
                            Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(4, 6), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0, i * Main.rand.NextFloat(0.15f, 0.5f));
                            a.friendly = false;
                            a.hostile = true;
                        }
                    }
                    if (AITimer == 305)
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
                        SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                        Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2(), ModContent.ProjectileType<VileTearTelegraph>(), 0, 0);
                    }
                    if (AITimer < 290)
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                    else
                        rotation = Helper.LerpAngle(rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.03f);
                    glareAlpha = MathHelper.Lerp(glareAlpha, 4f, 0.05f);
                }
                if (AITimer == 326)
                {
                    NPC.velocity = Vector2.Zero;
                    EbonianSystem.ScreenShakeAmount = 15f;
                    SoundEngine.PlaySound(EbonianSounds.chargedBeamImpactOnly, NPC.Center);
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2(), ModContent.ProjectileType<TBeam>(), 10000, 0);
                }
                if (AITimer == 480)
                    NPC.velocity = Vector2.UnitY;
                if (AITimer >= 480)
                {
                    Helper.DustExplosion(NPC.Center, Vector2.One, 2, Color.Gray * 0.35f, false, false, 0.4f, 0.5f, new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-6, -4)));
                    NPC.velocity *= 1.025f;
                    rotation += MathHelper.ToRadians(3 * NPC.velocity.Y);
                    if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 1920) < NPC.width / 2)
                    {
                        Projectile.NewProjectileDirect(NPC.InheritSource(NPC), Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 1920), Vector2.Zero, ModContent.ProjectileType<TExplosion>(), 0, 0).scale = 2f;
                        SoundEngine.PlaySound(EbonianSounds.eggplosion);
                        SoundEngine.PlaySound(EbonianSounds.evilOutro);
                        NPC.immortal = false;
                        NPC.life = 0;
                        if (!EbonianAchievementSystem.acquiredAchievement[1])
                            InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(1));
                        NPC.checkDead();
                    }
                }
            }
            else if (AIState == Intro)
            {
                SelectedClinger = 4;
                //if (NPC.life < NPC.lifeMax)
                AITimer++;
                //isLaughing = true;
                rotation = 0;
                if (AITimer == 1)
                {
                    //Helper.SetBossTitle(120, "Terrortoma", Color.LawnGreen, "The Conglomerate", 0);
                    EbonianSystem.ChangeCameraPos(NPC.Center, 100);
                    //add sound later
                }
                if (AITimer2 == 0 && AITimer % 5 == 0 && introFrame.Y < introFrame.Height * 15)
                {
                    introFrame.Y += introFrame.Height;
                    if (introFrame.Y >= introFrame.Height * 15)
                    {
                        AITimer2 = 1;
                    }
                    if (introFrame.Y == introFrame.Height * 11 || introFrame.Y == introFrame.Height * 13)
                        SoundEngine.PlaySound(EbonianSounds.blink, NPC.Center);
                }
                if (AITimer2 == 1 && AITimer % 5 == 0 && introFrame.Y > introFrame.Height)
                {
                    if (introFrame.Y > 9 * introFrame.Height)
                    {
                        introFrame.Y = 9 * introFrame.Height;
                    }
                    else
                    {
                        introFrame.Y -= introFrame.Height;
                    }
                }
                if (AITimer >= 300)
                {
                    SwitchToRandom();
                    AIState = Idle;
                    AITimer = 100;
                }
            }
            else if (AIState == Idle)
            {
                NPC.localAI[0] = 0;
                AITimer++;
                if (AITimer == 2)
                {
                    if (hasDonePhase2ApeShitMode)
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 rainPos = new Vector2(player.Center.X + 1920 / 2 * Main.rand.NextFloat(-1, 1), player.Center.Y + (1080 + 300) / 2);
                            Vector2 rainPos2 = new Vector2(player.Center.X + 1920 / 2 * Main.rand.NextFloat(-1, 1), player.Center.Y - 300 * 2);
                            Vector2 rainPos3 = new Vector2(player.Center.X - 300 / 2, player.Center.Y + 1080 * Main.rand.NextFloat(-.5f, .5f));
                            Vector2 rainPos4 = new Vector2(player.Center.X + 1920 / 2 + 300, player.Center.Y + 1080 * Main.rand.NextFloat(-.5f, .5f));
                            if (Main.rand.NextBool(5))
                            {
                                if (Main.rand.NextBool())
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, -10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, new Vector2(0, 10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);

                                if (Main.rand.NextBool())
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, new Vector2(10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, new Vector2(-10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                            }
                        }
                    SoundEngine.PlaySound(EbonianSounds.terrortomaLaugh, NPC.Center);
                }
                if (AITimer < 100)
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 4;
                else NPC.velocity *= 0.9f;
                isLaughing = AITimer < 100;
                rotation = Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                if (AITimer > 150)
                {
                    NPC.velocity = Vector2.Zero;
                    AIState = Next;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == ApeShitMode)
            {
                NPC.velocity = Vector2.Zero;
                isLaughing = false;
                Vector2 toPlayer = player.Center - NPC.Center;
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                if (AITimer >= 60)
                {
                    AIState = Vilethorn;
                    AITimer = 0;
                }
            }
            else if (AIState == Vilethorn)
            {
                SelectedClinger = 4;
                if (AITimer == 1)
                    bloomAlpha = 1f;
                NPC.damage = 100;
                NPC.localAI[0] = 100;
                if (AITimer < 250 && AITimer >= 50)
                {
                    if (!hasDonePhase2ApeShitMode)
                    {
                        if (++AITimer2 % 25 == 0)
                        {
                            Vector2 rainPos = new Vector2(player.Center.X + 1920 * Main.rand.NextFloat(-0.5f, 0.5f), player.Center.Y + 1300);
                            Vector2 rainPos2 = new Vector2(player.Center.X + 1920 * Main.rand.NextFloat(-0.5f, 0.5f), player.Center.Y - 600);
                            if (Main.rand.NextBool())
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, -10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                            else
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, new Vector2(0, 10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                        }
                    }
                    else
                    {
                        if (++AITimer2 % 20 == 0)
                        {
                            Vector2 rainPos = new Vector2(player.Center.X + 1920 / 2 * Main.rand.NextFloat(-1, 1), player.Center.Y + (1080 + 300) / 2);
                            Vector2 rainPos2 = new Vector2(player.Center.X + 1920 / 2 * Main.rand.NextFloat(-1, 1), player.Center.Y - 300 * 2);
                            Vector2 rainPos3 = new Vector2(player.Center.X - 300 / 2, player.Center.Y + 1080 * Main.rand.NextFloat(-.5f, .5f));
                            Vector2 rainPos4 = new Vector2(player.Center.X + 1920 / 2 + 300, player.Center.Y + 1080 * Main.rand.NextFloat(-.5f, .5f));
                            if (Main.rand.NextBool())
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, -10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                            else
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, new Vector2(0, 10), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);

                            if (Main.rand.NextBool(5))
                            {
                                if (Main.rand.NextBool())
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, new Vector2(10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, new Vector2(-10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                            }
                        }
                    }
                    if (AITimer2 % 50 == 4)
                    {
                        SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        }
                    }
                    if (AITimer2 % 50 < 10)
                    {
                        NPC.velocity += Helper.FromAToB(NPC.Center, player.Center) * 2;
                    }
                    if (AITimer2 % 50 > 40)
                        NPC.velocity *= 0.9f;
                    rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
                if (AITimer >= 290)
                {
                    ResetState();
                }
            }
            else if (AIState == DifferentClingerAttacks)
            {
                if (SelectedClinger == 4)
                    SelectedClinger = Main.rand.Next(3);
                NPC.damage = 0;
                NPC.localAI[0] = 0;
                if (AITimer <= 300)
                {
                    Vector2 down = new Vector2(0, 10);
                    rotation = down.ToRotation() - MathHelper.PiOver2;
                    Vector2 pos = player.Center + new Vector2(0, -200);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.02545f;
                }
                else NPC.velocity *= 0.9f;
                if (AITimer >= 300)
                {
                    ResetState();
                }
            }
            else if (AIState == HeadSlam)
            {
                SelectedClinger = 3;
                NPC.damage = 0;
                NPC.localAI[0] = 100;
                if (AITimer < 100)
                    NPC.velocity = -Vector2.UnitY * MathHelper.Clamp(MathHelper.Lerp(1, 5, player.Center.Y - NPC.Center.Y / 300), 1, 5);
                else
                    NPC.velocity *= 0.9f;
                rotation = Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                if (AITimer >= 170)
                {
                    ResetState();
                }
            }
            else if (AIState == CursedFlamesRain)
            {
                SelectedClinger = 4;
                NPC.damage = 0;
                Vector2 toPlayer = new Vector2(0, -10);
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                NPC.localAI[0] = 100;
                if (AITimer <= 150)
                {
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 75);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.0445f;
                    if (++AITimer2 % 60 == 0)
                    {
                        for (int i = 0; i <= 5 + (Main.expertMode ? 5 : 0); i++)
                        {
                            Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -10), ModContent.ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                            projectile.tileCollide = false;
                            projectile.hostile = true;
                            projectile.friendly = false;
                            projectile.timeLeft = 230;
                        }
                        Projectile projectileb = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -10), ModContent.ProjectileType<TFlameThrower2>(), 30, 1f, Main.myPlayer)];
                        projectileb.tileCollide = false;
                        projectileb.hostile = true;
                        projectileb.friendly = false;
                        projectileb.timeLeft = 230;
                    }
                    if (hasDonePhase2ApeShitMode)
                        if (AITimer2 % 20 == 0)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-7f, 7f), -10), ModContent.ProjectileType<TerrorStaffPEvil>(), 30, 1f, NPC.target);
                }
                else NPC.velocity *= 0.9f;
                if (AITimer >= 270 - (hasDonePhase2ApeShitMode ? 100 : 0))
                {
                    ResetState();
                }
            }
            else if (AIState == Pendulum)
            {
                SelectedClinger = 3;
                NPC.damage = 0;
                NPC.localAI[0] = 100;
                if (++AITimer2 % 25 == 0)
                {
                    Vector2 rainPos3 = new Vector2(player.Center.X + 1920 * Main.rand.NextFloat(-0.5f, 0.5f), player.Center.Y + 1300);
                    Vector2 rainPos4 = new Vector2(player.Center.X + 1920 * Main.rand.NextFloat(-0.5f, 0.5f), player.Center.Y - 600);
                    if (Main.rand.NextBool(5))
                    {
                        if (Main.rand.NextBool())
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, new Vector2(10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, new Vector2(-10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                    }
                }
                Vector2 toPlayer = player.Center - NPC.Center;
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = new Vector2(player.position.X, player.position.Y - 280);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.0545f;
                if (AITimer >= 370 - (hasDonePhase2ApeShitMode ? 100 : 0))
                {
                    ResetState();
                }
            }
            else if (AIState == ThrowUpVilethorns)
            {
                SelectedClinger = 4;
                if (AITimer == 1)
                    bloomAlpha = 1f;
                rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                if (AITimer > 30 && AITimer % 7 == 0 && AITimer <= (hasDonePhase2ApeShitMode ? 100 : 75))
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver2) * 0.5f, ModContent.ProjectileType<TerrorVilethorn1>(), 35, 0, 0);
                }
                if (AITimer >= 100)
                {
                    ResetState();
                }
            }
            else if (AIState == DoubleDash)
            {
                SelectedClinger = 2;
                if (AITimer == 1)
                    if (!player.velocity.Y.CloseTo(0, 0.2f) || player.Center.Y < NPC.Center.Y)
                        ResetState();
                if (AITimer < 20)
                {
                    NPC.velocity = Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -200), false) / 50f;
                    rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                }
                else if (AITimer < 30 && AITimer >= 20)
                    NPC.velocity *= 0.8f;
                if (AITimer == 30)
                {
                    lastPos = Helper.TRay.Cast(NPC.Center, Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center), new Vector2(-0.35f, 1), new Vector2(0.35f, 1)), 2028);
                    bloomAlpha = 1f;
                }
                if (AITimer > 100 && AITimer < 170)
                {
                    NPC.velocity += Helper.FromAToB(NPC.Center, lastPos, false) / 40f;
                    if (NPC.Center.Distance(lastPos) < NPC.height * 0.75f)
                    {
                        if (NPC.velocity.Y > 0)
                            AITimer2 = 1;
                        AITimer = 170;
                    }
                }
                if (AITimer == 170 && AITimer2 == 1)
                {
                    SoundEngine.PlaySound(EbonianSounds.chomp1, NPC.Center);
                    EbonianSystem.ScreenShakeAmount = 5f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(0, 0), ModContent.ProjectileType<GluttonImpact>(), 50, 0, 0, 0);
                }
                if (AITimer >= 170) { NPC.velocity *= 0.1f; }

                NPC.damage = 100;
                if (AITimer >= 200)
                {
                    ResetState();
                }
            }
            else if (AIState == Ostertagi)
            {
                if (AITimer == 1)
                    bloomAlpha = 0.5f;
                if (AITimer == 30)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                }
                if (AITimer == 30)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    for (int i = 0; i < 10; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 10);
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                    }
                }
                if (AITimer == 40)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    for (int i = 0; i < 10; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 10) + MathHelper.PiOver4;
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                    }
                }
                if (hasDonePhase2ApeShitMode)
                    if (AITimer == 50)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                        for (int i = 0; i < 10; i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 10) + MathHelper.PiOver2;
                            Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
                            a.friendly = false;
                            a.hostile = true;
                        }
                    }
                if (AITimer >= 60)
                {
                    ResetState();
                }
            }
        }
        void ResetState()
        {
            AITimer = 0;
            AITimer2 = 0;
            SelectedClinger = 4;
            SwitchToRandom();
            AIState = Idle;
            NPC.damage = 0;
            NPC.velocity = Vector2.Zero;
            isLaughing = false;
        }
        Vector2 lastPos;
    }
}