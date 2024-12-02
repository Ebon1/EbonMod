using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.Items.Tiles;
using EbonianMod.Items.Consumables.BossItems;
using EbonianMod.Items.Pets;
using EbonianMod.Items.Consumables.Food;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.Items.Weapons.Melee;

namespace EbonianMod.Common.Systems.Worldgen
{
    public class ChestSpawnSystem : ModSystem
    {
        void FillChests()
        {
            int[] goldChestMainLoot = { ModContent.ItemType<GarbageRemote>(), ModContent.ItemType<SpudCannon>() };

            int[] goldChestSecondaryLoot = { ModContent.ItemType<WaspPaintingI>(), ModContent.ItemType<DjungelskogI>(), ModContent.ItemType<Potato>() };

            int[] shadowChestMainLoot = { ModContent.ItemType<Corebreaker>(), ModContent.ItemType<RingOfFire>(), ModContent.ItemType<Exolsaw>() };

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    if (Main.tile[chest.x, chest.y].TileFrameX == 1 * 36)
                    {
                        if (WorldGen.genRand.NextBool(5)) //secondary
                        {
                            int type = Main.rand.Next(goldChestSecondaryLoot);
                            chest.item[1].SetDefaults(type);
                            if (chest.item[1].type == ModContent.ItemType<Potato>())
                                chest.item[1].stack = Main.rand.Next(2, 20);
                        }
                        if (WorldGen.genRand.NextBool(5)) //primary
                        {
                            int type = Main.rand.Next(goldChestMainLoot);
                            chest.item[0].SetDefaults(type);
                            if (chest.item[0].type == ModContent.ItemType<SpudCannon>())
                            {
                                chest.item[1].SetDefaults(ModContent.ItemType<Potato>());
                                chest.item[1].stack = Main.rand.Next(2, 20);
                            }

                        }
                    }
                    if (Main.tile[chest.x, chest.y].TileFrameX == 3 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 4 * 36)
                    {
                        if (WorldGen.genRand.NextBool(3))
                            chest.item[0].SetDefaults(Main.rand.Next(shadowChestMainLoot));
                    }
                }
            }
        }
        public override void PostWorldGen()
        {
            FillChests();
        }
    }
}
