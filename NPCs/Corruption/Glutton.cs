using Terraria;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using static Terraria.ModLoader.PlayerDrawLayer;
using EbonianMod.Dusts;
using EbonianMod.Items.Misc;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Terrortoma;
using System.Net.Sockets;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Enemy.Corruption;
using EbonianMod.Items.Materials;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.NPCs.Corruption
{
    public class Glutton : ModNPC
    {
        public override bool? CanFallThroughPlatforms()
        {
            return Main.player[NPC.target].Center.Y - 50 > NPC.Bottom.Y && AIState == 0;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(50f, 70),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("A towering beast with curious traits for a corruption entity, such as its shape and face. It appears to have been much simpler once, and the reason for its strengthening is unknown."),
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 150;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 15);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt && Main.hardMode ? 0.01f : 0;
        }
        public const int ActualWidth = 170;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frame.Width = ActualWidth;
            NPC.frame.X = (AIState == 0 ? 0 : 1) * ActualWidth;
            if (NPC.frameCounter % 5 == 0)
            {
                if (AIState == 0)
                {
                    if (!NPC.collideY)
                    {
                        NPC.frame.Y = 0;
                    }
                    else
                    {
                        if (NPC.frame.Y < 5 * frameHeight && !NPC.velocity.X.CloseTo(0, 0.1f))
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                }
                else if (AIState == 1)
                {
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
                else if (AIState == Conjure)
                {
                    if (AITimer < 50)
                    {
                        if (NPC.frame.Y < 4 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 2 * frameHeight;
                    }
                    else
                    {
                        if (NPC.frame.Y < 8 * frameHeight)
                            NPC.frame.Y += frameHeight;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 8 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<TerrortomaMaterial>(), 2, 1, 3));
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Corruption/Glutton");
            Texture2D glowTexture = Helper.GetTexture("NPCs/Corruption/Glutton_Glow");
            Texture2D bloomTexture = Helper.GetTexture("NPCs/Corruption/Glutton_Bloom");
            Vector2 origin = new Vector2((drawTexture.Width / 2) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 2) - (Helper.GetTexture("NPCs/Corruption/Glutton").Width / 2) * NPC.scale / 2f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Corruption/Glutton").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(glowTexture, drawPos, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Reload(BlendState.Additive);
            spriteBatch.Draw(bloomTexture, drawPos, NPC.frame, Color.White * eyeBeamAlpha, NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
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
        const int Walk = 0, GroundPound = 1, Conjure = 2, EyeBeam = 3;
        Vector2 storedPlayerPos;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if ((hit.Damage >= NPC.life && NPC.life <= 0))
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center - new Vector2(0, 25), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GluttonGore1").Type, NPC.scale);
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GluttonGore5").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GluttonGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GluttonGore3").Type, NPC.scale);
                    for (int j = 0; j < 2; j++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 25), Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GluttonGore4").Type, NPC.scale);
                }
            }
        }
        float eyeBeamAlpha = 0;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (player.Distance(NPC.Center) > 1800) return;
            NPC.TargetClosest(AIState != 3);
            if (AIState != 3)
                NPC.spriteDirection = NPC.direction;
            if (AIState == Walk)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                if ((NPC.collideY && NPC.collideX))
                    NPC.velocity.Y = -10;
                if (player.Center.Distance(NPC.Center) < NPC.width * 4)
                {
                    AITimer++;
                }
                if (player.Center.Distance(NPC.Center) < NPC.width)
                {
                    if (AITimer < 130)
                        AITimer = 130;
                }
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 55).X * 3, 0.1f);
                if (AITimer >= 200)
                {
                    AITimer = 0;
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                    if (NPC.collideY && player.Center.Y - NPC.Center.Y < -100)
                        AIState = (Main.rand.NextBool(3) ? EyeBeam : Conjure);
                    else
                        AIState = (player.Center.Distance(NPC.Center) > NPC.width ? (Main.rand.NextBool(3) ? EyeBeam : Conjure) : GroundPound);
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (AIState == GroundPound)
            {
                AITimer++;
                NPC.velocity.Y += .5f;
                if (AITimer == 30 || AITimer == 75)
                {

                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(NPC.direction * 60, Helper.TRay.CastLength(NPC.Bottom, Vector2.UnitY, 40, true)), new Vector2(0, 0), ProjectileType<GluttonImpact>(), 20, 2.5f, 0, 0);
                    a.friendly = false;
                    a.hostile = true;
                }
                if (AITimer >= 85)
                {
                    AITimer = 160;
                    NPC.frame.Y = 0;
                    AIState = Walk;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else if (AIState == Conjure)
            {
                AITimer++;
                if (AITimer == 65)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        Vector2 pos = NPC.Bottom + new Vector2(NPC.direction * Main.rand.NextFloat(-30, 30), 0);
                        Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center).RotatedBy(-MathHelper.PiOver4 * 0.3f).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(5, 10), ProjectileType<TerrorVilethorn1>(), 20, 0);
                    }

                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(NPC.direction * 60, Helper.TRay.CastLength(NPC.Bottom, Vector2.UnitY, 40, true)), new Vector2(0, 0), ProjectileType<GluttonImpact>(), 20, 2.5f, 0, 0);
                    a.friendly = false;
                    a.hostile = true;
                }
                if (AITimer >= 105)
                {
                    AITimer = 0;
                    NPC.frame.Y = 0;
                    AIState = Walk;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else
            {
                AITimer++;
                if (AITimer < 120 && AITimer > 60)
                    eyeBeamAlpha = MathHelper.Lerp(eyeBeamAlpha, 1, 0.1f);
                if (AITimer == 30)
                {

                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                    Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Bottom + new Vector2(NPC.direction * 60, Helper.TRay.CastLength(NPC.Bottom, Vector2.UnitY, 40, true)), new Vector2(0, 0), ProjectileType<GluttonImpact>(), 20, 2.5f, 0, 0);
                    a.friendly = false;
                    a.hostile = true;
                }
                if (AITimer == 45)
                    SoundEngine.PlaySound(EbonianSounds.BeamWindUp, NPC.Center);
                if (AITimer == 100)
                {
                    storedPlayerPos = player.Center;
                    Projectile.NewProjectile(null, NPC.Center + new Vector2(56 * NPC.direction, 22), Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                }
                if (AITimer == 90)
                    Projectile.NewProjectile(null, NPC.Center + new Vector2(56 * NPC.direction, 22), Vector2.Zero, ProjectileType<GreenChargeUp>(), 0, 0);
                if (AITimer == 155)
                    Projectile.NewProjectile(null, NPC.Center + new Vector2(56 * NPC.direction, 22), Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0);
                if (AITimer > 155 && AITimer <= 174)
                {
                    if (AITimer % 2 == 0)
                    {

                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                        SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(0.4f).WithVolumeScale(1.2f), NPC.Center);
                        Projectile.NewProjectile(null, NPC.Center + new Vector2(56 * NPC.direction, 22), Helper.FromAToB(NPC.Center + new Vector2(56 * NPC.direction, 22), storedPlayerPos).RotatedByRandom(MathHelper.PiOver4 * 0.35f) * Main.rand.NextFloat(2, 13), ProjectileType<TFlameThrower>(), 10, 0);
                    }
                    eyeBeamAlpha = MathHelper.Lerp(eyeBeamAlpha, 0, 0.2f);
                }
                if (AITimer >= 190)
                {
                    AITimer = 0;
                    NPC.frame.Y = 0;
                    AIState = Walk;
                    NPC.velocity = Vector2.Zero;
                }
            }
        }
    }
    public class FatSmash : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.aiStyle = 135;
            AIType = 683;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
        }
        public override void AI()
        {
        }
    }
}
