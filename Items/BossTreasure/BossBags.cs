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
using EbonianMod.NPCs.Garbage;
using EbonianMod.Projectiles.Garbage;
using EbonianMod.Items.Weapons.Ranged;
using EbonianMod.Items.Weapons.Magic;
using EbonianMod.NPCs.ArchmageX;

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
            Item.width = 16;
            Item.height = 16;
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

    public class GarbageBag : BossBags
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<HotShield>(), 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<Chainsword>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DoomsdayRemote>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DoomsdayRemote>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PipebombI>(), 1, 20, 100));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<HotGarbage>()));
        }
    }

    public class ArchmageBag : BossBags
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            //itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<>(), 1)); *for expert
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhantasmalGreatsword>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<XareusPotion>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaffofXWeapon>(), 4));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArchmageXTome>(), 4));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<ArchmageX>()));
        }
    }
}
