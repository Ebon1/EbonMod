using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Crimson;

namespace EbonianMod.Items.Weapons.Magic
{
    public class Latcher : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 80;
            Item.useTime = 1;
            Item.mana = 1;
            Item.useAnimation = 10;
            Item.shoot = ProjectileType<LatcherSprite>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = 5;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Vertebrae, 20).AddIngredient(ItemID.Hook).AddTile(TileID.Anvils).Register();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<LatcherP>()] < 1;
        }
        public override bool? CanAutoReuseItem(Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class LatcherSprite : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/Latcher";
        
        Vector2 Scale = new Vector2(0, 0);

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + player.direction * Pi;
        }

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(60, 38);
        }

        bool CanShoot = true;

        public override void AI()
        {
            Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

            Player player = Main.player[Projectile.owner];
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.Center = new Vector2(player.Center.X, player.Center.Y);

            if((player.ownedProjectileCounts[ProjectileType<LatcherP>()] < 1 && CanShoot == false) || !player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            if(CanShoot == false)
            {
                Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation(), 0.02f);
            }
            else
            {
                Projectile.rotation = Helper.LerpAngle(Projectile.rotation, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation(), 0.08f);
                if(player.channel == false)
                {
                    CanShoot = false;
                    Scale = new Vector2(0.65f, 1.6f);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.Center.X, Projectile.Center.Y) + Projectile.rotation.ToRotationVector2() * 30, Projectile.rotation.ToRotationVector2()*37, ProjectileType<LatcherP>(), 1, Projectile.knockBack, Projectile.owner);       
                }
            }
            player.direction = player.Center.X - Main.MouseWorld.X < 0 ? 1 : -1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetTexture(Mod.Name + "/Items/Weapons/Magic/Latcher");
            Rectangle frameRect = new Rectangle(0, 0, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frameRect, lightColor, Projectile.rotation, new Vector2(Projectile.width / 2-25, Projectile.height / 2), Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }
}
