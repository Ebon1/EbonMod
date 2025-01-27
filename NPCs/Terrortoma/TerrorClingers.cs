using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Projectiles.Terrortoma;
using System.Text.Encodings.Web;
using EbonianMod.Projectiles;
using System.Net.Http.Headers;
using Terraria.Audio;
using EbonianMod.Dusts;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Items.Misc;
using Terraria.GameContent;
using EbonianMod.NPCs.Corruption.Ebonflies;
using EbonianMod.NPCs.Corruption;

namespace EbonianMod.NPCs.Terrortoma
{
    public class TerrorClingerRanged : ModNPC
    {
        public override void SetStaticDefaults()
        {

            NPCID.Sets.MustAlwaysDraw[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma with part of the Eater of World's spine. The glow tricks living things into thinking it's an eye, but it is a tiny spark of flame deep in its maw."),
            });
        }
        float lerpSpeed;
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 12323;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 54;
            NPC.height = 58;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index) => Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        //npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;

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
        float AITimer3;
        private Vector2 terrortomaCenter;
        float bloomAlpha;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.damage = 0;
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (!center.active || center.type != NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            lerpSpeed = Lerp(lerpSpeed, (center.ai[0] == 0 ? 0.05f : 0.2f), 0.1f);
            float AIState = center.ai[0];
            bool phase2 = center.life <= center.lifeMax - center.lifeMax / 3 + 3500;
            float CenterAITimer = center.ai[1];
            terrortomaCenter = center.Center;
            if (!player.active || player.dead || center.ai[0] == -12124)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead || center.ai[0] == -12124)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
            if (center.ai[1] < 2)
            {
                AITimer = center.ai[1];
                AITimer2 = 0;
            }
            if (center.ai[0] == -1)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(80, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
                if (center.ai[1] == 50)
                {
                    Vector2 neckOrigin = terrortomaCenter;
                    Vector2 NPCcenter = NPC.Center;
                    Vector2 distToProj = neckOrigin - NPC.Center;
                    float projRotation = distToProj.ToRotation() - 1.57f;
                    float distance = distToProj.Length();
                    while (distance > 20 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 20;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    EbonianSystem.ScreenShakeAmount += 5f;
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                }
            }
            if (center.ai[2] == 0 && NPC.ai[3] == 0 && AIState != 0)
            {
                bloomAlpha = 1f;
                NPC.ai[3] = 1;
            }
            if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
            if ((center.ai[2] != 0 && center.ai[2] <= 2) || center.ai[2] == 4)
            {
                NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                if (AIState == 6 || AIState == 14)
                {
                    pos = center.Center - new Vector2(85, 85).RotatedBy(center.rotation);
                }
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * lerpSpeed;
                NPC.ai[3] = 0;
            }
            else
            {
                if (AIState == 0 || AIState == 1 || AIState == 15 || AIState == 9 || AIState == 14 || AIState == 10 || AIState == 13 || AIState == 11 || AIState == -2 || AIState == 4 || AIState == 999)
                {
                    AITimer3 = 0;
                    AITimer = 0;
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                    Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * lerpSpeed;
                }
                if (center.ai[2] != 4)
                {
                    switch (AIState)
                    {
                        case 2:
                            AITimer++;
                            if (AITimer > 30)
                            {
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.2f);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(120).RotatedBy(MathHelper.ToRadians(AITimer * 4f)), false) * 0.25f, 0.3f);
                                if (AITimer % 10 == 0 && AITimer > 50)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center) * 2, ProjectileType<TFlameThrower4>(), 20, 0, ai2: AITimer - 100);
                                }
                            }
                            else
                            {
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = (moveTo) * lerpSpeed;
                            }
                            if (AITimer >= 101)
                            {
                                lerpSpeed = 0.005f;
                                center.ai[2] = 4;
                                AITimer = 0;
                            }
                            break;
                        case 3:
                            if (AITimer2 == 0 || (AITimer2 > 30 && AITimer2 < 40))
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, center.Center + new Vector2(105, -125).RotatedBy(center.rotation + MathHelper.ToRadians(AITimer * (5 + AITimer2 * 0.1f))), false) * 0.1f, 0.1f + AITimer * 0.03f);
                                AITimer++;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC npc = Main.npc[i];
                                    if (npc.active && npc.type == NPCType<TerrorClingerMelee>())
                                    {
                                        if (npc.Center.Distance(NPC.Center) < npc.width)
                                        {
                                            for (int j = 0; j < 30; j++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center + Helper.FromAToB(NPC.Center, npc.Center) * npc.width / 2, DustID.CursedTorch, (Main.rand.NextBool(5) ? Main.rand.NextVector2Unit() : Helper.FromAToB(NPC.Center, npc.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 5)), Scale: Main.rand.NextFloat(2));
                                            }
                                            if (AITimer2 > 30)
                                            {
                                                for (int j = -5 - phase2.ToInt(); j < 6 + phase2.ToInt(); j++)
                                                {
                                                    if (!phase2 && j == 0) continue;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npc.Center, Vector2.UnitY.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(-100, 100, (float)(j + 5) / 10))) * 10, ProjectileType<TFlameThrower3>(), 20, 0);
                                                }
                                            }
                                            else
                                            {
                                                for (int j = -3 - phase2.ToInt(); j < 4 + phase2.ToInt(); j++)
                                                {
                                                    if (!phase2 && j == 0) continue;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npc.Center, Vector2.UnitY.RotatedBy(j * 0.5f) * 10, ProjectileType<TFlameThrower3>(), 20, 0);
                                                }
                                            }
                                            SoundEngine.PlaySound(EbonianSounds.fleshHit, npc.Center);
                                            NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(85, 85).RotatedBy(center.rotation)) * 20;
                                            AITimer2 = AITimer2 > 30 ? 41 : 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = Vector2.Lerp(NPC.velocity, (moveTo) * 0.05f, 0.1f);

                                AITimer2++;
                                if (AITimer2 > 30)
                                {
                                    AITimer = 0;
                                }
                            }
                            break;
                        case 5:
                            {
                                Vector2 pos = player.Center - new Vector2((float)Math.Sin(Main.GameUpdateCount * 0.05f) * 120, 250);
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = (moveTo) * 0.05f;

                                NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.2f);
                                AITimer++;
                                if (AITimer > 100) AITimer = 0;
                                if (AITimer >= 25)
                                {
                                    if (((float)(Math.Sin(Main.GameUpdateCount * 0.05f)) < -0.95f || (float)(Math.Sin(Main.GameUpdateCount * 0.05f)) > 0.95f))
                                    {
                                        if (AITimer2 == 0)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 / 3) * 5, ProjectileType<TFlameThrower3>(), 25, 0);
                                            AITimer2 = 1;
                                        }
                                    }
                                    else AITimer2 = 0;
                                }
                            }
                            break;
                        case 12:
                            {
                                AITimer++;
                                if (AITimer < 80)
                                {
                                    AITimer3 = 0;
                                    if (AITimer > 30)
                                        NPC.velocity = Vector2.Lerp(NPC.velocity, -Vector2.UnitY.RotatedBy(PiOver4 * 0.3f) * 10, 0.02f);
                                    else
                                    {
                                        NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                                        Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                                        Vector2 target = pos;
                                        Vector2 moveTo = target - NPC.Center;
                                        NPC.velocity = (moveTo) * 0.1f;
                                    }
                                }
                                else if (AITimer > 80 && AITimer3 < 4)
                                {

                                    Vector2 to = Helper.TRay.Cast(NPC.Center - new Vector2(0, 50), Vector2.UnitY, 800);
                                    if (AITimer2 < 0.5f)
                                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, to) * 40, 0.05f);
                                    AITimer2 = Lerp(AITimer2, 0, 0.1f);
                                    if (NPC.Distance(to) < NPC.width && AITimer2 < 0.5f)
                                    {
                                        SoundEngine.PlaySound(EbonianSounds.eggplosion, NPC.Center);
                                        NPC.velocity = new Vector2(Main.rand.NextFloat(0, 50) * (Helper.FromAToB(NPC.Center, player.Center).X > 0 ? 1 : -1), -50); ;
                                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                                        center.velocity = Helper.FromAToB(NPC.Center, center.Center) * 15;
                                        AITimer3++;
                                        for (int i = -2; i < 2; i++)
                                        {
                                            Projectile.NewProjectile(null, NPC.Center, new Vector2(Main.rand.NextFloat(-1.25f, 1.25f), -1) * 5, ProjectileType<TFlameThrower4>(), 25, 0);
                                        }
                                        AITimer2 = 1;
                                    }
                                }
                                else
                                {
                                    NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                                    Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                                    Vector2 target = pos;
                                    Vector2 moveTo = target - NPC.Center;
                                    NPC.velocity = (moveTo) * 0.1f;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                    Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.1f;
                }
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            NPC _center = Main.npc[(int)NPC.ai[0]];
            if (_center.Distance(NPC.Center) > 2000 || (_center.ai[0] == 0 && _center.ai[1] < 2)) return true;
            Player player = Main.player[NPC.target];

            if (NPC.IsABestiaryIconDummy || NPC.Center == Vector2.Zero)
                return true;
            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 20 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 20;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain

                if (new Rectangle((int)center.X, (int)center.Y, 5, 5).Intersects(new Rectangle((int)Main.screenPosition.X - 200, (int)Main.screenPosition.Y - 200, Main.screenWidth + 200, Main.screenHeight + 200)))
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = Request<Texture2D>(Texture + "_Bloom").Value;
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 - new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale * 1.05f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return true;

        }
    }
    public class TerrorClingerSummoner : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma with part of the Eater of World's spine. This creature in particular appears to contain a microcosm of corruption creatures, the apex predators being the tiny ebonflies it releases."),
            });
        }
        private float angle = 0;
        public override void SetStaticDefaults()
        {

            NPCID.Sets.MustAlwaysDraw[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        float lerpSpeed;
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 12323;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 58;
            NPC.height = 62;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index) => Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        private const int TimerSlot = 1;

        public float AITimer
        {
            get => NPC.ai[TimerSlot];
            set => NPC.ai[TimerSlot] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        private Vector2 terrortomaCenter;
        float bloomAlpha;
        public override void AI()
        {
            Player player = Main.player[NPC.target];

            NPC center = Main.npc[(int)NPC.ai[0]];
            if (!center.active || center.type != NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            lerpSpeed = Lerp(lerpSpeed, (center.ai[0] == 0 ? 0.05f : 0.2f), 0.1f);
            float AIState = center.ai[0];
            bool phase2 = center.life <= center.lifeMax - center.lifeMax / 3 + 3500;
            float CenterAITimer = center.ai[1];
            terrortomaCenter = center.Center;
            if (!player.active || player.dead || center.ai[0] == -12124)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead || center.ai[0] == -12124)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
            if (center.ai[1] < 2)
            {
                AITimer = center.ai[1];
                AITimer2 = 0;
            }
            if (center.ai[0] == -1)
            {
                angle = 0;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(-80, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
                if (center.ai[1] == 150)
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    Vector2 neckOrigin = terrortomaCenter;
                    Vector2 NPCcenter = NPC.Center;
                    Vector2 distToProj = neckOrigin - NPC.Center;
                    float projRotation = distToProj.ToRotation() - 1.57f;
                    float distance = distToProj.Length();
                    while (distance > 20 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 20;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }

                    EbonianSystem.ScreenShakeAmount += 5f;
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    for (int i = 0; i < 5; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 20, 0, 0);
                        a.friendly = false;
                        a.hostile = true;
                    }
                }
            }
            if (center.ai[2] == 1 && NPC.ai[3] == 0)
            {
                if (center.ai[0] != 15)
                    bloomAlpha = 1f;
                NPC.ai[3] = 1;
            }
            if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
            if (AIState == -2)
            {
                if (CenterAITimer % 120 == 0)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), NPCType<BloatedEbonfly>()).dontTakeDamage = true;
                }
            }
            if ((center.ai[2] != 1 && center.ai[2] <= 2) || center.ai[2] == 4)
            {
                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                if (AIState == 6 || AIState == 14)
                {
                    pos = center.Center - new Vector2(-85, 85).RotatedBy(center.rotation);
                }
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * lerpSpeed;
                NPC.ai[3] = 0;
            }
            else
            {
                if (AIState == 0 || AIState == 1 || AIState == 12 || AIState == 9 || AIState == 14 || AIState == 10 || AIState == 13 || AIState == 11 || AIState == -2 || AIState == 4 || AIState == 999 || AIState == 5)
                {
                    Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * lerpSpeed;
                    NPC.scale = Lerp(NPC.scale, 1, 0.1f);
                }
                if (center.ai[2] != 4)
                {
                    switch (AIState)
                    {
                        case 2:
                            if (CenterAITimer <= 300)
                            {
                                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(MathHelper.ToRadians(Main.GameUpdateCount * 2));
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = (moveTo) * 0.05f;

                                AITimer++;
                                if (AITimer % 25 == 0)
                                {
                                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center) * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 20, 0, 0);
                                    a.friendly = false;
                                    a.hostile = true;
                                }
                                if (AITimer == 50)
                                {
                                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), NPCType<BloatedEbonfly>()).dontTakeDamage = true;
                                    //float angle = Helper.CircleDividedEqually(i, 6) + off;
                                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ProjectileType<TSpike>(), 15, 0);
                                }
                                if (AITimer == 80)
                                {
                                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), NPCType<BloatedEbonfly>()).dontTakeDamage = true;
                                    //float angle = Helper.CircleDividedEqually(i, 8) + off;
                                    //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ProjectileType<TSpike>(), 15, 0);
                                }
                                if (AITimer >= 100)
                                {
                                    lerpSpeed = 0.005f;
                                    center.ai[2] = 4;
                                    AITimer = 0;
                                    AITimer2 = 0;
                                }
                            }
                            break;

                        case 3:
                            if (AITimer2 == 0 || (AITimer2 > 30 && AITimer2 < 40))
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, center.Center + new Vector2(-105, -125).RotatedBy(center.rotation + MathHelper.ToRadians(-AITimer * (5 + AITimer2 * 0.1f))), false) * 0.1f, 0.1f + AITimer * 0.03f);
                                AITimer++;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC npc = Main.npc[i];
                                    if (npc.active && npc.type == NPCType<TerrorClingerMelee>())
                                    {
                                        if (npc.Center.Distance(NPC.Center) < npc.width)
                                        {
                                            for (int j = 0; j < 30; j++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center + Helper.FromAToB(NPC.Center, npc.Center) * npc.width / 2, DustID.CursedTorch, (Main.rand.NextBool(5) ? Main.rand.NextVector2Unit() : Helper.FromAToB(NPC.Center, npc.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 5)), Scale: Main.rand.NextFloat(2));
                                            }
                                            NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(-85, 85).RotatedBy(center.rotation)) * 20;
                                            AITimer2 = AITimer2 > 30 ? 41 : 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = Vector2.Lerp(NPC.velocity, (moveTo) * 0.05f, 0.1f);

                                AITimer2++;
                                if (AITimer2 > 30)
                                {
                                    AITimer = 0;
                                }
                            }
                            break;
                        case 15:
                            {
                                AITimer++;
                                if (AITimer < 100)
                                {
                                    if (AITimer < 85)
                                    {
                                        Vector2 pos = NPC.Center + new Vector2(0, 300).RotatedByRandom(TwoPi);
                                        Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10), newColor: Color.LawnGreen, Scale: Main.rand.NextFloat(0.06f, 0.2f)).customData = NPC.Center + new Vector2(0, 20);

                                        Dust.NewDustPerfect(pos, DustID.CursedTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(4, 10)).noGravity = true;
                                    }
                                    if (AITimer > 50)
                                        NPC.Center = savedP + Main.rand.NextVector2Circular(30, 30) * (NPC.scale - 1);
                                    NPC.scale = Lerp(NPC.scale, 2, 0.01f);

                                    if (AITimer == 50) savedP = NPC.Center;

                                    if (AITimer < 50)
                                    {
                                        Vector2 pos = center.Center + new Vector2(-150, -85).RotatedBy(center.rotation);
                                        Vector2 target = pos;
                                        Vector2 moveTo = target - NPC.Center;
                                        NPC.velocity = (moveTo) * 0.05f;
                                    }
                                    else
                                        NPC.velocity *= 0.8f;
                                }
                                else
                                {
                                    NPC.velocity *= 0.8f;
                                    if (AITimer < 107)
                                        NPC.scale = Lerp(NPC.scale, 0.5f, 0.5f);
                                    else
                                        NPC.scale = Lerp(NPC.scale, 1f, 0.1f);
                                }
                                if (AITimer == 100)
                                {
                                    Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);

                                    for (int i = 0; i < 5; i++)
                                    {
                                        float angle = Helper.CircleDividedEqually(i, 5);
                                        Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2() * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 24, 0, 0);
                                        a.friendly = false;
                                        a.hostile = true;
                                    }

                                    for (int i = 0; i < 5; i++)
                                    {
                                        float angle = Helper.CircleDividedEqually(i, 5);
                                        NPC tit = NPC.NewNPCDirect(null, NPC.Center, NPCType<Regorger>(), ai3: 1);
                                        tit.velocity = angle.ToRotationVector2() * 10;
                                        tit.lifeMax *= 6;
                                        tit.life = tit.lifeMax;
                                        //tit.Size
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
                }
            }
        }
        Vector2 savedP;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            NPC _center = Main.npc[(int)NPC.ai[0]];
            if (_center.Distance(NPC.Center) > 2000 || (_center.ai[0] == 0 && _center.ai[1] < 2)) return true;
            Player player = Main.player[NPC.target];


            if (NPC.IsABestiaryIconDummy || NPC.Center == Vector2.Zero)
                return true;
            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 20 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 20;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain

                if (new Rectangle((int)center.X, (int)center.Y, 5, 5).Intersects(new Rectangle((int)Main.screenPosition.X - 200, (int)Main.screenPosition.Y - 200, Main.screenWidth + 200, Main.screenHeight + 200)))
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                        new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                        new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = Request<Texture2D>(Texture + "_Bloom").Value;
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2, NPC.scale * 1.05f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return true;

        }
    }
    public class TerrorClingerMelee : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma, creating a sort of small hive mind together."),
            });
        }
        public override void SetStaticDefaults()
        {

            NPCID.Sets.MustAlwaysDraw[Type] = true;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Terrortoma/TerrorClinger_Bestiary",
                Position = new Vector2(7f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -45f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
        float lerpSpeed;
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 12323;
            NPC.dontTakeDamage = true;
            NPC.damage = 40;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 58;
            NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }

        private Vector2 terrortomaCenter;
        Vector2 lastPos;
        private bool IsDashing = false;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (alpha > 0)
            {
                Texture2D tex = Helper.GetExtraTexture("Extras2/magic_03");
                spriteBatch.Reload(BlendState.Additive);
                alpha = MathHelper.Clamp(alpha, 0, 1f);
                //spriteBatch.Draw(tex, lastPos - screenPos, null, Color.LawnGreen * 0.65f * alpha, Main.GameUpdateCount * 0.02f, tex.Size() / 2, 0.3f * alpha * 2, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        float alpha;
        float bloomAlpha;
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (NPC.Center == Vector2.Zero && center.Center != Vector2.Zero)
                NPC.Center = center.Center;
        }
        Vector2 savedP2;
        public override void AI()
        {

            NPC center = Main.npc[(int)NPC.ai[0]];
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            terrortomaCenter = center.Center;
            if (!player.active || player.dead || center.ai[0] == -12124)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || center.ai[0] == -12124)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
            if (center.ai[1] < 2)
            {
                AITimer = center.ai[1];
            }
            NPC.damage = (int)center.localAI[0];
            if (!center.active || center.type != NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            lerpSpeed = Lerp(lerpSpeed, (center.ai[0] == 0 ? 0.05f : 0.3f), 0.1f);
            float AIState = center.ai[0];
            bool phase2 = center.life <= center.lifeMax - center.lifeMax / 3 + 3500;
            float CenterAITimer = center.ai[1];
            if (center.ai[0] == -1)
            {
                center.ai[3] = 0;
                IsDashing = false;
                if (center.ai[1] > 100)
                {
                    NPC.velocity *= 1.025f;
                    NPC.rotation += MathHelper.ToRadians(3);
                }
                else
                {

                    Vector2 toPlayer = player.Center - NPC.Center;
                    NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                    Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.09f;
                }
                if (center.ai[1] == 100)
                {
                    NPC.velocity = Vector2.UnitY * 5;
                    EbonianSystem.ScreenShakeAmount += 5f;
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                    Vector2 neckOrigin = terrortomaCenter;
                    Vector2 NPCcenter = NPC.Center;
                    Vector2 distToProj = neckOrigin - NPC.Center;
                    float projRotation = distToProj.ToRotation() - 1.57f;
                    float distance = distToProj.Length();
                    while (distance > 20 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 20;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }
                }
                if (center.ai[1] == 350)
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    }
                }
            }
            else
            {
                if (center.ai[2] == 2 && NPC.ai[3] == 0)
                {
                    bloomAlpha = 1f;
                    NPC.ai[3] = 1;
                }
                if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
                if (alpha > 0f) alpha -= 0.01f;
                if ((center.ai[2] != 2 && center.ai[2] <= 2) || center.ai[2] == 4)
                {
                    alpha = 0;
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                    Vector2 pos = center.Center + new Vector2(0, 105).RotatedBy(center.rotation);
                    if (AIState == 6 || AIState == 14)
                    {
                        pos = center.Center - new Vector2(0, 85).RotatedBy(center.rotation);
                    }
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * lerpSpeed;
                    NPC.ai[3] = 0;
                }
                else
                {
                    if (AIState == 0 || AIState == 1 || AIState == 15 || AIState == 12 || AIState == 9 || AIState == 14 || AIState == 10 || AIState == 13 || AIState == 11 || AIState == -2 || AIState == 4 || AIState == 999)
                    {
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, center.rotation, 0.2f);
                        Vector2 pos = center.Center + new Vector2(0, 105).RotatedBy(center.rotation);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * lerpSpeed;
                    }
                    if (center.ai[2] != 4)
                    {
                        switch (AIState)
                        {
                            case 2:
                                if (CenterAITimer <= 300)
                                {
                                    NPC.damage = 100;
                                    AITimer++;
                                    if (AITimer < 75 && AITimer > 30)
                                    {
                                        NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.Center.FromAToB(player.Center).ToRotation() - MathHelper.PiOver2, 0.1f);
                                        if (AITimer < 45)
                                        {
                                            savedP2 = Helper.FromAToB(center.Center, NPC.Center, false);
                                            NPC.velocity -= Helper.FromAToB(NPC.Center, lastPos) * 1.6f;
                                        }
                                        else
                                        {
                                            NPC.velocity *= 0.8f;
                                            NPC.Center = center.Center + savedP2 + Main.rand.NextVector2Circular(SmoothStep(0, AITimer - 45, (AITimer - 45) / 30) * 0.9f, SmoothStep(0, AITimer - 45, (AITimer - 45) / 30) * 0.9f);
                                        }
                                    }
                                    if (AITimer < 65)
                                    {
                                        lastPos = player.Center + player.velocity * 4;
                                    }
                                    if (AITimer == 50)
                                    {
                                        NPC.velocity = Vector2.Zero;
                                        alpha = 1f;
                                    }
                                    if (AITimer == 75)
                                    {
                                        NPC.velocity = Helper.FromAToB(NPC.Center, lastPos) * 35;
                                    }
                                    if (AITimer > 75)
                                    {
                                        if (AITimer < 95)
                                            NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() - MathHelper.PiOver2, 0.1f);
                                        //NPC.Center += NPC.velocity * 0.75f;
                                        if (NPC.Center.Distance(lastPos) < NPC.width * 0.75f && AITimer < 90)
                                        {
                                            AITimer = 90;
                                        }
                                    }
                                    if (AITimer == 95)
                                        Projectile.NewProjectile(null, NPC.Center + NPC.velocity, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                                    if (AITimer > 95 || AITimer < 50)
                                    {
                                        NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.2f);
                                        Vector2 pos = center.Center + new Vector2(0, 105).RotatedBy(center.rotation);
                                        Vector2 target = pos;
                                        Vector2 moveTo = target - NPC.Center;
                                        NPC.velocity = (moveTo) * 0.05f;
                                    }
                                    if (AITimer >= 100)
                                    {
                                        lerpSpeed = 0.005f;
                                        center.ai[2] = 4;
                                        AITimer = 0;
                                    }
                                }
                                break;
                            case 3:
                                {
                                    NPC.damage = 100;
                                    NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.2f);
                                    Vector2 pos = center.Center + new Vector2(0, 125).RotatedBy(center.rotation);
                                    Vector2 target = pos;
                                    Vector2 moveTo = target - NPC.Center;
                                    NPC.velocity = (moveTo) * 0.1f;
                                }
                                break;
                            case 5:
                                {
                                    if (CenterAITimer < 40)
                                        NPC.damage = 0;
                                    else NPC.damage = 100;
                                    if (CenterAITimer == 41)
                                        bloomAlpha = 1f;
                                    NPC.Center = Vector2.Lerp(NPC.Center, center.Center + new Vector2(0, Helper.TRay.CastLength(center.Center, Vector2.UnitY, 360)).RotatedBy((float)Math.Sin((float)Main.GlobalTimeWrappedHourly * 2)), 0.1f);
                                    NPC.rotation = Helper.FromAToB(NPC.Center, center.Center + new Vector2(0, 340).RotatedBy((float)Math.Sin((float)Main.GlobalTimeWrappedHourly * 2))).ToRotation();
                                    if (CenterAITimer > 369 + (center.life < center.lifeMax / 2 ? 50 : 0))
                                        NPC.damage = 0;
                                }
                                break;
                            case 7:
                                AITimer++;
                                if (AITimer == 1)
                                    bloomAlpha = 1f;
                                if (AITimer == 30)
                                {
                                    NPC.velocity = Vector2.Zero;
                                    alpha = 1f;
                                    lastPos = Helper.TRay.Cast(NPC.Center, Vector2.Clamp(Helper.FromAToB(NPC.Center, player.Center), new Vector2(-0.45f, 1), new Vector2(0.45f, 1)), 2028);
                                }
                                if (AITimer > 30 && AITimer < 100)
                                {
                                    NPC.velocity += Helper.FromAToB(NPC.Center, lastPos, false) / 50f;
                                    if (NPC.Center.Distance(lastPos) < NPC.width)
                                    {
                                        AITimer = 100;
                                    }
                                }
                                else NPC.velocity *= 0.1f;
                                break;
                        }
                    }
                    else
                    {
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, 0, 0.2f);
                        Vector2 pos = center.Center + new Vector2(0, 105).RotatedBy(center.rotation);
                        Vector2 target = pos;
                        Vector2 moveTo = target - NPC.Center;
                        NPC.velocity = (moveTo) * 0.05f;
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            NPC _center = Main.npc[(int)NPC.ai[0]];
            if (_center.Distance(NPC.Center) > 2000 || (_center.ai[0] == 0 && _center.ai[1] < 2)) return true;
            Player player = Main.player[NPC.target];

            if (NPC.IsABestiaryIconDummy || NPC.Center == Vector2.Zero)
                return true;
            Vector2 drawOrigin = new Vector2(Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrorClingerMelee").Value.Width * 0.5f, NPC.height * 0.5f);
            if (IsDashing)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - pos + drawOrigin + new Vector2(0, NPC.gfxOffY);
                    spriteBatch.Draw(Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrorClingerMelee").Value, drawPos, NPC.frame, Color.White * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            if (Main.npc[(int)NPC.ai[0]].ai[0] == -1 && Main.npc[(int)NPC.ai[0]].ai[1] > 100)
                return true;
            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 20 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 20;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain

                if (new Rectangle((int)center.X, (int)center.Y, 5, 5).Intersects(new Rectangle((int)Main.screenPosition.X - 200, (int)Main.screenPosition.Y - 200, Main.screenWidth + 200, Main.screenHeight + 200)))
                    spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                        new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                        new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = Request<Texture2D>(Texture + "_Bloom").Value;
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 - new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale * 1.05f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return true;

        }
    }
}