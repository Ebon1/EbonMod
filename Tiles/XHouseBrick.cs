using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace EbonianMod.Tiles
{
    public class XHouseBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            DustType = DustID.GemAmethyst;
            //ItemDrop = ModContent.ItemType<Items.Tiles.EbonHiveI>();
            MineResist = 10;
            MinPick = 1000;
            AddMapEntry(Color.Indigo);
        }
        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            tile.IsActuated = !Main.tile[i, j].IsActuated;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            return false;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
