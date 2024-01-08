using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace EbonianMod.Achievements
{
    public class EbonianAchievementSystem : ModSystem
    {
        public UserInterface achievementUI;
        public UserInterface achievementButtonUI;
        public static EbonianAchievementUIState achievementUIState;
        public static EbonianAchievementButtonUIState achievementButtonUIState;
        public override void Load()
        {
            achievementUIState = new();
            achievementUI = new();
            achievementButtonUIState = new();
            achievementButtonUI = new();
            achievementUI.SetState(achievementUIState);
            achievementButtonUI.SetState(achievementButtonUIState);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            achievementUI?.Update(gameTime);
            achievementButtonUI?.Update(gameTime);
        }
        public struct AchievementTemplate
        {
            public string Text;
            public string Subtext;
            public bool HiddenUntilUnlocked;
            public AchievementTemplate(string text, string subtext, bool hidden = false)
            {
                Text = text; Subtext = subtext; HiddenUntilUnlocked = hidden;
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "EbonianMod: Achievements",
                    delegate
                    {
                        achievementUI.Draw(Main.spriteBatch, new GameTime());
                        if (Main.playerInventory && Main.LocalPlayer.chest == -1 && Main.npcShop <= 0)
                            achievementButtonUI.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public const int maxAchievements = 12;
        public static AchievementTemplate[] achievementTemplates = new AchievementTemplate[maxAchievements]
        {
            new("Nuclear Waste", "Defeat Hot Garbage, the first and only AI to ever reach true singularity."),
            new("Who's laughing now?", "Defeat Terrortoma, the vile conglomerate of The Corruption."),
            new("Seeing Red", "Defeat Cecitior, the unsightly aberration of The Crimson"),
            new("Infernal Geology", "Defeat Inferos, a beast of curious origin and unusual anatomy."),
            new("Soul-Crushing", "Defeat the spirit of Exol, valiant knight of Ignos.", true),
            new("Too Spicy", "Anything is edible, the aftermath does not matter."),
            new("Quit hitting yourself!", "Terrortoma was never the brightest corpse in the pile..."),
            new("Djungelskog", "Acquire Djungelskog."),
            new("Organ Harvest", "Acquire a sentient organ, found near the edge of the world."),
            new("Lore Accurate", "Defeat Inferos using the blade of Exol.", true),
            new("What, still here?", "Arrive at the buried land of Ignos."),
            new("Dr. Abian", "'Blowing up the moon would solve all of our problems!'\nDefeat the Moon Lord through an... unconvential method."),
        };
        public static bool[] acquiredAchievement = new bool[maxAchievements];
        public override void ClearWorld()
        {
            for (int i = 0; i < maxAchievements; i++)
            {
                acquiredAchievement[i] = false;
            }
        }
        public override void SaveWorldData(TagCompound tag)
        {
            for (int i = 0; i < maxAchievements; i++)
            {
                if (acquiredAchievement[i])
                    tag["EbonianAchievement" + i] = true;
            }
        }
        public override void LoadWorldData(TagCompound tag)
        {
            for (int i = 0; i < maxAchievements; i++)
            {
                acquiredAchievement[i] = tag.ContainsKey("EbonianAchievement" + i);
            }
        }
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags1 = new BitsByte();
            for (int i = 0; i < 8; i++)
            {
                flags1[i] = acquiredAchievement[i];
            }
            writer.Write(flags1);
            BitsByte flags2 = new BitsByte();
            for (int i = 8; i < maxAchievements; i++)
            {
                flags2[i] = acquiredAchievement[i - 8];
            }
            writer.Write(flags2);
        }
        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags1 = reader.ReadByte();
            for (int i = 0; i < 8; i++)
            {
                acquiredAchievement[i] = flags1[i];
            }
            BitsByte flags2 = reader.ReadByte();
            for (int i = 8; i < maxAchievements; i++)
            {
                acquiredAchievement[i] = flags2[i];
            }
        }
    }
}
