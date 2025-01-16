using EbonianMod.NPCs.Overworld.Critters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Items.Misc
{
    public class SheepItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Bunny);
            Item.makeNPC = NPCType<Sheep>();
            Item.value += Item.buyPrice(0, 0, 10, 0);
        }
    }
}
