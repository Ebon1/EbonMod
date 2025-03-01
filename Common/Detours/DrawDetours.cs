using EbonianMod.Dusts;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.Projectiles.Garbage;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Terraria.Graphics.Effects;
using EbonianMod.Common.Systems.Verlets;

namespace EbonianMod.Common.Detours
{
    public class DrawDetours : ModSystem
    {
        public override void Load()
        {
            On_FilterManager.EndCapture += FilterManager_EndCapture;
            On_Main.DrawBG += DrawBehindTilesAndWalls;
            On_Main.DrawNPC += DrawNPC;
            On_Main.DrawPlayers_AfterProjectiles += PreDraw;
            On_VanillaPlayerDrawLayer.Draw += DrawPlayer;
            On_Main.DrawCachedProjs += PostDraw;
        }
        public static void DrawAdditiveDusts(SpriteBatch sb, GraphicsDevice gd)
        {

            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Dust d in Main.dust)
            {
                FireDust.DrawAll(sb, d);
                ColoredFireDust.DrawAll(sb, d);
                GenericAdditiveDust.DrawAll(sb, d);
                SparkleDust.DrawAll(sb, d);
                IntenseDustFollowPoint.DrawAll(sb, d);
                LineDustFollowPoint.DrawAll(sb, d);
                EbonianMod.blurDrawCache.Add(() => BlurDust.DrawAll(sb, d));
            }
            sb.End();
        }
        public static void DrawGenericPostScreen(SpriteBatch sb, GraphicsDevice gd)
        {

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (EbonianSystem.FlashAlpha > 0)
            {
                Main.spriteBatch.Draw(ExtraTextures.Line, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * EbonianSystem.FlashAlpha * 2);
            }
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.active && (EbonianMod.projectileFinalDrawList.Contains(projectile.type)))
                {
                    Color color = Color.White;
                    projectile.ModProjectile.PreDraw(ref color);
                }
            }
            foreach (Action draw in EbonianMod.finalDrawCache)
            {
                draw?.Invoke();
            }
            EbonianMod.finalDrawCache.Clear();
            sb.End();
        }
        public static void DrawBlurredContent(SpriteBatch sb, GraphicsDevice gd)
        {

            gd.SetRenderTarget(EbonianMod.Instance.blurrender);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Action draw in EbonianMod.blurDrawCache)
            {
                draw?.Invoke();
            }
            EbonianMod.blurDrawCache.Clear();
            sb.End();
            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            EbonianMod.Test2.CurrentTechnique.Passes[0].Apply();
            EbonianMod.Test2.Parameters["tex0"].SetValue(Main.screenTargetSwap);
            EbonianMod.Test2.Parameters["i"].SetValue(0.02f);
            sb.Draw(EbonianMod.Instance.blurrender, Vector2.Zero, Color.White);
            sb.End();
        }
        public static void DrawInvisMasks(SpriteBatch sb, GraphicsDevice gd)
        {

            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(EbonianMod.Instance.affectedByInvisRender);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (Action draw in EbonianMod.affectedByInvisibleMaskCache)
            {
                draw?.Invoke();
            }
            EbonianMod.affectedByInvisibleMaskCache.Clear();
            sb.End();

            gd.SetRenderTarget(EbonianMod.Instance.invisRender);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (Action draw in EbonianMod.invisibleMaskCache)
            {
                draw?.Invoke();
            }
            EbonianMod.invisibleMaskCache.Clear();
            sb.End();
            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            EbonianMod.invisibleMask.CurrentTechnique.Passes[0].Apply();
            gd.Textures[1] = EbonianMod.Instance.invisRender;
            sb.Draw(EbonianMod.Instance.affectedByInvisRender, Vector2.Zero, Color.White);
            sb.End();
            gd.Textures[1] = null;
        }
        public static void DrawPixelatedContent(bool afterEverything, bool additive, SpriteBatch sb, GraphicsDevice gd)
        {
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(EbonianMod.Instance.renders[5]);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, null);

            var list = (afterEverything ? EbonianMod.pixelationDrawCachePost : EbonianMod.pixelationDrawCachePre);
            if (additive) list = (afterEverything ? EbonianMod.addPixelationDrawCachePost : EbonianMod.addPixelationDrawCachePre);
            foreach (Action draw in list)
            {
                draw?.Invoke();
            }
            if (additive)
            {
                if (afterEverything)
                    EbonianMod.addPixelationDrawCachePost.Clear();
                else
                    EbonianMod.addPixelationDrawCachePre.Clear();
            }
            else
            {
                if (afterEverything)
                    EbonianMod.pixelationDrawCachePost.Clear();
                else
                    EbonianMod.pixelationDrawCachePre.Clear();
            }
            sb.End();

            gd.SetRenderTarget(EbonianMod.Instance.renders[1]);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null);
            sb.Draw(EbonianMod.Instance.renders[5], Vector2.Zero, null, Color.White, 0, Vector2.Zero, .5f, SpriteEffects.None, 0);
            sb.End();

            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);


            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White); //Draw Screen
            sb.End();


            sb.Begin(SpriteSortMode.Immediate, additive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            sb.Draw(EbonianMod.Instance.renders[1], Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            sb.End();
        }
        void DrawNPC(Terraria.On_Main.orig_DrawNPC orig, global::Terraria.Main self, int iNPCIndex, bool behindTiles)
        {
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            SmokeDustAkaFireDustButNoGlow.DrawAll(Main.spriteBatch);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            orig(self, iNPCIndex, behindTiles);
        }

        public void DrawBehindTilesAndWalls(Terraria.On_Main.orig_DrawBG orig, global::Terraria.Main self)
        {
            orig(self);
            if (EbonianMod.sys != null)
                EbonianMod.sys.DrawParticles();
        }
        void DrawPlayer(On_VanillaPlayerDrawLayer.orig_Draw orig, PlayerDrawLayer self, ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.GetModPlayer<EbonianPlayer>().sheep || drawInfo.drawPlayer.ownedProjectileCounts[ProjectileType<player_sheep>()] > 0)
            {
                self.Hide();
                drawInfo.hideEntirePlayer = true;
                return;
            }
            orig(self, ref drawInfo);
        }
        void PostDraw(On_Main.orig_DrawCachedProjs orig, Main self, List<int> projCache, bool startSpriteBatch)
        {
            orig(self, projCache, startSpriteBatch);

        }
        public static void DrawVerlets(SpriteBatch sb, GraphicsDevice gd)
        {
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
        }
        void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (EbonianSystem.FlashAlpha > 0)
            {
                Main.spriteBatch.Draw(ExtraTextures.Line, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * EbonianSystem.FlashAlpha * 2);
            }

            if (EbonianSystem.DarkAlpha > 0)
            {
                Main.spriteBatch.Draw(ExtraTextures.Line, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * EbonianSystem.DarkAlpha);
            }
            Main.spriteBatch.End();
        }

        void PreDraw(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            if (S_VerletSystem.verlets.Any())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                DrawVerlets(sb, gd);
                Main.spriteBatch.End();
            }

            var old = gd.GetRenderTargets();
            if (!Main.gameMenu && !(Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy && Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro) && gd.GetRenderTargets().Contains(Main.screenTarget))
            {
                gd.SetRenderTarget(Main.screenTargetSwap);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();

                DrawRei(false, sb, gd);

                DrawXareusGoop(false, sb, gd);

                DrawGarbageFlame(false, sb, gd);

                DrawXareusSpawn(false, sb, gd);

                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                sb.End();

                DrawRei(true, sb, gd);

                DrawXareusGoop(true, sb, gd);

                //DrawRedGoop(true, sb, gd);
                for (int i = 0; i < 3; i++)
                    EbonianMod.pixelationDrawCachePost.Add(() => DrawGarbageFlame(true, sb, gd));

                DrawXareusSpawn(true, sb, gd);

                gd.Textures[1] = null;
                gd.Textures[2] = null;
                gd.Textures[3] = null;
                gd.Textures[4] = null;
                sb.End();
            }

            sb.Begin(SpriteSortMode.Deferred, MiscDrawingMethods.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            ReiSmoke.DrawAll(sb);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, MiscDrawingMethods.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.active && proj.timeLeft > 1 && proj.type == ProjectileType<ReiCapeP>())
                {
                    Color color = Color.Transparent;
                    proj.ModProjectile.PostDraw(color);
                }
            }
            sb.End();

            if (EbonianMod.pixelationDrawCachePre.Any())
                DrawPixelatedContent(false, false, sb, gd);
            if (EbonianMod.addPixelationDrawCachePre.Any())
                DrawPixelatedContent(false, true, sb, gd);
            orig(self);

            if (!Main.gameMenu && !(Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy && Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro) && gd.GetRenderTargets().Contains(Main.screenTarget))
            {
                if (EbonianMod.blurDrawCache.Any())
                    DrawBlurredContent(sb, gd);

                if (EbonianMod.affectedByInvisibleMaskCache.Any() && EbonianMod.invisibleMaskCache.Any())
                    DrawInvisMasks(sb, gd);
            }
            if (EbonianMod.pixelationDrawCachePost.Any())
                DrawPixelatedContent(true, false, sb, gd);
            if (EbonianMod.addPixelationDrawCachePost.Any())
                DrawPixelatedContent(true, true, sb, gd);

            DrawAdditiveDusts(sb, gd);

            DrawGenericPostScreen(sb, gd);
        }
        public static void DrawRei(bool secondPart, SpriteBatch sb, GraphicsDevice gd)
        {

            if (secondPart)
            {
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                gd.Textures[1] = ExtraTextures.space;
                EbonianMod.RTOutline.CurrentTechnique.Passes[0].Apply();
                EbonianMod.RTOutline.Parameters["m"].SetValue(0.62f);
                EbonianMod.RTOutline.Parameters["n"].SetValue(0.01f);
                EbonianMod.RTOutline.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.005f, 0));

                sb.Draw(EbonianMod.Instance.renders[0], Vector2.Zero, Color.White);
            }
            else
            {
                gd.SetRenderTarget(EbonianMod.Instance.renders[0]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.timeLeft > 0 && proj.type == ProjectileType<ReiCapeP>())
                    {
                        Color color = Color.White;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();
            }
        }
        public static void DrawXareusGoop(bool secondPart, SpriteBatch sb, GraphicsDevice gd)
        {
            if (secondPart)
            {

                gd.Textures[1] = ExtraTextures.darkShadowflameGradient;
                gd.Textures[2] = ExtraTextures.space_full;
                gd.Textures[3] = ExtraTextures.seamlessNoiseHighContrast;
                gd.Textures[4] = ExtraTextures.alphaGradient;
                EbonianMod.metaballGradientNoiseTex.CurrentTechnique.Passes[0].Apply();
                EbonianMod.metaballGradientNoiseTex.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
                EbonianMod.metaballGradientNoiseTex.Parameters["offsetX"].SetValue(1f);
                EbonianMod.metaballGradientNoiseTex.Parameters["offsetY"].SetValue(1f);
                sb.Draw(EbonianMod.Instance.renders[2], Vector2.Zero, Color.White);
            }
            else
            {
                gd.SetRenderTarget(EbonianMod.Instance.renders[2]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                XGoopDust.DrawAll(sb);
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.timeLeft > 0 && (proj.type == ProjectileType<ArchmageChargeUp>() || proj.type == ProjectileType<PhantasmalGreatswordP>() || proj.type == ProjectileType<PhantasmalWave>() || proj.type == ProjectileType<PhantasmalGreatswordP2>()))
                    {
                        Color color = Color.Transparent;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();
            }
        }
        public static void DrawGarbageFlame(bool secondPart, SpriteBatch sb, GraphicsDevice gd)
        {

            if (secondPart)
            {
                gd.Textures[1] = ExtraTextures.coherentNoise;
                EbonianMod.displacementMap.CurrentTechnique.Passes[0].Apply();
                EbonianMod.displacementMap.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
                EbonianMod.displacementMap.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.25f);
                EbonianMod.displacementMap.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
                EbonianMod.displacementMap.Parameters["offset"].SetValue(0.0075f);
                EbonianMod.displacementMap.Parameters["alpha"].SetValue(0.1f);
                sb.Draw(EbonianMod.Instance.renders[3], Vector2.Zero, Color.White * 0.25f);
                gd.Textures[1] = ExtraTextures.swirlyNoise;
                EbonianMod.displacementMap.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.34f);
                sb.Draw(EbonianMod.Instance.renders[3], Vector2.Zero, Color.White * 0.25f);

                gd.Textures[1] = ExtraTextures.coherentNoise;
                EbonianMod.displacementMap.Parameters["offsetY"].SetValue(0);
                EbonianMod.displacementMap.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
                EbonianMod.displacementMap.Parameters["offset"].SetValue(0.0075f);
                EbonianMod.displacementMap.Parameters["alpha"].SetValue(0.1f);
                sb.Draw(EbonianMod.Instance.renders[3], Vector2.Zero, Color.White * 0.25f);
                gd.Textures[1] = ExtraTextures.swirlyNoise;
                EbonianMod.displacementMap.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.74f);
                sb.Draw(EbonianMod.Instance.renders[3], Vector2.Zero, Color.White * 0.25f);

            }
            else
            {
                gd.SetRenderTarget(EbonianMod.Instance.renders[3]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.timeLeft > 0 && (proj.type == ProjectileType<GarbageFlame>() || proj.type == ProjectileType<GarbageGiantFlame>() || proj.type == ProjectileType<GarbageLaserSmall3>() || proj.type == ProjectileType<GarbageLaserSmall2>() || proj.type == ProjectileType<GarbageLaserSmall1>()))
                    {
                        Color color = Color.Transparent;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();
            }
        }
        public static void DrawXareusSpawn(bool secondPart, SpriteBatch sb, GraphicsDevice gd)
        {

            if (secondPart)
            {
                gd.Textures[1] = ExtraTextures.shadowflameGradient;
                gd.Textures[2] = ExtraTextures.space_full;
                gd.Textures[3] = ExtraTextures.swirlyNoise;
                gd.Textures[4] = ExtraTextures.alphaGradient;
                EbonianMod.metaballGradientNoiseTex.CurrentTechnique.Passes[0].Apply();
                EbonianMod.metaballGradientNoiseTex.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.1f);
                EbonianMod.metaballGradientNoiseTex.Parameters["offsetX"].SetValue(1f);
                EbonianMod.metaballGradientNoiseTex.Parameters["offsetY"].SetValue(1f);
                sb.Draw(EbonianMod.Instance.renders[4], Vector2.Zero, Color.White);
            }
            else
            {
                gd.SetRenderTarget(EbonianMod.Instance.renders[4]);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.active && proj.timeLeft > 0 && proj.type == ProjectileType<ArchmageXSpawnAnim>())
                    {
                        Color color = Color.Transparent;
                        proj.ModProjectile.PreDraw(ref color);
                    }
                }
                sb.End();
            }
        }
    }
}
