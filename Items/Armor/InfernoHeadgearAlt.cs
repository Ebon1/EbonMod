using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class InfernoHeadgearAlt : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 24;
        }
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<InfernoBreastplate>() && legs.type == ModContent.ItemType<InfernoLeggings>();
        }
    }
}
