﻿using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Crimson;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;
using EbonianMod.Items.Materials;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class BallLauncher : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.useTime = 70;
            Item.value = Item.buyPrice(0, 40, 0, 0);
            Item.useAnimation = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = ItemRarityID.Red;
            Item.autoReuse = true;
            Item.shootSpeed = 14;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.useAmmo = AmmoID.Rocket;
        }


    	public override bool AltFunctionUse(Player player) 
        {
			return true;
		}

		public override bool CanUseItem(Player player) 
        {
			if (player.altFunctionUse == 2) 
            {
                Item.shoot = ProjectileType<BallLauncherCharge>();
			}
            else
            {
                Item.shoot = ProjectileType<BallLauncherSprite>();
            }
            return base.CanUseItem(player);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool? CanAutoReuseItem(Player player) => false;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.RocketLauncher).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
        }
    }
    

    public class BallLauncherSprite : ModProjectile
    {
        int frameCounter = 0;
        int frame = 0;
        Vector2 Scale = new Vector2(0, 0);

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(64, 48);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + player.direction * PiOver2;
            Projectile.scale = 0.2f;
        }

        public static float AngleLerp(float currentAngle, float targetAngle, float lerpAmount)
        {
            currentAngle = MathHelper.WrapAngle(currentAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            float difference = MathHelper.WrapAngle(targetAngle - currentAngle);
            return currentAngle + difference * lerpAmount;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.Center = new Vector2(player.Center.X, player.Center.Y-10);
            Projectile.ai[0]++;

            if (!player.active || player.dead || player.CCed || player.noItems || player.channel == false)
            {
                Projectile.Kill();
                return;
            }

            if(Projectile.ai[0] > 15)
            {
                frameCounter++;
                if (frameCounter > 5)
                {
                    if(frame == 0)
                    {
                        Scale = new Vector2(0.65f, 1.6f);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.Center.X, Projectile.Center.Y-10) + Projectile.rotation.ToRotationVector2()*30, Projectile.rotation.ToRotationVector2()*16, ProjectileType<CrimsonBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                        for (int j = 0; j < 58; j++)
                        {
                            if (player.inventory[j].ammo == AmmoID.Rocket && player.inventory[j].stack > 0)
                            {
                                if (player.inventory[j].maxStack > 1 && Projectile.ai[2] % 10 == 0)
                                    player.inventory[j].stack--;
                                break;
                            }
                        }
                    }
                    frameCounter = 0;
                    frame++;
                    if (frame > 3)
                        frame = 0;
                }
            }
            player.direction = player.Center.X - Main.MouseWorld.X < 0 ? 1 : -1;
            Projectile.rotation = AngleLerp(Projectile.rotation, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation(), 0.12f);
            Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetTexture(Mod.Name + "/Items/Weapons/Ranged/BallLauncherSprite");
            Rectangle frameRect = new Rectangle(0, frame * Projectile.height, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frameRect, lightColor, Projectile.rotation, new Vector2(Projectile.Size.X / 2-25, Projectile.Size.Y / 2), Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }

    public class BallLauncherCharge : ModProjectile
    {
        int frameCounter = 0;
        int frame = 0;
        int ChargeMeter=1;
        bool IsCharging = true; 
        Vector2 Scale = new Vector2(0, 0);
        float ScaleNoise=1f;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(58, 50);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + player.direction * PiOver2;
            Projectile.scale = 1f;
        }

        public static float AngleLerp(float currentAngle, float targetAngle, float lerpAmount)
        {
            currentAngle = MathHelper.WrapAngle(currentAngle);
            targetAngle = MathHelper.WrapAngle(targetAngle);
            float difference = MathHelper.WrapAngle(targetAngle - currentAngle);
            return currentAngle + difference * lerpAmount;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.Center = new Vector2(player.Center.X, player.Center.Y-10);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);
            Projectile.ai[0]++;

            if(IsCharging == true)
            {
                Scale = Vector2.Lerp(Scale, new Vector2(Main.rand.NextFloat(2f-ScaleNoise, ScaleNoise), Main.rand.NextFloat(2f-ScaleNoise, ScaleNoise)), ScaleNoise/10);
                if(ChargeMeter < 120)
                {
                    if(Projectile.ai[0] > 15)
                    {
                        ScaleNoise += 0.004f;
                        frameCounter++;
                        if (frameCounter > 20 && frame < 6)
                        {
                            frameCounter = 0;
                            frame++;    
                        }  
                        ChargeMeter++;
                    }
                    else
                    {
                        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);
                    }
                }
                if(!Main.mouseRight)
                {
                    frame = 6;
                    IsCharging = false;
                    for(int i=0; i < ChargeMeter/15; i++)
                    {
                        Scale = new Vector2(0.55f, 1.7f);
                        SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), new Vector2(Projectile.Center.X, Projectile.Center.Y-10) + Projectile.rotation.ToRotationVector2()*30, Main.rand.NextFloat(Projectile.rotation-(PiOver2*20/ChargeMeter), Projectile.rotation+(PiOver2*20/ChargeMeter)).ToRotationVector2() * Main.rand.NextFloat(ChargeMeter/5, ChargeMeter/8), ProjectileType<CorruptionBalls>(), Projectile.damage*2, Projectile.knockBack, Projectile.owner);
                        for (int j = 0; j < 58; j++)
                        {
                            if (player.inventory[j].ammo == AmmoID.Rocket && player.inventory[j].stack > 0)
                            {
                                if (player.inventory[j].maxStack > 1 && Projectile.ai[2] % 10 == 0)
                                    player.inventory[j].stack--;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.17f);
                frameCounter++;
                if (frameCounter > 4)
                {
                    frameCounter = 0;
                    frame++;
                    if (frame > 8)
                        Projectile.Kill();
                }
            }
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
                return;
            }
            player.direction = player.Center.X - Main.MouseWorld.X < 0 ? 1 : -1;
            Projectile.rotation = AngleLerp(Projectile.rotation, Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation(), 0.12f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetTexture(Mod.Name + "/Items/Weapons/Ranged/BallLauncherCharge");
            Rectangle frameRect = new Rectangle(0, frame * Projectile.height, Projectile.width, Projectile.height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frameRect, lightColor, Projectile.rotation, new Vector2(Projectile.Size.X / 2-25, Projectile.Size.Y / 2), Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }
}