using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using EbonianMod.Common.Systems;
using EbonianMod.Projectiles.Friendly.Underworld;
using Terraria.Audio;
using EbonianMod.Projectiles.Cecitior;
using EbonianMod.Items.Materials;
using EbonianMod.Projectiles.Friendly.Crimson;
namespace EbonianMod.Items.Weapons.Ranged
{
    public class FangSlinger : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 60;
            Item.crit = 10;
            Item.damage = 23;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Green;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<FangSlingerP>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.ShadewoodBow).AddIngredient(ItemID.TissueSample, 20).AddTile(TileID.Anvils).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ProjectileType<FangSlingerP>(), damage, knockback, player.whoAmI);
            return false;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
    public class FangSlingerP : ModProjectile
    {
        const float holdOffset = 20;
        const int maxTime = 85;
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
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - new Vector2(8, 24).RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + Projectile.ai[2], -off).RotatedBy(Projectile.rotation), new Color(162, 44, 31), new Color(118, 28, 20), 2);
            Utils.DrawLine(Main.spriteBatch, Projectile.Center - new Vector2(10, -32).RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + off + Projectile.ai[2], off).RotatedBy(Projectile.rotation), new Color(162, 44, 31), new Color(118, 28, 20), 2);
            return true;
        }
        float[] alpha = new float[3];
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
            Texture2D tex = Helper.GetTexture("Projectiles/Friendly/Crimson/CrimsonArrow");
            for (int i = -1; i < 2; i++)
            {
                if (i == 0)
                    continue;
                Main.spriteBatch.Draw(tex, Projectile.Center + Vector2.Lerp(new Vector2(20, 0).RotatedBy(Projectile.rotation), Vector2.Zero, alpha[i + 1]) - Main.screenPosition, null, lightColor * alpha[i + 1], Projectile.rotation + (i * 0.25f) + MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center + Vector2.Lerp(new Vector2(20, 0).RotatedBy(Projectile.rotation), Vector2.Zero, alpha[2]) - Main.screenPosition, null, lightColor * alpha[2], Projectile.rotation + MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0, 0);
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                if (Projectile.timeLeft == 1 && player.channel)
                {
                    Projectile.timeLeft = maxTime;
                }
            }
            if (player.HeldItem.type != ItemType<FangSlinger>()) { player.itemTime = 0; player.itemAnimation = 0; Projectile.Kill(); }
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
                for (int i = 0; i < 1 + MathHelper.Clamp((-Projectile.timeLeft + maxTime) / 5, 0, 2); i++)
                {
                    alpha[i] = MathHelper.Lerp(alpha[i], 1f, 0.1f);
                }
                if (Projectile.timeLeft > 50 && Projectile.timeLeft < maxTime - 1)
                    Projectile.ai[2] += 0.5f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.5f - MathHelper.Lerp(0, 0.2f, Projectile.timeLeft / 30)).SafeNormalize(Vector2.UnitX);
            }
            else
            {
                if (Projectile.timeLeft == 24)
                {
                    SoundEngine.PlaySound(release, Projectile.Center);
                    for (int i = -2; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(i * 0.25f) * 20, ProjectileType<CrimsonArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner, -i);
                    }
                    for (int i = 0; i < 3; i++)
                        alpha[i] = 0;
                }
                if (Projectile.timeLeft > 20)
                    Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], -5, 0.4f);
                else
                    Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.2f);
            }
        }
    }
}
