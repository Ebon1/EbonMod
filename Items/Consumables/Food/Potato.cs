using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace EbonianMod.Items.Consumables.Food
{
    internal class Potato : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.DrinkParticleColors[Type] = new Color[2] {
                new Color(149, 97, 37),
                new Color(116, 61, 7 ),
            };
            ItemID.Sets.IsFood[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(14, 20, BuffID.WellFed3, 60 * 60);

            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 9999;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = 2800;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;


        }
    }
}
