using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.NPCs.Overworld.Critters
{
    public class Sheep : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Bunny);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = EbonianSounds.sheep;
            NPC.width = 38;
            NPC.height = 28;
            NPC.Size = new Vector2(38, 28);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneForest ? 0.3f : 0;
        }
        public override void OnKill()
        {
            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/Wool").Type, NPC.scale);

            for (int i = 0; i < 50; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection());
        }
        public override void AI()
        {
            if (Main.rand.NextBool(2000))
                SoundEngine.PlaySound(EbonianSounds.sheep.WithVolumeScale(0.5f), NPC.Center);
            NPC.spriteDirection = -NPC.direction;
            Collision.StepDown(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.sheep, NPC.Center);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (!NPC.velocity.Y.CloseTo(0, 0.2f))
            {
                NPC.frame.Y = frameHeight * 4;
            }
            else
            {
                if (NPC.velocity.X.CloseTo(0, 0.05f))
                {
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frameCounter % 5 == 0)
                    {
                        if (NPC.frame.Y < frameHeight * 3)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                }
            }
        }
    }
}
