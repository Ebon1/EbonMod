using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.NPCs.Corruption;
using EbonianMod.NPCs.Terrortoma;
using EbonianMod.NPCs.Cecitior;
using EbonianMod.Worldgen.Subworlds;
using SubworldLibrary;
using EbonianMod.NPCs.Exol;

namespace EbonianMod.Items.Consumables.BossItems
{
    public class RedditGluttonSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<TheTrueGluttonEXNeoGod>());
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<TheTrueGluttonEXNeoGod>());
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
    }
    public class TerrortomaSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 2;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Terrortoma>());
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<Terrortoma>());
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
    }
    public class CecitiorSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 20;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Cecitior>());
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 20).AddTile(TileID.Anvils).Register();
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<Cecitior>());
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
    }
    public class ExolSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Exol>());
        }
        public override void AddRecipes()
        {
            CreateRecipe().Register();
        }

        public override bool? UseItem(Player player)
        {
            NPC.NewNPCDirect(player.GetSource_FromThis(), player.Center + new Microsoft.Xna.Framework.Vector2(300, -200), ModContent.NPCType<Exol>());
            return true;
        }
    }
    public class IgnosSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }
        public override string Texture => Helper.Placeholder;
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 1000000;
            Item.rare = ItemRarityID.Red;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 20).AddTile(TileID.Anvils).Register();
        }

        public override bool? UseItem(Player player)
        {
            if (!SubworldSystem.IsActive<Ignos>())
                SubworldSystem.Enter<Ignos>();
            else
                SubworldSystem.Exit();
            return true;
        }
    }
}