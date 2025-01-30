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
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.Tiles;
using Terraria.Graphics.CameraModifiers;
using EbonianMod.Common.CameraModifiers;

namespace EbonianMod
{
    public class EbonianSystem : ModSystem
    {
        public static float savedMusicVol, setMusicBackTimer, setMusicBackTimerMax;
        public int timesDiedToXareus;
        public static void TemporarilySetMusicTo0(float time)
        {
            savedMusicVol = Main.musicVolume;
            setMusicBackTimer = time;
            setMusicBackTimerMax = time;
        }
        public override void PostWorldGen()
        {
            downedXareus = false;
            gotTheStaff = false;
            xareusFuckingDies = false;
            timesDiedToXareus = 0;
        }
        public static bool heardXareusIntroMonologue;
        public static int xareusFightCooldown;
        public override void Load()
        {
            heardXareusIntroMonologue = false;
            xareusFightCooldown = 0;
        }
        public override void UpdateUI(GameTime gameTime)
        {
            if (--setMusicBackTimer < 0)
            {
                savedMusicVol = Main.musicVolume;
            }
            else
                Main.musicVolume = Lerp(savedMusicVol, 0, setMusicBackTimer / setMusicBackTimerMax);
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
        public bool downedXareus = false, gotTheStaff = false, xareusFuckingDies = false;
        public int constantTimer;
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("XarusDown", downedXareus);
            tag.Set("XarusDownForReal", gotTheStaff);
            tag.Set("XarusDownForRealReal", xareusFuckingDies);
            tag.Set("XareusDieTimes", timesDiedToXareus);
        }
        public override void LoadWorldData(TagCompound tag)
        {
            downedXareus = tag.GetBool("XarusDown");
            gotTheStaff = tag.GetBool("XarusDownForReal");
            xareusFuckingDies = tag.GetBool("XarusDownForRealReal");
            timesDiedToXareus = tag.GetInt("XareusDieTimes");
        }
        public override void PostUpdateEverything()
        {
            xareusFightCooldown--;
            constantTimer++;

            if (constantTimer % 1000 == 0)
                if (!NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()))
                {
                    for (int i = Main.maxTilesX / 2 - 440; i < Main.maxTilesX / 2 + 440; i++)
                        for (int j = 135; j < Main.maxTilesY / 2; j++)
                        {
                            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == (ushort)TileType<ArchmageStaffTile>())
                            {
                                NPC.NewNPCDirect(null, new Vector2(i * 16 + 20, j * 16 + 40), NPCType<ArchmageStaffNPC>(), ai3: 1);
                                break;
                            }
                        }
                }
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
                //Helper.DrawBossTitle();

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
        public static int zoomChangeLength, zoomChangeLengthMax;
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
            ChangeCameraPos(pos, length, new ZoomInfo(zoom, 0.05f), lerpMult, easingFunction, snappingRate);

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
                    zoomChangeLength--;
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
