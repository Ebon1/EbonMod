using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;

namespace ExolRebirth
{
    public class EbonianPlayer : ModPlayer
    {
        public int bossTextProgress, bossMaxProgress, dialogueMax, dialogueProg;
        public string bossName;
        public string bossTitle;
        public string dialogue;
        public Color bossColor, dialogueColor;
        public static EbonianPlayer Instance;
        public override void OnEnterWorld(Player player)
        {
            Instance = Player.GetModPlayer<EbonianPlayer>();
        }
        public override void PostUpdate()
        {
            if (bossTextProgress > 0)
                bossTextProgress--;
            if (bossTextProgress == 0)
            {
                bossName = null;
                bossTitle = null;
                bossMaxProgress = 0;
                bossColor = Color.White;
            }
            if (dialogueProg > 0)
                dialogueProg--;
            if (dialogueProg == 0)
            {
                dialogue = null;
                dialogueMax = 0;
                dialogueColor = Color.White;
            }
        }
    }
}
