using EbonianMod.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace EbonianMod.Common.Achievements
{
    public class EbonianAchievement : UIPanel
    {
        public EbonianAchievement(int index, string text, string subtext, Color hoverColor, bool hiddenUntilUnlocked = false, string texture = "Extras/Sprites/Achievements")
        {
            Index = index;
            Text = text;
            Subtext = subtext;
            TexturePath = texture;
            HiddenUntilUnlocked = hiddenUntilUnlocked;
            BackgroundColor = /*new Color(167, 51, 16)*/ new Color(17, 17, 17) * 0.75f;
            BorderColor = Color.Black * 0.75f;
            Height.Set(82f, 0f);
            Width.Set(0f, 1f);
            PaddingTop = 8f;
            PaddingLeft = 9f;
            HoverColor = hoverColor;
            icon = new UIImageFramed(ModContent.Request<Texture2D>("EbonianMod/" + TexturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad), new Rectangle(0, 64 * index, 64, 64));
            iconBorder = new UIImageFramed(ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/iconBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad), new Rectangle(0, 0, 64, 64));
            iconBorder.Color = Color.Gray * 0.4f;
            Append(icon);
            Append(iconBorder);
        }
        public UIImageFramed icon, iconBorder;
        public Color realBorderColor = Color.White * 0.35f, color = Color.Gray, HoverColor;
        public int Index;
        public bool locked;
        public bool HiddenUntilUnlocked;
        public string Text;
        public string Subtext;
        public string TexturePath;
        public override void Update(GameTime gameTime)
        {
            locked = !EbonianAchievementSystem.acquiredAchievement[Index];
            base.Update(gameTime);
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float offset = (MathF.Sin(Main.GlobalTimeWrappedHourly * 2.5f + ((Index + 1) * -20)) + 1) * 0.5f;
            locked = !EbonianAchievementSystem.acquiredAchievement[Index];
            base.DrawSelf(spriteBatch);
            CalculatedStyle iconSize = icon.GetDimensions();
            float width = GetInnerDimensions().Width - iconSize.Width + 1f;
            icon.SetFrame(new Rectangle(locked ? 0 : 64, 64 * Index, 64, 64));

            Vector2 textScale = new(0.85f);
            Vector2 pos = new Vector2(iconSize.X + iconSize.Width + 7f, GetInnerDimensions().Y - 2f);

            Texture2D topTex = Helper.GetExtraTexture("Sprites/TopPanel");
            Texture2D bottomTex = Helper.GetExtraTexture("Sprites/BottomPanel");
            Color _color = IsMouseHovering ? Color.Lerp(HoverColor * 0.8f, HoverColor, offset) : Color.Lerp(Color.Gray, Color.LightGray, offset);
            color = Color.Lerp(color, _color, 0.15f);

            Color borderColor = IsMouseHovering ? Color.White : Color.Lerp(Color.Gray, Color.White, offset);
            realBorderColor = Color.Lerp(realBorderColor, borderColor, 0.15f);
            iconBorder.Color = realBorderColor;

            spriteBatch.Draw(topTex, pos, new Rectangle(0, 0, 2, topTex.Height), color);
            spriteBatch.Draw(topTex, new Vector2(pos.X + 2f, pos.Y), new Rectangle(2, 0, 2, topTex.Height), color, 0f, Vector2.Zero, new Vector2((width - 4f) / 2f, 1f), SpriteEffects.None, 0f);
            spriteBatch.Draw(topTex, new Vector2(pos.X + width - 2f, pos.Y), new Rectangle(4, 0, 2, topTex.Height), color);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, Text, pos + new Vector2(9, 5), Color.Gold, 0f, Vector2.Zero, textScale, width);

            pos += new Vector2(0, 29f);

            spriteBatch.Draw(bottomTex, pos, new Rectangle(0, 0, 6, bottomTex.Height), color);
            spriteBatch.Draw(bottomTex, new Vector2(pos.X + 6f, pos.Y), new Rectangle(6, 0, 7, bottomTex.Height), color, 0f, Vector2.Zero, new Vector2((width - 12f) / 7f, 1f), SpriteEffects.None, 0f);
            spriteBatch.Draw(bottomTex, new Vector2(pos.X + width - 6f, pos.Y), new Rectangle(13, 0, 6, bottomTex.Height), color);

            pos += new Vector2(8, 4);
            Vector2 subtextScale = new(0.87f);
            string subtext = FontAssets.ItemStack.Value.CreateWrappedText(Subtext, (width - 20f) * (1f / subtextScale.X), Language.ActiveCulture.CultureInfo);
            if (subtext.Contains("\n"))
                subtextScale = new(0.84f);
            if (locked && HiddenUntilUnlocked)
                subtext = "???";
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, subtext, pos, locked ? Color.Gray : Color.White, 0f, Vector2.Zero, subtextScale);
        }
    }
    public class EbonianAchievementUIState : UIState
    {
        UIElement ui;
        UIList achievementList;
        List<EbonianAchievement> achievements = new List<EbonianAchievement>();
        public UIScrollbar realScrollbar;
        bool[] achievementActive = new bool[EbonianAchievementSystem.maxAchievements];
        int timer;
        public override void OnInitialize()
        {
            achievements.Clear();
            RemoveAllChildren();
            ui = null;
            achievementList = null;

            ui = new UIElement();
            ui.Width.Set(0f, 0.8f);
            ui.MaxWidth.Set(900f, 0f);
            ui.MinWidth.Set(700f, 0f);
            ui.Top.Set(150f, 0f);
            ui.Height.Set(-220f, 1f);
            ui.HAlign = 0.5f;
            Append(ui);

            UIPanel uIPanel = new();
            uIPanel.Width.Set(0f, 1f);
            uIPanel.Height.Set(-110f, 1f);
            uIPanel.BackgroundColor = new Color(17, 17, 17) * 0.75f;
            uIPanel.PaddingTop = 0f;
            ui.Append(uIPanel);

            achievementList = new UIList();
            achievementList.Width.Set(-25f, 1f);
            achievementList.Height.Set(-50f, 1f);
            achievementList.Top.Set(50f, 0f);
            achievementList.ListPadding = 5f;
            uIPanel.Append(achievementList);

            UITextPanel<LocalizedText> uITextPanel = new(Language.GetText("UI.Achievements"), 1f, large: true)
            {
                HAlign = 0.5f
            };
            uITextPanel.Top.Set(-33f, 0f);
            uITextPanel.SetPadding(13f);
            uITextPanel.BackgroundColor = new Color(17, 17, 17) * 0.85f;
            /*UIPanel uiTextBG = new()
            {
                HAlign = 0.5f
            };
            uiTextBG.Top.Set(-176f, 0f);
            uiTextBG.Height.Set(140, 0f);
            uiTextBG.Width.Set(350, 0f);
            uiTextBG.SetPadding(13f);
            uiTextBG.BackgroundColor = new Color(17, 17, 17) * 0.85f;

            UIImage logo = new UIImage(ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/Logo"))
            {
                HAlign = 0.5f
            };
            logo.Top.Set(-170f, 0f);
            logo.Height.Set(130, 0f);
            logo.Width.Set(334, 0f);
            logo.SetPadding(13f);

            ui.Append(uiTextBG);
            ui.Append(logo);*/
            ui.Append(uITextPanel);

            UITextPanel<LocalizedText> uITextPanel2 = new(Language.GetText("UI.Back"), 0.7f, large: true);
            uITextPanel2.Width.Set(-10f, 0.5f);
            uITextPanel2.Height.Set(50f, 0f);
            uITextPanel2.VAlign = 1f;
            uITextPanel2.HAlign = 0.5f;
            uITextPanel2.Top.Set(-45f, 0f);
            uITextPanel2.OnMouseOver += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((UIPanel)evt.Target).BackgroundColor = new Color(17, 17, 17);
                ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
            };
            uITextPanel2.OnMouseOut += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                ((UIPanel)evt.Target).BackgroundColor = new Color(17, 17, 17) * 0.75f;
                ((UIPanel)evt.Target).BorderColor = Color.Black;
            };
            uITextPanel2.OnLeftClick += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                if (timer > 30 && Main.inFancyUI)
                {
                    timer = 0;
                    Main.menuMode = -1;
                    IngameFancyUI.Close();
                    Main.playerInventory = false;
                }
            };
            uITextPanel2.BackgroundColor = new Color(17, 17, 17) * 0.75f;
            ui.Append(uITextPanel2);

            for (int i = 0; i < EbonianAchievementSystem.maxAchievements; i++)
            {
                EbonianAchievement achievement = new EbonianAchievement(i, EbonianAchievementSystem.achievementTemplates[i].Text, EbonianAchievementSystem.achievementTemplates[i].Subtext, EbonianAchievementSystem.achievementTemplates[i].HoverColor, EbonianAchievementSystem.achievementTemplates[i].HiddenUntilUnlocked);
                achievements.Add(achievement);
                achievementList.Add(achievement);
            }

            CustomScrollbar scrollbar = new();
            scrollbar.SetView(100f, 1000f);
            scrollbar.Height.Set(-50f, 1f);
            scrollbar.Top.Set(50f, 0f);
            scrollbar.HAlign = 1f;
            uIPanel.Append(scrollbar);

            achievementList.SetScrollbar(scrollbar);
            realScrollbar = scrollbar;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.inFancyUI)
                return;
            Main.player[Main.myPlayer].mouseInterface = true;
            base.Draw(spriteBatch);
        }
        public override void OnActivate()
        {
            timer = 1;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (timer > 0)
                timer++;
        }
    }
    public class EbonianAchievementButtonUIState : UIState
    {
        UIImageFramed uiOutline, hover;
        public bool Hovering;
        public override void OnInitialize()
        {
            Vector2 location = new Vector2(68f, 265f);
            Vector2 vector = new Vector2(353f, 258f);
            new Vector2(40f, 267f);
            _ = vector + new Vector2(50f, 50f);
            if (Main.screenHeight < 650)
            {
                location.X += 52f * Main.inventoryScale;
            }
            uiOutline = new(ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/smallIconOutline"), new Rectangle(0, 0, 40, 40));
            uiOutline.Width.Set(40, 0f);
            uiOutline.Height.Set(40f, 0f);
            //uiOutline.VAlign = 0.255f;
            //uiOutline.HAlign = 0.0f;
            uiOutline.Left.Set(location.X, 0);
            uiOutline.Top.Set(location.Y, 0);
            //uiOutline.Left.Set(0, 0.035f);
            uiOutline.Color = Color.Black * 0.5f;
            hover = new(ModContent.Request<Texture2D>("EbonianMod/Extras/Sprites/smallIconHover"), new Rectangle(0, 0, 32, 32));
            hover.Width.Set(32, 0f);
            hover.Height.Set(32f, 0f);
            hover.Left.Set(-1, 0);
            hover.Top.Set(-1, 0);
            hover.Color = Color.Transparent;
            UIImageFramed uITextPanel2 = new(ModContent.Request<Texture2D>("EbonianMod/icon_small"), new Rectangle(0, 0, 30, 30));
            uITextPanel2.Width.Set(30, 0f);
            uITextPanel2.Height.Set(30f, 0f);
            uITextPanel2.Left.Set(3, 0);
            uITextPanel2.Top.Set(3, 0);
            uITextPanel2.OnMouseOver += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                Main.hoverItemName = "Ebonian Mod Achievements";
                hover.Color = Color.White;
                Hovering = true;
            };
            uITextPanel2.OnMouseOut += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                hover.Color = Color.Transparent;
                Hovering = false;
            };
            uITextPanel2.OnLeftClick += delegate (UIMouseEvent evt, UIElement listeningElement)
            {
                if (Main.playerInventory && !(Main.LocalPlayer.chest != -1 || Main.npcShop != 0))
                {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    IngameFancyUI.OpenUIState(EbonianAchievementSystem.achievementUIState);
                }
            };
            uITextPanel2.Append(hover);
            uiOutline.Append(uITextPanel2);
            Append(uiOutline);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (Hovering)
            {
                Main.player[Main.myPlayer].mouseInterface = true;
                Main.hoverItemName = "Ebonian Mod Achievements";
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Hovering)
            {
                Main.hoverItemName = "Ebonian Mod Achievements";
            }
        }
    }
}
