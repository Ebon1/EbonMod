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
        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (type == TileID.GemLocks && Main.tile[i, j + 5].TileType == TileType<XHouseBrick>()) return false;
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }
        public override bool CanExplode(int i, int j, int type)
        {
            if (type == TileID.GemLocks && Main.tile[i, j + 5].TileType == TileType<XHouseBrick>()) return false;
            return base.CanExplode(i, j, type);
        }
    }
    public class EbonGlobalWall : GlobalWall
    {
        public override void KillWall(int i, int j, int type, ref bool fail)
        {
            if (Main.tile[i, j].TileType == TileID.GemLocks && Main.tile[i, j + 5].TileType == TileType<XHouseBrick>())
                fail = true;
            base.KillWall(i, j, type, ref fail);
        }
        public override bool CanExplode(int i, int j, int type)
        {
            if (Main.tile[i, j].TileType == TileID.GemLocks && Main.tile[i, j + 5].TileType == TileType<XHouseBrick>())
                return false;
            return base.CanExplode(i, j, type);
        }
    }
}
