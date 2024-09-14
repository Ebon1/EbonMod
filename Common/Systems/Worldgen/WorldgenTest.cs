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
                //tasks.Add(new PassLegacy("Generating Hive thing", GenMeteors));
            }
        }
        public void GenMeteors(GenerationProgress progress, GameConfiguration _)
        {

        }
    }
}
