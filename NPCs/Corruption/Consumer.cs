using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.NPCs.Corruption
{
    public class Consumer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
        }
        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 48;
            NPC.damage = 20;
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
            NPC.behindTiles = true;
            NPC.hide = true;
        }
        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsMoonMoon.Add(index);
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.frameCounter++ % 5 == 0)
            {
                if ((NPC.frame.Y += frameHeight) > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
            }
        }
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
                    NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
                    if (AITimer >= 230)
                    {
                        AITimer2 = Main.rand.NextBool() ? Main.rand.Next(3) : 0;
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 1:
                    AITimer++;
                    if (AITimer < 10)
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.25f);
                    else
                        NPC.rotation = NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0);
                    if (AITimer >= 10)
                        NPC.velocity += Helper.FromAToB(NPC.Center, player.Center) * 3;
                    if (AITimer > 15)
                    {
                        SoundEngine.PlaySound(EbonianSounds.chomp2.WithPitchOffset(0.25f), NPC.Center);
                        AITimer = 0;
                        AIState++;
                    }
                    break;
                case 2:
                    NPC.velocity *= 0.9f;
                    AITimer++;
                    if (AITimer > 15)
                    {
                        AITimer2++;
                        if (AITimer2 < 3)
                            AIState = 1;
                        else
                            AIState = 0;
                        AITimer = 0;
                    }
                    break;
            }
        }
    }
}
