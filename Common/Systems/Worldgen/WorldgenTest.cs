using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using StructureHelper;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.GameContent.Generation;
using EbonianMod.Tiles;
using Microsoft.Xna.Framework;

namespace EbonianMod.Common.Systems.Worldgen
{
    public class WorldgenTest : ModSystem
    {
        public override void PostWorldGen()
        {
        }
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Generating Hive thing", GenMeteors));
            }
        }
        public void GenMeteors(GenerationProgress progress, GameConfiguration _)
        {
            if (!WorldGen.crimson)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0)
                        continue;
                    int x = Main.maxTilesX / 2 + Main.maxTilesX / 5 * i;
                    int y = 0;
                    for (int it = 0; it < 13; it++)
                    {
                        while (!Main.tile[x + it, y].HasTile || Main.tile[x + it, y].TileType == TileID.Cloud || Main.tile[x + it, y].TileType == TileID.Sunplate)
                            y++;
                    }
                    Point16 pos = new(x, y - 8);
                    Logging.PublicLogger.Debug(pos);
                    Generator.GenerateStructure("Common/Systems/Worldgen/Structures/CorruptionMeteorNoNulls", pos, EbonianMod.Instance);
                    for (int ite = x; ite < x + 13; ite++)
                    {
                        for (int iter = 0; iter < 12; iter++)
                        {
                            if (Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveBlock>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveBlockSpecial>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveRock>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveRock2>())
                                Main.tile[ite, y - iter].ClearTile();
                            if (!Main.tile[ite, y + iter].HasTile)

                                WorldGen.PlaceTile(ite, y + iter, ModContent.TileType<EbonHiveBlock>());
                        }
                    }
                }
            }
            else
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0)
                        continue;
                    int x = Main.maxTilesX / 2 + Main.maxTilesX / 5 * i;
                    int y = 0;
                    for (int it = 0; it < 13; it++)
                    {
                        while (!Main.tile[x + it, y].HasTile || Main.tile[x + it, y].TileType == TileID.Cloud || Main.tile[x + it, y].TileType == TileID.Sunplate)
                            y++;
                    }
                    Point16 pos = new(x, y - 8);
                    Logging.PublicLogger.Debug(pos);
                    Generator.GenerateStructure("Common/Systems/Worldgen/Structures/CrimsonMeteor", pos, EbonianMod.Instance);
                    for (int ite = x; ite < x + 13; ite++)
                    {
                        for (int iter = 0; iter < 12; iter++)
                        {
                            if (Main.tile[ite, y - iter].TileType != ModContent.TileType<CrimsonBrainBlock>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<CrimsonBrainBlockSpecial>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveRock>() && Main.tile[ite, y - iter].TileType != ModContent.TileType<EbonHiveRock2>())
                                Main.tile[ite, y - iter].ClearTile();
                            if (!Main.tile[ite, y + iter].HasTile)

                                WorldGen.PlaceTile(ite, y + iter, ModContent.TileType<CrimsonBrainBlock>());
                        }
                    }
                }
            }
        }
    }
}
