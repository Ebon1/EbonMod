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
using Terraria.GameContent.Creative;
namespace EbonianMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class EbonianHeart : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 1, 5); // Defense reduction, Minion increase, Summon damage increase
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.rare = 4;
            Item.defense = -5;
            Item.expert = true;
        }
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!NPC.AnyNPCs(NPCType<EbonianHeartNPC>()))
            {
                NPC.NewNPC(player.GetSource_Accessory(Item), (int)player.Center.X, (int)player.Center.Y, NPCType<EbonianHeartNPC>(), Target: player.whoAmI);
            }
            player.GetDamage(DamageClass.Summon) += 0.05f;
            player.maxMinions += 1;
            EbonianPlayer modPlayer = player.GetModPlayer<EbonianPlayer>();
            modPlayer.heartAcc = true;
        }
    }
}