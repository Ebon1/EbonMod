using EbonianMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Audio;
using EbonianMod.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using static EbonianMod.Helper;
using EbonianMod.Projectiles.VFXProjectiles;

namespace EbonianMod.Items.Weapons.Magic
{
    internal class CursedToy : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 500;
            Item.useTime = 1;
            Item.useAnimation = 10;
            Item.reuseDelay = 25;
            Item.shoot = ModContent.ProjectileType<CursedToyP>();
            Item.shootSpeed = 1f;
            Item.rare = 2;
            Item.expert = true;
            Item.useStyle = 5;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }
        public override bool? CanAutoReuseItem(Player player) => false;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity.Normalize();
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 350);
            return false;
        }
    }
    public class CursedToyP : ModProjectile
    {
        public override string Texture => "EbonianMod/Items/Weapons/Magic/CursedToy";
        float holdOffset = 20;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.Size = new Vector2(52, 30);
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
        }
        public override bool? CanDamage() => false;
        float[] pulseAlpha = new float[3];
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
            {
                Projectile.Kill();
                return;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            Projectile.timeLeft = 10;
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
            Projectile.Center = pos;
            Projectile.rotation = Projectile.velocity.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.1f - MathHelper.Lerp(0.09f, 0f, Projectile.ai[0] / 350)).SafeNormalize(Vector2.UnitX);
            if (Projectile.ai[0] > 2)
                Projectile.ai[0]--;
            else
                Projectile.Kill();
            if (Projectile.ai[0] <= 300)
            {
                if (Projectile.ai[0] == 299)
                    SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(0.1f), player.Center);
                pulseAlpha[0] = MathHelper.Lerp(pulseAlpha[0], 1, 0.1f);
            }
            if (Projectile.ai[0] <= 200)
            {
                if (Projectile.ai[0] == 199)
                    SoundEngine.PlaySound(EbonianSounds.cursedToyCharge, player.Center);
                pulseAlpha[1] = MathHelper.Lerp(pulseAlpha[1], 1, 0.1f);
            }
            if (Projectile.ai[0] <= 100 && Projectile.ai[0] > 50)
            {
                if (Projectile.ai[0] == 99)
                    SoundEngine.PlaySound(EbonianSounds.cursedToyCharge.WithPitchOffset(-0.1f), player.Center);
                pulseAlpha[2] = MathHelper.Lerp(pulseAlpha[2], 1, 0.1f);
            }
            if (Projectile.ai[0] == 50)
            {
                EbonianSystem.ScreenShakeAmount = 15f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<BigGreenShockwave>(), 0, Projectile.knockBack, Projectile.owner);

                for (int i = -3; i < 4; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, Projectile.velocity.RotatedBy(i * 0.05f), ModContent.ProjectileType<CursedToyBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                SoundEngine.PlaySound(EbonianSounds.chargedBeamImpactOnly, player.Center);
            }
            if (Projectile.ai[0] <= 40)
            {
                for (int i = 0; i < 3; i++)
                    pulseAlpha[i] = MathHelper.Lerp(pulseAlpha[i], 0, 0.2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Helper.GetExtraTexture("circlething");
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(EbonianMod.SpriteRotation);
            for (int i = 0; i < 3; i++)
            {
                Vector2 scale = new Vector2(1f, 0.25f) * 0.5f;
                EbonianMod.SpriteRotation.Parameters["scale"].SetValue(scale * 0.75f);
                EbonianMod.SpriteRotation.Parameters["rotation"].SetValue(-Main.GameUpdateCount * 0.003f);
                Vector4 col = (Color.Lerp(Color.PaleGreen, Color.Lime, (pulseAlpha[2] + pulseAlpha[1]) * 0.5f)).ToVector4();
                EbonianMod.SpriteRotation.Parameters["uColor"].SetValue(col);
                Main.spriteBatch.Draw(texture, Projectile.Center + Main.rand.NextVector2Circular(MathHelper.Lerp(0, 5, (pulseAlpha[2] + pulseAlpha[1]) * 0.5f), MathHelper.Lerp(0, 5, (pulseAlpha[2] + pulseAlpha[1]) * 0.5f)) + Projectile.velocity * (25 + i * 5) * i - Main.screenPosition, null, Color.White * pulseAlpha[i], Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, MathF.Sin(pulseAlpha[i]) * (i + 1) * 0.25f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
    public class CursedToyBeam : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 165;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 2;
        }
        int damage;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.ai[0], 60, ref a) && Projectile.scale > 0.5f;
        }
        bool RunOnce;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
            {
                Projectile.Kill();
                return;
            }
            if (!RunOnce)
            {
                Projectile.velocity.Normalize();
                damage = Projectile.damage;
                RunOnce = true;
            }
            Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
            pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * 40f;
            Projectile.Center = pos;
            //Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.1f - MathHelper.Lerp(0.09f, 0f, Projectile.ai[0] / 35)).SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(Projectile.position - new Vector2(30, 0) + Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(Projectile.ai[0]), 60, 60, DustID.CursedTorch, 2f);
                Main.dust[dust].scale = 2f;
                Main.dust[dust].velocity = Main.rand.NextVector2Circular(5, 5);
                Main.dust[dust].noGravity = true;
            }
            if (visualOffset == 0)
                visualOffset = Main.rand.NextFloat(0.75f, 1);
            visual1 += 30 * visualOffset;
            visual2 += 35 * visualOffset;

            if (Projectile.position != Projectile.oldPos[1])
            {
                float len = TRay.CastLength(Projectile.Center, Projectile.velocity, 2048);
                Vector2 vel = Projectile.velocity;
                vel.Normalize();
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + vel * (len), DustID.CursedTorch, Main.rand.NextVector2Circular(15, 15));
                    dust.scale = 2f;
                    dust.noGravity = true;
                }
                Projectile.ai[1] = len;
            }
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], .006f, 0.015f);
            Projectile.ai[0] = MathHelper.SmoothStep(Projectile.ai[0], Projectile.ai[1], 0.25f);
            Projectile.rotation = Projectile.velocity.ToRotation();

            //Projectile.velocity = -Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]));

            float progress = Utils.GetLerpValue(0, 165, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 1f);
        }
        float visualOffset;
        float visual1, visual2;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Helper.GetExtraTexture("terrortomaBeam");
            Texture2D tex2 = Helper.GetExtraTexture("oracleBeamv2");
            Texture2D tex3 = Helper.GetExtraTexture("vortex3");
            //Texture2D tex3 = Helper.GetExtraTexture("spark_06");
            Vector2 pos = Projectile.Center;
            Vector2 scale = new Vector2(1f, Projectile.scale * 0.65f);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < Projectile.ai[0]; i++)
            {
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.LawnGreen, Projectile.rotation, new Vector2(0, tex.Height / 2), scale, SpriteEffects.None, 0);
                pos += Projectile.rotation.ToRotationVector2();
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            Vector2 pos2 = Projectile.Center;

            int off = 0;
            if (Projectile.ai[0] < 512)
                off = 3;
            else if (Projectile.ai[0] < 1024)
                off = 2;
            else if (Projectile.ai[0] < 1536)
                off = 1;
            for (int i = 0; i < 4 - off; i++)
            {
                int len = 512;
                if (i == 3 && Projectile.ai[0] < 2048 && Projectile.ai[0] - 1536 > 0)
                    len = (int)Projectile.ai[0] - 1536;
                if (i == 2 && Projectile.ai[0] < 1536 && Projectile.ai[0] - 1024 > 0)
                    len = (int)Projectile.ai[0] - 1024;
                if (i == 1 && Projectile.ai[0] < 1024 && Projectile.ai[0] - 512 > 0)
                    len = (int)Projectile.ai[0] - 512;
                if (i == 0 && Projectile.ai[0] < 512)
                    len = (int)Projectile.ai[0];
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual1, 0, len, 512), Color.LawnGreen, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual1, 0, len, 512), Color.White, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.None, 0);

                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual2, 0, len, 512), Color.LawnGreen, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.FlipVertically, 0);
                Main.spriteBatch.Draw(tex2, pos2 - Main.screenPosition, new Rectangle((int)-visual2, 0, len, 512), Color.White, Projectile.rotation, new Vector2(0, tex2.Height / 2), scale, SpriteEffects.FlipVertically, 0);
                pos2 += Projectile.rotation.ToRotationVector2() * len;
            }
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Main.GameUpdateCount * 0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White, Main.GameUpdateCount * -0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.LawnGreen, Main.GameUpdateCount * -0.003f, tex3.Size() / 2, scale.Y * 0.15f, SpriteEffects.None, 0);
            float alpha = MathHelper.Lerp(-1f, 1f, (Projectile.ai[0] / Projectile.ai[1]));
            Main.spriteBatch.Draw(tex3, Projectile.Center + Projectile.ai[0] * Projectile.rotation.ToRotationVector2() - Main.screenPosition, null, Color.LawnGreen * alpha, Main.GameUpdateCount * 0.003f, tex3.Size() / 2, scale.Y * 0.25f * alpha, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex3, Projectile.Center + Projectile.ai[0] * Projectile.rotation.ToRotationVector2() - Main.screenPosition, null, Color.White * alpha, Main.GameUpdateCount * -0.03f, tex3.Size() / 2, scale.Y * 0.25f * alpha, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            return false;
        }
    }
}
