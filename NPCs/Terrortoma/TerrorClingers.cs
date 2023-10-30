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
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using System.Text.Encodings.Web;
using EbonianMod.Projectiles;
using System.Net.Http.Headers;
using Terraria.Audio;
using EbonianMod.Dusts;

namespace EbonianMod.NPCs.Terrortoma
{
    public class TerrorClingerRanged : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma with part of the Eater of World's spine. The glow tricks living things into thinking it's an eye, but it is a tiny spark of flame deep in its maw."),
            });
        }
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
        }
        //npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
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
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            float AIState = center.ai[0];
            float CenterAITimer = center.ai[1];
            terrortomaCenter = center.Center;
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
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

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    for (int i = 0; i < 10; i++)
                    {

                        Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Main.rand.NextVector2Unit() * 2f, ModContent.ProjectileType<TFlameThrower2>(), 0, 1f, Main.myPlayer)];
                        projectilee.tileCollide = false;
                        projectilee.timeLeft = 310;
                        projectilee.friendly = false;
                        projectilee.hostile = true;
                        Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    }
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
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.2f);
                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                NPC.ai[3] = 0;
            }
            else
            {
                if (AIState == 0 || AIState == 1 || AIState == 4 || AIState == 6)
                {
                    AITimer = 0;
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, center.rotation, 0.2f);
                    Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
                }
                if (center.ai[2] != 4)
                {
                    switch (AIState)
                    {
                        case 2:
                            if (CenterAITimer <= 300)
                            {
                                Vector2 pos = player.Center - new Vector2(0, 500).RotatedBy(MathHelper.ToRadians(AITimer * 3.6f));
                                Vector2 target = pos;
                                Vector2 moveTo = target - NPC.Center;
                                NPC.velocity = (moveTo) * 0.05f;

                                NPC.rotation = MathHelper.Lerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.2f);
                                AITimer++;
                                if (AITimer >= 30 && AITimer <= 90 && AITimer % 10 == 0)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 / 3) * Main.rand.NextFloat(5, 10), ModContent.ProjectileType<TFlameThrower3>(), 15, 0);
                                }
                                if (AITimer == 100)
                                {
                                    center.ai[2] = 1;
                                    AITimer = 0;
                                }
                            }
                            break;
                        case 3:
                            if (AITimer2 == 0 || (AITimer2 > 30 && AITimer2 < 40))
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, center.Center + new Vector2(85, -125).RotatedBy(center.rotation + MathHelper.ToRadians(AITimer * (5 + AITimer2 * 0.1f))), false) * 0.1f, 0.05f + AITimer * 0.03f);
                                AITimer++;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC npc = Main.npc[i];
                                    if (npc.active && npc.type == ModContent.NPCType<TerrorClingerMelee>())
                                    {
                                        if (npc.Center.Distance(NPC.Center) < npc.width)
                                        {
                                            for (int j = 0; j < 30; j++)
                                            {
                                                Dust.NewDustPerfect(NPC.Center + Helper.FromAToB(NPC.Center, npc.Center) * npc.width / 2, DustID.CursedTorch, (Main.rand.NextBool(5) ? Main.rand.NextVector2Unit() : Helper.FromAToB(NPC.Center, npc.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(1, 5)), Scale: Main.rand.NextFloat(2));
                                            }
                                            if (AITimer2 > 30)
                                            {
                                                for (int j = -5; j < 5; j++)
                                                {
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npc.Center, Vector2.UnitY.RotatedBy(MathHelper.ToRadians(MathHelper.Lerp(-100, 100, (float)(j + 5) / 10))) * 10, ModContent.ProjectileType<TFlameThrower3>(), 10, 0);
                                                }
                                            }
                                            else
                                            {
                                                for (int j = -3; j < 4; j++)
                                                {
                                                    if (j == 0) continue;
                                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), npc.Center, Vector2.UnitY.RotatedBy(j * 0.5f) * 10, ModContent.ProjectileType<TFlameThrower3>(), 10, 0);
                                                }
                                            }
                                            SoundEngine.PlaySound(new SoundStyle("EbonianMod/Sounds/NPCHit/fleshHit"), npc.Center);
                                            NPC.velocity = Helper.FromAToB(NPC.Center, center.Center + new Vector2(85, 85).RotatedBy(center.rotation)) * 20;
                                            AITimer2 = AITimer2 > 30 ? 41 : 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NPC.rotation = MathHelper.Lerp(NPC.rotation, center.rotation, 0.2f);
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

                                NPC.rotation = MathHelper.Lerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() - MathHelper.PiOver2, 0.2f);
                                AITimer++;
                                if (AITimer > 100) AITimer = 0;
                                if (AITimer >= 25)
                                {
                                    if (((float)(Math.Sin(Main.GameUpdateCount * 0.05f)) < -0.95f || (float)(Math.Sin(Main.GameUpdateCount * 0.05f)) > 0.95f))
                                    {
                                        if (AITimer2 == 0)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 / 3) * 5, ModContent.ProjectileType<TFlameThrower3>(), 15, 0);
                                            AITimer2 = 1;
                                        }
                                    }
                                    else AITimer2 = 0;
                                }
                            }
                            break;
                    }
                }
            }

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Player player = Main.player[NPC.target];

            if (NPC.IsABestiaryIconDummy)
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
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Bloom").Value;
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
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma with part of the Eater of World's spine. This creature in particular appears to contain a microcosm of corruption creatures, the apex predators being the tiny ebonflies it releases."),
            });
        }
        private float angle = 0;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 12323;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 46;
            NPC.height = 50;
            NPC.npcSlots = 1f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.netAlways = true;
        }
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
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            float AIState = center.ai[0];
            float CenterAITimer = center.ai[1];
            terrortomaCenter = center.Center;
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (NPC.HasValidTarget)
                {
                    AITimer = 0;
                }
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
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

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                        NPC funny = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NPCs.Corruption.EbonFly>())];
                        funny.lifeMax = 450;
                        funny.life = 450;
                        funny.damage = 45;
                    }
                }
            }
            if (center.ai[2] == 1 && NPC.ai[3] == 0)
            {
                bloomAlpha = 1f;
                NPC.ai[3] = 1;
            }
            if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
            if ((center.ai[2] != 1 && center.ai[2] <= 2) || center.ai[2] == 4)
            {
                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                NPC.ai[3] = 0;
            }
            else
            {
                if (AIState == 0 || AIState == 1 || AIState == 4 || AIState == 6)
                {
                    Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
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
                                if (AITimer == 50)
                                {
                                    float off = Main.rand.NextFloat(MathHelper.Pi);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), ModContent.NPCType<EbonFly>());
                                        //float angle = Helper.CircleDividedEqually(i, 6) + off;
                                        //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ModContent.ProjectileType<TSpike>(), 15, 0);
                                    }
                                }
                                if (AITimer == 80)
                                {
                                    float off = Main.rand.NextFloat(MathHelper.Pi);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center + Main.rand.NextVector2Circular(NPC.width / 2, NPC.height / 2), ModContent.NPCType<BloatedEbonfly>());
                                        //float angle = Helper.CircleDividedEqually(i, 8) + off;
                                        //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One.RotatedBy(angle), ModContent.ProjectileType<TSpike>(), 15, 0);
                                    }
                                }
                                if (AITimer >= 100)
                                {
                                    center.ai[2] = 2;
                                    AITimer = 0;
                                    AITimer2 = 0;
                                }
                            }
                            break;

                        case 3:
                            if (AITimer2 == 0 || (AITimer2 > 30 && AITimer2 < 40))
                            {
                                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, center.Center + new Vector2(-85, -125).RotatedBy(center.rotation + MathHelper.ToRadians(-AITimer * (5 + AITimer2 * 0.1f))), false) * 0.1f, 0.05f + AITimer * 0.03f);
                                AITimer++;
                                for (int i = 0; i < Main.maxNPCs; i++)
                                {
                                    NPC npc = Main.npc[i];
                                    if (npc.active && npc.type == ModContent.NPCType<TerrorClingerMelee>())
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
                                NPC.rotation = MathHelper.Lerp(NPC.rotation, center.rotation, 0.2f);
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
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Player player = Main.player[NPC.target];


            if (NPC.IsABestiaryIconDummy)
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
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Bloom").Value;
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 + new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale * 1.05f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return true;

        }
    }
    public class TerrorClingerMelee : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Organic Construct"),
                new FlavorTextBestiaryInfoElement("These creatures were the heads of infected corpses, they connect to the Terrortoma with part of the Eater of World's spine. This creature's body is made from the remains of a long extinct ancestor to all of the many eaters in this region."),
            });
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }
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
                spriteBatch.Draw(tex, lastPos - screenPos, null, Color.LawnGreen * 0.65f * alpha, Main.GameUpdateCount * 0.02f, tex.Size() / 2, 0.3f * alpha, SpriteEffects.None, 0);
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
        public override void AI()
        {
            Player player = Main.player[NPC.target];

            NPC center = Main.npc[(int)NPC.ai[0]];
            terrortomaCenter = center.Center;
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    NPC.velocity = new Vector2(0, 10f);
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    return;
                }
            }
            NPC.damage = (int)center.localAI[0];
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            float AIState = center.ai[0];
            float CenterAITimer = center.ai[1];
            if (center.ai[0] == -1)
            {
                center.ai[3] = 0;
                IsDashing = false;
                if (center.ai[1] > 100)
                {
                    NPC.velocity = Vector2.UnitY * 5;
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

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    }
                }
                if (center.ai[1] == 350)
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    }
                }
            }
            if (center.ai[2] == 2 && NPC.ai[3] == 0)
            {
                bloomAlpha = 1f;
                NPC.ai[3] = 1;
            }
            if (bloomAlpha > 0f) bloomAlpha -= 0.025f;
            if (alpha > 0f) alpha -= 0.01f;
            if ((center.ai[2] != 2 && center.ai[2] <= 2) || center.ai[2] == 4)
            {
                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.2f);
                Vector2 pos = center.Center + new Vector2(0, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                NPC.ai[3] = 0;
            }
            else
            {
                if (AIState == 0 || AIState == 1 || AIState == 4 || AIState == 6)
                {
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.2f);
                    Vector2 pos = center.Center + new Vector2(0, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
                }
                if (center.ai[2] != 4)
                {
                    switch (AIState)
                    {
                        case 2:
                            if (CenterAITimer <= 300)
                            {
                                NPC.damage = 40;
                                AITimer++;
                                if (AITimer == 50)
                                {
                                    NPC.velocity = Vector2.Zero;
                                    alpha = 1f;
                                    lastPos = player.Center;
                                }
                                if (AITimer == 75)
                                {
                                    NPC.velocity = Helper.FromAToB(NPC.Center, lastPos) * 30;
                                }
                                if (AITimer > 95 || AITimer < 50)
                                {
                                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.2f);
                                    Vector2 pos = center.Center + new Vector2(0, 85).RotatedBy(center.rotation);
                                    Vector2 target = pos;
                                    Vector2 moveTo = target - NPC.Center;
                                    NPC.velocity = (moveTo) * 0.05f;
                                }
                                else NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.ToRotation() - MathHelper.PiOver2, 0.1f);
                                if (AITimer >= 100)
                                {
                                    center.ai[2] = 4;
                                    AITimer = 0;
                                }
                            }
                            break;
                        case 3:
                            {
                                NPC.damage = 40;
                                NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.2f);
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
                                else NPC.damage = 40;
                                if (CenterAITimer == 41)
                                    bloomAlpha = 1f;
                                NPC.Center = Vector2.Lerp(NPC.Center, center.Center + new Vector2(0, Helper.TRay.CastLength(center.Center, Vector2.UnitY, 360)).RotatedBy((float)Math.Sin((float)Main.GlobalTimeWrappedHourly * 2)), 0.1f);
                                NPC.rotation = Helper.FromAToB(NPC.Center, center.Center + new Vector2(0, 340).RotatedBy((float)Math.Sin((float)Main.GlobalTimeWrappedHourly * 2))).ToRotation();
                                if (CenterAITimer > 369 + (center.life < center.lifeMax / 2 ? 50 : 0))
                                    NPC.damage = 0;
                            }
                            break;
                    }
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Player player = Main.player[NPC.target];

            if (NPC.IsABestiaryIconDummy)
                return true;
            Vector2 drawOrigin = new Vector2(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrorClingerMelee").Value.Width * 0.5f, NPC.height * 0.5f);
            if (IsDashing)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - pos + drawOrigin + new Vector2(0, NPC.gfxOffY);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/NPCs/Terrortoma/TerrorClingerMelee").Value, drawPos, NPC.frame, Color.White * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
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
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 26, 20), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(26 * 0.5f, 20 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            Texture2D tex = ModContent.Request<Texture2D>(Texture + "_Bloom").Value;
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.LawnGreen * bloomAlpha, NPC.rotation, tex.Size() / 2 - new Vector2(0, 2).RotatedBy(NPC.rotation), NPC.scale * 1.05f, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return true;

        }
    }
}