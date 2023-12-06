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
        EbonianAchievementUIState achievementUIState;
        public override void Load()
        {
            achievementUIState = new();
            achievementUI = new();
            achievementUI.SetState(achievementUIState);
        }
        public override void UpdateUI(GameTime gameTime)
        {
            achievementUI?.Update(gameTime);
            if (Main.mouseRight)
                IngameFancyUI.OpenUIState(achievementUIState);
        }
        public struct AchievementTemplate
        {
            public string Text;
            public string Subtext;
            public AchievementTemplate(string text, string subtext)
            {
                Text = text; Subtext = subtext;
            }
        }
        public static AchievementTemplate[] achievementTemplates = new AchievementTemplate[maxAchievements]
        {
            new("Nuclear Waste", "Defeat Hot Garbage."),
            new("Who's laughing now?", "Defeat the vile conglomerate of The Corruption, Terrortoma."),
            new("Seeing Red", "Defeat the unsightly aberration of The Crimson, Cecitior"),
            new("Infernal Archeology", "Unleash Exol of Ignos' soul through weakening Inferos the Rock (idk lmfao)."),
            new("Soul-Crushing", "Defeat Exol of Ignos"),
            new("Too Spicy", "Anything is edible, the aftermath does not matter."),
            new("Quit hitting yourself!", "Terrortoma was never the brightest corpse in the pile..."),
            new("Djungelskog", "Acquire Djungelskog."),
            new("Voidrunner", "Defeat 10 enemies at once using one time slip."),
            new("Organ Harvest", "Acquire a sentient organ."),
            new("Lore Accurate", "Defeat Inferos using the blade of Exol."),
        };
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
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        public const int maxAchievements = 11;
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
