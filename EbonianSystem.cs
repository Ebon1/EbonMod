using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Graphics;
using System;
using Terraria.UI.Chat;
using Terraria.GameContent.Shaders;
using Terraria.GameContent.UI;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.Initializers;
using Terraria.GameContent.Skies;
using Terraria.GameContent.ItemDropRules;
using Terraria.IO;
using static Terraria.ModLoader.ModContent;

namespace EbonianMod
{
    public class EbonianSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime)
        {
            if (Main.WaveQuality == 0)
            {
                Main.NewText("Ebonian Mod doesn't currently work properly when the Wave Quality is set to Off.", Main.errorColor);
                Main.WaveQuality = 1;
            }
            if (Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy || Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro)
            {
                Main.NewText("Ebonian Mod doesn't currently work properly with Trippy or Retro lights.", Main.errorColor);
                Lighting.Mode = Terraria.Graphics.Light.LightMode.Color;
            }
        }
        public override void PostUpdateEverything()
        {
            xareusFightCooldown--;
        }
        public override void OnWorldLoad()
        {
            xareusFightCooldown = 0;
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int textIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            int textIndex2 = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Map / Minimap")) + 1;
            layers.Insert(textIndex, new LegacyGameInterfaceLayer("EbonianMod: BossText", () =>
            {
                Helper.DrawBossTitle();

                return true;
            }, InterfaceScaleType.UI));
            layers.Insert(textIndex2, new LegacyGameInterfaceLayer("EbonianMod: BossText", () =>
            {
                Helper.DrawDialogue();

                return true;
            }, InterfaceScaleType.UI));
        }
        public static int ShakeTimer = 0;
        public static float ScreenShakeAmount = 0;

        public override void ModifyScreenPosition()
        {
            Player player = Main.LocalPlayer;
            if (EbonianMod.FlashAlpha > 0)
                EbonianMod.FlashAlpha -= EbonianMod.FlashAlphaDecrement;
            else
                EbonianMod.FlashAlphaDecrement = 0.01f;
            if (!isChangingCameraPos && !isChangingZoom)
            {
                zoomBefore = Main.GameZoomTarget;
            }
            if (isChangingZoom)
            {
                if (--zoomChangeLength > 0)
                {
                    if (zoomAmount != 1 && zoomAmount > zoomBefore)
                    {
                        Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + 0.05f, 1f, zoomAmount);
                    }
                    if (zoomChangeTransition <= 1f)
                    {
                        zoomChangeTransition += 0.025f;
                    }
                }
                else if (zoomChangeTransition >= 0)
                {
                    if (Main.GameZoomTarget > zoomBefore)
                    {
                        Main.GameZoomTarget -= 0.05f;
                    }
                    zoomChangeTransition -= 0.025f;
                }
                else isChangingZoom = false;
            }
            if (isChangingCameraPos)
            {
                if (CameraChangeLength > 0)
                {
                    if (zoomAmount != 1 && zoomAmount > zoomBefore)
                    {
                        Main.GameZoomTarget = Utils.Clamp(Main.GameZoomTarget + 0.05f, 1f, zoomAmount);
                    }
                    if (CameraChangeTransition <= 1f)
                    {
                        Main.screenPosition = Vector2.SmoothStep(cameraChangeStartPoint, CameraChangePos, CameraChangeTransition += 0.025f);
                    }
                    else
                    {
                        Main.screenPosition = CameraChangePos;
                    }
                    CameraChangeLength--;
                }
                else if (CameraChangeTransition >= 0)
                {
                    if (Main.GameZoomTarget > zoomBefore)
                    {
                        Main.GameZoomTarget -= 0.05f;
                    }
                    Main.screenPosition = Vector2.SmoothStep(player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), CameraChangePos, CameraChangeTransition -= 0.05f);
                }
                else
                    isChangingCameraPos = false;
            }
            else
            {
                CameraChangeLength = 0;
                cameraChangeStartPoint = Vector2.Zero;
                CameraChangeTransition = 0;
                CameraChangePos = Vector2.Zero;
            }
            if (!Main.gameMenu)
            {
                ShakeTimer++;
                if (ScreenShakeAmount >= 0 && ShakeTimer >= 5)
                    ScreenShakeAmount -= 0.1f;
                if (ScreenShakeAmount < 0)
                    ScreenShakeAmount = 0;
                Main.screenPosition += new Vector2(ScreenShakeAmount * Main.rand.NextFloat(), ScreenShakeAmount * Main.rand.NextFloat());
            }
            else
            {
                ScreenShakeAmount = 0;
                ShakeTimer = 0;
            }
        }
        float zoomBefore;
        public static float zoomAmount;
        public static Vector2 cameraChangeStartPoint;
        public static Vector2 CameraChangePos;
        public static float CameraChangeTransition, zoomChangeTransition;
        public static int CameraChangeLength, zoomChangeLength;
        public static bool isChangingCameraPos, isChangingZoom;
        public static void ChangeZoom(float zoom, int len)
        {
            zoomAmount = zoom;
            zoomChangeLength = len;
            isChangingZoom = true;
            zoomChangeTransition = 0;
        }
        public static void ChangeCameraPos(Vector2 pos, int length, float zoom = 1.65f)
        {
            cameraChangeStartPoint = Main.screenPosition;
            CameraChangeLength = length;
            CameraChangePos = pos - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            isChangingCameraPos = true;
            CameraChangeTransition = 0;
            if (Main.GameZoomTarget < zoom)
                zoomAmount = zoom;
        }
        public static bool heardXareusIntroMonologue;
        public static int xareusFightCooldown;
        public override void Load()
        {
            heardXareusIntroMonologue = false;
            xareusFightCooldown = 0;
        }
    }
}
