using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria.ID;
using EbonianMod.Dusts;
using EbonianMod.NPCs.Exol;
using EbonianMod.Common.Systems.Skies;
using System.Collections.Generic;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using ReLogic.Graphics;
using EbonianMod.Projectiles.Exol;
////using EbonianMod.Worldgen.Subworlds;
////
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using EbonianMod.NPCs.Garbage;
using Humanizer;
using System;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Linq;
using EbonianMod.Common.Systems;
using Terraria.GameContent.Skies;
using EbonianMod.Projectiles.ArchmageX;
using Microsoft.CodeAnalysis;
using Terraria.DataStructures;
using EbonianMod.Common.Systems.Misc;

namespace EbonianMod
{
    public class EbonianMod : Mod
    {
        public static EbonianMod Instance;
        public static Effect Tentacle, TentacleBlack, TentacleRT, ScreenDistort, SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, HorizBlur,
            TrailShader, RTAlpha, Crack, Blur, RTOutline, metaballGradient, metaballGradientNoiseTex, invisibleMask, PullingForce, displacementMap;
        public readonly List<Effect> Effects = new List<Effect>()
        {
            Tentacle, TentacleBlack, TentacleRT, ScreenDistort, SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha,
            Crack, Blur, RTOutline, metaballGradient,metaballGradientNoiseTex, invisibleMask, PullingForce,  displacementMap
    };
        public RenderTarget2D blurrender, invisRender, affectedByInvisRender;
        public RenderTarget2D[] renders = new RenderTarget2D[5];
        public static DynamicSpriteFont lcd;
        public static BGParticleSys sys;
        internal static void SolidTopCollision(Terraria.On_Player.orig_Update_NPCCollision orig, Player self) //https://discord.com/channels/103110554649894912/711551818194485259/998428409455714397
        {
            var modSelf = self.GetModPlayer<EbonianPlayer>();

            if (self.grappling[0] < 0)
            {
                modSelf.platformDropTimer--;

                if (self.controlDown && modSelf.platformTimer >= 6)
                    modSelf.platformDropTimer = 8;

                bool success = false;

                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile proj = Main.projectile[i];

                    if (!proj.active || proj.ModProjectile == null || proj.type != ModContent.ProjectileType<EPlatform>() || (proj.whoAmI == modSelf.platformWhoAmI && modSelf.platformDropTimer > 0))
                        continue;

                    var playerBox = new Rectangle((int)self.position.X, (int)self.position.Y + self.height, self.width, 1);
                    var floorBox = new Rectangle((int)proj.position.X, (int)proj.position.Y - (int)proj.velocity.Y, proj.width, 16 + (int)Math.Max(self.velocity.Y, 0));

                    if (playerBox.Intersects(floorBox) && self.velocity.Y > 0 && !Collision.SolidCollision(self.Bottom, self.width, (int)Math.Max(1 + proj.velocity.Y, 0)))
                    {
                        proj.timeLeft -= 2;
                        proj.ai[2] = 3;
                        proj.ai[0]++;
                        self.gfxOffY = proj.gfxOffY;
                        self.position.Y = proj.position.Y - self.height + 4;
                        self.velocity.Y = 0;
                        self.fallStart = (int)(self.position.Y / 16f);

                        if (self == Main.LocalPlayer)
                            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                        if (modSelf.platformTimer < 0)
                            modSelf.platformTimer = -1;

                        modSelf.platformTimer++;
                        modSelf.platformWhoAmI = proj.whoAmI;

                        orig(self);

                        success = true;
                        break;
                    }
                }

                if (!success && modSelf.platformDropTimer <= 0)
                {
                    modSelf.platformTimer--;
                    modSelf.platformWhoAmI = -1;
                }
            }

            orig(self);
        }

