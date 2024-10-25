using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class InfernoBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;
            Item.defense = 17;
        }
        public override bool IsLoadingEnabled(Mod mod) => false;
        public override void UpdateEquip(Player player)
        {

        }
    }
}
