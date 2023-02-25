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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clinger");
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
            NPC.width = 44;
            NPC.height = 40;
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
        private Vector2 terrortomaCenter;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
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
            else if (center.ai[0] != 2 && center.ai[0] != -12124 && center.ai[0] != 1 && center.ai[0] != 11 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 10 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                AITimer++;
                if (AITimer >= 180)
                {
                    float rotation = (float)Math.Atan2(NPC.Center.Y - (player.position.Y + (player.height * 0.5f)), NPC.Center.X - (player.position.X + (player.width * 0.5f)));
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)((Math.Cos(rotation) * 12f) * -1), (float)((Math.Sin(rotation) * 12f) * -1), ModContent.ProjectileType<TFlameThrower3>(), 35, 1f, Main.myPlayer)];
                }
                if (AITimer >= 183)
                {
                    AITimer = 0;
                }
            }
            else if (center.ai[0] == 5 || center.ai[0] == 1 || center.ai[0] == 8)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(80, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 2)
            {
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(5, 5).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 10)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                AITimer++;
                if (AITimer >= 90)
                {
                    float rotation = (float)Math.Atan2(NPC.Center.Y - (player.position.Y + (player.height * 0.5f)), NPC.Center.X - (player.position.X + (player.width * 0.5f)));
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)((Math.Cos(rotation) * 12f) * -1), (float)((Math.Sin(rotation) * 12f) * -1), ModContent.ProjectileType<TFlameThrower3>(), 20, 1f, Main.myPlayer)];
                }
                if (AITimer >= 95)
                {
                    AITimer = 0;
                }
            }
            else if (center.ai[0] == 3)
            {
                Vector2 toPlayer = new Vector2(0, -10);
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = new Vector2(player.Center.X, center.Center.Y - 100);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
                AITimer++;
                if (AITimer >= 80)
                {
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5), ModContent.ProjectileType<TFlameThrower2>(), 20, 1f, Main.myPlayer)];
                    projectilee.tileCollide = false;
                    projectilee.timeLeft = 310;
                    projectilee.friendly = false;
                    projectilee.hostile = true;
                }
                if (AITimer >= 90)
                {
                    AITimer = 0;
                }
            }
            else
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(30, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            /*else if (center.ai[0] == 9) {
		Vector2 toPlayer = new Vector2(0, -10);
		NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
             Vector2 pos = new Vector2(player.Center.X, center.Center.Y - 100);
            Vector2 target = pos;
            Vector2 moveTo = target - NPC.Center;
            NPC.velocity = (moveTo) * 0.09f;
        AITimer++;
        if (AITimer >= 40) {
			for (int i = 0; i <= Main.rand.Next(4, 8); i++)
			{
			Projectile projectile = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 6.5f * Utils.RotatedBy(NPC.DirectionTo(new Vector2(0, -10)), (double)(MathHelper.ToRadians(Main.rand.NextFloat(15f, 34f)) * (float)i), default(Vector2)), 95, 40, 1f, Main.myPlayer)];
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.friendly = false;
            projectile.timeLeft = 230;
			}
            AITimer = 0;    
            }
            }*/
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
            DisplayName.SetDefault("Clinger");
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
        private Vector2 terrortomaCenter;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC center = Main.npc[(int)NPC.ai[0]];
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
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
            else if (center.ai[0] != 2 && center.ai[0] != -12124 && center.ai[0] != 1 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
            {
                angle = 0;
                Vector2 pos = center.Center + new Vector2(-85, 85).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.05f;
                AITimer++;
                if (AITimer >= 240)
                {
                    NPC funny = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NPCs.Corruption.EbonFly>())];
                    funny.lifeMax = 450;
                    funny.life = 450;
                    funny.damage = 45;
                    AITimer = 0;
                }
            }

            else if (center.ai[0] == 5 || center.ai[0] == 1 || center.ai[0] == 8)
            {
                angle = 0;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(-80, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 2)
            {
                angle = 0;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(-5, 5).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 3 /*|| center.ai[0] == 9*/)
            {
                angle += 0.05f;
                Vector2 pos = center.Center + Vector2.One.RotatedBy(angle) * 100;
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
                AITimer++;
                if (AITimer >= 45)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 3) + Main.rand.NextFloat(MathHelper.Pi * 2);
                        Vector2 vel = Vector2.UnitX.RotatedBy(angle) * 1;
                        Helper.SpawnTelegraphLine(NPC.Center, vel);
                        Projectile.NewProjectile(NPC.InheritSource(center), NPC.Center, vel, ModContent.ProjectileType<TSpike>(), 15, 0);
                    }
                    NPC funny = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EbonFly>())];
                    funny.lifeMax = 200;
                    funny.life = 200;
                    funny.damage = 45;
                    AITimer = 0;
                }
            }
            else
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(-30, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
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
            DisplayName.SetDefault("Clinger");
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
            if (center.ai[3] > 20)
            {
                Texture2D tex = Helper.GetExtraTexture("Extras2/magic_03");
                spriteBatch.Reload(BlendState.Additive);
                alpha = MathHelper.Clamp(alpha, 0, 1f);
                spriteBatch.Draw(tex, lastPos - screenPos, null, Color.LawnGreen * 0.65f, Main.GameUpdateCount * 0.02f, tex.Size() / 2, 0.3f * alpha, SpriteEffects.None, 0);
                spriteBatch.Reload(BlendState.AlphaBlend);
            }
        }
        float alpha;
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
            else if (center.ai[0] != 0 && center.ai[0] != 2 && center.ai[0] != -12124 && center.ai[0] != 1 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
            {

                if (!IsDashing)
                {
                    Vector2 pos = center.Center + new Vector2(0, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
                }
                center.ai[3]++;
                if (NPC.Center.Distance(player.Center) > 750)
                    center.ai[3] = -400;
                if (center.ai[3] > 20 && center.ai[3] < 40)
                    alpha += 0.1f;
                else if (center.ai[3] > 40)
                    alpha -= 0.1f;
                if (center.ai[3] == 20)
                {
                    lastPos = player.Center;
                }
                if (center.ai[3] == 40)
                {
                    IsDashing = true;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y *= 0.98f;
                    float rotation2 = (float)Math.Atan2((NPC.Center.Y) - (lastPos.Y), (NPC.Center.X) - (lastPos.X));
                    NPC.velocity.X = (float)(Math.Cos(rotation2) * 20) * -1;
                    NPC.velocity.Y = (float)(Math.Sin(rotation2) * 20) * -1;
                }
                if (IsDashing)
                {
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
                }
                else
                {
                    Vector2 toPlayer = player.Center - NPC.Center;
                    NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                }
                if (center.ai[3] >= 60)
                {
                    alpha = 0;
                    IsDashing = false;
                    NPC.velocity = Vector2.Zero;
                    center.ai[3] = 0;
                }
            }
            else if (center.ai[0] == 5 || center.ai[0] == 1 || center.ai[0] == 8 || center.ai[0] == 3)
            {
                center.ai[3] = 0;
                IsDashing = false;
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 14 || center.ai[0] == 12)
            {
                center.ai[3] = 0;
                IsDashing = false;
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 2)
            {
                center.ai[3] = 0;
                IsDashing = false;
                Vector2 pos = center.Center + new Vector2(0, 5).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else
            {
                center.ai[3] = 0;
                IsDashing = false;
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
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
            return true;

        }
    }
}