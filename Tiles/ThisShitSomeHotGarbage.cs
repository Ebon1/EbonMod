using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using Terraria.ObjectData;
using EbonianMod.Items.Tiles;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;

namespace EbonianMod.Tiles
{
    internal class ThisShitSomeHotGarbage : ModTile
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

            RegisterItemDrop(ItemType<HotMusic>());
            /*ModTranslation name = CreateMapEntryName();
            name.SetDefault("Music Box");*/
            AddMapEntry(new Color(200, 200, 200));
        }
        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            // This code spawns the music notes when the music box is open.
            if (Lighting.UpdateEveryFrame && new FastRandom(Main.TileFrameSeed).WithModifier(i, j).Next(4) != 0)
            {
                return;
            }

            Tile tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile) || tile.TileFrameX != 36 || tile.TileFrameY % 36 != 0 || (int)Main.timeForVisualEffects % 7 != 0 || !Main.rand.NextBool(3))
            {
                return;
            }

            int MusicNote = Main.rand.Next(570, 573);
            Vector2 SpawnPosition = new Vector2(i * 16 + 8, j * 16 - 8);
            Vector2 NoteMovement = new Vector2(Main.WindForVisuals * 2f, -0.5f);
            NoteMovement.X *= Main.rand.NextFloat(0.5f, 1.5f);
            NoteMovement.Y *= Main.rand.NextFloat(0.5f, 1.5f);
            switch (MusicNote)
            {
                case 572:
                    SpawnPosition.X -= 8f;
                    break;
                case 571:
                    SpawnPosition.X -= 4f;
                    break;
            }

            Gore.NewGore(new EntitySource_TileUpdate(i, j), SpawnPosition, NoteMovement, MusicNote, 0.8f);
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<HotMusic>();
        }
    }
}
