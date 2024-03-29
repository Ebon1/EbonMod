using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Weapons.Melee
{
    public class TerrorFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = 4;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.knockBack = 4f;
            Item.damage = 20;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<TerrortomaFlail>();
            Item.shootSpeed = 15.1f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Generic;
            Item.crit = 9;
            Item.channel = true;
        }
    }
}