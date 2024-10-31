using EbonianMod.Items.Consumables.Food;
using EbonianMod.Projectiles.Friendly.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class SpudCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(58, 24);
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Blue;
            Item.damage = 5;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.UseSound = SoundID.Item20;
            Item.useAmmo = ModContent.ItemType<Potato>();
            Item.shootSpeed = 17;
            Item.shoot = ModContent.ProjectileType<PotatoP>();
        }
        public override Vector2? HoldoutOffset()
        {
            return Vector2.UnitX * -16;
        }
    }
}
