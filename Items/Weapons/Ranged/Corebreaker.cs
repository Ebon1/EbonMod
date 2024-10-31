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

namespace EbonianMod.Items.Weapons.Ranged
{
    public class Corebreaker : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.width = 40;
            Item.height = 40;
            Item.reuseDelay = 25;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = 5;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item14;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CorebreakerP>();
            Item.shootSpeed = 14;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.HellstoneBar, 35).AddTile(TileID.MythrilAnvil).Register();
        }
        public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //   for (int i = 0; i < 2; i++)
            //     Helper.DustExplosion(position + velocity, Vector2.One, 2, Color.White, false, false, 0.6f, 0.5f, new(velocity.X / 2, Main.rand.NextFloat(-5, -3)));
            Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<CorebreakerP>(), damage, knockback, player.whoAmI);
            /*for (int i = -1; i < 2; i++)
            {
                if (i == 0)
                    continue;
                Projectile a = Projectile.NewProjectileDirect(Item.InheritSource(Item), position, velocity + new Vector2(0, 5 * i).RotatedBy(velocity.ToRotation()), ModContent.ProjectileType<EFireBreath2>(), damage, knockback, player.whoAmI);
                a.friendly = true;
                a.hostile = false;
                a.localAI[0] = 600;
                a.scale = 1;

            }*/
            Projectile a = Projectile.NewProjectileDirect(Terraria.Entity.InheritSource(Item), position, velocity, ModContent.ProjectileType<EFireBreath2>(), damage / 3, knockback, player.whoAmI);
            a.friendly = true;
            a.hostile = false;
            a.localAI[0] = 400;
            a.localAI[1] = 2;
            a.scale = 1;
            return false;
        }
    }
    public class EFireBreath2 : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DD2BetsyFlameBreath;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetRect)
        {
            float collisionPoint17 = 0f;
            float num22 = Projectile.ai[0] / 25f;
            if (num22 > 1f)
            {
                num22 = 1f;
            }
            float num23 = (Projectile.ai[0] - 38f) / 40f;
            if (num23 < 0f)
            {
                num23 = 0f;
            }
            Vector2 lineStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num23;
            Vector2 lineEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num22;
            if (Collision.CheckAABBvLineCollision(targetRect.TopLeft(), targetRect.Size(), lineStart, lineEnd, 40f * Projectile.scale, ref collisionPoint17))
            {
                return true;
            }
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile proj = Projectile;
            Vector2 center2 = proj.Center;
            center2 -= Main.screenPosition;
            float num215 = 40f;
            float num216 = num215 * 2f;
            float num217 = (float)proj.frameCounter / num215;
            Texture2D value40 = TextureAssets.Projectile[proj.type].Value;
            Microsoft.Xna.Framework.Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Microsoft.Xna.Framework.Color color60 = new Microsoft.Xna.Framework.Color(255, 255, 255, 0);
            Microsoft.Xna.Framework.Color color61 = new Microsoft.Xna.Framework.Color(180, 30, 30, 200);
            Microsoft.Xna.Framework.Color color62 = new Microsoft.Xna.Framework.Color(0, 0, 0, 30);
            ulong seed = 1uL;
            for (float num218 = 0f; num218 < 15f; num218 += 1f)
            {
                float num219 = Utils.RandomFloat(ref seed) * 0.25f - 0.125f;
                Vector2 vector49 = (proj.rotation + (Projectile.scale < 1 ? 0 : num219)).ToRotationVector2();
                Vector2 value41 = center2 + vector49 * Projectile.localAI[0];
                float num220 = num217 + num218 * (1f / 15f);
                int num221 = (int)(num220 / (1f / 15f));
                num220 %= 1f;
                if ((!(num220 > num217 % 1f) || !((float)proj.frameCounter < num215)) && (!(num220 < num217 % 1f) || !((float)proj.frameCounter >= num216 - num215)))
                {
                    transparent = ((num220 < 0.1f) ? Microsoft.Xna.Framework.Color.Lerp(Microsoft.Xna.Framework.Color.Transparent, color60, Utils.GetLerpValue(0f, 0.1f, num220, clamped: true)) : ((num220 < 0.35f) ? color60 : ((num220 < 0.7f) ? Microsoft.Xna.Framework.Color.Lerp(color60, color61, Utils.GetLerpValue(0.35f, 0.7f, num220, clamped: true)) : ((num220 < 0.9f) ? Microsoft.Xna.Framework.Color.Lerp(color61, color62, Utils.GetLerpValue(0.7f, 0.9f, num220, clamped: true)) : ((!(num220 < 1f)) ? Microsoft.Xna.Framework.Color.Transparent : Microsoft.Xna.Framework.Color.Lerp(color62, Microsoft.Xna.Framework.Color.Transparent, Utils.GetLerpValue(0.9f, 1f, num220, clamped: true)))))));
                    float num222 = 0.9f + num220 * 0.8f;
                    num222 *= num222;
                    num222 *= 0.8f;
                    Vector2 position7 = Vector2.SmoothStep(center2, value41, num220);
                    Microsoft.Xna.Framework.Rectangle rectangle8 = value40.Frame(1, 7, 0, (int)(num220 * 7f));
                    Main.EntitySpriteDraw(value40, position7, rectangle8, transparent, proj.rotation + (float)Math.PI * 2f * (num220 + Main.GlobalTimeWrappedHourly * 1.2f) * 0.2f + (float)num221 * ((float)Math.PI * 2f / 5f), rectangle8.Size() / 2f, num222 * Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = 1;
            if (Projectile.localAI[0] == 0)
                Projectile.localAI[0] = 400f;
            if (Projectile.ai[1] == 0)
                Projectile.ai[1] = 1;
        }
        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center;
            if (Projectile.ai[1] != 0)
                Projectile.scale = Projectile.ai[1];
            //NPC nPC = Main.npc[(int)Projectile.ai[1]];
            float num = -8f;
            //Vector2 vector2 = (Projectile.Center = nPC.Center + new Vector2((110f + num) * (float)nPC.spriteDirection, 30f).RotatedBy(nPC.rotation));
            Projectile.rotation = Projectile.velocity.ToRotation();
            DelegateMethods.v3_1 = new Vector3(1.2f, 1f, 0.3f);
            float num2 = Projectile.ai[0] / 40f;
            if (num2 > 1f)
            {
                num2 = 1f;
            }
            float num3 = (Projectile.ai[0] - 38f) / 40f;
            if (num3 < 0f)
            {
                num3 = 0f;
            }
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583) * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(0.19634954631328583) * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            Utils.PlotTileLine(Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583) * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-0.19634954631328583) * Projectile.localAI[0] * num2, 16f, DelegateMethods.CastLight);
            if (num3 == 0f && num2 > 0.1f && Projectile.ai[0] > 300)
            {
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 6);
                    dust.fadeIn = 1.5f;
                    dust.velocity = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI / 12f)) * (0.5f + Main.rand.NextFloat() * 2.5f) * 15f * Projectile.scale;
                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.alpha = 200;
                }
            }
            if (Main.rand.Next(5) == 0 && Projectile.ai[0] >= 15f)
            {
                Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 300f - Utils.RandomVector2(Main.rand, -20f, 20f), Vector2.Zero, 61 + Main.rand.Next(3), 0.5f);
                gore.velocity *= 0.3f;
                gore.velocity += Projectile.rotation.ToRotationVector2() * 4f;
            }
            /*for (int j = 0; j < 1; j++)
            {
                Dust dust2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 31);
                dust2.fadeIn = 1.5f;
                dust2.scale = 0.4f;
                dust2.velocity = Projectile.rotation.ToRotationVector2().RotatedBy(Main.rand.NextFloatDirection() * ((float)Math.PI / 12f)) * (0.5f + Main.rand.NextFloat() * 2.5f) * 15f;
                dust2.velocity += nPC.velocity * 2f;
                dust2.velocity *= 0.3f;
                dust2.noLight = true;
                dust2.noGravity = true;
                float num4 = Main.rand.NextFloat();
                dust2.position = Vector2.Lerp(Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num3, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0] * num2, num4);
                dust2.position += Projectile.rotation.ToRotationVector2().RotatedBy(1.5707963705062866) * (20f + 100f * (num4 - 0.5f));
            }*/
            Projectile.frameCounter += (int)Projectile.localAI[1];
            Projectile.ai[0] += Projectile.localAI[1];
            if (Projectile.ai[0] >= 78f)
            {
                Projectile.Kill();
            }
        }
    }
}
