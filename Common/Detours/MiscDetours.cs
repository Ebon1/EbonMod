using EbonianMod.NPCs.ArchmageX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace EbonianMod.Common.Detours
{
    public class MiscDetours : ModSystem
    {
        public override void Load()
        {
            Main.OnResolutionChanged += (Vector2 obj) => CreateRender();
            On_NPC.SetEventFlagCleared += EventClear;
            On_Main.Update += UpdateDeltaTime;
            CreateRender();
        }
        void UpdateDeltaTime(On_Main.orig_Update orig, Main self, GameTime gameTime)
        {
            float oldFrameRate = Main.frameRate;
            orig(self, gameTime);

            if (Main.FrameSkipMode == Terraria.Enums.FrameSkipMode.On) EbonianSystem.deltaTime = 1;
            else
            {
                float averageFrameRate = (Main.frameRate + oldFrameRate) / 2f;
                EbonianSystem.deltaTime = Clamp((float)(gameTime.TotalGameTime.TotalSeconds - gameTime.ElapsedGameTime.TotalSeconds) / (averageFrameRate), 0.2f, 1.1f);
            }
        }
        public void CreateRender()
        {
            if (Main.netMode != NetmodeID.Server)
                Main.QueueMainThreadAction(() =>
                {
                    for (int i = 0; i < EbonianMod.Instance.renders.Length; i++)
                    {
                        if (EbonianMod.Instance.renders[i] != null && !EbonianMod.Instance.renders[i].IsDisposed)
                            EbonianMod.Instance.renders[i].Dispose();
                        EbonianMod.Instance.renders[i] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    }
                    if (EbonianMod.Instance.invisRender != null && !EbonianMod.Instance.invisRender.IsDisposed)
                        EbonianMod.Instance.invisRender.Dispose();
                    if (EbonianMod.Instance.affectedByInvisRender != null && !EbonianMod.Instance.affectedByInvisRender.IsDisposed)
                        EbonianMod.Instance.affectedByInvisRender.Dispose();
                    if (EbonianMod.Instance.blurrender != null && !EbonianMod.Instance.blurrender.IsDisposed)
                        EbonianMod.Instance.blurrender.Dispose();

                    EbonianMod.Instance.invisRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    EbonianMod.Instance.affectedByInvisRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                    EbonianMod.Instance.blurrender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
        }
        void EventClear(On_NPC.orig_SetEventFlagCleared orig, ref bool eventFlag, int gameEventId)
        {
            if (gameEventId == 3 && !GetInstance<EbonianSystem>().xareusFuckingDies && GetInstance<EbonianSystem>().downedXareus)
            {
                NPC.NewNPCDirect(null, Main.player[0].Center, NPCType<ArchmageCutsceneMartian>(), 0, -1);
                GetInstance<EbonianSystem>().xareusFuckingDies = true;
            }
            orig(ref eventFlag, gameEventId);
        }
    }
}
