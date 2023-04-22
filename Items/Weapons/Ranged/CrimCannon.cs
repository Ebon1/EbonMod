using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using EbonianMod.Projectiles;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
namespace EbonianMod.Items.Weapons.Ranged
{
    public class CrimCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Maw");
            Tooltip.SetDefault("Sprays blood on enemies\nEvery 10th attack also shoots out a blood helix that deals 4x damage.");
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = 5;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 0;
            Item.rare = 2;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BloodArrow;
            Item.shootSpeed = 8f;
        }
        int uses = -1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            uses++;
            if (uses >= 10)
            {
                for (int i = -1; i < 2; i++)
                {
                    if (i == 0)
                        continue;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BloodOrb>(), damage * 4, knockBack, player.whoAmI, i);
                }
                uses = 0;
            }
            int numberProjectiles = 2;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(35));
                Projectile a = Projectile.NewProjectileDirect(source, position, perturbedSpeed, type, damage, knockBack, player.whoAmI);
                a.friendly = true;
                a.hostile = false;
                a.penetrate = 1;
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return (new Vector2(-3, 0));
        }
    }
    public class BloodOrb : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 50;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.DarkRed * alpha * (1f - fadeMult * i), 0, a.Size() / 2, 0.1f * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.DarkRed * alpha, 0, a.Size() / 2, 0.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(a, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Black * alpha * 0.5f * (1f - fadeMult * i), 0, a.Size() / 2, 0.1f * (1f - fadeMult * i), SpriteEffects.None, 0);
            }
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.Black * alpha * 0.5f, 0, a.Size() / 2, 0.1f, SpriteEffects.None, 0);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
        }
        /*public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.timeLeft > 60)
                Projectile.velocity = -oldVelocity;
            else
                Projectile.velocity = Vector2.Zero;
            return false;
        }*/
        Vector2 initCenter, initVel;
        float alpha = 1;
        public override void AI()
        {
            if (Projectile.timeLeft == 299)
            {
                initCenter = Projectile.Center;
                initVel = Projectile.velocity;
            }
            if (Projectile.timeLeft < 60)
            {
                if (alpha > 0)
                    alpha -= 0.025f;
                Projectile.velocity *= 0.5f;
                Projectile.aiStyle = -1;
            }
            else
            {
                if (initCenter != Vector2.Zero)
                    Projectile.SineMovement(initCenter, initVel, 0.15f, 60);
            }

        }
    }
}
