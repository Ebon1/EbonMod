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
            NPC.DeathSound = SoundID.NPCDeath1;
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
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/WoolGore").Type, NPC.scale);

            for (int i = 0; i < 50; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection());

        }
        int dyeId = -1, lastClicked = 0;
        public override void AI()
        {
            if (Main.rand.NextBool(2000))
                SoundEngine.PlaySound(EbonianSounds.sheep.WithVolumeScale(0.5f), NPC.Center);
            if (new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5).Intersects(NPC.getRect()) && Main.mouseRight && --lastClicked < 0 && Main.LocalPlayer.HeldItem.dye > 0 && dyeId != Main.LocalPlayer.HeldItem.type)
            {
                dyeId = Main.LocalPlayer.HeldItem.type;
                SoundEngine.PlaySound(SoundID.Item176, NPC.Center);
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke);
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.ShimmerSpark);
                }
            }
            NPC.spriteDirection = -NPC.direction;
            Collision.StepDown(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        public override void PostAI()
        {
            if (new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5).Intersects(NPC.getRect()) && Main.mouseRight)
                lastClicked = 30;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = Helper.GetTexture(Texture + "_Wool");
            if (dyeId > 0)
            {
                DrawData data = new(tex, NPC.Center + new Vector2(0, NPC.gfxOffY + 2) - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                MiscDrawingMethods.DrawWithDye(spriteBatch, data, dyeId, NPC);
            }
            if (new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5).Intersects(NPC.getRect()) && --lastClicked < 0 && Main.LocalPlayer.HeldItem.dye > 0 && dyeId != Main.LocalPlayer.HeldItem.type)
            {
                EbonianMod.finalDrawCache.Add(() => { spriteBatch.Draw(TextureAssets.Item[Main.LocalPlayer.HeldItem.type].Value, Main.MouseWorld + new Vector2(20, -20), null, Color.White, 0, TextureAssets.Item[Main.LocalPlayer.HeldItem.type].Value.Size() / 2, 1, SpriteEffects.None, 0)});
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.sheep, NPC.Center);

            if (Main.rand.NextBool(12))
            {
                WeightedRandom<int> dye = new();
                dye.Add(ItemID.PinkDye, 0.01f);
                dye.Add(ItemID.BlackDye);
                dye.Add(ItemID.BlueDye, 0.2f);
                dye.Add(ItemID.BrownDye);
                dye.Add(ItemID.YellowDye, 0.3f);
                dye.Add(ItemID.NegativeDye, 0.0001f);
                dye.Add(ItemID.RedDye, 0.3f);
                dye.Add(ItemID.BrightSilverDye);
                dye.Add(ItemID.ReflectiveGoldDye, 0.1f);
                dyeId = dye;
            }
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
