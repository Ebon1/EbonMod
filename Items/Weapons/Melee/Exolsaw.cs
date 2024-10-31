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
            Item.damage = 27;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ExolsawP>();
            Item.rare = ItemRarityID.Orange;
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
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;

        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(15);

            //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3));
            Player player = Main.player[Projectile.owner];
            if (player.active)
            {
                Projectile.timeLeft = 2;
                Projectile.ai[0]++;
                if (Projectile.ai[0] < 30)
                    Projectile.velocity *= 1.02f;
                if (Projectile.ai[0] > 30 && Projectile.ai[0] < 50)
                    Projectile.velocity *= 0.86f;
                if (Projectile.ai[0] > 50)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, player.Center) * 30f, 0.1f);
                if (Projectile.ai[0] > 50 && Projectile.Center.Distance(player.Center) < 50)
                    Projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            if (hitinfo.Crit)
            {
                Projectile a = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileID.DaybreakExplosion, 20, 0);
                a.hostile = false;
                a.friendly = true;
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
