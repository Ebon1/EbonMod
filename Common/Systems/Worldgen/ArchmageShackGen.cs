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

            int x = Main.maxTilesX / 2 + 120;
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
            for (int i = Main.maxTilesX / 2 - 440; i < Main.maxTilesX / 2 - 140; i++)
            {
                int tempY = 110;
                while (!Main.tile[i, tempY].HasTile || (Main.tile[i, tempY].HasTile && !Main.tileSolid[Main.tile[i, tempY].TileType]) || Main.tile[i, tempY].TileType == TileID.Cloud || Main.tile[i, tempY].TileType == TileID.Plants || Main.tile[i, tempY].TileType == TileID.Cactus || Main.tile[i, tempY].TileType == TileID.Trees || Main.tile[i, tempY].TileType == TileID.Sunplate)
                    tempY++;
                tempHeightsL.Add(tempY);
            }
            for (int i = Main.maxTilesX / 2 + 140; i < Main.maxTilesX / 2 + 440; i++)
            {
                int tempY = 110;
                while (!Main.tile[i, tempY].HasTile || (Main.tile[i, tempY].HasTile && !Main.tileSolid[Main.tile[i, tempY].TileType]) || Main.tile[i, tempY].TileType == TileID.Cloud || Main.tile[i, tempY].TileType == TileID.Plants || Main.tile[i, tempY].TileType == TileID.Cactus || Main.tile[i, tempY].TileType == TileID.Trees || Main.tile[i, tempY].TileType == TileID.Sunplate)
                    tempY++;
                tempHeightsR.Add(tempY);
            }

            var leftValues = tempHeightsL.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count);
            var rightValues = tempHeightsR.GroupBy(x => x).Select(g => new { Value = g.Key, Count = g.Count() }).OrderByDescending(x => x.Count);

            int side = leftValues.Count() > rightValues.Count() ? 1 : -1;

            //int side = ((tempHeightsL.Max() - tempHeightsL.Min()) > (tempHeightsR.Max() - tempHeightsR.Min())) ? 1 : -1;
            int boundaries = 440 - 140;
            int x = Main.maxTilesX / 2 + 140 * side;
            int _y = 110;
            int atts = 0;
            bool failed = false;
            while (atts < boundaries)
            {
                int y = 110;
                List<int> _heights = new List<int>();
                for (int it = -3; it < 39; it++)
                {
                    while (!Main.tile[x + it, y].HasTile || (Main.tile[x + it, y].HasTile && !Main.tileSolid[Main.tile[x + it, y].TileType]) || Main.tile[x + it, y].TileType == TileID.Cloud || Main.tile[x + it, y].TileType == TileID.Plants || Main.tile[x + it, y].TileType == TileID.Cactus || Main.tile[x + it, y].TileType == TileID.Trees || Main.tile[x + it, y].TileType == TileID.Sunplate)
                        y++;
                    _heights.Add(y);
                }
                List<int> heights = new List<int>();
                for (int it = -3; it < 39; it++)
                {
                    int tempY = _heights.Min() - 30;
                    while (!Main.tile[x + it, tempY].HasTile || (Main.tile[x + it, tempY].HasTile && !Main.tileSolid[Main.tile[x + it, tempY].TileType]) || Main.tile[x + it, tempY].TileType == TileID.Cloud || Main.tile[x + it, tempY].TileType == TileID.Plants || Main.tile[x + it, tempY].TileType == TileID.Cactus || Main.tile[x + it, tempY].TileType == TileID.Trees || Main.tile[x + it, tempY].TileType == TileID.Sunplate)
                        tempY++;
                    heights.Add(tempY);
                }
                if ((heights.Max() - heights.Min()) > 5 || Main.tile[x, y].TileType == TileID.Sand || Main.tile[x, y].TileType == TileID.LivingWood)
                {
                    x += side;
                    _y = y + 1;
                    atts++;
                    if (atts >= boundaries)
                        failed = true;
                    continue;
                }
                else
                {
                    _y = heights.Min() + (int)MathF.Round((heights.Max() - heights.Min()) / 2) + 1;
                    atts = boundaries + 1;
                    break;
                }
            }
            if (failed)
            {
                atts = 0;
                _y = 110;
                x = Main.maxTilesX / 2 + 140 * -side;
                while (atts < boundaries)
                {
                    int y = 110;
                    List<int> _heights = new List<int>();
                    for (int it = -3; it < 39; it++)
                    {
                        while (!Main.tile[x + it, y].HasTile || (Main.tile[x + it, y].HasTile && !Main.tileSolid[Main.tile[x + it, y].TileType]) || Main.tile[x + it, y].TileType == TileID.Cloud || Main.tile[x + it, y].TileType == TileID.Plants || Main.tile[x + it, y].TileType == TileID.Cactus || Main.tile[x + it, y].TileType == TileID.Trees || Main.tile[x + it, y].TileType == TileID.Sunplate)
                            y++;
                        _heights.Add(y);
                    }
                    List<int> heights = new List<int>();
                    for (int it = -3; it < 39; it++)
                    {
                        int tempY = _heights.Min() - 30;
                        while (!Main.tile[x + it, tempY].HasTile || (Main.tile[x + it, tempY].HasTile && !Main.tileSolid[Main.tile[x + it, tempY].TileType]) || Main.tile[x + it, tempY].TileType == TileID.Cloud || Main.tile[x + it, tempY].TileType == TileID.Plants || Main.tile[x + it, tempY].TileType == TileID.Cactus || Main.tile[x + it, tempY].TileType == TileID.Trees || Main.tile[x + it, tempY].TileType == TileID.Sunplate)
                            tempY++;
                        heights.Add(tempY);
                    }
                    if ((heights.Max() - heights.Min()) > 5 || Main.tile[x, y].TileType == TileID.Sand || Main.tile[x, y].TileType == TileID.LivingWood)
                    {
                        x += -side;
                        _y = y + 1;
                        atts++;
                        continue;
                    }
                    else
                    {
                        _y = heights.Min() + (int)MathF.Round((heights.Max() - heights.Min()) / 2) + 1;
                        atts = boundaries + 1;
                        break;
                    }
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
