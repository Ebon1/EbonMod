using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using EbonianMod.Projectiles.Minions;
using EbonianMod.Buffs;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using EbonianMod.Items.Materials;
using EbonianMod.Items.Weapons.Melee;

namespace EbonianMod.Items.Weapons.Summoner
{
    public class CecitiorClawSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 73;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.NPCHit1;
            Item.shoot = ProjectileType<CecitiorClawMinion>();
            Item.buffType = BuffType<CecitiorClawBuff>();
            Item.shootSpeed = 1;
            Item.buffTime = 100;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Hook).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
