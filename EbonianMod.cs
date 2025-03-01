using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using EbonianMod.Common.Graphics.Skies;
using System.Collections.Generic;
using System;
using Terraria.GameContent.Skies;
using System.IO;
using EbonianMod.Common.Graphics;

namespace EbonianMod
{
    public class EbonianMod : Mod
    {
        public static EbonianMod Instance;
        public static List<int> projectileFinalDrawList = new List<int>();
        public RenderTarget2D blurrender, invisRender, affectedByInvisRender;
        public RenderTarget2D[] renders = new RenderTarget2D[10];
        public static BGParticleSys sys;
        public override void Load()
        {
            Instance = this;
            LoadEffects();
            LoadDrawCache();
            sys = new();
        }
        public override void Unload()
        {
            projectileFinalDrawList.Clear();
            ResetCache(ref invisibleMaskCache);
            ResetCache(ref affectedByInvisibleMaskCache);
            ResetCache(ref blurDrawCache);
            ResetCache(ref pixelationDrawCachePre);
            ResetCache(ref pixelationDrawCachePost);
            ResetCache(ref addPixelationDrawCachePre);
            ResetCache(ref addPixelationDrawCachePost);
            ResetCache(ref finalDrawCache);
            sys = null;
        }
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            EbonianNetCode.HandlePacket(reader, whoAmI);
        }


        public static List<Action> invisibleMaskCache = [], affectedByInvisibleMaskCache = [],
            blurDrawCache = [], pixelationDrawCachePre = [], pixelationDrawCachePost = [],
            addPixelationDrawCachePre = [], addPixelationDrawCachePost = [], finalDrawCache = [];
        public void LoadDrawCache()
        {
            invisibleMaskCache ??= [];
            affectedByInvisibleMaskCache ??= [];
            blurDrawCache ??= [];
            pixelationDrawCachePre ??= [];
            pixelationDrawCachePost ??= [];
            addPixelationDrawCachePre ??= [];
            addPixelationDrawCachePost ??= [];
            finalDrawCache ??= [];
        }
        public void ResetCache(ref List<Action> cache)
        {
            cache.Clear();
            cache = [];
        }


        public static Effect bloom, softBloom, Tentacle, TentacleBlack, TentacleRT, ScreenDistort,
            SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1,
            Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha, Crack, Blur,
            RTOutline, metaballGradient, metaballGradientNoiseTex, invisibleMask, PullingForce,
            displacementMap, waterEffect, spherize;
        public void LoadEffects()
        {
            bloom = Request<Effect>("EbonianMod/Effects/bloom", AssetRequestMode.ImmediateLoad).Value;
            Test1 = Request<Effect>("EbonianMod/Effects/Test1", AssetRequestMode.ImmediateLoad).Value;
            HorizBlur = Request<Effect>("EbonianMod/Effects/horizBlur", AssetRequestMode.ImmediateLoad).Value;
            Blur = Request<Effect>("EbonianMod/Effects/Blur", AssetRequestMode.ImmediateLoad).Value;
            Crack = Request<Effect>("EbonianMod/Effects/crackTest", AssetRequestMode.ImmediateLoad).Value;
            RTAlpha = Request<Effect>("EbonianMod/Effects/RTAlpha", AssetRequestMode.ImmediateLoad).Value;
            RTOutline = Request<Effect>("EbonianMod/Effects/RTOutline", AssetRequestMode.ImmediateLoad).Value;
            CrystalShine = Request<Effect>("EbonianMod/Effects/CrystalShine", AssetRequestMode.ImmediateLoad).Value;
            TextGradient = Request<Effect>("EbonianMod/Effects/TextGradient", AssetRequestMode.ImmediateLoad).Value;
            TextGradient2 = Request<Effect>("EbonianMod/Effects/TextGradient2", AssetRequestMode.ImmediateLoad).Value;
            TextGradientY = Request<Effect>("EbonianMod/Effects/TextGradientY", AssetRequestMode.ImmediateLoad).Value;
            Test2 = Request<Effect>("EbonianMod/Effects/Test2", AssetRequestMode.ImmediateLoad).Value;
            Galaxy = Request<Effect>("EbonianMod/Effects/Galaxy", AssetRequestMode.ImmediateLoad).Value;
            LavaRT = Request<Effect>("EbonianMod/Effects/LavaRT", AssetRequestMode.ImmediateLoad).Value;
            SpriteRotation = Request<Effect>("EbonianMod/Effects/spriteRotation", AssetRequestMode.ImmediateLoad).Value;
            BeamShader = Request<Effect>("EbonianMod/Effects/Beam", AssetRequestMode.ImmediateLoad).Value;
            Lens = Request<Effect>("EbonianMod/Effects/Lens", AssetRequestMode.ImmediateLoad).Value;
            Tentacle = Request<Effect>("EbonianMod/Effects/Tentacle", AssetRequestMode.ImmediateLoad).Value;
            TentacleRT = Request<Effect>("EbonianMod/Effects/TentacleRT", AssetRequestMode.ImmediateLoad).Value;
            ScreenDistort = Request<Effect>("EbonianMod/Effects/DistortMove", AssetRequestMode.ImmediateLoad).Value;
            TentacleBlack = Request<Effect>("EbonianMod/Effects/TentacleBlack", AssetRequestMode.ImmediateLoad).Value;
            TrailShader = Request<Effect>("EbonianMod/Effects/TrailShader", AssetRequestMode.ImmediateLoad).Value;
            metaballGradient = Request<Effect>("EbonianMod/Effects/metaballGradient", AssetRequestMode.ImmediateLoad).Value;
            metaballGradientNoiseTex = Request<Effect>("EbonianMod/Effects/metaballGradientNoiseTex", AssetRequestMode.ImmediateLoad).Value;
            invisibleMask = Request<Effect>("EbonianMod/Effects/invisibleMask", AssetRequestMode.ImmediateLoad).Value;
            PullingForce = Request<Effect>("EbonianMod/Effects/PullingForce", AssetRequestMode.ImmediateLoad).Value;
            displacementMap = Request<Effect>("EbonianMod/Effects/displacementMap", AssetRequestMode.ImmediateLoad).Value;
            waterEffect = Request<Effect>("EbonianMod/Effects/waterEffect", AssetRequestMode.ImmediateLoad).Value;
            spherize = Request<Effect>("EbonianMod/Effects/spherize", AssetRequestMode.ImmediateLoad).Value;
            Filters.Scene["EbonianMod:CorruptTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.68f, .56f, .73f).UseOpacity(0.35f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CorruptTint"] = new BasicTint();

            Filters.Scene["EbonianMod:XMartian"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0, 0, 0).UseOpacity(0), EffectPriority.High);
            SkyManager.Instance["EbonianMod:XMartian"] = new MartianSky();

            Filters.Scene["EbonianMod:CrimsonTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.75f, 0f, 0f).UseOpacity(0.35f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CrimsonTint"] = new BasicTint();

            Filters.Scene["EbonianMod:Conglomerate"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.25f, .1f, 0f).UseOpacity(0.45f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:Conglomerate"] = new ConglomerateSky();

            Filters.Scene["EbonianMod:HellTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.1f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint"] = new BasicTint();
            Filters.Scene["EbonianMod:HellTint2"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0.03f, 0f, .18f).UseOpacity(0.425f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint2"] = new BasicTint();
            Filters.Scene["EbonianMod:ScreenFlash"] = new Filter(new ScreenShaderData(Request<Effect>("EbonianMod/Effects/ScreenFlash", AssetRequestMode.ImmediateLoad), "Flash"), EffectPriority.VeryHigh);
        }
    }
}
