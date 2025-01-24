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
        public override void AddRecipeGroups()
        {
            // SilverBars = new RecipeGroup(() => "SilverOrTungsten", [ItemID.SilverBar, ItemID.TungstenBar]);
        }
    }
}
