using EbonianMod.Projectiles.VFXProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EbonianMod.Items.Weapons.Melee
{
    public class Exolsaw : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.Size = new(20);
            Item.useAnimation = 17;
            Item.crit = 20;
            Item.useTime = 17;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 60;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ExolsawP>();
            Item.rare = ItemRarityID.LightPurple;
            Item.shootSpeed = 16f;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<ExolsawP>()] < 10;
        }
    }
    public class ExolsawP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/Exolsaw";
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.aiStyle = 3;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GenericExplosion>(), 20, 0);
                a.hostile = true;
                a.friendly = false;
            }
            target.AddBuff(BuffID.OnFire, 150);
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        }
    }
}
