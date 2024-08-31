using EbonianMod.Projectiles.VFXProjectiles;
using EbonianMod.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Collections;
using EbonianMod.Dusts;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria.Audio;
using EbonianMod.Common.Systems;

namespace EbonianMod.Items.Weapons.Melee
{
    public class PhantasmalGreatsword : ModItem
    {

        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 168;
            Item.height = 178;
            Item.crit = 40;
            Item.damage = 15;
            Item.useAnimation = 2;
            Item.useTime = 2;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.channel = true;
            //Item.reuseDelay = 45;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<PhantasmalGreatswordP>();
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
    public class PhantasmalGreatswordP : HeldSword
    {
        public override string Texture => "EbonianMod/Items/Weapons/Melee/PhantasmalGreatsword";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
            EbonianMod.projectileFinalDrawList.Add(Type);
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetExtraDefaults()
        {
            Projectile.width = 168;
            Projectile.height = 178;
            useHeld = false;
            swingTime = 140;
            Projectile.extraUpdates = 4;
            holdOffset = 120;
        }
        bool hit, summoned;
        public override void OnHit(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Player player = Main.player[Projectile.owner];
            if (!hit)
            {
                int b = (hitinfo.Crit ? 3 : 2);
                for (int i = 0; i < b; i++)
                {
                    float angle = Helper.CircleDividedEqually(i, b) + MathHelper.PiOver2;
                    if (b == 3)
                    {
                        angle = Helper.CircleDividedEqually(i, b + 1) + MathHelper.Pi + MathHelper.PiOver2;
                    }
                    Projectile.NewProjectile(null, target.Center + Helper.FromAToB(player.Center, target.Center).RotatedBy(angle) * (target.Size.Length() + Projectile.Size.Length() / 2), -Projectile.velocity.RotatedBy(angle), ModContent.ProjectileType<PhantasmalGreatswordP2>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.ai[1]);
                }
                hit = true;
            }
        }
        public override void ExtraAI()
        {
            Player player = Main.player[Projectile.owner];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float rot = Projectile.rotation - MathHelper.PiOver4;
            Vector2 start = player.Center;
            Vector2 end = player.Center + rot.ToRotationVector2() * (Projectile.height + holdOffset * 0.575f);
            Vector2 offset = (Projectile.Size / 2) + ((Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * holdOffset * 0.25f);

            int direction = (int)Projectile.ai[1];
            float defRot = Projectile.velocity.ToRotation();
            float _start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float _end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rotation = direction == 1 ? _start + MathHelper.Pi * 3 / 2 : _end - MathHelper.Pi * 3 / 2;
            Vector2 position = player.GetFrontHandPosition(stretch, rotation - MathHelper.PiOver2) +
                    rotation.ToRotationVector2() * holdOffset; //Final swing position

            if (swingProgress.CloseTo(0.5f, 0.35f))
            {

                if (Projectile.timeLeft % 3 == 0)
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 pos = Vector2.Lerp(start, end, Main.rand.NextFloat());
                        //Dust.NewDustPerfect(pos, ModContent.DustType<SparkleDust>(), Helper.FromAToB(pos, player.Center + Helper.FromAToB(player.Center, pos, false).RotatedBy(-Projectile.ai[1] * 0.5f)) * 5, 0, Color.Indigo, Main.rand.NextFloat(0.1f, 0.15f)).noGravity = true;

                        Dust.NewDustPerfect(pos, ModContent.DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, player.Center + Helper.FromAToB(player.Center, pos, false).RotatedBy(-Projectile.ai[1] * 0.5f)) * 5, 0, Color.Indigo, Main.rand.NextFloat(0.1f, 0.15f)).customData = position;
                    }

            }
            alpha = MathHelper.Clamp((float)Math.Sin(swingProgress * Math.PI) * 3f, 0, 1);
        }
        float alpha;
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(EbonianSounds.magicSlash, Projectile.Center);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Main.spriteBatch.Reload(BlendState.Additive);


            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float s = 1;
            if (Projectile.oldPos.Length > 2)
            {
                Texture2D tex2 = Helper.GetExtraTexture("fireball");
                VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(Projectile.oldPos.Length - 1) * 6];
                for (int i = 1; i < Projectile.oldPos.Length - 3; i++)
                {
                    if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                    {
                        float rot1 = Projectile.oldRot[i] - MathHelper.PiOver4;
                        float rot2 = Projectile.oldRot[i + 1] - MathHelper.PiOver4;
                        Vector2 start = player.Center + rot1.ToRotationVector2() * (Projectile.height + holdOffset * 0.25f);
                        Vector2 end = player.Center + rot2.ToRotationVector2() * (Projectile.height + holdOffset * 0.25f);
                        for (int j = 0; j < 4; j++)
                        {
                            Vector2 pos = Vector2.Lerp(start, end, (float)j / 4);
                            Main.spriteBatch.Draw(tex2, pos - Main.screenPosition, null, Color.Indigo * (alpha * s * 0.2f), Helper.FromAToB(start, end).ToRotation() - MathHelper.PiOver2, tex2.Size() / 2, s * 0.5f, SpriteEffects.None, 0);
                        }

                        s -= i / (float)Projectile.oldPos.Length * 0.03f;
                    }
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);


            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, Projectile.rotation + (Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            if (glowAlpha > 0 && glowBlend != null)
            {
                Texture2D glow = Helper.GetTexture(GlowTexture);
                Main.spriteBatch.Reload(glowBlend);
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White * glowAlpha, Projectile.rotation + (Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver2 * 3), glow.Size() / 2, Projectile.scale, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                Main.spriteBatch.Reload(BlendState.AlphaBlend);
            }
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D slash = Helper.GetExtraTexture("Extras2/slash_06_half");
            float mult = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            if (mult > 0.5f)
                slash = Helper.GetExtraTexture("Extras2/slash_06");
            Vector2 pos = player.Center + Projectile.velocity * 90;
            /*Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.Indigo * (alpha * 0.5f), Projectile.velocity.ToRotation(), slash.Size() / 2, Projectile.scale * 1.75f, Projectile.ai[1] == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);

            slash = Helper.GetExtraTexture("Extras2/slash_02");
            Main.spriteBatch.Draw(slash, pos - Main.screenPosition, null, Color.Indigo * (alpha * 0.5f), Projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);*/
        }
    }
    public class PhantasmalGreatswordP2 : ModProjectile
    {
        public int swingTime = 20;
        public bool modifyCooldown;
        public float holdOffset = 50f;
        public float baseHoldOffset = 50f;
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            SetExtraDefaults();
            if (!modifyCooldown)
                Projectile.localNPCHitCooldown = swingTime;
            Projectile.timeLeft = swingTime;
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public void SetExtraDefaults()
        {
            Projectile.width = 168;
            Projectile.height = 178;
            swingTime = 55 * 4;
            Projectile.extraUpdates = 4;
            holdOffset = 115;
        }
        public virtual float Ease(float f)
        {
            return 1 - (float)Math.Pow(2, 10 * f - 10);
        }
        public virtual float ScaleFunction(float progress)
        {
            return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
        }
        Vector2 startP;
        public override void AI()
        {
            if (startP == Vector2.Zero)
                startP = Projectile.Center;
            float direction = Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            Vector2 position = startP +
                rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - startP).ToRotation() + MathHelper.PiOver4;

