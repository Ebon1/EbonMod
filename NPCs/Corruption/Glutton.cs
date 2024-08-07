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

namespace EbonianMod.NPCs.Corruption
{
    public class Glutton : ModNPC
    {
        public override bool? CanFallThroughPlatforms()
        {
            return Main.player[NPC.target].Bottom.Y < NPC.Center.Y;
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
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
            NPC.defense = 5;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0;
            NPC.aiStyle = 0;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
        public const int ActualWidth = 170;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            NPC.frame.Width = ActualWidth;
            NPC.frame.X = (int)AIState * ActualWidth;
            if (!NPC.collideY)
            {
                NPC.frame.Y = 0;
            }
            else
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (AIState == 0)
                    {
                        if (NPC.frame.Y < 5 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                    else
                    {
                        if (NPC.frame.Y < 8 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                }
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneCorrupt)
            {
                return .05f;
            }
            else
            {
                return 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color lightColor)
        {
            Texture2D drawTexture = Helper.GetTexture("NPCs/Corruption/Glutton");
            Vector2 origin = new Vector2((drawTexture.Width / 2) * 0.5F, (drawTexture.Height / Main.npcFrameCount[NPC.type]) * 0.5F);

            Vector2 drawPos = new Vector2(
                NPC.position.X - pos.X + (NPC.width / 2) - (Helper.GetTexture("NPCs/Corruption/Glutton").Width / 2) * NPC.scale / 2f + origin.X * NPC.scale,
                NPC.position.Y - pos.Y + NPC.height - Helper.GetTexture("NPCs/Corruption/Glutton").Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + origin.Y * NPC.scale + NPC.gfxOffY);

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(drawTexture, drawPos, NPC.frame, lightColor, NPC.rotation, origin, NPC.scale, effects, 0);
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
        const int Walk = 0, Attack = 1;
        public override void HitEffect(NPC.HitInfo hit)
        {
            if ((hit.Damage >= NPC.life && NPC.life <= 0) || hit.InstantKill)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center - new Vector2(0, 25), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/GluttonGore1").Type, NPC.scale);
                for (int i = 0; i < 2; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/GluttonGore5").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/GluttonGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/GluttonGore3").Type, NPC.scale);
                    for (int j = 0; j < 2; j++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(0, 25), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/GluttonGore4").Type, NPC.scale);
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            if (AIState == Walk)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                if (NPC.collideY && NPC.collideX)
                    NPC.velocity.Y = -10;
                if (player.Center.Distance(NPC.Center) < NPC.width * 2)
                {
                    AITimer++;
                }
                if (player.Center.Distance(NPC.Center) < NPC.width / 2)
                {
                    AITimer = 300;
                }
                NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * 3;
                if (AITimer >= 300)
                {
                    AITimer = 0;
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 0;
                    AIState = Attack;
                    NPC.velocity = Vector2.Zero;
                }
            }
            else
            {
                AITimer++;
                if (AITimer == 30 || AITimer == 75)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(0, 0), ModContent.ProjectileType<FatSmash>(), 20, 0, 0, 0);
                }
                if (AITimer >= 85)
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
        public override void SetStaticDefaults()
        {
        }

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
