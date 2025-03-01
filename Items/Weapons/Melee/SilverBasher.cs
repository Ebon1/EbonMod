using EbonianMod.Items.Materials;
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
    public class SilverBasher : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.consumable = false;
            Item.Size = new(20);
            Item.useAnimation = 20;
            Item.crit = 15;
            Item.useTime = 20;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 73;
            Item.reuseDelay = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.shoot = ProjectileType<SilverBasherP>();
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SilverBar, 20).AddIngredient(ItemID.Ruby, 1).AddIngredient(ItemID.SoulofNight, 5).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 1.4f);
            return false;
        }
    }
    public class SilverBasherP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = true;

        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = 1.4f;
        }
        public override void AI()
        {
            if (Projectile.ai[2] == 0)
                Projectile.ai[2] = Projectile.velocity.Length();
            Lighting.AddLight(Projectile.Center, TorchID.Red);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.025f);

            Projectile.ai[1] = Lerp(Projectile.ai[1], 0f, 0.01f);
            Projectile.rotation += Projectile.ai[1];
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.ai[2] * 0.5f;
            if (Projectile.ai[0] == 0)
            {
                for (int i = -1; i < 2; i++)
                {
                    float angle = i * 0.7f;
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, vel.RotatedBy(angle), Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1, 1.4f);
                }
            }
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<SilverBasherFlash>(), 0, 0);
            Projectile Proj = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), Projectile.damage, 100, Projectile.owner);
            Proj.friendly = true;
            Proj.hostile = false;
        }
    }
    public class SilverBasherFlash : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.height = 66;
            Projectile.width = 66;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 35;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[1] = 0.7f;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation += Projectile.ai[1];
            Projectile.ai[1] = Lerp(Projectile.ai[1], 0, 0.1f);
            if (Projectile.ai[0] > 6)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.15f);
            }
            if (Projectile.Opacity > 0.7f)
            {
                Lighting.AddLight(Projectile.Center, TorchID.Torch);
            }
        }
    }
}
