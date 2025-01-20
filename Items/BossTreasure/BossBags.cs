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
using EbonianMod.Items.Misc;

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
            itemLoot.Add(ItemDropRule.Common(ItemType<CecitiorMaterial>(), 1, 40, 60));
            itemLoot.Add(ItemDropRule.Common(ItemType<BrainAcc>(), 1));
            itemLoot.Add(ItemDropRule.Common(ItemType<SelfStab>(), 1));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<Cecitior>()));
        }
    }
    public class TerrortomaBag : BossBags
    {
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ItemType<TerrortomaMaterial>(), 1, 40, 60));
            itemLoot.Add(ItemDropRule.Common(ItemType<EbonianHeart>(), 1));
            itemLoot.Add(ItemDropRule.Common(ItemType<Ostertagi>(), 1));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<Terrortoma>()));
        }
    }

    public class GarbageBagI : BossBags
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            ItemID.Sets.BossBag[Type] = true;
        }
        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.Common(ItemType<HotShield>(), 1));
            itemLoot.Add(ItemDropRule.Common(ItemType<Chainsword>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<DoomsdayRemote>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<MailboxStaff>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<SalvagedThruster>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<GarbageFlail>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<PipebombI>(), 1, 20, 100));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<HotGarbage>()));
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
            itemLoot.Add(ItemDropRule.Common(ItemType<XTentacleAcc>(), 1));
            itemLoot.Add(ItemDropRule.Common(ItemType<PhantasmalGreatsword>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<XareusPotion>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<StaffofXWeapon>(), 3));
            itemLoot.Add(ItemDropRule.Common(ItemType<ArchmageXTome>(), 3));
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<ArchmageX>()));
        }
    }
}
