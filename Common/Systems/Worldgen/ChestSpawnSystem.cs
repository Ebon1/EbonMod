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
            int[] goldChestMainLoot = { ItemType<GarbageRemote>(), ItemType<SpudCannon>() };

            int[] goldChestSecondaryLoot = { ItemType<WaspPaintingI>(), ItemType<DjungelskogI>(), ItemType<Potato>() };

            int[] shadowChestMainLoot = { ItemType<Corebreaker>(), ItemType<RingOfFire>(), ItemType<Exolsaw>() };

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
                            if (chest.item[1].type == ItemType<Potato>())
                                chest.item[1].stack = Main.rand.Next(2, 20);
                        }
                        if (WorldGen.genRand.NextBool(5)) //primary
                        {
                            int type = Main.rand.Next(goldChestMainLoot);
                            chest.item[0].SetDefaults(type);
                            if (chest.item[0].type == ItemType<SpudCannon>())
                            {
                                chest.item[1].SetDefaults(ItemType<Potato>());
                                chest.item[1].stack = Main.rand.Next(50, 200);
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
