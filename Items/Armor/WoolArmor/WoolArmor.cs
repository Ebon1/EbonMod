using EbonianMod.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Items.Armor.WoolArmor
{
    [AutoloadEquip(EquipType.Head)]
    public class WoolenUshanka : ModItem
    {
        public static LocalizedText SetBonusText { get; private set; }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(2.5); // summon damage increase
        public override void SetStaticDefaults()
        {
            SetBonusText = this.GetLocalization("SetBonus").WithFormatArgs(1); // minion increase
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SetBonusText.Value;
            player.maxMinions += 1;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.025f;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<WoolenCoat>() && legs.type == ItemType<WoolenBoots>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 10).AddIngredient<Wool>(20).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 10).AddIngredient<Wool>(20).AddTile(TileID.Anvils).Register();
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class WoolenCoat : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 1); // summon damage increase, minion increase
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.05f;
            player.maxMinions += 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 15).AddIngredient<Wool>(30).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 15).AddIngredient<Wool>(30).AddTile(TileID.Anvils).Register();
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class WoolenBoots : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(10, 5); // whip damage increase, whip speed increase
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = 100;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.1f;
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.05f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 10).AddIngredient<Wool>(25).AddTile(TileID.Anvils).Register();
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 10).AddIngredient<Wool>(25).AddTile(TileID.Anvils).Register();
        }
    }
}
