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
using System.Runtime.InteropServices;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;

namespace EbonianMod.Items.Weapons.Melee
{
    public class PhantasmalGreatsword : ModItem
    {

        public override void SetDefaults()
        {
            Item.knockBack = 10f;
            Item.width = 60;
            Item.height = 60;
            Item.crit = 10;
            Item.damage = 19;
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
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 1f;
            Item.shoot = ProjectileType<PhantasmalGreatswordP>();
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
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetExtraDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            useHeld = false;
            swingTime = 270;
            Projectile.extraUpdates = 5;
            holdOffset = 38;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = swingTime;
        }
        bool hit;
        public override void OnHit(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Player player = Main.player[Projectile.owner];
            if (!hit)
            {
                Projectile.ai[0] += 0.1f;
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
                        //Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(pos, player.Center + Helper.FromAToB(player.Center, pos, false).RotatedBy(-Projectile.ai[1] * 0.5f)) * 5, 0, Color.Indigo, Main.rand.NextFloat(0.1f, 0.15f)).noGravity = true;

                        Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, player.Center + Helper.FromAToB(player.Center, pos, false).RotatedBy(-Projectile.ai[1] * 0.5f)) * 5, 0, Color.Indigo, Main.rand.NextFloat(0.1f, 0.15f)).customData = position;
                    }

            }
            if (Projectile.timeLeft == 160)
            {

            }
            if (Projectile.timeLeft == 20)
            {
                if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Vector2 dir = Vector2.Normalize(Main.MouseWorld - player.Center);
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center, dir, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, (false && hit && player.ownedProjectileCounts[ProjectileType<PhantasmalGreatswordP2>()] <= 2) ? MathHelper.Clamp(Projectile.ai[0], 0f, 0.5f) : 0, (-Projectile.ai[1]));
                        proj.rotation = Projectile.rotation;
                        proj.Center = Projectile.Center;

                        if (hit && player.ownedProjectileCounts[ProjectileType<PhantasmalGreatswordP2>()] <= 2)
                        {
                            Projectile proj2 = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), player.Center + Projectile.velocity * 50, dir, ProjectileType<PhantasmalGreatswordP2>(), Projectile.damage, Projectile.knockBack, player.whoAmI, -.5f + Projectile.ai[0], (-Projectile.ai[1]));
                            proj2.rotation = Projectile.rotation;
                            proj2.Center = Projectile.Center + Projectile.velocity * 50;
                            //float _rot = PiOver4 * direction + Main.rand.NextFloat(-PiOver4 * 0.3f, PiOver4 * 0.3f);
                            //Projectile.NewProjectileDirect(null, player.Center - Projectile.velocity.RotatedBy(_rot) * holdOffset * 6, Projectile.velocity.RotatedBy(_rot) * 50, ProjectileType<PhantasmalWave>(), Projectile.damage, 0, Projectile.owner, -8, direction).timeLeft = 59;
                        }
                    }
                    Projectile.active = false;
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
        float visualOff;
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;
            visualOff -= 0.05f;
            if (visualOff <= 0)
                visualOff = 1;
            visualOff = MathHelper.Clamp(visualOff, float.Epsilon, 1 - float.Epsilon);

            Player player = Main.player[Projectile.owner];

            Main.spriteBatch.Reload(BlendState.Additive);


            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            if (Projectile.oldPos.Length > 2)
            {
                Texture2D tex2 = Helper.GetExtraTexture("TrailShape_LongSolidWavy");
                List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
                List<Vector2> oldPositions = new List<Vector2>(Projectile.oldPos.Length);
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (Projectile.oldPos[i] != Vector2.Zero)
                        oldPositions.Add(Projectile.oldPos[i]);
                    else
                        break;
                }
                if (oldPositions.Count > 3)
                {
                    if (Projectile.ai[1] == -1)
                        for (int i = 0; i < oldPositions.Count - 2; i++)
                        {
                            if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                            {
                                float s = MathHelper.SmoothStep(0, 1f, (float)i / (float)oldPositions.Count);
                                float cS = MathF.Pow(MathHelper.Lerp(1, 0, (float)i / (float)oldPositions.Count), 2);
                                float rot2 = Projectile.oldRot[i] - MathHelper.PiOver4;
                                Vector2 end = player.Center + rot2.ToRotationVector2().RotatedBy(MathHelper.ToRadians(10 * swingProgress)) * (Projectile.height + holdOffset * 0.5f);
                                Vector2 start = Vector2.Lerp(player.Center, end, Lerp(0, 1, MathF.Pow(s, 2)));
                                Color col = Color.Lerp(Color.White * 0.7f, Color.White, (float)i / (float)oldPositions.Count) * cS * alpha * Clamp(MathF.Sin(swingProgress * MathHelper.Pi) * 0.5f, 0, 1) * 2 * SmoothStep(1, 0, s) * 5 * cS;

                                float _off = MathF.Abs((s) % 1f);

                                vertices.Add(Helper.AsVertex(start - Main.screenPosition, col, new Vector2(_off, 1)));
                                vertices.Add(Helper.AsVertex(end - Main.screenPosition, col, new Vector2(_off, 0)));

                            }
                        }
                    else
                        for (int i = oldPositions.Count - 2; i > 0; i--)
                        {
                            if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                            {
                                float s = MathHelper.SmoothStep(0, 1f, (float)i / (float)oldPositions.Count);
                                float cS = MathF.Pow(MathHelper.Lerp(1, 0, (float)i / (float)oldPositions.Count), 2);
                                float rot2 = Projectile.oldRot[i] - MathHelper.PiOver4;
                                Vector2 end = player.Center + rot2.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-20 * swingProgress)) * (Projectile.height + holdOffset * 0.5f);
                                Vector2 start = Vector2.Lerp(player.Center, end, Lerp(0, 1, MathF.Pow(s, 2)));
                                Color col = Color.Lerp(Color.White * 0.7f, Color.White, s) * cS * alpha * Clamp(MathF.Sin(swingProgress * MathHelper.Pi) * 0.5f, 0, 1) * 2 * SmoothStep(1, 0, s) * 5 * cS;

                                float _off = MathF.Abs((s) % 1f);

                                vertices.Add(Helper.AsVertex(start - Main.screenPosition, col, new Vector2(_off, 1)));
                                vertices.Add(Helper.AsVertex(end - Main.screenPosition, col, new Vector2(_off, 0)));
                            }
                        }
                }
                if (vertices.Count > 3)
                {
                    for (int i = 0; i < 3; i++)
                        Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex2, false);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override float Ease(float x)
        {
            return x == 0
  ? 0
  : x == 1
  ? 1
  : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
  : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
        }
        public override void PostDraw(Color lightColor)
        {

            EbonianMod.finalDrawCache.Add(() =>
            {
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 orig = texture.Size() / 2;
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                if (glowAlpha > 0 && glowBlend != null)
                {
                    Texture2D glow = Helper.GetTexture(GlowTexture);
                    Main.spriteBatch.Reload(glowBlend);
                    Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White * glowAlpha, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), glow.Size() / 2, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
                    Main.spriteBatch.Reload(BlendState.AlphaBlend);
                }
            });

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
            Projectile.extraUpdates = 2;
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
        Vector2 startP, startP2;
        float alpha;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 10);
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 160)
            {
                //Projectile.NewProjectile(null, startP, Projectile.velocity * 20, ProjectileType<PhantasmalWave>(), Projectile.damage, 0, Projectile.owner, 1, Projectile.scale * 1.5f);

                if (Projectile.ai[2] < 4)
                    Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 50 * Projectile.scale, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 0.1f, -Projectile.ai[1], Projectile.ai[2] + 1);
            }

            Projectile.scale = 1 + Projectile.ai[0];
            if (startP == Vector2.Zero)
            {
                startP = Projectile.Center;
            }
            float direction = Projectile.ai[1];
            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            float defRot = Projectile.velocity.ToRotation();
            float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
            float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
            float rotation = direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress;
            Vector2 position = startP +
                rotation.ToRotationVector2() * holdOffset * Projectile.scale * ScaleFunction(swingProgress);
            Projectile.Center = position;
            Projectile.rotation = (position - startP).ToRotation() + MathHelper.PiOver4;
            alpha = MathHelper.Clamp((float)Math.Sin(swingProgress * Math.PI) * 2f, 0, 1);

        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;
            Main.spriteBatch.Reload(BlendState.Additive);


            float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
            if (Projectile.oldPos.Length > 2)
            {
                Texture2D tex2 = Helper.GetExtraTexture("TrailShape_LongSolidWavy");
                List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
                List<Vector2> oldPositions = new List<Vector2>(Projectile.oldPos.Length);
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    if (Projectile.oldPos[i] != Vector2.Zero)
                        oldPositions.Add(Projectile.oldPos[i]);
                    else
                        break;
                }
                float sizeOffset = 0.84f + .2f * MathF.Pow(MathF.Sin(swingProgress * Pi), 1);
                float rotOffset = ToRadians(4 + 35 * MathF.Pow(MathF.Sin(swingProgress * Pi) * 0.7f, 3));
                switch (Projectile.ai[2])
                {
                    case 1:
                        sizeOffset = 0.93f + 0.05f * MathF.Pow(MathF.Sin(swingProgress * Pi), 3);
                        rotOffset = ToRadians(4 + 25 * MathF.Pow(MathF.Sin(swingProgress * Pi) * 0.7f, 3));
                        break;
                    case 2:
                        sizeOffset = 0.75f + .3f * MathF.Pow(MathF.Sin(swingProgress * Pi), 1);
                        rotOffset = ToRadians(4 + 20 * MathF.Pow(MathF.Sin(swingProgress * Pi) * 0.7f, 3));
                        break;
                    case 3:
                        sizeOffset = 0.75f + .3f * MathF.Pow(MathF.Sin(swingProgress * Pi), 1);
                        rotOffset = ToRadians(4 + 15 * MathF.Pow(MathF.Sin(swingProgress * Pi) * 0.7f, 3));
                        break;
                    case 4:
                        sizeOffset = 0.75f + .3f * MathF.Pow(MathF.Sin(swingProgress * Pi), 1);
                        rotOffset = ToRadians(4 + 15 * MathF.Pow(MathF.Sin(swingProgress * Pi) * 0.7f, 3));
                        break;
                }
                if (oldPositions.Count > 3)
                {
                    if (Projectile.ai[1] == -1)
                        for (int i = 0; i < oldPositions.Count - 2; i++)
                        {
                            if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                            {
                                float s = MathHelper.SmoothStep(0, 1f, (float)i / (float)oldPositions.Count);
                                float cS = MathF.Pow(MathHelper.Lerp(1, 0, (float)i / (float)oldPositions.Count) * alpha, 2);
                                float rot2 = Projectile.oldRot[i] - MathHelper.PiOver4;
                                Vector2 end = startP + rot2.ToRotationVector2().RotatedBy(-rotOffset) * ((Projectile.height + holdOffset * 0.5f) * Projectile.scale * sizeOffset);
                                Vector2 start = Vector2.Lerp(startP, end, Lerp(0, 1, MathF.Pow(s, 2)));
                                Color col = Color.Lerp(Color.White * 0.7f, Color.White, (float)i / (float)oldPositions.Count) * cS * Clamp(MathF.Sin(swingProgress * MathHelper.Pi) * 0.5f, 0, 1) * 2 * SmoothStep(1, 0, s) * 5 * cS;

                                float _off = MathF.Abs((s) % 1f);

                                vertices.Add(Helper.AsVertex(start - Main.screenPosition, col, new Vector2(_off, 1)));
                                vertices.Add(Helper.AsVertex(end - Main.screenPosition, col, new Vector2(_off, 0)));

                            }
                        }
                    else
                        for (int i = oldPositions.Count - 2; i > 0; i--)
                        {
                            if (Projectile.oldPos[i] != Vector2.Zero && Projectile.oldPos[i + 1] != Vector2.Zero)
                            {
                                float s = MathHelper.SmoothStep(0, 1f, (float)i / (float)oldPositions.Count);
                                float cS = MathF.Pow(MathHelper.Lerp(1, 0, (float)i / (float)oldPositions.Count) * alpha, 2);
                                float rot2 = Projectile.oldRot[i] - MathHelper.PiOver4;
                                Vector2 end = startP + rot2.ToRotationVector2().RotatedBy(rotOffset) * ((Projectile.height + holdOffset * 0.5f) * Projectile.scale * sizeOffset);
                                Vector2 start = Vector2.Lerp(startP, end, Lerp(0, 1, MathF.Pow(s, 2)));
                                Color col = Color.Lerp(Color.White * 0.7f, Color.White, s) * cS * Clamp(MathF.Sin(swingProgress * MathHelper.Pi) * 0.5f, 0, 1) * 2 * SmoothStep(1, 0, s) * 5 * cS;

                                float _off = MathF.Abs((s) % 1f);

                                vertices.Add(Helper.AsVertex(start - Main.screenPosition, col, new Vector2(_off, 1)));
                                vertices.Add(Helper.AsVertex(end - Main.screenPosition, col, new Vector2(_off, 0)));
                            }
                        }
                }
                if (vertices.Count > 3)
                {
                    for (int i = 0; i < 3; i++)
                        Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex2, false);
                }
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Reload(BlendState.Additive);
            Vector2 orig = texture.Size() / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), Color.White * alpha * 4, Projectile.rotation + (Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
    }
    public class PhantasmalWave : ModProjectile
    {
        public override string Texture => Helper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 40;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.height = 80;
            Projectile.width = 80;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 120;
            Projectile.timeLeft = 80;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.ShadowFlame, 30);
        }
        public override bool? CanDamage()
        {
            return true;
        }
        public override bool ShouldUpdatePosition()
        {
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (lightColor != Color.Transparent) return false;
            Texture2D tex = Helper.GetExtraTexture("Extras2/slash_06");
            Main.spriteBatch.Reload(BlendState.Additive);
            float fadeMult = Helper.Safe(1f / Projectile.oldPos.Length);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float mult = MathF.Pow(1f - fadeMult * i, 2);
                Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.velocity.ToRotation().ToRotationVector2() * (tex.Height / 3 * mult - tex.Height / 3) - Main.screenPosition, null, Color.White * Projectile.ai[2] * mult * increment * 4, Projectile.velocity.ToRotation(), tex.Size() / 2, Projectile.ai[0] * 0.7f * Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        float increment = 0.6f;
        public override void AI()
        {
            if (Projectile.timeLeft % 3 == 0)
            {
                Projectile.velocity *= 0.9f;
                increment *= 0.9f;
            }
            Projectile.scale = Projectile.ai[1];
            //Projectile.Center = Main.player[Projectile.owner].Center + Projectile.velocity * Projectile.ai[1];
            Projectile.ai[0] = Lerp(Projectile.ai[0], 0, 0.005f);
            Projectile.ai[2] = Lerp(Projectile.ai[2], 1, 0.1f);
            float Ease(float x)
            {
                return 1 - MathF.Sqrt(1 - MathF.Pow(x, 2));
            }
        }
    }
}

