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
            DisplayName.SetDefault("fish gun");
        }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 2;
            Item.useTime = 15;
            Item.useAnimation = 30;
            Item.shoot = ProjectileID.ToxicBubble;
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
            if (player.itemAnimation < 15)
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(source, position, velocity + new Vector2(0, Main.rand.NextFloat(-5, 5)).RotatedBy(velocity.ToRotation()), ProjectileID.ToxicBubble, damage, knockback, player.whoAmI);
                }
            else
                Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<WeakCursedBullet>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}
