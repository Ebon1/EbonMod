using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Enemy.Corruption;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace EbonianMod.NPCs.Corruption
{
    public class VileTear : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            NPC.width = 136;
            NPC.height = 136;
            NPC.damage = 10;
            NPC.defense = 1;
            NPC.lifeMax = 1000;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.DD2_DrakinHurt;
            NPC.DeathSound = SoundID.DD2_DrakinDeath;
            NPC.value = Item.buyPrice(0, 1);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.noTileCollide = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D tex = Helper.GetTexture("NPCs/Corruption/VileTear");
            Texture2D tex2 = Helper.GetTexture("NPCs/Corruption/VileTear_Glow");
            var fadeMult = 1f / NPC.oldPos.Length;
            if (AIState == 1)
                for (int i = 0; i < NPC.oldPos.Length; i++)
                {
                    Main.EntitySpriteDraw(tex, NPC.oldPos[i] + NPC.Size / 2 - screenPos, NPC.frame, drawColor * NPC.ai[3] * (1f - fadeMult * i), NPC.oldRot[i], NPC.Size / 2, NPC.scale, effects, 0);
                }
            Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
            return false;
        }
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 4; i++)
                        Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVector2FromRectangle(NPC.getRect()), Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
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
        public override void FindFrame(int frameHeight)
        {
            //NPC.frame.Y = frameHeight * (AIState == 1 && AITimer > 90 && AITimer < 150 ? 0 : 1);
        }
        Vector2 p;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            //NPC.spriteDirection = NPC.direction = NPC.velocity.X > 0 ? -1 : 1;
            NPC.spriteDirection = NPC.direction = -1;
            switch (AIState)
            {
                case 0:
                    AITimer++;
                    NPC.damage = 10;
                    if (AITimer < 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
                    if (AITimer > 200)
                        NPC.velocity *= 0.9f;
                    if (AITimer > 1)
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() + MathHelper.Pi, 0.25f);
                    if (AITimer >= 230)
                    {
                        AITimer2 = Main.rand.Next(4);
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer < 70)
                    {
                        p = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 1000;
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -200).RotatedBy(MathF.Sin(AITimer + Main.GlobalTimeWrappedHourly * 3) * 1.5f), true) * 20, 0.025f);
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                    }
                    if (AITimer > 50 && AITimer < 70)
                    {
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], 1, 0.1f);
                    }

                    if (AITimer == 30)
                    {
                        NPC.damage = 40;
                        NPC.velocity *= 0.5f;
                        //Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center), ModContent.ProjectileType<VileTearTelegraph>(), 0, 0);
                        SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(-0.4f), NPC.Center);
                    }
                    if (AITimer == 70)
                    {
                        NPC.velocity = Vector2.Zero;
                        SoundEngine.PlaySound(EbonianSounds.terrortomaDash.WithPitchOffset(-0.25f), NPC.Center);
                    }

                    if (AITimer >= 70 && AITimer < 92)
                    {
                        NPC.velocity += Helper.FromAToB(NPC.Center, p) * 2;
                    }
                    if (AITimer > 100)
                    {
                        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], 0, 0.1f);
                        NPC.damage = 10;
                        NPC.velocity *= 0.9f;
                    }

                    if (AITimer > 120)
                    {
                        NPC.velocity = Vector2.Zero;
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 2:
                    AITimer++;
                    NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + MathHelper.Pi, 0.25f);
                    if (AITimer > 30)
                    {
                        AITimer2++;
                        if (AITimer2 < 4)
                        {
                            AIState = 1;
                        }
                        else
                        {
                            AIState = 0;
                        }
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
