using Terraria.ModLoader;

namespace EbonianMod
{
    public class RecipeGroups : ModSystem
    {
        public static RecipeGroup silverGroup;

        public override void Unload()
        {
            silverGroup = null;
        }

        public override void AddRecipeGroups()
        {
            silverGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
                ItemID.SilverBar, ItemID.TungstenBar);
            RecipeGroup.RegisterGroup("EbonianMod:AnySilver", silverGroup);
        }
    }
}
