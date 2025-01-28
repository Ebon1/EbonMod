using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace EbonianMod.Tiles.Trophies
{
    public class TerrortomaTrophyTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(120, 85, 60), Language.GetText("MapObject.Trophy"));
            DustType = 7;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (plinkTimer[i, j] > 0)
            {
                plinkTimer[i, j] -= 0.1f;
            }

            Tile tile = Main.tile[i, j];

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            spriteBatch.Draw(
                TextureAssets.Tile[Type].Value,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

            if (plinkTimer[i, j] <= 0)
            {
                spriteBatch.Draw(
                    Request<Texture2D>(Texture + "_Glow").Value,
                    new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                    new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                    Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }

        float[,] plinkTimer = new float[Main.maxTilesX, Main.maxTilesY];

        public override void RandomUpdate(int i, int j)
        {
            if (new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight).Intersects(new Rectangle(i * 16, j * 16, 16, 16))
                && Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
                SoundEngine.PlaySound(EbonianSounds.blink, new Vector2(i * 16, j * 16));
            plinkTimer[i, j] = 1;
        }
    }
}