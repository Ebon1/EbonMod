using EbonianMod.Items.Consumables.Food;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Tiles
{
    public class ModGlobalTile : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (type == TileID.Dirt)
            {
                if (Main.rand.NextBool(200))
                    Item.NewItem(Item.GetSource_NaturalSpawn(), new Vector2(i * 16, j * 16), ItemType<Potato>(), Main.rand.Next(1, 10));
            }
        }
    }
}
