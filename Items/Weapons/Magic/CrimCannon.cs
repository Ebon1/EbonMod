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
using EbonianMod.Projectiles.Friendly.Crimson;

namespace EbonianMod.Items.Weapons.Magic
{
    public class CrimCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 5;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = 0;
            Item.rare = 2;
            Item.mana = 5;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CrimCannonP>();
            Item.shootSpeed = 20;
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
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D a = Helper.GetExtraTexture("explosion");
            Main.spriteBatch.Reload(BlendState.Additive);
            var fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
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
                //Projectile.velocity *= 0.5f;
                //Projectile.aiStyle = -1;
            }
            if (initCenter != Vector2.Zero)
                Projectile.SineMovement(initCenter, initVel, 0.15f, 60);

        }
    }
}
