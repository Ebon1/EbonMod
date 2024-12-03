using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.Items.Weapons.Melee;
using EbonianMod.NPCs.Terrortoma;
using EbonianMod.Items.Weapons.Summoner;
using EbonianMod.Items.Materials;
using EbonianMod.Items.Accessories;

namespace EbonianMod.Items.BossTreasure
{
    public abstract class BossBags : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;

            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Purple;
            Item.expert = true;
        }

        public override bool CanRightClick()
        {
            return true;
        }
    }
    public class CecitiorBag : BossBags
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CecitiorMaterial>(), 1, 40, 60));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrainAcc>(), 1));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Cecitior>()));
        }
    }
    public class TerrortomaBag : BossBags
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TerrortomaMaterial>(), 1, 40, 60));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<EbonianHeart>(), 1));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Terrortoma>()));
        }
    }
}
