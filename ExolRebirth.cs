using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria.ID;
using ExolRebirth.Dusts;
using ExolRebirth.NPCs.Exol;

namespace ExolRebirth
{
    public class ExolRebirth : Mod
    {
        public static ExolRebirth Instance;
        public static Effect BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, TrailShader, RTAlpha;
        public static Effect Tentacle, TentacleBlack, TentacleRT, ScreenDistort, TextGradient, TextGradient2, TextGradientY;
        public RenderTarget2D render, render3, render4;
        public override void Load()
        {
            Instance = this;
            Test1 = ModContent.Request<Effect>("ExolRebirth/Effects/Test1", (AssetRequestMode)1).Value;
            RTAlpha = ModContent.Request<Effect>("ExolRebirth/Effects/RTAlpha", (AssetRequestMode)1).Value;
            CrystalShine = ModContent.Request<Effect>("ExolRebirth/Effects/CrystalShine", (AssetRequestMode)1).Value;
            TextGradient = ModContent.Request<Effect>("ExolRebirth/Effects/TextGradient", (AssetRequestMode)1).Value;
            TextGradient2 = ModContent.Request<Effect>("ExolRebirth/Effects/TextGradient2", (AssetRequestMode)1).Value;
            TextGradientY = ModContent.Request<Effect>("ExolRebirth/Effects/TextGradientY", (AssetRequestMode)1).Value;
            Test2 = ModContent.Request<Effect>("ExolRebirth/Effects/Test2", (AssetRequestMode)1).Value;
            Galaxy = ModContent.Request<Effect>("ExolRebirth/Effects/Galaxy", (AssetRequestMode)1).Value;
            LavaRT = ModContent.Request<Effect>("ExolRebirth/Effects/LavaRT", (AssetRequestMode)1).Value;
            BeamShader = ModContent.Request<Effect>("ExolRebirth/Effects/Beam", (AssetRequestMode)1).Value;
            Lens = ModContent.Request<Effect>("ExolRebirth/Effects/Lens", (AssetRequestMode)1).Value;
            Tentacle = ModContent.Request<Effect>("ExolRebirth/Effects/Tentacle", (AssetRequestMode)1).Value;
            TentacleRT = ModContent.Request<Effect>("ExolRebirth/Effects/TentacleRT", (AssetRequestMode)1).Value;
            ScreenDistort = ModContent.Request<Effect>("ExolRebirth/Effects/DistortMove", (AssetRequestMode)1).Value;
            TentacleBlack = ModContent.Request<Effect>("ExolRebirth/Effects/TentacleBlack", (AssetRequestMode)1).Value;
            TrailShader = ModContent.Request<Effect>("ExolRebirth/Effects/TrailShader", (AssetRequestMode)1).Value;
            On.Terraria.Graphics.Effects.FilterManager.EndCapture += FilterManager_EndCapture;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            VerletSystem.Load();
            CreateRender();
        }
        private void Main_OnResolutionChanged(Vector2 obj)
        {
            CreateRender();
        }
        public void CreateRender()
        {
            Main.QueueMainThreadAction(() =>
            {
                render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                render3 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                render4 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }
        public static int ExolID = ModContent.NPCType<Exol>();
        private void FilterManager_EndCapture(On.Terraria.Graphics.Effects.FilterManager.orig_EndCapture orig, Terraria.Graphics.Effects.FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;
            #region "rt2d"
            /*gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type == DukeRainbowID)
                {
                    sb.Draw(Helper.GetExtraTexture("Line"), new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2), Color.White);
                    npc.ModNPC.PreDraw(sb, Main.screenPosition, default);
                }
            }
            sb.End();

            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            gd.Textures[1] = ModContent.Request<Texture2D>("ExolRebirth/Extras/bg", (AssetRequestMode)1).Value;
            Effect effect = ExolRebirth.RTAlpha;
            effect.Parameters["m"].SetValue(1f);
            effect.Parameters["screenPosition"].SetValue(Main.screenPosition);
            effect.Parameters["noiseTex"].SetValue(ModContent.Request<Texture2D>("ExolRebirth/Extras/seamlessNoise").Value);
            effect.Parameters["distortionMultiplier"].SetValue(1.5f);
            effect.Parameters["screenSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) * -25f);
            effect.Parameters["alpha"].SetValue(0.25f);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(render, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.End();*/
            /*gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.timeLeft > 1 && proj.type == ModContent.ProjectileType<Items.Accessories.GinnungagapP>())
                {
                    Color color = Color.White;
                    proj.ModProjectile.PreDraw(ref color);
                }
            }
            sb.End();

            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            gd.Textures[1] = ModContent.Request<Texture2D>("ExolRebirth/Extras/starSky", (AssetRequestMode)1).Value;
            Effect effect = ExolRebirth.RTAlpha;
            effect.Parameters["m"].SetValue(1f);
            effect.Parameters["screenPosition"].SetValue(Main.screenPosition);
            effect.Parameters["noiseTex"].SetValue(ModContent.Request<Texture2D>("ExolRebirth/Extras/seamlessNoise").Value);
            effect.Parameters["distortionMultiplier"].SetValue(1.5f);
            effect.Parameters["screenSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) * -25f);
            effect.Parameters["alpha"].SetValue(0.5f);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(render, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.End();




            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();

            gd.SetRenderTarget(render);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            GinnungagapDust.DrawAll(sb);
            sb.End();

            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            gd.Textures[1] = ModContent.Request<Texture2D>("ExolRebirth/Extras/starSky", (AssetRequestMode)1).Value;
            effect.Parameters["m"].SetValue(1f);
            effect.Parameters["screenPosition"].SetValue(Main.screenPosition);
            effect.Parameters["noiseTex"].SetValue(ModContent.Request<Texture2D>("ExolRebirth/Extras/seamlessNoise").Value);
            effect.Parameters["distortionMultiplier"].SetValue(1.5f);
            effect.Parameters["screenSize"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight) * -25f);
            effect.Parameters["alpha"].SetValue(0.5f);
            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(render, Vector2.Zero, Color.White);
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.End();

            */
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            FireDust.DrawAll(sb);
            ColoredFireDust.DrawAll(sb);
            sb.End();
            #endregion
            #region "ripple"
            gd.SetRenderTarget(render3);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
            sb.End();
            gd.SetRenderTarget(Main.screenTargetSwap);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.type == ModContent.ProjectileType<Projectiles.Ripple>())
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
            sb.Draw(render3, Vector2.Zero, Color.White);
            sb.End();


            #endregion
            /*

            gd.SetRenderTarget(render4);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            var font = FontAssets.DeathText.Value;
            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, font, "poopshitter", Vector2.Zero, Color.White, 0, Vector2.Zero, 10, SpriteEffects.None, 0f);
            sb.End();
            gd.SetRenderTarget(Main.screenTarget);
            gd.Clear(Color.Transparent);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            ExolRebirth.TextGradient2.CurrentTechnique.Passes[0].Apply();
            ExolRebirth.TextGradient2.Parameters["color2"].SetValue(new Vector4(0.7803921568627451f, 0.0941176470588235f, 1, 1));
            ExolRebirth.TextGradient2.Parameters["color3"].SetValue(new Vector4(0.0509803921568627f, 1, 1, 1));
            ExolRebirth.TextGradient2.Parameters["amount"].SetValue(Main.GlobalTimeWrappedHourly);
            sb.Draw(render4, Vector2.Zero, Color.White);
            sb.End();
            */
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
    }
}
