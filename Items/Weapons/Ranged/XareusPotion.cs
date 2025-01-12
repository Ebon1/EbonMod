using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using EbonianMod.Projectiles.Friendly.Corruption;
using Terraria.DataStructures;

using EbonianMod.Projectiles.VFXProjectiles;
using Terraria.Audio;
using EbonianMod.Projectiles.Friendly.Underworld;
using Terraria.UI;
using System.IO;
using Terraria.GameContent;
using EbonianMod.Projectiles.ArchmageX;
using EbonianMod.Common.Systems;
using EbonianMod.Dusts;

namespace EbonianMod.Items.Weapons.Ranged
{
    public class XareusPotion : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<XareusPotionPro>();
            Item.shootSpeed = 14;
        }
    }

    public class XareusPotionPro : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(20, 26);
            Projectile.friendly = true;
            Projectile.aiStyle = 2;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) => Color.White * 0.9f;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.Glass, Main.rand.NextVector2Circular(Projectile.velocity.Length(), Projectile.velocity.Length()));
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<XareusPotionExplosion>(), Projectile.damage, 0);
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
        }
    }

    public class XareusPotionExplosion : ModProjectile
    {
        public override string Texture => Helper.Placeholder;
        public override void SetStaticDefaults()
        {
            EbonianMod.projectileFinalDrawList.Add(Type);
        }
        public override void SetDefaults()
        {
            Projectile.height = 100;
            Projectile.width = 100;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 200;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            float alpha2 = MathHelper.Lerp(0.5f, 0, Projectile.ai[0]);
            Texture2D ring = Helper.GetExtraTexture("ring");
            Texture2D explosion = Helper.GetExtraTexture("explosion");
            Texture2D flameEye2 = Helper.GetExtraTexture("flameEye2");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(explosion, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha2 * 2, Projectile.rotation, explosion.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha, Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 1.1f * 2, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.Indigo * alpha * 0.5f, Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 4f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.xSpirit.WithPitchOffset(-0.5f), Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150 * Projectile.ai[0], 150 * Projectile.ai[0]), ModContent.DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 0.5f)
                Projectile.Kill();
        }
    }
}
