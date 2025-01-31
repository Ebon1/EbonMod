using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Items.Weapons.Melee;
using Terraria.GameContent;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class EbonianGun : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 48;
            Item.height = 66;
            Item.crit = 15;
            Item.damage = 29;
            Item.useAnimation = 10;
            Item.useTime = 20;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Ranged;
            //Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<EbonianGunP>();
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ShadowScale, 15).AddIngredient(ItemID.FlintlockPistol).AddTile(TileID.Anvils).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback);

            return false;
        }
        public override void SetStaticDefaults()
        {
            //Tooltip.SetDefault("");
        }
    }
    public class EbonianGunP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Ranged/EbonianGun";
        public virtual float Ease(float f)
        {
            return 1 - (float)Math.Pow(2, 10 * f - 10);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        float holdOffset;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.Size = new(28, 24);
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 25;
            holdOffset = 22;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                return;
            }
            float progress = Ease(Utils.GetLerpValue(0f, 15, Projectile.timeLeft));
            if (Projectile.timeLeft == 1 && player.ownedProjectileCounts[Type] < 1)
            {
                if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center).RotatedByRandom(0.5f);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, ai2: 1);
                        proj.rotation = dir.ToRotation();
                        proj.Center = Projectile.oldPos[3] + Projectile.Size;
                        proj.timeLeft = 45;
                    }
                }
            }
            if (Projectile.timeLeft == 15)
            {
                SoundEngine.PlaySound(SoundID.Item11);
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center - Projectile.velocity * 15, Projectile.velocity * 20, ProjectileID.CursedBullet, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (Projectile.timeLeft > 10 && Projectile.timeLeft < 15)
            {
                holdOffset--;
                if (Projectile.direction == -1)
                {
                    Projectile.ai[0] += MathHelper.ToRadians(4.5f / 2);
                }
                else
                {
                    Projectile.ai[0] -= MathHelper.ToRadians(4.5f / 2);
                }
            }
            else if (Projectile.timeLeft < 10)
            {
                if (Projectile.direction == -1)
                {
                    Projectile.ai[0] -= MathHelper.ToRadians(.9f);
                }
                else
                {
                    Projectile.ai[0] += MathHelper.ToRadians(.9f);
                }
            }
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation() + Projectile.ai[0]).ToRotationVector2() * holdOffset;
            if (player.gravDir != -1)
                if (Projectile.ai[2] == 0)
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 + Projectile.ai[0]);
                else
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2 + Projectile.ai[0]);
            Projectile.rotation = (pos - player.Center).ToRotation() + Projectile.ai[0] * Projectile.spriteDirection;
            Projectile.Center = pos - Vector2.UnitY * 2;
            if (player.itemTime <= 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            if (player.HeldItem.type != ItemType<EbonianGun>()) { Projectile.Kill(); }
            if (Projectile.ai[2] == 0)
                player.heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, effects, 0);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    if (Projectile.ai[2] == 1)
                    {
                        dir = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.MouseWorld - player.Center), 0.75f);
                        dir.Normalize();
                    }
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, ai2: Projectile.ai[2]);
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                }
            }
        }
    }
}
