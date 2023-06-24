using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Projectiles;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Items.Weapons.Melee
{
    public class VileBlade : ModItem
    {

        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 40;
            Item.height = 40;
            Item.crit = 10;
            Item.damage = 34;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<VileBladeP>();
        }
        int dir = 1;
        public override bool? CanAutoReuseItem(Player player)
        {
            return false;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            dir = -dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
            return false;
        }
    }
    public class VileBladeP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/VileBlade";
        public override void SetExtraDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            swingTime = 15;
            holdOffset = 15;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            if (hitinfo.Crit)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GreenShockwave>(), 0, 0);
                target.AddBuff(BuffID.CursedInferno, 60);
            }
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D slash = Helper.GetExtraTexture("Extras2/slash_02");
            float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * 25f;
            Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, new Color(152, 187, 63) * alpha * 0.5f, Projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, Projectile.scale / 3f, SpriteEffects.None, 0f);
        }
    }
}
