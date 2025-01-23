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

namespace EbonianMod
{
    public class EbonianSystem : ModSystem
    {
        public static float savedMusicVol, setMusicBackTimer, setMusicBackTimerMax;
        public static void TemporarilySetMusicTo0(float time)
        {
            setMusicBackTimer = time;
            setMusicBackTimerMax = time;
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
        public bool downedXareus = false;
        public int constantTimer;
        public override void SaveWorldData(TagCompound tag)
        {
            tag.Set("XarusDown", downedXareus);
        }
        public override void LoadWorldData(TagCompound tag)
        {
            downedXareus = tag.GetBool("XarusDown");
        }
        public override void PostUpdateEverything()
        {
            xareusFightCooldown--;
            constantTimer++;

            if (constantTimer % 600 == 0)
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
                zoomAmount = 0;
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
            else
            {
                zoomChangeLength = 0;
                zoomChangeTransition = 0;
            }
            if (isChangingCameraPos)
            {
                if (CameraChangeLength > 0)
                {
                    if (zoomAmount != 1)
                    {
                        Main.GameZoomTarget = MathHelper.SmoothStep(Main.GameZoomTarget, zoomAmount, CameraChangeTransition);
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
                    Main.screenPosition = Vector2.SmoothStep((stickZoomLerpVal > 0 ? cameraChangeStartPoint : player.Center - Main.ScreenSize.ToVector2() / 2), CameraChangePos, CameraChangeTransition -= 0.05f);
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

            if (NPC.AnyNPCs(NPCType<ArchmageStaffNPC>()) && !isChangingCameraPos)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type == NPCType<ArchmageStaffNPC>())
                    {
                        if (npc.Center.Distance(player.Center) < 800 && !NPC.AnyNPCs(NPCType<ArchmageX>()) && stickZoomLerpVal > 0)
                        {
                            Main.screenPosition = Vector2.SmoothStep(player.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), npc.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), stickZoomLerpVal) + new Vector2(ScreenShakeAmount * Main.rand.NextFloat(), ScreenShakeAmount * Main.rand.NextFloat());
                        }
                        break;
                    }
                }
            }

            if (NPC.AnyNPCs(NPCType<ArchmageX>()) || EbonianSystem.xareusFightCooldown > 0)
            {
                stickZoomLerpVal = MathHelper.Lerp(stickZoomLerpVal, 0, 0.1f);
                if (stickZoomLerpVal < 0.01f)
                    stickZoomLerpVal = 0;
            }
        }
        public static float stickZoomLerpVal;
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
