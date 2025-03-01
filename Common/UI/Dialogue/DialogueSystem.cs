using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;

namespace EbonianMod.Common.UI.Dialogue
{
    public class DialogueSystem : ModSystem
    {
        public static FloatingDialogueBox[] DialogueBox = new FloatingDialogueBox[15];
        public override void Load()
        {
            for (int i = 0; i < DialogueBox.Length; i++)
            {
                DialogueBox[i] = new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
            }
        }
        public override void OnWorldLoad()
        {
            for (int i = 0; i < DialogueBox.Length; i++)
            {
                DialogueBox[i] = new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int textIndex2 = layers.FindIndex(layer => layer.Name.Equals("Vanilla: MP Player Names"));
            layers.Insert(textIndex2, new LegacyGameInterfaceLayer("EbonianMod: Dialogue", () =>
            {

                for (int i = 0; i < DialogueBox.Length; i++)
                {
                    if (DialogueBox[i].timeLeft > 0)
                        DialogueBox[i].Draw(Main.spriteBatch);
                }

                return true;
            }, InterfaceScaleType.Game));
        }
        public static FloatingDialogueBox NewDialogueBox(int timeLeft, Vector2 center, string text, Color textColor, int maxWidth = -1, float scale = 0.5f, Color borderColor = default, float lerpSpeed = 2f, bool substring = true, DialogueAnimationIDs animationType = DialogueAnimationIDs.None, SoundStyle sound = default, int soundInterval = 10)
        {
            if (sound == default)
                sound = EbonianSounds.None;
            int i = 0;
            while (DialogueBox[i].timeLeft > 0 && i < DialogueBox.Length - 1)
            {
                if (DialogueBox[i] == null)
                    DialogueBox[i] = new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
                i++;
            }
            if (i < DialogueBox.Length)
            {
                DialogueBox[i] = new FloatingDialogueBox(timeLeft, center, text, textColor, maxWidth, scale, borderColor, lerpSpeed, substring, animationType, sound, soundInterval);
                return DialogueBox[i];
            }
            return new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
        }
        public override void PostUpdateEverything()
        {
            for (int i = 0; i < DialogueBox.Length; i++)
            {
                if (DialogueBox[i] == null)
                    DialogueBox[i] = new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
                if (DialogueBox[i].timeLeft > 0)
                {
                    DialogueBox[i].Update();
                }
                else if (DialogueBox[i].timeLeft == 0)
                {
                    DialogueBox[i] = new FloatingDialogueBox(-1, Vector2.Zero, "", Color.White);
                }
            }
        }
    }
}
