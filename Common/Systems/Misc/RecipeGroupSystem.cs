using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Common.Systems.Misc
{
    public class RecipeGroupSystem : ModSystem
    {
        public static RecipeGroup SilverBars;

        public override void Unload()
        {
            SilverBars = null;
        }
        
        public override void AddRecipeGroups()
        {
            SilverBars = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {Lang.GetItemNameValue(ItemID.SilverBar)}",
                ItemID.SilverBar, ItemID.TungstenBar);
            RecipeGroup.RegisterGroup("EbonianMod:AnySilver", SilverBars);

        
        }
    }
}
