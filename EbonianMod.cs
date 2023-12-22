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
using EbonianMod.Skies;
using System.Collections.Generic;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.VFXProjectiles;
using ReLogic.Graphics;
using EbonianMod.Misc;
using EbonianMod.Projectiles.Exol;
////using EbonianMod.Worldgen.Subworlds;
////using SubworldLibrary;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using EbonianMod.NPCs.Garbage;
using Humanizer;
using System;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Linq;

namespace EbonianMod
{
    public class EbonianMod : Mod
    {
        public static EbonianMod Instance;
        public static Effect Tentacle, TentacleBlack, TentacleRT, ScreenDistort, SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha, Crack, Blur, RTOutline;
        public List<Effect> Effects = new List<Effect>()
        {
            Tentacle, TentacleBlack, TentacleRT, ScreenDistort, SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1, Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha, Crack, Blur, RTOutline
    };
        public RenderTarget2D render, render2, blurrender;
        public static DynamicSpriteFont lcd;
        public static BGParticleSys sys;
        public static SoundStyle flesh0, flesh1, flesh2;
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
        }
        public override void Load()
        {

            flesh0 = new SoundStyle("EbonianMod/Sounds/flesh0");
            flesh1 = new SoundStyle("EbonianMod/Sounds/flesh1");
            flesh2 = new SoundStyle("EbonianMod/Sounds/flesh2");

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
            Filters.Scene["EbonianMod:CorruptTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.68f, .56f, .73f).UseOpacity(0.35f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CorruptTint"] = new BasicTint();
            Filters.Scene["EbonianMod:CrimsonTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.75f, 0f, 0f).UseOpacity(0.45f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:CrimsonTint"] = new BasicTint();
            Filters.Scene["EbonianMod:HellTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.2f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint"] = new BasicTint();
            Filters.Scene["EbonianMod:HellTint2"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.425f), EffectPriority.Medium);
            SkyManager.Instance["EbonianMod:HellTint2"] = new BasicTint();
            Filters.Scene["EbonianMod:ScreenFlash"] = new Filter(new ScreenShaderData(new Ref<Effect>(ModContent.Request<Effect>("EbonianMod/Effects/ScreenFlash", (AssetRequestMode)1).Value), "Flash"), EffectPriority.VeryHigh);
            //Terraria.Graphics.Effects.On_FilterManager.EndCapture += FilterManager_EndCapture;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            Terraria.On_Main.DrawBG += DrawBehindTilesAndWalls;
            Terraria.On_Main.DrawNPC += DrawNPC;
            Terraria.On_Player.Update_NPCCollision += SolidTopCollision;
            On_Main.DrawPlayers_AfterProjectiles += PreDraw;
            CreateRender();
        }
        void PreDraw(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            GraphicsDevice gd = Main.instance.GraphicsDevice;
            SpriteBatch sb = Main.spriteBatch;

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

                gd.SetRenderTarget(render2);
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

                gd.SetRenderTarget(render);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                BlackWhiteDust.DrawAll(sb);
                sb.End();


                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                sb.End();
                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/space", (AssetRequestMode)1).Value;
                RTOutline.CurrentTechnique.Passes[0].Apply();
                RTOutline.Parameters["m"].SetValue(0.62f); // for more percise textures use 0.62f
                RTOutline.Parameters["n"].SetValue(0.01f); // and 0.01f here.
                RTOutline.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.005f, 0));
                sb.Draw(render2, Vector2.Zero, Color.White);

                gd.Textures[1] = ModContent.Request<Texture2D>("EbonianMod/Extras/black", (AssetRequestMode)1).Value;
                RTOutline.CurrentTechnique.Passes[0].Apply();
                RTOutline.Parameters["m"].SetValue(0.22f); // for more percise textures use 0.62f
                RTOutline.Parameters["n"].SetValue(0.1f); // and 0.01f here.
                sb.Draw(render, Vector2.Zero, Color.White);
                gd.Textures[1] = null;
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
            }
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            FireDust.DrawAll(sb);
            ColoredFireDust.DrawAll(sb);
            GenericAdditiveDust.DrawAll(sb);
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
                if (projectile.active && (projectile.type == ModContent.ProjectileType<TExplosion>()))
                {
                    Color color = Color.White;
                    projectile.ModProjectile.PreDraw(ref color);
                }
            }
            sb.End();
        }
        private void Main_OnResolutionChanged(Vector2 obj)
        {
            CreateRender();
        }
        public void CreateRender()
        {
            if (Main.netMode != NetmodeID.Server)
                Main.QueueMainThreadAction(() =>
                {
                    if (render != null && !render.IsDisposed)
                        render.Dispose();
                    if (render2 != null && !render2.IsDisposed)
                        render2.Dispose();
                    if (blurrender != null && !blurrender.IsDisposed)
                        blurrender.Dispose();
                    render = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    render2 = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
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
