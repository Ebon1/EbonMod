using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Consumables.Food
{
    public class VileNoodleBox : ModItem
    {
        public override void SetStaticDefaults()
        {

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;

            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

            ItemID.Sets.DrinkParticleColors[Type] = new Color[2] {
                new Color(40, 240, 4),
                new Color(80, 120, 90 ),
            };
            ItemID.Sets.IsFood[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToFood(22, 22, BuffID.WellFed3, 18000);

            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = 2800;
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.UseSound = SoundID.Item2;


        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "VileNoodleBox", "'These are either worms or noodles....'")
            {
                OverrideColor = new Color(80, 210, 73)

            };
            tooltips.Add(line);


        }
        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.CursedInferno, 320);
        }

    }
}
