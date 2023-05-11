using EbonianMod.Tiles;
using IL.Terraria.GameContent;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace EbonianMod.Worldgen.Subworlds
{
    public class Ignos : Subworld
    {
        public override int Width => 500;
        public override int Height => 200;
        public override bool ShouldSave => false;
        //public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new IgnosPass()
        };
        public override void OnEnter()
        {
            SubworldSystem.hideUnderworld = false;
        }
        public override bool GetLight(Tile tile, int x, int y, ref FastRandom rand, ref Vector3 color)
        {
            color = Color.Orange.ToVector3();
            return true;
        }
        public override void OnLoad()
        {
            Main.dayTime = false;
            Main.time = 27000;
            Main.cloudAlpha = 0;
            Main.numClouds = 0;
            Main.rainTime = 0;
            Main.raining = false;
            Main.maxRaining = 0f;
            Main.slimeRain = false;

        }
    }
    public class IgnosPass : GenPass
    {
        public IgnosPass() : base("Ignos", 1f) { }
        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating Ignos"; // Sets the text displayed for this pass
            Main.worldSurface = -100; // Hides the underground layer just out of bounds
            Main.rockLayer = 0; // Hides the cavern layer way out of bounds
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                int height = 2;
                int rand = Main.rand.Next(15);
                if (rand == 0)
                {
                    if (height > 0)
                        height--;
                }
                else if (rand == 1)
                {
                    if (height < 2)
                        height++;
                }
                for (int j = 0; j < Main.maxTilesY; j++)
                {

                    progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                    Tile tile = Main.tile[i, j];
                    if (j > Main.maxTilesY - 55 + (int)(Math.Sin(i * 0.1f) * height) || j < 75 - (int)(Math.Sin(i * 0.1f) * height))
                        tile.HasTile = true;
                    tile.TileType = (ushort)ModContent.TileType<InfernalTile>();
                }
            }
        }
    }
    public class UpdateSubworldSystem : ModSystem
    {
        public override void PreUpdateWorld()
        {
            if (SubworldSystem.IsActive<Ignos>())
            {

                // Update mechanisms
                Wiring.UpdateMech();

                // Update tile entities
                TileEntity.UpdateStart();
                foreach (TileEntity te in TileEntity.ByID.Values)
                {
                    te.Update();
                }
                TileEntity.UpdateEnd();

                // Update liquid
                if (++Liquid.skipCount > 1)
                {
                    Liquid.UpdateLiquid();
                    Liquid.skipCount = 0;
                }
            }
        }
    }
}