            Projectile.ai[2] = MathHelper.Clamp((float)Math.Sin(swingProgress * Math.PI) * 2f, 0, 1);

        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Main.spriteBatch.Reload(BlendState.Additive);


            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float s = 1;
            if (Projectile.oldPos.Length > 2)
            {
                Texture2D tex2 = Helper.GetExtraTexture("fireball");
                VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(Projectile.oldPos.Length - 1) * 6];
                for (int i = 1; i < Projectile.oldPos.Length - 3; i++)
                {
                    if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                    {
                        float rot1 = Projectile.oldRot[i] - MathHelper.PiOver4;
                        float rot2 = Projectile.oldRot[i + 1] - MathHelper.PiOver4;
                        Vector2 start = startP + rot1.ToRotationVector2() * (Projectile.height + holdOffset * 0.25f);
                        Vector2 end = startP + rot2.ToRotationVector2() * (Projectile.height + holdOffset * 0.25f);
                        for (int j = 0; j < 4; j++)
                        {
                            Vector2 pos = Vector2.Lerp(start, end, (float)j / 4);
                            Main.spriteBatch.Draw(tex2, pos - Main.screenPosition, null, Color.Indigo * (Projectile.ai[2] * s * 0.13f), Helper.FromAToB(start, end).ToRotation() - MathHelper.PiOver2, tex2.Size() / 2, s * 0.5f, SpriteEffects.None, 0);
                        }

                        s -= i / (float)Projectile.oldPos.Length * 0.03f;
                    }
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.Indigo * Projectile.ai[2], Projectile.rotation + (Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
}

