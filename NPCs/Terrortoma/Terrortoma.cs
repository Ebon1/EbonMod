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
                new FlavorTextBestiaryInfoElement("The Corruption is not capable of thought and only acts based on stimulus, it created the Terrortoma from the corpses of infected creatures specifically as a defense from the creatures released from the demise of the Wall of Flesh."),
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
            NPC.lifeMax = 4750;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 118;
            NPC.height = 106;
            NPC.rarity = 999;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.boss = false;
            NPC.BossBar = ModContent.GetInstance<TerrortomaBar>();
            SoundStyle hit = new("EbonianMod/Sounds/NPCHit/TerrorHit");
            SoundStyle death = new("EbonianMod/Sounds/NPCHit/TerrorDeath");
            NPC.HitSound = hit;
            NPC.DeathSound = death;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.alpha = 255;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Player player = Main.player[NPC.target];
            if ((player.ZoneCorrupt && AIState != -12124) || NPC.IsABestiaryIconDummy)
            {
                Vector2 drawOrigin = new Vector2(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value.Width * 0.5f, NPC.height * 0.5f);
                /*if ()
                {
                    for (int k = 0; k < NPC.oldPos.Length; k++)
                    {
                        Vector2 drawPos = NPC.oldPos[k] - pos + drawOrigin + new Vector2(0, NPC.gfxOffY);
                        spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value, drawPos, NPC.frame, lightColor * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                    }
                }*/
                spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/Terrortoma").Value, NPC.Center - pos, NPC.frame, lightColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value, NPC.Center - pos, null, lightColor, NPC.rotation, ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/trollface").Value.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            if (player.ZoneCorrupt && AIState != -12124)
            {
                Texture2D tex = Helper.GetTexture("NPCs/Terrortoma/TerrorEye");
                Vector2 eyeOGPosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
                Vector2 eyePosition = NPC.Center - new Vector2(-7, 14).RotatedBy(NPC.rotation);
                if (NPC.IsABestiaryIconDummy)
                {
                    spriteBatch.Draw(tex, eyePosition - screenPos, null, Color.White, 0, Vector2.One * 2, 1, SpriteEffects.None, 0);
                }
                else
                {
                    Vector2 fromTo = Helper.FromAToB(eyeOGPosition, player.Center);
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
                    Main.spriteBatch.Reload(BlendState.AlphaBlend);
                }
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
        private bool isLaughing;
        private bool HasSummonedClingers = false;
        private const int AISlot = 0;
        private const int TimerSlot = 1;

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

        public const int ApeShitMode = 999;
        public const int Death = -1;
        public const int Intro = 0;
        private const int Vilethorn = 1;
        private const int ClingerAttack1 = 2;
        private const int ClingerAttack2 = 3;
        private const int CursedFlamesRain = 4;
        private const int ClingerAttack3 = 5;
        private const int Vilethorn2 = 6;
        float rotation;
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
                EbonianSystem.ChangeCameraPos(NPC.Center, 250);
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
        public override void AI()
        {
            NPC.rotation = MathHelper.Lerp(NPC.rotation, rotation, 0.35f);
            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || !player.ZoneCorrupt)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AIState = Intro;
                    AITimer = 0;
                }
                if (!player.active || player.dead || !player.ZoneCorrupt)
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
            if (NPC.life <= NPC.lifeMax - (NPC.lifeMax / 3) && !hasDonePhase2ApeShitMode)
            {
                glareAlpha = 1f;
                AIState = ApeShitMode;
                hasDonePhase2ApeShitMode = true;
                AITimer = 0;
            }
            if (NPC.alpha >= 255)
            {
                NPC.Center = new Vector2(player.Center.X, player.Center.Y - 230);
            }
            if (NPC.alpha <= 0 && AIState != Death && NPC.life < NPC.lifeMax)
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
                        AIState = Intro;
                    else
                        AIState = ApeShitMode;
                }
            }
            else if (AIState == Death)
            {
                SelectedClinger = 4;
                rotation = 0;
                NPC.damage = 0;
                NPC.timeLeft = 2;
                AITimer++;
                if (AITimer == 250)
                    NPC.velocity = Vector2.UnitY;

                if (AITimer >= 250)
                {
                    NPC.velocity *= 1.025f;
                    rotation += MathHelper.ToRadians(3 * NPC.velocity.Y);
                    if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, Main.screenWidth) < NPC.width / 2)
                    {
                        Projectile.NewProjectileDirect(NPC.InheritSource(NPC), Helper.TRay.Cast(NPC.Center, Vector2.UnitY, Main.screenWidth), Vector2.Zero, ModContent.ProjectileType<TExplosion>(), 0, 0).scale = 2f;
                        SoundEngine.PlaySound(new("EbonianMod/Sounds/Eggplosion"));
                        NPC.immortal = false;
                        NPC.life = 0;
                        NPC.checkDead();
                    }
                }
            }
            else if (AIState == Intro)
            {
                SelectedClinger = 4;
                if (NPC.life < NPC.lifeMax)
                    AITimer++;
                isLaughing = true;
                rotation = 0;
                if (AITimer == 1)
                {
                    NPC.boss = true;
                    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/EvilMiniboss");
                    Helper.SetBossTitle(120, "Terrortoma", Color.LawnGreen, "The Conglomerate", 0);
                    EbonianSystem.ChangeCameraPos(NPC.Center, 120);
                    //add sound later
                }
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
                if (AITimer >= 150)
                {
                    AIState = Vilethorn;
                    AITimer = 0;
                    isLaughing = false;
                }
            }
            else if (AIState == ApeShitMode)
            {
                NPC.velocity = Vector2.Zero;
                isLaughing = false;
                Vector2 toPlayer = player.Center - NPC.Center;
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 60)
                {
                    AIState = Vilethorn2;
                    AITimer = 0;
                }
            }
            else if (AIState == Vilethorn)
            {
                SelectedClinger = 4;
                NPC.damage = 0;
                NPC.localAI[0] = 40;
                if (AITimer <= 250)
                {
                    if (++AITimer2 % 25 == 0)
                    {
                        Vector2 rainPos = new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y + Main.screenHeight + 300);
                        Vector2 rainPos2 = new Vector2(Main.screenPosition.X + Main.screenWidth * Main.rand.NextFloat(), Main.screenPosition.Y - 300);
                        if (Main.rand.NextBool())
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, -10), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, new Vector2(0, 10), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);
                    }
                }
                if (AITimer >= 250)
                {
                    Vector2 toPlayer = player.Center - NPC.Center;
                    rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                    NPC.velocity *= 0.9f;
                    isLaughing = true;
                }
                else
                    rotation = 0;
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 290)
                {
                    AIState = ClingerAttack1;
                    isLaughing = false;
                    AITimer = 0;
                    SelectedClinger = 0;
                }
            }
            else if (AIState == ClingerAttack1)
            {
                NPC.damage = 0;
                NPC.localAI[0] = 0;
                if (AITimer <= 300)
                {
                    Vector2 down = new Vector2(0, 10);
                    rotation = down.ToRotation() - MathHelper.PiOver2;
                    Vector2 pos = player.Center + new Vector2(0, -310);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.0545f;
                }
                if (AITimer >= 300)
                {
                    NPC.velocity *= 0.9f;
                    isLaughing = true;
                }
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 360)
                {
                    angle = 0;
                    AIState = ClingerAttack2;
                    isLaughing = false;
                    AITimer = 0;
                }
            }
            else if (AIState == ClingerAttack2)
            {
                SelectedClinger = 3;
                NPC.damage = 0;
                NPC.localAI[0] = 40;
                if (AITimer <= 5)
                {
                    Vector2 toPlayer = player.Center - NPC.Center;
                    rotation = 0;
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 340);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.0445f;
                }
                else
                {
                    NPC.velocity *= 0.5f;
                }
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 370)
                {
                    AIState = CursedFlamesRain;
                    isLaughing = false;
                    AITimer = 0;
                }
            }
            else if (AIState == CursedFlamesRain)
            {
                SelectedClinger = 4;
                NPC.damage = 0;
                Vector2 toPlayer = new Vector2(0, -10);
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                NPC.localAI[0] = 40;
                if (AITimer <= 150)
                {
                    Vector2 pos = new Vector2(player.position.X, player.position.Y - 340);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.0445f;
                    if (++AITimer2 % 60 == 0)
                    {
                        for (int i = 0; i <= 5 + (Main.expertMode ? 5 : 0); i++)
                        {
                            Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.NextFloat(-10f, 10f), -10), ModContent.ProjectileType<TFlameThrower2>(), 20, 1f, Main.myPlayer)];
                            projectile.tileCollide = false;
                            projectile.hostile = true;
                            projectile.friendly = false;
                            projectile.timeLeft = 230;
                        }
                    }
                }
                if (AITimer >= 150)
                {
                    NPC.velocity *= 0.9f;
                    isLaughing = true;
                }
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 270)
                {
                    AIState = ClingerAttack3;
                    isLaughing = false;
                    AITimer = 0;
                    AITimer2 = 0;
                }
            }
            else if (AIState == ClingerAttack3)
            {
                SelectedClinger = 3;
                NPC.damage = 0;
                NPC.localAI[0] = 40;
                Vector2 toPlayer = player.Center - NPC.Center;
                rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = new Vector2(player.position.X, player.position.Y - 280);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.0545f;
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 370)
                {
                    NPC.velocity = Vector2.Zero;
                    if (hasDonePhase2ApeShitMode)
                        AIState = Vilethorn2;
                    else AIState = Vilethorn;
                    isLaughing = false;
                    AITimer = 0;
                }
            }
            else if (AIState == Vilethorn2)
            {
                SelectedClinger = 4;
                NPC.damage = 0;
                NPC.localAI[0] = 40;
                if (AITimer <= 250)
                {
                    if (++AITimer2 % 20 == 0)
                    {
                        Vector2 rainPos = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y + 1080 + 300);
                        Vector2 rainPos2 = new Vector2(Main.screenPosition.X + 1920 * Main.rand.NextFloat(), Main.screenPosition.Y - 300);
                        Vector2 rainPos3 = new Vector2(Main.screenPosition.X - 300, Main.screenPosition.Y + 1080 * Main.rand.NextFloat());
                        Vector2 rainPos4 = new Vector2(Main.screenPosition.X + 1920 + 300, Main.screenPosition.Y + 1080 * Main.rand.NextFloat());
                        if (Main.rand.NextBool())
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos, new Vector2(0, -10), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);
                        else
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos2, new Vector2(0, 10), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);

                        if (Main.rand.NextBool(3))
                        {
                            if (Main.rand.NextBool())
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos3, new Vector2(10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);
                            else
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), rainPos4, new Vector2(-10, 0), ModContent.ProjectileType<TerrorVilethorn1>(), 15, 0, 0);
                        }
                    }
                }
                if (AITimer >= 250)
                {
                    Vector2 toPlayer = player.Center - NPC.Center;
                    rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                    NPC.velocity *= 0.9f;
                    isLaughing = true;
                }
                else
                    rotation = 0;
                if (++AITimer >= (hasDonePhase2ApeShitMode ? 0 : 50) + 370)
                {
                    AIState = ClingerAttack1;
                    isLaughing = false;
                    AITimer = 0;
                    SelectedClinger = 0;
                }
            }
        }
        Vector2 lastPos;
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = Main.LocalPlayer;
            if (player.ZoneCorrupt && !NPC.AnyNPCs(ModContent.NPCType<Terrortoma>()))
            {
                return .15f;
            }
            else
            {
                return 0;
            }
        }
    }
}