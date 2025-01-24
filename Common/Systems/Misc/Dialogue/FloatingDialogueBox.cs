using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace EbonianMod.Common.Systems.Misc.Dialogue
{
    [Flags]
    public enum DialogueAnimationIDs
    {
        None = 1, BopDown = 2, BopUp = 4, ScaleUp = 8, ScaleDown = 16, ColorWhite = 32
    }
    public class FloatingDialogueBox
    {
        public int timeLeft, timeLeftMax;
        public Vector2 Center, VisibleCenter;
        public List<Vector2> charVisibleCenter;
        public string text, visibleText, oldVisibleText;
        public float scale, visibleScale;
        public List<float> charVisibleScale;
        public float lerpSpeed;
        public bool substring = true;
        public int maxWidth = -1, soundInterval = 10;
        public Color borderColor = Color.Transparent;
        public Color textColor = Color.White;
        public SoundStyle sound;
        public Color actualBorderColor = Color.Transparent;
        public Color actualTextColor = Color.White;
        public List<Color> charTextColor, charBorderColor;
        public DialogueAnimationIDs animationType;
        //public int animationInterval = 5;
        public FloatingDialogueBox(int timeLeft, Vector2 center, string text, Color textColor, int maxWidth = -1, float scale = 0.5f, Color borderColor = default, float lerpSpeed = 2f, bool substring = true, DialogueAnimationIDs animationType = DialogueAnimationIDs.None, SoundStyle sound = default, int soundInterval = 10)
        {
            this.soundInterval = soundInterval;
            this.sound = sound;
            this.timeLeft = timeLeft;
            this.timeLeftMax = timeLeft;
            Center = center;
            VisibleCenter = center;
            charVisibleCenter = new List<Vector2>(text.Length);
            charVisibleScale = new List<float>(text.Length);
            charTextColor = new List<Color>(text.Length);
            charBorderColor = new List<Color>(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                charVisibleCenter.Add(center);
                charVisibleScale.Add(scale);
                charTextColor.Add(textColor);
                charBorderColor.Add(borderColor);
            }
            this.text = text;
            this.textColor = textColor;
            this.actualTextColor = textColor;
            this.maxWidth = maxWidth;
            this.scale = scale;
            this.visibleScale = scale;
            this.borderColor = borderColor;
            this.actualBorderColor = borderColor;
            this.lerpSpeed = lerpSpeed;
            this.substring = substring;
            this.animationType = animationType;
        }
        public void SetVisibleText()
        {
            oldVisibleText = visibleText;
            int newLineAmount = 0;
            foreach (char ch in text)
            {
                if (ch == '\n' && newLineAmount < timeLeftMax) newLineAmount += 10;
            }
            float progress = Utils.GetLerpValue(0, timeLeftMax - newLineAmount, timeLeft - newLineAmount);
            float lerpV = ((MathHelper.Clamp((1f - progress) * lerpSpeed, 0, 1)));

            int count = (int)(text.Length * lerpV);
            visibleText = $"{text.Substring(0, count)}";
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //draw border later
            if (substring)
            {
                int newLineAmount = 0;
                foreach (char ch in text)
                {
                    if (ch == '\n') newLineAmount++;
                }
                if (newLineAmount == 0)
                {
                    if (visibleText.Length != 0)
                    {
                        for (int i = 0; i < visibleText.Length - 1; i++)
                        {
                            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[i].ToString(), charVisibleCenter[i] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(i)).X * (scale), 0) - Main.screenPosition, charTextColor[i], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[i], charVisibleScale[i]), maxWidth);
                            ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[i].ToString(), charVisibleCenter[i] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(i)).X * (scale), 0) - Main.screenPosition, charBorderColor[i], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[i], charVisibleScale[i]), maxWidth);
                            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, visibleText[i].ToString(), charVisibleCenter[i] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(i)).X * (scale), 0) - Main.screenPosition, charTextColor[i], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[i], charVisibleScale[i]), maxWidth);
                        }
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[visibleText.Length - 1].ToString(), charVisibleCenter[visibleText.Length - 1] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(visibleText.Length - 1)).X * (scale), 0) - Main.screenPosition, charTextColor[visibleText.Length - 1], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[visibleText.Length - 1], charVisibleScale[visibleText.Length - 1]), maxWidth);
                        ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[visibleText.Length - 1].ToString(), charVisibleCenter[visibleText.Length - 1] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(visibleText.Length - 1)).X * (scale), 0) - Main.screenPosition, charBorderColor[visibleText.Length - 1], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[visibleText.Length - 1], charVisibleScale[visibleText.Length - 1]), maxWidth);
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, visibleText[visibleText.Length - 1].ToString(), charVisibleCenter[visibleText.Length - 1] + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(visibleText.Length - 1)).X * (scale), 0) - Main.screenPosition, charTextColor[visibleText.Length - 1], 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / (2), FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale[visibleText.Length - 1], charVisibleScale[visibleText.Length - 1]), maxWidth);
                    }
                }
                else
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth); // add animations to new lines later
                    ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualBorderColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth); // add animations to new lines later
                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth); // add animations to new lines later
                }
            }
            else
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth);
                ChatManager.DrawColorCodedStringShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualBorderColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth);
                ChatManager.DrawColorCodedString(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth);
            }
        }
        public void Update()
        {
            timeLeft--;
            if (timeLeft > 0)
            {
                SetVisibleText();
                float progress = Utils.GetLerpValue(0, timeLeftMax, timeLeft);
                float lerpV = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * lerpSpeed, 0, 1);
                if (visibleText.Length > 0)
                {
                    for (int i = 0; i < visibleText.Length; i++)
                    {
                        charTextColor[i] = Color.Lerp(Color.Lerp(charTextColor[i] * 0, charTextColor[i], lerpV), Color.Lerp(textColor * 0, textColor, lerpV), 0.2f);
                        charBorderColor[i] = Color.Lerp(Color.Lerp(charBorderColor[i] * 0, charBorderColor[i], lerpV), Color.Lerp(borderColor * 0, borderColor, lerpV), 0.2f);
                        charVisibleScale[i] = MathHelper.Lerp(charVisibleScale[i], scale, 0.25f);
                        charVisibleCenter[i] = Vector2.Lerp(charVisibleCenter[i], Center, 0.25f);
                    }
                }
                actualTextColor = Color.Lerp(Color.Lerp(actualTextColor * 0, actualTextColor, lerpV), Color.Lerp(textColor * 0, textColor, lerpV), 0.2f);
                actualBorderColor = Color.Lerp(borderColor * 0, borderColor, lerpV);
                visibleScale = MathHelper.Lerp(visibleScale, scale, 0.25f);
                VisibleCenter = Vector2.Lerp(VisibleCenter, Center, 0.25f);
                if (visibleText.Length > 0)
                {
                    if (oldVisibleText != visibleText && visibleText != text)
                    {
                        if (timeLeft % soundInterval == 0 && sound != EbonianSounds.None)
                            SoundEngine.PlaySound(sound, Center);
                        if (animationType != DialogueAnimationIDs.ColorWhite)
                        {
                            charTextColor[visibleText.Length - 1] = actualTextColor * 0f;
                            charBorderColor[visibleText.Length - 1] = actualBorderColor * 0f;
                        }
                        if (animationType.HasFlag(DialogueAnimationIDs.BopDown)) charVisibleCenter[visibleText.Length - 1] -= Vector2.UnitY * 10 * scale;
                        if (animationType.HasFlag(DialogueAnimationIDs.BopUp)) charVisibleCenter[visibleText.Length - 1] += Vector2.UnitY * 10 * scale;
                        if (animationType.HasFlag(DialogueAnimationIDs.ScaleUp)) charVisibleScale[visibleText.Length - 1] += .15f * scale;
                        if (animationType.HasFlag(DialogueAnimationIDs.ScaleDown)) charVisibleScale[visibleText.Length - 1] -= .15f * scale;
                        if (animationType.HasFlag(DialogueAnimationIDs.ColorWhite)) charTextColor[visibleText.Length - 1] = Color.White;
                        if (animationType.HasFlag(DialogueAnimationIDs.ColorWhite)) charBorderColor[visibleText.Length - 1] = Color.White;
                    }
                }
            }
        }
    }
}
