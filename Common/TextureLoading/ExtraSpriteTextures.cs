using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EbonianMod.Common.TextureLoading
{
    public static class ExtraSpriteTextures
    {
        public static Texture2D Achievements, ArchmageXHeli, ArchmageXHeli_Glow, arrow, BottomPanel, iconBorder, Logo, scrollbarBg, smallIconHover,
            smallIconOutline, TopPanel;

        public static void LoadExtraSpriteTextures()
        {
            Achievements = Helper.GetExtraTexture("Sprites/Achievements");

            ArchmageXHeli = Helper.GetExtraTexture("Sprites/ArchmageXHeli");

            ArchmageXHeli_Glow = Helper.GetExtraTexture("Sprites/ArchmageXHeli_Glow");

            arrow = Helper.GetExtraTexture("Sprites/arrow");

            BottomPanel = Helper.GetExtraTexture("Sprites/BottomPanel");

            iconBorder = Helper.GetExtraTexture("Sprites/iconBorder");

            Logo = Helper.GetExtraTexture("Sprites/Logo");

            scrollbarBg = Helper.GetExtraTexture("Sprites/scrollbarBg");

            smallIconHover = Helper.GetExtraTexture("Sprites/smallIconHover");

            smallIconOutline = Helper.GetExtraTexture("Sprites/smallIconOutline");

            TopPanel = Helper.GetExtraTexture("Sprites/TopPanel");

        }
    }
}
