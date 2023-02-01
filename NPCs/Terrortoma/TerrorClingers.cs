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
using ExolRebirth.NPCs.Corruption;
using ExolRebirth.Projectiles.Terrortoma;

namespace ExolRebirth.NPCs.Terrortoma
{
    public class TerrorClingerRanged : ModNPC
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<Terrortoma>()], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("These Clingers help The Terrortoma in battle, this one shoots cursed flames at foes."),
            });
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clinger");
            Main.npcFrameCount[NPC.type] = 3;
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
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (NPC.frameCounter < 30)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
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
                    while (distance > 6 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 6;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                    }
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    }
                }
            }
            else if (center.ai[0] != 2 && center.ai[0] != 1 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 10 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
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
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)((Math.Cos(rotation) * 12f) * -1), (float)((Math.Sin(rotation) * 12f) * -1), ModContent.ProjectileType<TFlameThrower3>(), 35, 1f, Main.myPlayer)];
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
                    Projectile projectilee = Main.projectile[Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -5), ModContent.ProjectileType<TFlameThrower2>(), 35, 1f, Main.myPlayer)];
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

            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 6 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 6;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 18, 6), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(18 * 0.5f, 6 * 0.5f), 1f, SpriteEffects.None, 0);
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
                new FlavorTextBestiaryInfoElement("These Clingers help The Terrortoma in battle, this one spits out Ebon Flies to track down and kill foes."),
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
                    while (distance > 6 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 6;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                        NPC funny = Main.npc[NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<NPCs.Corruption.EbonFly>())];
                        funny.lifeMax = 450;
                        funny.life = 450;
                        funny.damage = 45;
                    }
                }
            }
            else if (center.ai[0] != 2 && center.ai[0] != 1 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
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
                    for (int i = 0; i < 5; i++)
                    {
                        float angle = Helper.CircleDividedEqually(i, 5);
                        Vector2 vel = Vector2.UnitX.RotatedBy(angle) * 5;
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

            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 6 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 6;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 18, 6), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(18 * 0.5f, 6 * 0.5f), 1f, SpriteEffects.None, 0);
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
                new FlavorTextBestiaryInfoElement("These Clingers help The Terrortoma in battle, this one charges at nearby foes with incredible speed."),
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
            NPC.width = 46;
            NPC.height = 42;
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
        private bool IsDashing = false;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC center = Main.npc[(int)NPC.ai[0]];
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
            NPC.damage = (int)center.localAI[0];
            if (!center.active || center.type != ModContent.NPCType<Terrortoma>())
            {
                NPC.life = 0;
            }
            if (center.ai[0] == -1)
            {
                IsDashing = false;
                NPC.velocity = Vector2.UnitY * 5;
                NPC.rotation += MathHelper.ToRadians(3);
                if (center.ai[1] == 1)
                {
                    Vector2 neckOrigin = terrortomaCenter;
                    Vector2 NPCcenter = NPC.Center;
                    Vector2 distToProj = neckOrigin - NPC.Center;
                    float projRotation = distToProj.ToRotation() - 1.57f;
                    float distance = distToProj.Length();
                    while (distance > 6 && !float.IsNaN(distance))
                    {
                        distToProj.Normalize();
                        distToProj *= 6;
                        NPCcenter += distToProj;
                        distToProj = neckOrigin - NPCcenter;
                        distance = distToProj.Length();

                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPCcenter, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                    }
                }
                if (center.ai[1] == 350)
                {
                    NPC.life = 0;
                    NPC.checkDead();
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("ExolRebirth/EbonFlyGore3").Type, NPC.scale);
                    for (int i = 0; i < 10; i++)
                    {
                        Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.CursedTorch, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    }
                }
            }
            else if (center.ai[0] != 2 && center.ai[0] != 1 && center.ai[0] != 8 && center.ai[0] != 5 && center.ai[0] != 3 && center.ai[0] != 14 && center.ai[0] != 13 && center.ai[0] != 7 && center.ai[0] != 12)
            {

                if (!IsDashing)
                {
                    Vector2 pos = center.Center + new Vector2(0, 85).RotatedBy(center.rotation);
                    Vector2 target = pos;
                    Vector2 moveTo = target - NPC.Center;
                    NPC.velocity = (moveTo) * 0.05f;
                }
                AITimer++;
                if (AITimer == 130)
                {
                    IsDashing = true;
                    NPC.velocity.X *= 0.98f;
                    NPC.velocity.Y *= 0.98f;
                    {
                        float rotation2 = (float)Math.Atan2((NPC.Center.Y) - (player.Center.Y), (NPC.Center.X) - (player.Center.X));
                        NPC.velocity.X = (float)(Math.Cos(rotation2) * 23) * -1;
                        NPC.velocity.Y = (float)(Math.Sin(rotation2) * 23) * -1;
                    }
                    Vector2 offset = new Vector2((float)Math.Cos(NPC.ai[0]), (float)Math.Sin(NPC.ai[0]));
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
                if (AITimer >= 160)
                {
                    IsDashing = false;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
            else if (center.ai[0] == 5 || center.ai[0] == 1 || center.ai[0] == 8 || center.ai[0] == 3)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 14 || center.ai[0] == 12)
            {
                Vector2 toPlayer = player.Center - NPC.Center;
                NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(0, 80).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
            }
            else if (center.ai[0] == 2)
            {
                AITimer = 0;
                Vector2 pos = center.Center + new Vector2(0, 5).RotatedBy(center.rotation);
                Vector2 target = pos;
                Vector2 moveTo = target - NPC.Center;
                NPC.velocity = (moveTo) * 0.09f;
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
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Vector2 drawOrigin = new Vector2(ModContent.Request<Texture2D>("ExolRebirth/NPCs/Terrortoma/TerrorClingerMelee").Value.Width * 0.5f, NPC.height * 0.5f);
            if (IsDashing)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] - pos + drawOrigin + new Vector2(0, NPC.gfxOffY);
                    spriteBatch.Draw(ModContent.Request<Texture2D>("ExolRebirth/NPCs/Terrortoma/TerrorClingerMelee").Value, drawPos, NPC.frame, Color.White * 0.5f, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0);
                }
            }
            if (Main.npc[(int)NPC.ai[0]].ai[0] == -1)
                return true;
            Vector2 neckOrigin = terrortomaCenter;
            Vector2 center = NPC.Center;
            Vector2 distToProj = neckOrigin - NPC.Center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            while (distance > 6 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 6;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("NPCs/Terrortoma/ClingerChain").Value, center - pos,
                    new Rectangle(0, 0, 18, 6), Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                    new Vector2(18 * 0.5f, 6 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            return true;

        }
    }
}