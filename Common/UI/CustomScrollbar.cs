using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using EbonianMod.Common.Achievements;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.ModLoader;

namespace EbonianMod.Common.UI
{
    public class CustomScrollbar : UIScrollbar
    {
        // Code taken directly from Infernum, you can probably tell why
        public readonly Asset<Texture2D> _texture;

        public readonly Asset<Texture2D> _innerTexture;

        public CustomScrollbar()
        {
            Width.Set(20f, 0f);
            MaxWidth.Set(20f, 0f);
            _texture = ModContent.Request<Texture2D>("Ebonianmod/Extras/Sprites/scrollbarBg", AssetRequestMode.ImmediateLoad);
            _innerTexture = Main.Assets.Request<Texture2D>("Images/UI/ScrollbarInner");
            PaddingTop = 5f;
            PaddingBottom = 5f;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if ((bool)typeof(UIScrollbar).GetField("_isDragging", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar))
            {
                float offset = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y -
                    (float)typeof(UIScrollbar).GetField("_dragYOffset", BindingFlags.NonPublic | BindingFlags.Instance).
                    GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar);

                ViewPosition = MathHelper.Clamp(offset / innerDimensions.Height * (float)typeof(UIScrollbar).
                    GetField("_maxViewSize", BindingFlags.NonPublic | BindingFlags.Instance).
                    GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar), 0f, (float)typeof(UIScrollbar).
                    GetField("_maxViewSize", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar) - (float)typeof(UIScrollbar).
                    GetField("_viewSize", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar));
            }

            Rectangle handleRectangle = (Rectangle)typeof(UIScrollbar).GetMethod("GetHandleRectangle", BindingFlags.Instance | BindingFlags.NonPublic).
                Invoke(EbonianAchievementSystem.achievementUIState.realScrollbar, null);

            Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
            bool isHoveringOverHandle = (bool)typeof(UIScrollbar).GetField("_isHoveringOverHandle", BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar);

            bool tempBool = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
            typeof(UIScrollbar).GetField("_isHoveringOverHandle", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(EbonianAchievementSystem.achievementUIState.realScrollbar, tempBool);
            if (!isHoveringOverHandle && (bool)typeof(UIScrollbar).GetField("_isHoveringOverHandle", BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar) && Main.hasFocus)
                SoundEngine.PlaySound(SoundID.MenuTick);

            DrawBar(spriteBatch, _texture.Value, dimensions.ToRectangle(), Color.White);
            DrawBar(spriteBatch, _innerTexture.Value, handleRectangle, Color.White * ((bool)typeof(UIScrollbar).
                GetField("_isDragging", BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar) || (bool)typeof(UIScrollbar).
                GetField("_isHoveringOverHandle", BindingFlags.NonPublic | BindingFlags.Instance).
                GetValue(EbonianAchievementSystem.achievementUIState.realScrollbar) ? 1f : 0.85f));
        }

        internal static void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
        {
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle(0, 0, texture.Width, 6), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, 6, texture.Width, 4), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle(0, texture.Height - 6, texture.Width, 6), color);
        }
    }
}
