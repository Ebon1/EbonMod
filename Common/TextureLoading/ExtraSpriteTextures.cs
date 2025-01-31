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
            Achievements = Helper.GetExtraTexture("Sprites/Achievements", assetRequestMode: AssetRequestMode.ImmediateLoad);

            ArchmageXHeli = Helper.GetExtraTexture("Sprites/ArchmageXHeli", assetRequestMode: AssetRequestMode.ImmediateLoad);

            ArchmageXHeli_Glow = Helper.GetExtraTexture("Sprites/ArchmageXHeli_Glow", assetRequestMode: AssetRequestMode.ImmediateLoad);

            arrow = Helper.GetExtraTexture("Sprites/arrow", assetRequestMode: AssetRequestMode.ImmediateLoad);

            BottomPanel = Helper.GetExtraTexture("Sprites/BottomPanel", assetRequestMode: AssetRequestMode.ImmediateLoad);

            iconBorder = Helper.GetExtraTexture("Sprites/iconBorder", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Logo = Helper.GetExtraTexture("Sprites/Logo", assetRequestMode: AssetRequestMode.ImmediateLoad);

            scrollbarBg = Helper.GetExtraTexture("Sprites/scrollbarBg", assetRequestMode: AssetRequestMode.ImmediateLoad);

            smallIconHover = Helper.GetExtraTexture("Sprites/smallIconHover", assetRequestMode: AssetRequestMode.ImmediateLoad);

            smallIconOutline = Helper.GetExtraTexture("Sprites/smallIconOutline", assetRequestMode: AssetRequestMode.ImmediateLoad);

            TopPanel = Helper.GetExtraTexture("Sprites/TopPanel", assetRequestMode: AssetRequestMode.ImmediateLoad);

        }
    }
}
