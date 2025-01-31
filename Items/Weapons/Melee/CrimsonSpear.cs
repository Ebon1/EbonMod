using EbonianMod.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Audio;
using System.Collections.Generic;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Common.Systems.Misc.Dialogue;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Melee
{
    public class CrimsonSpear : ModItem
    {

        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 48;
            Item.height = 66;
            Item.crit = 15;
            Item.damage = 40;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<CrimsonSpearP>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TheRottedFork).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        int dir = -1;
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (dir < 1)
                dir++;
            else
                dir = -1;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            return false;
        }
    }
}