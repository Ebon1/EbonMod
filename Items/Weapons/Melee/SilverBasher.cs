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
            Item.useAnimation = 50;
            Item.crit = 15;
            Item.useTime = 50;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 73;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 8, 0, 0);
            Item.shoot = ProjectileType<SilverBasherP>();
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.SilverBar, 20).AddIngredient(ItemID.Ruby, 1).AddIngredient(ItemID.SoulofNight, 5).AddTile(TileID.MythrilAnvil).Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 0);
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
            Projectile.timeLeft = 350;
            Projectile.tileCollide = true;
            Projectile.velocity = Helper.FromAToB(Main.player[Projectile.owner].Center, Main.MouseWorld)*300;
            Projectile.frame = 0;

        }
        bool Exploded;
        float Rotation=1.4f;

        public override void OnSpawn(IEntitySource source)
        {
            Exploded = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, TorchID.Torch);
            Player player = Main.player[Projectile.owner];
            foreach(Projectile projectile in Main.ActiveProjectiles)
            {
                if(Projectile.Distance(projectile.Center)<45 && projectile != Projectile && Type == projectile.type)
                {
                    Projectile.velocity = Helper.FromAToB(Projectile.Center, projectile.Center)*-projectile.velocity.Length()*1.3f;
                    projectile.velocity *= 0.14f;
                }
            }
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(0, 0), 0.05f);
            if(Exploded == false)
            {
                if(Main.mouseRight && player.active)
                {
                    Exploded = true;
                    foreach(Projectile projectile in Main.ActiveProjectiles)
                    {
                        if(Type == projectile.type)
                        {
                            Projectile.NewProjectile(null, projectile.Center, Vector2.Zero, ProjectileType<SilverBasherFlash>(), 0, 0);
                            projectile.timeLeft=50;
                            projectile.frame = 1;
                            Projectile.damage = 0;
                        }
                    }
                }
            }
            else
            {
                Rotation = Lerp(Rotation, 0f, 0.1f);
            }
            Projectile.rotation += Rotation;
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
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
        float Rotation=0.7f;
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.rotation += Rotation;
            Rotation = Lerp(Rotation, 0, 0.1f);
            if(Projectile.ai[0]>6)
            {
                Projectile.Opacity = Lerp(Projectile.Opacity, 0, 0.15f); 
            }
            if(Projectile.Opacity>0.7f)
            {
                Lighting.AddLight(Projectile.Center, TorchID.Torch);
            }
        }
    }
}
