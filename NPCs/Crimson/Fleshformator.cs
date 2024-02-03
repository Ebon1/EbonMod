using Terraria.Audio;
using Terraria.ModLoader;
using EbonianMod.NPCs.Corruption;
using EbonianMod.Projectiles.Terrortoma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using EbonianMod.Misc;
using EbonianMod.Common.Systems;

namespace EbonianMod.NPCs.Crimson
{
    public class Fleshformator : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 58;
            NPC.height = 42;
            NPC.damage = 0;
            NPC.defense = 3;
            NPC.lifeMax = 800;
            NPC.HitSound = EbonianSounds.fleshHit;
            NPC.value = 60f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.buffImmune[BuffID.Ichor] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Type: Infected Creature"),
                new FlavorTextBestiaryInfoElement("Fleshformators are bottom-feeders of the deep crimson. When they are born, they roam from their starting spot and latch onto a nutrient-rich area to grab onto anything they can and devour it. None have ever seen what is on the other end of a Fleshformator."),
            });
        }
        Verlet[] verlet = new Verlet[3];
        Vector2[] endPos = new Vector2[3];
        Vector2[] ogEndPos = new Vector2[3];
        Vector2[] startPos = new Vector2[3];
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/Crimorrhage1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk9").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type);
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/Bone2").Type);


                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < verlet[i].segments.Count; j++)
                        Gore.NewGore(NPC.GetSource_Death(), verlet[i].segments[j].pointA.position, Main.rand.NextVector2Unit(), ModContent.Find<ModGore>("EbonianMod/CecitiorChainGore").Type);
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy && verlet[0] != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!Main.gamePaused)
                        verlet[i].Update(startPos[i], endPos[i]);
                    verlet[i].Draw(spriteBatch, "NPCs/Crimson/Fleshformator_Hook0", endTex: "NPCs/Crimson/Fleshformator_Hook1");
                }
            }
            return true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            int rand = Main.rand.Next(4);
            int attempts = 0;
            while (attempts < 25 && NPC.ai[3] == 0)
            {
                switch (rand)
                {
                    case 0:
                        if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitY, 1500) < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.Center = Helper.TRay.Cast(NPC.Center, Vector2.UnitY, 1500) - new Vector2(0, 14);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 1:
                        if (Helper.TRay.CastLength(NPC.Center, Vector2.UnitX, 1500) < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.rotation = MathHelper.ToRadians(-90);
                            NPC.Center = Helper.TRay.Cast(NPC.Center, Vector2.UnitX, 1500) - new Vector2(14, 0);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 2:
                        if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitX, 1500) < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.rotation = MathHelper.ToRadians(90);
                            NPC.Center = Helper.TRay.Cast(NPC.Center, -Vector2.UnitX, 1500) + new Vector2(14, 0);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 3:
                        if (Helper.TRay.CastLength(NPC.Center, -Vector2.UnitY, 1500) < 1490)
                        {
                            NPC.ai[3] = -0.5f;
                            NPC.rotation = MathHelper.ToRadians(180);
                            NPC.Center = Helper.TRay.Cast(NPC.Center, -Vector2.UnitY, 1500) + new Vector2(0, 14);
                        }
                        rand = Main.rand.Next(4);
                        break;
                }
                attempts++;
            }
            if (NPC.ai[3] == 0)
            {
                NPC.active = false;
                return;
            }
            for (int i = -1; i < 2; i++)
            {
                verlet[i + 1] = new Verlet(NPC.Center + new Vector2(21 * i, -5), 12, 13, NPC.ai[3], stiffness: 30);
                Vector2 pos = NPC.Center + new Vector2(40 * i, Main.rand.NextFloat(-85, -100)).RotatedBy(NPC.rotation);
                endPos[i + 1] = pos;
                ogEndPos[i + 1] = pos;
                startPos[i + 1] = NPC.Center + new Vector2(15 * i, 0).RotatedBy(NPC.rotation);
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
        bool currentControl;
        float alpha;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Vector2 offset = Vector2.Zero;
            Texture2D tex = Helper.GetExtraTexture("Sprites/arrow");
            if (player.whoAmI == Main.myPlayer)
            {
                if (currentControl) offset += Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 5);
                if (AITimer2 > 50)
                {
                    if (NPC.rotation == MathHelper.ToRadians(90))
                    {
                        spriteBatch.Draw(tex, player.Center + new Vector2(-80, 0) - screenPos + offset, null, Color.White * alpha, -MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(tex, player.Center + new Vector2(80, 0) - screenPos + offset, null, Color.White * alpha, MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                    }
                }
                else if (AITimer2 < 50 && AITimer2 > 0)
                {
                    if (NPC.rotation == MathHelper.ToRadians(90))
                    {
                        spriteBatch.Draw(tex, player.Center + new Vector2(80, 0) - screenPos + offset, null, Color.White * alpha, MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(tex, player.Center + new Vector2(-80, 0) - screenPos + offset, null, Color.White * alpha, -MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                    }
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (verlet[0] != null)
                for (int i = 0; i < 3; i++)
                {
                    if (verlet[i].lastP.position.Distance(player.Center) < 20 && AITimer2 <= -50)
                    {
                        AITimer = i;
                        alpha = 1;
                        player.GetModPlayer<EbonianPlayer>().fleshformators++;
                        AITimer2 = 100;
                    }

                    if (endPos[i].Distance(startPos[i]) < 139 && player.Center.Distance(startPos[i]) < 150 && AITimer2 <= 0)
                        endPos[i] += Helper.FromAToB(endPos[i], player.Center).RotatedByRandom(0.5f) * 0.75f;
                    else
                    {
                        if (AITimer2 > 0 && AITimer == i)
                        {
                            if (player.GetModPlayer<EbonianPlayer>().fleshformators == 1)
                                endPos[i] += Helper.FromAToB(endPos[i], NPC.Center - new Vector2(0, 31).RotatedBy(NPC.rotation), false).RotatedByRandom(0.5f) / 30;
                        }
                        else
                            endPos[i] += Helper.FromAToB(endPos[i], ogEndPos[i], false).RotatedByRandom(0.5f) / 30;

                    }
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == i) continue;
                        if (endPos[i].Distance(endPos[j]) < 20 && AITimer2 <= 0)
                            endPos[i] += Helper.FromAToB(endPos[i], ogEndPos[i], true);
                    }
                }
            if (AITimer2 > 50)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                    currentControl = player.controlLeft;
                else
                    currentControl = player.controlRight;
            }
            if (AITimer2 == 50)
                alpha = 1;
            if (AITimer2 < 50)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                    currentControl = player.controlRight;
                else
                    currentControl = player.controlLeft;
            }
            if (currentControl)
            {
                AITimer2--;
                alpha -= 0.02f;
            }
            if (AITimer2 == 1)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                    player.velocity = Vector2.UnitX * 15;
                else
                    player.velocity = -Vector2.UnitX * 15;
                player.GetModPlayer<EbonianPlayer>().fleshformators--;
                AITimer2--;
            }
            if (AITimer2 > 0)
            {
                player.controlUseItem = false;
                player.controlUseTile = false;
                player.controlThrow = false;
                player.gravDir = 1f;
                if (AITimer2 > 1)
                {
                    player.velocity.Y = -1;
                    player.velocity.X = 0;
                    player.Center += Helper.FromAToB(player.Center, verlet[(int)AITimer].lastP.position, false) / 2f;
                }
                if (player.GetModPlayer<EbonianPlayer>().fleshformators > 1)
                {
                    if (player.Center.Distance(endPos[(int)AITimer]) < 10)
                        endPos[(int)AITimer] += Helper.FromAToB(endPos[(int)AITimer], NPC.Center - new Vector2(0, 31).RotatedBy(NPC.rotation)) * 1.75f * player.GetModPlayer<EbonianPlayer>().fleshformators * Main.rand.NextFloat(0.7f, 3f);
                    endPos[(int)AITimer] += Helper.FromAToB(endPos[(int)AITimer], player.Center) * 0.75f * player.GetModPlayer<EbonianPlayer>().fleshformators;
                }

                player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 15, 0);
            }
            else
            {
                alpha = 1;
                AITimer2--;
                alpha = 0;
            }
        }
    }
}
