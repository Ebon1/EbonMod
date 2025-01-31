using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Minions;
using EbonianMod.Projectiles.Garbage;

namespace EbonianMod.Items.Weapons.Magic
{
    public class DoomsdayRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Zombie67;
            Item.shoot = ProjectileType<GarbageDroneF>();
            Item.shootSpeed = 5;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 6);
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = player.Center - new Vector2(0, 400);
        }
    }
}

