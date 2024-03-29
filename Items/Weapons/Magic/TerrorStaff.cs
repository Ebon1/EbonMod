﻿using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.Minions;
using EbonianMod.Projectiles.Friendly.Corruption;

namespace EbonianMod.Items.Weapons.Magic
{
    public class TerrorStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 10;
            Item.reuseDelay = 20;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = 5;
            Item.knockBack = 10; Item.rare = 8;
            Item.value = 1000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerrorStaffP>();
            Item.shootSpeed = 14;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pointPoisition = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
            float num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            float num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
            Vector2 vector5 = new Vector2(num2, num3);
            vector5.X = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            vector5.Y = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y - 1000f;
            player.itemRotation = (float)Math.Atan2(vector5.Y * (float)player.direction, vector5.X * (float)player.direction);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
            NetMessage.SendData(41, -1, -1, null, player.whoAmI);

            Projectile.NewProjectile(source, position, new Vector2(velocity.X * 0.5f * Main.rand.NextFloat(1, 2), -Item.shootSpeed), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
