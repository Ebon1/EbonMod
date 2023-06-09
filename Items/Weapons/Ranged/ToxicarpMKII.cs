using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles.Friendly.Corruption;
using EbonianMod.Projectiles.Friendly;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class ToxicarpMKII : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 3;
            Item.useTime = 2;
            Item.useAnimation = 30;
            Item.shoot = ModContent.ProjectileType<TinyFish>();
            Item.shootSpeed = 8f;
            Item.rare = 2;
            Item.useStyle = 5;
            Item.UseSound = SoundID.Item11;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.itemAnimation > 21)
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(10)), ModContent.ProjectileType<TinyFish>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
