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
using EbonianMod.Items.Weapons.Summoner;
using EbonianMod.Items.Materials;
using EbonianMod.Items.BossTreasure;
using EbonianMod.Items.Pets.Hightoma;
using EbonianMod.Items.Tiles.Trophies;
using EbonianMod.Items.Tiles;
using EbonianMod.Items.Armor.Vanity;
using EbonianMod.Dusts;
using Humanizer;
using Terraria.Graphics.CameraModifiers;

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
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(new CommonDrop(ItemType<TerrortomaMaterial>(), 1, 40, 60));
            notExpertRule.OnSuccess(new CommonDrop(ItemType<Ostertagi>(), 1));
            npcLoot.Add(notExpertRule);

            npcLoot.Add(ItemDropRule.Common(ItemType<TTomaMask>(), 4));
            npcLoot.Add(ItemDropRule.Common(ItemType<TerrortomaTrophy>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<CursedCone>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<TerrortomaRelic>()));
            npcLoot.Add(ItemDropRule.Common(ItemType<DappertomaI>(), 5));

            npcLoot.Add(ItemDropRule.BossBag(ItemType<TerrortomaBag>()));
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
            Main.npcFrameCount[NPC.type] = 14;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            /*NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "EbonianMod/NPCs/Terrortoma/Terrortoma_Bosschecklist",
                PortraitScale = 0.6f,
                PortraitPositionYOverride = 0f,
            };*/
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 30000;
            NPC.boss = true;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 33;
            NPC.value = Item.buyPrice(0, 10);
            NPC.knockBackResist = 0;
            NPC.width = 118;
            NPC.height = 106;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.BossBar = GetInstance<TerrortomaBar>();
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss");
            NPC.buffImmune[24] = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.netAlways = true;
            NPC.dontTakeDamage = true;
        }
        Rectangle introFrame = new Rectangle(0, 0, 118, 108), laughFrame = new Rectangle(0, 0, 118, 108);
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D laughTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaLaughing").Value;
            Texture2D tomaTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value;
            Texture2D spawnTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaSpawn").Value;
            Player player = Main.player[NPC.target];
            Vector2 drawOrigin = new Vector2(tomaTex.Width * 0.5f, NPC.height * 0.5f);
            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(tomaTex, NPC.Center - pos, NPC.frame, NPC.IsABestiaryIconDummy ? Color.White : lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                if (AITimer % 5 == 0)
                {
                    if (laughFrame.Y < laughFrame.Height * 2)
                        laughFrame.Y += laughFrame.Height;
                    else
                        laughFrame.Y = 0;
                }
                if (isLaughing || AIState == -12124)
                {
                    spriteBatch.Draw(laughTex, NPC.Center - pos, laughFrame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (((AIState == EyeHomingFlames ? true : !isLaughing) && AIState != -12124 && AIState != Intro))
                {
                    Texture2D tex = Request<Texture2D>(Texture + "_Bloom").Value;
                    spriteBatch.Reload(BlendState.Additive);
                    spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 - new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale, SpriteEffects.None, 0);
                    spriteBatch.Reload(BlendState.AlphaBlend);
                    if (!isLaughing)
                        spriteBatch.Draw(tomaTex, NPC.Center - pos, NPC.frame, NPC.IsABestiaryIconDummy ? Color.White : lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (isLaughing || AIState == -12124)
                {
                    spriteBatch.Draw(laughTex, NPC.Center - pos, laughFrame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
                if (AIState == Intro)
                {
                    spriteBatch.Draw(spawnTex, NPC.Center - pos, introFrame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            //else
            //{
            //spriteBatch.Draw(Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value, NPC.Center - pos, null, lightColor, NPC.rotation, Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            //}
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Texture2D tex = Helper.GetTexture("NPCs/Terrortoma/TerrorEye");
            Texture2D laughTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaLaughing").Value;
            Texture2D tomaTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value;
            Texture2D spawnTex = Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaSpawn").Value;
            Vector2 eyeOGPosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
            Vector2 eyePosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
            Vector2 fromTo = Helper.FromAToB(eyeOGPosition, player.Center);
            if (NPC.IsABestiaryIconDummy)
            {
                fromTo = Helper.FromAToB(eyeOGPosition, Main.MouseScreen);
                float dist = MathHelper.Clamp(Helper.FromAToB(eyeOGPosition, Main.MouseScreen, false).Length() * 0.1f, 0, 5);
                eyePosition += dist * fromTo;
                spriteBatch.Draw(tex, eyePosition - screenPos, null, Color.White, 0, Vector2.One * 2, 1, SpriteEffects.None, 0);
            }
            if (AIState != -12124 && AIState != Intro && (AIState == EyeHomingFlames ? true : !isLaughing))
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
                if (!isLaughing)
                    spriteBatch.Draw(tex, eyePosition - screenPos, null, drawColor, 0, Vector2.One * 2, 1, SpriteEffects.None, 0);

                Texture2D tex2 = Helper.GetExtraTexture("crosslight");
                Main.spriteBatch.Reload(BlendState.Additive);
                Main.spriteBatch.Draw(tex2, isLaughing ? eyeOGPosition : eyePosition - Main.screenPosition, null, Color.LawnGreen * glareAlpha, 0, tex2.Size() / 2, glareAlpha * 0.2f, SpriteEffects.None, 0);
                if (AIState == Death)
                {
                    Texture2D tex3 = Helper.GetExtraTexture("Extras2/flare_01");
                    Texture2D tex4 = Helper.GetExtraTexture("Extras2/star_02");
                    Main.spriteBatch.Draw(tex2, eyePosition - Main.screenPosition, null, Color.Olive * (glareAlpha - 1), Main.GameUpdateCount * 0.03f, tex2.Size() / 2, (glareAlpha - 1) * 0.5f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex3, eyePosition - Main.screenPosition, null, Color.Green * (glareAlpha - 2), Main.GameUpdateCount * -0.03f, tex3.Size() / 2, (glareAlpha - 2) * 0.45f * 2, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex4, eyePosition - Main.screenPosition, null, Color.Green * (glareAlpha - 3), Main.GameUpdateCount * -0.03f, tex4.Size() / 2, (glareAlpha - 3) * 0.75f * 2, SpriteEffects.None, 0);
                }
                Main.spriteBatch.Reload(BlendState.AlphaBlend);

            }
            Vector2 drawOrigin = new Vector2(tomaTex.Width * 0.5f, NPC.height * 0.5f);
            if (isLaughing || AIState == -12124)
            {
                if (!NPC.IsABestiaryIconDummy)
                    spriteBatch.Draw(laughTex, NPC.Center - screenPos, laughFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            if (AIState == Intro)
            {
                if (!NPC.IsABestiaryIconDummy)
                    spriteBatch.Draw(Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrortomaSpawn").Value, NPC.Center - screenPos, introFrame, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
        }
        //npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
        //Vector2 toPlayer = player.Center - npc.Center;
        //npc.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
        bool angry = false;
        public override void FindFrame(int frameHeight)
        {
            if (AIState == Death || angry)
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
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.hostile && projectile.active)
                        projectile.Kill();
                }
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == NPCType<BloatedEbonfly>() && npc.active)
                    {
                        npc.life = 0;
                        npc.checkDead();
                    }
                }
                AIState = Death;
                NPC.frameCounter = 0;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                EbonianSystem.ChangeCameraPos(NPC.Center, 170, new ZoomInfo(2, 1.1f, InOutElastic, InOutCirc), 1.5f, InOutQuart);
                EbonianSystem.ScreenShakeAmount = 20;
                ded = true;
                AITimer = AITimer2 = 0;
                NPC.velocity = Vector2.Zero;
                NPC.life = 1;
                return false;
            }
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, Find<ModGore>("EbonianMod/Terrortoma1").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, Find<ModGore>("EbonianMod/Terrortoma2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, Find<ModGore>("EbonianMod/Terrortoma3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, Find<ModGore>("EbonianMod/Terrortoma4").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Unit() * 5, Find<ModGore>("EbonianMod/Terrortoma5").Type, NPC.scale);
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

        public const int ApeShitMode = 999, Idle = -2, Death = -1, Intro = 0, Vilethorn = 1, DifferentClingerAttacks = 2, HeadSlam = 3,
            CursedFlamesRain = 4, Pendulum = 5, ThrowUpVilethorns = 6, DoubleDash = 7, Ostertagi = 8, FlamesFallUp = 9, GeyserSweep = 10,
            EyeHomingFlames = 11, RangedHeadSlam = 12, CursedDollCopy = 13, ShadowOrbVomit = 14, TitteringSpawn = 15;
        float rotation;
        bool ded;
        float Next = 1;
        float OldState;
        const int attackNum = 15;
        public float[] pattern = new float[attackNum];
        public float[] oldPattern = new float[attackNum];
        bool finishedPattern;
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < attackNum - 8; i++)
            {
                pattern[i] = Main.rand.Next(1, attackNum - (hasDonePhase2ApeShitMode ? 0 : 8) + 1);
            }
        }
        public void GenerateNewPattern()
        {
            for (int i = 0; i < attackNum - (hasDonePhase2ApeShitMode ? 0 : 8); i++)
            {
                pattern[i] = Main.rand.Next(1, (hasDonePhase2ApeShitMode ? 9 : 16));
            }
            for (int i = 0; i < attackNum - (hasDonePhase2ApeShitMode ? 0 : 8); i++)
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
            /*if (pattern.Any())
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
            */
            if (finishedPattern)
            {
                if (Main.rand.NextBool(30))
                    Next = Intro;
                else
                {
                    if (hasDonePhase2ApeShitMode && Main.rand.NextBool(3))
                        Next = Main.rand.Next(8, 16);
                    else
                        Next = Main.rand.Next(1, (!hasDonePhase2ApeShitMode ? 9 : 16));
                }
            }
            else
            {
                Next = OldState + 1 * (hasDonePhase2ApeShitMode ? -1 : 1);
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
            if (!player.active || player.dead && AIState != Death)//|| !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead && AIState != Death)//|| !player.ZoneCorrupt)
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
            if (NPC.life <= NPC.lifeMax - NPC.lifeMax / 2 + 2000 && !hasDonePhase2ApeShitMode)
            {
                glareAlpha = 1f;
                AIState = ApeShitMode;
                finishedPattern = false;
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
                    NPC clinger = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerMelee>())];
                    NPC clinger2 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerSummoner>())];
                    NPC clinger3 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerRanged>())];
                    clinger.ai[0] = NPC.whoAmI;
                    clinger2.ai[0] = NPC.whoAmI;
                    clinger3.ai[0] = NPC.whoAmI;
                    HasSummonedClingers = true;
                }*/
                if (!NPC.AnyNPCs(NPCType<TerrorClingerMelee>()))
                {
                    NPC clinger = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerMelee>())];
                    clinger.ai[0] = NPC.whoAmI;
                }
                if (!NPC.AnyNPCs(NPCType<TerrorClingerSummoner>()))
                {
                    NPC clinger2 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerSummoner>())];
                    clinger2.ai[0] = NPC.whoAmI;
                }
                if (!NPC.AnyNPCs(NPCType<TerrorClingerRanged>()))
                {
                    NPC clinger3 = Main.npc[NPC.NewNPC(NPC.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y, NPCType<TerrorClingerRanged>())];
                    clinger3.ai[0] = NPC.whoAmI;
                }
            }
            if (AIState == -12124)
            {
                isLaughing = true;
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
                isLaughing = false;
                if (AITimer < 250)
                {
                    NPC.velocity = Vector2.Zero;
                    rotation = 0;
                }
                if (AITimer < 205 && AITimer >= 30)
                {
                    rotation = 0;
                    if (AITimer == 30)
                    {
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-0.6f).WithVolumeScale(1.6f), NPC.Center);
                    }
                    if (AITimer % 20 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 30, 12, 30, 1000));
                    angry = true;
                    if (AITimer % 5 == 0)
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<TerrortomaScream>(), 0, 0);
                }
                if (AITimer >= 250 && AITimer < 450)
                {
                    if (AITimer % 10 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver2) * 0.5f, ProjectileType<TerrorVilethorn1>(), 20, 0, 0);

                    }
                    if (AITimer % 25 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            if (i == 0) continue;
                            Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(4, 6), ProjectileType<OstertagiWorm>(), 24, 0, 0, i * Main.rand.NextFloat(0.15f, 0.5f));
                            a.friendly = false;
                            a.hostile = true;
                        }
                    }
                    if (AITimer == 305)
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/ambience");
                        SoundEngine.PlaySound(EbonianSounds.chargedBeamWindUp, NPC.Center);
                        Projectile.NewProjectile(NPC.InheritSource(NPC), NPC.Center, (rotation + MathHelper.PiOver2).ToRotationVector2(), ProjectileType<VileTearTelegraph>(), 0, 0);
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
                    Projectile.NewProjectileDirect(NPC.InheritSource(NPC), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2(), ProjectileType<TBeam>(), 10000, 0);
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
                        Projectile.NewProjectileDirect(NPC.InheritSource(NPC), Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 1920), Vector2.Zero, ProjectileType<TExplosion>(), 0, 0).scale = 2f;
                        SoundEngine.PlaySound(EbonianSounds.eggplosion);
                        SoundEngine.PlaySound(EbonianSounds.evilOutro);
                        NPC.immortal = false;
                        NPC.life = 0;
                        GetInstance<DownedBossSystem>().downedTerrortoma = true;
                        //if (!EbonianAchievementSystem.acquiredAchievement[1])
                        //  InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(1));
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
                    EbonianSystem.ChangeCameraPos(NPC.Center, 130, new ZoomInfo(2, 1.6f, InOutQuad), 1.5f, easingFunction: InOutCirc);
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
                    NPC.dontTakeDamage = false;
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
                            Vector2 rainPos = player.Center + new Vector2(900 * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), Main.rand.NextFloat(500));
                            Vector2 rainPos2 = player.Center + new Vector2(900 * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), Main.rand.NextFloat(500));
                            Vector2 rainPos3 = player.Center + new Vector2(500 * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), -800);
                            Vector2 rainPos4 = player.Center + new Vector2(500 * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), -800);
                            if (Main.rand.NextBool(5))
                            {
                                if (Main.rand.NextBool())
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, Helper.FromAToB(rainPos, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, Helper.FromAToB(rainPos2, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);

                                if (Main.rand.NextBool())
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, Helper.FromAToB(rainPos3, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, Helper.FromAToB(rainPos4, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
                            }
                        }
                    SoundEngine.PlaySound(EbonianSounds.terrortomaLaugh, NPC.Center);
                }
                if (NPC.Distance(player.Center) > 200)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200) + Helper.FromAToB(player.Center, NPC.Center) * 50) * 13, 0.1f);
                else NPC.velocity *= 0.8f;
                isLaughing = AITimer < 100;
                rotation = Vector2.UnitY.ToRotation() - MathHelper.PiOver2;
                if (AITimer > 190)
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
                NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.1f);
                if (AITimer == 30)
                    EbonianSystem.ChangeCameraPos(NPC.Center, 200, new ZoomInfo(2, 1.1f, InOutQuad, InOutCirc), 1.5f, InOutQuart);
                if (AITimer < 205 && AITimer >= 30)
                {
                    rotation = 0;
                    if (AITimer == 30)
                    {
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-0.6f).WithVolumeScale(1.6f), NPC.Center);
                    }
                    if (AITimer % 20 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 30, 12, 30, 1000));
                    angry = true;
                    if (AITimer % 5 == 0)
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<TerrortomaScream>(), 0, 0);
                }
                else angry = false;

                if (AITimer >= 230)
                {
                    AIState = TitteringSpawn; //vilethorn = dash
                    AITimer = 0;
                }
            }
            else if (AIState == Vilethorn)
            {
                SelectedClinger = 4;
                if (AITimer == 1)
                    bloomAlpha = 1f;
                NPC.damage = 65;
                NPC.localAI[0] = 100;
                AITimer2++;
                if (AITimer < 250 && AITimer >= 50)
                {
                    if (AITimer2 == 14)
                    {
                        SoundEngine.PlaySound(EbonianSounds.terrortomaDash, NPC.Center);
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        }
                    }
                    if (AITimer2 < 10)
                    {
                        rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - PiOver2;
                        NPC.velocity -= Helper.FromAToB(NPC.Center, player.Center) * (AITimer2 % 50) * 0.3f;
                    }
                    if (AITimer2 < 30 && AITimer > 10)
                    {
                        if (AITimer2 > 20)
                            rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                        if (AITimer2 == 14)
                        {
                            for (int i = -3; i < 4; i += (Main.expertMode ? 1 : 2))
                            {
                                Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedBy(i * (hasDonePhase2ApeShitMode ? 0.3f : 0.5f)) * 10, ProjectileType<TFlameThrower4>(), 20, 0);
                            }
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center) * 25;
                        }
                    }
                    if (AITimer2 > 40)
                        NPC.velocity *= 0.9f;

                    if (AITimer2 > 62) AITimer2 = 0;
                }
                if (AITimer >= 290)
                {
                    if (hasDonePhase2ApeShitMode)
                        finishedPattern = true;
                    ResetState();
                }
            }
            else if (AIState == DifferentClingerAttacks)
            {
                if (SelectedClinger == 4 && AITimer < 290)
                {
                    int[] allowedClingers = [0, 1, 2];
                    if (NPC.Center.Distance(player.Center) > 900)
                        allowedClingers = [0, 1];
                    SelectedClinger = Main.rand.Next(allowedClingers);
                }
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
                        for (int i = 0; i <= 5; i++)
                        {
                            Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -10), ProjectileType<TFlameThrower2>(), 20, 1f, Main.myPlayer)];
                            projectile.tileCollide = false;
                            projectile.hostile = true;
                            projectile.friendly = false;
                            projectile.timeLeft = 230;
                        }
                        Projectile projectileb = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -10), ProjectileType<TFlameThrower2>(), 20, 1f, Main.myPlayer)];
                        projectileb.tileCollide = false;
                        projectileb.hostile = true;
                        projectileb.friendly = false;
                        projectileb.timeLeft = 230;
                    }
                    if (hasDonePhase2ApeShitMode)
                        if (AITimer2 % 20 == 0)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-7f, 7f), -10), ProjectileType<TerrorStaffPEvil>(), 24, 1f, NPC.target);
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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, Helper.FromAToB(rainPos3, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, Helper.FromAToB(rainPos4, player.Center), ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
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
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(MathHelper.PiOver2) * 0.5f, ProjectileType<TerrorVilethorn1>(), 20, 0, 0);
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

                    for (int i = 0; i < 10; i += (hasDonePhase2ApeShitMode ? 1 : 2))
                    {
                        float angle = Helper.CircleDividedEqually(i, 10);
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 24, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(0, 0), ProjectileType<GluttonImpact>(), 50, 0, 0, 0);
                    }
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, new Vector2(0, 0), ProjectileType<GluttonImpact>(), 50, 0, 0, 0);
                }
                if (AITimer >= 170) { NPC.velocity *= 0.1f; }

                NPC.damage = 65;
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
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                }
                if (AITimer == 30)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    for (int i = 0; i < 10; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 10);
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 24, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                    }
                }
                if (AITimer == 40)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    for (int i = 0; i < 10; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 10) + MathHelper.PiOver4;
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 24, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                    }
                }
                if (hasDonePhase2ApeShitMode)
                    if (AITimer == 50)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                        for (int i = 0; i < 10; i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 10) + MathHelper.PiOver2;
                            Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 24, 0, 0);
                            a.friendly = false;
                            a.hostile = true;
                        }
                    }
                if (AITimer >= 60)
                {
                    if (!hasDonePhase2ApeShitMode) finishedPattern = true;
                    ResetState();
                }
            }
            else if (AIState == FlamesFallUp)
            {
                if (AITimer == 80)
                    glareAlpha = 1;
                if (AITimer < 80)
                {
                    if (NPC.Distance(player.Center) > 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200)) * 30, 0.02f);
                    else NPC.velocity *= 0.9f;
                    Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedByRandom(PiOver2 * 0.7f);
                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10), newColor: Color.LawnGreen, Scale: Main.rand.NextFloat(0.06f, 0.2f)).customData = NPC.Center + new Vector2(0, 20);

                    Dust.NewDustPerfect(pos, DustID.CursedTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                }
                else NPC.velocity *= 0.9f;
                if (AITimer >= 100 && AITimer < 190)
                {
                    Projectile.NewProjectile(null, NPC.Center + Vector2.UnitY.RotatedByRandom(PiOver4) * 20, Vector2.UnitY.RotatedByRandom(PiOver4) * Main.rand.NextFloat(3, 8), ProjectileType<TFlameThrower>(), 23, 0);
                    if (AITimer % 4 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                        if (AITimer < 170)
                            Projectile.NewProjectile(null, NPC.Center + Vector2.UnitY.RotatedByRandom(PiOver4) * 20, new Vector2(Main.rand.NextFloat(-20, 20), Main.rand.NextFloat(-23, -16)), ProjectileType<TFlameThrower2_Inverted>(), 23, 0);
                    }
                }
                if (AITimer >= 210)
                {
                    ResetState();
                }
            }
            else if (AIState == GeyserSweep)
            {
                if (AITimer < 80)
                {
                    rotation = -PiOver4;
                    if (AITimer < 20)
                        lastPos = player.Center;
                    Vector2 to = Helper.TRay.Cast(lastPos + new Vector2(750, -200), Vector2.UnitY, 800, true) - new Vector2(200);
                    if (NPC.Distance(to) > 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.1f);
                    else NPC.velocity *= 0.9f;
                    Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedByRandom(PiOver2 * 0.7f);
                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10), newColor: Color.LawnGreen, Scale: Main.rand.NextFloat(0.06f, 0.2f)).customData = NPC.Center + new Vector2(0, 20);

                    Dust.NewDustPerfect(pos, DustID.CursedTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                }
                if (AITimer == 85)
                {
                    bloomAlpha = 1;
                    glareAlpha = 1;
                }
                if (AITimer > 100)
                {
                    Vector2 to = Helper.TRay.Cast(lastPos + new Vector2(-750, -400), Vector2.UnitY, 800, true) - new Vector2(200);
                    if (NPC.Distance(to) > 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.02f);
                    else
                    {
                        AITimer++;
                        NPC.velocity *= 0.9f;
                    }


                    Projectile.NewProjectile(null, NPC.Center + Vector2.UnitY.RotatedByRandom(PiOver4) * 20, Vector2.UnitY.RotatedBy(NPC.rotation).RotatedByRandom(PiOver4) * Main.rand.NextFloat(3, 8), ProjectileType<TFlameThrower>(), 23, 0);
                    if (AITimer % 3 == 0)
                        SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                    if (AITimer % 9 == 0 && AITimer > 120 && NPC.Distance(to) > 200)
                    {
                        Projectile.NewProjectile(null, Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 500, true), -Vector2.UnitY, ProjectileType<TFlameThrower4>(), 23, 0);
                        Projectile.NewProjectile(null, Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 500, true), Vector2.UnitY, ProjectileType<TFlameThrower4>(), 23, 0);
                    }

                }
                if (AITimer >= 210)
                {
                    ResetState();
                }
            }
            else if (AIState == RangedHeadSlam)
            {
                SelectedClinger = 3;
                angry = true;
                if (AITimer == 1)
                {
                    lastPos = player.Center + new Vector2(Main.rand.NextFloat(-50, 50), 0);
                }
                Vector2 to = Helper.TRay.Cast(lastPos, Vector2.UnitY, 800, true) - new Vector2(0, 300);
                if (NPC.Distance(to) > 100)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.1f);
                else NPC.velocity *= 0.9f;

                if (AITimer >= 400)
                {
                    ResetState();
                }
            }
            else if (AIState == CursedDollCopy)
            {
                if (NPC.Distance(player.Center) > 200)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200)) * 30, 0.02f);
                else NPC.velocity *= 0.9f;
                if (AITimer < 175)
                {
                    if (AITimer == 1)
                    {
                        glareAlpha = 1;
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-1f).WithVolumeScale(1.6f), NPC.Center);
                        SoundEngine.PlaySound(EbonianSounds.shriek.WithPitchOffset(-0.6f).WithVolumeScale(1.6f), NPC.Center);
                    }
                    if (AITimer % 20 == 0)
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
                    angry = true;
                    if (AITimer % 5 == 0)
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<TerrortomaScream>(), 0, 0);
                }
                else angry = false;
                if (AITimer > 40 && AITimer % 12 == 0 && AITimer < 170)
                {
                    Vector2 pos = player.Center - new Vector2(Main.rand.NextFloat(-700, 700), 700);
                    Projectile.NewProjectile(null, pos, new Vector2(Helper.FromAToB(pos, player.Center).X * Main.rand.NextFloat(8), Main.rand.NextFloat(3, 10)), ProjectileType<TFlameThrower2>(), 20, 0);

                    pos = player.Center + new Vector2(900 * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), -20);
                    if (AITimer % 48 == 0)
                        Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center) * 16, ProjectileType<TerrorVilethorn1>(), 20, 0);
                }
                if (AITimer >= 180)
                {
                    ResetState();
                }
            }
            else if (AIState == ShadowOrbVomit)
            {
                SelectedClinger = 4;
                if (AITimer == 1)
                    bloomAlpha = 1f;
                rotation = Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2;
                if (AITimer == 40 || (AITimer > 80 && AITimer % 5 == 0 && AITimer <= 150))
                {
                    if (AITimer % 15 == 0 || AITimer == 40)
                        SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(.4f + Main.rand.NextFloat(-.3f, .1f)), NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (rotation + PiOver2).ToRotationVector2().RotatedByRandom(PiOver4) * 25, (NPC.rotation + MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(PiOver4 * 0.6f) * Main.rand.NextFloat(5, 20), ProjectileType<TerrorStaffPEvil>(), 20, 0, 0);
                }

                if (AITimer >= 180)
                {
                    ResetState();
                }
            }
            else if (AIState == TitteringSpawn)
            {
                SelectedClinger = 1;

                if (AITimer == 1)
                {
                    lastPos = player.Center + new Vector2(Main.rand.NextFloat(-50, 50), 0);
                }
                Vector2 to = Helper.TRay.Cast(lastPos, Vector2.UnitY, 800, true) - new Vector2(0, 200);
                if (NPC.Distance(to) > 100)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 30, 0.1f);
                else NPC.velocity *= 0.9f;


                if (AITimer >= 180)
                {
                    ResetState();
                }
            }
            else if (AIState == EyeHomingFlames)
            {
                Vector2 eyePos = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
                isLaughing = true;
                if ((AITimer % 30 == 0 || AITimer == 1) && AITimer <= 200)
                    SoundEngine.PlaySound(EbonianSounds.terrortomaLaugh, NPC.Center);

                if (AITimer == 1)
                    SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                if (AITimer == 55)
                {
                    glareAlpha = 1;
                    Projectile.NewProjectile(null, eyePos, Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                }
                if (AITimer == 100)
                    lastPos = player.Center;
                if (AITimer > 120)
                {
                    if (AITimer < 200 && AITimer % 15 == 0)
                    {
                        bloomAlpha = 1;
                        SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-1f), NPC.Center);
                        Projectile.NewProjectile(null, eyePos, Main.rand.NextVector2CircularEdge(5, 5) * Main.rand.NextFloat(0.5f, 1), ProjectileType<TFlameThrowerHoming>(), 23, 0);
                    }
                    if (AITimer < 170 && AITimer % 5 == 0)
                        Projectile.NewProjectile(null, eyePos, Helper.FromAToB(NPC.Center, player.Center) * Main.rand.NextFloat(0.5f, 1.5f), ProjectileType<RegorgerBolt>(), 23, 0);
                }

                if (AITimer >= 250)
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
            rotation = 0;
            NPC.velocity = Vector2.Zero;
            isLaughing = false;
            angry = false;
        }
        Vector2 lastPos;
    }
}