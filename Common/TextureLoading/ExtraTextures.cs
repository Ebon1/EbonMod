using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EbonianMod.Common.TextureLoading
{
    public static class ExtraTextures
    {
        // Extras folder
        public static Texture2D alphaGradient, black, Bloom, chevron, chevron_single, Circle, circlething, coherentNoise, cone2, cone2_blur, cone4, cone5, cone7, crosslight,
        darkShadowflameGradient, EbonianGatlingBullet, Empty, Ex1, exclamation, explosion, explosion2, Extra_201, Extra_209, Fire, fireball, flameEye2, FlamesSeamless,
        flare, GlowyLine, gradation, hazardUnblurred, laser, laser_purple, laser2, laser3, laser3_transparent, laser4, laser5, laserMask, Line, LintyTrail, MagicBurst,
        maroon, pixel, Placeholder, PulseCircle, PulseCircle2, red, redGradient, ring, ripple, rune_alt, rune_alt_bloom, saucerBeam, seamlessNoise, seamlessNoise2,
        seamlessNoise3, seamlessNoiseHighContrast, shadowflameGradient, space, space_full, speckle, Spotlight, swirlyNoise, Tentacle, textGlow, trail_01, trail_04, TrailShape,
        TrailShape_LongSolidWavy, vein, Vignette, vortex, vortex3, wavyLaser, wavyLaser2;

        public static void LoadExtraTextures1()
        {
            black = Helper.GetExtraTexture("black", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Bloom = Helper.GetExtraTexture("Bloom", assetRequestMode: AssetRequestMode.ImmediateLoad);

            chevron = Helper.GetExtraTexture("chevron", assetRequestMode: AssetRequestMode.ImmediateLoad);

            chevron_single = Helper.GetExtraTexture("chevron_single", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Circle = Helper.GetExtraTexture("Circle", assetRequestMode: AssetRequestMode.ImmediateLoad);

            circlething = Helper.GetExtraTexture("circlething", assetRequestMode: AssetRequestMode.ImmediateLoad);

            coherentNoise = Helper.GetExtraTexture("coherentNoise", assetRequestMode: AssetRequestMode.ImmediateLoad);

            cone2 = Helper.GetExtraTexture("cone2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            cone2_blur = Helper.GetExtraTexture("cone2_blur", assetRequestMode: AssetRequestMode.ImmediateLoad);

            cone4 = Helper.GetExtraTexture("cone4", assetRequestMode: AssetRequestMode.ImmediateLoad);

            cone5 = Helper.GetExtraTexture("cone5", assetRequestMode: AssetRequestMode.ImmediateLoad);

            cone7 = Helper.GetExtraTexture("cone7", assetRequestMode: AssetRequestMode.ImmediateLoad);

            crosslight = Helper.GetExtraTexture("crosslight", assetRequestMode: AssetRequestMode.ImmediateLoad);


            darkShadowflameGradient = Helper.GetExtraTexture("darkShadowflameGradient", assetRequestMode: AssetRequestMode.ImmediateLoad);

            EbonianGatlingBullet = Helper.GetExtraTexture("EbonianGatlingBullet", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Empty = Helper.GetExtraTexture("Empty", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Ex1 = Helper.GetExtraTexture("Ex1", assetRequestMode: AssetRequestMode.ImmediateLoad);

            exclamation = Helper.GetExtraTexture("exclamation", assetRequestMode: AssetRequestMode.ImmediateLoad);

            explosion = Helper.GetExtraTexture("explosion", assetRequestMode: AssetRequestMode.ImmediateLoad);

            explosion2 = Helper.GetExtraTexture("explosion2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Extra_201 = Helper.GetExtraTexture("Extra_201", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Extra_209 = Helper.GetExtraTexture("Extra_209", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Fire = Helper.GetExtraTexture("Fire", assetRequestMode: AssetRequestMode.ImmediateLoad);

            fireball = Helper.GetExtraTexture("fireball", assetRequestMode: AssetRequestMode.ImmediateLoad);

            flameEye2 = Helper.GetExtraTexture("flameEye2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            FlamesSeamless = Helper.GetExtraTexture("FlamesSeamless", assetRequestMode: AssetRequestMode.ImmediateLoad);


            flare = Helper.GetExtraTexture("flare", assetRequestMode: AssetRequestMode.ImmediateLoad);

            GlowyLine = Helper.GetExtraTexture("GlowyLine", assetRequestMode: AssetRequestMode.ImmediateLoad);

            gradation = Helper.GetExtraTexture("gradation", assetRequestMode: AssetRequestMode.ImmediateLoad);

            hazardUnblurred = Helper.GetExtraTexture("hazardUnblurred", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser = Helper.GetExtraTexture("laser", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser_purple = Helper.GetExtraTexture("laser_purple", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser2 = Helper.GetExtraTexture("laser2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser3 = Helper.GetExtraTexture("laser3", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser3_transparent = Helper.GetExtraTexture("laser3_transparent", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser4 = Helper.GetExtraTexture("laser4", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laser5 = Helper.GetExtraTexture("laser5", assetRequestMode: AssetRequestMode.ImmediateLoad);

            laserMask = Helper.GetExtraTexture("laserMask", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Line = Helper.GetExtraTexture("Line", assetRequestMode: AssetRequestMode.ImmediateLoad);

            LintyTrail = Helper.GetExtraTexture("LintyTrail", assetRequestMode: AssetRequestMode.ImmediateLoad);

            MagicBurst = Helper.GetExtraTexture("MagicBurst", assetRequestMode: AssetRequestMode.ImmediateLoad);


            maroon = Helper.GetExtraTexture("maroon", assetRequestMode: AssetRequestMode.ImmediateLoad);

            pixel = Helper.GetExtraTexture("pixel", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Placeholder = Helper.GetExtraTexture("Placeholder", assetRequestMode: AssetRequestMode.ImmediateLoad);

            PulseCircle = Helper.GetExtraTexture("PulseCircle", assetRequestMode: AssetRequestMode.ImmediateLoad);

            PulseCircle2 = Helper.GetExtraTexture("PulseCircle2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            red = Helper.GetExtraTexture("red", assetRequestMode: AssetRequestMode.ImmediateLoad);

            redGradient = Helper.GetExtraTexture("redGradient", assetRequestMode: AssetRequestMode.ImmediateLoad);

            ring = Helper.GetExtraTexture("ring", assetRequestMode: AssetRequestMode.ImmediateLoad);

            ripple = Helper.GetExtraTexture("ripple", assetRequestMode: AssetRequestMode.ImmediateLoad);

            rune_alt = Helper.GetExtraTexture("rune_alt", assetRequestMode: AssetRequestMode.ImmediateLoad);

            rune_alt_bloom = Helper.GetExtraTexture("rune_alt_bloom", assetRequestMode: AssetRequestMode.ImmediateLoad);

            saucerBeam = Helper.GetExtraTexture("saucerBeam", assetRequestMode: AssetRequestMode.ImmediateLoad);

            seamlessNoise = Helper.GetExtraTexture("seamlessNoise", assetRequestMode: AssetRequestMode.ImmediateLoad);

            seamlessNoise2 = Helper.GetExtraTexture("seamlessNoise2", assetRequestMode: AssetRequestMode.ImmediateLoad);

            seamlessNoise3 = Helper.GetExtraTexture("seamlessNoise3", assetRequestMode: AssetRequestMode.ImmediateLoad);


            seamlessNoiseHighContrast = Helper.GetExtraTexture("seamlessNoiseHighContrast", assetRequestMode: AssetRequestMode.ImmediateLoad);

            shadowflameGradient = Helper.GetExtraTexture("shadowflameGradient", assetRequestMode: AssetRequestMode.ImmediateLoad);

            space = Helper.GetExtraTexture("space", assetRequestMode: AssetRequestMode.ImmediateLoad);

            space_full = Helper.GetExtraTexture("space_full", assetRequestMode: AssetRequestMode.ImmediateLoad);

            speckle = Helper.GetExtraTexture("speckle", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Spotlight = Helper.GetExtraTexture("Spotlight", assetRequestMode: AssetRequestMode.ImmediateLoad);

            swirlyNoise = Helper.GetExtraTexture("swirlyNoise", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Tentacle = Helper.GetExtraTexture("Tentacle", assetRequestMode: AssetRequestMode.ImmediateLoad);

            textGlow = Helper.GetExtraTexture("textGlow", assetRequestMode: AssetRequestMode.ImmediateLoad);

            trail_01 = Helper.GetExtraTexture("trail_01", assetRequestMode: AssetRequestMode.ImmediateLoad);

            trail_04 = Helper.GetExtraTexture("trail_04", assetRequestMode: AssetRequestMode.ImmediateLoad);

            TrailShape = Helper.GetExtraTexture("TrailShape", assetRequestMode: AssetRequestMode.ImmediateLoad);


            TrailShape_LongSolidWavy = Helper.GetExtraTexture("TrailShape_LongSolidWavy", assetRequestMode: AssetRequestMode.ImmediateLoad);

            vein = Helper.GetExtraTexture("vein", assetRequestMode: AssetRequestMode.ImmediateLoad);

            Vignette = Helper.GetExtraTexture("Vignette", assetRequestMode: AssetRequestMode.ImmediateLoad);

            vortex = Helper.GetExtraTexture("vortex", assetRequestMode: AssetRequestMode.ImmediateLoad);

            vortex3 = Helper.GetExtraTexture("vortex3", assetRequestMode: AssetRequestMode.ImmediateLoad);

            wavyLaser = Helper.GetExtraTexture("wavyLaser", assetRequestMode: AssetRequestMode.ImmediateLoad);

            wavyLaser2 = Helper.GetExtraTexture("wavyLaser2", assetRequestMode: AssetRequestMode.ImmediateLoad);

        }
    }
}
