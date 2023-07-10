using EbonianMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Tiles
{
    public class EbonHiveBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileMerge[ModContent.TileType<EbonHiveBlockSpecial>()][Type] = true;
            Main.tileMerge[ModContent.TileType<EbonHiveRock>()][Type] = true;
            Main.tileMerge[ModContent.TileType<EbonHiveRock2>()][Type] = true;
            DustType = DustID.GreenBlood;
            RegisterItemDrop(ModContent.ItemType<Items.Tiles.EbonHiveI>());

            AddMapEntry(Color.LawnGreen);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    public class EbonHiveBlockSpecial : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[ModContent.TileType<EbonHiveBlock>()][Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            DustType = DustID.GreenBlood;
            RegisterItemDrop(ModContent.ItemType<Items.Accessories.EbonianHeart>());

            AddMapEntry(Color.LawnGreen);
        }
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<EbonianHeart>());
        }
        public Rectangle frame;
        public int frameCounter;
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Vector2 neckOrigin = new Vector2(i * 16, j * 16) + new Vector2(8, 8) - Main.screenPosition;
            Vector2 center = neckOrigin + new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly) * 20, -100);
            Vector2 finalCenter = neckOrigin + new Vector2((float)Math.Sin(Main.GlobalTimeWrappedHourly) * 20, -100);
            Vector2 distToProj = neckOrigin - center;
            float projRotation = distToProj.ToRotation() - 1.57f;
            float distance = distToProj.Length();
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            while (distance > 6 && !float.IsNaN(distance))
            {
                distToProj.Normalize();
                distToProj *= 6;
                center += distToProj;
                distToProj = neckOrigin - center;
                distance = distToProj.Length();

                //Draw chain
                spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/HeartChain").Value, center + zero,
                    null, Lighting.GetColor(new(i, j)), projRotation,
                    new Vector2(12 * 0.5f, 6 * 0.5f), 1f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(ModContent.Request<Texture2D>("EbonianMod/Items/Accessories/EbonianHeartNPC").Value, finalCenter + zero,
                        frame, Lighting.GetColor(new(i, j)), 0,
                        new Vector2(34 * 0.5f, 34 * 0.5f), 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Items/Accessories/EbonianHeartNPC_Glow").Value, finalCenter + zero,
                        frame, Color.White, 0,
                        new Vector2(34 * 0.5f, 34 * 0.5f), 1f, SpriteEffects.None, 0);

            frameCounter++;
            if (frameCounter < 2)
            {
                frame.Y = 0 * 36;
            }
            else if (frameCounter < 4)
            {
                frame.Y = 1 * 36;
            }
            else if (frameCounter < 6)
            {
                frame.Y = 2 * 36;
            }
            else
            {
                frameCounter = 0;
            }
            frame = new Rectangle(0, frame.Y, ModContent.Request<Texture2D>("EbonianMod/Items/Accessories/EbonianHeartNPC").Value.Width, ModContent.Request<Texture2D>("EbonianMod/Items/Accessories/EbonianHeartNPC").Value.Height / 3);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
