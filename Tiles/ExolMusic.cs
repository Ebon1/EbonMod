using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using Terraria.ObjectData;
using EbonianMod.Items.Tiles;

namespace EbonianMod.Tiles
{
    public class ExolMusic : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            /*ModTranslation name = CreateMapEntryName();
            name.SetDefault("Music Box");*/
            AddMapEntry(new Color(200, 200, 200));
        }

        public override bool RightClick(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX >= 36)
            {
                Helper.SetBossTitle(250, "Now Playing", Color.Orange, "Rage by Yuri O", -2);
                return true;
            }
            return false;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ExolMusicI>();
        }
    }
}
