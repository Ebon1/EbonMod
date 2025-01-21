using EbonianMod.Items.Materials;
using EbonianMod.Items.Misc;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Biomes.CaveHouse;
using Terraria.GameContent.ItemDropRules;

namespace EbonianMod.NPCs.Overworld.Critters
{
    public class Sheep : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.CountsAsCritter[Type] = true;
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;
            NPCID.Sets.TownCritter[Type] = true;
            Main.npcCatchable[Type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Type: Farm Animal"),
                new FlavorTextBestiaryInfoElement("Sheep are passive, friendly animals with a thick layer of wool and an appetite for grass, sheep's wool is quite common in many colorful items of clothing and especially comfort, to the surprise of very few"),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemType<Wool>(), 4, 1, 3));
        }
        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Bunny);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.width = 38;
            NPC.height = 28;
            NPC.catchItem = ItemType<SheepItem>();
            NPC.Size = new Vector2(38, 28);
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZoneForest ? 0.3f : 0;
        }
        public override void OnKill()
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/SheepGore0").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/SheepGore1").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/SheepGore2").Type, NPC.scale);

            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/SheepGore3").Type, NPC.scale);

            for (int i = 0; i < 50; i++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection());

        }
        int dyeId = -1, lastClicked = 0;
        public override void AI()
        {
            lastClicked--;
            if (Main.rand.NextBool(2000))
                SoundEngine.PlaySound(EbonianSounds.sheep.WithVolumeScale(0.5f), NPC.Center);
            if (Main.LocalPlayer.Center.Distance(NPC.Center) < 175 && new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5).Intersects(NPC.getRect()) && Main.mouseRight && lastClicked < 0 && Main.LocalPlayer.HeldItem.dye > 0 && dyeId != Main.LocalPlayer.HeldItem.type)
            {
                dyeId = Main.LocalPlayer.HeldItem.type;
                SoundEngine.PlaySound(SoundID.Item176, NPC.Center);
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke);
                    Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.ShimmerSpark);
                }
            }
            string name = Main.LocalPlayer.name;
            name.ApplyCase(LetterCasing.LowerCase);
            if (name == "dinnerbone" || name == "grumm")
                NPC.directionY = -1;
            NPC.spriteDirection = -NPC.direction;
            Collision.StepDown(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
        }
        public override void PostAI()
        {
            if (Main.LocalPlayer.Center.Distance(NPC.Center) < 175 && new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5).Intersects(NPC.getRect()) && Main.mouseRight && lastClicked < 0)
                lastClicked = 30;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return true;
            Texture2D tex = Helper.GetTexture(Texture);

            string name = Main.LocalPlayer.name;
            name.ApplyCase(LetterCasing.LowerCase);
            spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.gfxOffY + 2) - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, (NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) | (name == "dinnerbone" || name == "grumm" ? SpriteEffects.FlipVertically : SpriteEffects.None), 0);

            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy) return;
            Texture2D tex = Helper.GetTexture(Texture + "_Wool");
            if (dyeId > 0)
            {
                string name = Main.LocalPlayer.name;
                name.ApplyCase(LetterCasing.LowerCase);
                DrawData data = new(tex, NPC.Center + new Vector2(0, NPC.gfxOffY + 2) - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, (NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally) | (name == "dinnerbone" || name == "grumm" ? SpriteEffects.FlipVertically : SpriteEffects.None));
                MiscDrawingMethods.DrawWithDye(spriteBatch, data, dyeId, NPC);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.rand.NextBool(4))
                SoundEngine.PlaySound(EbonianSounds.sheep, NPC.Center);

            WeightedRandom<int> dye = new();
            dye.Add(ItemID.PinkDye, 0.01f);
            dye.Add(ItemID.NegativeDye, 0.001f);
            dye.Add(ItemID.BlackDye);
            dye.Add(ItemID.BlueDye, 0.1f);
            dye.Add(ItemID.BrownDye);
            dye.Add(ItemID.YellowDye, 0.3f);
            dye.Add(ItemID.BrightSilverDye);
            dye.Add(ItemID.GreenDye, 0.2f);
            dye.Add(ItemID.ShadowDye);
            dye.Add(ItemID.BrightBrownDye);
            dye.Add(ItemID.BrownAndBlackDye);
            dye.Add(ItemID.BrownAndSilverDye);
            dye.Add(ItemID.SkyBlueDye, 0.5f);
            dye.Add(ItemID.OrangeandSilverDye);
            dye.Add(ItemID.ReflectiveGoldDye, 0.025f);
            dye.Add(-1, 8);
            dyeId = dye;

            string name = Main.LocalPlayer.name;
            name.ApplyCase(LetterCasing.LowerCase);
            if (name == "jeb" || name == "jeb_")
                dyeId = ItemID.LivingRainbowDye;

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
