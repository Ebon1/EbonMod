using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class EbonianGatling : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 2;
            Item.useTime = Item.useAnimation = 5;
            Item.shoot = ModContent.ProjectileType<EbonianGatlingP>();
            Item.shootSpeed = 8f;
            Item.rare = 2;
            Item.useStyle = 5;
            Item.UseSound = SoundID.Item11;
            Item.useAmmo = AmmoID.Bullet;
            Item.autoReuse = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.DemoniteBar, 20).AddTile(TileID.Anvils).Register();
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextBool(2);
        public override Vector2? HoldoutOffset() => new(-10, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<EbonianGatlingP>();
            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
    public class EbonianGatlingP : ModProjectile
    {
        public override string Texture => "EbonianMod/Extras/EbonianGatlingBullet";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = 0;
            Projectile.timeLeft = 500;
            Projectile.Size = new(30, 10);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Reload(BlendState.Additive);
            Texture2D tex = Helper.GetExtraTexture("EbonianGatlingBullet");
            spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
            spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 30);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch);
            Projectile.velocity *= 1.025f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
    }
}
