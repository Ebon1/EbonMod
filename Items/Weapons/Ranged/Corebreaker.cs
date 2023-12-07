using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Projectiles.Exol;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Underworld;
using EbonianMod.Achievements;
using Terraria.UI;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class Corebreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.width = 40;
            Item.height = 40;
            Item.reuseDelay = 25;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = 8;
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CorebreakerP>();
            Item.shootSpeed = 14;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 35).AddTile(TileID.MythrilAnvil).Register();
        }
        public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //   for (int i = 0; i < 2; i++)
            //     Helper.DustExplosion(position + velocity, Vector2.One, 2, Color.White, false, false, 0.6f, 0.5f, new(velocity.X / 2, Main.rand.NextFloat(-5, -3)));
            InGameNotificationsTracker.AddNotification(new EbonianAchievementNotification(1));
            Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<CorebreakerP>(), damage * 2, knockback, player.whoAmI);
            /*for (int i = -1; i < 2; i++)
            {
                if (i == 0)
                    continue;
                Projectile a = Projectile.NewProjectileDirect(Item.InheritSource(Item), position, velocity + new Vector2(0, 5 * i).RotatedBy(velocity.ToRotation()), ModContent.ProjectileType<EFireBreath2>(), damage, knockback, player.whoAmI);
                a.friendly = true;
                a.hostile = false;
                a.localAI[0] = 600;
                a.scale = 1;

            }*/
            Projectile a = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Item), position, velocity, ModContent.ProjectileType<EFireBreath2>(), damage, knockback, player.whoAmI);
            a.friendly = true;
            a.hostile = false;
            a.localAI[0] = 400;
            a.localAI[1] = 2;
            a.scale = 1;
            return false;
        }
    }
}
