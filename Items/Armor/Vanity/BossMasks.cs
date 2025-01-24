using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Armor.Vanity
{
    public abstract class BossMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
    [AutoloadEquip(EquipType.Head)]
    public class XMask : BossMask { }
    [AutoloadEquip(EquipType.Head)]
    public class GarbageMask : BossMask { }
    [AutoloadEquip(EquipType.Head)]
    public class TTomaMask : BossMask { }
    [AutoloadEquip(EquipType.Head)]
    public class CeciMask : BossMask { }
}
