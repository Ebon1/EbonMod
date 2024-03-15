using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles;
using System.Collections.Generic;
using Terraria.Audio;
using System.Collections.ObjectModel;

namespace EbonianMod.Items.Weapons.Melee
{
    public class EbonianScythe : ModItem
    {
        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 32;
            Item.height = 34;
            Item.crit = 45;
            Item.damage = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1;
            Item.shoot = ModContent.ProjectileType<EbonianScytheP>();
        }
        int dir = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir, 1);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }
    }
    public class EbonianScytheP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/EbonianScythe";

        public override void SetExtraDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            swingTime = 10;
            holdOffset = 15;
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            int direction = (int)Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D slash = Helper.GetExtraTexture("Extras2/slash_06_half");
            float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            if (mult > 0.5f)
                slash = Helper.GetExtraTexture("Extras2/slash_06");
            Vector2 pos = player.Center + Projectile.velocity * 20f;
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, new Color(152, 187, 63) * alpha * 0.5f, Projectile.velocity.ToRotation(), slash.Size() / 2, Projectile.scale / 3.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, (Projectile.ai[0] > 5 ? -1 : Projectile.ai[0]) + 1, (-Projectile.ai[1]));
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                }
            }
        }
    }
}

