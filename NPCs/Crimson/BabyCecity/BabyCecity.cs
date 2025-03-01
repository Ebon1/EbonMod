using EbonianMod.Common.Systems.Verlets;
using EbonianMod.Items.Materials;
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using StructureHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.NPCs.Crimson.BabyCecity
{
    public class BabyCecity : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 68;
            NPC.height = 76;
            NPC.damage = 15;
            NPC.defense = 18;
            NPC.lifeMax = 2300;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = EbonianSounds.fleshHit;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            NPC.buffImmune[BuffID.Confused] = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.buffImmune[BuffID.Ichor] = true;

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneCrimson && Main.hardMode && spawnInfo.Player.ZoneOverworldHeight ? 0.035f : 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<CecitiorMaterial>(), 2, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.Ichor, 1, 5, 15));
            npcLoot.Add(ItemDropRule.Common(ItemType<Items.Weapons.Melee.Spinax>(), 5));
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.BabyCecity.Bestiary"),
            });
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && verlet[0] != null)
                for (int i = 0; i < 2; i++)
                {
                    if (!Main.gamePaused)
                        for (int j = 0; j < 2; j++)
                            verlet[i].Update(NPC.Center, ogPos[i]);
                    verlet[i].Draw(spriteBatch, "NPCs/Crimson/BabyCecity/BabyCecity_Hook0", endTex: "NPCs/Crimson/BabyCecity/BabyCecity_Hook2");
                }
            return true;
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
        Verlet[] verlet = new Verlet[2];
        Vector2[] dir = new Vector2[2];
        Vector2[] ogPos = new Vector2[2];
        //float[] len = new float[3];
        public override void OnSpawn(IEntitySource source)
        {
            NPC.Center = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 1000) - Vector2.UnitY * Main.rand.NextFloat(200, 300);
            for (int i = 0; i < 2; i++)
            {
                verlet[i] = new Verlet(NPC.Center, 10, 42, gravity: -5f, lastPointLocked: true, stiffness: 50);
                dir[i] = -Helper.CircleDividedEqually(i + 1, 6).ToRotationVector2().RotatedBy(MathHelper.Pi);
                ogPos[i] = Helper.TRay.Cast(NPC.Center, dir[i], 350) + Vector2.UnitY * 30;
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/Crimorrhage" + i).Type);
                }
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < verlet[i].segments.Count; j++)
                        Gore.NewGore(NPC.GetSource_Death(), verlet[i].segments[j].pointA.position, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/CrimorrhageChain").Type);
                }
            }
        }
        int seed = 55;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (player.Distance(NPC.Center) > 1800) return;
            NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
            if (verlet[0] == null)
                return;
            if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 600) >= 580)
            {
                NPC.velocity.Y += 2;
                ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.TRay.Cast(new Vector2(ogPos[0].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
                ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.TRay.Cast(new Vector2(ogPos[1].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
            }
            NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() - MathHelper.PiOver2;
            switch (AIState)
            {
                case 0:
                    if (player.Center.Distance(NPC.Center) < 1300)
                        AITimer++;

                    if (NPC.ai[3] == 0)
                        NPC.ai[3] = 1;

                    UnifiedRandom rand = new(seed);
                    float legOffset = 1;
                    if (++AITimer2 < 80)
                    {

                        if (AITimer2 > 30 && AITimer2 % 2 == 0)
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.FromAToB(Helper.TRay.Cast(player.Center + new Vector2(0, -200), Vector2.UnitY, 800) - new Vector2(0, 100)) * 4, 0.2f);
                        else
                            NPC.velocity *= 0.95f;
                        if (NPC.ai[3] == 1)
                        {
                            if (NPC.velocity.X > 0 && ogPos[1].X > NPC.Center.X + 30)
                                legOffset = 0.6f;
                            else if (NPC.velocity.X < 0 && ogPos[1].X < NPC.Center.X - 30)
                                legOffset = 0.6f;
                            else if (NPC.velocity.X > 0 && ogPos[1].X < NPC.Center.X - 30 && ogPos[0].X < NPC.Center.X - 30)
                                legOffset = 1.4f;
                            else if (NPC.velocity.X < 0 && ogPos[1].X > NPC.Center.X + 30 && ogPos[0].X > NPC.Center.X + 30)
                                legOffset = 1.4f;
                            if (Helper.FromAToB(ogPos[0], player.Center).X != 0)
                            {
                                if (AITimer2 < 40)
                                    ogPos[0] += new Vector2(NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[0], player.Center).X, -MathHelper.SmoothStep(5, 2, AITimer2 / 50));
                                else
                                {
                                    if (AITimer2 < 60)
                                        ogPos[0] += new Vector2((NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[0], player.Center).X) * MathHelper.SmoothStep(1, 0, (AITimer2 - 40) * 20 / 40), 0);

                                    ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.TRay.Cast(new Vector2(ogPos[0].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
                                }
                            }
                        }
                        else
                        {
                            if (NPC.velocity.X > 0 && ogPos[0].X > NPC.Center.X + 30)
                                legOffset = 0.6f;
                            else if (NPC.velocity.X < 0 && ogPos[0].X < NPC.Center.X - 30)
                                legOffset = 0.6f;
                            else if (NPC.velocity.X > 0 && ogPos[1].X < NPC.Center.X - 30 && ogPos[0].X < NPC.Center.X - 30)
                                legOffset = 1.4f;
                            else if (NPC.velocity.X < 0 && ogPos[1].X > NPC.Center.X + 30 && ogPos[0].X > NPC.Center.X + 30)
                                legOffset = 1.4f;
                            if (Helper.FromAToB(ogPos[1], player.Center).X != 0)
                            {
                                if (AITimer2 < 40)
                                    ogPos[1] += new Vector2(NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[1], player.Center).X, -MathHelper.SmoothStep(5, 2, AITimer2 / 50));
                                else
                                {
                                    if (AITimer2 < 60)
                                        ogPos[1] += new Vector2((NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[1], player.Center).X) * MathHelper.SmoothStep(1, 0, (AITimer2 - 40) * 2 / 40), 0);

                                    ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.TRay.Cast(new Vector2(ogPos[1].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
                                }
                            }
                        }
                    }
                    else
                    {
                        AITimer2 = 0;
                        NPC.ai[3] = -NPC.ai[3];
                    }
                    if (AITimer2 % 80 == 0)
                        seed = Main.rand.Next(9999999);

                    if (verlet[0].lastP.position.Distance(NPC.Center) > 600)
                    {
                        ogPos[0] = Vector2.Lerp(ogPos[0], new Vector2(NPC.Center.X, Helper.TRay.Cast(ogPos[0], Vector2.UnitY, 800).Y + 16), 0.01f);
                    }
                    if (verlet[1].lastP.position.Distance(NPC.Center) > 600)
                    {
                        ogPos[1] = Vector2.Lerp(ogPos[1], new Vector2(NPC.Center.X, Helper.TRay.Cast(ogPos[1], Vector2.UnitY, 800).Y + 16), 0.01f);
                    }
                    if (AITimer >= 400)
                    {
                        AIState++;
                        AITimer = 0;
                        AITimer2 = 0;
                        if (player.Center.Distance(NPC.Center) < 500)
                            seed = Main.rand.Next(2);
                        else seed = 0;
                    }
                    break;
                case 1:
                    AITimer++;

                    if (verlet[0].lastP.position.Distance(NPC.Center) > 600)
                    {
                        ogPos[0] = Vector2.Lerp(ogPos[0], new Vector2(NPC.Center.X, Helper.TRay.Cast(ogPos[0], Vector2.UnitY, 800).Y + 16), 0.01f);
                    }
                    else
                        ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.TRay.Cast(ogPos[0] - new Vector2(0, 400), Vector2.UnitY, 1200).Y + 16, 0.1f);

                    NPC.velocity *= 0.9f;
                    if (seed == 0)
                    {
                        if (verlet[1].lastP.position.Distance(NPC.Center) > 600)
                        {
                            ogPos[1] = Vector2.Lerp(ogPos[1], new Vector2(NPC.Center.X, Helper.TRay.Cast(ogPos[1], Vector2.UnitY, 800).Y + 16), 0.01f);
                        }
                        else
                            ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.TRay.Cast(ogPos[1] - new Vector2(0, 400), Vector2.UnitY, 1200).Y + 16, 0.1f);

                        if (AITimer > 45)
                            AITimer2++;
                        else
                        {
                            Vector2 pos = NPC.Center + NPC.rotation.ToRotationVector2().RotatedBy(PiOver2).RotatedByRandom(PiOver4) * Main.rand.NextFloat(10, 100);
                            Dust.NewDustPerfect(pos, DustID.IchorTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20)).noGravity = true;
                        }
                        if (AITimer2 % 20 == 5)
                        {
                            SoundStyle sound = EbonianSounds.bloodSpit;
                            SoundEngine.PlaySound(sound, NPC.Center);
                        }
                        if (AITimer2 % 15 == 10)
                        {
                            Vector2 vel = NPC.Center.FromAToB(player.Center - new Vector2(0, Main.rand.NextFloat(100))).RotatedByRandom(0.3f);
                            for (int i = 0; i < 15; i++)
                            {
                                Dust.NewDustDirect(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.IchorTorch, vel.X * Main.rand.NextFloat(5, 8), vel.Y * Main.rand.NextFloat(5, 8));
                            }
                            Projectile a = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, vel * Main.rand.NextFloat(10, 15), ProjectileType<CIchor>(), 20, 0);
                            a.friendly = false;
                            a.hostile = true;
                            a.tileCollide = false;

                        }
                    }
                    else
                    {
                        if (AITimer < 20)
                        {
                            savedP = ogPos[1];
                            savedP2 = player.Center;
                            ogPos[1] = Vector2.Lerp(ogPos[1], player.Center - new Vector2(0, 200), 0.1f);
                        }
                        if (AITimer == 40)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), savedP + Helper.FromAToB(savedP, savedP2) * 110, Helper.FromAToB(savedP, savedP2), ProjectileType<CecitiorClawSlash>(), 30, 0);

                        if (AITimer > 40 && AITimer < 65)
                        {
                            ogPos[1] = Vector2.Lerp(ogPos[1], savedP2 + Helper.FromAToB(savedP, savedP2) * 200, 0.1f);
                        }
                        if (AITimer > 65)
                        {

                            ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.TRay.Cast(ogPos[1] - new Vector2(0, 300), Vector2.UnitY, 1000).Y + 16, 0.1f);
                        }

                        if (AITimer >= 90)
                        {
                            AIState = 0;
                            AITimer = 0;
                            AITimer2 = 0;
                            NPC.ai[3] = -NPC.ai[3];
                        }
                    }
                    if (AITimer >= 160)
                    {
                        AIState = 0;
                        AITimer = 0;
                        AITimer2 = 0;
                        NPC.ai[3] = -NPC.ai[3];
                    }
                    break;
            }
        }
        Vector2 savedP;
        Vector2 savedP2;
    }
}
