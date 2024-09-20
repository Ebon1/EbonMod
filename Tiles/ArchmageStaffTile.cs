using EbonianMod.NPCs.ArchmageX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace EbonianMod.Tiles
{
    public class ArchmageStaffTile : ModTile
    {
        public float[,] staffAlpha = new float[Main.maxTilesX, Main.maxTilesY];
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = true;
            AddMapEntry(Color.Transparent);
            Main.tileBlockLight[Type] = false;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileMergeDirt[Type] = false;
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }
        public override bool KillSound(int i, int j, bool fail)
        {
            return false;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<ArchmageX>()))
                staffAlpha[i, j] = MathHelper.Lerp(staffAlpha[i, j], .1f, 0.1f);
            else
                staffAlpha[i, j] = MathHelper.Lerp(staffAlpha[i, j], 1f, 0.2f);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16 + MathF.Sin(Main.GlobalTimeWrappedHourly * .15f) * 16) + zero - Main.screenPosition;
            Texture2D tex = Helper.GetTexture("Items/Weapons/Magic/StaffOfX");
            Texture2D bloom = Helper.GetTexture("Items/Weapons/Magic/StaffOfX_Bloom");
            Texture2D streak = Helper.GetExtraTexture("Extras2/scratch_02");
            UnifiedRandom rand = new UnifiedRandom(i + j);

            Main.spriteBatch.SaveCurrent();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

            int max = 20;
            for (int k = 0; k < max; k++)
            {
                float factor = (MathF.Sin(Main.GlobalTimeWrappedHourly + i * rand.Next(1, 100) * .02f) + 1) * 0.5f * staffAlpha[i, j];
                float alpha = MathHelper.Clamp(MathHelper.Lerp(0.4f, -0.1f, factor) * 2, 0, 0.5f);
                float angle = Helper.CircleDividedEqually(k, max);
                float scale = rand.NextFloat(0.5f, 1.5f) * factor;
                Vector2 offset = new Vector2(rand.NextFloat(50) * factor * scale, 0).RotatedBy(angle);
                for (int l = 0; l < 2; l++)
                    Main.spriteBatch.Draw(streak, position + new Vector2(rand.NextFloat(-80, 80), rand.NextFloat(-20, 20)) + offset, null, Color.Violet * (alpha * staffAlpha[i, j]), angle, new Vector2(0, streak.Height / 2), new Vector2(alpha, factor * 2) * scale * 0.5f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.5f + MathHelper.Clamp(MathF.Sin(Main.GlobalTimeWrappedHourly * .5f), 0, 0.4f)) * staffAlpha[i, j]), MathHelper.PiOver4, bloom.Size() / 2, 1, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(bloom, position, null, Color.Violet * ((0.05f + (MathF.Sin(Main.GlobalTimeWrappedHourly * .55f) + .5f) * 0.3f) * staffAlpha[i, j]), MathHelper.PiOver4, bloom.Size() / 2, 1.1f, SpriteEffects.None, 0);
            Main.spriteBatch.ApplySaved();

            Main.spriteBatch.Draw(tex, position, null, Color.White * staffAlpha[i, j], MathHelper.PiOver4, tex.Size() / 2, 1, SpriteEffects.None, 0);
            return false;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            blockDamaged = false;
            return false;
        }
    }
}
