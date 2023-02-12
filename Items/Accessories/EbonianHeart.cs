using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    internal class EbonianHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebonian Heart");
            Tooltip.SetDefault("16 Minion damage\nSummons a corrupted heart that follows you and shoots cursed flames at nearby enemies.\n\"That thing is still beating...?\"");
        }
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
            Item.defense = 5;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<EbonianHeartNPC>()))
            {
                NPC.NewNPC(player.GetSource_Accessory(Item), (int)player.Center.X, (int)player.Center.Y, ModContent.NPCType<EbonianHeartNPC>(), Target: player.whoAmI);
            }
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.heartAcc = true;
        }
    }
}