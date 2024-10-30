using EbonianMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using static EbonianMod.Helper;
using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Magic
{
    internal class CursedToy : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 500;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.reuseDelay = 25;
            Item.shoot = ModContent.ProjectileType<CursedToyP>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = 5;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 350);
            return false;
        }
    }
    public class CursedToyP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/CursedToy";
        float holdOffset = 20;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
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
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.1f - MathHelper.Lerp(0.09f, 0f, Projectile.ai[0] / 350)).SafeNormalize(Vector2.UnitX);


        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
