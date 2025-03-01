using Terraria.ModLoader.IO;
using EbonianMod.NPCs.ArchmageX;
using EbonianMod.Tiles;
using EbonianMod.Items.Weapons.Magic;

namespace EbonianMod
{
    public class EbonianSystem : ModSystem
    {
        public static float savedMusicVol, setMusicBackTimer, setMusicBackTimerMax;
        public int timesDiedToXareus;

        public bool downedXareus = false, gotTheStaff = false, xareusFuckingDies = false;
        public int constantTimer;

        public static float conglomerateSkyFlash;
        public Color conglomerateSkyColorOverride;

        public static bool heardXareusIntroMonologue;
        public static int xareusFightCooldown;
        public static float deltaTime;
        public override void Load()
        {
            heardXareusIntroMonologue = false;
            xareusFightCooldown = 0;
        }
        public static float FlashAlpha, DarkAlpha;
        public override void PostUpdateEverything()
        {
            Main.NewText("Synced Rand: " + SyncedRand.rand.Next(100));
            Main.NewText("Main Rand: " + Main.rand.Next(100));

            if (FlashAlpha > 0)
                FlashAlpha -= 0.01f;

            if (!Main.gameInactive)
                DarkAlpha = Lerp(DarkAlpha, 0, 0.1f);
            if (DarkAlpha < .05f)
                DarkAlpha = 0;
            conglomerateSkyFlash = Lerp(conglomerateSkyFlash, 0, 0.07f);
            conglomerateSkyColorOverride = Color.Lerp(conglomerateSkyColorOverride, Color.White, 0.03f);
            if (conglomerateSkyFlash < 0.05f)
            {
                conglomerateSkyFlash = 0;
                conglomerateSkyColorOverride = Color.White;
            }

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
        public override void PostWorldGen()
        {
            downedXareus = false;
            gotTheStaff = false;
            xareusFuckingDies = false;
            timesDiedToXareus = 0;
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
        public override void OnWorldLoad()
        {
            xareusFightCooldown = 0;

            if (gotTheStaff)
                ItemID.Sets.ShimmerTransformToItem[ItemID.AmethystStaff] = ItemType<StaffOfX>();
            else
                ItemID.Sets.ShimmerTransformToItem[ItemID.AmethystStaff] = 0;
        }
        public static void TemporarilySetMusicTo0(float time)
        {
            savedMusicVol = Main.musicVolume;
            setMusicBackTimer = time;
            setMusicBackTimerMax = time;
        }
    }
}
