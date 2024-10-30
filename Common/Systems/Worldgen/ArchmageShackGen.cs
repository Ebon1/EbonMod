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
using EbonianMod.Dusts;

namespace EbonianMod.Common.Systems.Worldgen
{
    public class ArchmageShackGen : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Pots"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Generating the Archmage's shack", GenHouse));
            }
            ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Traps"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Generating the Archmage's dungeon", GenHouse2));
            }
            ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("Cleaning up the Archmage's shack", GenHouse3));
            }
        }
        Point16 arenaPos;
        public void GenHouse3(GenerationProgress progress, GameConfiguration _)
        {

            int x = Main.maxTilesX / 2 + 140;
            int _y = 0;
            for (int i = x; i < x + 400; i++)
            {
                for (int j = _y; j < Main.maxTilesY / 2; j++)
                {
                    if (Main.tile[i, j].TileType == TileID.GemLocks)
                    {
                        x = i;
                        _y = j;
                        break;
                    }
                }
                if (_y != 0)
                    break;
            }
            for (int i = x; i < x + 3; i++)
            {
                for (int j = _y; j < _y + 11; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (j > _y + 5)
                    {
                        tile.HasActuator = true;
                    }
                    tile.RedWire = true;
                }
            }
        }
        public void GenHouse2(GenerationProgress progress, GameConfiguration _)
        {
            Generator.GenerateStructure("Common/Systems/Worldgen/Structures/ArchmageArena", arenaPos, EbonianMod.Instance);
            for (int i = arenaPos.X; i < arenaPos.X + 50; i++)
            {
                for (int j = arenaPos.Y; j < arenaPos.Y + 100; j++)
                {
                    if (Main.tile[i, j].TileType == TileID.SillyBalloonGreen)
                    {
                        Tile tile = Main.tile[i, j];
                        tile.TileType = (ushort)ModContent.TileType<ArchmageStaffTile>();
                    }
                }
            }
        }
        public void GenHouse(GenerationProgress progress, GameConfiguration _)
        {
            List<int> tempHeightsL = new List<int>();
            List<int> tempHeightsR = new List<int>();
            for (int i = Main.maxTilesX / 2 - 200; i < Main.maxTilesX / 2 + 200; i++)
            {
                int tempY = 0;
                while (!Main.tile[i, tempY].HasTile)
                    tempY++;
                if (i < Main.maxTilesX / 2)
                    tempHeightsL.Add(tempY);
                else
                    tempHeightsR.Add(tempY);
            }
            int side = (tempHeightsL.Max() - tempHeightsL.Min() > tempHeightsR.Max() - tempHeightsR.Min()) ? 1 : -1;
            int sizeDiff = side == 1 ? tempHeightsR.Max() - tempHeightsR.Min() : tempHeightsL.Max() - tempHeightsL.Min();
            if (sizeDiff > 6)
            {
                tempHeightsL.Clear();
                tempHeightsR.Clear();
                for (int i = Main.maxTilesX / 2 - 340; i < Main.maxTilesX / 2 + 340; i++)
                {
                    int tempY = 0;
                    while (!Main.tile[i, tempY].HasTile)
                        tempY++;
                    if (i < Main.maxTilesX / 2)
                        tempHeightsL.Add(tempY);
                    else
                        tempHeightsR.Add(tempY);
                }
                side = (tempHeightsL.Max() - tempHeightsL.Min() > tempHeightsR.Max() - tempHeightsR.Min()) ? 1 : -1;
            }
            int x = Main.maxTilesX / 2 + 140 * side;
            int _y = 0;
            int atts = 0;
            while (atts < 200)
            {
                int y = 0;
                for (int it = -1; it < 37; it++)
                {
                    while (!Main.tile[x + it, y].HasTile || Main.tile[x + it, y].TileType == TileID.Cloud || Main.tile[x + it, y].TileType == TileID.Sunplate)
                        y++;
                }
                List<int> heights = new List<int>();
                for (int it = -2; it < 39; it++)
                {
                    int tempY = 0;
                    while (!Main.tile[x + it, tempY].HasTile)
                        tempY++;
                    heights.Add(tempY);
                }
                if (heights.Max() - heights.Min() > 4)
                {
                    x += side;
                    _y = y + 1;
                    atts++;
                    continue;
                }
                else
                {
                    _y = y + 1;
                    atts = 400;
                }
            }
            Point16 pos = new(x, _y - 31);
            Generator.GenerateStructure("Common/Systems/Worldgen/Structures/ArchmageShack", pos, EbonianMod.Instance);
            pos = new(x + 25, _y + 3);
            Generator.GenerateStructure("Common/Systems/Worldgen/Structures/ArchmageStair", pos, EbonianMod.Instance);
            pos = new(x + 25, _y + 23);
            Generator.GenerateStructure("Common/Systems/Worldgen/Structures/ArchmageStair", pos, EbonianMod.Instance);
            arenaPos = new(x - 38, _y + 43);
        }
    }
}
