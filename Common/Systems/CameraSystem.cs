using EbonianMod.Common.CameraModifiers;
using EbonianMod.NPCs.ArchmageX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;

namespace EbonianMod.Common.Systems
{
    public class CameraSystem : ModSystem
    {
        public static int ShakeTimer = 0;
        public static float ScreenShakeAmount = 0;
        public override void ModifyScreenPosition()
        {
            Player player = Main.LocalPlayer;
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

            if (NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()))
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.type == NPCType<ArchmageStaffNPC>())
                    {
                        stickPos = npc.Center;
                        if (npc.Center.Distance(player.Center) < 800 && stickZoomLerpVal > 0)
                        {
                            Main.screenPosition = Vector2.SmoothStep(player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), npc.Center - new Vector2(Main.screenWidth / 2 + stickZoomXOffset, Main.screenHeight / 2), stickZoomLerpVal) + new Vector2(ScreenShakeAmount * Main.rand.NextFloat(), ScreenShakeAmount * Main.rand.NextFloat());
                        }
                        break;
                    }
                }
            }

            if (!NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()) || NPC.AnyNPCs(NPCType<ArchmageX>()) || EbonianSystem.xareusFightCooldown > 0)
            {
                stickZoomLerpVal = MathHelper.Lerp(stickZoomLerpVal, 0, 0.05f);
                if (stickZoomLerpVal < 0.01f)
                    stickZoomLerpVal = 0;

                stickZoomXOffset = Lerp(stickZoomXOffset, 0, 0.1f);

                if (stickZoomXOffset < 0.01f)
                    stickZoomXOffset = 0;

                stickLerpOffset = Lerp(stickLerpOffset, 0, 0.1f);

                if (stickLerpOffset < 0.01f)
                    stickLerpOffset = 0;
            }
        }
        public static float stickZoomLerpVal, stickZoomXOffset, stickLerpOffset;
        float zoomBefore;
        public static float zoomAmount, zoomLerpMult;
        public static Func<float, float> zoomFunctionIn, zoomFunctionOut;
        public static bool useCurrentZoomIn, useCurrentZoomOut;
        public static Vector2 currentZoomForChangedZoom, currentZoomForChangedZoom2;
        public static Vector2 cameraChangePos, stickPos;
        public static int cameraChangeLength;
        public static float zoomChangeLength, zoomChangeLengthMax;
        public static void ChangeZoom(int length, ZoomInfo zoom)
        {
            zoomAmount = zoom.zoomAmount;
            zoomLerpMult = zoom.zoomLerpMult;
            zoomFunctionIn = zoom.zoomEaseIn;
            zoomFunctionOut = zoom.zoomEaseOut;
            useCurrentZoomIn = zoom.isCurrentZoomIn;
            useCurrentZoomOut = zoom.isCurrentZoomOut;
            zoomChangeLength = length;
            zoomChangeLengthMax = length;
            currentZoomForChangedZoom2 = Main.GameViewMatrix.Zoom;
        }
        public static void ChangeCameraPos(Vector2 pos, int length, ZoomInfo? zoom = null, float lerpMult = 2, Func<float, float> easingFunction = null, float snappingRate = 1)
        {
            if (zoom != null)
            {
                ChangeZoom(length, zoom.Value);
            }
            Main.instance.CameraModifiers.Add(new FocusCameraModifier(pos, length, lerpMult, easingFunction, snappingRate));
        }

        public static void ChangeCameraPos(Vector2 pos, int length, float zoom,
            float lerpMult = 2, Func<float, float> easingFunction = null, float snappingRate = 1) =>
            ChangeCameraPos(pos, length, new ZoomInfo(zoom, 2), lerpMult, easingFunction, snappingRate);

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (zoomChangeLength > 0 && zoomAmount > 0)
            {
                float lerpT = Clamp(MathF.Sin(Pi * Utils.GetLerpValue(zoomChangeLengthMax, 0, zoomChangeLength)) * zoomLerpMult, 0, 1);
                if (zoomFunctionIn != null && zoomChangeLength > zoomChangeLengthMax / 2)
                    lerpT = zoomFunctionIn.Invoke(Clamp(MathF.Sin(Pi * Utils.GetLerpValue(zoomChangeLengthMax, 0, zoomChangeLength)) * zoomLerpMult, 0, 1));

                if (zoomFunctionOut != null && zoomChangeLength <= zoomChangeLengthMax / 2)
                    lerpT = zoomFunctionOut.Invoke(Clamp(MathF.Sin(Pi * Utils.GetLerpValue(zoomChangeLengthMax, 0, zoomChangeLength)) * zoomLerpMult, 0, 1));

                Vector2 pos = currentZoomForChangedZoom;
                if (useCurrentZoomIn && zoomChangeLength > zoomChangeLengthMax / 2)
                {
                    pos = currentZoomForChangedZoom2;
                }
                if (useCurrentZoomOut && zoomChangeLength <= zoomChangeLengthMax / 2)
                {
                    pos = currentZoomForChangedZoom2;
                }
                Transform.Zoom = Vector2.Lerp(pos, new Vector2(zoomAmount), lerpT);
                if (!Main.gameInactive && !Main.gamePaused)
                    zoomChangeLength -= EbonianSystem.deltaTime;
            }
            else
            {
                zoomBefore = Main.GameZoomTarget;
                zoomAmount = 0;
                zoomChangeLength = 0;
                zoomChangeLengthMax = 0;
                currentZoomForChangedZoom = Transform.Zoom;
                zoomFunctionIn = null;
                zoomFunctionOut = null;
            }
        }
    }
    public struct ZoomInfo
    {
        public float zoomAmount;
        public float zoomLerpMult;
        public Func<float, float> zoomEaseIn, zoomEaseOut;
        public bool isCurrentZoomIn, isCurrentZoomOut;
        public ZoomInfo(float _zoomAmount, float lerpMult = 2, Func<float, float> easingFunctionIn = null, Func<float, float> easingFunctionOut = null, bool _isCurrentZoomIn = false, bool _isCurrentZoomOut = false)
        {
            zoomAmount = _zoomAmount;
            zoomEaseIn = easingFunctionIn;
            zoomEaseOut = easingFunctionOut;
            zoomLerpMult = lerpMult;
            isCurrentZoomIn = _isCurrentZoomIn;
            isCurrentZoomOut = _isCurrentZoomOut;
        }
    }
}
