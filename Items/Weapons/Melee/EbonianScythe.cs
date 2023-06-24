using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles;
using System.Collections.Generic;
using Terraria.Audio;
using System.Collections.ObjectModel;

namespace EbonianMod.Items.Weapons.Melee
{
    public class EbonianScythe : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 32;
            Item.height = 34;
            Item.crit = 45;
            Item.damage = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1;
            Item.shoot = ModContent.ProjectileType<EbonianScytheP>();
        }
        int uses, dir = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }
    }
    public class EbonianScytheP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/EbonianScythe";

        public override void SetExtraDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            swingTime = 10;
            holdOffset = 15;
        }
    }
}

