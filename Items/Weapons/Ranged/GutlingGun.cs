using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Projectiles.Cecitior;
using Mono.Cecil;
using Terraria.Audio;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class GutlingGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 30;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.reuseDelay = 25;
            Item.shoot = ProjectileType<GutlingGunP>();
            Item.shootSpeed = 8f;
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = 5;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.TheUndertaker).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = ProjectileType<GutlingGunP>();
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 35);
            return false;
        }
    }
    public class GutlingGunP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Ranged/GutlingGun";
        float holdOffset = 20;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(52, 30);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.15f - MathHelper.Lerp(0.09f, 0f, Projectile.ai[0] / 4)).SafeNormalize(Vector2.UnitX);
            if (Projectile.ai[0] > 2)
                Projectile.ai[0] -= 0.1f;
            if (++Projectile.ai[1] > Projectile.ai[0])
            {
                bool success = false;
                for (int j = 54; j < 58; j++)
                {
                    if (player.inventory[j].ammo == player.HeldItem.useAmmo && player.inventory[j].stack > 0)
                    {
                        if (player.inventory[j].maxStack > 1)
                            player.inventory[j].stack--;
                        success = true;
                        break;
                    }
                }
                if (!success)
                {
                    Projectile.Kill();
                    return;
                }
                SoundEngine.PlaySound(SoundID.Item11.WithPitchOffset(0.75f - (Projectile.ai[0] * 0.02f)), player.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 10, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)) * Main.rand.NextFloat(20, 25), ProjectileType<CecitiorTeethFriendly>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
                Projectile.ai[1] = 0;
            }
        }
    }
}
