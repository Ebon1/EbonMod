using EbonianMod.Items.Consumables.Food;
using EbonianMod.Projectiles.Friendly.Generic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
            Item.useAmmo = ItemType<Potato>();
            Item.shootSpeed = 17;
            Item.shoot = ProjectileType<PotatoP>();
        }
        public override Vector2? HoldoutOffset()
        {
            return Vector2.UnitX * -16;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity *= Main.rand.NextFloat(0.6f, 1.2f);
        }
    }
}
