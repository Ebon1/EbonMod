using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Weapons.Ranged
{
    public class FangSlinger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fang Slinger");
            Tooltip.SetDefault("Shoots fangs, who would've thought.");
        }

        public override void SetDefaults()
        {
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 5;
            Item.reuseDelay = 40;
            Item.useAnimation = 15;
            Item.useStyle = 5;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 0;
            Item.rare = 2;
            Item.shootSpeed = 7f;
            Item.useAmmo = AmmoID.Arrow;
            Item.shoot = ModContent.ProjectileType<Projectiles.Friendly.Crimson.Fangs>();
            Item.autoReuse = true;
        }
        int amount = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ModContent.ProjectileType<Projectiles.Friendly.Crimson.Fangs>();
            amount++;
            if (amount > 3)
                amount = 1;
            for (int i = 0; i < amount; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-30, 30) * (amount == 1 ? 0 : 1))), type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}