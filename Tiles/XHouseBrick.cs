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
            //ItemDrop = ItemType<Items.Tiles.EbonHiveI>();
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

    public class XHouseBrickReplica : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;

            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            DustType = DustID.Venom;
            MineResist = 1;
            MinPick = 0;
            HitSound = SoundID.Tink;
            AddMapEntry(Color.Indigo);
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class XHouseBrickReplicaItem : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = 0;
            Item.useTurn = true;
            Item.rare = 0;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.rare = 2;
            Item.useTime = 10;
            Item.useStyle = 1;
            Item.consumable = true;
            Item.createTile = TileType<XHouseBrickReplica>();
        }
        public override void AddRecipes()
        {
            CreateRecipe(4).AddIngredient(ItemID.Amethyst, 2).AddTile(TileID.HeavyWorkBench).Register();
        }
    }
}
