using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent;
using EbonianMod.Projectiles.Terrortoma;
using EbonianMod.Projectiles.Minions;

namespace EbonianMod.Items.Weapons.Magic
{
    public class TerrorStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.width = 40;
            Item.height = 40;
            Item.mana = 10;
            Item.reuseDelay = 20;
            Item.useTime = 10;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = 5;
            Item.knockBack = 10; Item.rare = 8;
            Item.value = 1000;
            Item.rare = 2;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TerrorStaffP>();
            Item.shootSpeed = 14;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pointPoisition = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
            float num2 = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            float num3 = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y;
            Vector2 vector5 = new Vector2(num2, num3);
            vector5.X = (float)Main.mouseX + Main.screenPosition.X - pointPoisition.X;
            vector5.Y = (float)Main.mouseY + Main.screenPosition.Y - pointPoisition.Y - 1000f;
            player.itemRotation = (float)Math.Atan2(vector5.Y * (float)player.direction, vector5.X * (float)player.direction);
            NetMessage.SendData(13, -1, -1, null, player.whoAmI);
            NetMessage.SendData(41, -1, -1, null, player.whoAmI);

            Projectile.NewProjectile(source, position, new Vector2(velocity.X * 0.5f * Main.rand.NextFloat(1, 2), -Item.shootSpeed), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class TerrorStaffP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = 2;
        }
        /*public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            DrawData data = new DrawData(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            MiscDrawingMethods.DrawWithDye(Main.spriteBatch, data, ItemID.GreenFlameDye, Projectile);
            return false;
        }*/
        public override void Kill(int timeLeft)
        {
            //Helper.DustExplosion(Projectile.Center, Projectile.Size, 0, Color.Green);

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Unit() * 10, ModContent.ProjectileType<EbonFlyMinion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitY, 1000) < Projectile.height)
            {
                Projectile.velocity.Y = 0;
                return false;
            }
            if ((Helper.TRay.CastLength(Projectile.Center, Vector2.UnitX, 1000) < Projectile.width || Helper.TRay.CastLength(Projectile.Center, -Vector2.UnitX, 1000) < Projectile.width) && Helper.TRay.CastLength(Projectile.Center, Vector2.UnitY, 1000) > Projectile.height)
            {
                Projectile.velocity.X = 0;
                return false;
            }
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Helper.TRay.Cast(Projectile.Center, Vector2.UnitY, Main.screenWidth), Vector2.Zero, ModContent.ProjectileType<TExplosion>(), 0, 0);
            Terraria.Audio.SoundEngine.PlaySound(new("EbonianMod/Sounds/Eggplosion"), Projectile.Center);
            return true;
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame == 0) Projectile.frame++;
                else Projectile.frame = 0;
            }
            Projectile.velocity.Y += 0.25f;
        }
    }
}
