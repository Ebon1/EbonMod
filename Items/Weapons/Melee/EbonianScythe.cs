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
            Item.useTime = 20;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 2.5f;
            Item.shoot = ModContent.ProjectileType<EbonianScytheP>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, i);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }
    }
    public class EbonianScytheP : ModProjectile
    {

        public override string Texture => "EbonianMod/Items/Weapons/Melee/EbonianScythe";
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
            Projectile.aiStyle = 0;
            Projectile.Size = new Vector2(36, 44);

        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(10);
            if (Projectile.ai[0] < 100)
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3) * Projectile.ai[1]);
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 100)
            {
                Projectile.velocity.X = 0;
                Projectile.velocity.Y++;
            }
        }
    }
}

