using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class InfernoHeadgear : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 16;
        }
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<InfernoBreastplate>() && legs.type == ItemType<InfernoLeggings>();
        }
    }
}
