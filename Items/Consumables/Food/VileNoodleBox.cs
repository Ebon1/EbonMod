using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonMod.Items.Consumables.Food
{
	public class VileNoodleBox : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Vile Noodle Box"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("Provides you 100% guaranteed literal heartburn");

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
        public override bool OnPickup(Player player)
        {
            bool pickupText = false;
            for (int i = 0; i < 50; i++)
            {
                if (player.inventory[i].type == ItemID.None && !pickupText)
                {
                    CombatText.NewText(new Rectangle((int)player.position.X, (int)player.position.Y - 20, player.width, player.height), new Color(40, 230, 40, 255), "Who let bro cook!!??", false, false);
                    pickupText = true;

                }
            }

            SoundEngine.PlaySound(SoundID.Item16, Item.position);
            return true;
        }
        
	}
}
