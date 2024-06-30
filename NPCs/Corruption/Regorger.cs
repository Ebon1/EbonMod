using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Projectiles.Enemy.Corruption;

namespace EbonianMod.NPCs.Corruption
{
    internal class Regorger : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;
        }
        public override void SetDefaults()
        {
            NPC.width = 76;
            NPC.height = 50;
            NPC.damage = 0;
            NPC.defense = 1;
            NPC.lifeMax = 60;
            NPC.aiStyle = -1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0.5f;
            NPC.noGravity = true;
            NPC.buffImmune[BuffID.CursedInferno] = true;
            NPC.noTileCollide = true;
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 5 == 0)
            {
                if (AIState != 2)
                {
                    if ((NPC.frame.Y += frameHeight) > 3 * frameHeight)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    if (NPC.frame.Y < 4 * frameHeight)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    if ((NPC.frame.Y += frameHeight) > 6 * frameHeight)
                    {
                        NPC.frame.Y = 4 * frameHeight;
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
        public override void HitEffect(NPC.HitInfo hitinfo)
        {
            if (hitinfo.Damage > NPC.life)
            {
                for (int i = 0; i < 4; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
            }
        }
        Vector2 p;
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(false);
            NPC.spriteDirection = NPC.direction = -1;
            switch (AIState)
            {
                case 0:
                    AITimer++;
                    if (AITimer < 200)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
                    if (AITimer > 200)
                        NPC.velocity *= 0.9f;
                    if (AITimer > 1)
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.25f);
                    if (AITimer >= 230)
                    {
                        AITimer2 = Main.rand.Next(4);
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer < 50)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -200).RotatedBy(MathF.Sin(AITimer + Main.GlobalTimeWrappedHourly * 3) * 1.5f), false) / 40, 0.02f);
                        NPC.rotation = Helper.LerpAngle(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.25f);
                    }

                    if (AITimer == 50)
                    {
                        NPC.velocity = Vector2.Zero;
                        p = player.Center;
                        Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center), ModContent.ProjectileType<RegorgerTelegraph>(), 0, 0);
                        SoundEngine.PlaySound(EbonianSounds.bloodSpit.WithPitchOffset(0.25f), NPC.Center);
                    }

                    if (AITimer == 90)
                    {
                        NPC.velocity = -Helper.FromAToB(NPC.Center, p) * 4;
                        for (int i = 0; i < 15; i++)
                            Dust.NewDustPerfect(NPC.Center, DustID.CursedTorch, Helper.FromAToB(NPC.Center, p).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(3, 6));
                        Projectile.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, p), ModContent.ProjectileType<RegorgerBolt>(), 10, 0);
                    }
                    if (AITimer > 90)
                        NPC.velocity *= 0.9f;

                    if (AITimer > 120)
                    {
                        AITimer = 0;
                        AIState++;
                        NPC.frame.Y = 4 * 52;
                    }
                    break;
                case 2:
                    AITimer++;
                    if (AITimer == 1)
                    {
                        SoundEngine.PlaySound(EbonianSounds.terrortomaLaugh.WithPitchOffset(1f), NPC.Center);
                    }
                    if (AITimer > 60)
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
