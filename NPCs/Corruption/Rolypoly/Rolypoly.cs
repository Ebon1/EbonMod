using EbonianMod.Common.Systems.Misc;
using EbonianMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption.Rolypoly
{
    public class Rolypoly : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(25, 25);
            NPC.lifeMax = 300;
            NPC.defense = 5;
            NPC.knockBackResist = 0.1f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            NPC.damage = 30;
            NPC.behindTiles = true;
        }
        Verlet verlet;
        Verlet[] extraVerlets = new Verlet[7];
        int texNum;
        float rot, rotFactor;
        int amount;
        float lerpFactor = 1f;
        public override void OnSpawn(IEntitySource source)
        {
            NPC.scale = Main.rand.Next(new float[] { 0.75f, 0.75f, 0.75f, 0.75f, 0.55f, 0.55f });
            if (Main.rand.NextBool(50) && NPC.scale == 1f)
                NPC.scale = 1.25f;
            amount = 10;
            switch (NPC.scale)
            {
                case 1.25f:
                    amount = 16;
                    break;
                case 0.75f:
                    amount = 14;
                    break;
                case 0.55f:
                    amount = 12;
                    break;
                default:
                    amount = 10;
                    break;
            }
            texNum = Main.rand.Next(9999999);
            NPC.Size = new Vector2(100, 100) * NPC.scale;
            verlet = new Verlet(NPC.Center, 16, amount, 0, false, false, 4, true, 8);

            for (int i = 0; i < 7; i++)
                extraVerlets[i] = new Verlet(NPC.Center, 16, amount - 3, 3f, true, true, 20, true, 8);
        }
        public override bool CheckDead()
        {
            if (verlet != null)
            {
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    if (i % 5 == 0)
                        Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Circular(4, 4), ModContent.Find<ModGore>("EbonianMod/Rolypoly" + Main.rand.Next(3)).Type);
                }
                Verlet v = verlet;
                v.gravity = 0;
                S_VerletSystem.verlets.Add(new SpawnableVerlet(v, new VerletDrawData(Texture + "_Tex", _textureVariation: true, _maxVariants: 3, _variantSeed: texNum), new Vector2(Main.rand.NextFloat(1, 7) * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), Main.rand.NextFloat(-20, 5)), 800));
            }
            for (int i = 0; i < (NPC.scale > 0.55f ? 7 : 2); i++)
                if (extraVerlets[i] != null)
                {
                    for (int j = 0; j < extraVerlets[i].points.Count; j++)
                    {
                        if (j % 5 == 0)
                            Gore.NewGore(NPC.GetSource_Death(), extraVerlets[i].points[j].position, Main.rand.NextVector2Circular(4, 4), ModContent.Find<ModGore>("EbonianMod/Rolypoly" + Main.rand.Next(3)).Type);
                    }
                    Verlet v = extraVerlets[i];
                    v.gravity = 0;
                    S_VerletSystem.verlets.Add(new SpawnableVerlet(v, new VerletDrawData(Texture + "_Tex", _textureVariation: true, _maxVariants: 3, _variantSeed: texNum), new Vector2(Main.rand.NextFloat(1, 7) * (Main.rand.NextFloatDirection() > 0 ? 1 : -1), Main.rand.NextFloat(-20, 5)), 800));
                }
            return true;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.velocity.Length() >= 1;
        }
        public override void AI()
        {
            //lerpFactor = MathHelper.Lerp(lerpFactor, 0.25f, 0.1f);
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            if (NPC.collideY)
            {
                if (NPC.ai[3]++ % 40 <= 20)
                {
                    NPC.FaceTarget();
                }

                Dust.NewDust(NPC.BottomLeft, NPC.width, 2, DustID.CorruptGibs, NPC.velocity.X, NPC.velocity.Y);
            }
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 2.5f * NPC.direction / (NPC.scale.Safe() + 0.3f), 0.05f);
            if (NPC.collideX)
            {
                if (NPC.velocity.Y > -5)
                    NPC.velocity.Y -= .4f;
            }
            else
            {
                if (NPC.velocity.Y > 10)
                    NPC.velocity.Y++;
            }

            rot = MathHelper.Lerp(rot, MathHelper.ToRadians(rotFactor), 1f);

            rotFactor += NPC.velocity.X * 0.25f;

            if (++NPC.ai[2] >= Main.rand.Next(300, 600) && player.Distance(NPC.Center) < 1800 && player.Distance(NPC.Center) > 200)
            {
                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center - new Vector2(0, 200 * NPC.scale)) * 5 / NPC.scale.Safe();
                NPC.velocity += new Vector2(vel.X * 0.7f, vel.Y * 2.2f);
                NPC.ai[2] = Main.rand.Next(-200, 20);
            }
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (verlet != null)
            {
                for (int i = 0; i < verlet.points.Count; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, amount - 1);
                    //NPC.Center + new Vector2(0, 75 * NPC.scale).RotatedBy(angle + rot)
                    float f = MathHelper.SmoothStep(0.5f, 1f, ((angle + rot).ToRotationVector2().Y + 1) * 0.5f);
                    Vector2 pos = Helper.TRay.Cast(NPC.Center, (angle + rot).ToRotationVector2(), 128 * NPC.scale * f, false);
                    verlet.points[i].position = Vector2.Lerp(verlet.points[i].position, pos, 0.2f);
                }
                for (int i = 0; i < (NPC.scale > 0.55f ? 5 : 2); i++)
                {
                    if (extraVerlets[i] != null)
                    {
                        int p1 = 0;
                        int p2 = amount - 1;
                        switch (i)
                        {
                            case 0:
                                p1 = 1;
                                p2 = amount - 1;
                                break;
                            case 1:
                                p1 = 2;
                                p2 = amount - 2;
                                break;
                            case 2:
                                p1 = 3;
                                p2 = 1;
                                break;
                            case 3:
                                p1 = 1;
                                p2 = amount - 2;
                                break;
                            case 4:
                                p1 = amount - 1;
                                p2 = amount - 5;
                                break;
                        }

                        extraVerlets[i].Update(verlet.points[p1].position, verlet.points[p2].position);
                        extraVerlets[i].Draw(spriteBatch, Texture + "_Tex", textureVariation: true, maxVariants: 3, variantSeed: texNum);
                    }
                }
                verlet.Draw(spriteBatch, Texture + "_Tex", textureVariation: true, maxVariants: 3, variantSeed: texNum);
                if (NPC.scale > 0.55f)
                    for (int i = 5; i < 7; i++)
                    {
                        if (extraVerlets[i] != null)
                        {
                            int p1 = 0;
                            int p2 = amount - 1;
                            switch (i)
                            {
                                case 5:
                                    p1 = 4;
                                    p2 = amount - 1;
                                    break;
                                case 6:
                                    p1 = 0;
                                    p2 = 5;
                                    break;
                            }

                            extraVerlets[i].Update(verlet.points[p1].position, verlet.points[p2].position);
                            extraVerlets[i].Draw(spriteBatch, Texture + "_Tex", textureVariation: true, maxVariants: 3, variantSeed: texNum);
                        }
                    }
            }
            return false;
        }
    }
}
