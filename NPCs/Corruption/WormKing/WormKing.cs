using EbonianMod.Common.Systems;
using EbonianMod.Common.Systems.Misc;
using EbonianMod.Items.Misc;
using EbonianMod.NPCs.Corruption.Trumpet;
using EbonianMod.Projectiles.Friendly.Corruption;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.WormKing
{
    public class WormKing : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 8;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 2000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 10;
            NPC.knockBackResist = 0;
            NPC.width = 114;
            NPC.height = 116;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.buffImmune[24] = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.netAlways = true;
            NPC.hide = true;
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
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);
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
            if (hitinfo.Damage > NPC.life)
            {
                for (int i = 0; i < 18; i++)
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);

                if (verlet != null)
                {
                    for (int i = 0; i < verlet.points.Count; i++)
                    {
                        for (int j = 0; j < 8; j++)
                            Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Circular(3, 3), ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
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
                stalkBase = Helper.TRay.Cast(NPC.Center, direction, 200) + new Vector2(0, 100);
            }
            if (verlet != null)
                verlet.Update(stalkBase, NPC.Center);

            scaleX = MathHelper.Lerp(scaleX, 1, 0.1f);
            scaleY = MathHelper.Lerp(scaleY, 1, 0.1f);

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
                            if (player.Distance(NPC.Center) > 400)
                            {
                                AIState = Main.rand.Next(0, 4);
                            }
                            else
                            {
                                if (Main.rand.NextBool(4))
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
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
                            for (int i = 0; i < 10; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 10);
                                Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_Death(), NPC.Center, angle.ToRotationVector2().RotatedByRandom(0.5f) * Main.rand.NextFloat(5, 7), ModContent.ProjectileType<OstertagiWorm>(), 30, 0, 0);
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
                        if (AITimer % 30 == 0)
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
                            NPC.NewNPCDirect(null, NPC.Center, ModContent.NPCType<MiniTrumpetHead>());
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
                        if (AITimer % 15 == 0 && AITimer > 30)
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

                            NPC.NewNPCDirect(null, NPC.Center, ModContent.NPCType<Consumer>(), ai1: Main.rand.Next(30, 130));
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
                            Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<OstertagiExplosion>(), 0, 0, 0);
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
                            EbonianSystem.ScreenShakeAmount = 5;
                            NPC.velocity = Vector2.UnitY * -17.5f;

                            scaleX = 1.2f;
                            scaleY = 0.8f;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FatSmash>(), 0, 0, 0, 0);
                            SoundEngine.PlaySound(EbonianSounds.terrortomaFlesh, NPC.Center);
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