        void DrawNPC(Terraria.On_Main.orig_DrawNPC orig, global::Terraria.Main self, int iNPCIndex, bool behindTiles)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            if (NPC.AnyNPCs(ModContent.NPCType<Exol>()))
                SmokeDustAkaFireDustButNoGlow.DrawAll(Main.spriteBatch);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            orig(self, iNPCIndex, behindTiles);
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && (projectile.type == ModContent.ProjectileType<TExplosion>()/* || projectile.type == ModContent.ProjectileType<ScreenFlash>()*/))
                {
                    Color color = Color.White;
                    projectile.ModProjectile.PreDraw(ref color);
                }
            }
            if (FlashAlpha > 0)
            {
                Main.spriteBatch.Draw(Helper.GetExtraTexture("Line"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * FlashAlpha * 2);
            }
        }

        public void DrawBehindTilesAndWalls(Terraria.On_Main.orig_DrawBG orig, global::Terraria.Main self)
        {
            orig(self);
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && (projectile.type == ModContent.ProjectileType<EBoulder>() || projectile.type == ModContent.ProjectileType<EBoulder2>()))
                {
                    Color color = Color.White;
                    projectile.ModProjectile.PreDraw(ref color);
                }
            }
            sys.DrawParticles();

        }
        public override void Unload()
        {
            sys = null;
            foreach (Effect effect in Effects)
            {
                if (effect != null && !effect.IsDisposed)
                    effect.Dispose();
            }
            //On_FilterManager.EndCapture -= FilterManager_EndCapture;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            On_Main.DrawBG -= DrawBehindTilesAndWalls;
            On_Main.DrawNPC -= DrawNPC;
            On_Player.Update_NPCCollision -= SolidTopCollision;
            On_Main.DrawPlayers_AfterProjectiles -= PreDraw;
            On_VanillaPlayerDrawLayer.Draw -= DrawPlayer;
        }
        public override void Load()
        {

            sys = new();
            Instance = this;
            Test1 = ModContent.Request<Effect>("EbonianMod/Effects/Test1", (AssetRequestMode)1).Value;
            HorizBlur = ModContent.Request<Effect>("EbonianMod/Effects/horizBlur", (AssetRequestMode)1).Value;
            Blur = ModContent.Request<Effect>("EbonianMod/Effects/Blur", (AssetRequestMode)1).Value;
            Crack = ModContent.Request<Effect>("EbonianMod/Effects/crackTest", (AssetRequestMode)1).Value;
            RTAlpha = ModContent.Request<Effect>("EbonianMod/Effects/RTAlpha", (AssetRequestMode)1).Value;
            RTOutline = ModContent.Request<Effect>("EbonianMod/Effects/RTOutline", (AssetRequestMode)1).Value;
            CrystalShine = ModContent.Request<Effect>("EbonianMod/Effects/CrystalShine", (AssetRequestMode)1).Value;
            TextGradient = ModContent.Request<Effect>("EbonianMod/Effects/TextGradient", (AssetRequestMode)1).Value;
            TextGradient2 = ModContent.Request<Effect>("EbonianMod/Effects/TextGradient2", (AssetRequestMode)1).Value;
            TextGradientY = ModContent.Request<Effect>("EbonianMod/Effects/TextGradientY", (AssetRequestMode)1).Value;
            Test2 = ModContent.Request<Effect>("EbonianMod/Effects/Test2", (AssetRequestMode)1).Value;
            Galaxy = ModContent.Request<Effect>("EbonianMod/Effects/Galaxy", (AssetRequestMode)1).Value;
            LavaRT = ModContent.Request<Effect>("EbonianMod/Effects/LavaRT", (AssetRequestMode)1).Value;
            SpriteRotation = ModContent.Request<Effect>("EbonianMod/Effects/spriteRotation", (AssetRequestMode)1).Value;
            BeamShader = ModContent.Request<Effect>("EbonianMod/Effects/Beam", (AssetRequestMode)1).Value;
            Lens = ModContent.Request<Effect>("EbonianMod/Effects/Lens", (AssetRequestMode)1).Value;
            Tentacle = ModContent.Request<Effect>("EbonianMod/Effects/Tentacle", (AssetRequestMode)1).Value;
            TentacleRT = ModContent.Request<Effect>("EbonianMod/Effects/TentacleRT", (AssetRequestMode)1).Value;
            ScreenDistort = ModContent.Request<Effect>("EbonianMod/Effects/DistortMove", (AssetRequestMode)1).Value;
            TentacleBlack = ModContent.Request<Effect>("EbonianMod/Effects/TentacleBlack", (AssetRequestMode)1).Value;
            TrailShader = ModContent.Request<Effect>("EbonianMod/Effects/TrailShader", (AssetRequestMode)1).Value;
            metaballGradient = ModContent.Request<Effect>("EbonianMod/Effects/metaballGradient", (AssetRequestMode)1).Value;
            metaballGradientNoiseTex = ModContent.Request<Effect>("EbonianMod/Effects/metaballGradientNoiseTex", (AssetRequestMode)1).Value;
            invisibleMask = ModContent.Request<Effect>("EbonianMod/Effects/invisibleMask", (AssetRequestMode)1).Value;
            PullingForce = ModContent.Request<Effect>("EbonianMod/Effects/PullingForce", (AssetRequestMode)1).Value;
            displacementMap = ModContent.Request<Effect>("EbonianMod/Effects/displacementMap", (AssetRequestMode)1).Value;
            Filters.Scene["EbonianMod:CorruptTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.68f, .56f, .73f).UseOpacity(0.35f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CorruptTint"] = new BasicTint();

            Filters.Scene["EbonianMod:CrimsonTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.75f, 0f, 0f).UseOpacity(0.45f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CrimsonTint"] = new BasicTint();
            Filters.Scene["EbonianMod:HellTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.2f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint"] = new BasicTint();
            Filters.Scene["EbonianMod:HellTint2"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0.03f, 0f, .18f).UseOpacity(0.425f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint2"] = new BasicTint();
            Filters.Scene["EbonianMod:ScreenFlash"] = new Filter(new ScreenShaderData(ModContent.Request<Effect>("EbonianMod/Effects/ScreenFlash", (AssetRequestMode)1), "Flash"), EffectPriority.VeryHigh);
            //Terraria.Graphics.Effects.On_FilterManager.EndCapture += FilterManager_EndCapture;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            Terraria.On_Main.DrawBG += DrawBehindTilesAndWalls;
            Terraria.On_Main.DrawNPC += DrawNPC;
            Terraria.On_Player.Update_NPCCollision += SolidTopCollision;
            On_Main.DrawPlayers_AfterProjectiles += PreDraw;
            On_VanillaPlayerDrawLayer.Draw += DrawPlayer;
            CreateRender();
        }
        void DrawPlayer(On_VanillaPlayerDrawLayer.orig_Draw orig, PlayerDrawLayer self, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.GetModPlayer<EbonianPlayer>().sheep)
                return;
            orig(self, ref drawInfo);
        }
        void PreDraw(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < S_VerletSystem.verlets.Count; i++)
            {
                if (S_VerletSystem.verlets[i].timeLeft > 0 && S_VerletSystem.verlets[i].verlet != null)
                {
                    float alpha = MathHelper.Clamp(MathHelper.Lerp(0, 2, (float)S_VerletSystem.verlets[i].timeLeft / S_VerletSystem.verlets[i].maxTime), 0, 1);
                    VerletDrawData verletDrawData = S_VerletSystem.verlets[i].drawData;
                    verletDrawData.useColor = true;
                    verletDrawData.color = Lighting.GetColor(S_VerletSystem.verlets[i].verlet.lastP.position.ToTileCoordinates()) * alpha;
                    S_VerletSystem.verlets[i].verlet.Draw(Main.spriteBatch, verletDrawData);
                }
            }
            Main.spriteBatch.End();

            sb.Begin(SpriteSortMode.Deferred, MiscDrawingMethods.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            ReiSmoke.DrawAll(sb);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, MiscDrawingMethods.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.timeLeft > 1 && proj.type == ModContent.ProjectileType<ReiCapeP>())
                {
                    Color color = new Color(69, 420, 0, 1);
                    proj.ModProjectile.PostDraw(color);
                }
            }
            sb.End();
            var old = gd.GetRenderTargets();
            if (!Main.gameMenu && !(Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy && Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro) && gd.GetRenderTargets().Contains(Main.screenTarget))
            {

                /*if (!gd.GetRenderTargets().Contains(Main.screenTarget))
                {
                    gd.Textures[1] = null;
                    gd.SetRenderTarget(Main.screenTarget);
                    gd.Clear(Color.Transparent);
                }

                */
                gd.SetRenderTarget(Main.screenTargetSwap);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();

                gd.SetRenderTarget(renders[0]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.timeLeft > 0 && proj.type == ModContent.ProjectileType<ReiCapeP>())
                    {
                        Color color = Color.White;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();

                gd.SetRenderTarget(renders[1]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                BlackWhiteDust.DrawAll(sb);
                sb.End();

                gd.SetRenderTarget(renders[2]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                XGoopDust.DrawAll(sb);
                sb.End();


                gd.SetRenderTarget(renders[3]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.timeLeft > 0 && proj.type == ModContent.ProjectileType<GarbageFlame>())
                    {
                        Color color = Color.Transparent;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();


                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/space", (AssetRequestMode)1).Value;
                RTOutline.CurrentTechnique.Passes[0].Apply();
                RTOutline.Parameters["m"].SetValue(0.62f);
                RTOutline.Parameters["n"].SetValue(0.01f);
                RTOutline.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.005f, 0));
                sb.Draw(renders[0], Vector2.Zero, Color.White);

                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/black", (AssetRequestMode)1).Value;
                RTOutline.CurrentTechnique.Passes[0].Apply();
                RTOutline.Parameters["m"].SetValue(0.22f);
                RTOutline.Parameters["n"].SetValue(0.1f);
                sb.Draw(renders[1], Vector2.Zero, Color.White);

                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/darkShadowflameGradient", (AssetRequestMode)1).Value;
                gd.Textures[2] = ModContent.Request<Texture2D>("EbonianMod/Extras/space_full", (AssetRequestMode)1).Value;
                gd.Textures[3] = ModContent.Request<Texture2D>("EbonianMod/Extras/seamlessNoiseHighContrast", (AssetRequestMode)1).Value;
                gd.Textures[4] = ModContent.Request<Texture2D>("EbonianMod/Extras/alphaGradient", (AssetRequestMode)1).Value;
                metaballGradientNoiseTex.CurrentTechnique.Passes[0].Apply();
                metaballGradientNoiseTex.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
                metaballGradientNoiseTex.Parameters["offsetX"].SetValue(MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
                metaballGradientNoiseTex.Parameters["offsetY"].SetValue(MathF.Cos(Main.GlobalTimeWrappedHourly) * 0.1f);
                sb.Draw(renders[2], Vector2.Zero, Color.White);

                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/coherentNoise", (AssetRequestMode)1).Value;
                displacementMap.CurrentTechnique.Passes[0].Apply();
                displacementMap.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
                displacementMap.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.25f);
                displacementMap.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
                displacementMap.Parameters["offset"].SetValue(0.0075f);
                displacementMap.Parameters["alpha"].SetValue(0.1f);
                sb.Draw(renders[3], Vector2.Zero, Color.White);
                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/swirlyNoise", (AssetRequestMode)1).Value;
                displacementMap.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
                displacementMap.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.34f);
                sb.Draw(renders[3], Vector2.Zero, Color.White);

                gd.Textures[1] = null;
                gd.Textures[2] = null;
                gd.Textures[3] = null;
                gd.Textures[4] = null;
                sb.End();
            }

            orig(self);

            if (!Main.gameMenu && !(Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy && Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro) && gd.GetRenderTargets().Contains(Main.screenTarget))
            {
                gd.SetRenderTarget(blurrender);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();
                gd.SetRenderTarget(Main.screenTargetSwap);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.active && projectile.type == ModContent.ProjectileType<Noise>())
                    {
                        Color color = Color.Transparent;
                        projectile.ModProjectile.PreDraw(ref color);
                    }
                    if (projectile.active && (projectile.type == ModContent.ProjectileType<Projectiles.Ripple>()))
                    {
                        Texture2D a = TextureAssets.Projectile[projectile.type].Value;
                        Main.spriteBatch.Draw(a, projectile.Center - Main.screenPosition, null, Color.White, 0, a.Size() / 2, projectile.ai[0], SpriteEffects.None, 0f);
                    }
                }
                sb.End();
                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Test2.CurrentTechnique.Passes[0].Apply();
                Test2.Parameters["tex0"].SetValue(Main.screenTargetSwap);
                Test2.Parameters["i"].SetValue(0.02f);
                sb.Draw(blurrender, Vector2.Zero, Color.White);
                sb.End();



                gd.SetRenderTarget(Main.screenTargetSwap);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();

                gd.SetRenderTarget(affectedByInvisRender);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.active && (projectileAffectedByInvisibleMaskList.Contains(projectile.type)))
                    {
                        Color color = Color.Transparent;
                        projectile.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();

                gd.SetRenderTarget(invisRender);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.active && (projectileInvisibleMaskList.Contains(projectile.type)))
                    {
                        Color color = Color.Transparent;
                        projectile.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();
                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                invisibleMask.CurrentTechnique.Passes[0].Apply();
                gd.Textures[1] = invisRender;
                sb.Draw(affectedByInvisRender, Vector2.Zero, Color.White);
                sb.End();
                gd.Textures[1] = null;
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            FireDust.DrawAll(sb);
            ColoredFireDust.DrawAll(sb);
            GenericAdditiveDust.DrawAll(sb);
            SparkleDust.DrawAll(sb);
            LineDustFollowPoint.DrawAll(sb);
            if (!NPC.AnyNPCs(ModContent.NPCType<Exol>()))
                SmokeDustAkaFireDustButNoGlow.DrawAll(Main.spriteBatch);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            if (FlashAlpha > 0)
            {
                Main.spriteBatch.Draw(Helper.GetExtraTexture("Line"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * FlashAlpha * 2);
            }
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && (projectileFinalDrawList.Contains(projectile.type)))
                {
                    Color color = Color.White;
                    projectile.ModProjectile.PreDraw(ref color);
                }
            }
            sb.End();
        }
        public static List<int> projectileFinalDrawList = new List<int>();
        public static List<int> projectileAffectedByInvisibleMaskList = new List<int>();
        public static List<int> projectileInvisibleMaskList = new List<int>();
        private void Main_OnResolutionChanged(Vector2 obj)
        {
            CreateRender();
        }
        public void CreateRender()
        {
            if (Main.netMode != NetmodeID.Server)
                Main.QueueMainThreadAction(() =>
                {
                    for (int i = 0; i < renders.Length; i++)
                    {
                        if (renders[i] != null && !renders[i].IsDisposed)
                            renders[i].Dispose();
                        renders[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    }
                    if (invisRender != null && !invisRender.IsDisposed)
                        invisRender.Dispose();
                    if (affectedByInvisRender != null && !affectedByInvisRender.IsDisposed)
                        affectedByInvisRender.Dispose();
                    if (blurrender != null && !blurrender.IsDisposed)
                        blurrender.Dispose();
                    invisRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    affectedByInvisRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    blurrender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
        }

        public static int ExolID = ModContent.NPCType<Exol>();
        public static float FlashAlpha, FlashAlphaDecrement;
    }
    public class EbonMenu : ModMenu
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/Exol");
        public override string DisplayName => "Ebonian Mod";
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<EbonMenuBG>();
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/Logo");
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            //spriteBatch.Draw(Helper.GetExtraTexture("menutest"), new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
            drawColor = Color.White;
            return true;
        }
        public class EbonMenuBG : ModSurfaceBackgroundStyle
        {
            public override void ModifyFarFades(float[] fades, float transitionSpeed)
            {
            }
        }
    }
}
