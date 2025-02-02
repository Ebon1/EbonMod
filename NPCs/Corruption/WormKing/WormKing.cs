using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc;
using EbonianMod.Items.Materials;
using EbonianMod.Items.Misc;
using EbonianMod.NPCs.Corruption.Ebonflies;
using EbonianMod.NPCs.Corruption.Rotling;
using EbonianMod.NPCs.Corruption.Trumpet;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.WormKing
{
    public class WormKing : ModNPC
    {
        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "EbonianMod/NPCs/Corruption/WormKing/WormKing_Bestiary",
                Position = new Vector2(0, 30f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 45f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
            Main.npcFrameCount[Type] = 8;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.WormKing.Bestiary"),
            });
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 2000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 15;
            NPC.knockBackResist = 0;
            NPC.width = 114;
            NPC.height = 116;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.netAlways = true;
            NPC.hide = true;
            NPC.value = Item.buyPrice(0, 20);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCorrupt && spawnInfo.Player.ZoneOverworldHeight ? 0.01f : 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.WormTooth, 1, 15, 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 1, 15, 30));
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 5 == 0)
            {
                if ((NPC.frame.Y += frameHeight) > 7 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
                verlet.Draw(spriteBatch, Texture + "_Chain");
            Texture2D texture = Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);


            if (Main.LocalPlayer.HasBuff(BuffID.Hunter) && !NPC.IsABestiaryIconDummy)
            {
                //if (verlet != null)
                //  verlet.Draw(spriteBatch, Texture + "_Chain", useColor: true, color: NPC.HunterPotionColor());

                spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, NPC.HunterPotionColor(), NPC.rotation, NPC.Size / 2, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);

            }
            return false;
        }
        Vector2 stalkBase;
        Verlet verlet;
        float scaleX = 1f, scaleY = 1f;
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override void OnSpawn(IEntitySource source)
        {
            verlet = new Verlet(NPC.Center, 15, 24, 2.25f, true, true, 15);
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life && NPC.life <= 0)
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                for (int i = 0; i < 18; i++)
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);

                if (verlet != null)
                {
                    for (int i = 0; i < verlet.points.Count; i++)
                    {
                        for (int j = 0; j < 8; j++)
                            Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Circular(3, 3), Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    }
                }
            }
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
        public const int Idle = 0, Ostertagi = 1, MiniTrumpets = 2, Eaters = 3, Slam = 4;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (stalkBase == Vector2.Zero)
            {
                Vector2 direction = Vector2.UnitY;
                int attempts = 0;
                stalkBase = Helper.TRay.Cast(NPC.Center + new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 0) - new Vector2(0, 500), direction, 2000) + new Vector2(0, 100);
            }
            if (AIState == Idle && Helper.TRay.CastLength(NPC.Top + new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 0), -Vector2.UnitY, NPC.height * 3) < NPC.height * 2.9f)
            {
                stalkBase = Helper.TRay.Cast(NPC.Center + new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 0) - new Vector2(0, 1400), Vector2.UnitY, 2000) + new Vector2(0, 100);
                NPC.Center = stalkBase - new Vector2(0, 800);
            }

            if (player.Distance(NPC.Center) > 1800) return;
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center);

            scaleX = MathHelper.Lerp(scaleX, 1, 0.1f);
            scaleY = MathHelper.Lerp(scaleY, 1, 0.1f);
            if (player.Distance(NPC.Center) < 600)
                AITimer++;
            switch (AIState)
            {
                case Idle:
                    {
                        NPC.ai[3]++;
                        NPC.Center = Vector2.Lerp(NPC.Center, stalkBase - new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 300 + (MathF.Cos(NPC.ai[3] * 0.02f) * 50)), 0.1f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, MathF.Sin(NPC.ai[3] * 0.02f), 0.1f);
                        if (AITimer > Main.rand.Next(200, 400))
                        {
                            if (player.Distance(NPC.Center) > 300)
                            {
                                if (Main.rand.NextBool(2))
                                    AIState = Ostertagi;
                                else
                                    AIState = Main.rand.Next(0, 4);
                            }
                            else
                            {
                                if (Main.rand.NextBool(2))
                                    AIState = Slam;
                                else
                                {
                                    AIState = Main.rand.Next(0, 4);
                                }
                            }
                            AITimer = 0;
                            AITimer2 = 0;
                        }
                    }
                    break;
                case Ostertagi:
                    {
                        if (AITimer < 40)
                        {
                            scaleX = MathHelper.Lerp(scaleX, 0.95f, 0.15f);
                            scaleY = MathHelper.Lerp(scaleY, 0.95f, 0.15f);
                        }
                        if (AITimer == 45)
                        {
                            scaleX = 1.15f;
                            scaleY = 0.85f;
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 10; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 10);
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(5, 7), ProjectileType<OstertagiWorm>(), 30, 0, 0);
                                a.friendly = false;
                                a.hostile = true;
                            }
                        }
                        if (AITimer > 90)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            AITimer2 = 0;
                        }
                    }
                    break;
                case MiniTrumpets:
                    {
                        NPC.ai[3]++;
                        NPC.Center = Vector2.Lerp(NPC.Center, stalkBase - new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 300 + (MathF.Cos(NPC.ai[3] * 0.02f) * 50)), 0.1f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, MathF.Sin(NPC.ai[3] * 0.02f), 0.1f);
                        if (AITimer % 15 == 0)
                        {
                            if (Main.rand.NextBool())
                            {
                                scaleX = 1.1f;
                                scaleY = 0.9f;
                            }
                            else
                            {
                                scaleX = 0.9f;
                                scaleY = 1.1f;
                            }
                            if (!NPC.Grounded())
                                NPC.NewNPCDirect(null, NPC.Center, NPCType<BloatedEbonfly>());
                        }
                        if (AITimer > 90)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            AITimer2 = 0;
                        }
                    }
                    break;
                case Eaters:
                    {
                        NPC.ai[3]++;
                        NPC.Center = Vector2.Lerp(NPC.Center, stalkBase - new Vector2(MathF.Sin(NPC.ai[3] * 0.02f) * 200, 300 + (MathF.Cos(NPC.ai[3] * 0.02f) * 50)), 0.1f);
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, MathF.Sin(NPC.ai[3] * 0.02f), 0.1f);
                        if (AITimer % 10 == 0 && AITimer > 30)
                        {
                            if (Main.rand.NextBool())
                            {
                                scaleX = 1.1f;
                                scaleY = 0.9f;
                            }
                            else
                            {
                                scaleX = 0.9f;
                                scaleY = 1.1f;
                            }

                            NPC.NewNPCDirect(null, NPC.Center, NPCType<RotlingHead>());
                        }
                        if (AITimer > 60)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            AITimer2 = 0;
                        }
                    }
                    break;
                case Slam:
                    {
                        if (AITimer == 10)
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                        if (AITimer <= 50 && AITimer >= 40)
                        {
                            NPC.velocity.Y += 3;
                            NPC.damage = 100;
                        }
                        if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, NPC.height * 2) < NPC.height && AITimer2 == 0)
                        {
                            if (AITimer < 50)
                                AITimer = 51;
                            AITimer2 = 1;

                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                            NPC.velocity = Vector2.UnitY * -17.5f;

                            scaleX = 1.2f;
                            scaleY = 0.8f;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<FatSmash>(), 0, 0, 0, 0);
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<GluttonImpact>(), 0, 0, 0, 0);
                            SoundEngine.PlaySound(EbonianSounds.terrortomaFlesh, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                        }
                        if (AITimer > 55 && AITimer2 == 1)
                        {
                            NPC.velocity *= 0.9f;
                            NPC.damage = 0;
                        }
                        if (AITimer > 90)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            NPC.velocity = Vector2.Zero;
                            AITimer2 = 0;
                        }
                    }
                    break;
            }
        }
    }
}
