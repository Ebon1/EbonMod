using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using EbonianMod;

namespace EbonianMod.Projectiles
{
    public abstract class HeldSword : ModProjectile
    {
        public int swingTime = 20;
        public float holdOffset = 50f;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            SetExtraDefaults();
            Projectile.localNPCHitCooldown = swingTime;
            Projectile.timeLeft = swingTime;
        }
        public virtual float Ease(float f)
        {
            return 1 - (float)Math.Pow(2, 10 * f - 10);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        public virtual void SetExtraDefaults()
        {

        }
        public virtual void ExtraAI()
        {

        }
        public Player.CompositeArmStretchAmount stretch = Player.CompositeArmStretchAmount.Full;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                return;
            }
            if (Projectile.ai[1] != 0)
            {
                int direction = (int)Projectile.ai[1];
                float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                float defRot = Projectile.velocity.ToRotation();
                float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
                Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                    rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
                Projectile.Center = position;
                Projectile.rotation = (position - player.Center).ToRotation() + MathHelper.PiOver4;
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.heldProj = Projectile.whoAmI;
                player.SetCompositeArmFront(true, stretch, rotation - MathHelper.PiOver2);
            }
            else
            {
                float progress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
                holdOffset = 35 * (progress + 0.25f);
                Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
                player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                player.itemRotation = Projectile.velocity.ToRotation() * player.direction;
                pos += Projectile.velocity.ToRotation().ToRotationVector2() * holdOffset;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
                Projectile.rotation = (pos - player.Center).ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
                Projectile.Center = pos;
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            if (player.itemTime < 2)
            {
                player.itemTime = 2;
                player.itemAnimation = 2;
            }
            ExtraAI();
        }
        public override void Kill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, (-Projectile.ai[1]));
                    proj.rotation = Projectile.rotation;
                    proj.Center = Projectile.Center;
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public float glowAlpha;
        public BlendState glowBlend;
        public override bool PreDraw(ref Color lightColor)
        {
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Helper.GetTexture(GlowTexture);
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), lightColor, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
            if (glowAlpha > 0 && glowBlend != null)
            {
                Main.spriteBatch.Reload(glowBlend);
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White * glowAlpha, Projectile.rotation, orig, Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player player = Main.player[Projectile.owner];
            float rot = Projectile.rotation - MathHelper.PiOver4;
            Vector2 start = player.Center;
            Vector2 end = player.Center + rot.ToRotationVector2() * (Projectile.height + holdOffset * 0.8f);
            float a = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, projHitbox.Width, ref a) && Collision.CanHitLine(player.TopLeft, player.width, player.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height);
        }
    }
}
