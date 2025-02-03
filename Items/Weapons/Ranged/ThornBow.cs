using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Items.Materials;
namespace EbonianMod.Items.Weapons.Ranged
{
    public class ThornBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 60;
            Item.crit = 10;
            Item.damage = 80;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<ThornBowP>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.EbonwoodBow).AddIngredient(ItemType<TerrortomaMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ProjectileType<ThornBowP>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
        public override bool? CanAutoReuseItem(Player player) => false;
    }
    public class ThornBowP : ModProjectile
    {
        const float holdOffset = 20;
        const int maxTime = 55;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.Size = new(26, 60);
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = maxTime;
        }
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public virtual float Ease(float x)
        {
            return 1 - MathF.Pow(1 - x, 5);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float off = MathHelper.Lerp(2, 0, Projectile.ai[2] / 30);
            if (Projectile.ai[2] > 30)
                off = 0;
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - new Vector2(8, 20).RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + Projectile.ai[2], -off).RotatedBy(Projectile.rotation), new Color(113, 132, 77), new Color(113, 132, 77), 2);
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - new Vector2(10, -16).RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + off + Projectile.ai[2], off).RotatedBy(Projectile.rotation), new Color(113, 132, 77), new Color(113, 132, 77), 2);
            return true;
        }
        float alpha;
        SoundStyle pull = EbonianSounds.bowPull with
        {
            PitchVariance = 0.25f,
        };
        SoundStyle release = EbonianSounds.bowRelease with
        {
            PitchVariance = 0.25f,
        };
        public override void PostDraw(Color lightColor)
        {
            Texture2D tex = Helper.GetTexture("Projectiles/Friendly/Corruption/CorruptArrow");
            Main.spriteBatch.Draw(tex, Projectile.Center + Vector2.Lerp(new Vector2(20, 0).RotatedBy(Projectile.rotation), Vector2.Zero, alpha) - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                if (Projectile.timeLeft == 1 && player.channel && !(!player.active || player.dead || player.CCed || player.noItems || !player.channel || player.HeldItem.type != ItemType<ThornBow>()))
                {
                    Projectile.timeLeft = maxTime;
                }
            }
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (player.HeldItem.type != ItemType<ThornBow>()) { Projectile.Kill(); }
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (player.gravDir != -1)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            if (player.gravDir != -1)
                player.SetCompositeArmBack(true, Projectile.timeLeft > 50 || Projectile.timeLeft < 5 ? Player.CompositeArmStretchAmount.Quarter : Player.CompositeArmStretchAmount.None, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            if (Projectile.timeLeft > 25)
            {
                if (Projectile.timeLeft == maxTime - 1)
                {
                    bool success = false;
                    for (int j = 0; j < 58; j++)
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
                }
                if (Projectile.timeLeft == maxTime - 3)
                    SoundEngine.PlaySound(pull, Projectile.Center);
                float progress = Ease(Utils.GetLerpValue(0f, maxTime - 25, Projectile.timeLeft));
                alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
                if (Projectile.timeLeft > 50 && Projectile.timeLeft < maxTime - 1)
                    Projectile.ai[2]++;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.5f - MathHelper.Lerp(0, 0.2f, Projectile.timeLeft / 30)).SafeNormalize(Vector2.UnitX);
            }
            else
            {
                if (Projectile.timeLeft == 24)
                {
                    SoundEngine.PlaySound(release, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 20, ProjectileType<CorruptArrow>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

                    alpha = 0;
                }
                if (Projectile.timeLeft > 20)
                    Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], -5, 0.4f);
                else
                    Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.2f);
            }
        }
    }
}

