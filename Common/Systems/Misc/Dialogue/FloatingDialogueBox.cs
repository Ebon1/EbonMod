using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace EbonianMod.Common.Systems.Misc.Dialogue
{
    public class DialogueAnimationIDs
    {
        public const int None = 0, BopUp = 1, BopDown = 2, ScaleUp = 3, ScaleDown = 4, ColorWhite = 5;
    }
    public class FloatingDialogueBox
    {
        public int timeLeft, timeLeftMax;
        public Vector2 Center, VisibleCenter, charVisibleCenter;
        public string text, visibleText, oldVisibleText;
        public float scale, visibleScale, charVisibleScale;
        public float lerpSpeed;
        public bool substring = true;
        public int maxWidth = -1;
        public Color borderColor = Color.Transparent;
        public Color textColor = Color.White;
        public Color actualBorderColor = Color.Transparent;
        public Color actualTextColor = Color.White;
        public Color charTextColor = Color.White;
        public int animationType;
        //public int animationInterval = 5;
        public FloatingDialogueBox(int timeLeft, Vector2 center, string text, Color textColor, int maxWidth = -1, float scale = 0.5f, Color borderColor = default, float lerpSpeed = 2f, bool substring = true, int animationType = DialogueAnimationIDs.None)
        {
            this.timeLeft = timeLeft;
            this.timeLeftMax = timeLeft;
            Center = center;
            VisibleCenter = center;
            charVisibleCenter = center;
            this.text = text;
            this.textColor = textColor;
            this.actualTextColor = textColor;
            this.maxWidth = maxWidth;
            this.scale = scale;
            this.visibleScale = scale;
            this.charVisibleScale = scale;
            this.borderColor = borderColor;
            this.actualBorderColor = borderColor;
            this.charTextColor = borderColor;
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
                    for (int i = 0; i < visibleText.Length - 1; i++)
                    {
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[i].ToString(), VisibleCenter + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(i)).X * scale, 0) - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth);
                    }
                    if (visibleText.Length != 0)
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, visibleText[visibleText.Length - 1].ToString(), charVisibleCenter + new Vector2(FontAssets.DeathText.Value.MeasureString(visibleText.Remove(visibleText.Length - 1)).X * scale, 0) - Main.screenPosition, charTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(charVisibleScale, charVisibleScale), maxWidth);
                }
                else
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth); // add animations to new lines later
                }
            }
            else
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, text, VisibleCenter - Main.screenPosition, actualTextColor, 0, new Vector2(FontAssets.DeathText.Value.MeasureString(text).X / 2, FontAssets.DeathText.Value.MeasureString(text).Y / 2), new Vector2(visibleScale, visibleScale), maxWidth);
            }
        }
        public void Update()
        {
            timeLeft--;
            if (timeLeft > 0)
            {
                float progress = Utils.GetLerpValue(0, timeLeftMax, timeLeft);
                float lerpV = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * lerpSpeed, 0, 1);
                actualTextColor = Color.Lerp(Color.Lerp(actualTextColor * 0, actualTextColor, lerpV), Color.Lerp(textColor * 0, textColor, lerpV), 0.2f);
                charTextColor = Color.Lerp(Color.Lerp(charTextColor * 0, charTextColor, lerpV), Color.Lerp(textColor * 0, textColor, lerpV), 0.2f);
                actualBorderColor = Color.Lerp(borderColor * 0, borderColor, lerpV);
                charVisibleScale = MathHelper.Lerp(charVisibleScale, scale, 0.25f);
                visibleScale = MathHelper.Lerp(visibleScale, scale, 0.25f);
                VisibleCenter = Vector2.Lerp(VisibleCenter, Center, 0.25f);
                charVisibleCenter = Vector2.Lerp(charVisibleCenter, Center, 0.25f);
                SetVisibleText();
                if (oldVisibleText != visibleText && visibleText != text)
                {
                    if (animationType != DialogueAnimationIDs.ColorWhite)
                    {
                        charTextColor = actualTextColor * 0f;
                    }
                    switch (animationType)
                    {
                        case DialogueAnimationIDs.BopUp: charVisibleCenter.Y -= 10 * scale; break;
                        case DialogueAnimationIDs.BopDown: charVisibleCenter.Y += 10 * scale; break;
                        case DialogueAnimationIDs.ScaleUp: charVisibleScale += .15f * scale; break;
                        case DialogueAnimationIDs.ScaleDown: charVisibleScale -= .15f * scale; break;
                        case DialogueAnimationIDs.ColorWhite: charTextColor = Color.White; break;
                    }
                }
            }
        }
    }
}
